using ECommerceApp.Data;
using ECommerceApp.Entities;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Repositories.Implements;

public class FeedbackRepository(ApplicationDbContext context) : IFeedbackRepository
{
    public async Task<bool> ExistsByCustomerAndProductAsync(int customerId, int productId)
    {
        return await context.Feedbacks.AnyAsync(f => f.CustomerId == customerId && f.ProductId == productId);
    }
    
    public async Task<List<Feedback>> GetByProductIdWithCustomerAsync(int productId, bool trackChanges = false)
    {
        var query = context.Feedbacks.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(f => f.Customer)
            .Where(f => f.ProductId == productId)
            .ToListAsync();
    }
    
    public async Task<List<Feedback>> GetAllWithDetailsAsync(bool trackChanges = false)
    {
        var query = context.Feedbacks.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(f => f.Customer)
            .Include(f => f.Product)
            .ToListAsync();
    }

    public async Task<Feedback?> GetByIdWithDetailsAsync(int id, bool trackChanges = false)
    {
        var query = context.Feedbacks.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(f => f.Customer)
            .Include(f => f.Product)
            .FirstOrDefaultAsync(f => f.Id == id);
    }
    
    public async Task<Feedback?> GetByIdAndCustomerIdWithDetailsAsync(int id, int customerId, bool trackChanges = false)
    {
        var query = context.Feedbacks.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query
            .Include(f => f.Customer)
            .Include(f => f.Product)
            .FirstOrDefaultAsync(f => f.Id == id && f.CustomerId == customerId);
    }
    
    public async Task<Feedback?> GetByIdAsync(int id, bool trackChanges = false)
    {
        var query = context.Feedbacks.AsQueryable();
    
        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(f => f.Id == id);
    }
    
    public void Add(Feedback feedback)
    {
        context.Feedbacks.Add(feedback);
    }
    
    public void Remove(Feedback feedback)
    {
        context.Feedbacks.Remove(feedback);
    }
}
