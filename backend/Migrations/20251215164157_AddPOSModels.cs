using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Restaurant.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPOSModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_branches_BranchId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_companies_CompanyId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryZones_branches_BranchId",
                table: "DeliveryZones");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryZones_companies_CompanyId",
                table: "DeliveryZones");

            migrationBuilder.DropForeignKey(
                name: "FK_GiftCards_users_CreatedByUserId",
                table: "GiftCards");

            migrationBuilder.DropForeignKey(
                name: "FK_Printers_branches_BranchId",
                table: "Printers");

            migrationBuilder.DropForeignKey(
                name: "FK_Printers_companies_CompanyId",
                table: "Printers");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_branches_BranchId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_companies_CompanyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_restaurant_tables_TableId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_users_CreatedByUserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemSettings_branches_BranchId",
                table: "SystemSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemSettings_companies_CompanyId",
                table: "SystemSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemSettings_users_UpdatedByUserId",
                table: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_CompanyId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Printers_CompanyId",
                table: "Printers");

            migrationBuilder.DropIndex(
                name: "IX_GiftCards_CreatedByUserId",
                table: "GiftCards");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryZones_CompanyId",
                table: "DeliveryZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "Benefits",
                table: "LoyaltyTiers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LoyaltyTiers");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "LoyaltyTiers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LoyaltyTiers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LoyaltySettings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LoyaltySettings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LoyaltySettings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "GiftCards");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "DeliveryZones");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DeliveryZones");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DeliveryZones");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "SystemSettings",
                newName: "system_settings");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "audit_log");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Reservations",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Reservations",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Channel",
                table: "Reservations",
                newName: "channel");

            migrationBuilder.RenameColumn(
                name: "TableId",
                table: "Reservations",
                newName: "table_id");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Reservations",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "ReservationDate",
                table: "Reservations",
                newName: "reservation_date");

            migrationBuilder.RenameColumn(
                name: "PartySize",
                table: "Reservations",
                newName: "party_size");

            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Reservations",
                newName: "duration_minutes");

            migrationBuilder.RenameColumn(
                name: "CustomerPhone",
                table: "Reservations",
                newName: "customer_phone");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Reservations",
                newName: "customer_name");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Reservations",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Reservations",
                newName: "created_by_user_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Reservations",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "Reservations",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Reservations",
                newName: "reservation_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_TableId",
                table: "Reservations",
                newName: "IX_Reservations_table_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                newName: "IX_Reservations_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_CreatedByUserId",
                table: "Reservations",
                newName: "IX_Reservations_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_BranchId",
                table: "Reservations",
                newName: "IX_Reservations_branch_id");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "DeliveryZones",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "ZoneName",
                table: "DeliveryZones",
                newName: "zone_name");

            migrationBuilder.RenameColumn(
                name: "MinOrderAmount",
                table: "DeliveryZones",
                newName: "min_order_amount");

            migrationBuilder.RenameColumn(
                name: "MaxDistanceKm",
                table: "DeliveryZones",
                newName: "max_distance_km");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "DeliveryZones",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ExtraFeePerKm",
                table: "DeliveryZones",
                newName: "extra_fee_per_km");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "DeliveryZones",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "BaseFee",
                table: "DeliveryZones",
                newName: "base_fee");

            migrationBuilder.RenameColumn(
                name: "DeliveryZoneId",
                table: "DeliveryZones",
                newName: "delivery_zone_id");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryZones_BranchId",
                table: "DeliveryZones",
                newName: "IX_DeliveryZones_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_SystemSettings_UpdatedByUserId",
                table: "system_settings",
                newName: "IX_system_settings_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemSettings_CompanyId_BranchId_SettingKey",
                table: "system_settings",
                newName: "IX_system_settings_CompanyId_BranchId_SettingKey");

            migrationBuilder.RenameIndex(
                name: "IX_SystemSettings_BranchId",
                table: "system_settings",
                newName: "IX_system_settings_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "audit_log",
                newName: "IX_audit_log_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_CompanyId",
                table: "audit_log",
                newName: "IX_audit_log_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_BranchId",
                table: "audit_log",
                newName: "IX_audit_log_BranchId");

            migrationBuilder.AddColumn<decimal>(
                name: "cost",
                table: "InventoryItems",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "currency_code",
                table: "InventoryItems",
                type: "character varying(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "quantity",
                table: "InventoryItems",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_system_settings",
                table: "system_settings",
                column: "SettingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_log",
                table: "audit_log",
                column: "AuditLogId");

            migrationBuilder.CreateTable(
                name: "approval_rules",
                columns: table => new
                {
                    approval_rule_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    rule_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: true),
                    max_discount_percent_without_approval = table.Column<decimal>(type: "numeric", nullable: false),
                    allow_price_change = table.Column<bool>(type: "boolean", nullable: false),
                    require_manager_pin_for_price_change = table.Column<bool>(type: "boolean", nullable: false),
                    can_void_paid_invoice = table.Column<bool>(type: "boolean", nullable: false),
                    require_manager_approval_for_void = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_rules", x => x.approval_rule_id);
                    table.ForeignKey(
                        name: "FK_approval_rules_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_approval_rules_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    clock_in = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    clock_out = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hours_worked = table.Column<decimal>(type: "numeric", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendance_Employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendance_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCategories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_InventoryCategories_InventoryCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "InventoryCategories",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_InventoryCategories_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchase_orders",
                columns: table => new
                {
                    purchase_order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    supplier_id = table.Column<int>(type: "integer", nullable: false),
                    po_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    po_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    expected_delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    currency_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    total_estimated_amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_orders", x => x.purchase_order_id);
                    table.ForeignKey(
                        name: "FK_purchase_orders_Suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_orders_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "branch_id");
                });

            migrationBuilder.CreateTable(
                name: "shifts",
                columns: table => new
                {
                    shift_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    branch_id = table.Column<int>(type: "integer", nullable: false),
                    cashier_user_id = table.Column<int>(type: "integer", nullable: false),
                    open_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    close_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expected_close_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    opening_cash = table.Column<decimal>(type: "numeric", nullable: false),
                    closing_cash = table.Column<decimal>(type: "numeric", nullable: true),
                    expected_cash = table.Column<decimal>(type: "numeric", nullable: true),
                    cash_difference = table.Column<decimal>(type: "numeric", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    force_closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    force_close_reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shifts", x => x.shift_id);
                    table.ForeignKey(
                        name: "FK_shifts_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_shifts_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_shifts_users_cashier_user_id",
                        column: x => x.cashier_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_adjustments",
                columns: table => new
                {
                    stock_adjustment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    adjustment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_adjustments", x => x.stock_adjustment_id);
                    table.ForeignKey(
                        name: "FK_stock_adjustments_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "branch_id");
                    table.ForeignKey(
                        name: "FK_stock_adjustments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "stock_counts",
                columns: table => new
                {
                    stock_count_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    count_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_counts", x => x.stock_count_id);
                });

            migrationBuilder.CreateTable(
                name: "stock_movements",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    inventory_item_id = table.Column<int>(type: "integer", nullable: false),
                    movement_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric", nullable: true),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_movements_InventoryItems_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "InventoryItems",
                        principalColumn: "InventoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_movements_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitConversions",
                columns: table => new
                {
                    ConversionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    FromUnitCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ToUnitCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ConversionFactor = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitConversions", x => x.ConversionId);
                    table.ForeignKey(
                        name: "FK_UnitConversions_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasure",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    UnitGroup = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasure", x => x.UnitId);
                    table.ForeignKey(
                        name: "FK_UnitsOfMeasure_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "wastages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    inventory_item_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    cost_impact = table.Column<decimal>(type: "numeric", nullable: false),
                    reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    recorded_by = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wastages", x => x.id);
                    table.ForeignKey(
                        name: "FK_wastages_InventoryItems_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "InventoryItems",
                        principalColumn: "InventoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wastages_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "goods_receipts",
                columns: table => new
                {
                    goods_receipt_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    branch_id = table.Column<int>(type: "integer", nullable: true),
                    supplier_id = table.Column<int>(type: "integer", nullable: false),
                    purchase_order_id = table.Column<int>(type: "integer", nullable: true),
                    grn_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    grn_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    total_before_tax = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods_receipts", x => x.goods_receipt_id);
                    table.ForeignKey(
                        name: "FK_goods_receipts_Suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_goods_receipts_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "branch_id");
                    table.ForeignKey(
                        name: "FK_goods_receipts_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "purchase_order_id");
                });

            migrationBuilder.CreateTable(
                name: "purchase_order_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    purchase_order_id = table.Column<int>(type: "integer", nullable: false),
                    inventory_item_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_order_lines_InventoryItems_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "InventoryItems",
                        principalColumn: "InventoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchase_order_lines_purchase_orders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "purchase_orders",
                        principalColumn: "purchase_order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    branch_id = table.Column<int>(type: "integer", nullable: false),
                    shift_id = table.Column<int>(type: "integer", nullable: true),
                    order_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    order_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    table_id = table.Column<int>(type: "integer", nullable: true),
                    waiter_user_id = table.Column<int>(type: "integer", nullable: true),
                    cashier_user_id = table.Column<int>(type: "integer", nullable: true),
                    customer_id = table.Column<int>(type: "integer", nullable: true),
                    order_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    exchange_rate_to_base = table.Column<decimal>(type: "numeric", nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric", nullable: false),
                    total_line_discount = table.Column<decimal>(type: "numeric", nullable: false),
                    bill_discount_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    bill_discount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    service_charge_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    service_charge_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    delivery_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    tips_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    grand_total = table.Column<decimal>(type: "numeric", nullable: false),
                    net_amount_for_loyalty = table.Column<decimal>(type: "numeric", nullable: true),
                    loyalty_points_earned = table.Column<decimal>(type: "numeric", nullable: true),
                    loyalty_points_redeemed = table.Column<decimal>(type: "numeric", nullable: true),
                    loyalty_discount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_paid = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_due = table.Column<decimal>(type: "numeric", nullable: false),
                    payment_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    voided_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    void_reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    void_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    approved_void_by_user_id = table.Column<int>(type: "integer", nullable: true),
                    merged_from_order_id = table.Column<int>(type: "integer", nullable: true),
                    split_from_order_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_orders_Customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_restaurant_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "restaurant_tables",
                        principalColumn: "table_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_shifts_shift_id",
                        column: x => x.shift_id,
                        principalTable: "shifts",
                        principalColumn: "shift_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_users_approved_void_by_user_id",
                        column: x => x.approved_void_by_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_users_cashier_user_id",
                        column: x => x.cashier_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_users_void_by_user_id",
                        column: x => x.void_by_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_orders_users_waiter_user_id",
                        column: x => x.waiter_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "stock_count_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stock_count_id = table.Column<int>(type: "integer", nullable: false),
                    inventory_item_id = table.Column<int>(type: "integer", nullable: false),
                    system_quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    counted_quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    variance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_count_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_count_lines_InventoryItems_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "InventoryItems",
                        principalColumn: "InventoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_count_lines_stock_counts_stock_count_id",
                        column: x => x.stock_count_id,
                        principalTable: "stock_counts",
                        principalColumn: "stock_count_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "goods_receipt_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    goods_receipt_id = table.Column<int>(type: "integer", nullable: false),
                    inventory_item_id = table.Column<int>(type: "integer", nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods_receipt_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_goods_receipt_lines_InventoryItems_inventory_item_id",
                        column: x => x.inventory_item_id,
                        principalTable: "InventoryItems",
                        principalColumn: "InventoryItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_goods_receipt_lines_goods_receipts_goods_receipt_id",
                        column: x => x.goods_receipt_id,
                        principalTable: "goods_receipts",
                        principalColumn: "goods_receipt_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_delivery_details",
                columns: table => new
                {
                    order_delivery_details_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    customer_address_id = table.Column<int>(type: "integer", nullable: true),
                    delivery_zone_id = table.Column<int>(type: "integer", nullable: true),
                    address_line = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    distance_km = table.Column<decimal>(type: "numeric", nullable: true),
                    delivery_fee_calculated = table.Column<decimal>(type: "numeric", nullable: false),
                    driver_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    driver_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    estimated_delivery_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    out_for_delivery_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    delivery_notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_delivery_details", x => x.order_delivery_details_id);
                    table.ForeignKey(
                        name: "FK_order_delivery_details_CustomerAddresses_customer_address_id",
                        column: x => x.customer_address_id,
                        principalTable: "CustomerAddresses",
                        principalColumn: "CustomerAddressId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_order_delivery_details_DeliveryZones_delivery_zone_id",
                        column: x => x.delivery_zone_id,
                        principalTable: "DeliveryZones",
                        principalColumn: "delivery_zone_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_order_delivery_details_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_lines",
                columns: table => new
                {
                    order_line_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    menu_item_id = table.Column<int>(type: "integer", nullable: false),
                    menu_item_size_id = table.Column<int>(type: "integer", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    base_unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    modifiers_extra_price = table.Column<decimal>(type: "numeric", nullable: false),
                    effective_unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    line_gross = table.Column<decimal>(type: "numeric", nullable: false),
                    discount_percent = table.Column<decimal>(type: "numeric", nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    line_net = table.Column<decimal>(type: "numeric", nullable: false),
                    notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    kitchen_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    kitchen_station_id = table.Column<int>(type: "integer", nullable: true),
                    sent_to_kitchen_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ready_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    served_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_lines", x => x.order_line_id);
                    table.ForeignKey(
                        name: "FK_order_lines_kitchen_stations_kitchen_station_id",
                        column: x => x.kitchen_station_id,
                        principalTable: "kitchen_stations",
                        principalColumn: "kitchen_station_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_order_lines_menu_item_sizes_menu_item_size_id",
                        column: x => x.menu_item_size_id,
                        principalTable: "menu_item_sizes",
                        principalColumn: "menu_item_size_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_order_lines_menu_items_menu_item_id",
                        column: x => x.menu_item_id,
                        principalTable: "menu_items",
                        principalColumn: "menu_item_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_lines_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_lines_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "order_payments",
                columns: table => new
                {
                    order_payment_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    payment_method_id = table.Column<int>(type: "integer", nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    amount_in_order_currency = table.Column<decimal>(type: "numeric", nullable: false),
                    exchange_rate_to_order_currency = table.Column<decimal>(type: "numeric", nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gift_card_id = table.Column<int>(type: "integer", nullable: true),
                    loyalty_points_used = table.Column<decimal>(type: "numeric", nullable: true),
                    notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_payments", x => x.order_payment_id);
                    table.ForeignKey(
                        name: "FK_order_payments_GiftCards_gift_card_id",
                        column: x => x.gift_card_id,
                        principalTable: "GiftCards",
                        principalColumn: "GiftCardId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_order_payments_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_payments_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_methods",
                        principalColumn: "payment_method_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_payments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_status_history",
                columns: table => new
                {
                    order_status_history_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    old_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    new_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_status_history", x => x.order_status_history_id);
                    table.ForeignKey(
                        name: "FK_order_status_history_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_status_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "order_line_modifiers",
                columns: table => new
                {
                    order_line_modifier_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_line_id = table.Column<int>(type: "integer", nullable: false),
                    modifier_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    extra_price = table.Column<decimal>(type: "numeric", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_line_modifiers", x => x.order_line_modifier_id);
                    table.ForeignKey(
                        name: "FK_order_line_modifiers_modifiers_modifier_id",
                        column: x => x.modifier_id,
                        principalTable: "modifiers",
                        principalColumn: "modifier_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_line_modifiers_order_lines_order_line_id",
                        column: x => x.order_line_id,
                        principalTable: "order_lines",
                        principalColumn: "order_line_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_currency_code",
                table: "InventoryItems",
                column: "currency_code");

            migrationBuilder.CreateIndex(
                name: "IX_approval_rules_company_id",
                table: "approval_rules",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_rules_role_id",
                table: "approval_rules",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_company_id",
                table: "attendance",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendance_employee_id",
                table: "attendance",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipt_lines_goods_receipt_id",
                table: "goods_receipt_lines",
                column: "goods_receipt_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipt_lines_inventory_item_id",
                table: "goods_receipt_lines",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_branch_id",
                table: "goods_receipts",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_purchase_order_id",
                table: "goods_receipts",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_goods_receipts_supplier_id",
                table: "goods_receipts",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCategories_CompanyId",
                table: "InventoryCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCategories_ParentCategoryId",
                table: "InventoryCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_order_delivery_details_customer_address_id",
                table: "order_delivery_details",
                column: "customer_address_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_delivery_details_delivery_zone_id",
                table: "order_delivery_details",
                column: "delivery_zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_delivery_details_order_id",
                table: "order_delivery_details",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_line_modifiers_modifier_id",
                table: "order_line_modifiers",
                column: "modifier_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_modifiers_order_line_id",
                table: "order_line_modifiers",
                column: "order_line_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_created_by_user_id",
                table: "order_lines",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_kitchen_station_id",
                table: "order_lines",
                column: "kitchen_station_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_menu_item_id",
                table: "order_lines",
                column: "menu_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_menu_item_size_id",
                table: "order_lines",
                column: "menu_item_size_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_order_id",
                table: "order_lines",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_gift_card_id",
                table: "order_payments",
                column: "gift_card_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_order_id",
                table: "order_payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_payment_method_id",
                table: "order_payments",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payments_user_id",
                table: "order_payments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_status_history_order_id",
                table: "order_status_history",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_status_history_user_id",
                table: "order_status_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_approved_void_by_user_id",
                table: "orders",
                column: "approved_void_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_branch_id_order_number",
                table: "orders",
                columns: new[] { "branch_id", "order_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_cashier_user_id",
                table: "orders",
                column: "cashier_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_company_id",
                table: "orders",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_shift_id",
                table: "orders",
                column: "shift_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_table_id",
                table: "orders",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_void_by_user_id",
                table: "orders",
                column: "void_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_waiter_user_id",
                table: "orders",
                column: "waiter_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_lines_inventory_item_id",
                table: "purchase_order_lines",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_order_lines_purchase_order_id",
                table: "purchase_order_lines",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_branch_id",
                table: "purchase_orders",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_orders_supplier_id",
                table: "purchase_orders",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_branch_id",
                table: "shifts",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_cashier_user_id",
                table: "shifts",
                column: "cashier_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_shifts_company_id",
                table: "shifts",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_branch_id",
                table: "stock_adjustments",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustments_user_id",
                table: "stock_adjustments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_count_lines_inventory_item_id",
                table: "stock_count_lines",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_count_lines_stock_count_id",
                table: "stock_count_lines",
                column: "stock_count_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_company_id",
                table: "stock_movements",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_inventory_item_id",
                table: "stock_movements",
                column: "inventory_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_UnitConversions_CompanyId",
                table: "UnitConversions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_CompanyId",
                table: "UnitsOfMeasure",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_wastages_company_id",
                table: "wastages",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_wastages_inventory_item_id",
                table: "wastages",
                column: "inventory_item_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_log_branches_BranchId",
                table: "audit_log",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_log_companies_CompanyId",
                table: "audit_log",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_audit_log_users_UserId",
                table: "audit_log",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryZones_branches_branch_id",
                table: "DeliveryZones",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_currencies_currency_code",
                table: "InventoryItems",
                column: "currency_code",
                principalTable: "currencies",
                principalColumn: "currency_code");

            migrationBuilder.AddForeignKey(
                name: "FK_Printers_branches_BranchId",
                table: "Printers",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_customer_id",
                table: "Reservations",
                column: "customer_id",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_branches_branch_id",
                table: "Reservations",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_restaurant_tables_table_id",
                table: "Reservations",
                column: "table_id",
                principalTable: "restaurant_tables",
                principalColumn: "table_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_users_created_by_user_id",
                table: "Reservations",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_system_settings_branches_BranchId",
                table: "system_settings",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_system_settings_companies_CompanyId",
                table: "system_settings",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_system_settings_users_UpdatedByUserId",
                table: "system_settings",
                column: "UpdatedByUserId",
                principalTable: "users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_log_branches_BranchId",
                table: "audit_log");

            migrationBuilder.DropForeignKey(
                name: "FK_audit_log_companies_CompanyId",
                table: "audit_log");

            migrationBuilder.DropForeignKey(
                name: "FK_audit_log_users_UserId",
                table: "audit_log");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryZones_branches_branch_id",
                table: "DeliveryZones");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_currencies_currency_code",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Printers_branches_BranchId",
                table: "Printers");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Customers_customer_id",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_branches_branch_id",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_restaurant_tables_table_id",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_users_created_by_user_id",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_system_settings_branches_BranchId",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_system_settings_companies_CompanyId",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_system_settings_users_UpdatedByUserId",
                table: "system_settings");

            migrationBuilder.DropTable(
                name: "approval_rules");

            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "goods_receipt_lines");

            migrationBuilder.DropTable(
                name: "InventoryCategories");

            migrationBuilder.DropTable(
                name: "order_delivery_details");

            migrationBuilder.DropTable(
                name: "order_line_modifiers");

            migrationBuilder.DropTable(
                name: "order_payments");

            migrationBuilder.DropTable(
                name: "order_status_history");

            migrationBuilder.DropTable(
                name: "purchase_order_lines");

            migrationBuilder.DropTable(
                name: "stock_adjustments");

            migrationBuilder.DropTable(
                name: "stock_count_lines");

            migrationBuilder.DropTable(
                name: "stock_movements");

            migrationBuilder.DropTable(
                name: "UnitConversions");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasure");

            migrationBuilder.DropTable(
                name: "wastages");

            migrationBuilder.DropTable(
                name: "goods_receipts");

            migrationBuilder.DropTable(
                name: "order_lines");

            migrationBuilder.DropTable(
                name: "stock_counts");

            migrationBuilder.DropTable(
                name: "purchase_orders");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "shifts");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_currency_code",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_log",
                table: "audit_log");

            migrationBuilder.DropColumn(
                name: "cost",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "currency_code",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "system_settings",
                newName: "SystemSettings");

            migrationBuilder.RenameTable(
                name: "audit_log",
                newName: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Reservations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Reservations",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "channel",
                table: "Reservations",
                newName: "Channel");

            migrationBuilder.RenameColumn(
                name: "table_id",
                table: "Reservations",
                newName: "TableId");

            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "Reservations",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "reservation_date",
                table: "Reservations",
                newName: "ReservationDate");

            migrationBuilder.RenameColumn(
                name: "party_size",
                table: "Reservations",
                newName: "PartySize");

            migrationBuilder.RenameColumn(
                name: "duration_minutes",
                table: "Reservations",
                newName: "DurationMinutes");

            migrationBuilder.RenameColumn(
                name: "customer_phone",
                table: "Reservations",
                newName: "CustomerPhone");

            migrationBuilder.RenameColumn(
                name: "customer_name",
                table: "Reservations",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "Reservations",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_by_user_id",
                table: "Reservations",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Reservations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "Reservations",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "reservation_id",
                table: "Reservations",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_table_id",
                table: "Reservations",
                newName: "IX_Reservations_TableId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_customer_id",
                table: "Reservations",
                newName: "IX_Reservations_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_created_by_user_id",
                table: "Reservations",
                newName: "IX_Reservations_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_branch_id",
                table: "Reservations",
                newName: "IX_Reservations_BranchId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "DeliveryZones",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "zone_name",
                table: "DeliveryZones",
                newName: "ZoneName");

            migrationBuilder.RenameColumn(
                name: "min_order_amount",
                table: "DeliveryZones",
                newName: "MinOrderAmount");

            migrationBuilder.RenameColumn(
                name: "max_distance_km",
                table: "DeliveryZones",
                newName: "MaxDistanceKm");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "DeliveryZones",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "extra_fee_per_km",
                table: "DeliveryZones",
                newName: "ExtraFeePerKm");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "DeliveryZones",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "base_fee",
                table: "DeliveryZones",
                newName: "BaseFee");

            migrationBuilder.RenameColumn(
                name: "delivery_zone_id",
                table: "DeliveryZones",
                newName: "DeliveryZoneId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryZones_branch_id",
                table: "DeliveryZones",
                newName: "IX_DeliveryZones_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_UpdatedByUserId",
                table: "SystemSettings",
                newName: "IX_SystemSettings_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_CompanyId_BranchId_SettingKey",
                table: "SystemSettings",
                newName: "IX_SystemSettings_CompanyId_BranchId_SettingKey");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_BranchId",
                table: "SystemSettings",
                newName: "IX_SystemSettings_BranchId");

            migrationBuilder.RenameIndex(
                name: "IX_audit_log_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_audit_log_CompanyId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_audit_log_BranchId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_BranchId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Suppliers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Suppliers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Reservations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Printers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Printers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Printers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Benefits",
                table: "LoyaltyTiers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LoyaltyTiers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "LoyaltyTiers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LoyaltyTiers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LoyaltySettings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LoyaltySettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LoyaltySettings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GiftCards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "GiftCards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "DeliveryZones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DeliveryZones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DeliveryZones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemSettings",
                table: "SystemSettings",
                column: "SettingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CompanyId",
                table: "Reservations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Printers_CompanyId",
                table: "Printers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftCards_CreatedByUserId",
                table: "GiftCards",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryZones_CompanyId",
                table: "DeliveryZones",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_branches_BranchId",
                table: "AuditLogs",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_companies_CompanyId",
                table: "AuditLogs",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryZones_branches_BranchId",
                table: "DeliveryZones",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryZones_companies_CompanyId",
                table: "DeliveryZones",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GiftCards_users_CreatedByUserId",
                table: "GiftCards",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Printers_branches_BranchId",
                table: "Printers",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Printers_companies_CompanyId",
                table: "Printers",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Customers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_branches_BranchId",
                table: "Reservations",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_companies_CompanyId",
                table: "Reservations",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_restaurant_tables_TableId",
                table: "Reservations",
                column: "TableId",
                principalTable: "restaurant_tables",
                principalColumn: "table_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_users_CreatedByUserId",
                table: "Reservations",
                column: "CreatedByUserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemSettings_branches_BranchId",
                table: "SystemSettings",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemSettings_companies_CompanyId",
                table: "SystemSettings",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemSettings_users_UpdatedByUserId",
                table: "SystemSettings",
                column: "UpdatedByUserId",
                principalTable: "users",
                principalColumn: "user_id");
        }
    }
}
