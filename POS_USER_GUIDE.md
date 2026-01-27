# Restaurant POS System - User Guide

## Quick Start Setup (Recommended Order)

Follow this order to set up your restaurant properly:

---

## 1. 🏢 Initial Setup (One-Time)

### 1.1 Branches
**Path:** Settings → Branches

Set up your restaurant locations first.
- Add branch name, address, phone
- Set operating hours
- Each branch can have its own settings

### 1.2 Currencies
**Path:** Settings → Currencies

Configure the currencies you accept.
- Set your default currency
- Add exchange rates if accepting multiple currencies

### 1.3 Payment Methods
**Path:** Settings → Payment Methods

Define how customers can pay.
- Cash, Credit Card, Mobile Payment, etc.
- Enable/disable methods per branch

---

## 2. 📦 Inventory Setup

### 2.1 Inventory Categories
**Path:** Inventory → Settings → Categories

Organize your ingredients into categories.
- Examples: Vegetables, Meats, Dairy, Spices, Beverages

**💡 Shortcut:** When adding inventory items, click the `+` button next to the Category dropdown to create a new category instantly!

### 2.2 Units of Measure
**Path:** Inventory → Settings → Units

Define measurement units for your ingredients.
- Examples: kg, g, liter, ml, pieces, dozen

### 2.3 Inventory Items (Ingredients)
**Path:** Inventory → Items

Add all your raw ingredients and stock items.

| Field | Description |
|-------|-------------|
| Name | Ingredient name (e.g., "Tomatoes") |
| Code | Optional SKU code (e.g., "TOM-001") |
| Unit | How it's measured (kg, pieces, etc.) |
| Category | Group for organization |
| Min Level | Alert when stock falls below this |
| Reorder Qty | Suggested amount to reorder |
| Current Qty | Stock on hand |
| Cost | Price per unit |

**💡 Tip:** Set minimum levels to get low-stock alerts!

---

## 3. 🍽️ Menu Setup

### 3.1 Menu Categories
**Path:** Menu → Categories

Create categories for your menu.
- Examples: Appetizers, Main Course, Desserts, Beverages, Sides

**💡 Shortcut:** When adding menu items, click the `+` button next to Category to add a new one!

### 3.2 Modifiers (Optional)
**Path:** Menu → Modifiers

Create options customers can add to items.
- Examples: Extra Cheese, No Onions, Spicy Level, Size options

### 3.3 Menu Items
**Path:** Menu → Items

Add your dishes and products.

| Field | Description |
|-------|-------------|
| Name | Dish name (e.g., "Margherita Pizza") |
| Category | Menu category |
| Price | Selling price |
| Description | Optional description |
| Image | Upload a photo |
| Sizes | Different size options with prices |
| Modifiers | Available add-ons/options |

### 3.4 Recipes (Cost Tracking)
**Path:** Menu → Recipes

Link menu items to inventory for cost tracking.
- Select a menu item
- Add ingredients and quantities used
- System calculates food cost automatically

**💡 Tip:** This helps track your profit margins!

---

## 4. 👥 Staff Setup

### 4.1 Roles & Permissions
**Path:** Staff → Roles & Permissions

Create roles with specific access levels.
- Manager: Full access
- Cashier: POS and orders only
- Waiter: Take orders, manage tables
- Kitchen: View kitchen orders

### 4.2 Staff Members
**Path:** Staff → Staff Management

Add your employees.
- Username and password for login
- Assign role
- Set default branch
- Optional: salary, hire date

---

## 5. 🪑 Restaurant Layout

### 5.1 Tables
**Path:** Settings → Tables

Set up your dining tables.
- Table number/name
- Seating capacity
- Assign to branch

### 5.2 Kitchen Stations
**Path:** Settings → Kitchen Stations

Organize your kitchen workflow.
- Examples: Grill, Fryer, Salad, Drinks

---

## 6. 💳 Using the POS

### Taking Orders
1. Open **POS** from the main menu
2. Select a table (for dine-in) or order type
3. Click menu items to add to order
4. Adjust quantity, add modifiers as needed
5. Click **Send to Kitchen** or **Pay**

### Processing Payments
1. Review the order total
2. Select payment method
3. Enter amount received
4. System calculates change
5. Print receipt (optional)

### Kitchen Display
- Kitchen staff sees incoming orders
- Mark items as "Preparing" → "Ready"
- Waiter gets notified when ready

---

## 7. 📊 Reports & Analytics

### Available Reports
**Path:** Reports

- **Sales Report:** Daily/weekly/monthly sales
- **Product Report:** Best/worst sellers
- **Staff Report:** Performance by employee
- **Inventory Report:** Stock levels, usage

---

## 🚀 Useful Shortcuts & Tips

### Quick Add Buttons (`+`)
These forms have `+` buttons next to dropdowns to create new items without leaving the form:

| Form | Quick Add Available |
|------|---------------------|
| **Inventory Items** | Category |
| **Menu Items** | Category, Kitchen Station |
| **Staff Management** | Role, Branch |
| **Purchase Orders** | Supplier |

### How to Use Quick Add
1. Click the `+` button next to any dropdown
2. Type the new item name
3. Click `✓` to save or `✕` to cancel
4. The dropdown refreshes with the new item

### Other Tips

| Feature | Shortcut/Tip |
|---------|--------------|
| Quick Search | Use search bars on all list pages |
| Toggle Status | Click Active/Inactive badges to toggle |
| Edit Item | Click the pencil icon on any row |
| Delete Item | Click the trash icon (with confirmation) |
| Filter by Branch | Use branch dropdown on applicable pages |

---

## 🔐 Default Login Credentials

### Super Admin (System Admin)
- **URL:** http://localhost:3001
- **Username:** superadmin
- **Password:** Admin@123

### Company Portal
- **URL:** http://localhost:3002
- Created via Super Admin panel

---

## 📱 Network Access

Access from other devices on your network:
- **Frontend:** http://YOUR_IP:3002
- **Backend API:** http://YOUR_IP:5000

Find your IP by running `ipconfig` in Command Prompt.

---

## ❓ Common Issues

### Can't see dropdown options?
- Make sure you've added items to that category first
- Check if items are set to "Active"

### Inventory not deducting?
- Ensure recipes are set up for menu items
- Check that inventory items have sufficient stock

### Staff can't login?
- Verify username/password
- Check if user is set to "Active"
- Confirm role has necessary permissions

---

## 📋 Setup Checklist

Use this checklist when setting up a new restaurant:

- [ ] Create branch(es)
- [ ] Set up currencies
- [ ] Add payment methods
- [ ] Create inventory categories
- [ ] Add units of measure
- [ ] Add inventory items (ingredients)
- [ ] Create menu categories
- [ ] Add modifiers (optional)
- [ ] Add menu items
- [ ] Set up recipes for cost tracking
- [ ] Create staff roles
- [ ] Add staff members
- [ ] Set up tables
- [ ] Configure kitchen stations
- [ ] Test a complete order flow

---

*Last Updated: January 2026*
