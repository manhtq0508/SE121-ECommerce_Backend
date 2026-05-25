using Microsoft.EntityFrameworkCore;
using ECommerceApp.Data;
using ECommerceApp.Enums;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements
{
    public class RefundProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RefundProcessingBackgroundService> logger) : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Refund processing background service started with interval {Interval}.", _interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingAndFailedRefundsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error in RefundProcessingBackgroundService.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            logger.LogInformation("Refund processing background service stopped.");
        }

        private async Task ProcessPendingAndFailedRefundsAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to use scoped services.
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var refundService = scope.ServiceProvider.GetRequiredService<IRefundService>();

                // Query refunds with status Pending or Failed.
                var refunds = await context.Refunds
                    .Include(r => r.Cancellation)
                        .ThenInclude(c => c.Order)
                            .ThenInclude(o => o.Customer)
                    .Include(r => r.Payment)
                    .Where(r => r.Status == RefundStatus.Pending || r.Status == RefundStatus.Failed)
                    .ToListAsync(cancellationToken);

                logger.LogInformation("Found {RefundCount} pending or failed refunds to process.", refunds.Count);

                foreach (var refund in refunds)
                {
                    var gatewayResponse = await refundService.ProcessRefundPaymentAsync(refund);
                    if (gatewayResponse.IsSuccess)
                    {
                        refund.Status = RefundStatus.Completed;
                        refund.TransactionId = gatewayResponse.TransactionId;
                        refund.CompletedAt = DateTime.UtcNow;
                        refund.Payment.Status = PaymentStatus.Refunded;
                        context.Payments.Update(refund.Payment);

                        context.Refunds.Update(refund);
                        await context.SaveChangesAsync(cancellationToken);
                        logger.LogInformation(
                            "Refund {RefundId} completed with transaction {TransactionId}.",
                            refund.Id,
                            refund.TransactionId);

                        if (refund.Cancellation?.Order?.Customer != null &&
                            !string.IsNullOrEmpty(refund.Cancellation.Order.Customer.Email))
                        {
                            await emailService.SendEmailAsync(
                                refund.Cancellation.Order.Customer.Email,
                                 $"Your Refund Has Been Processed Successfully, Order #{refund.Cancellation.Order.OrderNumber}",
                                refundService.GenerateRefundSuccessEmailBody(refund, refund.Cancellation.Order.OrderNumber, refund.Cancellation),
                                isBodyHtml: true);
                        }
                    }
                    else
                    {
                        refund.Status = gatewayResponse.Status;
                        refund.CompletedAt = DateTime.UtcNow;
                        await context.SaveChangesAsync(cancellationToken);
                        logger.LogWarning(
                            "Refund {RefundId} was not completed. Gateway status {RefundStatus}.",
                            refund.Id,
                            refund.Status);
                    }
                }
            }
        }
    }
}
