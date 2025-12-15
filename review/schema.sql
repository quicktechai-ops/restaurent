-- ============================================
-- Restaurant POS System - PostgreSQL Schema
-- For Render.com PostgreSQL Database
-- ============================================

-- Enable UUID extension (optional)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ============================================
-- 20. SUPERADMIN (MULTI-TENANT SaaS)
-- ============================================

-- 20.1 SuperAdmins
CREATE TABLE super_admins (
    super_admin_id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash BYTEA NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    phone VARCHAR(50),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    last_login_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- 20.3 Subscription Plans
CREATE TABLE subscription_plans (
    plan_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(255),
    price NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL DEFAULT 'USD',
    billing_cycle VARCHAR(20) NOT NULL, -- Monthly, Yearly
    duration_days INT NOT NULL,
    max_branches INT NOT NULL,
    max_users INT NOT NULL,
    max_orders_per_month INT,
    features JSONB, -- Array of feature flags
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- 20.2 Companies (Tenants)
CREATE TABLE companies (
    company_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash BYTEA NOT NULL,
    email VARCHAR(100),
    phone VARCHAR(50),
    address VARCHAR(255),
    logo VARCHAR(500),
    status VARCHAR(20) NOT NULL DEFAULT 'active', -- active, inactive, suspended, trial
    plan_id INT REFERENCES subscription_plans(plan_id),
    plan_expiry_date TIMESTAMPTZ,
    max_branches INT NOT NULL DEFAULT 1,
    max_users INT NOT NULL DEFAULT 5,
    trial_ends_at TIMESTAMPTZ,
    notes VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by_super_admin_id INT REFERENCES super_admins(super_admin_id),
    updated_at TIMESTAMPTZ
);

-- 20.4 Company Subscriptions
CREATE TABLE company_subscriptions (
    subscription_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    plan_id INT NOT NULL REFERENCES subscription_plans(plan_id),
    start_date TIMESTAMPTZ NOT NULL,
    end_date TIMESTAMPTZ NOT NULL,
    status VARCHAR(20) NOT NULL, -- active, expired, cancelled, upgraded
    amount NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL,
    payment_id INT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 20.5 Company Payments
CREATE TABLE company_payments (
    payment_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    amount NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL,
    payment_method VARCHAR(50) NOT NULL, -- CreditCard, BankTransfer, Cash, PayPal
    payment_reference VARCHAR(100),
    payment_date TIMESTAMPTZ NOT NULL,
    status VARCHAR(20) NOT NULL, -- pending, completed, failed, refunded
    notes VARCHAR(255),
    recorded_by_super_admin_id INT REFERENCES super_admins(super_admin_id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Add FK to company_subscriptions
ALTER TABLE company_subscriptions 
ADD CONSTRAINT fk_subscription_payment 
FOREIGN KEY (payment_id) REFERENCES company_payments(payment_id);

-- 20.6 Company Activity Log
CREATE TABLE company_activity_log (
    activity_log_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    activity_type VARCHAR(50) NOT NULL,
    description VARCHAR(500),
    metadata JSONB,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 20.7 SuperAdmin Audit Log
CREATE TABLE super_admin_audit_log (
    audit_log_id SERIAL PRIMARY KEY,
    super_admin_id INT NOT NULL REFERENCES super_admins(super_admin_id),
    action_type VARCHAR(50) NOT NULL,
    target_company_id INT REFERENCES companies(company_id),
    details JSONB,
    ip_address VARCHAR(50),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================================
-- 1. CORE & SECURITY
-- ============================================

-- 2.1 Currencies (create first for FK references)
CREATE TABLE currencies (
    currency_code VARCHAR(3) PRIMARY KEY,
    name VARCHAR(50),
    symbol VARCHAR(10),
    decimal_places SMALLINT NOT NULL,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 1.1 Branches
CREATE TABLE branches (
    branch_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50),
    country VARCHAR(100),
    city VARCHAR(100),
    address VARCHAR(255),
    phone VARCHAR(50),
    default_currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    vat_percent NUMERIC(5,2) NOT NULL,
    service_charge_percent NUMERIC(5,2) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    deleted_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by_user_id INT,
    updated_at TIMESTAMPTZ,
    updated_by_user_id INT,
    UNIQUE(company_id, code)
);

-- 1.2 Users
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    username VARCHAR(50) NOT NULL,
    password_hash BYTEA NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(50),
    email VARCHAR(100),
    default_branch_id INT REFERENCES branches(branch_id),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    deleted_at TIMESTAMPTZ,
    last_login_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id),
    updated_at TIMESTAMPTZ,
    updated_by_user_id INT REFERENCES users(user_id),
    UNIQUE(company_id, username)
);

-- Add FK to branches
ALTER TABLE branches 
ADD CONSTRAINT fk_branch_created_by FOREIGN KEY (created_by_user_id) REFERENCES users(user_id);
ALTER TABLE branches 
ADD CONSTRAINT fk_branch_updated_by FOREIGN KEY (updated_by_user_id) REFERENCES users(user_id);

-- 1.3 Roles
CREATE TABLE roles (
    role_id SERIAL PRIMARY KEY,
    company_id INT REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    name VARCHAR(50) NOT NULL,
    description VARCHAR(255),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    UNIQUE(company_id, branch_id, name)
);

-- 1.4 User Roles
CREATE TABLE user_roles (
    user_role_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(user_id),
    role_id INT NOT NULL REFERENCES roles(role_id),
    assigned_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    assigned_by_user_id INT REFERENCES users(user_id),
    UNIQUE(user_id, role_id)
);

-- 1.5 Permissions
CREATE TABLE permissions (
    permission_id SERIAL PRIMARY KEY,
    code VARCHAR(100) UNIQUE NOT NULL,
    description VARCHAR(255)
);

-- 1.6 Role Permissions
CREATE TABLE role_permissions (
    role_permission_id SERIAL PRIMARY KEY,
    role_id INT NOT NULL REFERENCES roles(role_id),
    permission_id INT NOT NULL REFERENCES permissions(permission_id),
    UNIQUE(role_id, permission_id)
);

-- 1.7 Payment Methods
CREATE TABLE payment_methods (
    payment_method_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    name VARCHAR(50) NOT NULL,
    type VARCHAR(20) NOT NULL, -- Cash, Card, GiftCard, LoyaltyPoints, Other
    requires_reference BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- ============================================
-- 2. CURRENCIES & EXCHANGE
-- ============================================

-- 2.2 Exchange Rates
CREATE TABLE exchange_rates (
    exchange_rate_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    base_currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    foreign_currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    rate NUMERIC(18,6) NOT NULL,
    valid_from TIMESTAMPTZ NOT NULL,
    valid_to TIMESTAMPTZ
);

-- ============================================
-- 3. DINING AREA (TABLES & KITCHEN)
-- ============================================

-- 3.1 Restaurant Tables
CREATE TABLE restaurant_tables (
    table_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    table_name VARCHAR(50) NOT NULL,
    zone VARCHAR(50),
    capacity INT NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 3.2 Kitchen Stations
CREATE TABLE kitchen_stations (
    kitchen_station_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    name VARCHAR(50) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- ============================================
-- 4. MENU, MODIFIERS & RECIPES
-- ============================================

-- 4.1 Categories
CREATE TABLE categories (
    category_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    parent_category_id INT REFERENCES categories(category_id),
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- 4.2 Menu Items
CREATE TABLE menu_items (
    menu_item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    category_id INT NOT NULL REFERENCES categories(category_id),
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    code VARCHAR(50),
    description VARCHAR(255),
    description_ar VARCHAR(255),
    default_price NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    tax_included BOOLEAN NOT NULL DEFAULT FALSE,
    allow_sizes BOOLEAN NOT NULL DEFAULT FALSE,
    has_recipe BOOLEAN NOT NULL DEFAULT FALSE,
    kitchen_station_id INT REFERENCES kitchen_stations(kitchen_station_id),
    is_visible_online_menu BOOLEAN NOT NULL DEFAULT TRUE,
    item_commission_per_unit NUMERIC(18,2),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    UNIQUE(company_id, code)
);

-- 4.2.1 Menu Item Price History
CREATE TABLE menu_item_price_history (
    price_history_id SERIAL PRIMARY KEY,
    menu_item_id INT NOT NULL REFERENCES menu_items(menu_item_id),
    menu_item_size_id INT,
    old_price NUMERIC(18,2) NOT NULL,
    new_price NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    changed_by_user_id INT NOT NULL REFERENCES users(user_id),
    reason VARCHAR(255)
);

-- 4.3 Menu Item Sizes
CREATE TABLE menu_item_sizes (
    menu_item_size_id SERIAL PRIMARY KEY,
    menu_item_id INT NOT NULL REFERENCES menu_items(menu_item_id),
    size_name VARCHAR(50) NOT NULL,
    price NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    cost NUMERIC(18,4)
);

-- Add FK to price history
ALTER TABLE menu_item_price_history 
ADD CONSTRAINT fk_price_history_size 
FOREIGN KEY (menu_item_size_id) REFERENCES menu_item_sizes(menu_item_size_id);

-- 4.4 Modifiers
CREATE TABLE modifiers (
    modifier_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    description VARCHAR(255),
    extra_price NUMERIC(18,2) NOT NULL DEFAULT 0,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ
);

-- 4.5 Menu Item Modifiers
CREATE TABLE menu_item_modifiers (
    menu_item_modifier_id SERIAL PRIMARY KEY,
    menu_item_id INT NOT NULL REFERENCES menu_items(menu_item_id),
    modifier_id INT NOT NULL REFERENCES modifiers(modifier_id),
    is_required BOOLEAN NOT NULL DEFAULT FALSE,
    max_quantity INT,
    sort_order INT NOT NULL DEFAULT 0,
    UNIQUE(menu_item_id, modifier_id)
);

-- 4.6 Inventory Items (Ingredients)
CREATE TABLE inventory_items (
    inventory_item_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50),
    unit_of_measure VARCHAR(20) NOT NULL,
    category VARCHAR(50),
    min_level NUMERIC(18,4) NOT NULL DEFAULT 0,
    reorder_qty NUMERIC(18,4) NOT NULL DEFAULT 0,
    cost_method VARCHAR(20) NOT NULL DEFAULT 'Average',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE(company_id, code)
);

-- 4.7 Recipes
CREATE TABLE recipes (
    recipe_id SERIAL PRIMARY KEY,
    menu_item_id INT NOT NULL REFERENCES menu_items(menu_item_id),
    menu_item_size_id INT REFERENCES menu_item_sizes(menu_item_size_id),
    yield_quantity NUMERIC(18,4) NOT NULL DEFAULT 1,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    UNIQUE(menu_item_id, menu_item_size_id)
);

-- 4.8 Recipe Ingredients
CREATE TABLE recipe_ingredients (
    recipe_ingredient_id SERIAL PRIMARY KEY,
    recipe_id INT NOT NULL REFERENCES recipes(recipe_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    quantity_per_yield NUMERIC(18,4) NOT NULL
);

-- ============================================
-- 5. CUSTOMERS, ADDRESSES, HISTORY
-- ============================================

-- 9.1 Delivery Zones (create first for FK)
CREATE TABLE delivery_zones (
    delivery_zone_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    zone_name VARCHAR(100) NOT NULL,
    description VARCHAR(255),
    min_order_amount NUMERIC(18,2),
    base_fee NUMERIC(18,2) NOT NULL DEFAULT 0,
    extra_fee_per_km NUMERIC(18,2),
    max_distance_km NUMERIC(18,2),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 5.1 Customers
CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    customer_code VARCHAR(50),
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(50),
    email VARCHAR(100),
    default_branch_id INT REFERENCES branches(branch_id),
    default_currency_code VARCHAR(3) REFERENCES currencies(currency_code),
    notes VARCHAR(255),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    UNIQUE(company_id, customer_code)
);

-- 5.2 Customer Addresses
CREATE TABLE customer_addresses (
    customer_address_id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customers(customer_id),
    label VARCHAR(50) NOT NULL,
    address_line1 VARCHAR(255) NOT NULL,
    address_line2 VARCHAR(255),
    city VARCHAR(100) NOT NULL,
    area VARCHAR(100),
    latitude NUMERIC(9,6),
    longitude NUMERIC(9,6),
    delivery_zone_id INT REFERENCES delivery_zones(delivery_zone_id),
    is_default BOOLEAN NOT NULL DEFAULT FALSE
);

-- ============================================
-- 6. LOYALTY (POINTS & TIERS)
-- ============================================

-- 6.1 Loyalty Settings
CREATE TABLE loyalty_settings (
    loyalty_settings_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    points_per_amount NUMERIC(18,4) NOT NULL DEFAULT 1,
    amount_unit NUMERIC(18,4) NOT NULL DEFAULT 10,
    earn_on_net_before_tax BOOLEAN NOT NULL DEFAULT TRUE,
    points_redeem_value NUMERIC(18,4) NOT NULL DEFAULT 0.1,
    points_expiry_months INT
);

-- 6.2 Loyalty Tiers
CREATE TABLE loyalty_tiers (
    loyalty_tier_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    name VARCHAR(50) NOT NULL,
    min_total_spent NUMERIC(18,2) NOT NULL,
    min_total_points NUMERIC(18,2) NOT NULL DEFAULT 0,
    tier_discount_percent NUMERIC(5,2) NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 6.3 Loyalty Accounts
CREATE TABLE loyalty_accounts (
    loyalty_account_id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customers(customer_id),
    points_balance NUMERIC(18,2) NOT NULL DEFAULT 0,
    loyalty_tier_id INT REFERENCES loyalty_tiers(loyalty_tier_id),
    tier_assigned_at TIMESTAMPTZ
);

-- ============================================
-- 7. GIFT CARDS / VOUCHERS
-- ============================================

-- 7.1 Gift Cards
CREATE TABLE gift_cards (
    gift_card_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    gift_card_number VARCHAR(50) UNIQUE NOT NULL,
    branch_issued_id INT NOT NULL REFERENCES branches(branch_id),
    issue_date TIMESTAMPTZ NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    initial_value NUMERIC(18,2) NOT NULL,
    current_balance NUMERIC(18,2) NOT NULL,
    expiry_date TIMESTAMPTZ,
    status VARCHAR(20) NOT NULL, -- Active, UsedUp, Expired, Blocked
    customer_id INT REFERENCES customers(customer_id)
);

-- ============================================
-- 8. RESERVATIONS (NO-SHOW + DEPOSITS)
-- ============================================

-- 8.1 Reservations
CREATE TABLE reservations (
    reservation_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    customer_id INT REFERENCES customers(customer_id),
    reservation_date DATE NOT NULL,
    start_time TIME NOT NULL,
    duration_minutes INT NOT NULL,
    party_size INT NOT NULL,
    table_id INT REFERENCES restaurant_tables(table_id),
    status VARCHAR(20) NOT NULL, -- Pending, Confirmed, Seated, Canceled, NoShow
    channel VARCHAR(20) NOT NULL, -- Phone, WalkIn, Online
    notes VARCHAR(255),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by_user_id INT NOT NULL REFERENCES users(user_id)
);

-- 8.2 Reservation Deposits
CREATE TABLE reservation_deposits (
    reservation_deposit_id SERIAL PRIMARY KEY,
    reservation_id INT NOT NULL REFERENCES reservations(reservation_id),
    amount NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    status VARCHAR(20) NOT NULL, -- Pending, Paid, Forfeited, Refunded
    payment_method_id INT REFERENCES payment_methods(payment_method_id),
    paid_at TIMESTAMPTZ,
    refunded_at TIMESTAMPTZ,
    user_id INT REFERENCES users(user_id)
);

-- ============================================
-- 10. POS: SHIFTS, ORDERS, PAYMENTS
-- ============================================

-- 10.1 Shifts
CREATE TABLE shifts (
    shift_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    cashier_user_id INT NOT NULL REFERENCES users(user_id),
    open_time TIMESTAMPTZ NOT NULL,
    close_time TIMESTAMPTZ,
    expected_close_time TIMESTAMPTZ,
    opening_cash NUMERIC(18,2) NOT NULL,
    closing_cash NUMERIC(18,2),
    expected_cash NUMERIC(18,2),
    cash_difference NUMERIC(18,2),
    status VARCHAR(20) NOT NULL DEFAULT 'Open', -- Open, Closed, ForceClosedBySystem
    force_closed_at TIMESTAMPTZ,
    force_close_reason VARCHAR(255),
    notes VARCHAR(255)
);

-- 10.2 Orders
CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    shift_id INT REFERENCES shifts(shift_id),
    order_number VARCHAR(50) NOT NULL,
    order_type VARCHAR(20) NOT NULL, -- DineIn, Takeaway, Delivery
    table_id INT REFERENCES restaurant_tables(table_id),
    waiter_user_id INT REFERENCES users(user_id),
    cashier_user_id INT REFERENCES users(user_id),
    customer_id INT REFERENCES customers(customer_id),
    order_status VARCHAR(20) NOT NULL, -- Draft, SentToKitchen, Ready, Served, Paid, Voided
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    exchange_rate_to_base NUMERIC(18,6) NOT NULL DEFAULT 1,
    sub_total NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_line_discount NUMERIC(18,2) NOT NULL DEFAULT 0,
    bill_level_discount NUMERIC(18,2) NOT NULL DEFAULT 0,
    service_charge_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    delivery_fee NUMERIC(18,2) NOT NULL DEFAULT 0,
    tips_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    grand_total NUMERIC(18,2) NOT NULL DEFAULT 0,
    net_amount_for_loyalty NUMERIC(18,2),
    loyalty_points_earned NUMERIC(18,2),
    loyalty_points_redeemed NUMERIC(18,2),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    paid_at TIMESTAMPTZ,
    total_paid NUMERIC(18,2) NOT NULL DEFAULT 0,
    balance_due NUMERIC(18,2) NOT NULL DEFAULT 0,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'Unpaid', -- Unpaid, PartiallyPaid, Paid, Overpaid
    voided_at TIMESTAMPTZ,
    void_reason VARCHAR(255),
    void_by_user_id INT REFERENCES users(user_id),
    approved_void_by_user_id INT REFERENCES users(user_id),
    merged_from_order_id INT REFERENCES orders(order_id),
    split_from_order_id INT REFERENCES orders(order_id),
    updated_at TIMESTAMPTZ,
    UNIQUE(branch_id, order_number)
);

-- 10.3 Order Lines
CREATE TABLE order_lines (
    order_line_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id),
    menu_item_id INT NOT NULL REFERENCES menu_items(menu_item_id),
    menu_item_size_id INT REFERENCES menu_item_sizes(menu_item_size_id),
    quantity NUMERIC(18,4) NOT NULL,
    base_unit_price NUMERIC(18,2) NOT NULL,
    modifiers_extra_price NUMERIC(18,2) NOT NULL DEFAULT 0,
    line_gross NUMERIC(18,2) NOT NULL,
    line_discount NUMERIC(18,2) NOT NULL DEFAULT 0,
    line_net NUMERIC(18,2) NOT NULL,
    notes VARCHAR(255),
    kitchen_status VARCHAR(20) NOT NULL DEFAULT 'New' -- New, InProgress, Ready, Served, Cancelled
);

-- 10.4 Order Line Modifiers
CREATE TABLE order_line_modifiers (
    order_line_modifier_id SERIAL PRIMARY KEY,
    order_line_id INT NOT NULL REFERENCES order_lines(order_line_id),
    modifier_id INT NOT NULL REFERENCES modifiers(modifier_id),
    quantity NUMERIC(18,4) NOT NULL DEFAULT 1,
    extra_price NUMERIC(18,2) NOT NULL
);

-- 10.5 Order Payments
CREATE TABLE order_payments (
    order_payment_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id),
    payment_method_id INT NOT NULL REFERENCES payment_methods(payment_method_id),
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    amount NUMERIC(18,2) NOT NULL,
    exchange_rate_to_order_currency NUMERIC(18,6) NOT NULL DEFAULT 1,
    gift_card_id INT REFERENCES gift_cards(gift_card_id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    user_id INT NOT NULL REFERENCES users(user_id)
);

-- 10.6 Order Status History
CREATE TABLE order_status_history (
    order_status_history_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id),
    old_status VARCHAR(20),
    new_status VARCHAR(20) NOT NULL,
    changed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    user_id INT REFERENCES users(user_id)
);

-- 10.7 Order Delivery Details
CREATE TABLE order_delivery_details (
    order_delivery_details_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id),
    customer_address_id INT NOT NULL REFERENCES customer_addresses(customer_address_id),
    delivery_zone_id INT REFERENCES delivery_zones(delivery_zone_id),
    distance_km NUMERIC(18,2),
    delivery_fee_calculated NUMERIC(18,2) NOT NULL,
    driver_name VARCHAR(100),
    out_for_delivery_at TIMESTAMPTZ,
    delivered_at TIMESTAMPTZ
);

-- 6.4 Loyalty Transactions
CREATE TABLE loyalty_transactions (
    loyalty_transaction_id SERIAL PRIMARY KEY,
    loyalty_account_id INT NOT NULL REFERENCES loyalty_accounts(loyalty_account_id),
    transaction_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    type VARCHAR(20) NOT NULL, -- Earn, Redeem, Adjust, Expire
    points_change NUMERIC(18,2) NOT NULL,
    points_before NUMERIC(18,2) NOT NULL,
    points_after NUMERIC(18,2) NOT NULL,
    order_id INT REFERENCES orders(order_id),
    notes VARCHAR(255),
    user_id INT REFERENCES users(user_id)
);

-- 7.2 Gift Card Transactions
CREATE TABLE gift_card_transactions (
    gift_card_transaction_id SERIAL PRIMARY KEY,
    gift_card_id INT NOT NULL REFERENCES gift_cards(gift_card_id),
    transaction_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    type VARCHAR(20) NOT NULL, -- Load, Redeem, Refund, Adjust
    amount NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    balance_before NUMERIC(18,2) NOT NULL,
    balance_after NUMERIC(18,2) NOT NULL,
    order_id INT REFERENCES orders(order_id),
    user_id INT REFERENCES users(user_id),
    notes VARCHAR(255)
);

-- ============================================
-- 11. INVENTORY & PURCHASING
-- ============================================

-- 11.1 Suppliers
CREATE TABLE suppliers (
    supplier_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    name VARCHAR(100) NOT NULL,
    contact_person VARCHAR(100),
    phone VARCHAR(50),
    email VARCHAR(100),
    address VARCHAR(255),
    payment_terms VARCHAR(50),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 11.2 Purchase Orders
CREATE TABLE purchase_orders (
    purchase_order_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    supplier_id INT NOT NULL REFERENCES suppliers(supplier_id),
    po_number VARCHAR(50) NOT NULL,
    po_date DATE NOT NULL,
    status VARCHAR(20) NOT NULL, -- Draft, Approved, Closed, Canceled
    expected_delivery_date DATE,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    total_estimated_amount NUMERIC(18,2) NOT NULL DEFAULT 0
);

-- 11.3 Purchase Order Lines
CREATE TABLE purchase_order_lines (
    purchase_order_line_id SERIAL PRIMARY KEY,
    purchase_order_id INT NOT NULL REFERENCES purchase_orders(purchase_order_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    ordered_qty NUMERIC(18,4) NOT NULL,
    expected_unit_cost NUMERIC(18,4) NOT NULL,
    line_estimated_total NUMERIC(18,2) NOT NULL
);

-- 11.4 Goods Receipts
CREATE TABLE goods_receipts (
    goods_receipt_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    supplier_id INT NOT NULL REFERENCES suppliers(supplier_id),
    purchase_order_id INT REFERENCES purchase_orders(purchase_order_id),
    grn_number VARCHAR(50) NOT NULL,
    grn_date TIMESTAMPTZ NOT NULL,
    status VARCHAR(20) NOT NULL, -- Draft, Posted
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    total_before_tax NUMERIC(18,2) NOT NULL DEFAULT 0,
    tax_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    grand_total NUMERIC(18,2) NOT NULL DEFAULT 0
);

-- 11.5 Goods Receipt Lines
CREATE TABLE goods_receipt_lines (
    goods_receipt_line_id SERIAL PRIMARY KEY,
    goods_receipt_id INT NOT NULL REFERENCES goods_receipts(goods_receipt_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    received_qty NUMERIC(18,4) NOT NULL,
    unit_cost NUMERIC(18,4) NOT NULL,
    line_total NUMERIC(18,2) NOT NULL
);

-- 11.6 Stock Transactions
CREATE TABLE stock_transactions (
    stock_transaction_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    transaction_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    type VARCHAR(20) NOT NULL, -- IN-Purchase, OUT-Sales, OUT-Waste, IN-Adjust, OUT-Adjust, TransferIn, TransferOut, Reserve, ReleaseReserve
    quantity NUMERIC(18,4) NOT NULL,
    cost_per_unit NUMERIC(18,4),
    reference_type VARCHAR(20),
    reference_id INT,
    user_id INT REFERENCES users(user_id),
    reason VARCHAR(255)
);

-- 11.6.1 Branch Inventory (Snapshot)
CREATE TABLE branch_inventory (
    branch_inventory_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    quantity_on_hand NUMERIC(18,4) NOT NULL DEFAULT 0,
    quantity_reserved NUMERIC(18,4) NOT NULL DEFAULT 0,
    average_cost NUMERIC(18,4) NOT NULL DEFAULT 0,
    last_updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_transaction_id INT REFERENCES stock_transactions(stock_transaction_id),
    UNIQUE(branch_id, inventory_item_id)
);

-- 11.7 Stock Adjustments
CREATE TABLE stock_adjustments (
    stock_adjustment_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    adjustment_date TIMESTAMPTZ NOT NULL,
    status VARCHAR(20) NOT NULL, -- Draft, Posted
    user_id INT NOT NULL REFERENCES users(user_id),
    notes VARCHAR(255)
);

-- 11.8 Stock Adjustment Lines
CREATE TABLE stock_adjustment_lines (
    stock_adjustment_line_id SERIAL PRIMARY KEY,
    stock_adjustment_id INT NOT NULL REFERENCES stock_adjustments(stock_adjustment_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    system_qty NUMERIC(18,4) NOT NULL,
    physical_qty NUMERIC(18,4) NOT NULL,
    difference_qty NUMERIC(18,4) NOT NULL
);

-- 11.9 Stock Counts
CREATE TABLE stock_counts (
    stock_count_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    count_date TIMESTAMPTZ NOT NULL,
    area VARCHAR(50),
    status VARCHAR(20) NOT NULL -- Draft, Completed, Posted
);

-- 11.10 Stock Count Lines
CREATE TABLE stock_count_lines (
    stock_count_line_id SERIAL PRIMARY KEY,
    stock_count_id INT NOT NULL REFERENCES stock_counts(stock_count_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    system_qty NUMERIC(18,4) NOT NULL,
    physical_qty NUMERIC(18,4) NOT NULL
);

-- 11.11 Wastage Records
CREATE TABLE wastage_records (
    wastage_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    wastage_date TIMESTAMPTZ NOT NULL,
    quantity NUMERIC(18,4) NOT NULL,
    reason VARCHAR(255) NOT NULL,
    user_id INT NOT NULL REFERENCES users(user_id)
);

-- 11.12 Inventory Reservations
CREATE TABLE inventory_reservations (
    reservation_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    order_id INT NOT NULL REFERENCES orders(order_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    reserved_qty NUMERIC(18,4) NOT NULL,
    reserved_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    status VARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Released, Consumed
    released_at TIMESTAMPTZ,
    UNIQUE(order_id, inventory_item_id)
);

-- ============================================
-- 12. HR & COMMISSIONS
-- ============================================

-- 12.1 Employees
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    user_id INT REFERENCES users(user_id),
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    full_name VARCHAR(100) NOT NULL,
    position VARCHAR(50) NOT NULL,
    base_salary NUMERIC(18,2),
    hire_date DATE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 12.2 Attendance Records
CREATE TABLE attendance_records (
    attendance_id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL REFERENCES employees(employee_id),
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    date DATE NOT NULL,
    clock_in TIMESTAMPTZ,
    clock_out TIMESTAMPTZ,
    total_hours NUMERIC(18,2)
);

-- 12.3 Commission Policies
CREATE TABLE commission_policies (
    commission_policy_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    sales_percent NUMERIC(5,2) NOT NULL DEFAULT 0,
    fixed_per_invoice NUMERIC(18,2) NOT NULL DEFAULT 0,
    apply_on_net_before_tax BOOLEAN NOT NULL DEFAULT TRUE,
    exclude_discounted_invoices BOOLEAN NOT NULL DEFAULT FALSE
);

-- 12.4 Commission Transactions
CREATE TABLE commission_transactions (
    commission_transaction_id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL REFERENCES employees(employee_id),
    order_id INT NOT NULL REFERENCES orders(order_id),
    commission_date TIMESTAMPTZ NOT NULL,
    sales_commission NUMERIC(18,2) NOT NULL DEFAULT 0,
    item_commission NUMERIC(18,2) NOT NULL DEFAULT 0,
    upsell_commission NUMERIC(18,2) NOT NULL DEFAULT 0,
    tips_share NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_commission NUMERIC(18,2) NOT NULL DEFAULT 0
);

-- ============================================
-- 13. APPROVALS & AUDIT
-- ============================================

-- 13.1 Approval Rules
CREATE TABLE approval_rules (
    approval_rule_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    rule_type VARCHAR(50) NOT NULL, -- MaxDiscount, PriceChange, VoidPaidInvoice
    role_id INT NOT NULL REFERENCES roles(role_id),
    max_discount_percent_without_approval NUMERIC(5,2),
    allow_price_change BOOLEAN,
    require_manager_pin_for_price_change BOOLEAN,
    can_void_paid_invoice BOOLEAN,
    require_manager_approval_for_void BOOLEAN
);

-- 13.2 Approval Requests
CREATE TABLE approval_requests (
    approval_request_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    rule_type VARCHAR(50) NOT NULL,
    requested_by_user_id INT NOT NULL REFERENCES users(user_id),
    related_order_id INT REFERENCES orders(order_id),
    requested_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    status VARCHAR(20) NOT NULL, -- Pending, Approved, Rejected
    approved_by_user_id INT REFERENCES users(user_id),
    approved_at TIMESTAMPTZ,
    details JSONB
);

-- 13.3 Audit Log
CREATE TABLE audit_log (
    audit_log_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    user_id INT REFERENCES users(user_id),
    branch_id INT REFERENCES branches(branch_id),
    action_type VARCHAR(50) NOT NULL,
    entity_name VARCHAR(50),
    entity_id INT,
    details JSONB
);

-- ============================================
-- 15. PRINTING & RECEIPTS
-- ============================================

-- 15.1 Printers
CREATE TABLE printers (
    printer_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    name VARCHAR(50) NOT NULL,
    printer_type VARCHAR(20) NOT NULL, -- Receipt, Kitchen, Label
    connection_type VARCHAR(20) NOT NULL, -- USB, Network, Bluetooth
    connection_string VARCHAR(255),
    paper_width INT NOT NULL DEFAULT 80,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- 15.2 Kitchen Station Printers
CREATE TABLE kitchen_station_printers (
    kitchen_station_printer_id SERIAL PRIMARY KEY,
    kitchen_station_id INT NOT NULL REFERENCES kitchen_stations(kitchen_station_id),
    printer_id INT NOT NULL REFERENCES printers(printer_id),
    UNIQUE(kitchen_station_id, printer_id)
);

-- 15.3 Receipt Templates
CREATE TABLE receipt_templates (
    receipt_template_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    template_type VARCHAR(20) NOT NULL, -- CustomerReceipt, KitchenTicket, DailyReport
    name VARCHAR(50) NOT NULL,
    header_text VARCHAR(500),
    footer_text VARCHAR(500),
    show_logo BOOLEAN NOT NULL DEFAULT TRUE,
    show_barcode BOOLEAN NOT NULL DEFAULT FALSE,
    language VARCHAR(10) NOT NULL DEFAULT 'en', -- en, ar, both
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- ============================================
-- 16. OFFLINE SYNC & QUEUE
-- ============================================

-- 16.1 Offline Queue
CREATE TABLE offline_queue (
    queue_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL,
    terminal_id VARCHAR(50) NOT NULL,
    entity_type VARCHAR(50) NOT NULL,
    entity_local_id VARCHAR(50) NOT NULL,
    entity_server_id INT,
    payload JSONB NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    status VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Syncing, Synced, Failed
    synced_at TIMESTAMPTZ,
    retry_count INT NOT NULL DEFAULT 0,
    last_error VARCHAR(500)
);

-- 16.2 Sync Log
CREATE TABLE sync_log (
    sync_log_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    terminal_id VARCHAR(50) NOT NULL,
    sync_started_at TIMESTAMPTZ NOT NULL,
    sync_completed_at TIMESTAMPTZ,
    status VARCHAR(20) NOT NULL, -- InProgress, Completed, PartialFailure, Failed
    total_records INT NOT NULL DEFAULT 0,
    synced_records INT NOT NULL DEFAULT 0,
    failed_records INT NOT NULL DEFAULT 0,
    error_details JSONB
);

-- 16.3 Conflict Resolutions
CREATE TABLE conflict_resolutions (
    conflict_id SERIAL PRIMARY KEY,
    sync_log_id INT NOT NULL REFERENCES sync_log(sync_log_id),
    entity_type VARCHAR(50) NOT NULL,
    entity_id INT NOT NULL,
    local_payload JSONB NOT NULL,
    server_payload JSONB NOT NULL,
    resolution VARCHAR(20) NOT NULL, -- LocalWins, ServerWins, Merged, Manual
    resolved_payload JSONB,
    resolved_at TIMESTAMPTZ,
    resolved_by_user_id INT REFERENCES users(user_id)
);

-- ============================================
-- 17. TABLE OPERATIONS
-- ============================================

-- 17.1 Table Merges
CREATE TABLE table_merges (
    table_merge_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    target_order_id INT NOT NULL REFERENCES orders(order_id),
    merged_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    merged_by_user_id INT NOT NULL REFERENCES users(user_id)
);

-- 17.2 Table Merge Details
CREATE TABLE table_merge_details (
    table_merge_detail_id SERIAL PRIMARY KEY,
    table_merge_id INT NOT NULL REFERENCES table_merges(table_merge_id),
    source_order_id INT NOT NULL REFERENCES orders(order_id),
    source_table_id INT NOT NULL REFERENCES restaurant_tables(table_id)
);

-- 17.3 Table Splits
CREATE TABLE table_splits (
    table_split_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    source_order_id INT NOT NULL REFERENCES orders(order_id),
    split_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    split_by_user_id INT NOT NULL REFERENCES users(user_id),
    split_type VARCHAR(20) NOT NULL -- ByItem, ByAmount, ByGuest, Equal
);

-- 17.4 Table Split Details
CREATE TABLE table_split_details (
    table_split_detail_id SERIAL PRIMARY KEY,
    table_split_id INT NOT NULL REFERENCES table_splits(table_split_id),
    new_order_id INT NOT NULL REFERENCES orders(order_id),
    amount NUMERIC(18,2) NOT NULL
);

-- ============================================
-- 18. SYSTEM SETTINGS
-- ============================================

-- 18.1 System Settings
CREATE TABLE system_settings (
    setting_id SERIAL PRIMARY KEY,
    company_id INT REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    setting_key VARCHAR(100) NOT NULL,
    setting_value TEXT,
    setting_type VARCHAR(20) NOT NULL, -- String, Integer, Decimal, Boolean, JSON
    description VARCHAR(255),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by_user_id INT REFERENCES users(user_id),
    UNIQUE(company_id, branch_id, setting_key)
);

-- ============================================
-- INDEXES FOR PERFORMANCE
-- ============================================

-- Companies
CREATE INDEX idx_companies_status ON companies(status);
CREATE INDEX idx_companies_plan_expiry ON companies(plan_expiry_date);

-- Branches
CREATE INDEX idx_branches_company ON branches(company_id);

-- Users
CREATE INDEX idx_users_company ON users(company_id);
CREATE INDEX idx_users_branch ON users(default_branch_id);

-- Orders
CREATE INDEX idx_orders_company ON orders(company_id);
CREATE INDEX idx_orders_branch ON orders(branch_id);
CREATE INDEX idx_orders_customer ON orders(customer_id);
CREATE INDEX idx_orders_status ON orders(order_status);
CREATE INDEX idx_orders_created ON orders(created_at);
CREATE INDEX idx_orders_shift ON orders(shift_id);

-- Order Lines
CREATE INDEX idx_order_lines_order ON order_lines(order_id);
CREATE INDEX idx_order_lines_menu_item ON order_lines(menu_item_id);

-- Customers
CREATE INDEX idx_customers_company ON customers(company_id);
CREATE INDEX idx_customers_phone ON customers(phone);

-- Menu Items
CREATE INDEX idx_menu_items_company ON menu_items(company_id);
CREATE INDEX idx_menu_items_category ON menu_items(category_id);

-- Stock Transactions
CREATE INDEX idx_stock_transactions_branch ON stock_transactions(branch_id);
CREATE INDEX idx_stock_transactions_item ON stock_transactions(inventory_item_id);
CREATE INDEX idx_stock_transactions_date ON stock_transactions(transaction_date);

-- Audit Log
CREATE INDEX idx_audit_log_company ON audit_log(company_id);
CREATE INDEX idx_audit_log_user ON audit_log(user_id);
CREATE INDEX idx_audit_log_timestamp ON audit_log(timestamp);

-- ============================================
-- SEED DATA
-- ============================================

-- Default Currencies
INSERT INTO currencies (currency_code, name, symbol, decimal_places, is_default, is_active) VALUES
('USD', 'US Dollar', '$', 2, TRUE, TRUE),
('EUR', 'Euro', '€', 2, FALSE, TRUE),
('GBP', 'British Pound', '£', 2, FALSE, TRUE),
('LBP', 'Lebanese Pound', 'ل.ل', 0, FALSE, TRUE),
('AED', 'UAE Dirham', 'د.إ', 2, FALSE, TRUE),
('SAR', 'Saudi Riyal', 'ر.س', 2, FALSE, TRUE);

-- Default Permissions
INSERT INTO permissions (code, description) VALUES
('POS.OPEN_SHIFT', 'Can open a new shift'),
('POS.CLOSE_SHIFT', 'Can close a shift'),
('POS.CREATE_ORDER', 'Can create orders'),
('POS.VOID_ORDER', 'Can void orders'),
('POS.APPLY_DISCOUNT', 'Can apply discounts'),
('POS.CHANGE_PRICE', 'Can change item prices'),
('DISCOUNT.APPROVE_HIGH', 'Can approve high discounts'),
('VOID.APPROVE', 'Can approve void requests'),
('INVENTORY.VIEW', 'Can view inventory'),
('INVENTORY.ADJUST', 'Can adjust stock'),
('INVENTORY.CREATE_PO', 'Can create purchase orders'),
('INVENTORY.APPROVE_PO', 'Can approve purchase orders'),
('REPORTS.VIEW_SALES', 'Can view sales reports'),
('REPORTS.VIEW_INVENTORY', 'Can view inventory reports'),
('SETTINGS.MANAGE', 'Can manage settings'),
('USERS.MANAGE', 'Can manage users'),
('MENU.MANAGE', 'Can manage menu items');

-- Default SuperAdmin (password: admin123)
-- Note: Replace this hash with actual bcrypt hash in production
INSERT INTO super_admins (username, password_hash, full_name, email) VALUES
('superadmin', decode('243261243130244a4b7865786d694b4f7a4c657a376a6d4c3262716c75', 'hex'), 'Super Admin', 'admin@restaurant.com');

-- Default Subscription Plans
INSERT INTO subscription_plans (name, description, price, currency_code, billing_cycle, duration_days, max_branches, max_users, features, is_active, sort_order) VALUES
('Basic', 'Perfect for small restaurants', 29.99, 'USD', 'Monthly', 30, 1, 5, '["pos_module"]', TRUE, 1),
('Professional', 'For growing restaurants', 79.99, 'USD', 'Monthly', 30, 3, 15, '["pos_module", "inventory_module", "loyalty_module"]', TRUE, 2),
('Enterprise', 'Full-featured for large operations', 199.99, 'USD', 'Monthly', 30, 10, 50, '["pos_module", "inventory_module", "loyalty_module", "delivery_module", "multi_language", "api_access", "priority_support"]', TRUE, 3),
('Basic Annual', 'Basic plan - yearly billing', 299.99, 'USD', 'Yearly', 365, 1, 5, '["pos_module"]', TRUE, 4),
('Professional Annual', 'Professional plan - yearly billing', 799.99, 'USD', 'Yearly', 365, 3, 15, '["pos_module", "inventory_module", "loyalty_module"]', TRUE, 5),
('Enterprise Annual', 'Enterprise plan - yearly billing', 1999.99, 'USD', 'Yearly', 365, 10, 50, '["pos_module", "inventory_module", "loyalty_module", "delivery_module", "multi_language", "api_access", "priority_support"]', TRUE, 6);

-- ============================================
-- 19. ENHANCED FEATURES (V2)
-- ============================================

-- 19.1 Table Status Enhancement
ALTER TABLE restaurant_tables ADD COLUMN status VARCHAR(20) DEFAULT 'Available';
-- Status: Available, Occupied, Reserved, NeedsCleaning, OutOfService

ALTER TABLE restaurant_tables ADD COLUMN floor_number INT DEFAULT 1;
ALTER TABLE restaurant_tables ADD COLUMN position_x INT;
ALTER TABLE restaurant_tables ADD COLUMN position_y INT;

-- 19.2 Table Sections (Waiter Stations)
CREATE TABLE table_sections (
    section_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    name VARCHAR(50) NOT NULL,
    color VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE
);

ALTER TABLE restaurant_tables ADD COLUMN section_id INT REFERENCES table_sections(section_id);

-- 19.3 Waiter Section Assignments
CREATE TABLE waiter_sections (
    waiter_section_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(user_id),
    section_id INT NOT NULL REFERENCES table_sections(section_id),
    shift_id INT REFERENCES shifts(shift_id),
    assigned_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(user_id, section_id, shift_id)
);

-- ============================================
-- 20. PROMOTIONS & DISCOUNTS
-- ============================================

-- 20.1 Promotions
CREATE TABLE promotions (
    promotion_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    code VARCHAR(50), -- Coupon code (optional)
    type VARCHAR(30) NOT NULL, -- Percentage, FixedAmount, BOGO, BuyXGetY, FreeItem, Bundle
    discount_value NUMERIC(18,2),
    buy_quantity INT, -- For BOGO/BuyXGetY
    get_quantity INT, -- For BOGO/BuyXGetY
    free_item_id INT REFERENCES menu_items(menu_item_id), -- For FreeItem promo
    min_order_amount NUMERIC(18,2),
    max_discount_amount NUMERIC(18,2),
    applies_to VARCHAR(30) DEFAULT 'AllItems', -- AllItems, Category, SpecificItems
    applicable_category_ids JSONB, -- Array of category IDs
    applicable_item_ids JSONB, -- Array of menu_item IDs
    valid_from TIMESTAMPTZ NOT NULL,
    valid_to TIMESTAMPTZ,
    valid_days VARCHAR(50), -- Mon,Tue,Wed,Thu,Fri,Sat,Sun or ALL
    valid_hours_start TIME,
    valid_hours_end TIME,
    order_types VARCHAR(50) DEFAULT 'ALL', -- DineIn,Takeaway,Delivery or ALL
    customer_types VARCHAR(50) DEFAULT 'ALL', -- ALL, NewCustomers, LoyaltyMembers
    max_uses_total INT,
    max_uses_per_customer INT,
    current_uses INT DEFAULT 0,
    is_auto_apply BOOLEAN DEFAULT FALSE, -- Auto-apply if conditions met
    is_combinable BOOLEAN DEFAULT FALSE, -- Can combine with other promos
    priority INT DEFAULT 0, -- Higher = applied first
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id),
    updated_at TIMESTAMPTZ
);

-- 20.2 Promotion Usage Tracking
CREATE TABLE promotion_usages (
    usage_id SERIAL PRIMARY KEY,
    promotion_id INT NOT NULL REFERENCES promotions(promotion_id),
    order_id INT NOT NULL REFERENCES orders(order_id),
    customer_id INT REFERENCES customers(customer_id),
    discount_amount NUMERIC(18,2) NOT NULL,
    used_at TIMESTAMPTZ DEFAULT NOW()
);

-- 20.3 Order Promotions (Link orders to promotions)
CREATE TABLE order_promotions (
    order_promotion_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id),
    promotion_id INT NOT NULL REFERENCES promotions(promotion_id),
    discount_amount NUMERIC(18,2) NOT NULL,
    applied_at TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================
-- 21. DELIVERY MANAGEMENT
-- ============================================

-- 21.1 Drivers
CREATE TABLE drivers (
    driver_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    user_id INT REFERENCES users(user_id), -- If driver has app login
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(50) NOT NULL,
    email VARCHAR(100),
    vehicle_type VARCHAR(30), -- Motorcycle, Car, Bicycle, OnFoot
    vehicle_plate VARCHAR(30),
    vehicle_model VARCHAR(50),
    license_number VARCHAR(50),
    license_expiry DATE,
    is_internal BOOLEAN DEFAULT TRUE, -- FALSE = third-party
    third_party_provider VARCHAR(50), -- UberEats, Deliveroo, Talabat, etc.
    commission_type VARCHAR(20), -- PerOrder, Percentage
    commission_value NUMERIC(18,2),
    is_available BOOLEAN DEFAULT TRUE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 21.2 Driver Shifts
CREATE TABLE driver_shifts (
    driver_shift_id SERIAL PRIMARY KEY,
    driver_id INT NOT NULL REFERENCES drivers(driver_id),
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    shift_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    actual_start TIMESTAMPTZ,
    actual_end TIMESTAMPTZ,
    deliveries_count INT DEFAULT 0,
    total_earnings NUMERIC(18,2) DEFAULT 0,
    status VARCHAR(20) DEFAULT 'Scheduled' -- Scheduled, Active, Completed, NoShow
);

-- 21.3 Delivery Orders Enhancement
ALTER TABLE order_delivery_details ADD COLUMN driver_id INT REFERENCES drivers(driver_id);
ALTER TABLE order_delivery_details ADD COLUMN delivery_status VARCHAR(30) DEFAULT 'Pending';
-- Pending, Assigned, Preparing, ReadyForPickup, PickedUp, EnRoute, Arrived, Delivered, Failed, Returned
ALTER TABLE order_delivery_details ADD COLUMN estimated_prep_time INT; -- minutes
ALTER TABLE order_delivery_details ADD COLUMN estimated_delivery_time TIMESTAMPTZ;
ALTER TABLE order_delivery_details ADD COLUMN actual_delivery_time TIMESTAMPTZ;
ALTER TABLE order_delivery_details ADD COLUMN delivery_instructions TEXT;
ALTER TABLE order_delivery_details ADD COLUMN contactless_delivery BOOLEAN DEFAULT FALSE;
ALTER TABLE order_delivery_details ADD COLUMN driver_tip NUMERIC(18,2) DEFAULT 0;
ALTER TABLE order_delivery_details ADD COLUMN delivery_rating INT CHECK (delivery_rating >= 1 AND delivery_rating <= 5);
ALTER TABLE order_delivery_details ADD COLUMN delivery_feedback TEXT;

-- 21.4 Driver Location History
CREATE TABLE driver_locations (
    location_id SERIAL PRIMARY KEY,
    driver_id INT NOT NULL REFERENCES drivers(driver_id),
    latitude NUMERIC(9,6) NOT NULL,
    longitude NUMERIC(9,6) NOT NULL,
    recorded_at TIMESTAMPTZ DEFAULT NOW(),
    order_id INT REFERENCES orders(order_id) -- Current delivery
);

-- ============================================
-- 22. REFUNDS & ADJUSTMENTS
-- ============================================

-- 22.1 Refunds
CREATE TABLE refunds (
    refund_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    order_id INT NOT NULL REFERENCES orders(order_id),
    original_payment_id INT REFERENCES order_payments(order_payment_id),
    refund_amount NUMERIC(18,2) NOT NULL,
    refund_type VARCHAR(20) NOT NULL, -- Full, Partial
    reason_category VARCHAR(50), -- WrongOrder, FoodQuality, LateDelivery, CustomerRequest, Other
    reason VARCHAR(255),
    status VARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Processed, Rejected
    refund_method VARCHAR(30), -- SameAsPayment, Cash, StoreCredit, GiftCard
    store_credit_amount NUMERIC(18,2),
    gift_card_id INT REFERENCES gift_cards(gift_card_id),
    requested_at TIMESTAMPTZ DEFAULT NOW(),
    requested_by_user_id INT NOT NULL REFERENCES users(user_id),
    approved_at TIMESTAMPTZ,
    approved_by_user_id INT REFERENCES users(user_id),
    processed_at TIMESTAMPTZ,
    processed_by_user_id INT REFERENCES users(user_id),
    rejected_reason VARCHAR(255)
);

-- 22.2 Refund Lines (For partial refunds)
CREATE TABLE refund_lines (
    refund_line_id SERIAL PRIMARY KEY,
    refund_id INT NOT NULL REFERENCES refunds(refund_id),
    order_line_id INT NOT NULL REFERENCES order_lines(order_line_id),
    quantity NUMERIC(18,4) NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    reason VARCHAR(255)
);

-- ============================================
-- 23. ORDER ENHANCEMENTS
-- ============================================

-- Order Source & Scheduling
ALTER TABLE orders ADD COLUMN order_source VARCHAR(30) DEFAULT 'POS';
-- POS, Website, MobileApp, Phone, UberEats, Deliveroo, Talabat, WhatsApp, Zomato, GrabFood
ALTER TABLE orders ADD COLUMN external_order_id VARCHAR(100); -- ID from third-party platform
ALTER TABLE orders ADD COLUMN scheduled_for TIMESTAMPTZ; -- Future order
ALTER TABLE orders ADD COLUMN is_preorder BOOLEAN DEFAULT FALSE;
ALTER TABLE orders ADD COLUMN special_instructions TEXT;

-- Order Line Enhancements
ALTER TABLE order_lines ADD COLUMN course_number INT DEFAULT 1; -- 1=appetizer, 2=main, 3=dessert
ALTER TABLE order_lines ADD COLUMN seat_number INT;
ALTER TABLE order_lines ADD COLUMN is_sent_to_kitchen BOOLEAN DEFAULT FALSE;
ALTER TABLE order_lines ADD COLUMN sent_to_kitchen_at TIMESTAMPTZ;
ALTER TABLE order_lines ADD COLUMN started_prep_at TIMESTAMPTZ;
ALTER TABLE order_lines ADD COLUMN ready_at TIMESTAMPTZ;
ALTER TABLE order_lines ADD COLUMN served_at TIMESTAMPTZ;
ALTER TABLE order_lines ADD COLUMN prep_time_minutes INT;
ALTER TABLE order_lines ADD COLUMN priority VARCHAR(20) DEFAULT 'Normal'; -- Rush, VIP, Normal
ALTER TABLE order_lines ADD COLUMN is_gift BOOLEAN DEFAULT FALSE; -- Don't show price on receipt
ALTER TABLE order_lines ADD COLUMN void_reason VARCHAR(255);
ALTER TABLE order_lines ADD COLUMN voided_by_user_id INT REFERENCES users(user_id);

-- Guest Count
ALTER TABLE orders ADD COLUMN guest_count INT DEFAULT 1;

-- ============================================
-- 24. MENU ENHANCEMENTS
-- ============================================

-- Menu Item Enhancements
ALTER TABLE menu_items ADD COLUMN image_url VARCHAR(500);
ALTER TABLE menu_items ADD COLUMN thumbnail_url VARCHAR(500);
ALTER TABLE menu_items ADD COLUMN calories INT;
ALTER TABLE menu_items ADD COLUMN prep_time_minutes INT;
ALTER TABLE menu_items ADD COLUMN allergens VARCHAR(255); -- Comma-separated: gluten,dairy,nuts,shellfish,eggs,soy
ALTER TABLE menu_items ADD COLUMN dietary_tags VARCHAR(255); -- vegetarian,vegan,halal,kosher,gluten-free
ALTER TABLE menu_items ADD COLUMN spice_level INT CHECK (spice_level >= 0 AND spice_level <= 5);
ALTER TABLE menu_items ADD COLUMN is_available BOOLEAN DEFAULT TRUE; -- 86'd tracking
ALTER TABLE menu_items ADD COLUMN unavailable_until TIMESTAMPTZ;
ALTER TABLE menu_items ADD COLUMN unavailable_reason VARCHAR(100);
ALTER TABLE menu_items ADD COLUMN is_featured BOOLEAN DEFAULT FALSE;
ALTER TABLE menu_items ADD COLUMN is_new BOOLEAN DEFAULT FALSE;

-- Branch-Specific Pricing
CREATE TABLE branch_menu_prices (
    branch_price_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    menu_item_id INT NOT NULL REFERENCES menu_items(menu_item_id),
    menu_item_size_id INT REFERENCES menu_item_sizes(menu_item_size_id),
    price NUMERIC(18,2) NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    valid_from TIMESTAMPTZ DEFAULT NOW(),
    valid_to TIMESTAMPTZ,
    UNIQUE(branch_id, menu_item_id, menu_item_size_id)
);

-- 24.1 Combo/Meal Deals
CREATE TABLE combo_deals (
    combo_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    name VARCHAR(100) NOT NULL,
    name_ar VARCHAR(100),
    description VARCHAR(255),
    image_url VARCHAR(500),
    combo_price NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    savings_amount NUMERIC(18,2), -- How much customer saves
    valid_from TIMESTAMPTZ,
    valid_to TIMESTAMPTZ,
    valid_hours_start TIME,
    valid_hours_end TIME,
    is_active BOOLEAN DEFAULT TRUE,
    sort_order INT DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE combo_items (
    combo_item_id SERIAL PRIMARY KEY,
    combo_id INT NOT NULL REFERENCES combo_deals(combo_id),
    menu_item_id INT REFERENCES menu_items(menu_item_id), -- Specific item
    category_id INT REFERENCES categories(category_id), -- OR choose from category
    group_name VARCHAR(50), -- "Choose your drink", "Choose your side"
    quantity INT NOT NULL DEFAULT 1,
    is_required BOOLEAN DEFAULT TRUE,
    extra_price NUMERIC(18,2) DEFAULT 0, -- Upgrade price
    sort_order INT DEFAULT 0
);

-- ============================================
-- 25. CUSTOMER ENHANCEMENTS
-- ============================================

-- Customer Additional Fields
ALTER TABLE customers ADD COLUMN date_of_birth DATE;
ALTER TABLE customers ADD COLUMN gender VARCHAR(10);
ALTER TABLE customers ADD COLUMN language_preference VARCHAR(10) DEFAULT 'en';
ALTER TABLE customers ADD COLUMN allergies VARCHAR(500);
ALTER TABLE customers ADD COLUMN dietary_preferences VARCHAR(255); -- vegetarian,vegan,halal
ALTER TABLE customers ADD COLUMN favorite_items JSONB; -- Array of menu_item_ids
ALTER TABLE customers ADD COLUMN total_orders INT DEFAULT 0;
ALTER TABLE customers ADD COLUMN total_spent NUMERIC(18,2) DEFAULT 0;
ALTER TABLE customers ADD COLUMN average_order_value NUMERIC(18,2) DEFAULT 0;
ALTER TABLE customers ADD COLUMN last_order_date TIMESTAMPTZ;
ALTER TABLE customers ADD COLUMN is_blacklisted BOOLEAN DEFAULT FALSE;
ALTER TABLE customers ADD COLUMN blacklist_reason VARCHAR(255);
ALTER TABLE customers ADD COLUMN blacklisted_at TIMESTAMPTZ;
ALTER TABLE customers ADD COLUMN marketing_opt_in BOOLEAN DEFAULT TRUE;
ALTER TABLE customers ADD COLUMN source VARCHAR(30); -- WalkIn, Website, App, Referral, Social

-- 25.1 Customer Feedback/Reviews
CREATE TABLE customer_feedback (
    feedback_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    customer_id INT REFERENCES customers(customer_id),
    order_id INT REFERENCES orders(order_id),
    overall_rating INT CHECK (overall_rating >= 1 AND overall_rating <= 5),
    food_rating INT CHECK (food_rating >= 1 AND food_rating <= 5),
    service_rating INT CHECK (service_rating >= 1 AND service_rating <= 5),
    ambiance_rating INT CHECK (ambiance_rating >= 1 AND ambiance_rating <= 5),
    delivery_rating INT CHECK (delivery_rating >= 1 AND delivery_rating <= 5),
    value_rating INT CHECK (value_rating >= 1 AND value_rating <= 5),
    comments TEXT,
    would_recommend BOOLEAN,
    is_public BOOLEAN DEFAULT FALSE,
    source VARCHAR(30), -- InApp, Email, Google, TripAdvisor, Yelp
    created_at TIMESTAMPTZ DEFAULT NOW(),
    responded_at TIMESTAMPTZ,
    responded_by_user_id INT REFERENCES users(user_id),
    response TEXT,
    is_resolved BOOLEAN DEFAULT FALSE
);

-- 25.2 Customer Complaints
CREATE TABLE customer_complaints (
    complaint_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    customer_id INT REFERENCES customers(customer_id),
    order_id INT REFERENCES orders(order_id),
    complaint_type VARCHAR(50), -- FoodQuality, Service, Delivery, Billing, Other
    severity VARCHAR(20) DEFAULT 'Medium', -- Low, Medium, High, Critical
    description TEXT NOT NULL,
    status VARCHAR(20) DEFAULT 'Open', -- Open, InProgress, Resolved, Closed
    assigned_to_user_id INT REFERENCES users(user_id),
    resolution TEXT,
    compensation_type VARCHAR(30), -- Refund, Discount, FreeItem, GiftCard, None
    compensation_value NUMERIC(18,2),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    resolved_at TIMESTAMPTZ,
    resolved_by_user_id INT REFERENCES users(user_id)
);

-- 25.3 Store Credits
CREATE TABLE store_credits (
    store_credit_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    customer_id INT NOT NULL REFERENCES customers(customer_id),
    initial_amount NUMERIC(18,2) NOT NULL,
    current_balance NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    reason VARCHAR(255),
    source_type VARCHAR(30), -- Refund, Complaint, Promotion, Manual
    source_id INT, -- refund_id, complaint_id, etc.
    expiry_date TIMESTAMPTZ,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id)
);

CREATE TABLE store_credit_transactions (
    transaction_id SERIAL PRIMARY KEY,
    store_credit_id INT NOT NULL REFERENCES store_credits(store_credit_id),
    order_id INT REFERENCES orders(order_id),
    transaction_type VARCHAR(20) NOT NULL, -- Credit, Debit
    amount NUMERIC(18,2) NOT NULL,
    balance_before NUMERIC(18,2) NOT NULL,
    balance_after NUMERIC(18,2) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    user_id INT REFERENCES users(user_id)
);

-- ============================================
-- 26. CASH MANAGEMENT
-- ============================================

-- 26.1 Cash Drawer Operations
CREATE TABLE cash_drawer_operations (
    operation_id SERIAL PRIMARY KEY,
    shift_id INT NOT NULL REFERENCES shifts(shift_id),
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    operation_type VARCHAR(30) NOT NULL, -- PayIn, PayOut, SafeDrop, NoSaleOpen, FloatAdjust
    amount NUMERIC(18,2) NOT NULL,
    reason VARCHAR(255),
    reference_number VARCHAR(50),
    user_id INT NOT NULL REFERENCES users(user_id),
    approved_by_user_id INT REFERENCES users(user_id),
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 26.2 Safe Management
CREATE TABLE safe_transactions (
    safe_transaction_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    transaction_type VARCHAR(30) NOT NULL, -- Deposit, Withdrawal, BankDeposit, FloatToDrawer
    amount NUMERIC(18,2) NOT NULL,
    currency_code VARCHAR(3) NOT NULL REFERENCES currencies(currency_code),
    shift_id INT REFERENCES shifts(shift_id),
    reference_number VARCHAR(50),
    notes VARCHAR(255),
    user_id INT NOT NULL REFERENCES users(user_id),
    witnessed_by_user_id INT REFERENCES users(user_id),
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================
-- 27. STAFF SCHEDULING
-- ============================================

-- 27.1 Staff Schedules
CREATE TABLE staff_schedules (
    schedule_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    employee_id INT NOT NULL REFERENCES employees(employee_id),
    schedule_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    break_minutes INT DEFAULT 0,
    position VARCHAR(50), -- Cashier, Waiter, Kitchen, Host, Manager, Barista
    section_id INT REFERENCES table_sections(section_id),
    notes VARCHAR(255),
    status VARCHAR(20) DEFAULT 'Scheduled', -- Scheduled, Confirmed, NoShow, Completed
    created_at TIMESTAMPTZ DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id),
    UNIQUE(employee_id, schedule_date, start_time)
);

-- 27.2 Shift Swap Requests
CREATE TABLE shift_swap_requests (
    swap_id SERIAL PRIMARY KEY,
    original_schedule_id INT NOT NULL REFERENCES staff_schedules(schedule_id),
    requesting_employee_id INT NOT NULL REFERENCES employees(employee_id),
    target_employee_id INT REFERENCES employees(employee_id),
    target_schedule_id INT REFERENCES staff_schedules(schedule_id),
    reason VARCHAR(255),
    status VARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected, Cancelled
    requested_at TIMESTAMPTZ DEFAULT NOW(),
    responded_at TIMESTAMPTZ,
    approved_by_user_id INT REFERENCES users(user_id)
);

-- 27.3 Time Off Requests
CREATE TABLE time_off_requests (
    request_id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL REFERENCES employees(employee_id),
    request_type VARCHAR(30) NOT NULL, -- Vacation, Sick, Personal, Unpaid
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    reason VARCHAR(255),
    status VARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected
    requested_at TIMESTAMPTZ DEFAULT NOW(),
    approved_at TIMESTAMPTZ,
    approved_by_user_id INT REFERENCES users(user_id),
    rejection_reason VARCHAR(255)
);

-- ============================================
-- 28. INVENTORY ENHANCEMENTS
-- ============================================

-- 28.1 Inventory Expiry/Batch Tracking
ALTER TABLE inventory_items ADD COLUMN track_expiry BOOLEAN DEFAULT FALSE;
ALTER TABLE inventory_items ADD COLUMN track_batch BOOLEAN DEFAULT FALSE;
ALTER TABLE inventory_items ADD COLUMN default_expiry_days INT;
ALTER TABLE inventory_items ADD COLUMN storage_location VARCHAR(50);

CREATE TABLE inventory_batches (
    batch_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id),
    batch_number VARCHAR(50),
    quantity NUMERIC(18,4) NOT NULL,
    quantity_remaining NUMERIC(18,4) NOT NULL,
    cost_per_unit NUMERIC(18,4) NOT NULL,
    received_date DATE NOT NULL,
    production_date DATE,
    expiry_date DATE,
    supplier_id INT REFERENCES suppliers(supplier_id),
    goods_receipt_line_id INT REFERENCES goods_receipt_lines(goods_receipt_line_id),
    status VARCHAR(20) DEFAULT 'Active', -- Active, LowStock, Expired, Depleted
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- 28.2 Low Stock Alerts Configuration
CREATE TABLE inventory_alert_settings (
    alert_setting_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    branch_id INT REFERENCES branches(branch_id),
    inventory_item_id INT REFERENCES inventory_items(inventory_item_id),
    alert_type VARCHAR(30) NOT NULL, -- LowStock, Expiring, Expired, Reorder
    threshold_quantity NUMERIC(18,4),
    days_before_expiry INT,
    notify_users JSONB, -- Array of user_ids
    notify_email BOOLEAN DEFAULT TRUE,
    notify_push BOOLEAN DEFAULT TRUE,
    is_active BOOLEAN DEFAULT TRUE
);

-- ============================================
-- 29. CORPORATE ACCOUNTS
-- ============================================

-- 29.1 Corporate Accounts (Credit Customers)
CREATE TABLE corporate_accounts (
    corporate_account_id SERIAL PRIMARY KEY,
    company_id INT NOT NULL REFERENCES companies(company_id),
    account_name VARCHAR(100) NOT NULL,
    account_code VARCHAR(50),
    tax_id VARCHAR(50),
    credit_limit NUMERIC(18,2) NOT NULL DEFAULT 0,
    current_balance NUMERIC(18,2) NOT NULL DEFAULT 0,
    payment_terms INT DEFAULT 30, -- Days
    contact_name VARCHAR(100),
    contact_phone VARCHAR(50),
    contact_email VARCHAR(100),
    billing_address VARCHAR(255),
    billing_email VARCHAR(100),
    discount_percent NUMERIC(5,2) DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id),
    UNIQUE(company_id, account_code)
);

-- Link customers to corporate accounts
ALTER TABLE customers ADD COLUMN corporate_account_id INT REFERENCES corporate_accounts(corporate_account_id);

-- 29.2 Corporate Account Transactions
CREATE TABLE corporate_account_transactions (
    transaction_id SERIAL PRIMARY KEY,
    corporate_account_id INT NOT NULL REFERENCES corporate_accounts(corporate_account_id),
    transaction_type VARCHAR(20) NOT NULL, -- Charge, Payment, Adjustment, CreditNote
    order_id INT REFERENCES orders(order_id),
    amount NUMERIC(18,2) NOT NULL,
    balance_before NUMERIC(18,2) NOT NULL,
    balance_after NUMERIC(18,2) NOT NULL,
    reference VARCHAR(100),
    notes VARCHAR(255),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id)
);

-- 29.3 Corporate Account Invoices
CREATE TABLE corporate_invoices (
    invoice_id SERIAL PRIMARY KEY,
    corporate_account_id INT NOT NULL REFERENCES corporate_accounts(corporate_account_id),
    invoice_number VARCHAR(50) NOT NULL,
    invoice_date DATE NOT NULL,
    due_date DATE NOT NULL,
    period_start DATE,
    period_end DATE,
    subtotal NUMERIC(18,2) NOT NULL,
    tax_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    total_amount NUMERIC(18,2) NOT NULL,
    paid_amount NUMERIC(18,2) NOT NULL DEFAULT 0,
    status VARCHAR(20) DEFAULT 'Draft', -- Draft, Sent, Paid, PartiallyPaid, Overdue, Cancelled
    sent_at TIMESTAMPTZ,
    paid_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    created_by_user_id INT REFERENCES users(user_id)
);

-- ============================================
-- 30. KITCHEN DISPLAY SYSTEM (KDS)
-- ============================================

-- KDS Enhancements
ALTER TABLE kitchen_stations ADD COLUMN average_prep_time INT DEFAULT 10; -- minutes
ALTER TABLE kitchen_stations ADD COLUMN display_order INT DEFAULT 0;
ALTER TABLE kitchen_stations ADD COLUMN color VARCHAR(20);
ALTER TABLE kitchen_stations ADD COLUMN alert_after_minutes INT DEFAULT 15;

-- 30.1 KDS Order Queue
CREATE TABLE kds_queue (
    kds_queue_id SERIAL PRIMARY KEY,
    branch_id INT NOT NULL REFERENCES branches(branch_id),
    kitchen_station_id INT NOT NULL REFERENCES kitchen_stations(kitchen_station_id),
    order_id INT NOT NULL REFERENCES orders(order_id),
    order_line_id INT NOT NULL REFERENCES order_lines(order_line_id),
    status VARCHAR(20) DEFAULT 'Pending', -- Pending, InProgress, Ready, Bumped, Cancelled
    priority INT DEFAULT 0, -- Higher = more urgent
    received_at TIMESTAMPTZ DEFAULT NOW(),
    started_at TIMESTAMPTZ,
    completed_at TIMESTAMPTZ,
    bumped_at TIMESTAMPTZ,
    bumped_by_user_id INT REFERENCES users(user_id),
    time_in_queue_seconds INT
);

-- ============================================
-- ADDITIONAL INDEXES
-- ============================================

-- Promotions
CREATE INDEX idx_promotions_company ON promotions(company_id);
CREATE INDEX idx_promotions_active ON promotions(is_active, valid_from, valid_to);
CREATE INDEX idx_promotions_code ON promotions(code) WHERE code IS NOT NULL;

-- Drivers
CREATE INDEX idx_drivers_company ON drivers(company_id);
CREATE INDEX idx_drivers_available ON drivers(is_available, is_active);

-- Refunds
CREATE INDEX idx_refunds_order ON refunds(order_id);
CREATE INDEX idx_refunds_status ON refunds(status);

-- Customer Feedback
CREATE INDEX idx_feedback_company ON customer_feedback(company_id);
CREATE INDEX idx_feedback_customer ON customer_feedback(customer_id);
CREATE INDEX idx_feedback_order ON customer_feedback(order_id);

-- Corporate Accounts
CREATE INDEX idx_corporate_accounts_company ON corporate_accounts(company_id);

-- KDS Queue
CREATE INDEX idx_kds_queue_station ON kds_queue(kitchen_station_id, status);
CREATE INDEX idx_kds_queue_order ON kds_queue(order_id);

-- Staff Schedules
CREATE INDEX idx_staff_schedules_date ON staff_schedules(branch_id, schedule_date);
CREATE INDEX idx_staff_schedules_employee ON staff_schedules(employee_id);

-- Inventory Batches
CREATE INDEX idx_inventory_batches_expiry ON inventory_batches(expiry_date) WHERE status = 'Active';
CREATE INDEX idx_inventory_batches_item ON inventory_batches(inventory_item_id, branch_id);

-- ============================================
-- END OF SCHEMA
-- ============================================
