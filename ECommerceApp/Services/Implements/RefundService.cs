using ECommerceApp.Commons;
using ECommerceApp.DTOs.RefundDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Enums;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class RefundService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<RefundService> logger) : IRefundService
    {
        public async Task<ApiResponse<PagedResult<PendingRefundResponse>>> GetEligibleRefundsAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var eligibleCancellations = await unitOfWork.CancellationRepository.GetApprovedCancellationsPendingRefundAsync(trackChanges: false);

                var pendingRefunds = eligibleCancellations.Select(c => new PendingRefundResponse
                {
                    CancellationId = c.Id,
                    OrderId = c.OrderId,
                    OrderAmount = c.OrderAmount,
                    CancellationCharge = c.CancellationCharges ?? 0.00m,
                    ComputedRefundAmount = c.OrderAmount - (c.CancellationCharges ?? 0.00m),
                    CancellationRemarks = c.Remarks
                }).ToList();

                return new ApiResponse<PagedResult<PendingRefundResponse>>(200, pendingRefunds.ToPagedResult(paginationRequest));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in RefundService.");
                return new ApiResponse<PagedResult<PendingRefundResponse>>(500, $"An unexpected error occurred while retrieving eligible refunds: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<RefundResponse>> ProcessRefundAsync(int cancellationId, int processedBy, RefundRequest refundRequest)
        {
            try
            {
                var cancellation = await unitOfWork.CancellationRepository.GetByIdWithFullDetailsAsync(
                    cancellationId, 
                    trackChanges: true);

                if (cancellation == null)
                    return new ApiResponse<RefundResponse>(404, "Cancellation request not found.");

                if (cancellation.Status != CancellationStatus.Approved)
                    return new ApiResponse<RefundResponse>(400, "Only approved cancellations are eligible for refunds.");

                var existingRefund = await unitOfWork.RefundRepository.GetByCancellationIdAsync(
                    cancellationId, 
                    trackChanges: false);

                if (existingRefund != null)
                    return new ApiResponse<RefundResponse>(400, "Refund for this cancellation request has already been initiated.");

                var payment = cancellation.Order.Payment;
                if (payment == null || payment.PaymentMethod.ToLower() == "cod")
                    return new ApiResponse<RefundResponse>(400, "No payment associated with the order or payment method is COD.");

                decimal computedRefundAmount = cancellation.OrderAmount - (cancellation.CancellationCharges ?? 0.00m);
                if (computedRefundAmount <= 0)
                    return new ApiResponse<RefundResponse>(400, "Computed refund amount is not valid.");

                var refund = new Refund
                {
                    CancellationId = cancellationId,
                    PaymentId = payment.Id,
                    Amount = computedRefundAmount,
                    RefundMethod = refundRequest.RefundMethod.ToString(),
                    RefundReason = refundRequest.RefundReason,
                    Status = RefundStatus.Pending,
                    InitiatedAt = DateTime.UtcNow,
                    ProcessedBy = processedBy
                };

                unitOfWork.RefundRepository.Add(refund);
                
                await unitOfWork.SaveChangesAsync();

                var gatewayResponse = await ProcessRefundPaymentAsync(refund);
                
                if (gatewayResponse.IsSuccess)
                {
                    refund.Status = RefundStatus.Completed;
                    refund.TransactionId = gatewayResponse.TransactionId;
                    refund.CompletedAt = DateTime.UtcNow;

                    payment.Status = PaymentStatus.Refunded;

                    if (cancellation.Order.Customer != null && !string.IsNullOrEmpty(cancellation.Order.Customer.Email))
                    {
                        await emailService.SendEmailAsync(
                            cancellation.Order.Customer.Email,
                            $"Your Refund Has Been Processed Successfully, Order #{cancellation.Order.OrderNumber}",
                            GenerateRefundSuccessEmailBody(refund, cancellation.Order.OrderNumber, cancellation),
                            isBodyHtml: true);
                    }
                }
                else
                {
                    refund.Status = RefundStatus.Failed;
                }

                await unitOfWork.SaveChangesAsync();

                return new ApiResponse<RefundResponse>(200, MapRefundToDTO(refund));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in RefundService.");
                return new ApiResponse<RefundResponse>(500, $"An unexpected error occurred while processing the refund: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<ConfirmationResponse>> UpdateRefundStatusAsync(int refundId, int processedBy, RefundStatusUpdateRequest statusUpdate)
        {
            try
            {
                var refund = await unitOfWork.RefundRepository.GetByIdWithFullDetailsAsync(refundId, trackChanges: true);

                if (refund == null)
                    return new ApiResponse<ConfirmationResponse>(404, "Refund not found.");

                if (refund.Status != RefundStatus.Pending && refund.Status != RefundStatus.Failed)
                    return new ApiResponse<ConfirmationResponse>(400, "Only pending or failed refunds can be updated.");

                refund.RefundMethod = statusUpdate.RefundMethod.ToString();
                refund.Status = RefundStatus.Completed;
                refund.TransactionId = statusUpdate.TransactionId;
                refund.CompletedAt = DateTime.UtcNow;
                refund.ProcessedBy = processedBy;
                refund.RefundReason = statusUpdate.RefundReason;

                if (refund.Payment != null)
                {
                    refund.Payment.Status = PaymentStatus.Refunded;
                }

                await unitOfWork.SaveChangesAsync();

                if (refund.Cancellation?.Order?.Customer != null && !string.IsNullOrEmpty(refund.Cancellation.Order.Customer.Email))
                {
                    await emailService.SendEmailAsync(
                        refund.Cancellation.Order.Customer.Email,
                        $"Your Refund Has Been Processed Successfully, Order #{refund.Cancellation.Order.OrderNumber}",
                        GenerateRefundSuccessEmailBody(refund, refund.Cancellation.Order.OrderNumber, refund.Cancellation),
                        isBodyHtml: true);
                }

                var confirmation = new ConfirmationResponse
                {
                    Message = $"Refund with ID {refund.Id} has been updated to {refund.Status}."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in RefundService.");
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while updating the refund status: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<RefundResponse>> GetRefundByIdAsync(int id)
        {
            try
            {
                var refund = await unitOfWork.RefundRepository.GetByIdWithDetailsAsync(id, trackChanges: false);

                if (refund == null)
                    return new ApiResponse<RefundResponse>(404, "Refund not found.");

                return new ApiResponse<RefundResponse>(200, MapRefundToDTO(refund));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in RefundService.");
                return new ApiResponse<RefundResponse>(500, $"An unexpected error occurred while retrieving the refund: {ex.Message}");
            }
        }
        
        public async Task<ApiResponse<PagedResult<RefundResponse>>> GetAllRefundsAsync(PaginationRequest paginationRequest)
        {
            try
            {
                var refunds = await unitOfWork.RefundRepository.GetAllWithDetailsAsync(trackChanges: false);

                var refundList = refunds.Select(r => MapRefundToDTO(r)).ToList();

                return new ApiResponse<PagedResult<RefundResponse>>(200, refundList.ToPagedResult(paginationRequest));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in RefundService.");
                return new ApiResponse<PagedResult<RefundResponse>>(500, $"An unexpected error occurred while retrieving all refunds: {ex.Message}");
            }
        }

        // Private Helper Methods
        private RefundResponse MapRefundToDTO(Refund refund)
        {
            return new RefundResponse
            {
                Id = refund.Id,
                CancellationId = refund.CancellationId,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                RefundMethod = Enum.Parse<RefundMethod>(refund.RefundMethod),
                RefundReason = refund.RefundReason,
                Status = refund.Status,
                InitiatedAt = refund.InitiatedAt,
                CompletedAt = refund.CompletedAt,
                TransactionId = refund.TransactionId
            };
        }

        // Simulates calling a payment gateway to process the refund.
        // In production, replace this with actual integration code.
        public async Task<PaymentGatewayRefundResponse> ProcessRefundPaymentAsync(Refund refund)
        {
            // Simulate a network delay of 1 second.
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Create a Random instance. (In production, consider reusing a static instance.)
            var random = new Random();
            double chance = random.NextDouble(); // Generates a double between 0.0 and 1.0.

            if (chance < 0.70) // 70% chance for Completed.
            {
                return new PaymentGatewayRefundResponse
                {
                    IsSuccess = true,
                    Status = RefundStatus.Completed,
                    TransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                };
            }
            else if (chance < 0.90) // Next 20% chance for Failed.
            {
                return new PaymentGatewayRefundResponse
                {
                    IsSuccess = false,
                    Status = RefundStatus.Failed
                };
            }
            else // Remaining 10% chance for Pending.
            {
                return new PaymentGatewayRefundResponse
                {
                    IsSuccess = false,
                    Status = RefundStatus.Pending
                };
            }
        }

        // Generates an HTML email body (with inline CSS) to notify the customer.
        public string GenerateRefundSuccessEmailBody(Refund refund, string orderNumber, Cancellation cancellation)
        {
            // Format CompletedAt if available; otherwise show "N/A".

            // Define the IST time zone.
            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            // Convert CompletedAt from UTC to IST, if available.
            string completedAtStr = refund.CompletedAt.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(refund.CompletedAt.Value, istZone).ToString("dd MMM yyyy HH:mm:ss")
                : "N/A";

            return $@"
            <html>
            <body style='font-family: Arial, sans-serif; margin: 0; padding: 0;'>
                <div style='background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #ddd;'>
                        <div style='padding: 20px; text-align: center; background-color: #2E86C1; color: #ffffff;'>
                            <h2>Your Refund is Complete</h2>
                        </div>
                        <div style='padding: 20px;'>
                            <p>Dear Customer,</p>
                            <p>Your refund has been processed successfully. Below are the details:</p>
                            <table style='width: 100%; border-collapse: collapse;'>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Order Number</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{orderNumber}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Refund Transaction ID</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{refund.TransactionId}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Order Amount</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>₹{cancellation.OrderAmount}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Cancellation Charges</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>₹{cancellation.CancellationCharges ?? 0.00m}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Cancellation Reason</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{cancellation.Reason}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Refunded Method</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{refund.RefundMethod}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Refunded Amount</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>₹{refund.Amount}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>CompletedAt At</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{completedAtStr}</td>
                                </tr>
                            </table>
                            <p>Thank you for shopping with us.</p>
                            <p>Best regards,<br/>The ECommerce Team</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}
