using ECommerceApp.Commons;
using ECommerceApp.DTOs.PaymentDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Enums;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class PaymentService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<PaymentService> logger) : IPaymentService
    {
        public async Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(int orderId, int currentCustomerId, bool isAdmin, PaymentRequest paymentRequest)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var order = isAdmin
                    ? await unitOfWork.OrderRepository.GetOrderWithFullDetailsAsync(orderId, trackChanges: true)
                    : await unitOfWork.OrderRepository.GetByIdAndCustomerIdWithPaymentAsync(
                        orderId, 
                        currentCustomerId, 
                        trackChanges: true);

                if (order == null)
                {
                    return new ApiResponse<PaymentResponse>(404, "Order not found.");
                }

                if (Math.Round(paymentRequest.Amount, 2) != Math.Round(order.TotalAmount, 2))
                {
                    return new ApiResponse<PaymentResponse>(400, "Payment amount does not match the order total.");
                }

                Payment payment;

                if (order.Payment != null)
                {
                    if (order.Payment.Status == PaymentStatus.Failed && order.OrderStatus == OrderStatus.Pending)
                    {
                        payment = order.Payment;

                        payment.PaymentMethod = paymentRequest.PaymentMethod;
                        payment.Amount = paymentRequest.Amount;
                        payment.PaymentDate = DateTime.UtcNow;
                        payment.Status = PaymentStatus.Pending;
                        payment.TransactionId = null; 
                    }
                    else
                    {
                        return new ApiResponse<PaymentResponse>(400, "Order already has an associated payment in progress or completed.");
                    }
                }
                else
                {
                    payment = new Payment
                    {
                        OrderId = orderId,
                        PaymentMethod = paymentRequest.PaymentMethod,
                        Amount = paymentRequest.Amount,
                        PaymentDate = DateTime.UtcNow,
                        Status = PaymentStatus.Pending
                    };

                    unitOfWork.PaymentRepository.Add(payment);
                }

                if (!IsCashOnDelivery(paymentRequest.PaymentMethod))
                {
                    var simulatedStatus = await SimulatePaymentGateway();
                    payment.Status = simulatedStatus;

                    if (simulatedStatus == PaymentStatus.Completed)
                    {
                        payment.TransactionId = GenerateTransactionId();
                        order.OrderStatus = OrderStatus.Processing;
                    }
                    else 
                    {
                    }
                }
                else
                {
                    order.OrderStatus = OrderStatus.Processing;
                }

                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();

                if (order.OrderStatus == OrderStatus.Processing)
                {
                    await SendOrderConfirmationEmailAsync(orderId);
                }

                var paymentResponse = new PaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    CustomerId = order.CustomerId,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    Status = payment.Status
                };

                return new ApiResponse<PaymentResponse>(200, paymentResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in PaymentService.");
                await unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<PaymentResponse>(500, $"An unexpected error occurred while processing the payment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PaymentResponse>> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment = await unitOfWork.PaymentRepository.GetByIdAsync(paymentId);

                if (payment == null)
                {
                    return new ApiResponse<PaymentResponse>(404, "Payment not found.");
                }

                var paymentResponse = new PaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    CustomerId = payment.Order.CustomerId,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    Status = payment.Status
                };

                return new ApiResponse<PaymentResponse>(200, paymentResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in PaymentService.");
                return new ApiResponse<PaymentResponse>(500, $"An unexpected error occurred while retrieving the payment: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<PaymentResponse>> GetPaymentByOrderIdAsync(int orderId)
        {
            try
            {
                var payment = await unitOfWork.PaymentRepository.GetByOrderIdAsync(orderId);

                if (payment == null)
                {
                    return new ApiResponse<PaymentResponse>(404, "Payment not found for this order.");
                }

                var paymentResponse = new PaymentResponse
                {
                    PaymentId = payment.Id,
                    OrderId = payment.OrderId,
                    CustomerId = payment.Order.CustomerId,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionId = payment.TransactionId,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    Status = payment.Status
                };

                return new ApiResponse<PaymentResponse>(200, paymentResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in PaymentService.");
                return new ApiResponse<PaymentResponse>(500, $"An unexpected error occurred while retrieving the payment: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdatePaymentStatusAsync(int paymentId, PaymentStatusUpdateRequest statusUpdate)
        {
            try
            {
                var payment = await unitOfWork.PaymentRepository.GetByIdWithOrderAsync(paymentId, trackChanges: true);

                if (payment == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Payment not found.");
                }
        
                payment.Status = statusUpdate.Status;

                if (statusUpdate.Status == PaymentStatus.Completed && !IsCashOnDelivery(payment.PaymentMethod))
                {
                    payment.TransactionId = statusUpdate.TransactionId;
                    payment.Order.OrderStatus = OrderStatus.Processing; 
                }

                await unitOfWork.SaveChangesAsync();

                if (payment.Order.OrderStatus == OrderStatus.Processing)
                {
                    await SendOrderConfirmationEmailAsync(payment.Order.Id);
                }

                var confirmation = new ConfirmationResponse
                {
                    Message = $"Payment with ID {payment.Id} updated to status '{payment.Status}'."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in PaymentService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while updating the payment status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> CompleteCodPaymentAsync(int paymentId)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var payment = await unitOfWork.PaymentRepository.GetByIdWithOrderAsync(paymentId, trackChanges: true);

                if (payment == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Payment not found.");
                }

                if (payment.Order == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "No Order associated with this Payment.");
                }

                if (payment.Order.OrderStatus != OrderStatus.Shipped)
                {
                    return new ApiResponse<ConfirmationResponse>(400, $"Order cannot be marked as Delivered from '{payment.Order.OrderStatus}' State.");
                }

                if (!IsCashOnDelivery(payment.PaymentMethod))
                {
                    return new ApiResponse<ConfirmationResponse>(409, "Payment method is not Cash on Delivery.");
                }

                payment.Status = PaymentStatus.Completed;
                payment.Order.OrderStatus = OrderStatus.Delivered;

                await unitOfWork.SaveChangesAsync();
                
                await unitOfWork.CommitTransactionAsync();

                var confirmation = new ConfirmationResponse
                {
                    Message = $"COD Payment for Order ID {payment.Order.Id} has been marked as 'Completed' and the order status updated to 'Delivered'."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in PaymentService.");
                await unitOfWork.RollbackTransactionAsync();
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while completing the COD payment: {ex.Message}");
            }
        }

        #region Helper Methods

        // Simulate a payment gateway response using Random.Shared for performance
        private async Task<PaymentStatus> SimulatePaymentGateway()
        {
            //Simulate the PG
            await Task.Delay(TimeSpan.FromMilliseconds(1));

            int chance = Random.Shared.Next(1, 101); // 1 to 100

            if (chance <= 60)
                return PaymentStatus.Completed;
            else if (chance <= 90)
                return PaymentStatus.Pending;
            else
                return PaymentStatus.Failed;
        }

        // Generate a unique 12-character transaction ID
        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12)}";
        }

        // Determines if the provided payment method indicates Cash on Delivery
        private bool IsCashOnDelivery(string paymentMethod)
        {
            return paymentMethod.Equals("CashOnDelivery", StringComparison.OrdinalIgnoreCase) ||
                   paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase);
        }

        public async Task SendOrderConfirmationEmailAsync(int orderId)
        {
            var order = await unitOfWork.OrderRepository.GetOrderWithFullDetailsAsync(orderId);

            if (order == null)
            {
                return;
            }

            var payment = order.Payment;

            string subject = $"Order Confirmation - {order.OrderNumber}";

            string emailBody = $@"
            <html>
              <head>
                <meta charset='UTF-8'>
              </head>
              <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px;'>
                <div style='max-width: 700px; margin: auto; background-color: #ffffff; padding: 20px; border: 1px solid #dddddd;'>
                  <!-- Header -->
                  <div style='background-color: #007bff; padding: 15px; text-align: center; color: #ffffff;'>
                    <h2 style='margin: 0;'>Order Confirmation</h2>
                  </div>
          
                  <!-- Greeting and Order Details -->
                  <p style='margin: 20px 0 5px 0;'>Dear {order.Customer?.FirstName} {order.Customer?.LastName},</p>
                  <p style='margin: 5px 0 20px 0;'>Thank you for your order. Please find your invoice below.</p>
                  <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Order Number:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{order.OrderNumber}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Order Date:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{order.OrderDate:MMMM dd, yyyy}</td>
                    </tr>
                  </table>
          
                  <!-- Order Summary -->
                  <h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Order Summary</h3>
                  <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Sub Total:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{order.TotalBaseAmount:C}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Discount:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>-{order.TotalDiscountAmount:C}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Shipping Cost:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{order.ShippingCost:C}</td>
                    </tr>
                    <tr style='font-weight: bold;'>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Total Amount:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{order.TotalAmount:C}</td>
                    </tr>
                  </table>
          
                  <!-- Order Items -->
                  <h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Order Items</h3>
                  <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                    <tr style='background-color: #28a745; color: #ffffff;'>
                      <th style='padding: 8px; border: 1px solid #dddddd;'>Product</th>
                      <th style='padding: 8px; border: 1px solid #dddddd;'>Quantity</th>
                      <th style='padding: 8px; border: 1px solid #dddddd;'>Unit Price</th>
                      <th style='padding: 8px; border: 1px solid #dddddd;'>Discount</th>
                      <th style='padding: 8px; border: 1px solid #dddddd;'>Total Price</th>
                    </tr>
                    {string.Join("", order.OrderItems.Select(item => $@"
                    <tr>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{item.Product?.Name}</td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{item.Quantity}</td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{item.UnitPrice:C}</td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{item.Discount:C}</td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{item.TotalPrice:C}</td>
                    </tr>
                    "))}
                  </table>
          
                  <!-- Addresses: Combined Billing and Shipping -->
                  <h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Addresses</h3>
                  <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                    <tr>
                      <td style='width: 50%; vertical-align: top; padding: 8px; border: 1px solid #dddddd;'>
                        <strong>Billing Address</strong><br/>
                        {order.BillingAddress?.AddressLine1}<br/>
                        {(string.IsNullOrWhiteSpace(order.BillingAddress?.AddressLine2) ? "" : order.BillingAddress?.AddressLine2 + "<br/>")}
                        {order.BillingAddress?.City}, {order.BillingAddress?.State} {order.BillingAddress?.PostalCode}<br/>
                        {order.BillingAddress?.Country}
                      </td>
                      <td style='width: 50%; vertical-align: top; padding: 8px; border: 1px solid #dddddd;'>
                        <strong>Shipping Address</strong><br/>
                        {order.ShippingAddress?.AddressLine1}<br/>
                        {(string.IsNullOrWhiteSpace(order.ShippingAddress?.AddressLine2) ? "" : order.ShippingAddress?.AddressLine2 + "<br/>")}
                        {order.ShippingAddress?.City}, {order.ShippingAddress?.State} {order.ShippingAddress?.PostalCode}<br/>
                        {order.ShippingAddress?.Country}
                      </td>
                    </tr>
                  </table>
          
                  <!-- Payment Details -->
                  <h3 style='color: #007bff; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Payment Details</h3>
                  <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Payment Method:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.PaymentMethod : "N/A")}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Payment Date:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.PaymentDate.ToString("MMMM dd, yyyy HH:mm") : "N/A")}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Transaction ID:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.TransactionId : "N/A")}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Status:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{(payment != null ? payment.Status.ToString() : "N/A")}</td>
                    </tr>
                  </table>
          
                  <p style='margin-top: 20px;'>If you have any questions, please contact our support team.</p>
                  <p>Best regards,<br/>Your E-Commerce Store Team</p>
                </div>
              </body>
            </html>";

            if (order.Customer != null && !string.IsNullOrWhiteSpace(order.Customer.Email))
            {
                await emailService.SendEmailAsync(order.Customer.Email, subject, emailBody, isBodyHtml: true);
            }
        }

        #endregion
    }
}
