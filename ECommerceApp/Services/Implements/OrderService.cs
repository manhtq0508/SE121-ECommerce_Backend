using System.Security.Cryptography;
using ECommerceApp.Commons;
using ECommerceApp.DTOs.OrderDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Enums;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger) : IOrderService
    {
        private static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedStatusTransitions = new()
        {
            { OrderStatus.Pending, [OrderStatus.Processing, OrderStatus.Canceled] },
            { OrderStatus.Processing, [OrderStatus.Shipped, OrderStatus.Canceled] },
            { OrderStatus.Shipped, [OrderStatus.Delivered] },
            { OrderStatus.Delivered, [] },
            { OrderStatus.Canceled, [] }
        };

        public async Task<ApiResponse<OrderResponse>> CreateOrderAsync(OrderCreateRequest orderDto)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var customer = await unitOfWork.CustomerRepository.GetActiveByIdAsync(orderDto.CustomerId);
                if (customer == null)
                    return new ApiResponse<OrderResponse>(404, "Customer does not exist.");

                var billingAddress = await unitOfWork.AddressRepository.GetByIdAsync(orderDto.BillingAddressId);
                if (billingAddress == null || billingAddress.CustomerId != orderDto.CustomerId)
                    return new ApiResponse<OrderResponse>(400, "Billing Address is invalid or does not belong to the customer.");

                var shippingAddress = await unitOfWork.AddressRepository.GetByIdAndCustomerIdAsync(orderDto.ShippingAddressId, orderDto.CustomerId);
                if (shippingAddress == null)
                    return new ApiResponse<OrderResponse>(400, "Shipping Address is invalid or does not belong to the customer.");

                var productIds = orderDto.OrderItems.Select(x => x.ProductId).Distinct().ToList();
                
                var products = await unitOfWork.ProductRepository.GetByIdsAsync(productIds, trackChanges: false);
                var productDict = products.ToDictionary(p => p.Id);

                decimal totalBaseAmount = 0;
                decimal totalDiscountAmount = 0;
                decimal shippingCost = 10.00m;
                var orderItems = new List<OrderItem>();

                foreach (var itemDto in orderDto.OrderItems)
                {
                    if (!productDict.TryGetValue(itemDto.ProductId, out var product))
                    {
                        return new ApiResponse<OrderResponse>(404, $"Product with ID {itemDto.ProductId} does not exist.");
                    }

                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        return new ApiResponse<OrderResponse>(400, $"Only {product.StockQuantity} units of '{product.Name}' are available in stock.");
                    }

                    bool isDeducted = await unitOfWork.ProductRepository.DeductStockAsync(itemDto.ProductId, itemDto.Quantity);
                    if (!isDeducted)
                    {
                        await unitOfWork.RollbackTransactionAsync(); 
                        return new ApiResponse<OrderResponse>(409, $"Unfortunately, product '{product.Name}' was just purchased by another customer. Please try again.");
                    }

                    decimal basePrice = itemDto.Quantity * product.Price;
                    decimal discount = (product.DiscountPercentage / 100.0m) * basePrice;
                    
                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.Price,
                        Discount = discount,
                        TotalPrice = basePrice - discount
                    });

                    totalBaseAmount += basePrice;
                    totalDiscountAmount += discount;
                }

                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    BillingAddressId = orderDto.BillingAddressId,
                    ShippingAddressId = orderDto.ShippingAddressId,
                    TotalBaseAmount = totalBaseAmount,
                    TotalDiscountAmount = totalDiscountAmount,
                    ShippingCost = shippingCost,
                    TotalAmount = totalBaseAmount - totalDiscountAmount + shippingCost,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = orderItems
                };
                
                unitOfWork.OrderRepository.Add(order);

                var cart = await unitOfWork.CartRepository.GetActiveCartByCustomerIdAsync(orderDto.CustomerId, trackChanges: true);
                if (cart != null)
                {
                    cart.IsCheckedOut = true;
                    cart.UpdatedAt = DateTime.UtcNow;
                }

                await unitOfWork.SaveChangesAsync();

                await unitOfWork.CommitTransactionAsync();

                var orderResponse = MapOrderToDto(order, customer, billingAddress, shippingAddress);
                return new ApiResponse<OrderResponse>(200, orderResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in OrderService.");
                await unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<OrderResponse>(500, $"An unexpected error occurred during checkout: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<OrderResponse>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    return new ApiResponse<OrderResponse>(404, "Order not found.");
                }

                var orderResponse = MapOrderToDto(order, order.Customer!, order.BillingAddress!, order.ShippingAddress!);
        
                return new ApiResponse<OrderResponse>(200, orderResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in OrderService.");
                return new ApiResponse<OrderResponse>(500, $"An unexpected error occurred while processing your request: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<ConfirmationResponse>> UpdateOrderStatusAsync(OrderStatusUpdateRequest statusDto)
        {
            try
            {
                var order = await unitOfWork.OrderRepository.GetByIdAsync(statusDto.OrderId, trackChanges: true);
                
                if (order == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Order not found.");
                }

                var currentStatus = order.OrderStatus;
                var newStatus = statusDto.OrderStatus;

                if (currentStatus == newStatus)
                {
                    return new ApiResponse<ConfirmationResponse>(200, new ConfirmationResponse 
                    { 
                        Message = $"Order status is already {newStatus}." 
                    });
                }

                if (!AllowedStatusTransitions.TryGetValue(currentStatus, out var allowedStatuses))
                {
                    return new ApiResponse<ConfirmationResponse>(500, "Current order status is invalid.");
                }
                
                if (!allowedStatuses.Contains(newStatus))
                {
                    return new ApiResponse<ConfirmationResponse>(400, $"Cannot change order status from {currentStatus} to {newStatus}.");
                }

                order.OrderStatus = newStatus;
                
                await unitOfWork.SaveChangesAsync();

                var confirmation = new ConfirmationResponse
                {
                    Message = $"Order status for ID {statusDto.OrderId} was updated successfully to {newStatus}."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in OrderService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<PagedResult<OrderResponse>>> GetAllOrdersAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var orders = await unitOfWork.OrderRepository.GetAllWithDetailsAsync();

                var orderList = orders.Select(o => MapOrderToDto(o, o.Customer!, o.BillingAddress!, o.ShippingAddress!)).ToList();
        
                return new ApiResponse<PagedResult<OrderResponse>>(200, orderList.ToPagedResult(paginationRequest));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in OrderService.");
                return new ApiResponse<PagedResult<OrderResponse>>(500, $"An unexpected error occurred while processing your request: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<PagedResult<OrderResponse>>> GetOrdersByCustomerAsync(int customerId, PaginationRequest paginationRequest)
        {
            try
            {
                var customerExists = await unitOfWork.CustomerRepository.GetActiveByIdAsync(customerId);
                if (customerExists == null)
                {
                    return new ApiResponse<PagedResult<OrderResponse>>(404, "Customer not found.");
                }

                var orders = await unitOfWork.OrderRepository.GetByCustomerIdWithDetailsAsync(customerId);

                var orderList = orders.Select(o => MapOrderToDto(o, o.Customer!, o.BillingAddress!, o.ShippingAddress!)).ToList();
        
                return new ApiResponse<PagedResult<OrderResponse>>(200, orderList.ToPagedResult(paginationRequest));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in OrderService.");
                return new ApiResponse<PagedResult<OrderResponse>>(500, $"An unexpected error occurred while processing your request: {ex.Message}");
            }
        }

        #region Helper Methods

        // Maps an Order entity to an OrderResponse.
        private OrderResponse MapOrderToDto(Order order, Customer customer, Address billingAddress, Address shippingAddress)
        {
            // Map order items.
            var orderItemsDto = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Discount = oi.Discount,
                TotalPrice = oi.TotalPrice
            }).ToList();

            // Create and return the DTO.
            return new OrderResponse
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                BillingAddressId = order.BillingAddressId,
                ShippingAddressId = order.ShippingAddressId,
                TotalBaseAmount = order.TotalBaseAmount,
                TotalDiscountAmount = order.TotalDiscountAmount,
                ShippingCost = order.ShippingCost,
                TotalAmount = Math.Round(order.TotalAmount, 2),
                OrderStatus = order.OrderStatus,
                OrderItems = orderItemsDto
            };
        }

        // Generates a unique order number using the current UTC date/time and a random number.
        // Format: ORD-yyyyMMdd-HHmmss-XXXX
        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{RandomNumber(1000, 9999)}";
        }

        // Generates a random number between min and max.
        private int RandomNumber(int min, int max)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            return Math.Abs(BitConverter.ToInt32(bytes, 0) % (max - min + 1)) + min;
        }

        #endregion
    }
}
