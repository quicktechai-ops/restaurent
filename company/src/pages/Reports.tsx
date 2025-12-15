import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import api from '../lib/api'
import { BarChart3, TrendingUp, Package, Users, DollarSign, Calendar, Download, FileText } from 'lucide-react'

export default function Reports() {
  const [activeTab, setActiveTab] = useState('sales')
  const [dateRange, setDateRange] = useState({ from: new Date(Date.now() - 30*24*60*60*1000).toISOString().split('T')[0], to: new Date().toISOString().split('T')[0] })

  const { data: salesReport } = useQuery({ 
    queryKey: ['report-sales', dateRange], 
    queryFn: () => api.get('/api/company/reports/sales', { params: dateRange }).then(r => r.data),
    enabled: activeTab === 'sales'
  })

  const { data: inventoryReport } = useQuery({ 
    queryKey: ['report-inventory'], 
    queryFn: () => api.get('/api/company/reports/inventory').then(r => r.data),
    enabled: activeTab === 'inventory'
  })

  const { data: customerReport } = useQuery({ 
    queryKey: ['report-customers', dateRange], 
    queryFn: () => api.get('/api/company/reports/customers', { params: dateRange }).then(r => r.data),
    enabled: activeTab === 'customers'
  })

  const tabs = [
    { id: 'sales', label: 'Sales', icon: TrendingUp },
    { id: 'inventory', label: 'Inventory', icon: Package },
    { id: 'customers', label: 'Customers', icon: Users },
  ]

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800 flex items-center gap-2"><BarChart3 size={28} /> Reports</h1>
        <button className="btn-secondary flex items-center gap-2"><Download size={18} /> Export</button>
      </div>

      {/* Date Range Filter */}
      <div className="card p-4 mb-6 flex items-center gap-4">
        <Calendar size={18} className="text-gray-500" />
        <span className="text-sm font-medium">Date Range:</span>
        <input type="date" value={dateRange.from} onChange={(e) => setDateRange({ ...dateRange, from: e.target.value })} className="input-field" />
        <span>to</span>
        <input type="date" value={dateRange.to} onChange={(e) => setDateRange({ ...dateRange, to: e.target.value })} className="input-field" />
      </div>

      {/* Tabs */}
      <div className="flex gap-2 mb-6 border-b">
        {tabs.map(tab => (
          <button key={tab.id} onClick={() => setActiveTab(tab.id)}
            className={`flex items-center gap-2 px-4 py-2 border-b-2 transition-colors ${activeTab === tab.id ? 'border-primary-600 text-primary-600' : 'border-transparent text-gray-600 hover:text-gray-800'}`}>
            <tab.icon size={18} /> {tab.label}
          </button>
        ))}
      </div>

      {/* Sales Report */}
      {activeTab === 'sales' && (
        <div className="space-y-6">
          {/* Summary Cards */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="card p-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center"><DollarSign size={20} className="text-green-600" /></div>
                <div>
                  <p className="text-2xl font-bold">${salesReport?.totalRevenue?.toFixed(2) || '0.00'}</p>
                  <p className="text-sm text-gray-500">Total Revenue</p>
                </div>
              </div>
            </div>
            <div className="card p-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center"><FileText size={20} className="text-blue-600" /></div>
                <div>
                  <p className="text-2xl font-bold">{salesReport?.totalOrders || 0}</p>
                  <p className="text-sm text-gray-500">Total Orders</p>
                </div>
              </div>
            </div>
            <div className="card p-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-purple-100 rounded-lg flex items-center justify-center"><TrendingUp size={20} className="text-purple-600" /></div>
                <div>
                  <p className="text-2xl font-bold">${salesReport?.averageTicket?.toFixed(2) || '0.00'}</p>
                  <p className="text-sm text-gray-500">Avg. Ticket</p>
                </div>
              </div>
            </div>
            <div className="card p-4">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-orange-100 rounded-lg flex items-center justify-center"><Users size={20} className="text-orange-600" /></div>
                <div>
                  <p className="text-2xl font-bold">{salesReport?.uniqueCustomers || 0}</p>
                  <p className="text-sm text-gray-500">Unique Customers</p>
                </div>
              </div>
            </div>
          </div>

          {/* Sales by Category */}
          <div className="card p-4">
            <h3 className="font-semibold mb-4">Sales by Category</h3>
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="text-left p-3">Category</th>
                  <th className="text-left p-3">Items Sold</th>
                  <th className="text-left p-3">Revenue</th>
                  <th className="text-left p-3">% of Total</th>
                </tr>
              </thead>
              <tbody>
                {salesReport?.byCategory?.map((cat: any, idx: number) => (
                  <tr key={idx} className="border-t">
                    <td className="p-3 font-medium">{cat.name}</td>
                    <td className="p-3">{cat.quantity}</td>
                    <td className="p-3">${cat.revenue?.toFixed(2)}</td>
                    <td className="p-3">
                      <div className="flex items-center gap-2">
                        <div className="w-20 h-2 bg-gray-200 rounded-full overflow-hidden">
                          <div className="h-full bg-primary-500" style={{ width: `${cat.percentage}%` }}></div>
                        </div>
                        <span className="text-sm">{cat.percentage?.toFixed(1)}%</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!salesReport?.byCategory || salesReport.byCategory.length === 0) && <p className="text-center text-gray-500 py-4">No data available</p>}
          </div>

          {/* Top Selling Items */}
          <div className="card p-4">
            <h3 className="font-semibold mb-4">Top Selling Items</h3>
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="text-left p-3">#</th>
                  <th className="text-left p-3">Item</th>
                  <th className="text-left p-3">Quantity</th>
                  <th className="text-left p-3">Revenue</th>
                </tr>
              </thead>
              <tbody>
                {salesReport?.topItems?.map((item: any, idx: number) => (
                  <tr key={idx} className="border-t">
                    <td className="p-3 font-medium">{idx + 1}</td>
                    <td className="p-3">{item.name}</td>
                    <td className="p-3">{item.quantity}</td>
                    <td className="p-3 font-medium">${item.revenue?.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!salesReport?.topItems || salesReport.topItems.length === 0) && <p className="text-center text-gray-500 py-4">No data available</p>}
          </div>
        </div>
      )}

      {/* Inventory Report */}
      {activeTab === 'inventory' && (
        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="card p-4">
              <p className="text-2xl font-bold">{inventoryReport?.totalItems || 0}</p>
              <p className="text-sm text-gray-500">Total Items</p>
            </div>
            <div className="card p-4">
              <p className="text-2xl font-bold text-red-600">{inventoryReport?.lowStockItems || 0}</p>
              <p className="text-sm text-gray-500">Low Stock Items</p>
            </div>
            <div className="card p-4">
              <p className="text-2xl font-bold">${inventoryReport?.totalValue?.toFixed(2) || '0.00'}</p>
              <p className="text-sm text-gray-500">Total Inventory Value</p>
            </div>
          </div>

          <div className="card p-4">
            <h3 className="font-semibold mb-4">Stock Status</h3>
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="text-left p-3">Item</th>
                  <th className="text-left p-3">Current Stock</th>
                  <th className="text-left p-3">Min Level</th>
                  <th className="text-left p-3">Unit Cost</th>
                  <th className="text-left p-3">Total Value</th>
                  <th className="text-left p-3">Status</th>
                </tr>
              </thead>
              <tbody>
                {inventoryReport?.items?.map((item: any) => (
                  <tr key={item.id} className="border-t">
                    <td className="p-3 font-medium">{item.name}</td>
                    <td className="p-3">{item.quantity} {item.unit}</td>
                    <td className="p-3">{item.minLevel} {item.unit}</td>
                    <td className="p-3">${item.cost?.toFixed(2)}</td>
                    <td className="p-3">${item.totalValue?.toFixed(2)}</td>
                    <td className="p-3">
                      <span className={`px-2 py-1 rounded text-xs ${item.quantity <= item.minLevel ? 'bg-red-100 text-red-700' : 'bg-green-100 text-green-700'}`}>
                        {item.quantity <= item.minLevel ? 'Low Stock' : 'OK'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!inventoryReport?.items || inventoryReport.items.length === 0) && <p className="text-center text-gray-500 py-4">No data available</p>}
          </div>
        </div>
      )}

      {/* Customer Report */}
      {activeTab === 'customers' && (
        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="card p-4">
              <p className="text-2xl font-bold">{customerReport?.totalCustomers || 0}</p>
              <p className="text-sm text-gray-500">Total Customers</p>
            </div>
            <div className="card p-4">
              <p className="text-2xl font-bold text-green-600">{customerReport?.newCustomers || 0}</p>
              <p className="text-sm text-gray-500">New This Period</p>
            </div>
            <div className="card p-4">
              <p className="text-2xl font-bold">{customerReport?.repeatRate?.toFixed(1) || 0}%</p>
              <p className="text-sm text-gray-500">Repeat Rate</p>
            </div>
          </div>

          <div className="card p-4">
            <h3 className="font-semibold mb-4">Top Customers</h3>
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="text-left p-3">#</th>
                  <th className="text-left p-3">Customer</th>
                  <th className="text-left p-3">Orders</th>
                  <th className="text-left p-3">Total Spent</th>
                  <th className="text-left p-3">Last Visit</th>
                </tr>
              </thead>
              <tbody>
                {customerReport?.topCustomers?.map((cust: any, idx: number) => (
                  <tr key={cust.id} className="border-t">
                    <td className="p-3 font-medium">{idx + 1}</td>
                    <td className="p-3">{cust.name}</td>
                    <td className="p-3">{cust.orderCount}</td>
                    <td className="p-3 font-medium">${cust.totalSpent?.toFixed(2)}</td>
                    <td className="p-3 text-sm text-gray-600">{cust.lastVisit ? new Date(cust.lastVisit).toLocaleDateString() : '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!customerReport?.topCustomers || customerReport.topCustomers.length === 0) && <p className="text-center text-gray-500 py-4">No data available</p>}
          </div>
        </div>
      )}
    </div>
  )
}
