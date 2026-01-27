using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Restaurant.API.Models;

namespace Restaurant.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
        base.OnConfiguring(optionsBuilder);
    }

    // SuperAdmin Tables
    public DbSet<SuperAdmin> SuperAdmins { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyPayment> CompanyPayments { get; set; }

    // Core Tables
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<User> Users { get; set; }

    // Security & Roles
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    // Menu
    public DbSet<Category> Categories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<MenuItemSize> MenuItemSizes { get; set; }
    public DbSet<Modifier> Modifiers { get; set; }
    public DbSet<MenuItemModifier> MenuItemModifiers { get; set; }

    // Dining & Kitchen
    public DbSet<RestaurantTable> RestaurantTables { get; set; }
    public DbSet<KitchenStation> KitchenStations { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    // Customers
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<DeliveryZone> DeliveryZones { get; set; }

    // Exchange Rates
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    // Inventory
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<InventoryCategory> InventoryCategories { get; set; }
    public DbSet<UnitOfMeasure> UnitsOfMeasure { get; set; }
    public DbSet<UnitConversion> UnitConversions { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    
    // Purchasing & Stock
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
    public DbSet<GoodsReceiptLine> GoodsReceiptLines { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<Wastage> Wastages { get; set; }
    public DbSet<StockAdjustment> StockAdjustments { get; set; }
    public DbSet<StockCount> StockCounts { get; set; }
    public DbSet<StockCountLine> StockCountLines { get; set; }

    // HR & Employees
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<CommissionPolicy> CommissionPolicies { get; set; }
    
    // Approval Rules
    public DbSet<ApprovalRule> ApprovalRules { get; set; }

    // Loyalty
    public DbSet<LoyaltySettings> LoyaltySettings { get; set; }
    public DbSet<LoyaltyTier> LoyaltyTiers { get; set; }
    public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; }
    public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }

    // Gift Cards
    public DbSet<GiftCard> GiftCards { get; set; }
    public DbSet<GiftCardTransaction> GiftCardTransactions { get; set; }

    // Reservations
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationDeposit> ReservationDeposits { get; set; }

    // Printing
    public DbSet<Printer> Printers { get; set; }
    public DbSet<KitchenStationPrinter> KitchenStationPrinters { get; set; }
    public DbSet<ReceiptTemplate> ReceiptTemplates { get; set; }

    // Settings & Audit
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    // POS & Orders
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<OrderLineModifier> OrderLineModifiers { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<OrderDeliveryDetails> OrderDeliveryDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Table name mappings for tables with non-standard names
        modelBuilder.Entity<AuditLog>().ToTable("audit_log");
        modelBuilder.Entity<SystemSetting>().ToTable("system_settings");

        // Plan -> Companies
        modelBuilder.Entity<Company>()
            .HasOne(c => c.Plan)
            .WithMany(p => p.Companies)
            .HasForeignKey(c => c.PlanId)
            .OnDelete(DeleteBehavior.SetNull);

        // SuperAdmin -> Companies (created by)
        modelBuilder.Entity<Company>()
            .HasOne(c => c.CreatedBySuperAdmin)
            .WithMany()
            .HasForeignKey(c => c.CreatedBySuperAdminId)
            .OnDelete(DeleteBehavior.SetNull);

        // Company -> Branches
        modelBuilder.Entity<Branch>()
            .HasOne(b => b.Company)
            .WithMany(c => c.Branches)
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Branch -> Currency
        modelBuilder.Entity<Branch>()
            .HasOne(b => b.DefaultCurrency)
            .WithMany()
            .HasForeignKey(b => b.DefaultCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Company -> Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Default Branch
        modelBuilder.Entity<User>()
            .HasOne(u => u.DefaultBranch)
            .WithMany()
            .HasForeignKey(u => u.DefaultBranchId)
            .OnDelete(DeleteBehavior.SetNull);

        // Company -> Payments
        modelBuilder.Entity<CompanyPayment>()
            .HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // SuperAdmin -> Payments (recorded by)
        modelBuilder.Entity<CompanyPayment>()
            .HasOne(p => p.RecordedBySuperAdmin)
            .WithMany()
            .HasForeignKey(p => p.RecordedBySuperAdminId)
            .OnDelete(DeleteBehavior.SetNull);

        // Unique constraints
        modelBuilder.Entity<SuperAdmin>()
            .HasIndex(s => s.Username)
            .IsUnique();

        modelBuilder.Entity<Company>()
            .HasIndex(c => c.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.CompanyId, u.Username })
            .IsUnique();

        modelBuilder.Entity<Branch>()
            .HasIndex(b => new { b.CompanyId, b.Code })
            .IsUnique();

        // Role -> Company
        modelBuilder.Entity<Role>()
            .HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // RolePermission -> Role & Permission
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserRole -> User & Role
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category -> Company
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // MenuItem -> Company & Category
        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.Company)
            .WithMany()
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.Category)
            .WithMany(c => c.MenuItems)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.KitchenStation)
            .WithMany(k => k.MenuItems)
            .HasForeignKey(m => m.KitchenStationId)
            .OnDelete(DeleteBehavior.SetNull);

        // MenuItemSize -> MenuItem
        modelBuilder.Entity<MenuItemSize>()
            .HasOne(s => s.MenuItem)
            .WithMany(m => m.Sizes)
            .HasForeignKey(s => s.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Modifier -> Company
        modelBuilder.Entity<Modifier>()
            .HasOne(m => m.Company)
            .WithMany()
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // MenuItemModifier -> MenuItem & Modifier
        modelBuilder.Entity<MenuItemModifier>()
            .HasOne(mm => mm.MenuItem)
            .WithMany(m => m.MenuItemModifiers)
            .HasForeignKey(mm => mm.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuItemModifier>()
            .HasOne(mm => mm.Modifier)
            .WithMany(m => m.MenuItemModifiers)
            .HasForeignKey(mm => mm.ModifierId)
            .OnDelete(DeleteBehavior.Cascade);

        // RestaurantTable -> Branch
        modelBuilder.Entity<RestaurantTable>()
            .HasOne(t => t.Branch)
            .WithMany()
            .HasForeignKey(t => t.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // KitchenStation -> Branch
        modelBuilder.Entity<KitchenStation>()
            .HasOne(k => k.Branch)
            .WithMany()
            .HasForeignKey(k => k.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // PaymentMethod -> Company
        modelBuilder.Entity<PaymentMethod>()
            .HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Customer -> Company
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => new { c.CompanyId, c.CustomerCode })
            .IsUnique()
            .HasFilter("customer_code IS NOT NULL");

        // CustomerAddress -> Customer
        modelBuilder.Entity<CustomerAddress>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.Addresses)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // DeliveryZone -> Branch
        modelBuilder.Entity<DeliveryZone>()
            .HasOne(d => d.Branch)
            .WithMany()
            .HasForeignKey(d => d.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // ExchangeRate -> Company
        modelBuilder.Entity<ExchangeRate>()
            .HasOne(e => e.Company)
            .WithMany()
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // InventoryItem -> Company
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Company)
            .WithMany()
            .HasForeignKey(i => i.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => new { i.CompanyId, i.Code })
            .IsUnique()
            .HasFilter("code IS NOT NULL");

        // Recipe -> Company & MenuItem
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.MenuItem)
            .WithMany()
            .HasForeignKey(r => r.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Recipe>()
            .HasIndex(r => new { r.MenuItemId, r.MenuItemSizeId })
            .IsUnique();

        // RecipeIngredient -> Recipe & InventoryItem
        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.InventoryItem)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Supplier -> Company
        modelBuilder.Entity<Supplier>()
            .HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Employee -> Company & Branch
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Company)
            .WithMany()
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Branch)
            .WithMany()
            .HasForeignKey(e => e.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // CommissionPolicy -> Company
        modelBuilder.Entity<CommissionPolicy>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // LoyaltySettings -> Company
        modelBuilder.Entity<LoyaltySettings>()
            .HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // LoyaltyTier -> Company
        modelBuilder.Entity<LoyaltyTier>()
            .HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // LoyaltyAccount -> Customer
        modelBuilder.Entity<LoyaltyAccount>()
            .HasOne(l => l.Customer)
            .WithMany()
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // LoyaltyTransaction -> LoyaltyAccount
        modelBuilder.Entity<LoyaltyTransaction>()
            .HasOne(l => l.LoyaltyAccount)
            .WithMany()
            .HasForeignKey(l => l.LoyaltyAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // GiftCard -> Company
        modelBuilder.Entity<GiftCard>()
            .HasOne(g => g.Company)
            .WithMany()
            .HasForeignKey(g => g.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GiftCard>()
            .HasIndex(g => new { g.CompanyId, g.GiftCardNumber })
            .IsUnique();

        // GiftCardTransaction -> GiftCard
        modelBuilder.Entity<GiftCardTransaction>()
            .HasOne(t => t.GiftCard)
            .WithMany(g => g.Transactions)
            .HasForeignKey(t => t.GiftCardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Reservation -> Branch
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Branch)
            .WithMany()
            .HasForeignKey(r => r.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // ReservationDeposit -> Reservation
        modelBuilder.Entity<ReservationDeposit>()
            .HasOne(d => d.Reservation)
            .WithOne(r => r.Deposit)
            .HasForeignKey<ReservationDeposit>(d => d.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Printer -> Branch
        modelBuilder.Entity<Printer>()
            .HasOne(p => p.Branch)
            .WithMany()
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // KitchenStationPrinter -> KitchenStation & Printer
        modelBuilder.Entity<KitchenStationPrinter>()
            .HasOne(kp => kp.KitchenStation)
            .WithMany()
            .HasForeignKey(kp => kp.KitchenStationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KitchenStationPrinter>()
            .HasOne(kp => kp.Printer)
            .WithMany()
            .HasForeignKey(kp => kp.PrinterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<KitchenStationPrinter>()
            .HasIndex(kp => new { kp.KitchenStationId, kp.PrinterId })
            .IsUnique();

        // ReceiptTemplate -> Company
        modelBuilder.Entity<ReceiptTemplate>()
            .HasOne(r => r.Company)
            .WithMany()
            .HasForeignKey(r => r.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // SystemSetting -> Company
        modelBuilder.Entity<SystemSetting>()
            .HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SystemSetting>()
            .HasIndex(s => new { s.CompanyId, s.BranchId, s.SettingKey })
            .IsUnique();

        // AuditLog -> Company
        modelBuilder.Entity<AuditLog>()
            .HasOne(a => a.Company)
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========== POS & ORDERS ==========

        // Shift -> Company & Branch & User
        modelBuilder.Entity<Shift>()
            .HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shift>()
            .HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shift>()
            .HasOne(s => s.CashierUser)
            .WithMany()
            .HasForeignKey(s => s.CashierUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order -> Company & Branch & Shift & Table & Users & Customer
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Company)
            .WithMany()
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Branch)
            .WithMany()
            .HasForeignKey(o => o.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Shift)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ShiftId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Table)
            .WithMany()
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.WaiterUser)
            .WithMany()
            .HasForeignKey(o => o.WaiterUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.CashierUser)
            .WithMany()
            .HasForeignKey(o => o.CashierUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.VoidByUser)
            .WithMany()
            .HasForeignKey(o => o.VoidByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.ApprovedVoidByUser)
            .WithMany()
            .HasForeignKey(o => o.ApprovedVoidByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasIndex(o => new { o.BranchId, o.OrderNumber })
            .IsUnique();

        // OrderLine -> Order & MenuItem & Size & KitchenStation
        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.Order)
            .WithMany(o => o.OrderLines)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.MenuItem)
            .WithMany()
            .HasForeignKey(ol => ol.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.MenuItemSize)
            .WithMany()
            .HasForeignKey(ol => ol.MenuItemSizeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.KitchenStation)
            .WithMany()
            .HasForeignKey(ol => ol.KitchenStationId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderLine>()
            .HasOne(ol => ol.CreatedByUser)
            .WithMany()
            .HasForeignKey(ol => ol.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // OrderLineModifier -> OrderLine & Modifier
        modelBuilder.Entity<OrderLineModifier>()
            .HasOne(olm => olm.OrderLine)
            .WithMany(ol => ol.OrderLineModifiers)
            .HasForeignKey(olm => olm.OrderLineId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderLineModifier>()
            .HasOne(olm => olm.Modifier)
            .WithMany()
            .HasForeignKey(olm => olm.ModifierId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderPayment -> Order & PaymentMethod & GiftCard & User
        modelBuilder.Entity<OrderPayment>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderPayments)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderPayment>()
            .HasOne(op => op.PaymentMethod)
            .WithMany()
            .HasForeignKey(op => op.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderPayment>()
            .HasOne(op => op.GiftCard)
            .WithMany()
            .HasForeignKey(op => op.GiftCardId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderPayment>()
            .HasOne(op => op.User)
            .WithMany()
            .HasForeignKey(op => op.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderStatusHistory -> Order & User
        modelBuilder.Entity<OrderStatusHistory>()
            .HasOne(osh => osh.Order)
            .WithMany(o => o.StatusHistory)
            .HasForeignKey(osh => osh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderStatusHistory>()
            .HasOne(osh => osh.User)
            .WithMany()
            .HasForeignKey(osh => osh.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // OrderDeliveryDetails -> Order & CustomerAddress & DeliveryZone
        modelBuilder.Entity<OrderDeliveryDetails>()
            .HasOne(odd => odd.Order)
            .WithOne(o => o.DeliveryDetails)
            .HasForeignKey<OrderDeliveryDetails>(odd => odd.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDeliveryDetails>()
            .HasOne(odd => odd.CustomerAddress)
            .WithMany()
            .HasForeignKey(odd => odd.CustomerAddressId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderDeliveryDetails>()
            .HasOne(odd => odd.DeliveryZone)
            .WithMany()
            .HasForeignKey(odd => odd.DeliveryZoneId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
