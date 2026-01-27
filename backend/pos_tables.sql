-- POS Tables Migration

-- Shifts table
CREATE TABLE IF NOT EXISTS shifts (
    shift_id SERIAL PRIMARY KEY,
    company_id INTEGER NOT NULL REFERENCES companies(company_id) ON DELETE CASCADE,
    branch_id INTEGER NOT NULL REFERENCES branches(branch_id) ON DELETE RESTRICT,
    cashier_user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT,
    open_time TIMESTAMP NOT NULL DEFAULT NOW(),
    close_time TIMESTAMP,
    expected_close_time TIMESTAMP,
    opening_cash DECIMAL(18,2) NOT NULL DEFAULT 0,
    closing_cash DECIMAL(18,2),
    expected_cash DECIMAL(18,2),
    cash_difference DECIMAL(18,2),
    status VARCHAR(20) NOT NULL DEFAULT 'Open',
    force_closed_at TIMESTAMP,
    force_close_reason VARCHAR(255),
    notes VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Orders table
CREATE TABLE IF NOT EXISTS orders (
    order_id SERIAL PRIMARY KEY,
    company_id INTEGER NOT NULL REFERENCES companies(company_id) ON DELETE CASCADE,
    branch_id INTEGER NOT NULL REFERENCES branches(branch_id) ON DELETE RESTRICT,
    shift_id INTEGER REFERENCES shifts(shift_id) ON DELETE SET NULL,
    order_number VARCHAR(50) NOT NULL,
    order_type VARCHAR(20) NOT NULL DEFAULT 'DineIn',
    table_id INTEGER REFERENCES restaurant_tables(table_id) ON DELETE SET NULL,
    waiter_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    cashier_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    customer_id INTEGER REFERENCES customers(customer_id) ON DELETE SET NULL,
    order_status VARCHAR(20) NOT NULL DEFAULT 'Draft',
    currency_code VARCHAR(3) NOT NULL DEFAULT 'USD',
    exchange_rate_to_base DECIMAL(18,6) NOT NULL DEFAULT 1,
    sub_total DECIMAL(18,2) NOT NULL DEFAULT 0,
    total_line_discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    bill_discount_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    bill_discount_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    service_charge_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    service_charge_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    tax_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    tax_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    delivery_fee DECIMAL(18,2) NOT NULL DEFAULT 0,
    tips_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    grand_total DECIMAL(18,2) NOT NULL DEFAULT 0,
    net_amount_for_loyalty DECIMAL(18,2),
    loyalty_points_earned DECIMAL(18,2),
    loyalty_points_redeemed DECIMAL(18,2),
    loyalty_discount_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    total_paid DECIMAL(18,2) NOT NULL DEFAULT 0,
    balance_due DECIMAL(18,2) NOT NULL DEFAULT 0,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'Unpaid',
    notes VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    paid_at TIMESTAMP,
    voided_at TIMESTAMP,
    void_reason VARCHAR(255),
    void_by_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    approved_void_by_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    merged_from_order_id INTEGER,
    split_from_order_id INTEGER,
    UNIQUE(branch_id, order_number)
);

-- Order Lines table
CREATE TABLE IF NOT EXISTS order_lines (
    order_line_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    menu_item_id INTEGER NOT NULL REFERENCES menu_items(menu_item_id) ON DELETE RESTRICT,
    menu_item_size_id INTEGER REFERENCES menu_item_sizes(menu_item_size_id) ON DELETE SET NULL,
    quantity DECIMAL(18,4) NOT NULL DEFAULT 1,
    base_unit_price DECIMAL(18,2) NOT NULL,
    modifiers_extra_price DECIMAL(18,2) NOT NULL DEFAULT 0,
    effective_unit_price DECIMAL(18,2) NOT NULL,
    line_gross DECIMAL(18,2) NOT NULL,
    discount_percent DECIMAL(5,2) NOT NULL DEFAULT 0,
    discount_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    line_net DECIMAL(18,2) NOT NULL,
    notes VARCHAR(255),
    kitchen_status VARCHAR(20) NOT NULL DEFAULT 'New',
    kitchen_station_id INTEGER REFERENCES kitchen_stations(kitchen_station_id) ON DELETE SET NULL,
    sent_to_kitchen_at TIMESTAMP,
    ready_at TIMESTAMP,
    served_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by_user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL
);

-- Order Line Modifiers table
CREATE TABLE IF NOT EXISTS order_line_modifiers (
    order_line_modifier_id SERIAL PRIMARY KEY,
    order_line_id INTEGER NOT NULL REFERENCES order_lines(order_line_id) ON DELETE CASCADE,
    modifier_id INTEGER NOT NULL REFERENCES modifiers(modifier_id) ON DELETE RESTRICT,
    quantity DECIMAL(18,4) NOT NULL DEFAULT 1,
    extra_price DECIMAL(18,2) NOT NULL,
    total_price DECIMAL(18,2) NOT NULL
);

-- Order Payments table
CREATE TABLE IF NOT EXISTS order_payments (
    order_payment_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    payment_method_id INTEGER NOT NULL REFERENCES payment_methods(payment_method_id) ON DELETE RESTRICT,
    currency_code VARCHAR(3) NOT NULL DEFAULT 'USD',
    amount DECIMAL(18,2) NOT NULL,
    amount_in_order_currency DECIMAL(18,2) NOT NULL,
    exchange_rate_to_order_currency DECIMAL(18,6) NOT NULL DEFAULT 1,
    reference VARCHAR(100),
    gift_card_id INTEGER REFERENCES gift_cards(gift_card_id) ON DELETE SET NULL,
    loyalty_points_used DECIMAL(18,2),
    notes VARCHAR(255),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT
);

-- Order Status History table
CREATE TABLE IF NOT EXISTS order_status_history (
    order_status_history_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    old_status VARCHAR(20),
    new_status VARCHAR(20) NOT NULL,
    changed_at TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id INTEGER REFERENCES users(user_id) ON DELETE SET NULL,
    notes VARCHAR(255)
);

-- Order Delivery Details table
CREATE TABLE IF NOT EXISTS order_delivery_details (
    order_delivery_details_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL UNIQUE REFERENCES orders(order_id) ON DELETE CASCADE,
    customer_address_id INTEGER REFERENCES customer_addresses(customer_address_id) ON DELETE SET NULL,
    delivery_zone_id INTEGER REFERENCES delivery_zones(delivery_zone_id) ON DELETE SET NULL,
    address_line VARCHAR(500),
    city VARCHAR(100),
    area VARCHAR(100),
    phone VARCHAR(50),
    distance_km DECIMAL(18,2),
    delivery_fee_calculated DECIMAL(18,2) NOT NULL DEFAULT 0,
    driver_name VARCHAR(100),
    driver_phone VARCHAR(50),
    estimated_delivery_time TIMESTAMP,
    out_for_delivery_at TIMESTAMP,
    delivered_at TIMESTAMP,
    delivery_notes VARCHAR(500)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_shifts_company ON shifts(company_id);
CREATE INDEX IF NOT EXISTS idx_shifts_branch ON shifts(branch_id);
CREATE INDEX IF NOT EXISTS idx_shifts_cashier ON shifts(cashier_user_id);
CREATE INDEX IF NOT EXISTS idx_shifts_status ON shifts(status);

CREATE INDEX IF NOT EXISTS idx_orders_company ON orders(company_id);
CREATE INDEX IF NOT EXISTS idx_orders_branch ON orders(branch_id);
CREATE INDEX IF NOT EXISTS idx_orders_shift ON orders(shift_id);
CREATE INDEX IF NOT EXISTS idx_orders_customer ON orders(customer_id);
CREATE INDEX IF NOT EXISTS idx_orders_status ON orders(order_status);
CREATE INDEX IF NOT EXISTS idx_orders_created ON orders(created_at);

CREATE INDEX IF NOT EXISTS idx_order_lines_order ON order_lines(order_id);
CREATE INDEX IF NOT EXISTS idx_order_lines_menu_item ON order_lines(menu_item_id);
CREATE INDEX IF NOT EXISTS idx_order_lines_kitchen_status ON order_lines(kitchen_status);

CREATE INDEX IF NOT EXISTS idx_order_payments_order ON order_payments(order_id);
CREATE INDEX IF NOT EXISTS idx_order_status_history_order ON order_status_history(order_id);
