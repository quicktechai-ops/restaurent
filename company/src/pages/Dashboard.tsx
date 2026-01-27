import { useQuery } from '@tanstack/react-query'
import { dashboardApi } from '../lib/api'
import { useAuth } from '../contexts/AuthContext'
import {
  Building2,
  Users,
  UtensilsCrossed,
  FolderTree,
  Grid3X3,
  Calendar,
  AlertCircle,
} from 'lucide-react'
import { Link } from 'react-router-dom'

const styles = {
  cardBg: 'rgba(255, 255, 255, 0.08)',
  cardBorder: 'rgba(255, 255, 255, 0.1)',
  textMain: '#ffffff',
  textMuted: '#a0a0a0',
}

export default function Dashboard() {
  const { user } = useAuth()
  
  const { data: stats, isLoading, error } = useQuery({
    queryKey: ['dashboard'],
    queryFn: () => dashboardApi.getStats(),
  })

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="p-4 rounded-lg flex items-center gap-3" style={{ background: 'rgba(239,68,68,0.15)', border: '1px solid rgba(239,68,68,0.3)', color: '#fca5a5' }}>
        <AlertCircle />
        <span>Failed to load dashboard data</span>
      </div>
    )
  }

  const data = stats?.data

  const statCards = [
    { name: 'Branches', value: data?.totalBranches || 0, max: data?.maxBranches, icon: Building2, href: '/branches', color: 'bg-blue-500' },
    { name: 'Users', value: data?.totalUsers || 0, max: data?.maxUsers, icon: Users, href: '/users', color: 'bg-green-500' },
    { name: 'Menu Items', value: data?.totalMenuItems || 0, icon: UtensilsCrossed, href: '/menu-items', color: 'bg-orange-500' },
    { name: 'Categories', value: data?.totalCategories || 0, icon: FolderTree, href: '/categories', color: 'bg-purple-500' },
    { name: 'Tables', value: data?.totalTables || 0, icon: Grid3X3, href: '/tables', color: 'bg-pink-500' },
  ]

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold" style={{ color: styles.textMain }}>Dashboard</h1>
        <p style={{ color: styles.textMuted }}>Welcome back, {user?.name}</p>
      </div>

      {/* Plan Info */}
      <div className="p-6 mb-8 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-lg font-semibold" style={{ color: styles.textMain }}>{data?.companyName}</h2>
            <p style={{ color: styles.textMuted }}>
              Plan: <span className="font-medium text-blue-400">{data?.planName}</span>
            </p>
          </div>
          {data?.planExpiryDate && (
            <div className="flex items-center gap-2" style={{ color: styles.textMuted }}>
              <Calendar size={18} />
              <span>Expires: {new Date(data.planExpiryDate).toLocaleDateString()}</span>
            </div>
          )}
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-6 mb-8">
        {statCards.map((card) => (
          <Link 
            key={card.name} 
            to={card.href} 
            className="p-6 rounded-xl transition-all hover:-translate-y-1 hover:shadow-lg"
            style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}
          >
            <div className="flex items-center gap-4">
              <div className={`p-3 rounded-lg ${card.color}`}>
                <card.icon className="w-6 h-6 text-white" />
              </div>
              <div>
                <p className="text-2xl font-bold" style={{ color: styles.textMain }}>
                  {card.value}
                  {card.max && <span className="text-sm font-normal" style={{ color: styles.textMuted }}>/{card.max}</span>}
                </p>
                <p className="text-sm" style={{ color: styles.textMuted }}>{card.name}</p>
              </div>
            </div>
          </Link>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="p-6 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
        <h2 className="text-lg font-semibold mb-4" style={{ color: styles.textMain }}>Quick Actions</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <Link
            to="/branches"
            className="p-4 rounded-lg transition-all hover:-translate-y-1 text-center"
            style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}
          >
            <Building2 className="w-8 h-8 mx-auto mb-2 text-blue-400" />
            <span className="text-sm font-medium" style={{ color: styles.textMain }}>Add Branch</span>
          </Link>
          <Link
            to="/users"
            className="p-4 rounded-lg transition-all hover:-translate-y-1 text-center"
            style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}
          >
            <Users className="w-8 h-8 mx-auto mb-2 text-blue-400" />
            <span className="text-sm font-medium" style={{ color: styles.textMain }}>Add User</span>
          </Link>
          <Link
            to="/menu-items"
            className="p-4 rounded-lg transition-all hover:-translate-y-1 text-center"
            style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}
          >
            <UtensilsCrossed className="w-8 h-8 mx-auto mb-2 text-blue-400" />
            <span className="text-sm font-medium" style={{ color: styles.textMain }}>Add Menu Item</span>
          </Link>
          <Link
            to="/categories"
            className="p-4 rounded-lg transition-all hover:-translate-y-1 text-center"
            style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}
          >
            <FolderTree className="w-8 h-8 mx-auto mb-2 text-blue-400" />
            <span className="text-sm font-medium" style={{ color: styles.textMain }}>Add Category</span>
          </Link>
        </div>
      </div>
    </div>
  )
}
