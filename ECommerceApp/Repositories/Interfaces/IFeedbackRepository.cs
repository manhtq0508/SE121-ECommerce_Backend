using ECommerceApp.Entities;

namespace ECommerceApp.Repositories.Interfaces;

public interface IFeedbackRepository
{
    Task<bool> ExistsByCustomerAndProductAsync(int customerId, int productId);
    Task<List<Feedback>> GetByProductIdWithCustomerAsync(int productId, bool trackChanges = false);
    Task<List<Feedback>> GetAllWithDetailsAsync(bool trackChanges = false);
    Task<Feedback?> GetByIdWithDetailsAsync(int id, bool trackChanges = false);
    Task<Feedback?> GetByIdAndCustomerIdWithDetailsAsync(int id, int customerId, bool trackChanges = false);
    Task<Feedback?> GetByIdAsync(int id, bool trackChanges = false);
    void Add(Feedback feedback);
    void Remove(Feedback feedback);
}
