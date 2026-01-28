# Restaurant POS System - Complete Testing Workflow

## Table of Contents
1. [Initial Setup](#1-initial-setup)
2. [SuperAdmin Portal Testing](#2-superadmin-portal-testing)
3. [Company Admin Portal Testing](#3-company-admin-portal-testing)
4. [POS Operations Testing](#4-pos-operations-testing)
5. [Inventory Management Testing](#5-inventory-management-testing)
6. [Customer & Loyalty Testing](#6-customer--loyalty-testing)
7. [Reports Testing](#7-reports-testing)
---

## 1. Initial Setup

### 1.1 Start the Application

```bash
# Terminal 1 - Start Backend
cd backend
dotnet run
# Backend runs on http://localhost:5000

# Terminal 2 - Start Company Portal
cd company
npm run dev
# Frontend runs on http://localhost:3002

# Terminal 3 - Start Admin Portal (SuperAdmin)
cd admin
npm run dev
# Admin portal runs on http://localhost:3001
```

### 1.2 Seed Initial Data

Open browser and navigate to:
```
http://localhost:5000/api/seed/all
```

This creates:
- SuperAdmin account
- Subscription plans
- Currencies

### 1.3 Default Credentials

| Portal | URL | Username | Password |
|--------|-----|----------|----------|
| SuperAdmin | http://localhost:3001 | superadmin | Admin@123 |
| Company | http://localhost:3002 | (created by SuperAdmin) | (created by SuperAdmin) |

---

## 2. SuperAdmin Portal Testing

### 2.1 Login to SuperAdmin Portal
1. Go to `http://localhost:3001`
2. Enter: `superadmin` / `Admin@123`
3. ✅ Verify: Dashboard loads with stats

### 2.2 Create a Company
1. Click **Companies** in sidebar
2. Click **Add Company**
3. Fill in:
   - Company Name: `Test Restaurant`
   - Username: `testrestaurant`
   - Password: `Test@123`
   - Email: `test@restaurant.com`
   - Phone: `+1234567890`
   - Select a Plan
4. Click **Save**
5. ✅ Verify: Company appears in list

### 2.3 Create Subscription Plans
1. Click **Plans** in sidebar
2. Click **Add Plan**
3. Fill in:
   - Plan Name: `Basic`
   - Price: `99`
   - Duration: `30` days
   - Max Branches: `1`
   - Max Users: `5`
4. Click **Save**
5. ✅ Verify: Plan appears in list

### 2.4 View Billing
1. Click **Billing** in sidebar
2. ✅ Verify: Payment list loads (may be empty initially)

---

## 3. Company Admin Portal Testing

### 3.1 Login to Company Portal
1. Go to `http://localhost:3002`
2. Enter credentials created in step 2.2
3. ✅ Verify: Dashboard loads

### 3.2 Setup Master Data

#### 3.2.1 Create Branch
1. Go to **Settings > Branches**
2. Click **Add Branch**
3. Fill in:
   - Name: `Main Branch`
   - Code: `MB01`
   - Address: `123 Main St`
   - VAT Percent: `10`
   - Service Charge: `5`
4. Click **Save**
5. ✅ Verify: Branch appears in list

#### 3.2.2 Create Categories
1. Go to **Menu > Categories**
2. Click **Add Category**
3. Create categories:
   - `Appetizers`
   - `Main Courses`
   - `Beverages`
   - `Desserts`
4. ✅ Verify: Categories appear in list

#### 3.2.3 Create Menu Items
1. Go to **Menu > Menu Items**
2. Click **Add Item**
3. Fill in:
   - Name: `Grilled Chicken`
   - Code: `GC01`
   - Category: `Main Courses`
   - Default Price: `15.00`
4. Add more items:
   - `Caesar Salad` - $12.00 (Appetizers)
   - `Coca Cola` - $3.00 (Beverages)
   - `Chocolate Cake` - $8.00 (Desserts)
5. ✅ Verify: Items appear in list

#### 3.2.4 Create Tables (for Dine-In)
1. Go to **Settings > Tables**
2. Click **Add Table**
3. Create tables:
   - Table 1, Zone: Hall, Capacity: 4
   - Table 2, Zone: Hall, Capacity: 2
   - Table 3, Zone: Terrace, Capacity: 6
4. ✅ Verify: Tables appear in list

#### 3.2.5 Create Payment Methods
1. Go to **Settings > Payment Methods**
2. Click **Add Method**
3. Create:
   - `Cash` (Type: Cash)
   - `Credit Card` (Type: Card)
4. ✅ Verify: Methods appear in list

#### 3.2.6 Create Kitchen Stations
1. Go to **Settings > Kitchen Stations**
2. Click **Add Station**
3. Create:
   - `Hot Kitchen`
   - `Cold Kitchen`
   - `Bar`
4. ✅ Verify: Stations appear in list

---

## 4. POS Operations Testing

### 4.1 Navigate to POS
1. Click **POS** in sidebar
2. ✅ Verify: POS screen loads with categories and items

### 4.2 Create Dine-In Order
1. Select order type: **Dine-In**
2. Select a table (e.g., Table 1)
3. Click on menu items to add:
   - `Caesar Salad` x 1
   - `Grilled Chicken` x 2
   - `Coca Cola` x 2
4. ✅ Verify: Order summary shows correct items and totals
5. Check calculations:
   - Subtotal = $12 + $30 + $6 = $48.00
   - Service Charge (5%) = $2.40
   - VAT (10%) = $5.04
   - Grand Total = $55.44

### 4.3 Process Payment
1. Click **Pay** button
2. ✅ Verify: Payment modal shows total amount
3. Click **Cash**
4. ✅ Verify: Success message with order number
5. ✅ Verify: Order is cleared after payment

### 4.4 Create Takeaway Order
1. Select order type: **Takeaway**
2. Add items:
   - `Chocolate Cake` x 1
   - `Coca Cola` x 1
3. Click **Pay**
4. Click **Cash**
5. ✅ Verify: Order saved successfully

### 4.5 Test Item Modifiers
1. If modifiers are configured, add item with modifier
2. ✅ Verify: Modifier price adds to item total

### 4.6 Test Line Discount
1. Add items to order
2. Click on an item in the order
3. Apply discount percentage
4. ✅ Verify: Discount reflected in totals

### 4.7 Test Bill Discount
1. Add items to order
2. Enter bill discount percentage
3. ✅ Verify: Discount applied to entire order

---

## 5. Inventory Management Testing

### 5.1 Create Inventory Categories
1. Go to **Inventory > Settings**
2. Click **Categories** tab
3. Add categories:
   - `Raw Ingredients`
   - `Beverages`
   - `Packaging`
4. ✅ Verify: Categories appear in list

### 5.2 Create Inventory Items
1. Go to **Inventory > Items**
2. Click **Add Item**
3. Create items:
   - Name: `Chicken Breast`
   - Unit: `kg`
   - Category: `Raw Ingredients`
   - Min Level: `5`
   - Reorder Qty: `20`
4. ✅ Verify: Items appear in list

### 5.3 Create Supplier
1. Go to **Inventory > Suppliers**
2. Click **Add Supplier**
3. Fill in:
   - Name: `ABC Foods`
   - Contact: `John Smith`
   - Phone: `+1234567890`
4. ✅ Verify: Supplier appears in list

### 5.4 Create Recipe
1. Go to **Inventory > Recipes**
2. Click **Add Recipe**
3. Select Menu Item: `Grilled Chicken`
4. Add ingredients:
   - `Chicken Breast` - 0.3 kg
5. ✅ Verify: Recipe saved

### 5.5 Stock Adjustment
1. Go to **Inventory > Stock Adjustment**
2. Add adjustment:
   - Item: `Chicken Breast`
   - Quantity: `50`
   - Reason: `Initial Stock`
3. ✅ Verify: Stock updated

### 5.6 View Stock Movements
1. Go to **Inventory > Stock Movements**
2. ✅ Verify: Adjustment appears in history

---

## 6. Customer & Loyalty Testing

### 6.1 Create Customer
1. Go to **Customers > Customer List**
2. Click **Add Customer**
3. Fill in:
   - Name: `John Doe`
   - Phone: `+1234567890`
   - Email: `john@example.com`
4. ✅ Verify: Customer appears in list

### 6.2 Configure Loyalty Settings
1. Go to **Customers > Loyalty**
2. Configure:
   - Points per amount: `1 point per $10`
   - Point value: `$0.10 per point`
3. ✅ Verify: Settings saved

### 6.3 Test Loyalty Points
1. Go to POS
2. Select customer: `John Doe`
3. Add items and complete payment
4. Go to customer profile
5. ✅ Verify: Points earned from purchase

### 6.4 Create Gift Card
1. Go to **Customers > Gift Cards**
2. Click **Add Gift Card**
3. Fill in:
   - Card Number: `GC001`
   - Initial Value: `50.00`
4. ✅ Verify: Gift card created
/////////////////////////////////////////////////////////
### 6.5 Create Reservation
1. Go to **Customers > Reservations**
2. Click **Add Reservation**
3. Fill in:
   - Customer: `John Doe`
   - Date: Tomorrow
   - Time: `19:00`
   - Party Size: `4`
   - Table: `Table 3`
4. ✅ Verify: Reservation appears in calendar

---

## 7. Reports Testing

### 7.1 View Dashboard
1. Go to **Dashboard**
2. ✅ Verify: Stats cards show:
   - Today's sales
   - Order count
   - Average ticket
3. ✅ Verify: Charts display correctly

### 7.2 Sales Reports
1. Go to **Reports**
2. Select report type: **Sales Summary**
3. Set date range
4. Click **Generate**
5. ✅ Verify: Report shows sales data

### 7.3 Inventory Reports
1. Select report type: **Stock on Hand**
2. ✅ Verify: Current stock levels displayed

---

## 8. Additional Testing Scenarios

### 8.1 Multi-Currency
1. Go to **Settings > Currencies**
2. Add currency (e.g., `EUR`)
3. Go to **Settings > Exchange Rates**
4. Set rate: `1 USD = 0.92 EUR`
5. ✅ Verify: Currency conversion works










/////////////////////////////////////////////////////////////




### 8.2 User Management
1. Go to **Settings > Users**
2. Create new user with limited permissions
3. Logout and login as new user
4. ✅ Verify: User can only access permitted features

### 8.3 Role & Permissions
1. Go to **Settings > Roles**
2. Create role: `Cashier`
3. Assign permissions: POS only
4. Assign role to user
5. ✅ Verify: Role restrictions apply

---

## 9. Common Issues & Troubleshooting

### 9.1 Backend Not Starting
```bash
# Check if port 5000 is in use
netstat -an | findstr :5000

# Kill existing process if needed
taskkill /F /PID <process_id>
```

### 9.2 Database Connection Failed
- Verify PostgreSQL is running
- Check connection string in `appsettings.json`
- Ensure database exists

### 9.3 API Returns 401 Unauthorized
- Token may be expired
- Logout and login again
- Check if user has proper permissions

### 9.4 POS Page Not Loading Data
- Check browser console for errors
- Verify API endpoints are correct
- Ensure backend is running

### 9.5 Payment Fails
- Check if payment methods exist
- Verify branch has proper configuration
- Check for foreign key issues in backend logs

---

## 10. Testing Checklist

### SuperAdmin Portal
- [ ] Login works
- [ ] Dashboard displays stats
- [ ] Can create/edit/delete companies
- [ ] Can manage subscription plans
- [ ] Can view billing

### Company Portal - Setup
- [ ] Login works
- [ ] Can create branches
- [ ] Can create categories
- [ ] Can create menu items
- [ ] Can create tables
- [ ] Can create payment methods
- [ ] Can create kitchen stations

### Company Portal - POS
- [ ] POS loads correctly
- [ ] Can add items to order
- [ ] Dine-in orders work
- [ ] Takeaway orders work
- [ ] Delivery orders work
- [ ] Cash payment works
- [ ] Card payment works
- [ ] Order saved to database
- [ ] Order clears after payment
- [ ] Discounts calculate correctly
- [ ] Tax calculates correctly
- [ ] Service charge calculates correctly

### Company Portal - Inventory
- [ ] Can create inventory items
- [ ] Can create suppliers
- [ ] Can create recipes
- [ ] Stock adjustments work
- [ ] Stock movements display

### Company Portal - Customers
- [ ] Can create customers
- [ ] Customer profile displays
- [ ] Loyalty points work
- [ ] Gift cards work
- [ ] Reservations work

### Company Portal - Reports
- [ ] Dashboard shows stats
- [ ] Sales reports generate
- [ ] Inventory reports generate

---

## 11. API Testing with Postman/cURL

### Get Auth Token
```bash
curl -X POST http://localhost:5000/api/auth/company \
  -H "Content-Type: application/json" \
  -d '{"username":"testrestaurant","password":"Test@123"}'
```

### List Menu Items
```bash
curl -X GET http://localhost:5000/api/company/menu-items \
  -H "Authorization: Bearer <token>"
```

### Create Order
```bash
curl -X POST http://localhost:5000/api/company/orders \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"branchId":1,"orderType":"Takeaway"}'
```

---

## Quick Start Summary

1. **Start Backend**: `cd backend && dotnet run`
2. **Start Frontend**: `cd company && npm run dev`
3. **Seed Data**: Visit `http://localhost:5000/api/seed/all`
4. **Login SuperAdmin**: `http://localhost:3001` → superadmin/Admin@123
5. **Create Company**: Add company in SuperAdmin portal
6. **Login Company**: `http://localhost:3002` with new credentials
7. **Setup**: Create branch, categories, items, tables, payment methods
8. **Test POS**: Add items, pay with Cash
9. **Verify**: Check order saved in database

---

*Last Updated: December 2024*
