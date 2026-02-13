import { createContext, useContext, useState, useEffect, ReactNode } from 'react'
import i18n from '../i18n'
import api from '../lib/api'

interface User {
  id: number
  name: string
  username: string
  email?: string
  companyId: number
  companyName: string
  branchId?: number
  branchName?: string
  role: string
}

interface AuthContextType {
  user: User | null
  token: string | null
  login: (username: string, password: string) => Promise<void>
  logout: () => void
  isLoading: boolean
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(localStorage.getItem('company_token'))
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const initAuth = async () => {
      const savedToken = localStorage.getItem('company_token')
      const savedUser = localStorage.getItem('company_user')
      
      if (savedToken && savedUser) {
        setToken(savedToken)
        setUser(JSON.parse(savedUser))
        
        // Load language setting on app init
        try {
          const settingsRes = await api.get('/api/company/settings')
          const langSetting = settingsRes.data?.find((s: any) => s.settingKey === 'DefaultLanguage')?.settingValue
          if (langSetting) {
            i18n.changeLanguage(langSetting)
            document.documentElement.dir = langSetting === 'ar' ? 'rtl' : 'ltr'
            document.documentElement.lang = langSetting
          }
        } catch (e) {
          // Settings may not exist yet
        }
      }
      setIsLoading(false)
    }
    initAuth()
  }, [])

  const login = async (username: string, password: string) => {
    const response = await api.post('/api/auth/company/login', { username, password })
    const { token: newToken, user: userData } = response.data
    
    setToken(newToken)
    setUser(userData)
    localStorage.setItem('company_token', newToken)
    localStorage.setItem('company_user', JSON.stringify(userData))

    // Load language setting after login
    try {
      const settingsRes = await api.get('/api/company/settings')
      const langSetting = settingsRes.data?.find((s: any) => s.settingKey === 'DefaultLanguage')?.settingValue
      if (langSetting) {
        i18n.changeLanguage(langSetting)
        document.documentElement.dir = langSetting === 'ar' ? 'rtl' : 'ltr'
        document.documentElement.lang = langSetting
      }
    } catch (e) {
      // Settings may not exist yet, use default
    }
  }

  const logout = () => {
    setToken(null)
    setUser(null)
    localStorage.removeItem('company_token')
    localStorage.removeItem('company_user')
  }

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isLoading }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
