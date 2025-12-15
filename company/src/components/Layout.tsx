import { ReactNode } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import {
  LayoutDashboard,
  Building2,
  Users,
  Shield,
  FolderTree,
  UtensilsCrossed,
  Grid3X3,
  ChefHat,
  Sliders,
  CreditCard,
  LogOut,
  Menu,
  X,
  UserCheck,
  MapPin,
  Package,
  Truck,
  Calendar,
  Gift,
  Star,
  Settings,
  Printer,
  FileText,
  Coins,
  ArrowLeftRight,
  BookOpen,
  ShoppingCart,
  ClipboardList,
  ArrowUpDown,
  AlertTriangle,
  Clock,
  DollarSign,
  Lock,
  Receipt,
  BarChart3,
} from 'lucide-react'
import { useState } from 'react'

interface LayoutProps {
  children: ReactNode
}

const navigation = [
  { name: 'Dashboard', href: '/', icon: LayoutDashboard },
  { name: 'Reports', href: '/reports', icon: BarChart3 },
  // Menu & Dining
  { name: 'Categories', href: '/categories', icon: FolderTree },
  { name: 'Menu Items', href: '/menu-items', icon: UtensilsCrossed },
  { name: 'Modifiers', href: '/modifiers', icon: Sliders },
  { name: 'Recipes', href: '/recipes', icon: BookOpen },
  { name: 'Tables', href: '/tables', icon: Grid3X3 },
  { name: 'Kitchen Stations', href: '/kitchen-stations', icon: ChefHat },
  // Customers & Orders
  { name: 'Customers', href: '/customers', icon: Users },
  { name: 'Reservations', href: '/reservations', icon: Calendar },
  { name: 'Delivery Zones', href: '/delivery-zones', icon: MapPin },
  // Inventory & Purchasing
  { name: 'Inventory', href: '/inventory', icon: Package },
  { name: 'Inventory Settings', href: '/inventory-settings', icon: Settings },
  { name: 'Purchase Orders', href: '/purchase-orders', icon: ShoppingCart },
  { name: 'Goods Receipt', href: '/goods-receipt', icon: ClipboardList },
  { name: 'Stock Movements', href: '/stock-movements', icon: ArrowUpDown },
  { name: 'Wastage', href: '/wastage', icon: AlertTriangle },
  { name: 'Stock Adjustment', href: '/stock-adjustment', icon: Settings },
  { name: 'Stock Count', href: '/stock-count', icon: ClipboardList },
  { name: 'Suppliers', href: '/suppliers', icon: Truck },
  // HR & Employees
  { name: 'Employees', href: '/employees', icon: UserCheck },
  { name: 'Attendance', href: '/attendance', icon: Clock },
  { name: 'Commission Policies', href: '/commission-policies', icon: DollarSign },
  // Loyalty & Gift Cards
  { name: 'Loyalty', href: '/loyalty', icon: Star },
  { name: 'Loyalty Transactions', href: '/loyalty-transactions', icon: Star },
  { name: 'Gift Cards', href: '/gift-cards', icon: Gift },
  // Settings & Security
  { name: 'Branches', href: '/branches', icon: Building2 },
  { name: 'Users', href: '/users', icon: Users },
  { name: 'Roles', href: '/roles', icon: Shield },
  { name: 'Approval Rules', href: '/approval-rules', icon: Lock },
  { name: 'Payment Methods', href: '/payment-methods', icon: CreditCard },
  { name: 'Currencies', href: '/currencies', icon: Coins },
  { name: 'Exchange Rates', href: '/exchange-rates', icon: ArrowLeftRight },
  { name: 'Printers', href: '/printers', icon: Printer },
  { name: 'Receipt Templates', href: '/receipt-templates', icon: Receipt },
  { name: 'Settings', href: '/settings', icon: Settings },
  { name: 'Audit Logs', href: '/audit-logs', icon: FileText },
]

export default function Layout({ children }: LayoutProps) {
  const { user, logout } = useAuth()
  const location = useLocation()
  const [sidebarOpen, setSidebarOpen] = useState(false)

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Mobile sidebar backdrop */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed top-0 left-0 z-50 h-full w-64 bg-primary-800 text-white transform transition-transform duration-300 lg:translate-x-0 ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div className="p-4 border-b border-primary-700 flex items-center justify-between">
          <div>
            <h1 className="text-xl font-bold">Restaurant POS</h1>
            <p className="text-sm text-primary-300">Company Admin</p>
          </div>
          <button className="lg:hidden" onClick={() => setSidebarOpen(false)}>
            <X size={20} />
          </button>
        </div>

        <nav className="p-4 space-y-1 overflow-y-auto h-[calc(100vh-180px)]">
          {navigation.map((item) => {
            const isActive = location.pathname === item.href
            return (
              <Link
                key={item.name}
                to={item.href}
                onClick={() => setSidebarOpen(false)}
                className={`flex items-center gap-3 px-3 py-2 rounded-lg transition-colors ${
                  isActive
                    ? 'bg-primary-600 text-white'
                    : 'text-primary-200 hover:bg-primary-700 hover:text-white'
                }`}
              >
                <item.icon size={20} />
                <span>{item.name}</span>
              </Link>
            )
          })}
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-primary-700">
          <div className="mb-3">
            <p className="font-medium truncate">{user?.companyName}</p>
            <p className="text-sm text-primary-300 truncate">{user?.name}</p>
          </div>
          <button
            onClick={logout}
            className="flex items-center gap-2 text-primary-300 hover:text-white transition-colors w-full"
          >
            <LogOut size={18} />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main content */}
      <div className="lg:ml-64">
        {/* Mobile header */}
        <header className="bg-white shadow-sm lg:hidden">
          <div className="flex items-center justify-between px-4 py-3">
            <button onClick={() => setSidebarOpen(true)}>
              <Menu size={24} />
            </button>
            <h1 className="font-semibold">{user?.companyName}</h1>
            <div className="w-6" />
          </div>
        </header>

        <main className="p-6">{children}</main>
      </div>
    </div>
  )
}
