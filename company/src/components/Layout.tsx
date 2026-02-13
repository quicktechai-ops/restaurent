import { ReactNode } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useAuth } from '../contexts/AuthContext'
import { ArrowLeft, LogOut, Home } from 'lucide-react'

interface LayoutProps {
  children: ReactNode
}

const styles = {
  bgColor: '#1a1d23',
  cardBorder: 'rgba(255, 255, 255, 0.08)',
  accent: '#4da6e8',
}

// Map each route to its parent section on the Home page
const routeToSection: Record<string, string> = {
  // Inventory section
  '/inventory': 'inventory',
  '/stock-count': 'inventory',
  '/stock-adjustment': 'inventory',
  '/stock-movements': 'inventory',
  '/goods-receipt': 'inventory',
  '/wastage': 'inventory',
  '/purchase-orders': 'inventory',
  '/suppliers': 'inventory',
  '/recipes': 'menu',
  '/inventory-settings': 'inventory',
  // Menu section
  '/categories': 'menu',
  '/menu-items': 'menu',
  '/modifiers': 'menu',
  '/tables': 'menu',
  '/kitchen-stations': 'menu',
  // Staff section
  '/staff': 'staff',
  '/roles': 'staff',
  '/attendance': 'staff',
  '/commission-policies': 'staff',
  '/approval-rules': 'staff',
  // Customers section
  '/customers': 'customers',
  '/reservations': 'customers',
  '/delivery-zones': 'customers',
  '/loyalty': 'customers',
  '/loyalty-transactions': 'customers',
  '/gift-cards': 'customers',
  // Admin section
  '/settings': 'admin',
  '/branches': 'admin',
  '/printers': 'admin',
  '/receipt-templates': 'admin',
  '/payment-methods': 'admin',
  '/currencies': 'admin',
  '/exchange-rates': 'admin',
  '/audit-logs': 'admin',
  // Standalone pages (go to home)
  '/dashboard': '',
  '/reports': '',
  '/orders-history': '',
  '/pos': '',
}

export default function Layout({ children }: LayoutProps) {
  const { user, logout } = useAuth()
  const { t } = useTranslation()
  const navigate = useNavigate()
  const location = useLocation()

  const isHomePage = location.pathname === '/'

  if (isHomePage) {
    return <>{children}</>
  }

  // Find the parent section for the current route
  const currentPath = location.pathname
  // Handle dynamic routes like /customers/:id
  const matchedSection = routeToSection[currentPath] 
    ?? (currentPath.startsWith('/customers/') ? 'customers' : '')
  const section = matchedSection

  const handleBack = () => {
    if (section) {
      // Navigate to home with section state so Home opens the right section
      navigate('/', { state: { section } })
    } else {
      navigate('/')
    }
  }

  return (
    <div className="min-h-screen flex flex-col" style={{
      backgroundColor: styles.bgColor,
      backgroundImage: 'radial-gradient(circle at top right, #242830 0%, #1a1d23 100%)',
      color: 'white',
      fontFamily: "'Segoe UI', sans-serif"
    }}>
      {/* Top Navigation Bar */}
      <div className="h-[60px] flex items-center px-[30px] z-[100] shrink-0" style={{
        borderBottom: `1px solid ${styles.cardBorder}`,
        backdropFilter: 'blur(10px)',
        background: 'rgba(0,0,0,0.2)'
      }}>
        <button 
          onClick={handleBack}
          className="flex items-center gap-2 pr-5 mr-5 hover:text-[#4da6e8] transition-colors"
          style={{ borderRight: `1px solid rgba(255,255,255,0.08)` }}
        >
          <ArrowLeft size={20} />
          {t('common.back')}
        </button>
        
        <button 
          onClick={() => navigate('/')}
          className="flex items-center gap-2 hover:text-[#4da6e8] transition-colors"
        >
          <Home size={20} />
        </button>

        <div className="flex-1" />

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

      {/* Main content */}
      <main className="flex-1 p-6 overflow-auto">{children}</main>
    </div>
  )
}
