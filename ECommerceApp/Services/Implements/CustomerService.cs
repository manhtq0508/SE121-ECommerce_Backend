using ECommerceApp.Commons;
using ECommerceApp.DTOs.CustomerDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Customers;
using ECommerceApp.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerceApp.Repositories.Interfaces;

namespace ECommerceApp.Services.Implements;

public class CustomerService(
    IUnitOfWork unitOfWork, 
    IConfiguration configuration, 
    ICustomerMapper mapper,
    ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<ApiResponse<CustomerResponse>> RegisterCustomerAsync(CustomerRegistrationRequest customerDto)
    {
        try
        {
            if (await unitOfWork.CustomerRepository.ExistsByEmailAsync(customerDto.Email))
            {
                return new ApiResponse<CustomerResponse>(400, "Email is already in use.");
            }

            var customer = mapper.Map(customerDto);
            customer.IsActive = true;
            customer.Password = BCrypt.Net.BCrypt.HashPassword(customerDto.Password);

            unitOfWork.CustomerRepository.Add(customer);
            await unitOfWork.SaveChangesAsync();

            var customerResponse = mapper.Map(customer);

            return new ApiResponse<CustomerResponse>(200, customerResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in CustomerService.");
            return new ApiResponse<CustomerResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetByEmailAsync(request.Email);

            if (customer == null || !customer.IsActive)
            {
                return new ApiResponse<LoginResponse>(401, "Invalid email or password.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, customer.Password);
            if (!isPasswordValid)
            {
                return new ApiResponse<LoginResponse>(401, "Invalid email or password.");
            }

            var role = IsAdminCustomer(customer.Email) ? "Admin" : "Customer";
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(GetTokenLifetimeMinutes());

            var loginResponse = new LoginResponse
            {
                Message = "Login successful.",
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                AccessToken = GenerateJwtToken(customer, role, expiresAtUtc),
                ExpiresAtUtc = expiresAtUtc,
                Role = role
            };

            return new ApiResponse<LoginResponse>(200, loginResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in CustomerService.");
            return new ApiResponse<LoginResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetActiveByIdAsync(id);

            if (customer == null)
            {
                return new ApiResponse<CustomerResponse>(404, "Customer not found.");
            }

            var customerResponse = mapper.Map(customer);

            return new ApiResponse<CustomerResponse>(200, customerResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in CustomerService.");
            return new ApiResponse<CustomerResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> UpdateCustomerAsync(int customerId, CustomerUpdateRequest customerDto)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetActiveByIdAsync(customerId, trackChanges: true);
            if (customer == null)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Customer not found or inactive.");
            }

            if (customer.Email != customerDto.Email && await unitOfWork.CustomerRepository.ExistsByEmailAsync(customerDto.Email, excludeCustomerId: customerId))
            {
                return new ApiResponse<ConfirmationResponse>(400, "Email is already in use.");
            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Email = customerDto.Email;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.DateOfBirth = customerDto.DateOfBirth;

            await unitOfWork.SaveChangesAsync();

            var confirmationMessage = new ConfirmationResponse
            {
                Message = $"Customer with Id {customerId} updated successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in CustomerService.");
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> DeleteCustomerAsync(int id)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetActiveByIdAsync(id, trackChanges: true);

            if (customer == null)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Customer not found or inactive.");
            }

            customer.IsActive = false;
            await unitOfWork.SaveChangesAsync();

            var confirmationMessage = new ConfirmationResponse
            {
                Message = $"Customer with Id {id} deleted successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in CustomerService.");
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> ChangePasswordAsync(int customerId, ChangePasswordRequest request)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId, trackChanges: true);
            
            if (customer == null || !customer.IsActive)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Customer not found or inactive.");
            }

            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, customer.Password);
            if (!isCurrentPasswordValid)
            {
                return new ApiResponse<ConfirmationResponse>(401, "Current password is incorrect.");
            }

            customer.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await unitOfWork.SaveChangesAsync();

            var confirmationMessage = new ConfirmationResponse
            {
                Message = "Password changed successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in CustomerService.");
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    private int GetTokenLifetimeMinutes()
    {
        return int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var minutes) && minutes > 0 ? minutes : 480;
    }

    private bool IsAdminCustomer(string email)
    {
        var adminEmails = configuration["Jwt:AdminEmails"];

        if (string.IsNullOrWhiteSpace(adminEmails))
        {
            return false;
        }

        return adminEmails
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(adminEmail => adminEmail.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    private string GenerateJwtToken(Customer customer, string role, DateTime expiresAtUtc)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new("customer_id", customer.Id.ToString()),
            new(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
            new(ClaimTypes.Email, customer.Email),
            new(ClaimTypes.Role, role)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
