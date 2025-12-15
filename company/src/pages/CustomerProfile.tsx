import { useParams, Link } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import api from '../lib/api'
import { ArrowLeft, User, Phone, Mail, MapPin, Star, Gift, ShoppingBag, TrendingUp, Calendar, Award } from 'lucide-react'

export default function CustomerProfile() {
  const { id } = useParams()
  
  const { data: customer, isLoading } = useQuery({
    queryKey: ['customer', id],
    queryFn: () => api.get(`/api/company/customers/${id}`).then(r => r.data)
  })
  
  const { data: stats } = useQuery({
    queryKey: ['customer-stats', id],
    queryFn: () => api.get(`/api/company/customers/${id}/stats`).then(r => r.data)
  })
  
  const { data: addresses } = useQuery({
    queryKey: ['customer-addresses', id],
    queryFn: () => api.get(`/api/company/customers/${id}/addresses`).then(r => r.data)
  })
  
  const { data: orderHistory } = useQuery({
    queryKey: ['customer-orders', id],
    queryFn: () => api.get(`/api/company/customers/${id}/orders`).then(r => r.data)
  })
  
  const { data: loyaltyInfo } = useQuery({
    queryKey: ['customer-loyalty', id],
    queryFn: () => api.get(`/api/company/customers/${id}/loyalty`).then(r => r.data)
  })

  if (isLoading) return <div className="p-6">Loading...</div>
  if (!customer) return <div className="p-6">Customer not found</div>

  return (
    <div className="p-6">
      <Link to="/customers" className="flex items-center gap-2 text-gray-600 hover:text-gray-800 mb-4">
        <ArrowLeft size={20} /> Back to Customers
      </Link>

      {/* Header */}
      <div className="card p-6 mb-6">
        <div className="flex items-start gap-6">
          <div className="w-20 h-20 bg-primary-100 rounded-full flex items-center justify-center">
            <User size={40} className="text-primary-600" />
          </div>
          <div className="flex-1">
            <h1 className="text-2xl font-bold text-gray-800">{customer.name}</h1>
            <p className="text-gray-500">{customer.customerCode || 'No code'}</p>
            <div className="flex gap-6 mt-3 text-sm">
              {customer.phone && <span className="flex items-center gap-1"><Phone size={14} /> {customer.phone}</span>}
              {customer.email && <span className="flex items-center gap-1"><Mail size={14} /> {customer.email}</span>}
            </div>
          </div>
          <div className="text-right">
            <span className={`px-3 py-1 rounded-full text-sm ${customer.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
              {customer.isActive ? 'Active' : 'Inactive'}
            </span>
          </div>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div className="card p-4">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
              <ShoppingBag size={20} className="text-blue-600" />
            </div>
            <div>
              <p className="text-2xl font-bold">{stats?.totalOrders || 0}</p>
              <p className="text-sm text-gray-500">Total Orders</p>
            </div>
          </div>
        </div>
        <div className="card p-4">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center">
              <TrendingUp size={20} className="text-green-600" />
            </div>
            <div>
              <p className="text-2xl font-bold">${stats?.totalSpent?.toFixed(2) || '0.00'}</p>
              <p className="text-sm text-gray-500">Total Spent</p>
            </div>
          </div>
        </div>
        <div className="card p-4">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center">
              <Star size={20} className="text-purple-600" />
            </div>
            <div>
              <p className="text-2xl font-bold">${stats?.averageTicket?.toFixed(2) || '0.00'}</p>
              <p className="text-sm text-gray-500">Avg. Ticket</p>
            </div>
          </div>
        </div>
        <div className="card p-4">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-orange-100 rounded-lg flex items-center justify-center">
              <Calendar size={20} className="text-orange-600" />
            </div>
            <div>
              <p className="text-2xl font-bold">{stats?.totalVisits || 0}</p>
              <p className="text-sm text-gray-500">Total Visits</p>
            </div>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Loyalty Info */}
        <div className="card p-4">
          <h2 className="font-semibold mb-4 flex items-center gap-2"><Award size={18} /> Loyalty Program</h2>
          {loyaltyInfo ? (
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-gray-600">Card Number</span>
                <span className="font-medium">{loyaltyInfo.cardNumber || '-'}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Current Tier</span>
                <span className="font-medium text-primary-600">{loyaltyInfo.tierName || 'None'}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Points Balance</span>
                <span className="font-bold text-lg">{loyaltyInfo.pointsBalance || 0}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Member Since</span>
                <span className="font-medium">{loyaltyInfo.memberSince ? new Date(loyaltyInfo.memberSince).toLocaleDateString() : '-'}</span>
              </div>
            </div>
          ) : (
            <p className="text-gray-500 text-center py-4">Not enrolled in loyalty program</p>
          )}
        </div>

        {/* Addresses */}
        <div className="card p-4">
          <h2 className="font-semibold mb-4 flex items-center gap-2"><MapPin size={18} /> Delivery Addresses</h2>
          <div className="space-y-3 max-h-48 overflow-y-auto">
            {addresses?.map((addr: any) => (
              <div key={addr.id} className="border-b pb-2">
                <span className="font-medium text-primary-600 text-sm">{addr.label}</span>
                <p className="text-sm text-gray-600">{addr.addressLine1}</p>
                <p className="text-xs text-gray-500">{[addr.area, addr.city].filter(Boolean).join(', ')}</p>
              </div>
            ))}
            {(!addresses || addresses.length === 0) && <p className="text-gray-500 text-center py-4">No addresses</p>}
          </div>
        </div>

        {/* Favorite Items */}
        <div className="card p-4">
          <h2 className="font-semibold mb-4 flex items-center gap-2"><Gift size={18} /> Favorite Items</h2>
          <div className="space-y-2 max-h-48 overflow-y-auto">
            {stats?.favoriteItems?.map((item: any, idx: number) => (
              <div key={idx} className="flex justify-between items-center">
                <span className="text-sm">{item.name}</span>
                <span className="text-xs bg-gray-100 px-2 py-0.5 rounded">{item.quantity}x</span>
              </div>
            ))}
            {(!stats?.favoriteItems || stats.favoriteItems.length === 0) && <p className="text-gray-500 text-center py-4">No orders yet</p>}
          </div>
        </div>
      </div>

      {/* Order History */}
      <div className="card mt-6">
        <h2 className="font-semibold p-4 border-b flex items-center gap-2"><ShoppingBag size={18} /> Order History</h2>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b bg-gray-50">
                <th className="text-left p-3">Order #</th>
                <th className="text-left p-3">Date</th>
                <th className="text-left p-3">Type</th>
                <th className="text-left p-3">Items</th>
                <th className="text-left p-3">Total</th>
                <th className="text-left p-3">Status</th>
              </tr>
            </thead>
            <tbody>
              {orderHistory?.map((order: any) => (
                <tr key={order.id} className="border-b hover:bg-gray-50">
                  <td className="p-3 font-medium">#{order.orderNumber}</td>
                  <td className="p-3">{new Date(order.createdAt).toLocaleDateString()}</td>
                  <td className="p-3">{order.orderType}</td>
                  <td className="p-3">{order.itemCount} items</td>
                  <td className="p-3 font-medium">${order.grandTotal?.toFixed(2)}</td>
                  <td className="p-3">
                    <span className={`px-2 py-0.5 rounded text-xs ${order.status === 'Paid' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'}`}>
                      {order.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          {(!orderHistory || orderHistory.length === 0) && <p className="text-center text-gray-500 py-8">No orders yet</p>}
        </div>
      </div>
    </div>
  )
}
