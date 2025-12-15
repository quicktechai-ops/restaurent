namespace Restaurant.API.DTOs;

// Dashboard
public class DashboardResponse
{
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int InactiveCompanies { get; set; }
    public int SuspendedCompanies { get; set; }
    public decimal TotalIncome { get; set; }
    public int TotalBranches { get; set; }
    public int TotalUsers { get; set; }
    public List<RecentCompanyDto> RecentCompanies { get; set; } = new();
    public List<RecentBillingDto> RecentBillings { get; set; } = new();
}

public class RecentCompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecentBillingDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}

// Companies
public class CompanyListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? PlanId { get; set; }
    public string? PlanName { get; set; }
    public DateTime? PlanExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int? PlanId { get; set; }
    public DateTime? PlanExpiryDate { get; set; }
    public string Status { get; set; } = "active";
    public string? Notes { get; set; }
}

public class UpdateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int? PlanId { get; set; }
    public DateTime? PlanExpiryDate { get; set; }
    public string Status { get; set; } = "active";
    public string? Notes { get; set; }
    public string? NewPassword { get; set; }
}

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

// Plans
public class PlanListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string BillingCycle { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public int MaxBranches { get; set; }
    public int MaxUsers { get; set; }
    public int? MaxOrdersPerMonth { get; set; }
    public string? Features { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreatePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string BillingCycle { get; set; } = "Monthly";
    public int DurationDays { get; set; } = 30;
    public int MaxBranches { get; set; } = 1;
    public int MaxUsers { get; set; } = 5;
    public int? MaxOrdersPerMonth { get; set; }
    public string? Features { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}

public class UpdatePlanRequest : CreatePlanRequest
{
}

// Billing
public class BillingListDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateBillingRequest
{
    public int CompanyId { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string PaymentMethod { get; set; } = "Cash";
    public string? PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "completed";
    public string? Notes { get; set; }
}

public class UpdateBillingRequest : CreateBillingRequest
{
}
