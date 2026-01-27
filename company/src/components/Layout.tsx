import { ReactNode } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../contexts/AuthContext'
import { ArrowLeft, LogOut, Home } from 'lucide-react'

interface LayoutProps {
  children: ReactNode
}

const styles = {
  bgColor: '#121212',
  cardBorder: 'rgba(255, 255, 255, 0.1)',
  accent: '#0078d4',
}

export default function Layout({ children }: LayoutProps) {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const isHomePage = location.pathname === '/'

  if (isHomePage) {
    return <>{children}</>
  }

  return (
    <div className="min-h-screen flex flex-col" style={{
      backgroundColor: styles.bgColor,
      backgroundImage: 'radial-gradient(circle at top right, #1f1f2e 0%, #0f0f13 100%)',
      color: 'white',
      fontFamily: "'Segoe UI', sans-serif"
    }}>
      {/* Top Navigation Bar */}
      <div className="h-[60px] flex items-center px-[30px] z-[100] shrink-0" style={{
        borderBottom: `1px solid ${styles.cardBorder}`,
        backdropFilter: 'blur(10px)',
        background: 'rgba(0,0,0,0.3)'
      }}>
        <button 
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 pr-5 mr-5 hover:text-[#0078d4] transition-colors"
          style={{ borderRight: '1px solid #333' }}
        >
          <ArrowLeft size={20} />
          Back
        </button>
        
        <button 
          onClick={() => navigate('/')}
          className="flex items-center gap-2 hover:text-[#0078d4] transition-colors"
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
