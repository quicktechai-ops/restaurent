# Restaurant POS System - Complete Test Scenarios

## 🔐 Test Credentials

| Portal | URL | Username | Password |
|--------|-----|----------|----------|
| Admin Portal | http://localhost:3001 | superadmin | Admin@123 |
| Company Portal | http://localhost:3002 | (company user) | (set during setup) |

---

## 📋 Test Scenario 1: Initial Setup (Admin Portal)

### 1.1 Create a Company
1. Go to http://localhost:3001
2. Login with `superadmin` / `Admin@123`
3. Navigate to **Companies**
4. Click **+ Add Company**
5. Fill in:
   - Name: `Test Restaurant`
   - Code: `TEST001`
   - Email: `test@restaurant.com`
   - Phone: `123-456-7890`
   - Address: `123 Main Street`
6. Click **Save**
7. ✅ **Expected:** Company appears in the list

### 1.2 Create Company Admin User
1. Navigate to **Users**
2. Click **+ Add User**
3. Fill in:
   - Username: `testadmin`
   - Password: `Test@123`
   - Full Name: `Test Admin`
   - Email: `admin@test.com`
   - Role: `CompanyAdmin`
   - Company: `Test Restaurant`
4. Click **Save**
5. ✅ **Expected:** User created successfully

---

## 📋 Test Scenario 2: Company Setup (Company Portal)

### 2.1 Login to Company Portal
1. Go to http://localhost:3002
2. Login with `testadmin` / `Test@123`
3. ✅ **Expected:** Dashboard loads with navigation menu

### 2.2 Create a Branch
1. Navigate to **Admin** → **Branches**
2. Click **+ Add Branch**
3. Fill in:
   - Name: `Main Branch`
   - Code: `MAIN`
   - Address: `123 Main Street`
   - VAT %: `10`
   - Service Charge %: `5`
4. Click **Save**
5. ✅ **Expected:** Branch appears in list

### 2.3 Create Roles
1. Navigate to **Admin** → **Roles**
2. Click **+ Add Role**
3. Create roles:
   - `Cashier`
   - `Waiter`
   - `Kitchen Staff`
   - `Manager`
4. ✅ **Expected:** All roles appear in list

### 2.4 Create Staff Members
1. Navigate to **Staff**
2. Click **+ Add Staff**
3. Fill in:
   - Username: `cashier1`
   - Password: `Cash@123`
   - Full Name: `John Cashier`
   - Role: `Cashier` (use **+** button to add new role if needed)
   - Branch: `Main Branch` (use **+** button to add new branch if needed)
4. Click **Save**
5. ✅ **Expected:** Staff member created

---

## 📋 Test Scenario 3: Inventory Setup

### 3.1 Create Inventory Categories
1. Navigate to **Inventory** → **Inv. Settings**
2. In **Categories** tab, click **+ Add**
3. Create categories:
   - `Raw Materials`
   - `Beverages`
   - `Packaging`
4. ✅ **Expected:** Categories appear in list

### 3.2 Create Units of Measure
1. In **Units** tab, click **+ Add**
2. Create units:
   - `kg` (Kilogram)
   - `ltr` (Liter)
   - `pcs` (Pieces)
   - `box` (Box)
4. ✅ **Expected:** Units appear in list

### 3.3 Create Inventory Items
1. Navigate to **Inventory** → **Inventory**
2. Click **+ Add Item**
3. Create items:

   **Item 1:**
   - Name: `Chicken Breast`
   - Category: `Raw Materials` (use **+** to add if needed)
   - Unit: `kg`
   - Cost: `5.00`
   - Min Stock: `10`
   - Reorder Point: `15`

   **Item 2:**
   - Name: `Coca Cola 330ml`
   - Category: `Beverages`
   - Unit: `pcs`
   - Cost: `0.50`
   - Min Stock: `50`
   - Reorder Point: `100`

   **Item 3:**
   - Name: `Rice`
   - Category: `Raw Materials`
   - Unit: `kg`
   - Cost: `2.00`
   - Min Stock: `20`
   - Reorder Point: `30`

4. ✅ **Expected:** All items appear in inventory list

### 3.4 Create Suppliers
1. Navigate to **Inventory** → **Suppliers**
2. Click **+ Add Supplier**
3. Create suppliers:
   - `Fresh Foods Co.` - Phone: `111-111-1111`
   - `Beverage Distributors` - Phone: `222-222-2222`
4. ✅ **Expected:** Suppliers appear in list

---

## 📋 Test Scenario 4: Menu Setup

### 4.1 Create Menu Categories
1. Navigate to **Menu** → **Categories**
2. Click **+ Add Category**
3. Create categories:
   - `Main Dishes`
   - `Beverages`
   - `Desserts`
   - `Appetizers`
4. ✅ **Expected:** Categories appear in list

### 4.2 Create Kitchen Stations
1. Navigate to **Menu** → **Kitchen Stations**
2. Click **+ Add Station**
3. Create stations:
   - `Hot Kitchen`
   - `Cold Kitchen`
   - `Beverage Station`
4. ✅ **Expected:** Stations appear in list

### 4.3 Create Menu Items
1. Navigate to **Menu** → **Menu Items**
2. Click **+ Add Item**
3. Create items:

   **Item 1:**
   - Name: `Grilled Chicken`
   - Category: `Main Dishes` (use **+** to add if needed)
   - Price: `15.00`
   - Kitchen Station: `Hot Kitchen` (use **+** to add if needed)
   - Description: `Grilled chicken breast with herbs`

   **Item 2:**
   - Name: `Coca Cola`
   - Category: `Beverages`
   - Price: `3.00`
   - Kitchen Station: `Beverage Station`

   **Item 3:**
   - Name: `Caesar Salad`
   - Category: `Appetizers`
   - Price: `8.00`
   - Kitchen Station: `Cold Kitchen`

   **Item 4 (with sizes):**
   - Name: `Pizza Margherita`
   - Category: `Main Dishes`
   - Allow Sizes: ✅ Yes
   - Sizes:
     - Small: `10.00`
     - Medium: `14.00`
     - Large: `18.00`
   - Kitchen Station: `Hot Kitchen`

4. ✅ **Expected:** All menu items appear in list

### 4.4 Create Modifiers
1. Navigate to **Menu** → **Modifiers**
2. Click **+ Add Modifier**
3. Create modifiers:
   - `Extra Cheese` - Price: `2.00`
   - `No Onions` - Price: `0.00`
   - `Spicy` - Price: `0.00`
   - `Add Bacon` - Price: `3.00`
4. ✅ **Expected:** Modifiers appear in list

### 4.5 Create Recipes (Link Menu Items to Inventory)
1. Navigate to **Inventory** → **Recipes**
2. Click **+ Add Recipe**
3. Create recipe for `Grilled Chicken`:
   - Menu Item: `Grilled Chicken`
   - Ingredients:
     - `Chicken Breast` - Quantity: `0.3` kg
     - `Rice` - Quantity: `0.15` kg
4. ✅ **Expected:** Recipe created and linked

---

## 📋 Test Scenario 5: Purchase Orders & Stock

### 5.1 Create Purchase Order
1. Navigate to **Inventory** → **Purchase Orders**
2. Click **+ New PO**
3. Fill in:
   - Supplier: `Fresh Foods Co.` (use **+** to add if needed)
   - Add lines:
     - `Chicken Breast` - Qty: `50` - Unit Price: `5.00`
     - `Rice` - Qty: `100` - Unit Price: `2.00`
4. Click **Save**
5. ✅ **Expected:** PO created with status `Draft`

### 5.2 Receive Goods
1. Navigate to **Inventory** → **Goods Receipt**
2. Select the Purchase Order
3. Enter received quantities
4. Click **Receive**
5. ✅ **Expected:** Stock levels updated

### 5.3 Verify Stock Levels
1. Navigate to **Inventory** → **Inventory**
2. Check stock quantities for:
   - `Chicken Breast`: Should show `50 kg`
   - `Rice`: Should show `100 kg`
3. ✅ **Expected:** Stock levels match received quantities

---

## 📋 Test Scenario 6: Tables & Reservations

### 6.1 Create Tables
1. Navigate to **Admin** → **Tables**
2. Click **+ Add Table**
3. Create tables:
   - Table 1 - Zone: `Indoor` - Capacity: `4`
   - Table 2 - Zone: `Indoor` - Capacity: `2`
   - Table 3 - Zone: `Outdoor` - Capacity: `6`
   - Table 4 - Zone: `VIP` - Capacity: `8`
4. ✅ **Expected:** Tables appear in list

### 6.2 Create Reservation
1. Navigate to **Reservations**
2. Click **+ New Reservation**
3. Fill in:
   - Customer Name: `John Smith`
   - Phone: `555-123-4567`
   - Date: (today's date)
   - Time: `7:00 PM`
   - Party Size: `4`
   - Table: `Table 1`
4. Click **Save**
5. ✅ **Expected:** Reservation appears in calendar/list

---

## 📋 Test Scenario 7: Payment Methods

### 7.1 Create Payment Methods
1. Navigate to **Admin** → **Payment Methods**
2. Click **+ Add**
3. Create methods:
   - `Cash` - Type: `Cash`
   - `Credit Card` - Type: `Card`
   - `Debit Card` - Type: `Card`
4. ✅ **Expected:** Payment methods appear in list

---

## 📋 Test Scenario 8: POS Operations

### 8.1 Access POS
1. From Home, click **POS** or navigate to `/pos`
2. ✅ **Expected:** POS interface loads with categories

### 8.2 Create Dine-In Order
1. Select Order Type: `Dine In`
2. Select Table: `Table 1`
3. Browse categories and add items:
   - Click `Main Dishes` category
   - Add `Grilled Chicken` (qty: 2)
   - Add `Pizza Margherita` - Select size `Medium`
   - Go back, click `Beverages`
   - Add `Coca Cola` (qty: 2)
   - Click `Appetizers`
   - Add `Caesar Salad`
4. ✅ **Expected:** Order shows all items with correct prices

### 8.3 Apply Modifiers
1. When adding `Pizza Margherita`, select modifiers:
   - `Extra Cheese` (+$2.00)
   - `No Onions` (+$0.00)
2. ✅ **Expected:** Modifier prices added to item

### 8.4 Apply Line Discount
1. Click on `Grilled Chicken` line
2. Apply 10% discount
3. ✅ **Expected:** Line shows discounted price

### 8.5 Apply Bill Discount
1. Click **Bill Discount** button
2. Enter 5%
3. ✅ **Expected:** Total reflects 5% discount

### 8.6 Process Payment
1. Click **Pay**
2. Select `Cash`
3. Confirm payment
4. ✅ **Expected:** 
   - Order confirmed message
   - Order cleared
   - Receipt/confirmation shown

### 8.7 Create Takeaway Order
1. Select Order Type: `Takeaway`
2. Add items:
   - `Coca Cola` x 3
   - `Caesar Salad` x 1
3. Process payment with `Credit Card`
4. ✅ **Expected:** Order completed successfully

### 8.8 Create Delivery Order
1. Select Order Type: `Delivery`
2. Click **Customer** button
3. Add new customer or select existing
4. Add items to order
5. Process payment
6. ✅ **Expected:** Order linked to customer

---

## 📋 Test Scenario 9: Reports & Analytics

### 9.1 View Sales Report
1. Navigate to **Reports** → **Sales**
2. Select date range (today)
3. ✅ **Expected:** Shows orders created in Test Scenario 8

### 9.2 View Inventory Report
1. Navigate to **Reports** → **Inventory**
2. ✅ **Expected:** Shows current stock levels and movements

---

## 📋 Test Scenario 10: Stock Management

### 10.1 Stock Adjustment
1. Navigate to **Inventory** → **Stock Adjustment**
2. Create adjustment:
   - Item: `Coca Cola 330ml`
   - Adjustment: `-5` (damaged goods)
   - Reason: `Damaged`
3. ✅ **Expected:** Stock reduced by 5

### 10.2 Stock Count
1. Navigate to **Inventory** → **Stock Count**
2. Perform count for `Raw Materials` category
3. Enter actual counts
4. ✅ **Expected:** Variances calculated and shown

### 10.3 Record Wastage
1. Navigate to **Inventory** → **Wastage**
2. Record wastage:
   - Item: `Chicken Breast`
   - Quantity: `2` kg
   - Reason: `Expired`
3. ✅ **Expected:** Stock reduced, wastage recorded

---

## 📋 Test Scenario 11: Quick Add Shortcuts (+ Buttons)

### 11.1 Test Inventory Category Shortcut
1. Go to **Inventory** → **Inventory**
2. Click **+ Add Item**
3. Next to Category dropdown, click **+**
4. Enter: `Spices`
5. Click ✓
6. ✅ **Expected:** `Spices` appears in dropdown

### 11.2 Test Menu Category Shortcut
1. Go to **Menu** → **Menu Items**
2. Click **+ Add Item**
3. Next to Category dropdown, click **+**
4. Enter: `Specials`
5. Click ✓
6. ✅ **Expected:** `Specials` appears in dropdown

### 11.3 Test Kitchen Station Shortcut
1. In Menu Items form
2. Next to Kitchen Station dropdown, click **+**
3. Enter: `Grill Station`
4. Click ✓
5. ✅ **Expected:** `Grill Station` appears in dropdown

### 11.4 Test Supplier Shortcut
1. Go to **Inventory** → **Purchase Orders**
2. Click **+ New PO**
3. Next to Supplier dropdown, click **+**
4. Enter: `New Supplier Inc.`
5. Click ✓
6. ✅ **Expected:** `New Supplier Inc.` appears in dropdown

### 11.5 Test Role Shortcut
1. Go to **Staff**
2. Click **+ Add Staff**
3. Next to Role dropdown, click **+**
4. Enter: `Supervisor`
5. Click ✓
6. ✅ **Expected:** `Supervisor` appears in dropdown

### 11.6 Test Branch Shortcut
1. In Staff form
2. Next to Branch dropdown, click **+**
3. Enter: `Second Branch`
4. Click ✓
5. ✅ **Expected:** `Second Branch` appears in dropdown

---

## 📋 Test Scenario 12: Session & Authentication

### 12.1 Test Logout
1. In POS page, click the **red logout button** (top-right)
2. ✅ **Expected:** Redirected to login page

### 12.2 Test Session Expiry
1. Login to company portal
2. Wait for session to expire (or manually clear localStorage)
3. Try to access any page
4. ✅ **Expected:** Automatically redirected to login page

---

## ✅ Test Completion Checklist

| Scenario | Status |
|----------|--------|
| 1. Initial Setup (Admin) | ⬜ |
| 2. Company Setup | ⬜ |
| 3. Inventory Setup | ⬜ |
| 4. Menu Setup | ⬜ |
| 5. Purchase Orders & Stock | ⬜ |
| 6. Tables & Reservations | ⬜ |
| 7. Payment Methods | ⬜ |
| 8. POS Operations | ⬜ |
| 9. Reports & Analytics | ⬜ |
| 10. Stock Management | ⬜ |
| 11. Quick Add Shortcuts | ⬜ |
| 12. Session & Authentication | ⬜ |

---

## 🐛 Bug Reporting Template

If you find any issues during testing, document them using this template:

```
**Bug ID:** BUG-001
**Scenario:** [Which test scenario]
**Step:** [Which step failed]
**Expected:** [What should happen]
**Actual:** [What actually happened]
**Screenshot:** [If applicable]
**Browser:** [Chrome/Firefox/Edge]
**Priority:** [High/Medium/Low]
```

---

## 📝 Notes

- Always test in order (1 → 12) for first-time setup
- Some features depend on data created in earlier scenarios
- Use the **+** shortcut buttons to quickly add related items without leaving forms
- Check browser console (F12) for any JavaScript errors during testing
