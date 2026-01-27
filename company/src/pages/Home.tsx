import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import {
  ShoppingCart,
  BarChart3,
  Package,
  UtensilsCrossed,
  Users,
  UserCheck,
  Settings,
  FileText,
  ArrowLeft,
  LogOut,
  Store,
  Printer,
  Receipt,
  CreditCard,
  Coins,
  ArrowLeftRight,
  FileSearch,
  MapPin,
  ClipboardList,
  Sliders,
  Truck,
  AlertTriangle,
  BookOpen,
  Calendar,
  Gift,
  Star,
  Shield,
  Clock,
  DollarSign,
  Lock,
  FolderTree,
  Grid3X3,
  ChefHat,
} from 'lucide-react'

type ViewType = 'main' | 'inventory' | 'menu' | 'staff' | 'customers' | 'admin'

interface NavItem {
  name: string
  href: string
  icon: any
}

const inventoryItems: NavItem[] = [
  { name: 'Inventory', href: '/inventory', icon: Package },
  { name: 'Stock Count', href: '/stock-count', icon: ClipboardList },
  { name: 'Stock Adjustment', href: '/stock-adjustment', icon: Sliders },
  { name: 'Stock Movements', href: '/stock-movements', icon: Truck },
  { name: 'Goods Receipt', href: '/goods-receipt', icon: ClipboardList },
  { name: 'Wastage', href: '/wastage', icon: AlertTriangle },
  { name: 'Purchase Orders', href: '/purchase-orders', icon: ShoppingCart },
  { name: 'Suppliers', href: '/suppliers', icon: Truck },
  { name: 'Recipes', href: '/recipes', icon: BookOpen },
  { name: 'Inv. Settings', href: '/inventory-settings', icon: Settings },
]

const menuItems: NavItem[] = [
  { name: 'Categories', href: '/categories', icon: FolderTree },
  { name: 'Menu Items', href: '/menu-items', icon: UtensilsCrossed },
  { name: 'Modifiers', href: '/modifiers', icon: Sliders },
  { name: 'Tables', href: '/tables', icon: Grid3X3 },
  { name: 'Kitchen Stations', href: '/kitchen-stations', icon: ChefHat },
]

const staffItems: NavItem[] = [
  { name: 'Staff Management', href: '/staff', icon: UserCheck },
  { name: 'Roles & Permissions', href: '/roles', icon: Shield },
  { name: 'Attendance', href: '/attendance', icon: Clock },
  { name: 'Commissions', href: '/commission-policies', icon: DollarSign },
  { name: 'Approvals', href: '/approval-rules', icon: Lock },
]

const customerItems: NavItem[] = [
  { name: 'Customers', href: '/customers', icon: Users },
  { name: 'Reservations', href: '/reservations', icon: Calendar },
  { name: 'Delivery Zones', href: '/delivery-zones', icon: MapPin },
  { name: 'Loyalty', href: '/loyalty', icon: Star },
  { name: 'Loyalty Transactions', href: '/loyalty-transactions', icon: Star },
  { name: 'Gift Cards', href: '/gift-cards', icon: Gift },
]

const adminItems: NavItem[] = [
  { name: 'General Settings', href: '/settings', icon: Settings },
  { name: 'Branches', href: '/branches', icon: Store },
  { name: 'Printers', href: '/printers', icon: Printer },
  { name: 'Receipt Templates', href: '/receipt-templates', icon: Receipt },
  { name: 'Payment Methods', href: '/payment-methods', icon: CreditCard },
  { name: 'Currencies', href: '/currencies', icon: Coins },
  { name: 'Exchange Rates', href: '/exchange-rates', icon: ArrowLeftRight },
  { name: 'Audit Logs', href: '/audit-logs', icon: FileSearch },
  { name: 'Delivery Zones', href: '/delivery-zones', icon: MapPin },
]

const styles = {
  bgColor: '#121212',
  cardBg: 'rgba(255, 255, 255, 0.08)',
  cardBorder: 'rgba(255, 255, 255, 0.1)',
  accent: '#0078d4',
}

export default function Home() {
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [currentView, setCurrentView] = useState<ViewType>('main')
  const [pageTitle, setPageTitle] = useState('Home Dashboard')

  const openCategory = (view: ViewType, title: string) => {
    setCurrentView(view)
    setPageTitle(title)
  }

  const goHome = () => {
    setCurrentView('main')
    setPageTitle('Home Dashboard')
  }

  const getViewItems = (): NavItem[] => {
    switch (currentView) {
      case 'inventory': return inventoryItems
      case 'menu': return menuItems
      case 'staff': return staffItems
      case 'customers': return customerItems
      case 'admin': return adminItems
      default: return []
    }
  }

  return (
    <div className="min-h-screen flex flex-col" style={{
      backgroundColor: styles.bgColor,
      backgroundImage: 'radial-gradient(circle at top right, #1f1f2e 0%, #0f0f13 100%)',
      color: 'white',
      fontFamily: "'Segoe UI', sans-serif"
    }}>
      {/* Navbar */}
      <div className="h-[60px] flex items-center px-[30px] z-[100]" style={{
        borderBottom: `1px solid ${styles.cardBorder}`,
        backdropFilter: 'blur(10px)'
      }}>
        {currentView !== 'main' && (
          <button 
            onClick={goHome}
            className="flex items-center gap-2 pr-5 mr-5 hover:text-[#0078d4] transition-colors"
            style={{ borderRight: '1px solid #333' }}
          >
            <ArrowLeft size={20} />
            Back
          </button>
        )}
        <div className="text-xl font-semibold flex-1">{pageTitle}</div>
        <div className="flex items-center gap-4">
          <span className="text-sm text-gray-400">{user?.companyName} • {user?.name}</span>
          <button 
            onClick={logout}
            className="flex items-center gap-2 text-gray-400 hover:text-white transition-colors"
          >
            <LogOut size={18} />
          </button>
        </div>
      </div>

      {/* View Container */}
      <div className="flex-1 relative overflow-hidden">
        {/* Main View */}
        <div 
          className={`absolute inset-0 p-10 overflow-y-auto transition-all duration-400 ${
            currentView === 'main' ? 'opacity-100 translate-x-0 pointer-events-auto' : 'opacity-0 translate-x-[50px] pointer-events-none'
          }`}
        >
          <div className="grid gap-5 max-w-[1400px] mx-auto" style={{
            gridTemplateColumns: 'repeat(auto-fill, minmax(180px, 1fr))'
          }}>
            {/* POS - Hero Card */}
            <button
              onClick={() => navigate('/pos')}
              className="col-span-2 row-span-2 rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left"
              style={{
                background: 'linear-gradient(135deg, #00b09b, #96c93d)',
                border: `1px solid ${styles.cardBorder}`,
                aspectRatio: '1/1'
              }}
            >
              <ShoppingCart size={64} className="opacity-80" />
              <div>
                <h3 className="text-3xl font-medium m-0">POS Terminal</h3>
                <span className="text-sm text-white/70 mt-1 block">Start New Sale</span>
              </div>
            </button>

            {/* Analytics */}
            <button
              onClick={() => navigate('/dashboard')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left"
              style={{
                background: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <BarChart3 size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Analytics</h3>
                <span className="text-sm text-white/60 mt-1 block">Dashboard</span>
              </div>
            </button>

            {/* Inventory */}
            <button
              onClick={() => openCategory('inventory', 'Inventory Management')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 right-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {inventoryItems.length} Items
              </span>
              <Package size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Inventory</h3>
                <span className="text-sm text-white/60 mt-1 block">Stock, Suppliers, Goods Receipt</span>
              </div>
            </button>

            {/* Menu */}
            <button
              onClick={() => openCategory('menu', 'Menu Setup')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 right-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {menuItems.length} Items
              </span>
              <UtensilsCrossed size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Menu</h3>
                <span className="text-sm text-white/60 mt-1 block">Items, Categories, Modifiers</span>
              </div>
            </button>

            {/* Staff */}
            <button
              onClick={() => openCategory('staff', 'HR & Staff')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 right-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {staffItems.length} Items
              </span>
              <UserCheck size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Staff</h3>
                <span className="text-sm text-white/60 mt-1 block">Staff, Roles, Attendance</span>
              </div>
            </button>

            {/* Customers */}
            <button
              onClick={() => openCategory('customers', 'Customers & Loyalty')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 right-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {customerItems.length} Items
              </span>
              <Users size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Customers</h3>
                <span className="text-sm text-white/60 mt-1 block">Profiles, Loyalty, Gift Cards</span>
              </div>
            </button>

            {/* Settings */}
            <button
              onClick={() => openCategory('admin', 'System Configuration')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #434343 0%, #000000 100%)',
                border: '1px solid #555'
              }}
            >
              <span className="absolute top-4 right-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {adminItems.length} Items
              </span>
              <Settings size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Settings</h3>
                <span className="text-sm text-white/60 mt-1 block">Printers, Taxes, Branches</span>
              </div>
            </button>

            {/* Reports */}
            <button
              onClick={() => navigate('/reports')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left"
              style={{
                background: '#333',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <FileText size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">Reports</h3>
                <span className="text-sm text-white/60 mt-1 block">Sales & Analytics</span>
              </div>
            </button>
          </div>
        </div>

        {/* Sub Views */}
        {['inventory', 'menu', 'staff', 'customers', 'admin'].map(view => (
          <div 
            key={view}
            className={`absolute inset-0 p-10 overflow-y-auto transition-all duration-400 ${
              currentView === view ? 'opacity-100 translate-x-0 pointer-events-auto' : 'opacity-0 translate-x-[50px] pointer-events-none'
            }`}
          >
            <h2 className="text-2xl font-light mb-5 max-w-[1400px] mx-auto">{pageTitle}</h2>
            <div className="grid gap-5 max-w-[1400px] mx-auto" style={{
              gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))'
            }}>
              {currentView === view && getViewItems().map(item => (
                <button
                  key={item.href}
                  onClick={() => navigate(item.href)}
                  className="rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:bg-gray-900/[0.12] hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left"
                  style={{
                    background: '#252525',
                    border: `1px solid ${styles.cardBorder}`,
                    aspectRatio: '2/1'
                  }}
                >
                  <item.icon size={28} className="opacity-80" />
                  <h3 className="text-base font-medium m-0">{item.name}</h3>
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
