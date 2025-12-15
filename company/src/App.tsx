import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './contexts/AuthContext'
import Layout from './components/Layout'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Branches from './pages/Branches'
import Users from './pages/Users'
import Roles from './pages/Roles'
import Categories from './pages/Categories'
import MenuItems from './pages/MenuItems'
import Tables from './pages/Tables'
import KitchenStations from './pages/KitchenStations'
import Modifiers from './pages/Modifiers'
import PaymentMethods from './pages/PaymentMethods'
import Customers from './pages/Customers'
import CustomerProfile from './pages/CustomerProfile'
import DeliveryZones from './pages/DeliveryZones'
import Inventory from './pages/Inventory'
import Suppliers from './pages/Suppliers'
import Employees from './pages/Employees'
import Reservations from './pages/Reservations'
import GiftCards from './pages/GiftCards'
import Loyalty from './pages/Loyalty'
import LoyaltyTransactions from './pages/LoyaltyTransactions'
import Settings from './pages/Settings'
import Printers from './pages/Printers'
import AuditLogs from './pages/AuditLogs'
import Currencies from './pages/Currencies'
import ExchangeRates from './pages/ExchangeRates'
import Recipes from './pages/Recipes'
import InventorySettings from './pages/InventorySettings'
import PurchaseOrders from './pages/PurchaseOrders'
import GoodsReceipt from './pages/GoodsReceipt'
import StockMovements from './pages/StockMovements'
import Wastage from './pages/Wastage'
import StockAdjustment from './pages/StockAdjustment'
import StockCount from './pages/StockCount'
import Attendance from './pages/Attendance'
import CommissionPolicies from './pages/CommissionPolicies'
import ApprovalRules from './pages/ApprovalRules'
import ReceiptTemplates from './pages/ReceiptTemplates'
import Reports from './pages/Reports'

function App() {
  const { user, isLoading } = useAuth()

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-100">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  return (
    <Routes>
      <Route path="/login" element={user ? <Navigate to="/" /> : <Login />} />
      <Route
        path="/*"
        element={
          user ? (
            <Layout>
              <Routes>
                <Route path="/" element={<Dashboard />} />
                <Route path="/branches" element={<Branches />} />
                <Route path="/users" element={<Users />} />
                <Route path="/roles" element={<Roles />} />
                <Route path="/categories" element={<Categories />} />
                <Route path="/menu-items" element={<MenuItems />} />
                <Route path="/tables" element={<Tables />} />
                <Route path="/kitchen-stations" element={<KitchenStations />} />
                <Route path="/modifiers" element={<Modifiers />} />
                <Route path="/payment-methods" element={<PaymentMethods />} />
                <Route path="/customers" element={<Customers />} />
                <Route path="/customers/:id" element={<CustomerProfile />} />
                <Route path="/delivery-zones" element={<DeliveryZones />} />
                <Route path="/inventory" element={<Inventory />} />
                <Route path="/suppliers" element={<Suppliers />} />
                <Route path="/purchase-orders" element={<PurchaseOrders />} />
                <Route path="/goods-receipt" element={<GoodsReceipt />} />
                <Route path="/stock-movements" element={<StockMovements />} />
                <Route path="/wastage" element={<Wastage />} />
                <Route path="/stock-adjustment" element={<StockAdjustment />} />
                <Route path="/stock-count" element={<StockCount />} />
                <Route path="/employees" element={<Employees />} />
                <Route path="/attendance" element={<Attendance />} />
                <Route path="/commission-policies" element={<CommissionPolicies />} />
                <Route path="/reservations" element={<Reservations />} />
                <Route path="/gift-cards" element={<GiftCards />} />
                <Route path="/loyalty" element={<Loyalty />} />
                <Route path="/loyalty-transactions" element={<LoyaltyTransactions />} />
                <Route path="/approval-rules" element={<ApprovalRules />} />
                <Route path="/receipt-templates" element={<ReceiptTemplates />} />
                <Route path="/reports" element={<Reports />} />
                <Route path="/settings" element={<Settings />} />
                <Route path="/printers" element={<Printers />} />
                <Route path="/audit-logs" element={<AuditLogs />} />
                <Route path="/currencies" element={<Currencies />} />
                <Route path="/exchange-rates" element={<ExchangeRates />} />
                <Route path="/recipes" element={<Recipes />} />
                <Route path="/inventory-settings" element={<InventorySettings />} />
              </Routes>
            </Layout>
          ) : (
            <Navigate to="/login" />
          )
        }
      />
    </Routes>
  )
}

export default App
