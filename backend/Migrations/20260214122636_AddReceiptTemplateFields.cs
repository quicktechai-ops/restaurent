using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptTemplateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approval_rules_companies_company_id",
                table: "approval_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_approval_rules_roles_role_id",
                table: "approval_rules");

            migrationBuilder.DropForeignKey(
                name: "fk_attendance_companies_company_id",
                table: "attendance");

            migrationBuilder.DropForeignKey(
                name: "fk_attendance_employees_employee_id",
                table: "attendance");

            migrationBuilder.DropForeignKey(
                name: "fk_audit_log_branches_branch_id",
                table: "audit_log");

            migrationBuilder.DropForeignKey(
                name: "fk_audit_log_companies_company_id",
                table: "audit_log");

            migrationBuilder.DropForeignKey(
                name: "fk_audit_log_users_user_id",
                table: "audit_log");

            migrationBuilder.DropForeignKey(
                name: "fk_branches_companies_company_id",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "fk_branches_currencies_default_currency_code",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "fk_branches_users_created_by_user_id",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "fk_branches_users_updated_by_user_id",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "fk_categories_branches_branch_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_categories_categories_parent_category_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_categories_companies_company_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_commission_policies_branches_branch_id",
                table: "commission_policies");

            migrationBuilder.DropForeignKey(
                name: "fk_commission_policies_companies_company_id",
                table: "commission_policies");

            migrationBuilder.DropForeignKey(
                name: "fk_companies_subscription_plans_plan_id",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "fk_companies_super_admins_created_by_super_admin_id",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "fk_company_payments_companies_company_id",
                table: "company_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_company_payments_super_admins_recorded_by_super_admin_id",
                table: "company_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_customer_addresses_customers_customer_id",
                table: "customer_addresses");

            migrationBuilder.DropForeignKey(
                name: "fk_customer_addresses_delivery_zones_delivery_zone_id",
                table: "customer_addresses");

            migrationBuilder.DropForeignKey(
                name: "fk_customers_branches_default_branch_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_customers_companies_company_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_delivery_zones_branches_branch_id",
                table: "delivery_zones");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_branches_branch_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_companies_company_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_users_user_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_exchange_rates_companies_company_id",
                table: "exchange_rates");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_card_transactions_gift_cards_gift_card_id",
                table: "gift_card_transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_card_transactions_users_user_id",
                table: "gift_card_transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_cards_branches_branch_issued_id",
                table: "gift_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_cards_companies_company_id",
                table: "gift_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_gift_cards_customers_customer_id",
                table: "gift_cards");

            migrationBuilder.DropForeignKey(
                name: "fk_goods_receipt_lines_goods_receipts_goods_receipt_id",
                table: "goods_receipt_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_goods_receipt_lines_inventory_items_inventory_item_id",
                table: "goods_receipt_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_goods_receipts_branches_branch_id",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "fk_goods_receipts_purchase_orders_purchase_order_id",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "fk_goods_receipts_suppliers_supplier_id",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "fk_inventory_categories_companies_company_id",
                table: "inventory_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_inventory_categories_inventory_categories_parent_category_id",
                table: "inventory_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_inventory_items_companies_company_id",
                table: "inventory_items");

            migrationBuilder.DropForeignKey(
                name: "fk_inventory_items_currencies_currency_code",
                table: "inventory_items");

            migrationBuilder.DropForeignKey(
                name: "fk_kitchen_station_printers_kitchen_stations_kitchen_station_id",
                table: "kitchen_station_printers");

            migrationBuilder.DropForeignKey(
                name: "fk_kitchen_station_printers_printers_printer_id",
                table: "kitchen_station_printers");

            migrationBuilder.DropForeignKey(
                name: "fk_kitchen_stations_branches_branch_id",
                table: "kitchen_stations");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_accounts_customers_customer_id",
                table: "loyalty_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_accounts_loyalty_tiers_loyalty_tier_id",
                table: "loyalty_accounts");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_settings_branches_branch_id",
                table: "loyalty_settings");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_settings_companies_company_id",
                table: "loyalty_settings");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_tiers_companies_company_id",
                table: "loyalty_tiers");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_transactions_loyalty_accounts_loyalty_account_id",
                table: "loyalty_transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_loyalty_transactions_users_user_id",
                table: "loyalty_transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_item_modifiers_menu_items_menu_item_id",
                table: "menu_item_modifiers");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_item_modifiers_modifiers_modifier_id",
                table: "menu_item_modifiers");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_item_sizes_menu_items_menu_item_id",
                table: "menu_item_sizes");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_items_branches_branch_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_items_categories_category_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_items_companies_company_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "fk_menu_items_kitchen_stations_kitchen_station_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "fk_modifiers_branches_branch_id",
                table: "modifiers");

            migrationBuilder.DropForeignKey(
                name: "fk_modifiers_companies_company_id",
                table: "modifiers");

            migrationBuilder.DropForeignKey(
                name: "fk_order_delivery_details_customer_addresses_customer_address_",
                table: "order_delivery_details");

            migrationBuilder.DropForeignKey(
                name: "fk_order_delivery_details_delivery_zones_delivery_zone_id",
                table: "order_delivery_details");

            migrationBuilder.DropForeignKey(
                name: "fk_order_delivery_details_orders_order_id",
                table: "order_delivery_details");

            migrationBuilder.DropForeignKey(
                name: "fk_order_line_modifiers_modifiers_modifier_id",
                table: "order_line_modifiers");

            migrationBuilder.DropForeignKey(
                name: "fk_order_line_modifiers_order_lines_order_line_id",
                table: "order_line_modifiers");

            migrationBuilder.DropForeignKey(
                name: "fk_order_lines_kitchen_stations_kitchen_station_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_order_lines_menu_item_sizes_menu_item_size_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_order_lines_menu_items_menu_item_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_order_lines_orders_order_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_order_lines_users_created_by_user_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_order_payments_gift_cards_gift_card_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_order_payments_orders_order_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_order_payments_payment_methods_payment_method_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_order_payments_users_user_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "fk_order_status_history_orders_order_id",
                table: "order_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_order_status_history_users_user_id",
                table: "order_status_history");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_branches_branch_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_companies_company_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_customers_customer_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_restaurant_tables_table_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_shifts_shift_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_approved_void_by_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_cashier_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_void_by_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_orders_users_waiter_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_payment_methods_companies_company_id",
                table: "payment_methods");

            migrationBuilder.DropForeignKey(
                name: "fk_printers_branches_branch_id",
                table: "printers");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_lines_inventory_items_inventory_item_id",
                table: "purchase_order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_order_lines_purchase_orders_purchase_order_id",
                table: "purchase_order_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_orders_branches_branch_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_purchase_orders_suppliers_supplier_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_receipt_templates_branches_branch_id",
                table: "receipt_templates");

            migrationBuilder.DropForeignKey(
                name: "fk_receipt_templates_companies_company_id",
                table: "receipt_templates");

            migrationBuilder.DropForeignKey(
                name: "fk_recipe_ingredients_inventory_items_inventory_item_id",
                table: "recipe_ingredients");

            migrationBuilder.DropForeignKey(
                name: "fk_recipe_ingredients_recipes_recipe_id",
                table: "recipe_ingredients");

            migrationBuilder.DropForeignKey(
                name: "fk_recipes_companies_company_id",
                table: "recipes");

            migrationBuilder.DropForeignKey(
                name: "fk_recipes_menu_item_sizes_menu_item_size_id",
                table: "recipes");

            migrationBuilder.DropForeignKey(
                name: "fk_recipes_menu_items_menu_item_id",
                table: "recipes");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_deposits_payment_methods_payment_method_id",
                table: "reservation_deposits");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_deposits_reservations_reservation_id",
                table: "reservation_deposits");

            migrationBuilder.DropForeignKey(
                name: "fk_reservation_deposits_users_user_id",
                table: "reservation_deposits");

            migrationBuilder.DropForeignKey(
                name: "fk_reservations_branches_branch_id",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "fk_reservations_customers_customer_id",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "fk_reservations_restaurant_tables_table_id",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "fk_reservations_users_created_by_user_id",
                table: "reservations");

            migrationBuilder.DropForeignKey(
                name: "fk_restaurant_tables_branches_branch_id",
                table: "restaurant_tables");

            migrationBuilder.DropForeignKey(
                name: "fk_role_permissions_permissions_permission_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "fk_role_permissions_roles_role_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "fk_roles_branches_branch_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "fk_roles_companies_company_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_branches_branch_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_companies_company_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_users_cashier_user_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_adjustments_branches_branch_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_adjustments_inventory_items_inventory_item_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_adjustments_users_user_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_count_lines_inventory_items_inventory_item_id",
                table: "stock_count_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_count_lines_stock_counts_stock_count_id",
                table: "stock_count_lines");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_movements_companies_company_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_movements_inventory_items_inventory_item_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_companies_company_id",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "fk_system_settings_branches_branch_id",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "fk_system_settings_companies_company_id",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "fk_system_settings_users_updated_by_user_id",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "fk_unit_conversions_companies_company_id",
                table: "unit_conversions");

            migrationBuilder.DropForeignKey(
                name: "fk_units_of_measure_companies_company_id",
                table: "units_of_measure");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_users_assigned_by_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_users_branches_default_branch_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_users_companies_company_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_wastages_companies_company_id",
                table: "wastages");

            migrationBuilder.DropForeignKey(
                name: "fk_wastages_inventory_items_inventory_item_id",
                table: "wastages");

            migrationBuilder.DropPrimaryKey(
                name: "pk_wastages",
                table: "wastages");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_suppliers",
                table: "suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_super_admins",
                table: "super_admins");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subscription_plans",
                table: "subscription_plans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stock_movements",
                table: "stock_movements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stock_counts",
                table: "stock_counts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stock_count_lines",
                table: "stock_count_lines");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stock_adjustments",
                table: "stock_adjustments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shifts",
                table: "shifts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_role_permissions",
                table: "role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_restaurant_tables",
                table: "restaurant_tables");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservations",
                table: "reservations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_recipes",
                table: "recipes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_purchase_orders",
                table: "purchase_orders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_purchase_order_lines",
                table: "purchase_order_lines");

            migrationBuilder.DropPrimaryKey(
                name: "pk_printers",
                table: "printers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_permissions",
                table: "permissions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_payment_methods",
                table: "payment_methods");

            migrationBuilder.DropPrimaryKey(
                name: "pk_orders",
                table: "orders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_status_history",
                table: "order_status_history");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_payments",
                table: "order_payments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_lines",
                table: "order_lines");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_line_modifiers",
                table: "order_line_modifiers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_order_delivery_details",
                table: "order_delivery_details");

            migrationBuilder.DropPrimaryKey(
                name: "pk_modifiers",
                table: "modifiers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_menu_items",
                table: "menu_items");

            migrationBuilder.DropPrimaryKey(
                name: "pk_menu_item_sizes",
                table: "menu_item_sizes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_menu_item_modifiers",
                table: "menu_item_modifiers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_loyalty_accounts",
                table: "loyalty_accounts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_kitchen_stations",
                table: "kitchen_stations");

            migrationBuilder.DropPrimaryKey(
                name: "pk_goods_receipts",
                table: "goods_receipts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_goods_receipt_lines",
                table: "goods_receipt_lines");

            migrationBuilder.DropPrimaryKey(
                name: "pk_gift_cards",
                table: "gift_cards");

            migrationBuilder.DropPrimaryKey(
                name: "pk_gift_card_transactions",
                table: "gift_card_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "pk_customers",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "pk_company_payments",
                table: "company_payments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_companies",
                table: "companies");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_branches",
                table: "branches");

            migrationBuilder.DropPrimaryKey(
                name: "pk_audit_log",
                table: "audit_log");

            migrationBuilder.DropPrimaryKey(
                name: "pk_attendance",
                table: "attendance");

            migrationBuilder.DropPrimaryKey(
                name: "pk_approval_rules",
                table: "approval_rules");

            migrationBuilder.DropPrimaryKey(
                name: "pk_units_of_measure",
                table: "units_of_measure");

            migrationBuilder.DropPrimaryKey(
                name: "pk_unit_conversions",
                table: "unit_conversions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reservation_deposits",
                table: "reservation_deposits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_recipe_ingredients",
                table: "recipe_ingredients");

            migrationBuilder.DropPrimaryKey(
                name: "pk_receipt_templates",
                table: "receipt_templates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_loyalty_transactions",
                table: "loyalty_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_loyalty_tiers",
                table: "loyalty_tiers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_loyalty_settings",
                table: "loyalty_settings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_kitchen_station_printers",
                table: "kitchen_station_printers");

            migrationBuilder.DropPrimaryKey(
                name: "pk_inventory_items",
                table: "inventory_items");

            migrationBuilder.DropPrimaryKey(
                name: "pk_inventory_categories",
                table: "inventory_categories");

            migrationBuilder.DropPrimaryKey(
                name: "pk_exchange_rates",
                table: "exchange_rates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_delivery_zones",
                table: "delivery_zones");

            migrationBuilder.DropPrimaryKey(
                name: "pk_customer_addresses",
                table: "customer_addresses");

            migrationBuilder.DropPrimaryKey(
                name: "pk_commission_policies",
                table: "commission_policies");

            migrationBuilder.RenameTable(
                name: "suppliers",
                newName: "Suppliers");

            migrationBuilder.RenameTable(
                name: "reservations",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "recipes",
                newName: "Recipes");

            migrationBuilder.RenameTable(
                name: "printers",
                newName: "Printers");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "customers",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "units_of_measure",
                newName: "UnitsOfMeasure");

            migrationBuilder.RenameTable(
                name: "unit_conversions",
                newName: "UnitConversions");

            migrationBuilder.RenameTable(
                name: "reservation_deposits",
                newName: "ReservationDeposits");

            migrationBuilder.RenameTable(
                name: "recipe_ingredients",
                newName: "RecipeIngredients");

            migrationBuilder.RenameTable(
                name: "receipt_templates",
                newName: "ReceiptTemplates");

            migrationBuilder.RenameTable(
                name: "loyalty_transactions",
                newName: "LoyaltyTransactions");

            migrationBuilder.RenameTable(
                name: "loyalty_tiers",
                newName: "LoyaltyTiers");

            migrationBuilder.RenameTable(
                name: "loyalty_settings",
                newName: "LoyaltySettings");

            migrationBuilder.RenameTable(
                name: "kitchen_station_printers",
                newName: "KitchenStationPrinters");

            migrationBuilder.RenameTable(
                name: "inventory_items",
                newName: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "inventory_categories",
                newName: "InventoryCategories");

            migrationBuilder.RenameTable(
                name: "exchange_rates",
                newName: "ExchangeRates");

            migrationBuilder.RenameTable(
                name: "delivery_zones",
                newName: "DeliveryZones");

            migrationBuilder.RenameTable(
                name: "customer_addresses",
                newName: "CustomerAddresses");

            migrationBuilder.RenameTable(
                name: "commission_policies",
                newName: "CommissionPolicies");

            migrationBuilder.RenameIndex(
                name: "ix_wastages_inventory_item_id",
                table: "wastages",
                newName: "IX_wastages_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_wastages_company_id",
                table: "wastages",
                newName: "IX_wastages_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_default_branch_id",
                table: "users",
                newName: "IX_users_default_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_company_id_username",
                table: "users",
                newName: "IX_users_company_id_username");

            migrationBuilder.RenameIndex(
                name: "ix_user_roles_user_id",
                table: "user_roles",
                newName: "IX_user_roles_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                newName: "IX_user_roles_role_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_roles_assigned_by_user_id",
                table: "user_roles",
                newName: "IX_user_roles_assigned_by_user_id");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "system_settings",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "updated_by_user_id",
                table: "system_settings",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "system_settings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "setting_value",
                table: "system_settings",
                newName: "SettingValue");

            migrationBuilder.RenameColumn(
                name: "setting_type",
                table: "system_settings",
                newName: "SettingType");

            migrationBuilder.RenameColumn(
                name: "setting_key",
                table: "system_settings",
                newName: "SettingKey");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "system_settings",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "system_settings",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "setting_id",
                table: "system_settings",
                newName: "SettingId");

            migrationBuilder.RenameIndex(
                name: "ix_system_settings_updated_by_user_id",
                table: "system_settings",
                newName: "IX_system_settings_UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "ix_system_settings_company_id_branch_id_setting_key",
                table: "system_settings",
                newName: "IX_system_settings_CompanyId_BranchId_SettingKey");

            migrationBuilder.RenameIndex(
                name: "ix_system_settings_branch_id",
                table: "system_settings",
                newName: "IX_system_settings_BranchId");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Suppliers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Suppliers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Suppliers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Suppliers",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "payment_terms",
                table: "Suppliers",
                newName: "PaymentTerms");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Suppliers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "contact_person",
                table: "Suppliers",
                newName: "ContactPerson");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Suppliers",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "supplier_id",
                table: "Suppliers",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "ix_suppliers_company_id",
                table: "Suppliers",
                newName: "IX_Suppliers_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_super_admins_username",
                table: "super_admins",
                newName: "IX_super_admins_username");

            migrationBuilder.RenameIndex(
                name: "ix_stock_movements_inventory_item_id",
                table: "stock_movements",
                newName: "IX_stock_movements_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_stock_movements_company_id",
                table: "stock_movements",
                newName: "IX_stock_movements_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_stock_count_lines_stock_count_id",
                table: "stock_count_lines",
                newName: "IX_stock_count_lines_stock_count_id");

            migrationBuilder.RenameIndex(
                name: "ix_stock_count_lines_inventory_item_id",
                table: "stock_count_lines",
                newName: "IX_stock_count_lines_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_stock_adjustments_user_id",
                table: "stock_adjustments",
                newName: "IX_stock_adjustments_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_stock_adjustments_inventory_item_id",
                table: "stock_adjustments",
                newName: "IX_stock_adjustments_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_stock_adjustments_branch_id",
                table: "stock_adjustments",
                newName: "IX_stock_adjustments_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_company_id",
                table: "shifts",
                newName: "IX_shifts_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_cashier_user_id",
                table: "shifts",
                newName: "IX_shifts_cashier_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_shifts_branch_id",
                table: "shifts",
                newName: "IX_shifts_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_roles_company_id",
                table: "roles",
                newName: "IX_roles_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_roles_branch_id",
                table: "roles",
                newName: "IX_roles_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permissions",
                newName: "IX_role_permissions_role_id");

            migrationBuilder.RenameIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                newName: "IX_role_permissions_permission_id");

            migrationBuilder.RenameIndex(
                name: "ix_restaurant_tables_branch_id",
                table: "restaurant_tables",
                newName: "IX_restaurant_tables_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_reservations_table_id",
                table: "Reservations",
                newName: "IX_Reservations_table_id");

            migrationBuilder.RenameIndex(
                name: "ix_reservations_customer_id",
                table: "Reservations",
                newName: "IX_Reservations_customer_id");

            migrationBuilder.RenameIndex(
                name: "ix_reservations_created_by_user_id",
                table: "Reservations",
                newName: "IX_Reservations_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_reservations_branch_id",
                table: "Reservations",
                newName: "IX_Reservations_branch_id");

            migrationBuilder.RenameColumn(
                name: "yield_quantity",
                table: "Recipes",
                newName: "YieldQuantity");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Recipes",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "menu_item_size_id",
                table: "Recipes",
                newName: "MenuItemSizeId");

            migrationBuilder.RenameColumn(
                name: "menu_item_id",
                table: "Recipes",
                newName: "MenuItemId");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Recipes",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Recipes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Recipes",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "recipe_id",
                table: "Recipes",
                newName: "RecipeId");

            migrationBuilder.RenameIndex(
                name: "ix_recipes_menu_item_size_id",
                table: "Recipes",
                newName: "IX_Recipes_MenuItemSizeId");

            migrationBuilder.RenameIndex(
                name: "ix_recipes_menu_item_id_menu_item_size_id",
                table: "Recipes",
                newName: "IX_Recipes_MenuItemId_MenuItemSizeId");

            migrationBuilder.RenameIndex(
                name: "ix_recipes_company_id",
                table: "Recipes",
                newName: "IX_Recipes_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_orders_supplier_id",
                table: "purchase_orders",
                newName: "IX_purchase_orders_supplier_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_orders_branch_id",
                table: "purchase_orders",
                newName: "IX_purchase_orders_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_lines_purchase_order_id",
                table: "purchase_order_lines",
                newName: "IX_purchase_order_lines_purchase_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_purchase_order_lines_inventory_item_id",
                table: "purchase_order_lines",
                newName: "IX_purchase_order_lines_inventory_item_id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Printers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "printer_type",
                table: "Printers",
                newName: "PrinterType");

            migrationBuilder.RenameColumn(
                name: "paper_width",
                table: "Printers",
                newName: "PaperWidth");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "Printers",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Printers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "connection_type",
                table: "Printers",
                newName: "ConnectionType");

            migrationBuilder.RenameColumn(
                name: "connection_string",
                table: "Printers",
                newName: "ConnectionString");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "Printers",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "printer_id",
                table: "Printers",
                newName: "PrinterId");

            migrationBuilder.RenameIndex(
                name: "ix_printers_branch_id",
                table: "Printers",
                newName: "IX_Printers_BranchId");

            migrationBuilder.RenameIndex(
                name: "ix_payment_methods_company_id",
                table: "payment_methods",
                newName: "IX_payment_methods_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_waiter_user_id",
                table: "orders",
                newName: "IX_orders_waiter_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_void_by_user_id",
                table: "orders",
                newName: "IX_orders_void_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_table_id",
                table: "orders",
                newName: "IX_orders_table_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_shift_id",
                table: "orders",
                newName: "IX_orders_shift_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_customer_id",
                table: "orders",
                newName: "IX_orders_customer_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_company_id",
                table: "orders",
                newName: "IX_orders_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_cashier_user_id",
                table: "orders",
                newName: "IX_orders_cashier_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_branch_id_order_number",
                table: "orders",
                newName: "IX_orders_branch_id_order_number");

            migrationBuilder.RenameIndex(
                name: "ix_orders_approved_void_by_user_id",
                table: "orders",
                newName: "IX_orders_approved_void_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_status_history_user_id",
                table: "order_status_history",
                newName: "IX_order_status_history_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_status_history_order_id",
                table: "order_status_history",
                newName: "IX_order_status_history_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_payments_user_id",
                table: "order_payments",
                newName: "IX_order_payments_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_payments_payment_method_id",
                table: "order_payments",
                newName: "IX_order_payments_payment_method_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_payments_order_id",
                table: "order_payments",
                newName: "IX_order_payments_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_payments_gift_card_id",
                table: "order_payments",
                newName: "IX_order_payments_gift_card_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_lines_order_id",
                table: "order_lines",
                newName: "IX_order_lines_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_lines_menu_item_size_id",
                table: "order_lines",
                newName: "IX_order_lines_menu_item_size_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_lines_menu_item_id",
                table: "order_lines",
                newName: "IX_order_lines_menu_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_lines_kitchen_station_id",
                table: "order_lines",
                newName: "IX_order_lines_kitchen_station_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_lines_created_by_user_id",
                table: "order_lines",
                newName: "IX_order_lines_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_line_modifiers_order_line_id",
                table: "order_line_modifiers",
                newName: "IX_order_line_modifiers_order_line_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_line_modifiers_modifier_id",
                table: "order_line_modifiers",
                newName: "IX_order_line_modifiers_modifier_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_delivery_details_order_id",
                table: "order_delivery_details",
                newName: "IX_order_delivery_details_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_delivery_details_delivery_zone_id",
                table: "order_delivery_details",
                newName: "IX_order_delivery_details_delivery_zone_id");

            migrationBuilder.RenameIndex(
                name: "ix_order_delivery_details_customer_address_id",
                table: "order_delivery_details",
                newName: "IX_order_delivery_details_customer_address_id");

            migrationBuilder.RenameIndex(
                name: "ix_modifiers_company_id",
                table: "modifiers",
                newName: "IX_modifiers_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_modifiers_branch_id",
                table: "modifiers",
                newName: "IX_modifiers_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_items_kitchen_station_id",
                table: "menu_items",
                newName: "IX_menu_items_kitchen_station_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_items_company_id",
                table: "menu_items",
                newName: "IX_menu_items_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_items_category_id",
                table: "menu_items",
                newName: "IX_menu_items_category_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_items_branch_id",
                table: "menu_items",
                newName: "IX_menu_items_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_item_sizes_menu_item_id",
                table: "menu_item_sizes",
                newName: "IX_menu_item_sizes_menu_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_item_modifiers_modifier_id",
                table: "menu_item_modifiers",
                newName: "IX_menu_item_modifiers_modifier_id");

            migrationBuilder.RenameIndex(
                name: "ix_menu_item_modifiers_menu_item_id",
                table: "menu_item_modifiers",
                newName: "IX_menu_item_modifiers_menu_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_accounts_loyalty_tier_id",
                table: "loyalty_accounts",
                newName: "IX_loyalty_accounts_loyalty_tier_id");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_accounts_customer_id",
                table: "loyalty_accounts",
                newName: "IX_loyalty_accounts_customer_id");

            migrationBuilder.RenameIndex(
                name: "ix_kitchen_stations_branch_id",
                table: "kitchen_stations",
                newName: "IX_kitchen_stations_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_goods_receipts_supplier_id",
                table: "goods_receipts",
                newName: "IX_goods_receipts_supplier_id");

            migrationBuilder.RenameIndex(
                name: "ix_goods_receipts_purchase_order_id",
                table: "goods_receipts",
                newName: "IX_goods_receipts_purchase_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_goods_receipts_branch_id",
                table: "goods_receipts",
                newName: "IX_goods_receipts_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_goods_receipt_lines_inventory_item_id",
                table: "goods_receipt_lines",
                newName: "IX_goods_receipt_lines_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "ix_goods_receipt_lines_goods_receipt_id",
                table: "goods_receipt_lines",
                newName: "IX_goods_receipt_lines_goods_receipt_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_cards_customer_id",
                table: "gift_cards",
                newName: "IX_gift_cards_customer_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_cards_company_id_gift_card_number",
                table: "gift_cards",
                newName: "IX_gift_cards_company_id_gift_card_number");

            migrationBuilder.RenameIndex(
                name: "ix_gift_cards_branch_issued_id",
                table: "gift_cards",
                newName: "IX_gift_cards_branch_issued_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_card_transactions_user_id",
                table: "gift_card_transactions",
                newName: "IX_gift_card_transactions_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_gift_card_transactions_gift_card_id",
                table: "gift_card_transactions",
                newName: "IX_gift_card_transactions_gift_card_id");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "Employees",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Employees",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Employees",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Employees",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Employees",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Employees",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "hire_date",
                table: "Employees",
                newName: "HireDate");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Employees",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Employees",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Employees",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "Employees",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "base_salary",
                table: "Employees",
                newName: "BaseSalary");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "Employees",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "ix_employees_user_id",
                table: "Employees",
                newName: "IX_Employees_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_employees_company_id",
                table: "Employees",
                newName: "IX_Employees_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_employees_branch_id",
                table: "Employees",
                newName: "IX_Employees_BranchId");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Customers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Customers",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Customers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Customers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Customers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Customers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "default_currency_code",
                table: "Customers",
                newName: "DefaultCurrencyCode");

            migrationBuilder.RenameColumn(
                name: "default_branch_id",
                table: "Customers",
                newName: "DefaultBranchId");

            migrationBuilder.RenameColumn(
                name: "customer_code",
                table: "Customers",
                newName: "CustomerCode");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Customers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "Customers",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "Customers",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "ix_customers_default_branch_id",
                table: "Customers",
                newName: "IX_Customers_DefaultBranchId");

            migrationBuilder.RenameIndex(
                name: "ix_customers_company_id_customer_code",
                table: "Customers",
                newName: "IX_Customers_CompanyId_CustomerCode");

            migrationBuilder.RenameIndex(
                name: "ix_company_payments_recorded_by_super_admin_id",
                table: "company_payments",
                newName: "IX_company_payments_recorded_by_super_admin_id");

            migrationBuilder.RenameIndex(
                name: "ix_company_payments_company_id",
                table: "company_payments",
                newName: "IX_company_payments_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_companies_username",
                table: "companies",
                newName: "IX_companies_username");

            migrationBuilder.RenameIndex(
                name: "ix_companies_plan_id",
                table: "companies",
                newName: "IX_companies_plan_id");

            migrationBuilder.RenameIndex(
                name: "ix_companies_created_by_super_admin_id",
                table: "companies",
                newName: "IX_companies_created_by_super_admin_id");

            migrationBuilder.RenameIndex(
                name: "ix_categories_parent_category_id",
                table: "categories",
                newName: "IX_categories_parent_category_id");

            migrationBuilder.RenameIndex(
                name: "ix_categories_company_id",
                table: "categories",
                newName: "IX_categories_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_categories_branch_id",
                table: "categories",
                newName: "IX_categories_branch_id");

            migrationBuilder.RenameIndex(
                name: "ix_branches_updated_by_user_id",
                table: "branches",
                newName: "IX_branches_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_branches_default_currency_code",
                table: "branches",
                newName: "IX_branches_default_currency_code");

            migrationBuilder.RenameIndex(
                name: "ix_branches_created_by_user_id",
                table: "branches",
                newName: "IX_branches_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_branches_company_id_code",
                table: "branches",
                newName: "IX_branches_company_id_code");

            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "audit_log",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "details",
                table: "audit_log",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "audit_log",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "entity_name",
                table: "audit_log",
                newName: "EntityName");

            migrationBuilder.RenameColumn(
                name: "entity_id",
                table: "audit_log",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "audit_log",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "audit_log",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "action_type",
                table: "audit_log",
                newName: "ActionType");

            migrationBuilder.RenameColumn(
                name: "audit_log_id",
                table: "audit_log",
                newName: "AuditLogId");

            migrationBuilder.RenameIndex(
                name: "ix_audit_log_user_id",
                table: "audit_log",
                newName: "IX_audit_log_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_audit_log_company_id",
                table: "audit_log",
                newName: "IX_audit_log_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_audit_log_branch_id",
                table: "audit_log",
                newName: "IX_audit_log_BranchId");

            migrationBuilder.RenameIndex(
                name: "ix_attendance_employee_id",
                table: "attendance",
                newName: "IX_attendance_employee_id");

            migrationBuilder.RenameIndex(
                name: "ix_attendance_company_id",
                table: "attendance",
                newName: "IX_attendance_company_id");

            migrationBuilder.RenameIndex(
                name: "ix_approval_rules_role_id",
                table: "approval_rules",
                newName: "IX_approval_rules_role_id");

            migrationBuilder.RenameIndex(
                name: "ix_approval_rules_company_id",
                table: "approval_rules",
                newName: "IX_approval_rules_company_id");

            migrationBuilder.RenameColumn(
                name: "symbol",
                table: "UnitsOfMeasure",
                newName: "Symbol");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "UnitsOfMeasure",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "UnitsOfMeasure",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "unit_group",
                table: "UnitsOfMeasure",
                newName: "UnitGroup");

            migrationBuilder.RenameColumn(
                name: "sort_order",
                table: "UnitsOfMeasure",
                newName: "SortOrder");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "UnitsOfMeasure",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "UnitsOfMeasure",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "UnitsOfMeasure",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "unit_id",
                table: "UnitsOfMeasure",
                newName: "UnitId");

            migrationBuilder.RenameIndex(
                name: "ix_units_of_measure_company_id",
                table: "UnitsOfMeasure",
                newName: "IX_UnitsOfMeasure_CompanyId");

            migrationBuilder.RenameColumn(
                name: "to_unit_code",
                table: "UnitConversions",
                newName: "ToUnitCode");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "UnitConversions",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "from_unit_code",
                table: "UnitConversions",
                newName: "FromUnitCode");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "UnitConversions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "conversion_factor",
                table: "UnitConversions",
                newName: "ConversionFactor");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "UnitConversions",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "conversion_id",
                table: "UnitConversions",
                newName: "ConversionId");

            migrationBuilder.RenameIndex(
                name: "ix_unit_conversions_company_id",
                table: "UnitConversions",
                newName: "IX_UnitConversions_CompanyId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "ReservationDeposits",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "ReservationDeposits",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "ReservationDeposits",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "reservation_id",
                table: "ReservationDeposits",
                newName: "ReservationId");

            migrationBuilder.RenameColumn(
                name: "refunded_at",
                table: "ReservationDeposits",
                newName: "RefundedAt");

            migrationBuilder.RenameColumn(
                name: "payment_method_id",
                table: "ReservationDeposits",
                newName: "PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "paid_at",
                table: "ReservationDeposits",
                newName: "PaidAt");

            migrationBuilder.RenameColumn(
                name: "currency_code",
                table: "ReservationDeposits",
                newName: "CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "reservation_deposit_id",
                table: "ReservationDeposits",
                newName: "ReservationDepositId");

            migrationBuilder.RenameIndex(
                name: "ix_reservation_deposits_user_id",
                table: "ReservationDeposits",
                newName: "IX_ReservationDeposits_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_reservation_deposits_reservation_id",
                table: "ReservationDeposits",
                newName: "IX_ReservationDeposits_ReservationId");

            migrationBuilder.RenameIndex(
                name: "ix_reservation_deposits_payment_method_id",
                table: "ReservationDeposits",
                newName: "IX_ReservationDeposits_PaymentMethodId");

            migrationBuilder.RenameColumn(
                name: "recipe_id",
                table: "RecipeIngredients",
                newName: "RecipeId");

            migrationBuilder.RenameColumn(
                name: "quantity_per_yield",
                table: "RecipeIngredients",
                newName: "QuantityPerYield");

            migrationBuilder.RenameColumn(
                name: "inventory_item_id",
                table: "RecipeIngredients",
                newName: "InventoryItemId");

            migrationBuilder.RenameColumn(
                name: "recipe_ingredient_id",
                table: "RecipeIngredients",
                newName: "RecipeIngredientId");

            migrationBuilder.RenameIndex(
                name: "ix_recipe_ingredients_recipe_id",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_RecipeId");

            migrationBuilder.RenameIndex(
                name: "ix_recipe_ingredients_inventory_item_id",
                table: "RecipeIngredients",
                newName: "IX_RecipeIngredients_InventoryItemId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ReceiptTemplates",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "language",
                table: "ReceiptTemplates",
                newName: "Language");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ReceiptTemplates",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "template_type",
                table: "ReceiptTemplates",
                newName: "TemplateType");

            migrationBuilder.RenameColumn(
                name: "show_logo",
                table: "ReceiptTemplates",
                newName: "ShowLogo");

            migrationBuilder.RenameColumn(
                name: "show_barcode",
                table: "ReceiptTemplates",
                newName: "ShowBarcode");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "ReceiptTemplates",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "ReceiptTemplates",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "header_text",
                table: "ReceiptTemplates",
                newName: "HeaderText");

            migrationBuilder.RenameColumn(
                name: "footer_text",
                table: "ReceiptTemplates",
                newName: "FooterText");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ReceiptTemplates",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "ReceiptTemplates",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "ReceiptTemplates",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "receipt_template_id",
                table: "ReceiptTemplates",
                newName: "ReceiptTemplateId");

            migrationBuilder.RenameIndex(
                name: "ix_receipt_templates_company_id",
                table: "ReceiptTemplates",
                newName: "IX_ReceiptTemplates_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_receipt_templates_branch_id",
                table: "ReceiptTemplates",
                newName: "IX_ReceiptTemplates_BranchId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "LoyaltyTransactions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "LoyaltyTransactions",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "LoyaltyTransactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "transaction_date",
                table: "LoyaltyTransactions",
                newName: "TransactionDate");

            migrationBuilder.RenameColumn(
                name: "points_change",
                table: "LoyaltyTransactions",
                newName: "PointsChange");

            migrationBuilder.RenameColumn(
                name: "points_before",
                table: "LoyaltyTransactions",
                newName: "PointsBefore");

            migrationBuilder.RenameColumn(
                name: "points_after",
                table: "LoyaltyTransactions",
                newName: "PointsAfter");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "LoyaltyTransactions",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "loyalty_account_id",
                table: "LoyaltyTransactions",
                newName: "LoyaltyAccountId");

            migrationBuilder.RenameColumn(
                name: "loyalty_transaction_id",
                table: "LoyaltyTransactions",
                newName: "LoyaltyTransactionId");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_transactions_user_id",
                table: "LoyaltyTransactions",
                newName: "IX_LoyaltyTransactions_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_transactions_loyalty_account_id",
                table: "LoyaltyTransactions",
                newName: "IX_LoyaltyTransactions_LoyaltyAccountId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "LoyaltyTiers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "tier_discount_percent",
                table: "LoyaltyTiers",
                newName: "TierDiscountPercent");

            migrationBuilder.RenameColumn(
                name: "min_total_spent",
                table: "LoyaltyTiers",
                newName: "MinTotalSpent");

            migrationBuilder.RenameColumn(
                name: "min_total_points",
                table: "LoyaltyTiers",
                newName: "MinTotalPoints");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "LoyaltyTiers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "LoyaltyTiers",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "loyalty_tier_id",
                table: "LoyaltyTiers",
                newName: "LoyaltyTierId");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_tiers_company_id",
                table: "LoyaltyTiers",
                newName: "IX_LoyaltyTiers_CompanyId");

            migrationBuilder.RenameColumn(
                name: "points_redeem_value",
                table: "LoyaltySettings",
                newName: "PointsRedeemValue");

            migrationBuilder.RenameColumn(
                name: "points_per_amount",
                table: "LoyaltySettings",
                newName: "PointsPerAmount");

            migrationBuilder.RenameColumn(
                name: "points_expiry_months",
                table: "LoyaltySettings",
                newName: "PointsExpiryMonths");

            migrationBuilder.RenameColumn(
                name: "earn_on_net_before_tax",
                table: "LoyaltySettings",
                newName: "EarnOnNetBeforeTax");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "LoyaltySettings",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "LoyaltySettings",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "amount_unit",
                table: "LoyaltySettings",
                newName: "AmountUnit");

            migrationBuilder.RenameColumn(
                name: "loyalty_settings_id",
                table: "LoyaltySettings",
                newName: "LoyaltySettingsId");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_settings_company_id",
                table: "LoyaltySettings",
                newName: "IX_LoyaltySettings_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_loyalty_settings_branch_id",
                table: "LoyaltySettings",
                newName: "IX_LoyaltySettings_BranchId");

            migrationBuilder.RenameColumn(
                name: "printer_id",
                table: "KitchenStationPrinters",
                newName: "PrinterId");

            migrationBuilder.RenameColumn(
                name: "kitchen_station_id",
                table: "KitchenStationPrinters",
                newName: "KitchenStationId");

            migrationBuilder.RenameColumn(
                name: "kitchen_station_printer_id",
                table: "KitchenStationPrinters",
                newName: "KitchenStationPrinterId");

            migrationBuilder.RenameIndex(
                name: "ix_kitchen_station_printers_printer_id",
                table: "KitchenStationPrinters",
                newName: "IX_KitchenStationPrinters_PrinterId");

            migrationBuilder.RenameIndex(
                name: "ix_kitchen_station_printers_kitchen_station_id_printer_id",
                table: "KitchenStationPrinters",
                newName: "IX_KitchenStationPrinters_KitchenStationId_PrinterId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "InventoryItems",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "InventoryItems",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "InventoryItems",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "InventoryItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "unit_of_measure",
                table: "InventoryItems",
                newName: "UnitOfMeasure");

            migrationBuilder.RenameColumn(
                name: "reorder_qty",
                table: "InventoryItems",
                newName: "ReorderQty");

            migrationBuilder.RenameColumn(
                name: "min_level",
                table: "InventoryItems",
                newName: "MinLevel");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "InventoryItems",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "InventoryItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "cost_method",
                table: "InventoryItems",
                newName: "CostMethod");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "InventoryItems",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "inventory_item_id",
                table: "InventoryItems",
                newName: "InventoryItemId");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_items_currency_code",
                table: "InventoryItems",
                newName: "IX_InventoryItems_currency_code");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_items_company_id_code",
                table: "InventoryItems",
                newName: "IX_InventoryItems_CompanyId_Code");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "InventoryCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "InventoryCategories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "sort_order",
                table: "InventoryCategories",
                newName: "SortOrder");

            migrationBuilder.RenameColumn(
                name: "parent_category_id",
                table: "InventoryCategories",
                newName: "ParentCategoryId");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "InventoryCategories",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "InventoryCategories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "InventoryCategories",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "InventoryCategories",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_categories_parent_category_id",
                table: "InventoryCategories",
                newName: "IX_InventoryCategories_ParentCategoryId");

            migrationBuilder.RenameIndex(
                name: "ix_inventory_categories_company_id",
                table: "InventoryCategories",
                newName: "IX_InventoryCategories_CompanyId");

            migrationBuilder.RenameColumn(
                name: "rate",
                table: "ExchangeRates",
                newName: "Rate");

            migrationBuilder.RenameColumn(
                name: "valid_to",
                table: "ExchangeRates",
                newName: "ValidTo");

            migrationBuilder.RenameColumn(
                name: "valid_from",
                table: "ExchangeRates",
                newName: "ValidFrom");

            migrationBuilder.RenameColumn(
                name: "foreign_currency_code",
                table: "ExchangeRates",
                newName: "ForeignCurrencyCode");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "ExchangeRates",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "base_currency_code",
                table: "ExchangeRates",
                newName: "BaseCurrencyCode");

            migrationBuilder.RenameColumn(
                name: "exchange_rate_id",
                table: "ExchangeRates",
                newName: "ExchangeRateId");

            migrationBuilder.RenameIndex(
                name: "ix_exchange_rates_company_id",
                table: "ExchangeRates",
                newName: "IX_ExchangeRates_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_delivery_zones_branch_id",
                table: "DeliveryZones",
                newName: "IX_DeliveryZones_branch_id");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "CustomerAddresses",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "CustomerAddresses",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "label",
                table: "CustomerAddresses",
                newName: "Label");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "CustomerAddresses",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "area",
                table: "CustomerAddresses",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "CustomerAddresses",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "delivery_zone_id",
                table: "CustomerAddresses",
                newName: "DeliveryZoneId");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "CustomerAddresses",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "address_line2",
                table: "CustomerAddresses",
                newName: "AddressLine2");

            migrationBuilder.RenameColumn(
                name: "address_line1",
                table: "CustomerAddresses",
                newName: "AddressLine1");

            migrationBuilder.RenameColumn(
                name: "customer_address_id",
                table: "CustomerAddresses",
                newName: "CustomerAddressId");

            migrationBuilder.RenameIndex(
                name: "ix_customer_addresses_delivery_zone_id",
                table: "CustomerAddresses",
                newName: "IX_CustomerAddresses_DeliveryZoneId");

            migrationBuilder.RenameIndex(
                name: "ix_customer_addresses_customer_id",
                table: "CustomerAddresses",
                newName: "IX_CustomerAddresses_CustomerId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "CommissionPolicies",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "sales_percent",
                table: "CommissionPolicies",
                newName: "SalesPercent");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "CommissionPolicies",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "fixed_per_invoice",
                table: "CommissionPolicies",
                newName: "FixedPerInvoice");

            migrationBuilder.RenameColumn(
                name: "exclude_discounted_invoices",
                table: "CommissionPolicies",
                newName: "ExcludeDiscountedInvoices");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "CommissionPolicies",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "company_id",
                table: "CommissionPolicies",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "branch_id",
                table: "CommissionPolicies",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "apply_on_net_before_tax",
                table: "CommissionPolicies",
                newName: "ApplyOnNetBeforeTax");

            migrationBuilder.RenameColumn(
                name: "commission_policy_id",
                table: "CommissionPolicies",
                newName: "CommissionPolicyId");

            migrationBuilder.RenameIndex(
                name: "ix_commission_policies_company_id",
                table: "CommissionPolicies",
                newName: "IX_CommissionPolicies_CompanyId");

            migrationBuilder.RenameIndex(
                name: "ix_commission_policies_branch_id",
                table: "CommissionPolicies",
                newName: "IX_CommissionPolicies_BranchId");

            migrationBuilder.AddColumn<string>(
                name: "FooterText2",
                table: "ReceiptTemplates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FooterTextAr",
                table: "ReceiptTemplates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FooterTextAr2",
                table: "ReceiptTemplates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaperSize",
                table: "ReceiptTemplates",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowAddress",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowCustomer",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowDate",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowDiscountDetails",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowItemCode",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowModifiers",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOrderNumber",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOrderType",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPaymentDetails",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPaymentMethod",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPhone",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTable",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTaxNumber",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowTips",
                table: "ReceiptTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_wastages",
                table: "wastages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                column: "user_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_system_settings",
                table: "system_settings",
                column: "SettingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_super_admins",
                table: "super_admins",
                column: "super_admin_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription_plans",
                table: "subscription_plans",
                column: "plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stock_movements",
                table: "stock_movements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stock_counts",
                table: "stock_counts",
                column: "stock_count_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stock_count_lines",
                table: "stock_count_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stock_adjustments",
                table: "stock_adjustments",
                column: "stock_adjustment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shifts",
                table: "shifts",
                column: "shift_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "role_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_permissions",
                table: "role_permissions",
                column: "role_permission_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_restaurant_tables",
                table: "restaurant_tables",
                column: "table_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "reservation_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "RecipeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_purchase_orders",
                table: "purchase_orders",
                column: "purchase_order_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_purchase_order_lines",
                table: "purchase_order_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Printers",
                table: "Printers",
                column: "PrinterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_permissions",
                table: "permissions",
                column: "permission_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment_methods",
                table: "payment_methods",
                column: "payment_method_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orders",
                table: "orders",
                column: "order_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_status_history",
                table: "order_status_history",
                column: "order_status_history_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_payments",
                table: "order_payments",
                column: "order_payment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_lines",
                table: "order_lines",
                column: "order_line_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_line_modifiers",
                table: "order_line_modifiers",
                column: "order_line_modifier_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_delivery_details",
                table: "order_delivery_details",
                column: "order_delivery_details_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_modifiers",
                table: "modifiers",
                column: "modifier_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_menu_items",
                table: "menu_items",
                column: "menu_item_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_menu_item_sizes",
                table: "menu_item_sizes",
                column: "menu_item_size_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_menu_item_modifiers",
                table: "menu_item_modifiers",
                column: "menu_item_modifier_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_loyalty_accounts",
                table: "loyalty_accounts",
                column: "loyalty_account_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kitchen_stations",
                table: "kitchen_stations",
                column: "kitchen_station_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_goods_receipts",
                table: "goods_receipts",
                column: "goods_receipt_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_goods_receipt_lines",
                table: "goods_receipt_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gift_cards",
                table: "gift_cards",
                column: "gift_card_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gift_card_transactions",
                table: "gift_card_transactions",
                column: "gift_card_transaction_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_currencies",
                table: "currencies",
                column: "currency_code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_company_payments",
                table: "company_payments",
                column: "payment_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_companies",
                table: "companies",
                column: "company_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "category_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_branches",
                table: "branches",
                column: "branch_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_log",
                table: "audit_log",
                column: "AuditLogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_attendance",
                table: "attendance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_approval_rules",
                table: "approval_rules",
                column: "approval_rule_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitsOfMeasure",
                table: "UnitsOfMeasure",
                column: "UnitId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitConversions",
                table: "UnitConversions",
                column: "ConversionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationDeposits",
                table: "ReservationDeposits",
                column: "ReservationDepositId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecipeIngredients",
                table: "RecipeIngredients",
                column: "RecipeIngredientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptTemplates",
                table: "ReceiptTemplates",
                column: "ReceiptTemplateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoyaltyTransactions",
                table: "LoyaltyTransactions",
                column: "LoyaltyTransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoyaltyTiers",
                table: "LoyaltyTiers",
                column: "LoyaltyTierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoyaltySettings",
                table: "LoyaltySettings",
                column: "LoyaltySettingsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KitchenStationPrinters",
                table: "KitchenStationPrinters",
                column: "KitchenStationPrinterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems",
                column: "InventoryItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryCategories",
                table: "InventoryCategories",
                column: "CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExchangeRates",
                table: "ExchangeRates",
                column: "ExchangeRateId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryZones",
                table: "DeliveryZones",
                column: "delivery_zone_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses",
                column: "CustomerAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionPolicies",
                table: "CommissionPolicies",
                column: "CommissionPolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_approval_rules_companies_company_id",
                table: "approval_rules",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_approval_rules_roles_role_id",
                table: "approval_rules",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id");

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_Employees_employee_id",
                table: "attendance",
                column: "employee_id",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_companies_company_id",
                table: "attendance",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_branches_companies_company_id",
                table: "branches",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_branches_currencies_default_currency_code",
                table: "branches",
                column: "default_currency_code",
                principalTable: "currencies",
                principalColumn: "currency_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_branches_users_created_by_user_id",
                table: "branches",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_branches_users_updated_by_user_id",
                table: "branches",
                column: "updated_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_branches_branch_id",
                table: "categories",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_parent_category_id",
                table: "categories",
                column: "parent_category_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_companies_company_id",
                table: "categories",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionPolicies_branches_BranchId",
                table: "CommissionPolicies",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionPolicies_companies_CompanyId",
                table: "CommissionPolicies",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_companies_subscription_plans_plan_id",
                table: "companies",
                column: "plan_id",
                principalTable: "subscription_plans",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_companies_super_admins_created_by_super_admin_id",
                table: "companies",
                column: "created_by_super_admin_id",
                principalTable: "super_admins",
                principalColumn: "super_admin_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_company_payments_companies_company_id",
                table: "company_payments",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_company_payments_super_admins_recorded_by_super_admin_id",
                table: "company_payments",
                column: "recorded_by_super_admin_id",
                principalTable: "super_admins",
                principalColumn: "super_admin_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Customers_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_DeliveryZones_DeliveryZoneId",
                table: "CustomerAddresses",
                column: "DeliveryZoneId",
                principalTable: "DeliveryZones",
                principalColumn: "delivery_zone_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_branches_DefaultBranchId",
                table: "Customers",
                column: "DefaultBranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_companies_CompanyId",
                table: "Customers",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryZones_branches_branch_id",
                table: "DeliveryZones",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_branches_BranchId",
                table: "Employees",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_companies_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_users_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRates_companies_CompanyId",
                table: "ExchangeRates",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gift_card_transactions_gift_cards_gift_card_id",
                table: "gift_card_transactions",
                column: "gift_card_id",
                principalTable: "gift_cards",
                principalColumn: "gift_card_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gift_card_transactions_users_user_id",
                table: "gift_card_transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_gift_cards_Customers_customer_id",
                table: "gift_cards",
                column: "customer_id",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_gift_cards_branches_branch_issued_id",
                table: "gift_cards",
                column: "branch_issued_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_gift_cards_companies_company_id",
                table: "gift_cards",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipt_lines_InventoryItems_inventory_item_id",
                table: "goods_receipt_lines",
                column: "inventory_item_id",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipt_lines_goods_receipts_goods_receipt_id",
                table: "goods_receipt_lines",
                column: "goods_receipt_id",
                principalTable: "goods_receipts",
                principalColumn: "goods_receipt_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_Suppliers_supplier_id",
                table: "goods_receipts",
                column: "supplier_id",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_branches_branch_id",
                table: "goods_receipts",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_goods_receipts_purchase_orders_purchase_order_id",
                table: "goods_receipts",
                column: "purchase_order_id",
                principalTable: "purchase_orders",
                principalColumn: "purchase_order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCategories_InventoryCategories_ParentCategoryId",
                table: "InventoryCategories",
                column: "ParentCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCategories_companies_CompanyId",
                table: "InventoryCategories",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_companies_CompanyId",
                table: "InventoryItems",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_currencies_currency_code",
                table: "InventoryItems",
                column: "currency_code",
                principalTable: "currencies",
                principalColumn: "currency_code");

            migrationBuilder.AddForeignKey(
                name: "FK_kitchen_stations_branches_branch_id",
                table: "kitchen_stations",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KitchenStationPrinters_Printers_PrinterId",
                table: "KitchenStationPrinters",
                column: "PrinterId",
                principalTable: "Printers",
                principalColumn: "PrinterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KitchenStationPrinters_kitchen_stations_KitchenStationId",
                table: "KitchenStationPrinters",
                column: "KitchenStationId",
                principalTable: "kitchen_stations",
                principalColumn: "kitchen_station_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loyalty_accounts_Customers_customer_id",
                table: "loyalty_accounts",
                column: "customer_id",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_loyalty_accounts_LoyaltyTiers_loyalty_tier_id",
                table: "loyalty_accounts",
                column: "loyalty_tier_id",
                principalTable: "LoyaltyTiers",
                principalColumn: "LoyaltyTierId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltySettings_branches_BranchId",
                table: "LoyaltySettings",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltySettings_companies_CompanyId",
                table: "LoyaltySettings",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyTiers_companies_CompanyId",
                table: "LoyaltyTiers",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyTransactions_loyalty_accounts_LoyaltyAccountId",
                table: "LoyaltyTransactions",
                column: "LoyaltyAccountId",
                principalTable: "loyalty_accounts",
                principalColumn: "loyalty_account_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyTransactions_users_UserId",
                table: "LoyaltyTransactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_menu_item_modifiers_menu_items_menu_item_id",
                table: "menu_item_modifiers",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_item_modifiers_modifiers_modifier_id",
                table: "menu_item_modifiers",
                column: "modifier_id",
                principalTable: "modifiers",
                principalColumn: "modifier_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_item_sizes_menu_items_menu_item_id",
                table: "menu_item_sizes",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_items_branches_branch_id",
                table: "menu_items",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_menu_items_categories_category_id",
                table: "menu_items",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_items_companies_company_id",
                table: "menu_items",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_menu_items_kitchen_stations_kitchen_station_id",
                table: "menu_items",
                column: "kitchen_station_id",
                principalTable: "kitchen_stations",
                principalColumn: "kitchen_station_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_modifiers_branches_branch_id",
                table: "modifiers",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_modifiers_companies_company_id",
                table: "modifiers",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_delivery_details_CustomerAddresses_customer_address_id",
                table: "order_delivery_details",
                column: "customer_address_id",
                principalTable: "CustomerAddresses",
                principalColumn: "CustomerAddressId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_delivery_details_DeliveryZones_delivery_zone_id",
                table: "order_delivery_details",
                column: "delivery_zone_id",
                principalTable: "DeliveryZones",
                principalColumn: "delivery_zone_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_delivery_details_orders_order_id",
                table: "order_delivery_details",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_line_modifiers_modifiers_modifier_id",
                table: "order_line_modifiers",
                column: "modifier_id",
                principalTable: "modifiers",
                principalColumn: "modifier_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_line_modifiers_order_lines_order_line_id",
                table: "order_line_modifiers",
                column: "order_line_id",
                principalTable: "order_lines",
                principalColumn: "order_line_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_lines_kitchen_stations_kitchen_station_id",
                table: "order_lines",
                column: "kitchen_station_id",
                principalTable: "kitchen_stations",
                principalColumn: "kitchen_station_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_lines_menu_item_sizes_menu_item_size_id",
                table: "order_lines",
                column: "menu_item_size_id",
                principalTable: "menu_item_sizes",
                principalColumn: "menu_item_size_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_lines_menu_items_menu_item_id",
                table: "order_lines",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_lines_orders_order_id",
                table: "order_lines",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_lines_users_created_by_user_id",
                table: "order_lines",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_payments_gift_cards_gift_card_id",
                table: "order_payments",
                column: "gift_card_id",
                principalTable: "gift_cards",
                principalColumn: "gift_card_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_payments_orders_order_id",
                table: "order_payments",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_payments_payment_methods_payment_method_id",
                table: "order_payments",
                column: "payment_method_id",
                principalTable: "payment_methods",
                principalColumn: "payment_method_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_payments_users_user_id",
                table: "order_payments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_status_history_orders_order_id",
                table: "order_status_history",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_status_history_users_user_id",
                table: "order_status_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_Customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_branches_branch_id",
                table: "orders",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_companies_company_id",
                table: "orders",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_restaurant_tables_table_id",
                table: "orders",
                column: "table_id",
                principalTable: "restaurant_tables",
                principalColumn: "table_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_shifts_shift_id",
                table: "orders",
                column: "shift_id",
                principalTable: "shifts",
                principalColumn: "shift_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_approved_void_by_user_id",
                table: "orders",
                column: "approved_void_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_cashier_user_id",
                table: "orders",
                column: "cashier_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_void_by_user_id",
                table: "orders",
                column: "void_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_users_waiter_user_id",
                table: "orders",
                column: "waiter_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_payment_methods_companies_company_id",
                table: "payment_methods",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Printers_branches_BranchId",
                table: "Printers",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_order_lines_InventoryItems_inventory_item_id",
                table: "purchase_order_lines",
                column: "inventory_item_id",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_order_lines_purchase_orders_purchase_order_id",
                table: "purchase_order_lines",
                column: "purchase_order_id",
                principalTable: "purchase_orders",
                principalColumn: "purchase_order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_Suppliers_supplier_id",
                table: "purchase_orders",
                column: "supplier_id",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_branches_branch_id",
                table: "purchase_orders",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptTemplates_branches_BranchId",
                table: "ReceiptTemplates",
                column: "BranchId",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptTemplates_companies_CompanyId",
                table: "ReceiptTemplates",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_InventoryItems_InventoryItemId",
                table: "RecipeIngredients",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_companies_CompanyId",
                table: "Recipes",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_menu_item_sizes_MenuItemSizeId",
                table: "Recipes",
                column: "MenuItemSizeId",
                principalTable: "menu_item_sizes",
                principalColumn: "menu_item_size_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_menu_items_MenuItemId",
                table: "Recipes",
                column: "MenuItemId",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationDeposits_Reservations_ReservationId",
                table: "ReservationDeposits",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "reservation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationDeposits_payment_methods_PaymentMethodId",
                table: "ReservationDeposits",
                column: "PaymentMethodId",
                principalTable: "payment_methods",
                principalColumn: "payment_method_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationDeposits_users_UserId",
                table: "ReservationDeposits",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id");

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
                name: "FK_restaurant_tables_branches_branch_id",
                table: "restaurant_tables",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "permission_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_role_permissions_roles_role_id",
                table: "role_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_branches_branch_id",
                table: "roles",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_companies_company_id",
                table: "roles",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shifts_branches_branch_id",
                table: "shifts",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_shifts_companies_company_id",
                table: "shifts",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shifts_users_cashier_user_id",
                table: "shifts",
                column: "cashier_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustments_InventoryItems_inventory_item_id",
                table: "stock_adjustments",
                column: "inventory_item_id",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustments_branches_branch_id",
                table: "stock_adjustments",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_adjustments_users_user_id",
                table: "stock_adjustments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_count_lines_InventoryItems_inventory_item_id",
                table: "stock_count_lines",
                column: "inventory_item_id",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_count_lines_stock_counts_stock_count_id",
                table: "stock_count_lines",
                column: "stock_count_id",
                principalTable: "stock_counts",
                principalColumn: "stock_count_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_movements_InventoryItems_inventory_item_id",
                table: "stock_movements",
                column: "inventory_item_id",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_movements_companies_company_id",
                table: "stock_movements",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_companies_CompanyId",
                table: "Suppliers",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_UnitConversions_companies_CompanyId",
                table: "UnitConversions",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitsOfMeasure_companies_CompanyId",
                table: "UnitsOfMeasure",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_assigned_by_user_id",
                table: "user_roles",
                column: "assigned_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_branches_default_branch_id",
                table: "users",
                column: "default_branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_companies_company_id",
                table: "users",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wastages_InventoryItems_inventory_item_id",
                table: "wastages",
                column: "inventory_item_id",
                principalTable: "InventoryItems",
                principalColumn: "InventoryItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_wastages_companies_company_id",
                table: "wastages",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_approval_rules_companies_company_id",
                table: "approval_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_approval_rules_roles_role_id",
                table: "approval_rules");

            migrationBuilder.DropForeignKey(
                name: "FK_attendance_Employees_employee_id",
                table: "attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_attendance_companies_company_id",
                table: "attendance");

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
                name: "FK_branches_companies_company_id",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "FK_branches_currencies_default_currency_code",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "FK_branches_users_created_by_user_id",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "FK_branches_users_updated_by_user_id",
                table: "branches");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_branches_branch_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_parent_category_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_companies_company_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionPolicies_branches_BranchId",
                table: "CommissionPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_CommissionPolicies_companies_CompanyId",
                table: "CommissionPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_companies_subscription_plans_plan_id",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "FK_companies_super_admins_created_by_super_admin_id",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "FK_company_payments_companies_company_id",
                table: "company_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_company_payments_super_admins_recorded_by_super_admin_id",
                table: "company_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Customers_CustomerId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_DeliveryZones_DeliveryZoneId",
                table: "CustomerAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_branches_DefaultBranchId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_companies_CompanyId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryZones_branches_branch_id",
                table: "DeliveryZones");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_branches_BranchId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_companies_CompanyId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_users_UserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRates_companies_CompanyId",
                table: "ExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_gift_card_transactions_gift_cards_gift_card_id",
                table: "gift_card_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_gift_card_transactions_users_user_id",
                table: "gift_card_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_gift_cards_Customers_customer_id",
                table: "gift_cards");

            migrationBuilder.DropForeignKey(
                name: "FK_gift_cards_branches_branch_issued_id",
                table: "gift_cards");

            migrationBuilder.DropForeignKey(
                name: "FK_gift_cards_companies_company_id",
                table: "gift_cards");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipt_lines_InventoryItems_inventory_item_id",
                table: "goods_receipt_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipt_lines_goods_receipts_goods_receipt_id",
                table: "goods_receipt_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_Suppliers_supplier_id",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_branches_branch_id",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_goods_receipts_purchase_orders_purchase_order_id",
                table: "goods_receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCategories_InventoryCategories_ParentCategoryId",
                table: "InventoryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCategories_companies_CompanyId",
                table: "InventoryCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_companies_CompanyId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_currencies_currency_code",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_kitchen_stations_branches_branch_id",
                table: "kitchen_stations");

            migrationBuilder.DropForeignKey(
                name: "FK_KitchenStationPrinters_Printers_PrinterId",
                table: "KitchenStationPrinters");

            migrationBuilder.DropForeignKey(
                name: "FK_KitchenStationPrinters_kitchen_stations_KitchenStationId",
                table: "KitchenStationPrinters");

            migrationBuilder.DropForeignKey(
                name: "FK_loyalty_accounts_Customers_customer_id",
                table: "loyalty_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_loyalty_accounts_LoyaltyTiers_loyalty_tier_id",
                table: "loyalty_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltySettings_branches_BranchId",
                table: "LoyaltySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltySettings_companies_CompanyId",
                table: "LoyaltySettings");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyTiers_companies_CompanyId",
                table: "LoyaltyTiers");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyTransactions_loyalty_accounts_LoyaltyAccountId",
                table: "LoyaltyTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyTransactions_users_UserId",
                table: "LoyaltyTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_item_modifiers_menu_items_menu_item_id",
                table: "menu_item_modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_item_modifiers_modifiers_modifier_id",
                table: "menu_item_modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_item_sizes_menu_items_menu_item_id",
                table: "menu_item_sizes");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_items_branches_branch_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_items_categories_category_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_items_companies_company_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "FK_menu_items_kitchen_stations_kitchen_station_id",
                table: "menu_items");

            migrationBuilder.DropForeignKey(
                name: "FK_modifiers_branches_branch_id",
                table: "modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_modifiers_companies_company_id",
                table: "modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_order_delivery_details_CustomerAddresses_customer_address_id",
                table: "order_delivery_details");

            migrationBuilder.DropForeignKey(
                name: "FK_order_delivery_details_DeliveryZones_delivery_zone_id",
                table: "order_delivery_details");

            migrationBuilder.DropForeignKey(
                name: "FK_order_delivery_details_orders_order_id",
                table: "order_delivery_details");

            migrationBuilder.DropForeignKey(
                name: "FK_order_line_modifiers_modifiers_modifier_id",
                table: "order_line_modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_order_line_modifiers_order_lines_order_line_id",
                table: "order_line_modifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_order_lines_kitchen_stations_kitchen_station_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_order_lines_menu_item_sizes_menu_item_size_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_order_lines_menu_items_menu_item_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_order_lines_orders_order_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_order_lines_users_created_by_user_id",
                table: "order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_order_payments_gift_cards_gift_card_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_order_payments_orders_order_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_order_payments_payment_methods_payment_method_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_order_payments_users_user_id",
                table: "order_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_order_status_history_orders_order_id",
                table: "order_status_history");

            migrationBuilder.DropForeignKey(
                name: "FK_order_status_history_users_user_id",
                table: "order_status_history");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_Customers_customer_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_branches_branch_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_companies_company_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_restaurant_tables_table_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_shifts_shift_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_approved_void_by_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_cashier_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_void_by_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_users_waiter_user_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_methods_companies_company_id",
                table: "payment_methods");

            migrationBuilder.DropForeignKey(
                name: "FK_Printers_branches_BranchId",
                table: "Printers");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_order_lines_InventoryItems_inventory_item_id",
                table: "purchase_order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_order_lines_purchase_orders_purchase_order_id",
                table: "purchase_order_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_Suppliers_supplier_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_branches_branch_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptTemplates_branches_BranchId",
                table: "ReceiptTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptTemplates_companies_CompanyId",
                table: "ReceiptTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_InventoryItems_InventoryItemId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_Recipes_RecipeId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_companies_CompanyId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_menu_item_sizes_MenuItemSizeId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_menu_items_MenuItemId",
                table: "Recipes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationDeposits_Reservations_ReservationId",
                table: "ReservationDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationDeposits_payment_methods_PaymentMethodId",
                table: "ReservationDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationDeposits_users_UserId",
                table: "ReservationDeposits");

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
                name: "FK_restaurant_tables_branches_branch_id",
                table: "restaurant_tables");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_permissions_permission_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_permissions_roles_role_id",
                table: "role_permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_branches_branch_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_roles_companies_company_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_shifts_branches_branch_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_shifts_companies_company_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_shifts_users_cashier_user_id",
                table: "shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustments_InventoryItems_inventory_item_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustments_branches_branch_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_adjustments_users_user_id",
                table: "stock_adjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_count_lines_InventoryItems_inventory_item_id",
                table: "stock_count_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_count_lines_stock_counts_stock_count_id",
                table: "stock_count_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_movements_InventoryItems_inventory_item_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_movements_companies_company_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_companies_CompanyId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_system_settings_branches_BranchId",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_system_settings_companies_CompanyId",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_system_settings_users_UpdatedByUserId",
                table: "system_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitConversions_companies_CompanyId",
                table: "UnitConversions");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitsOfMeasure_companies_CompanyId",
                table: "UnitsOfMeasure");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_assigned_by_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_branches_default_branch_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_companies_company_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_wastages_InventoryItems_inventory_item_id",
                table: "wastages");

            migrationBuilder.DropForeignKey(
                name: "FK_wastages_companies_company_id",
                table: "wastages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_wastages",
                table: "wastages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_super_admins",
                table: "super_admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription_plans",
                table: "subscription_plans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stock_movements",
                table: "stock_movements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stock_counts",
                table: "stock_counts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stock_count_lines",
                table: "stock_count_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stock_adjustments",
                table: "stock_adjustments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shifts",
                table: "shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_permissions",
                table: "role_permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_restaurant_tables",
                table: "restaurant_tables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_purchase_orders",
                table: "purchase_orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_purchase_order_lines",
                table: "purchase_order_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Printers",
                table: "Printers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_permissions",
                table: "permissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment_methods",
                table: "payment_methods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orders",
                table: "orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_status_history",
                table: "order_status_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_payments",
                table: "order_payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_lines",
                table: "order_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_line_modifiers",
                table: "order_line_modifiers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_delivery_details",
                table: "order_delivery_details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_modifiers",
                table: "modifiers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_menu_items",
                table: "menu_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_menu_item_sizes",
                table: "menu_item_sizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_menu_item_modifiers",
                table: "menu_item_modifiers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_loyalty_accounts",
                table: "loyalty_accounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_kitchen_stations",
                table: "kitchen_stations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_goods_receipts",
                table: "goods_receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_goods_receipt_lines",
                table: "goods_receipt_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gift_cards",
                table: "gift_cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gift_card_transactions",
                table: "gift_card_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_currencies",
                table: "currencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_company_payments",
                table: "company_payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_companies",
                table: "companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_branches",
                table: "branches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_log",
                table: "audit_log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_attendance",
                table: "attendance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_approval_rules",
                table: "approval_rules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitsOfMeasure",
                table: "UnitsOfMeasure");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitConversions",
                table: "UnitConversions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationDeposits",
                table: "ReservationDeposits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecipeIngredients",
                table: "RecipeIngredients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptTemplates",
                table: "ReceiptTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoyaltyTransactions",
                table: "LoyaltyTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoyaltyTiers",
                table: "LoyaltyTiers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoyaltySettings",
                table: "LoyaltySettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KitchenStationPrinters",
                table: "KitchenStationPrinters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryItems",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryCategories",
                table: "InventoryCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExchangeRates",
                table: "ExchangeRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryZones",
                table: "DeliveryZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionPolicies",
                table: "CommissionPolicies");

            migrationBuilder.DropColumn(
                name: "FooterText2",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "FooterTextAr",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "FooterTextAr2",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "PaperSize",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowAddress",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowCustomer",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowDate",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowDiscountDetails",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowItemCode",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowModifiers",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowOrderNumber",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowOrderType",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowPaymentDetails",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowPaymentMethod",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowPhone",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowTable",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowTaxNumber",
                table: "ReceiptTemplates");

            migrationBuilder.DropColumn(
                name: "ShowTips",
                table: "ReceiptTemplates");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "suppliers");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "reservations");

            migrationBuilder.RenameTable(
                name: "Recipes",
                newName: "recipes");

            migrationBuilder.RenameTable(
                name: "Printers",
                newName: "printers");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "customers");

            migrationBuilder.RenameTable(
                name: "UnitsOfMeasure",
                newName: "units_of_measure");

            migrationBuilder.RenameTable(
                name: "UnitConversions",
                newName: "unit_conversions");

            migrationBuilder.RenameTable(
                name: "ReservationDeposits",
                newName: "reservation_deposits");

            migrationBuilder.RenameTable(
                name: "RecipeIngredients",
                newName: "recipe_ingredients");

            migrationBuilder.RenameTable(
                name: "ReceiptTemplates",
                newName: "receipt_templates");

            migrationBuilder.RenameTable(
                name: "LoyaltyTransactions",
                newName: "loyalty_transactions");

            migrationBuilder.RenameTable(
                name: "LoyaltyTiers",
                newName: "loyalty_tiers");

            migrationBuilder.RenameTable(
                name: "LoyaltySettings",
                newName: "loyalty_settings");

            migrationBuilder.RenameTable(
                name: "KitchenStationPrinters",
                newName: "kitchen_station_printers");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                newName: "inventory_items");

            migrationBuilder.RenameTable(
                name: "InventoryCategories",
                newName: "inventory_categories");

            migrationBuilder.RenameTable(
                name: "ExchangeRates",
                newName: "exchange_rates");

            migrationBuilder.RenameTable(
                name: "DeliveryZones",
                newName: "delivery_zones");

            migrationBuilder.RenameTable(
                name: "CustomerAddresses",
                newName: "customer_addresses");

            migrationBuilder.RenameTable(
                name: "CommissionPolicies",
                newName: "commission_policies");

            migrationBuilder.RenameIndex(
                name: "IX_wastages_inventory_item_id",
                table: "wastages",
                newName: "ix_wastages_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_wastages_company_id",
                table: "wastages",
                newName: "ix_wastages_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_default_branch_id",
                table: "users",
                newName: "ix_users_default_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_company_id_username",
                table: "users",
                newName: "ix_users_company_id_username");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_user_id",
                table: "user_roles",
                newName: "ix_user_roles_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                newName: "ix_user_roles_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_assigned_by_user_id",
                table: "user_roles",
                newName: "ix_user_roles_assigned_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "system_settings",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "system_settings",
                newName: "updated_by_user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "system_settings",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SettingValue",
                table: "system_settings",
                newName: "setting_value");

            migrationBuilder.RenameColumn(
                name: "SettingType",
                table: "system_settings",
                newName: "setting_type");

            migrationBuilder.RenameColumn(
                name: "SettingKey",
                table: "system_settings",
                newName: "setting_key");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "system_settings",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "system_settings",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "SettingId",
                table: "system_settings",
                newName: "setting_id");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_UpdatedByUserId",
                table: "system_settings",
                newName: "ix_system_settings_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_CompanyId_BranchId_SettingKey",
                table: "system_settings",
                newName: "ix_system_settings_company_id_branch_id_setting_key");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_BranchId",
                table: "system_settings",
                newName: "ix_system_settings_branch_id");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "suppliers",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "suppliers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "suppliers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "suppliers",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "PaymentTerms",
                table: "suppliers",
                newName: "payment_terms");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "suppliers",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ContactPerson",
                table: "suppliers",
                newName: "contact_person");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "suppliers",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "suppliers",
                newName: "supplier_id");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_CompanyId",
                table: "suppliers",
                newName: "ix_suppliers_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_super_admins_username",
                table: "super_admins",
                newName: "ix_super_admins_username");

            migrationBuilder.RenameIndex(
                name: "IX_stock_movements_inventory_item_id",
                table: "stock_movements",
                newName: "ix_stock_movements_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_stock_movements_company_id",
                table: "stock_movements",
                newName: "ix_stock_movements_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_stock_count_lines_stock_count_id",
                table: "stock_count_lines",
                newName: "ix_stock_count_lines_stock_count_id");

            migrationBuilder.RenameIndex(
                name: "IX_stock_count_lines_inventory_item_id",
                table: "stock_count_lines",
                newName: "ix_stock_count_lines_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_stock_adjustments_user_id",
                table: "stock_adjustments",
                newName: "ix_stock_adjustments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_stock_adjustments_inventory_item_id",
                table: "stock_adjustments",
                newName: "ix_stock_adjustments_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_stock_adjustments_branch_id",
                table: "stock_adjustments",
                newName: "ix_stock_adjustments_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_shifts_company_id",
                table: "shifts",
                newName: "ix_shifts_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_shifts_cashier_user_id",
                table: "shifts",
                newName: "ix_shifts_cashier_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_shifts_branch_id",
                table: "shifts",
                newName: "ix_shifts_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_roles_company_id",
                table: "roles",
                newName: "ix_roles_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_roles_branch_id",
                table: "roles",
                newName: "ix_roles_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_permissions_role_id",
                table: "role_permissions",
                newName: "ix_role_permissions_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_role_permissions_permission_id",
                table: "role_permissions",
                newName: "ix_role_permissions_permission_id");

            migrationBuilder.RenameIndex(
                name: "IX_restaurant_tables_branch_id",
                table: "restaurant_tables",
                newName: "ix_restaurant_tables_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_table_id",
                table: "reservations",
                newName: "ix_reservations_table_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_customer_id",
                table: "reservations",
                newName: "ix_reservations_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_created_by_user_id",
                table: "reservations",
                newName: "ix_reservations_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_branch_id",
                table: "reservations",
                newName: "ix_reservations_branch_id");

            migrationBuilder.RenameColumn(
                name: "YieldQuantity",
                table: "recipes",
                newName: "yield_quantity");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "recipes",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "MenuItemSizeId",
                table: "recipes",
                newName: "menu_item_size_id");

            migrationBuilder.RenameColumn(
                name: "MenuItemId",
                table: "recipes",
                newName: "menu_item_id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "recipes",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "recipes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "recipes",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "recipes",
                newName: "recipe_id");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_MenuItemSizeId",
                table: "recipes",
                newName: "ix_recipes_menu_item_size_id");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_MenuItemId_MenuItemSizeId",
                table: "recipes",
                newName: "ix_recipes_menu_item_id_menu_item_size_id");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_CompanyId",
                table: "recipes",
                newName: "ix_recipes_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_purchase_orders_supplier_id",
                table: "purchase_orders",
                newName: "ix_purchase_orders_supplier_id");

            migrationBuilder.RenameIndex(
                name: "IX_purchase_orders_branch_id",
                table: "purchase_orders",
                newName: "ix_purchase_orders_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_purchase_order_lines_purchase_order_id",
                table: "purchase_order_lines",
                newName: "ix_purchase_order_lines_purchase_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_purchase_order_lines_inventory_item_id",
                table: "purchase_order_lines",
                newName: "ix_purchase_order_lines_inventory_item_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "printers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "PrinterType",
                table: "printers",
                newName: "printer_type");

            migrationBuilder.RenameColumn(
                name: "PaperWidth",
                table: "printers",
                newName: "paper_width");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "printers",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "printers",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ConnectionType",
                table: "printers",
                newName: "connection_type");

            migrationBuilder.RenameColumn(
                name: "ConnectionString",
                table: "printers",
                newName: "connection_string");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "printers",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "PrinterId",
                table: "printers",
                newName: "printer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Printers_BranchId",
                table: "printers",
                newName: "ix_printers_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_payment_methods_company_id",
                table: "payment_methods",
                newName: "ix_payment_methods_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_waiter_user_id",
                table: "orders",
                newName: "ix_orders_waiter_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_void_by_user_id",
                table: "orders",
                newName: "ix_orders_void_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_table_id",
                table: "orders",
                newName: "ix_orders_table_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_shift_id",
                table: "orders",
                newName: "ix_orders_shift_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                newName: "ix_orders_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_company_id",
                table: "orders",
                newName: "ix_orders_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_cashier_user_id",
                table: "orders",
                newName: "ix_orders_cashier_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_orders_branch_id_order_number",
                table: "orders",
                newName: "ix_orders_branch_id_order_number");

            migrationBuilder.RenameIndex(
                name: "IX_orders_approved_void_by_user_id",
                table: "orders",
                newName: "ix_orders_approved_void_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_status_history_user_id",
                table: "order_status_history",
                newName: "ix_order_status_history_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_status_history_order_id",
                table: "order_status_history",
                newName: "ix_order_status_history_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_payments_user_id",
                table: "order_payments",
                newName: "ix_order_payments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_payments_payment_method_id",
                table: "order_payments",
                newName: "ix_order_payments_payment_method_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_payments_order_id",
                table: "order_payments",
                newName: "ix_order_payments_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_payments_gift_card_id",
                table: "order_payments",
                newName: "ix_order_payments_gift_card_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_lines_order_id",
                table: "order_lines",
                newName: "ix_order_lines_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_lines_menu_item_size_id",
                table: "order_lines",
                newName: "ix_order_lines_menu_item_size_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_lines_menu_item_id",
                table: "order_lines",
                newName: "ix_order_lines_menu_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_lines_kitchen_station_id",
                table: "order_lines",
                newName: "ix_order_lines_kitchen_station_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_lines_created_by_user_id",
                table: "order_lines",
                newName: "ix_order_lines_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_line_modifiers_order_line_id",
                table: "order_line_modifiers",
                newName: "ix_order_line_modifiers_order_line_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_line_modifiers_modifier_id",
                table: "order_line_modifiers",
                newName: "ix_order_line_modifiers_modifier_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_delivery_details_order_id",
                table: "order_delivery_details",
                newName: "ix_order_delivery_details_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_delivery_details_delivery_zone_id",
                table: "order_delivery_details",
                newName: "ix_order_delivery_details_delivery_zone_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_delivery_details_customer_address_id",
                table: "order_delivery_details",
                newName: "ix_order_delivery_details_customer_address_id");

            migrationBuilder.RenameIndex(
                name: "IX_modifiers_company_id",
                table: "modifiers",
                newName: "ix_modifiers_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_modifiers_branch_id",
                table: "modifiers",
                newName: "ix_modifiers_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_items_kitchen_station_id",
                table: "menu_items",
                newName: "ix_menu_items_kitchen_station_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_items_company_id",
                table: "menu_items",
                newName: "ix_menu_items_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_items_category_id",
                table: "menu_items",
                newName: "ix_menu_items_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_items_branch_id",
                table: "menu_items",
                newName: "ix_menu_items_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_item_sizes_menu_item_id",
                table: "menu_item_sizes",
                newName: "ix_menu_item_sizes_menu_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_item_modifiers_modifier_id",
                table: "menu_item_modifiers",
                newName: "ix_menu_item_modifiers_modifier_id");

            migrationBuilder.RenameIndex(
                name: "IX_menu_item_modifiers_menu_item_id",
                table: "menu_item_modifiers",
                newName: "ix_menu_item_modifiers_menu_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_loyalty_accounts_loyalty_tier_id",
                table: "loyalty_accounts",
                newName: "ix_loyalty_accounts_loyalty_tier_id");

            migrationBuilder.RenameIndex(
                name: "IX_loyalty_accounts_customer_id",
                table: "loyalty_accounts",
                newName: "ix_loyalty_accounts_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_kitchen_stations_branch_id",
                table: "kitchen_stations",
                newName: "ix_kitchen_stations_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_goods_receipts_supplier_id",
                table: "goods_receipts",
                newName: "ix_goods_receipts_supplier_id");

            migrationBuilder.RenameIndex(
                name: "IX_goods_receipts_purchase_order_id",
                table: "goods_receipts",
                newName: "ix_goods_receipts_purchase_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_goods_receipts_branch_id",
                table: "goods_receipts",
                newName: "ix_goods_receipts_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_goods_receipt_lines_inventory_item_id",
                table: "goods_receipt_lines",
                newName: "ix_goods_receipt_lines_inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_goods_receipt_lines_goods_receipt_id",
                table: "goods_receipt_lines",
                newName: "ix_goods_receipt_lines_goods_receipt_id");

            migrationBuilder.RenameIndex(
                name: "IX_gift_cards_customer_id",
                table: "gift_cards",
                newName: "ix_gift_cards_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_gift_cards_company_id_gift_card_number",
                table: "gift_cards",
                newName: "ix_gift_cards_company_id_gift_card_number");

            migrationBuilder.RenameIndex(
                name: "IX_gift_cards_branch_issued_id",
                table: "gift_cards",
                newName: "ix_gift_cards_branch_issued_id");

            migrationBuilder.RenameIndex(
                name: "IX_gift_card_transactions_user_id",
                table: "gift_card_transactions",
                newName: "ix_gift_card_transactions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_gift_card_transactions_gift_card_id",
                table: "gift_card_transactions",
                newName: "ix_gift_card_transactions_gift_card_id");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "employees",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "employees",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "employees",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "employees",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "employees",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "employees",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "HireDate",
                table: "employees",
                newName: "hire_date");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "employees",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "employees",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "employees",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "employees",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "BaseSalary",
                table: "employees",
                newName: "base_salary");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "employees",
                newName: "employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_UserId",
                table: "employees",
                newName: "ix_employees_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_CompanyId",
                table: "employees",
                newName: "ix_employees_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_BranchId",
                table: "employees",
                newName: "ix_employees_branch_id");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "customers",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "customers",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "customers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "customers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "customers",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "customers",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "DefaultCurrencyCode",
                table: "customers",
                newName: "default_currency_code");

            migrationBuilder.RenameColumn(
                name: "DefaultBranchId",
                table: "customers",
                newName: "default_branch_id");

            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "customers",
                newName: "customer_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customers",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "customers",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customers",
                newName: "customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_DefaultBranchId",
                table: "customers",
                newName: "ix_customers_default_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_CompanyId_CustomerCode",
                table: "customers",
                newName: "ix_customers_company_id_customer_code");

            migrationBuilder.RenameIndex(
                name: "IX_company_payments_recorded_by_super_admin_id",
                table: "company_payments",
                newName: "ix_company_payments_recorded_by_super_admin_id");

            migrationBuilder.RenameIndex(
                name: "IX_company_payments_company_id",
                table: "company_payments",
                newName: "ix_company_payments_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_companies_username",
                table: "companies",
                newName: "ix_companies_username");

            migrationBuilder.RenameIndex(
                name: "IX_companies_plan_id",
                table: "companies",
                newName: "ix_companies_plan_id");

            migrationBuilder.RenameIndex(
                name: "IX_companies_created_by_super_admin_id",
                table: "companies",
                newName: "ix_companies_created_by_super_admin_id");

            migrationBuilder.RenameIndex(
                name: "IX_categories_parent_category_id",
                table: "categories",
                newName: "ix_categories_parent_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_categories_company_id",
                table: "categories",
                newName: "ix_categories_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_categories_branch_id",
                table: "categories",
                newName: "ix_categories_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_branches_updated_by_user_id",
                table: "branches",
                newName: "ix_branches_updated_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_branches_default_currency_code",
                table: "branches",
                newName: "ix_branches_default_currency_code");

            migrationBuilder.RenameIndex(
                name: "IX_branches_created_by_user_id",
                table: "branches",
                newName: "ix_branches_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_branches_company_id_code",
                table: "branches",
                newName: "ix_branches_company_id_code");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "audit_log",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "audit_log",
                newName: "details");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "audit_log",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "audit_log",
                newName: "entity_name");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "audit_log",
                newName: "entity_id");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "audit_log",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "audit_log",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "ActionType",
                table: "audit_log",
                newName: "action_type");

            migrationBuilder.RenameColumn(
                name: "AuditLogId",
                table: "audit_log",
                newName: "audit_log_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_log_UserId",
                table: "audit_log",
                newName: "ix_audit_log_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_log_CompanyId",
                table: "audit_log",
                newName: "ix_audit_log_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_log_BranchId",
                table: "audit_log",
                newName: "ix_audit_log_branch_id");

            migrationBuilder.RenameIndex(
                name: "IX_attendance_employee_id",
                table: "attendance",
                newName: "ix_attendance_employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_attendance_company_id",
                table: "attendance",
                newName: "ix_attendance_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_approval_rules_role_id",
                table: "approval_rules",
                newName: "ix_approval_rules_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_approval_rules_company_id",
                table: "approval_rules",
                newName: "ix_approval_rules_company_id");

            migrationBuilder.RenameColumn(
                name: "Symbol",
                table: "units_of_measure",
                newName: "symbol");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "units_of_measure",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "units_of_measure",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "UnitGroup",
                table: "units_of_measure",
                newName: "unit_group");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "units_of_measure",
                newName: "sort_order");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "units_of_measure",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "units_of_measure",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "units_of_measure",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "UnitId",
                table: "units_of_measure",
                newName: "unit_id");

            migrationBuilder.RenameIndex(
                name: "IX_UnitsOfMeasure_CompanyId",
                table: "units_of_measure",
                newName: "ix_units_of_measure_company_id");

            migrationBuilder.RenameColumn(
                name: "ToUnitCode",
                table: "unit_conversions",
                newName: "to_unit_code");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "unit_conversions",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FromUnitCode",
                table: "unit_conversions",
                newName: "from_unit_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "unit_conversions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ConversionFactor",
                table: "unit_conversions",
                newName: "conversion_factor");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "unit_conversions",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "ConversionId",
                table: "unit_conversions",
                newName: "conversion_id");

            migrationBuilder.RenameIndex(
                name: "IX_UnitConversions_CompanyId",
                table: "unit_conversions",
                newName: "ix_unit_conversions_company_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "reservation_deposits",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "reservation_deposits",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "reservation_deposits",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "reservation_deposits",
                newName: "reservation_id");

            migrationBuilder.RenameColumn(
                name: "RefundedAt",
                table: "reservation_deposits",
                newName: "refunded_at");

            migrationBuilder.RenameColumn(
                name: "PaymentMethodId",
                table: "reservation_deposits",
                newName: "payment_method_id");

            migrationBuilder.RenameColumn(
                name: "PaidAt",
                table: "reservation_deposits",
                newName: "paid_at");

            migrationBuilder.RenameColumn(
                name: "CurrencyCode",
                table: "reservation_deposits",
                newName: "currency_code");

            migrationBuilder.RenameColumn(
                name: "ReservationDepositId",
                table: "reservation_deposits",
                newName: "reservation_deposit_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationDeposits_UserId",
                table: "reservation_deposits",
                newName: "ix_reservation_deposits_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationDeposits_ReservationId",
                table: "reservation_deposits",
                newName: "ix_reservation_deposits_reservation_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationDeposits_PaymentMethodId",
                table: "reservation_deposits",
                newName: "ix_reservation_deposits_payment_method_id");

            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "recipe_ingredients",
                newName: "recipe_id");

            migrationBuilder.RenameColumn(
                name: "QuantityPerYield",
                table: "recipe_ingredients",
                newName: "quantity_per_yield");

            migrationBuilder.RenameColumn(
                name: "InventoryItemId",
                table: "recipe_ingredients",
                newName: "inventory_item_id");

            migrationBuilder.RenameColumn(
                name: "RecipeIngredientId",
                table: "recipe_ingredients",
                newName: "recipe_ingredient_id");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "recipe_ingredients",
                newName: "ix_recipe_ingredients_recipe_id");

            migrationBuilder.RenameIndex(
                name: "IX_RecipeIngredients_InventoryItemId",
                table: "recipe_ingredients",
                newName: "ix_recipe_ingredients_inventory_item_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "receipt_templates",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Language",
                table: "receipt_templates",
                newName: "language");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "receipt_templates",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TemplateType",
                table: "receipt_templates",
                newName: "template_type");

            migrationBuilder.RenameColumn(
                name: "ShowLogo",
                table: "receipt_templates",
                newName: "show_logo");

            migrationBuilder.RenameColumn(
                name: "ShowBarcode",
                table: "receipt_templates",
                newName: "show_barcode");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "receipt_templates",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "receipt_templates",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "HeaderText",
                table: "receipt_templates",
                newName: "header_text");

            migrationBuilder.RenameColumn(
                name: "FooterText",
                table: "receipt_templates",
                newName: "footer_text");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "receipt_templates",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "receipt_templates",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "receipt_templates",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "ReceiptTemplateId",
                table: "receipt_templates",
                newName: "receipt_template_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptTemplates_CompanyId",
                table: "receipt_templates",
                newName: "ix_receipt_templates_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptTemplates_BranchId",
                table: "receipt_templates",
                newName: "ix_receipt_templates_branch_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "loyalty_transactions",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "loyalty_transactions",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "loyalty_transactions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "loyalty_transactions",
                newName: "transaction_date");

            migrationBuilder.RenameColumn(
                name: "PointsChange",
                table: "loyalty_transactions",
                newName: "points_change");

            migrationBuilder.RenameColumn(
                name: "PointsBefore",
                table: "loyalty_transactions",
                newName: "points_before");

            migrationBuilder.RenameColumn(
                name: "PointsAfter",
                table: "loyalty_transactions",
                newName: "points_after");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "loyalty_transactions",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "LoyaltyAccountId",
                table: "loyalty_transactions",
                newName: "loyalty_account_id");

            migrationBuilder.RenameColumn(
                name: "LoyaltyTransactionId",
                table: "loyalty_transactions",
                newName: "loyalty_transaction_id");

            migrationBuilder.RenameIndex(
                name: "IX_LoyaltyTransactions_UserId",
                table: "loyalty_transactions",
                newName: "ix_loyalty_transactions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_LoyaltyTransactions_LoyaltyAccountId",
                table: "loyalty_transactions",
                newName: "ix_loyalty_transactions_loyalty_account_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "loyalty_tiers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "TierDiscountPercent",
                table: "loyalty_tiers",
                newName: "tier_discount_percent");

            migrationBuilder.RenameColumn(
                name: "MinTotalSpent",
                table: "loyalty_tiers",
                newName: "min_total_spent");

            migrationBuilder.RenameColumn(
                name: "MinTotalPoints",
                table: "loyalty_tiers",
                newName: "min_total_points");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "loyalty_tiers",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "loyalty_tiers",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "LoyaltyTierId",
                table: "loyalty_tiers",
                newName: "loyalty_tier_id");

            migrationBuilder.RenameIndex(
                name: "IX_LoyaltyTiers_CompanyId",
                table: "loyalty_tiers",
                newName: "ix_loyalty_tiers_company_id");

            migrationBuilder.RenameColumn(
                name: "PointsRedeemValue",
                table: "loyalty_settings",
                newName: "points_redeem_value");

            migrationBuilder.RenameColumn(
                name: "PointsPerAmount",
                table: "loyalty_settings",
                newName: "points_per_amount");

            migrationBuilder.RenameColumn(
                name: "PointsExpiryMonths",
                table: "loyalty_settings",
                newName: "points_expiry_months");

            migrationBuilder.RenameColumn(
                name: "EarnOnNetBeforeTax",
                table: "loyalty_settings",
                newName: "earn_on_net_before_tax");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "loyalty_settings",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "loyalty_settings",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "AmountUnit",
                table: "loyalty_settings",
                newName: "amount_unit");

            migrationBuilder.RenameColumn(
                name: "LoyaltySettingsId",
                table: "loyalty_settings",
                newName: "loyalty_settings_id");

            migrationBuilder.RenameIndex(
                name: "IX_LoyaltySettings_CompanyId",
                table: "loyalty_settings",
                newName: "ix_loyalty_settings_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_LoyaltySettings_BranchId",
                table: "loyalty_settings",
                newName: "ix_loyalty_settings_branch_id");

            migrationBuilder.RenameColumn(
                name: "PrinterId",
                table: "kitchen_station_printers",
                newName: "printer_id");

            migrationBuilder.RenameColumn(
                name: "KitchenStationId",
                table: "kitchen_station_printers",
                newName: "kitchen_station_id");

            migrationBuilder.RenameColumn(
                name: "KitchenStationPrinterId",
                table: "kitchen_station_printers",
                newName: "kitchen_station_printer_id");

            migrationBuilder.RenameIndex(
                name: "IX_KitchenStationPrinters_PrinterId",
                table: "kitchen_station_printers",
                newName: "ix_kitchen_station_printers_printer_id");

            migrationBuilder.RenameIndex(
                name: "IX_KitchenStationPrinters_KitchenStationId_PrinterId",
                table: "kitchen_station_printers",
                newName: "ix_kitchen_station_printers_kitchen_station_id_printer_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "inventory_items",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "inventory_items",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "inventory_items",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "inventory_items",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "UnitOfMeasure",
                table: "inventory_items",
                newName: "unit_of_measure");

            migrationBuilder.RenameColumn(
                name: "ReorderQty",
                table: "inventory_items",
                newName: "reorder_qty");

            migrationBuilder.RenameColumn(
                name: "MinLevel",
                table: "inventory_items",
                newName: "min_level");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "inventory_items",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "inventory_items",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CostMethod",
                table: "inventory_items",
                newName: "cost_method");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "inventory_items",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "InventoryItemId",
                table: "inventory_items",
                newName: "inventory_item_id");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_currency_code",
                table: "inventory_items",
                newName: "ix_inventory_items_currency_code");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryItems_CompanyId_Code",
                table: "inventory_items",
                newName: "ix_inventory_items_company_id_code");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "inventory_categories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "inventory_categories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "inventory_categories",
                newName: "sort_order");

            migrationBuilder.RenameColumn(
                name: "ParentCategoryId",
                table: "inventory_categories",
                newName: "parent_category_id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "inventory_categories",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "inventory_categories",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "inventory_categories",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "inventory_categories",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryCategories_ParentCategoryId",
                table: "inventory_categories",
                newName: "ix_inventory_categories_parent_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryCategories_CompanyId",
                table: "inventory_categories",
                newName: "ix_inventory_categories_company_id");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "exchange_rates",
                newName: "rate");

            migrationBuilder.RenameColumn(
                name: "ValidTo",
                table: "exchange_rates",
                newName: "valid_to");

            migrationBuilder.RenameColumn(
                name: "ValidFrom",
                table: "exchange_rates",
                newName: "valid_from");

            migrationBuilder.RenameColumn(
                name: "ForeignCurrencyCode",
                table: "exchange_rates",
                newName: "foreign_currency_code");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "exchange_rates",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BaseCurrencyCode",
                table: "exchange_rates",
                newName: "base_currency_code");

            migrationBuilder.RenameColumn(
                name: "ExchangeRateId",
                table: "exchange_rates",
                newName: "exchange_rate_id");

            migrationBuilder.RenameIndex(
                name: "IX_ExchangeRates_CompanyId",
                table: "exchange_rates",
                newName: "ix_exchange_rates_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryZones_branch_id",
                table: "delivery_zones",
                newName: "ix_delivery_zones_branch_id");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "customer_addresses",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "customer_addresses",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "Label",
                table: "customer_addresses",
                newName: "label");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "customer_addresses",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Area",
                table: "customer_addresses",
                newName: "area");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "customer_addresses",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "DeliveryZoneId",
                table: "customer_addresses",
                newName: "delivery_zone_id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "customer_addresses",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "AddressLine2",
                table: "customer_addresses",
                newName: "address_line2");

            migrationBuilder.RenameColumn(
                name: "AddressLine1",
                table: "customer_addresses",
                newName: "address_line1");

            migrationBuilder.RenameColumn(
                name: "CustomerAddressId",
                table: "customer_addresses",
                newName: "customer_address_id");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerAddresses_DeliveryZoneId",
                table: "customer_addresses",
                newName: "ix_customer_addresses_delivery_zone_id");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "customer_addresses",
                newName: "ix_customer_addresses_customer_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "commission_policies",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SalesPercent",
                table: "commission_policies",
                newName: "sales_percent");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "commission_policies",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "FixedPerInvoice",
                table: "commission_policies",
                newName: "fixed_per_invoice");

            migrationBuilder.RenameColumn(
                name: "ExcludeDiscountedInvoices",
                table: "commission_policies",
                newName: "exclude_discounted_invoices");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "commission_policies",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "commission_policies",
                newName: "company_id");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "commission_policies",
                newName: "branch_id");

            migrationBuilder.RenameColumn(
                name: "ApplyOnNetBeforeTax",
                table: "commission_policies",
                newName: "apply_on_net_before_tax");

            migrationBuilder.RenameColumn(
                name: "CommissionPolicyId",
                table: "commission_policies",
                newName: "commission_policy_id");

            migrationBuilder.RenameIndex(
                name: "IX_CommissionPolicies_CompanyId",
                table: "commission_policies",
                newName: "ix_commission_policies_company_id");

            migrationBuilder.RenameIndex(
                name: "IX_CommissionPolicies_BranchId",
                table: "commission_policies",
                newName: "ix_commission_policies_branch_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_wastages",
                table: "wastages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_roles",
                table: "user_roles",
                column: "user_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_system_settings",
                table: "system_settings",
                column: "setting_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_suppliers",
                table: "suppliers",
                column: "supplier_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_super_admins",
                table: "super_admins",
                column: "super_admin_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subscription_plans",
                table: "subscription_plans",
                column: "plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stock_movements",
                table: "stock_movements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stock_counts",
                table: "stock_counts",
                column: "stock_count_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stock_count_lines",
                table: "stock_count_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stock_adjustments",
                table: "stock_adjustments",
                column: "stock_adjustment_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shifts",
                table: "shifts",
                column: "shift_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roles",
                table: "roles",
                column: "role_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_role_permissions",
                table: "role_permissions",
                column: "role_permission_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_restaurant_tables",
                table: "restaurant_tables",
                column: "table_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservations",
                table: "reservations",
                column: "reservation_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_recipes",
                table: "recipes",
                column: "recipe_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_purchase_orders",
                table: "purchase_orders",
                column: "purchase_order_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_purchase_order_lines",
                table: "purchase_order_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_printers",
                table: "printers",
                column: "printer_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_permissions",
                table: "permissions",
                column: "permission_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payment_methods",
                table: "payment_methods",
                column: "payment_method_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_orders",
                table: "orders",
                column: "order_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_status_history",
                table: "order_status_history",
                column: "order_status_history_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_payments",
                table: "order_payments",
                column: "order_payment_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_lines",
                table: "order_lines",
                column: "order_line_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_line_modifiers",
                table: "order_line_modifiers",
                column: "order_line_modifier_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_delivery_details",
                table: "order_delivery_details",
                column: "order_delivery_details_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_modifiers",
                table: "modifiers",
                column: "modifier_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_menu_items",
                table: "menu_items",
                column: "menu_item_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_menu_item_sizes",
                table: "menu_item_sizes",
                column: "menu_item_size_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_menu_item_modifiers",
                table: "menu_item_modifiers",
                column: "menu_item_modifier_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_loyalty_accounts",
                table: "loyalty_accounts",
                column: "loyalty_account_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_kitchen_stations",
                table: "kitchen_stations",
                column: "kitchen_station_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_goods_receipts",
                table: "goods_receipts",
                column: "goods_receipt_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_goods_receipt_lines",
                table: "goods_receipt_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_gift_cards",
                table: "gift_cards",
                column: "gift_card_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_gift_card_transactions",
                table: "gift_card_transactions",
                column: "gift_card_transaction_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_employees",
                table: "employees",
                column: "employee_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_customers",
                table: "customers",
                column: "customer_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_currencies",
                table: "currencies",
                column: "currency_code");

            migrationBuilder.AddPrimaryKey(
                name: "pk_company_payments",
                table: "company_payments",
                column: "payment_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_companies",
                table: "companies",
                column: "company_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categories",
                table: "categories",
                column: "category_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_branches",
                table: "branches",
                column: "branch_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_audit_log",
                table: "audit_log",
                column: "audit_log_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_attendance",
                table: "attendance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_approval_rules",
                table: "approval_rules",
                column: "approval_rule_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_units_of_measure",
                table: "units_of_measure",
                column: "unit_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_unit_conversions",
                table: "unit_conversions",
                column: "conversion_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_reservation_deposits",
                table: "reservation_deposits",
                column: "reservation_deposit_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_recipe_ingredients",
                table: "recipe_ingredients",
                column: "recipe_ingredient_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_receipt_templates",
                table: "receipt_templates",
                column: "receipt_template_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_loyalty_transactions",
                table: "loyalty_transactions",
                column: "loyalty_transaction_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_loyalty_tiers",
                table: "loyalty_tiers",
                column: "loyalty_tier_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_loyalty_settings",
                table: "loyalty_settings",
                column: "loyalty_settings_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_kitchen_station_printers",
                table: "kitchen_station_printers",
                column: "kitchen_station_printer_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_inventory_items",
                table: "inventory_items",
                column: "inventory_item_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_inventory_categories",
                table: "inventory_categories",
                column: "category_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_exchange_rates",
                table: "exchange_rates",
                column: "exchange_rate_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_delivery_zones",
                table: "delivery_zones",
                column: "delivery_zone_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_customer_addresses",
                table: "customer_addresses",
                column: "customer_address_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_commission_policies",
                table: "commission_policies",
                column: "commission_policy_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approval_rules_companies_company_id",
                table: "approval_rules",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_approval_rules_roles_role_id",
                table: "approval_rules",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attendance_companies_company_id",
                table: "attendance",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attendance_employees_employee_id",
                table: "attendance",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_audit_log_branches_branch_id",
                table: "audit_log",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_audit_log_companies_company_id",
                table: "audit_log",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_audit_log_users_user_id",
                table: "audit_log",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_branches_companies_company_id",
                table: "branches",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_branches_currencies_default_currency_code",
                table: "branches",
                column: "default_currency_code",
                principalTable: "currencies",
                principalColumn: "currency_code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_branches_users_created_by_user_id",
                table: "branches",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_branches_users_updated_by_user_id",
                table: "branches",
                column: "updated_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_categories_branches_branch_id",
                table: "categories",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_categories_categories_parent_category_id",
                table: "categories",
                column: "parent_category_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_companies_company_id",
                table: "categories",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_commission_policies_branches_branch_id",
                table: "commission_policies",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_commission_policies_companies_company_id",
                table: "commission_policies",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_companies_subscription_plans_plan_id",
                table: "companies",
                column: "plan_id",
                principalTable: "subscription_plans",
                principalColumn: "plan_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_companies_super_admins_created_by_super_admin_id",
                table: "companies",
                column: "created_by_super_admin_id",
                principalTable: "super_admins",
                principalColumn: "super_admin_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_company_payments_companies_company_id",
                table: "company_payments",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_company_payments_super_admins_recorded_by_super_admin_id",
                table: "company_payments",
                column: "recorded_by_super_admin_id",
                principalTable: "super_admins",
                principalColumn: "super_admin_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_customer_addresses_customers_customer_id",
                table: "customer_addresses",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_customer_addresses_delivery_zones_delivery_zone_id",
                table: "customer_addresses",
                column: "delivery_zone_id",
                principalTable: "delivery_zones",
                principalColumn: "delivery_zone_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_branches_default_branch_id",
                table: "customers",
                column: "default_branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_companies_company_id",
                table: "customers",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_delivery_zones_branches_branch_id",
                table: "delivery_zones",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_branches_branch_id",
                table: "employees",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_companies_company_id",
                table: "employees",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_users_user_id",
                table: "employees",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_exchange_rates_companies_company_id",
                table: "exchange_rates",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_card_transactions_gift_cards_gift_card_id",
                table: "gift_card_transactions",
                column: "gift_card_id",
                principalTable: "gift_cards",
                principalColumn: "gift_card_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_card_transactions_users_user_id",
                table: "gift_card_transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_gift_cards_branches_branch_issued_id",
                table: "gift_cards",
                column: "branch_issued_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_cards_companies_company_id",
                table: "gift_cards",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_gift_cards_customers_customer_id",
                table: "gift_cards",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_goods_receipt_lines_goods_receipts_goods_receipt_id",
                table: "goods_receipt_lines",
                column: "goods_receipt_id",
                principalTable: "goods_receipts",
                principalColumn: "goods_receipt_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_goods_receipt_lines_inventory_items_inventory_item_id",
                table: "goods_receipt_lines",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_goods_receipts_branches_branch_id",
                table: "goods_receipts",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_goods_receipts_purchase_orders_purchase_order_id",
                table: "goods_receipts",
                column: "purchase_order_id",
                principalTable: "purchase_orders",
                principalColumn: "purchase_order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_goods_receipts_suppliers_supplier_id",
                table: "goods_receipts",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "supplier_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_categories_companies_company_id",
                table: "inventory_categories",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_categories_inventory_categories_parent_category_id",
                table: "inventory_categories",
                column: "parent_category_id",
                principalTable: "inventory_categories",
                principalColumn: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_items_companies_company_id",
                table: "inventory_items",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_items_currencies_currency_code",
                table: "inventory_items",
                column: "currency_code",
                principalTable: "currencies",
                principalColumn: "currency_code");

            migrationBuilder.AddForeignKey(
                name: "fk_kitchen_station_printers_kitchen_stations_kitchen_station_id",
                table: "kitchen_station_printers",
                column: "kitchen_station_id",
                principalTable: "kitchen_stations",
                principalColumn: "kitchen_station_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_kitchen_station_printers_printers_printer_id",
                table: "kitchen_station_printers",
                column: "printer_id",
                principalTable: "printers",
                principalColumn: "printer_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_kitchen_stations_branches_branch_id",
                table: "kitchen_stations",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_accounts_customers_customer_id",
                table: "loyalty_accounts",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_accounts_loyalty_tiers_loyalty_tier_id",
                table: "loyalty_accounts",
                column: "loyalty_tier_id",
                principalTable: "loyalty_tiers",
                principalColumn: "loyalty_tier_id");

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_settings_branches_branch_id",
                table: "loyalty_settings",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_settings_companies_company_id",
                table: "loyalty_settings",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_tiers_companies_company_id",
                table: "loyalty_tiers",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_transactions_loyalty_accounts_loyalty_account_id",
                table: "loyalty_transactions",
                column: "loyalty_account_id",
                principalTable: "loyalty_accounts",
                principalColumn: "loyalty_account_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_loyalty_transactions_users_user_id",
                table: "loyalty_transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_menu_item_modifiers_menu_items_menu_item_id",
                table: "menu_item_modifiers",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_menu_item_modifiers_modifiers_modifier_id",
                table: "menu_item_modifiers",
                column: "modifier_id",
                principalTable: "modifiers",
                principalColumn: "modifier_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_menu_item_sizes_menu_items_menu_item_id",
                table: "menu_item_sizes",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_menu_items_branches_branch_id",
                table: "menu_items",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_menu_items_categories_category_id",
                table: "menu_items",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "category_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_menu_items_companies_company_id",
                table: "menu_items",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_menu_items_kitchen_stations_kitchen_station_id",
                table: "menu_items",
                column: "kitchen_station_id",
                principalTable: "kitchen_stations",
                principalColumn: "kitchen_station_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_modifiers_branches_branch_id",
                table: "modifiers",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_modifiers_companies_company_id",
                table: "modifiers",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_delivery_details_customer_addresses_customer_address_",
                table: "order_delivery_details",
                column: "customer_address_id",
                principalTable: "customer_addresses",
                principalColumn: "customer_address_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_order_delivery_details_delivery_zones_delivery_zone_id",
                table: "order_delivery_details",
                column: "delivery_zone_id",
                principalTable: "delivery_zones",
                principalColumn: "delivery_zone_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_order_delivery_details_orders_order_id",
                table: "order_delivery_details",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_line_modifiers_modifiers_modifier_id",
                table: "order_line_modifiers",
                column: "modifier_id",
                principalTable: "modifiers",
                principalColumn: "modifier_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_order_line_modifiers_order_lines_order_line_id",
                table: "order_line_modifiers",
                column: "order_line_id",
                principalTable: "order_lines",
                principalColumn: "order_line_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_lines_kitchen_stations_kitchen_station_id",
                table: "order_lines",
                column: "kitchen_station_id",
                principalTable: "kitchen_stations",
                principalColumn: "kitchen_station_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_order_lines_menu_item_sizes_menu_item_size_id",
                table: "order_lines",
                column: "menu_item_size_id",
                principalTable: "menu_item_sizes",
                principalColumn: "menu_item_size_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_order_lines_menu_items_menu_item_id",
                table: "order_lines",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_order_lines_orders_order_id",
                table: "order_lines",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_lines_users_created_by_user_id",
                table: "order_lines",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_order_payments_gift_cards_gift_card_id",
                table: "order_payments",
                column: "gift_card_id",
                principalTable: "gift_cards",
                principalColumn: "gift_card_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_order_payments_orders_order_id",
                table: "order_payments",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_payments_payment_methods_payment_method_id",
                table: "order_payments",
                column: "payment_method_id",
                principalTable: "payment_methods",
                principalColumn: "payment_method_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_order_payments_users_user_id",
                table: "order_payments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_order_status_history_orders_order_id",
                table: "order_status_history",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_status_history_users_user_id",
                table: "order_status_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_branches_branch_id",
                table: "orders",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_companies_company_id",
                table: "orders",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_restaurant_tables_table_id",
                table: "orders",
                column: "table_id",
                principalTable: "restaurant_tables",
                principalColumn: "table_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_shifts_shift_id",
                table: "orders",
                column: "shift_id",
                principalTable: "shifts",
                principalColumn: "shift_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_approved_void_by_user_id",
                table: "orders",
                column: "approved_void_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_cashier_user_id",
                table: "orders",
                column: "cashier_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_void_by_user_id",
                table: "orders",
                column: "void_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_waiter_user_id",
                table: "orders",
                column: "waiter_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_payment_methods_companies_company_id",
                table: "payment_methods",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_printers_branches_branch_id",
                table: "printers",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_lines_inventory_items_inventory_item_id",
                table: "purchase_order_lines",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_order_lines_purchase_orders_purchase_order_id",
                table: "purchase_order_lines",
                column: "purchase_order_id",
                principalTable: "purchase_orders",
                principalColumn: "purchase_order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_orders_branches_branch_id",
                table: "purchase_orders",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_purchase_orders_suppliers_supplier_id",
                table: "purchase_orders",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "supplier_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_receipt_templates_branches_branch_id",
                table: "receipt_templates",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_receipt_templates_companies_company_id",
                table: "receipt_templates",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_recipe_ingredients_inventory_items_inventory_item_id",
                table: "recipe_ingredients",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_recipe_ingredients_recipes_recipe_id",
                table: "recipe_ingredients",
                column: "recipe_id",
                principalTable: "recipes",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_companies_company_id",
                table: "recipes",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_menu_item_sizes_menu_item_size_id",
                table: "recipes",
                column: "menu_item_size_id",
                principalTable: "menu_item_sizes",
                principalColumn: "menu_item_size_id");

            migrationBuilder.AddForeignKey(
                name: "fk_recipes_menu_items_menu_item_id",
                table: "recipes",
                column: "menu_item_id",
                principalTable: "menu_items",
                principalColumn: "menu_item_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_deposits_payment_methods_payment_method_id",
                table: "reservation_deposits",
                column: "payment_method_id",
                principalTable: "payment_methods",
                principalColumn: "payment_method_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_deposits_reservations_reservation_id",
                table: "reservation_deposits",
                column: "reservation_id",
                principalTable: "reservations",
                principalColumn: "reservation_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reservation_deposits_users_user_id",
                table: "reservation_deposits",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_branches_branch_id",
                table: "reservations",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_customers_customer_id",
                table: "reservations",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_restaurant_tables_table_id",
                table: "reservations",
                column: "table_id",
                principalTable: "restaurant_tables",
                principalColumn: "table_id");

            migrationBuilder.AddForeignKey(
                name: "fk_reservations_users_created_by_user_id",
                table: "reservations",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_restaurant_tables_branches_branch_id",
                table: "restaurant_tables",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_role_permissions_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id",
                principalTable: "permissions",
                principalColumn: "permission_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_role_permissions_roles_role_id",
                table: "role_permissions",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_roles_branches_branch_id",
                table: "roles",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_roles_companies_company_id",
                table: "roles",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_branches_branch_id",
                table: "shifts",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_companies_company_id",
                table: "shifts",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_users_cashier_user_id",
                table: "shifts",
                column: "cashier_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_stock_adjustments_branches_branch_id",
                table: "stock_adjustments",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_stock_adjustments_inventory_items_inventory_item_id",
                table: "stock_adjustments",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id");

            migrationBuilder.AddForeignKey(
                name: "fk_stock_adjustments_users_user_id",
                table: "stock_adjustments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_stock_count_lines_inventory_items_inventory_item_id",
                table: "stock_count_lines",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_stock_count_lines_stock_counts_stock_count_id",
                table: "stock_count_lines",
                column: "stock_count_id",
                principalTable: "stock_counts",
                principalColumn: "stock_count_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_stock_movements_companies_company_id",
                table: "stock_movements",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_stock_movements_inventory_items_inventory_item_id",
                table: "stock_movements",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_suppliers_companies_company_id",
                table: "suppliers",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_system_settings_branches_branch_id",
                table: "system_settings",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "branch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_system_settings_companies_company_id",
                table: "system_settings",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_system_settings_users_updated_by_user_id",
                table: "system_settings",
                column: "updated_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_unit_conversions_companies_company_id",
                table: "unit_conversions",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_units_of_measure_companies_company_id",
                table: "units_of_measure",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_users_assigned_by_user_id",
                table: "user_roles",
                column: "assigned_by_user_id",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_branches_default_branch_id",
                table: "users",
                column: "default_branch_id",
                principalTable: "branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_users_companies_company_id",
                table: "users",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_wastages_companies_company_id",
                table: "wastages",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_wastages_inventory_items_inventory_item_id",
                table: "wastages",
                column: "inventory_item_id",
                principalTable: "inventory_items",
                principalColumn: "inventory_item_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
