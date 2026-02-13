import { useState, useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
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
  description?: string
  href: string
  icon: any
}

const getInventoryItems = (t: any): NavItem[] => [
  // Daily operations first
  { name: t('nav.inventory'), description: t('nav.inventoryItemDesc'), href: '/inventory', icon: Package },
  { name: t('nav.stockCount'), description: t('nav.stockCountDesc'), href: '/stock-count', icon: ClipboardList },
  { name: t('nav.stockAdjustment'), description: t('nav.stockAdjustmentDesc'), href: '/stock-adjustment', icon: Sliders },
  { name: t('nav.wastage'), description: t('nav.wastageDesc'), href: '/wastage', icon: AlertTriangle },
  // Procurement flow: Suppliers → PO → Receive
  { name: t('nav.suppliers'), description: t('nav.suppliersDesc'), href: '/suppliers', icon: Truck },
  { name: t('nav.purchaseOrders'), description: t('nav.purchaseOrdersDesc'), href: '/purchase-orders', icon: ShoppingCart },
  { name: t('nav.goodsReceipt'), description: t('nav.goodsReceiptDesc'), href: '/goods-receipt', icon: ClipboardList },
  { name: t('nav.stockMovements'), description: t('nav.stockMovementsDesc'), href: '/stock-movements', icon: Truck },
  // Setup & config
  { name: t('nav.inventorySettings'), description: t('nav.inventorySettingsDesc'), href: '/inventory-settings', icon: Settings },
]

const getMenuItems = (t: any): NavItem[] => [
  // Build menu: Categories → Items → Modifiers
  { name: t('nav.categories'), description: t('nav.categoriesDesc'), href: '/categories', icon: FolderTree },
  { name: t('nav.menuItems'), description: t('nav.menuItemsDesc'), href: '/menu-items', icon: UtensilsCrossed },
  { name: t('nav.modifiers'), description: t('nav.modifiersDesc'), href: '/modifiers', icon: Sliders },
  // Floor & kitchen setup
  { name: t('nav.kitchenStations'), description: t('nav.kitchenStationsDesc'), href: '/kitchen-stations', icon: ChefHat },
  { name: t('nav.tables'), description: t('nav.tablesDesc'), href: '/tables', icon: Grid3X3 },
  // Recipes
  { name: t('nav.recipes'), description: t('nav.recipesDesc'), href: '/recipes', icon: BookOpen },
]

const getStaffItems = (t: any): NavItem[] => [
  // Core HR first
  { name: t('nav.staffManagement'), description: t('nav.staffManagementDesc'), href: '/staff', icon: UserCheck },
  { name: t('nav.rolesPermissions'), description: t('nav.rolesPermissionsDesc'), href: '/roles', icon: Shield },
  { name: t('nav.attendance'), description: t('nav.attendanceDesc'), href: '/attendance', icon: Clock },
  // Policies & rules
  { name: t('nav.commissions'), description: t('nav.commissionsDesc'), href: '/commission-policies', icon: DollarSign },
  { name: t('nav.approvals'), description: t('nav.approvalsDesc'), href: '/approval-rules', icon: Lock },
]

const getCustomerItems = (t: any): NavItem[] => [
  // Customer management
  { name: t('nav.customers'), description: t('nav.customersItemDesc'), href: '/customers', icon: Users },
  { name: t('nav.reservations'), description: t('nav.reservationsDesc'), href: '/reservations', icon: Calendar },
  // Loyalty & rewards
  { name: t('nav.loyalty'), description: t('nav.loyaltyDesc'), href: '/loyalty', icon: Star },
  { name: t('nav.loyaltyTransactions'), description: t('nav.loyaltyTransactionsDesc'), href: '/loyalty-transactions', icon: Star },
  { name: t('nav.giftCards'), description: t('nav.giftCardsDesc'), href: '/gift-cards', icon: Gift },
  // Delivery
  { name: t('nav.deliveryZones'), description: t('nav.deliveryZonesDesc'), href: '/delivery-zones', icon: MapPin },
]

const getAdminItems = (t: any): NavItem[] => [
  // Company setup
  { name: t('nav.generalSettings'), description: t('nav.generalSettingsDesc'), href: '/settings', icon: Settings },
  { name: t('nav.branches'), description: t('nav.branchesDesc'), href: '/branches', icon: Store },
  // Payment & finance
  { name: t('nav.paymentMethods'), description: t('nav.paymentMethodsDesc'), href: '/payment-methods', icon: CreditCard },
  { name: t('nav.currencies'), description: t('nav.currenciesDesc'), href: '/currencies', icon: Coins },
  { name: t('nav.exchangeRates'), description: t('nav.exchangeRatesDesc'), href: '/exchange-rates', icon: ArrowLeftRight },
  // Hardware & printing
  { name: t('nav.printers'), description: t('nav.printersDesc'), href: '/printers', icon: Printer },
  { name: t('nav.receiptTemplates'), description: t('nav.receiptTemplatesDesc'), href: '/receipt-templates', icon: Receipt },
  // Operations
  { name: t('nav.deliveryZones'), description: t('nav.deliveryZonesDesc'), href: '/delivery-zones', icon: MapPin },
  { name: t('nav.auditLogs'), description: t('nav.auditLogsDesc'), href: '/audit-logs', icon: FileSearch },
]

const styles = {
  bgColor: '#1a1d23',
  cardBg: 'rgba(255, 255, 255, 0.05)',
  cardBorder: 'rgba(255, 255, 255, 0.08)',
  accent: '#4da6e8',
}

export default function Home() {
  const navigate = useNavigate()
  const location = useLocation()
  const { t, i18n } = useTranslation()
  const { user, logout } = useAuth()
  const [currentView, setCurrentView] = useState<ViewType>('main')
  const [pageTitle, setPageTitle] = useState(t('nav.homeDashboard'))

  // Section title mapping
  const sectionTitles: Record<string, string> = {
    inventory: t('nav.inventoryManagement'),
    menu: t('nav.menuSetup'),
    staff: t('nav.hrStaff'),
    customers: t('nav.customersLoyalty'),
    admin: t('nav.systemConfiguration'),
  }

  // Auto-open the correct section when navigating back from a sub-page
  useEffect(() => {
    const state = location.state as { section?: string } | null
    if (state?.section && sectionTitles[state.section]) {
      setCurrentView(state.section as ViewType)
      setPageTitle(sectionTitles[state.section])
      // Clear the state so refreshing doesn't re-open the section
      window.history.replaceState({}, '')
    }
  }, [location.state])

  // Set RTL direction based on language
  useEffect(() => {
    document.documentElement.dir = i18n.language === 'ar' ? 'rtl' : 'ltr'
    document.documentElement.lang = i18n.language
  }, [i18n.language])

  const openCategory = (view: ViewType, title: string) => {
    setCurrentView(view)
    setPageTitle(title)
  }

  const goHome = () => {
    setCurrentView('main')
    setPageTitle(t('nav.homeDashboard'))
  }

  const inventoryItems = getInventoryItems(t)
  const menuItems = getMenuItems(t)
  const staffItems = getStaffItems(t)
  const customerItems = getCustomerItems(t)
  const adminItems = getAdminItems(t)

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
      backgroundImage: 'radial-gradient(circle at top right, #242830 0%, #1a1d23 100%)',
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
            className="flex items-center gap-2 pr-5 mr-5 hover:text-[#4da6e8] transition-colors"
            style={{ borderRight: '1px solid rgba(255,255,255,0.08)' }}
          >
            <ArrowLeft size={20} />
            {t('common.back')}
          </button>
        )}
        <div className="text-xl font-semibold flex-1">{pageTitle}</div>
        <div className="flex items-center gap-4">
          <span className="text-sm text-gray-400">{user?.companyName} • {user?.name}</span>
          <button 
            onClick={logout}
            className="flex items-center gap-2 text-gray-400 hover:text-white transition-colors"
            title={t('common.logout')}
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
                <h3 className="text-3xl font-medium m-0">{t('nav.posTerminal')}</h3>
                <span className="text-sm text-white/70 mt-1 block">{t('nav.startNewSale')}</span>
              </div>
            </button>

            {/* Orders History */}
            <button
              onClick={() => navigate('/orders-history')}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left"
              style={{
                background: 'linear-gradient(135deg, #0f2027 0%, #203a43 50%, #2c5364 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <FileText size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">{t('ordersHistory.title')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('ordersHistory.description')}</span>
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
                <h3 className="text-lg font-medium m-0">{t('nav.analytics')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('common.dashboard')}</span>
              </div>
            </button>

            {/* Inventory */}
            <button
              onClick={() => openCategory('inventory', t('nav.inventoryManagement'))}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 ltr:right-4 rtl:left-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {inventoryItems.length} {t('common.items')}
              </span>
              <Package size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">{t('nav.inventory')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('nav.inventoryDesc')}</span>
              </div>
            </button>

            {/* Menu */}
            <button
              onClick={() => openCategory('menu', t('nav.menuSetup'))}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 ltr:right-4 rtl:left-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {menuItems.length} {t('common.items')}
              </span>
              <UtensilsCrossed size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">{t('nav.menu')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('nav.menuDesc')}</span>
              </div>
            </button>

            {/* Staff */}
            <button
              onClick={() => openCategory('staff', t('nav.hrStaff'))}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 ltr:right-4 rtl:left-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {staffItems.length} {t('common.items')}
              </span>
              <UserCheck size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">{t('nav.staff')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('nav.staffDesc')}</span>
              </div>
            </button>

            {/* Customers */}
            <button
              onClick={() => openCategory('customers', t('nav.customersLoyalty'))}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                border: `1px solid ${styles.cardBorder}`
              }}
            >
              <span className="absolute top-4 ltr:right-4 rtl:left-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {customerItems.length} {t('common.items')}
              </span>
              <Users size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">{t('nav.customers')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('nav.customersDesc')}</span>
              </div>
            </button>

            {/* Settings */}
            <button
              onClick={() => openCategory('admin', t('nav.systemConfiguration'))}
              className="aspect-square rounded-lg p-5 flex flex-col justify-between cursor-pointer transition-all hover:-translate-y-1 hover:shadow-[0_8px_20px_rgba(0,0,0,0.4)] text-left relative"
              style={{
                background: 'linear-gradient(135deg, #434343 0%, #000000 100%)',
                border: '1px solid #555'
              }}
            >
              <span className="absolute top-4 ltr:right-4 rtl:left-4 bg-gray-900/10 px-2 py-0.5 rounded-full text-xs">
                {adminItems.length} {t('common.items')}
              </span>
              <Settings size={40} className="opacity-80" />
              <div>
                <h3 className="text-lg font-medium m-0">{t('common.settings')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('nav.settingsDesc')}</span>
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
                <h3 className="text-lg font-medium m-0">{t('nav.reports')}</h3>
                <span className="text-sm text-white/60 mt-1 block">{t('nav.reportsDesc')}</span>
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
                    background: '#262a33',
                    border: `1px solid ${styles.cardBorder}`,
                    aspectRatio: '2/1'
                  }}
                >
                  <item.icon size={28} className="opacity-80" />
                  <div>
                    <h3 className="text-base font-medium m-0">{item.name}</h3>
                    {item.description && <span className="text-xs text-white/40 mt-1 block">{item.description}</span>}
                  </div>
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
