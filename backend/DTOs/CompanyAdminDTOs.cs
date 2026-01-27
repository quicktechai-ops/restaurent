namespace Restaurant.API.DTOs;

// ============================================
// DASHBOARD
// ============================================
public class CompanyDashboardDto
{
    public int TotalBranches { get; set; }
    public int TotalUsers { get; set; }
    public int TotalMenuItems { get; set; }
    public int TotalCategories { get; set; }
    public int TotalTables { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime? PlanExpiryDate { get; set; }
    public int MaxBranches { get; set; }
    public int MaxUsers { get; set; }
}

// ============================================
// BRANCHES
// ============================================
public class BranchListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal VatPercent { get; set; }
    public decimal ServiceChargePercent { get; set; }
    public bool IsActive { get; set; }
    public int TablesCount { get; set; }
    public int UsersCount { get; set; }
}

public class CreateBranchRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal VatPercent { get; set; }
    public decimal ServiceChargePercent { get; set; }
}

public class UpdateBranchRequest : CreateBranchRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// USERS
// ============================================
public class UserListDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public int? DefaultBranchId { get; set; }
    public string? DefaultBranchName { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public int? DefaultBranchId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

public class UpdateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public int? DefaultBranchId { get; set; }
    public bool IsActive { get; set; } = true;
    public List<int> RoleIds { get; set; } = new();
}

public class ResetUserPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

// ============================================
// ROLES & PERMISSIONS
// ============================================
public class RoleListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public bool IsActive { get; set; }
    public int UsersCount { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? BranchId { get; set; }
    public List<int> PermissionIds { get; set; } = new();
}

public class UpdateRoleRequest : CreateRoleRequest
{
    public bool IsActive { get; set; } = true;
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// ============================================
// CATEGORIES
// ============================================
public class CategoryListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? Image { get; set; }
    public int ItemsCount { get; set; }
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public int? ParentCategoryId { get; set; }
    public int? BranchId { get; set; }
    public int SortOrder { get; set; } = 0;
    public string? Image { get; set; }
}

public class UpdateCategoryRequest : CreateCategoryRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// MENU ITEMS
// ============================================
public class MenuItemListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal DefaultPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool TaxIncluded { get; set; }
    public bool AllowSizes { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageUrl { get; set; }
    public int? KitchenStationId { get; set; }
    public string? KitchenStationName { get; set; }
    public List<MenuItemSizeDto> Sizes { get; set; } = new();
}

public class MenuItemSizeDto
{
    public int Id { get; set; }
    public string SizeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}

public class CreateMenuItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public int CategoryId { get; set; }
    public int? BranchId { get; set; }
    public decimal DefaultPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool TaxIncluded { get; set; } = false;
    public bool AllowSizes { get; set; } = false;
    public int? KitchenStationId { get; set; }
    public string? ImageUrl { get; set; }
    public int? Calories { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public string? Allergens { get; set; }
    public List<CreateMenuItemSizeRequest> Sizes { get; set; } = new();
    public List<int> ModifierIds { get; set; } = new();
}

public class CreateMenuItemSizeRequest
{
    public string SizeName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal? Cost { get; set; }
}

public class UpdateMenuItemRequest : CreateMenuItemRequest
{
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
}

// ============================================
// MODIFIERS
// ============================================
public class ModifierListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public decimal ExtraPrice { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public bool IsActive { get; set; }
}

public class CreateModifierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? Description { get; set; }
    public decimal ExtraPrice { get; set; } = 0;
    public string CurrencyCode { get; set; } = "USD";
    public int? BranchId { get; set; }
}

public class UpdateModifierRequest : CreateModifierRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// TABLES
// ============================================
public class TableListDto
{
    public int Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string? Zone { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = "Available";
    public int FloorNumber { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateTableRequest
{
    public int BranchId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string? Zone { get; set; }
    public int Capacity { get; set; }
    public int FloorNumber { get; set; } = 1;
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
}

public class UpdateTableRequest : CreateTableRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// KITCHEN STATIONS
// ============================================
public class KitchenStationListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int AveragePrepTime { get; set; }
    public int DisplayOrder { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int MenuItemsCount { get; set; }
}

public class CreateKitchenStationRequest
{
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int AveragePrepTime { get; set; } = 10;
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateKitchenStationRequest : CreateKitchenStationRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// PAYMENT METHODS
// ============================================
public class PaymentMethodListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Cash";
    public bool RequiresReference { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreatePaymentMethodRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Cash"; // Cash, Card, GiftCard, LoyaltyPoints, Other
    public bool RequiresReference { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}

public class UpdatePaymentMethodRequest : CreatePaymentMethodRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// CURRENCIES & EXCHANGE RATES
// ============================================
public class CurrencyListDto
{
    public string CurrencyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public int DecimalPlaces { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCurrencyRequest
{
    public string CurrencyCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; } = 2;
}

public class UpdateCurrencyRequest
{
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; } = 2;
}

public class ExchangeRateListDto
{
    public int Id { get; set; }
    public string BaseCurrencyCode { get; set; } = string.Empty;
    public string ForeignCurrencyCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class CreateExchangeRateRequest
{
    public string BaseCurrencyCode { get; set; } = string.Empty;
    public string ForeignCurrencyCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

// ============================================
// CUSTOMERS
// ============================================
public class CustomerListDto
{
    public int Id { get; set; }
    public string? CustomerCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? DefaultBranchId { get; set; }
    public string? DefaultBranchName { get; set; }
    public bool IsActive { get; set; }
    public int AddressesCount { get; set; }
    public int OrdersCount { get; set; }
    public decimal? LoyaltyPoints { get; set; }
}

public class CreateCustomerRequest
{
    public string? CustomerCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? DefaultBranchId { get; set; }
    public string? DefaultCurrencyCode { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCustomerRequest : CreateCustomerRequest
{
    public bool IsActive { get; set; } = true;
}

public class CustomerAddressDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Area { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? DeliveryZoneId { get; set; }
    public string? DeliveryZoneName { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateCustomerAddressRequest
{
    public int CustomerId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Area { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? DeliveryZoneId { get; set; }
    public bool IsDefault { get; set; } = false;
}

// ============================================
// DELIVERY ZONES
// ============================================
public class DeliveryZoneListDto
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string ZoneName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal BaseFee { get; set; }
    public decimal? ExtraFeePerKm { get; set; }
    public decimal? MaxDistanceKm { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDeliveryZoneRequest
{
    public int BranchId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public decimal BaseFee { get; set; } = 0;
    public decimal? ExtraFeePerKm { get; set; }
    public decimal? MaxDistanceKm { get; set; }
}

public class UpdateDeliveryZoneRequest : CreateDeliveryZoneRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// INVENTORY
// ============================================
public class InventoryItemListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal MinLevel { get; set; }
    public decimal ReorderQty { get; set; }
    public string CostMethod { get; set; } = "Average";
    public decimal Quantity { get; set; }
    public decimal Cost { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsActive { get; set; }
    public int RecipesCount { get; set; }
}

public class CreateInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal MinLevel { get; set; } = 0;
    public decimal ReorderQty { get; set; } = 0;
    public string CostMethod { get; set; } = "Average";
    public decimal Quantity { get; set; } = 0;
    public decimal Cost { get; set; } = 0;
    public string? CurrencyCode { get; set; }
}

public class UpdateInventoryItemRequest : CreateInventoryItemRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// SUPPLIERS
// ============================================
public class SupplierListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PaymentTerms { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PaymentTerms { get; set; }
}

public class UpdateSupplierRequest : CreateSupplierRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// EMPLOYEES
// ============================================
public class EmployeeListDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal? BaseSalary { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class CreateEmployeeRequest
{
    public int? UserId { get; set; }
    public int BranchId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public decimal? BaseSalary { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class UpdateEmployeeRequest : CreateEmployeeRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// LOYALTY
// ============================================
public class LoyaltySettingsDto
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public decimal PointsPerAmount { get; set; }
    public decimal AmountUnit { get; set; }
    public bool EarnOnNetBeforeTax { get; set; }
    public decimal PointsRedeemValue { get; set; }
    public int? PointsExpiryMonths { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateLoyaltySettingsRequest
{
    public int? BranchId { get; set; }
    public decimal PointsPerAmount { get; set; } = 1;
    public decimal AmountUnit { get; set; } = 10;
    public bool EarnOnNetBeforeTax { get; set; } = true;
    public decimal PointsRedeemValue { get; set; } = 0.1m;
    public int? PointsExpiryMonths { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LoyaltyTierListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MinTotalSpent { get; set; }
    public decimal MinTotalPoints { get; set; }
    public decimal TierDiscountPercent { get; set; }
    public string? Benefits { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int CustomersCount { get; set; }
}

public class CreateLoyaltyTierRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal MinTotalSpent { get; set; }
    public decimal MinTotalPoints { get; set; } = 0;
    public decimal TierDiscountPercent { get; set; } = 0;
    public string? Benefits { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateLoyaltyTierRequest : CreateLoyaltyTierRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// GIFT CARDS
// ============================================
public class GiftCardListDto
{
    public int Id { get; set; }
    public string GiftCardNumber { get; set; } = string.Empty;
    public int BranchIssuedId { get; set; }
    public string BranchIssuedName { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal InitialValue { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "Active";
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
}

public class CreateGiftCardRequest
{
    public string? GiftCardNumber { get; set; } // Auto-generated if empty
    public int BranchIssuedId { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal InitialValue { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? CustomerId { get; set; }
}

public class GiftCardTransactionDto
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Notes { get; set; }
}

// ============================================
// RESERVATIONS
// ============================================
public class ReservationListDto
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int PartySize { get; set; }
    public int? TableId { get; set; }
    public string? TableName { get; set; }
    public string Status { get; set; } = "Pending";
    public string Channel { get; set; } = "Phone";
    public string? Notes { get; set; }
    public bool HasDeposit { get; set; }
    public decimal? DepositAmount { get; set; }
    public string? DepositStatus { get; set; }
}

public class CreateReservationRequest
{
    public int BranchId { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; } = 90;
    public int PartySize { get; set; }
    public int? TableId { get; set; }
    public string Channel { get; set; } = "Phone";
    public string? Notes { get; set; }
    public bool RequireDeposit { get; set; } = false;
    public decimal? DepositAmount { get; set; }
    public string DepositCurrencyCode { get; set; } = "USD";
}

public class UpdateReservationRequest : CreateReservationRequest
{
    public string Status { get; set; } = "Pending";
}

// ============================================
// PRINTERS
// ============================================
public class PrinterListDto
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PrinterType { get; set; } = "Receipt";
    public string ConnectionType { get; set; } = "Network";
    public string? ConnectionString { get; set; }
    public int PaperWidth { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePrinterRequest
{
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PrinterType { get; set; } = "Receipt";
    public string ConnectionType { get; set; } = "Network";
    public string? ConnectionString { get; set; }
    public int PaperWidth { get; set; } = 80;
    public bool IsDefault { get; set; } = false;
}

public class UpdatePrinterRequest : CreatePrinterRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// RECEIPT TEMPLATES
// ============================================
public class ReceiptTemplateListDto
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string TemplateType { get; set; } = "CustomerReceipt";
    public string Name { get; set; } = string.Empty;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public bool ShowLogo { get; set; }
    public bool ShowBarcode { get; set; }
    public string Language { get; set; } = "en";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class CreateReceiptTemplateRequest
{
    public int? BranchId { get; set; }
    public string TemplateType { get; set; } = "CustomerReceipt";
    public string Name { get; set; } = string.Empty;
    public string? HeaderText { get; set; }
    public string? FooterText { get; set; }
    public bool ShowLogo { get; set; } = true;
    public bool ShowBarcode { get; set; } = false;
    public string Language { get; set; } = "en";
    public bool IsDefault { get; set; } = false;
}

public class UpdateReceiptTemplateRequest : CreateReceiptTemplateRequest
{
    public bool IsActive { get; set; } = true;
}

// ============================================
// SYSTEM SETTINGS
// ============================================
public class SystemSettingDto
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
    public string SettingType { get; set; } = "String";
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateSystemSettingRequest
{
    public int? BranchId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
    public string SettingType { get; set; } = "String";
    public string? Description { get; set; }
}
