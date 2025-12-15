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

export default function Dashboard() {
  const { user } = useAuth()
  
  const { data: stats, isLoading, error } = useQuery({
    queryKey: ['dashboard'],
    queryFn: () => dashboardApi.getStats(),
  })

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  if (error) {
    return (
      <div className="bg-red-50 text-red-700 p-4 rounded-lg flex items-center gap-3">
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
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600">Welcome back, {user?.name}</p>
      </div>

      {/* Plan Info */}
      <div className="card p-6 mb-8">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-lg font-semibold text-gray-900">{data?.companyName}</h2>
            <p className="text-gray-600">
              Plan: <span className="font-medium text-primary-600">{data?.planName}</span>
            </p>
          </div>
          {data?.planExpiryDate && (
            <div className="flex items-center gap-2 text-gray-600">
              <Calendar size={18} />
              <span>Expires: {new Date(data.planExpiryDate).toLocaleDateString()}</span>
            </div>
          )}
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-5 gap-6 mb-8">
        {statCards.map((card) => (
          <Link key={card.name} to={card.href} className="card p-6 hover:shadow-md transition-shadow">
            <div className="flex items-center gap-4">
              <div className={`p-3 rounded-lg ${card.color}`}>
                <card.icon className="w-6 h-6 text-white" />
              </div>
              <div>
                <p className="text-2xl font-bold text-gray-900">
                  {card.value}
                  {card.max && <span className="text-sm text-gray-400 font-normal">/{card.max}</span>}
                </p>
                <p className="text-sm text-gray-600">{card.name}</p>
              </div>
            </div>
          </Link>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="card p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <Link
            to="/branches"
            className="p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
          >
            <Building2 className="w-8 h-8 mx-auto mb-2 text-primary-600" />
            <span className="text-sm font-medium">Add Branch</span>
          </Link>
          <Link
            to="/users"
            className="p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
          >
            <Users className="w-8 h-8 mx-auto mb-2 text-primary-600" />
            <span className="text-sm font-medium">Add User</span>
          </Link>
          <Link
            to="/menu-items"
            className="p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
          >
            <UtensilsCrossed className="w-8 h-8 mx-auto mb-2 text-primary-600" />
            <span className="text-sm font-medium">Add Menu Item</span>
          </Link>
          <Link
            to="/categories"
            className="p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors text-center"
          >
            <FolderTree className="w-8 h-8 mx-auto mb-2 text-primary-600" />
            <span className="text-sm font-medium">Add Category</span>
          </Link>
        </div>
      </div>
    </div>
  )
}
