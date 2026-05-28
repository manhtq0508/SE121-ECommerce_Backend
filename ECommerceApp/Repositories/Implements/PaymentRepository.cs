using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements;

public class PaymentRepository(ApplicationDbContext context) : IPaymentRepository
{
    public async Task<Payment?> GetByIdAsync(int id, bool trackChanges = false)
    {
        var query = context.Payments.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Payment?> GetByOrderIdAsync(int orderId, bool trackChanges = false)
    {
        var query = context.Payments.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }
    
    public async Task<Payment?> GetByIdWithOrderAsync(int id, bool trackChanges = false)
    {
        var query = context.Payments.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(p => p.Order) // Kéo theo thông tin Order
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Payment?> GetByIdAndOrderIdWithOrderAsync(int paymentId, int orderId, bool trackChanges = false)
    {
        var query = context.Payments.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.OrderId == orderId);
    }
    
    public void Add(Payment payment)
    {
        context.Payments.Add(payment);
    }
}
