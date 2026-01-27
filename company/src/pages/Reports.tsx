import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import api from '../lib/api'
import { BarChart3, TrendingUp, Package, Users, DollarSign, Calendar, Download, FileText } from 'lucide-react'

const styles = {
  cardBg: 'rgba(255, 255, 255, 0.08)',
  cardBorder: 'rgba(255, 255, 255, 0.1)',
  textMain: '#ffffff',
  textMuted: '#a0a0a0',
  inputBg: 'rgba(255, 255, 255, 0.05)',
}

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
        <h1 className="text-2xl font-bold flex items-center gap-2" style={{ color: styles.textMain }}><BarChart3 size={28} /> Reports</h1>
        <button className="px-4 py-2 rounded-lg flex items-center gap-2 transition-all hover:-translate-y-0.5" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}><Download size={18} /> Export</button>
      </div>

      {/* Date Range Filter */}
      <div className="p-4 mb-6 flex items-center gap-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
        <Calendar size={18} style={{ color: styles.textMuted }} />
        <span className="text-sm font-medium" style={{ color: styles.textMain }}>Date Range:</span>
        <input type="date" value={dateRange.from} onChange={(e) => setDateRange({ ...dateRange, from: e.target.value })} className="px-3 py-2 rounded-lg outline-none" style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }} />
        <span style={{ color: styles.textMuted }}>to</span>
        <input type="date" value={dateRange.to} onChange={(e) => setDateRange({ ...dateRange, to: e.target.value })} className="px-3 py-2 rounded-lg outline-none" style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }} />
      </div>

      {/* Tabs */}
      <div className="flex gap-2 mb-6" style={{ borderBottom: `1px solid ${styles.cardBorder}` }}>
        {tabs.map(tab => (
          <button key={tab.id} onClick={() => setActiveTab(tab.id)}
            className="flex items-center gap-2 px-4 py-2 border-b-2 transition-colors"
            style={{ 
              borderColor: activeTab === tab.id ? '#0078d4' : 'transparent', 
              color: activeTab === tab.id ? '#0078d4' : styles.textMuted 
            }}>
            <tab.icon size={18} /> {tab.label}
          </button>
        ))}
      </div>

      {/* Sales Report */}
      {activeTab === 'sales' && (
        <div className="space-y-6">
          {/* Summary Cards */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-green-500/20 rounded-lg flex items-center justify-center"><DollarSign size={20} className="text-green-400" /></div>
                <div>
                  <p className="text-2xl font-bold" style={{ color: styles.textMain }}>${salesReport?.totalRevenue?.toFixed(2) || '0.00'}</p>
                  <p className="text-sm" style={{ color: styles.textMuted }}>Total Revenue</p>
                </div>
              </div>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-blue-500/20 rounded-lg flex items-center justify-center"><FileText size={20} className="text-blue-400" /></div>
                <div>
                  <p className="text-2xl font-bold" style={{ color: styles.textMain }}>{salesReport?.totalOrders || 0}</p>
                  <p className="text-sm" style={{ color: styles.textMuted }}>Total Orders</p>
                </div>
              </div>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-purple-500/20 rounded-lg flex items-center justify-center"><TrendingUp size={20} className="text-purple-400" /></div>
                <div>
                  <p className="text-2xl font-bold" style={{ color: styles.textMain }}>${salesReport?.averageTicket?.toFixed(2) || '0.00'}</p>
                  <p className="text-sm" style={{ color: styles.textMuted }}>Avg. Ticket</p>
                </div>
              </div>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-orange-500/20 rounded-lg flex items-center justify-center"><Users size={20} className="text-orange-400" /></div>
                <div>
                  <p className="text-2xl font-bold" style={{ color: styles.textMain }}>{salesReport?.uniqueCustomers || 0}</p>
                  <p className="text-sm" style={{ color: styles.textMuted }}>Unique Customers</p>
                </div>
              </div>
            </div>
          </div>

          {/* Sales by Category */}
          <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
            <h3 className="font-semibold mb-4" style={{ color: styles.textMain }}>Sales by Category</h3>
            <table className="table">
              <thead style={{ background: 'rgba(255,255,255,0.05)' }}>
                <tr>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Category</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Items Sold</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Revenue</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>% of Total</th>
                </tr>
              </thead>
              <tbody>
                {salesReport?.byCategory?.map((cat: any, idx: number) => (
                  <tr key={idx} style={{ borderTop: `1px solid ${styles.cardBorder}` }}>
                    <td className="p-3 font-medium" style={{ color: styles.textMain }}>{cat.name}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>{cat.quantity}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>${cat.revenue?.toFixed(2)}</td>
                    <td className="p-3">
                      <div className="flex items-center gap-2">
                        <div className="w-20 h-2 rounded-full overflow-hidden" style={{ background: 'rgba(255,255,255,0.1)' }}>
                          <div className="h-full bg-blue-500" style={{ width: `${cat.percentage}%` }}></div>
                        </div>
                        <span className="text-sm" style={{ color: styles.textMuted }}>{cat.percentage?.toFixed(1)}%</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!salesReport?.byCategory || salesReport.byCategory.length === 0) && <p className="text-center py-4" style={{ color: styles.textMuted }}>No data available</p>}
          </div>

          {/* Top Selling Items */}
          <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
            <h3 className="font-semibold mb-4" style={{ color: styles.textMain }}>Top Selling Items</h3>
            <table className="table">
              <thead style={{ background: 'rgba(255,255,255,0.05)' }}>
                <tr>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>#</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Item</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Quantity</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Revenue</th>
                </tr>
              </thead>
              <tbody>
                {salesReport?.topItems?.map((item: any, idx: number) => (
                  <tr key={idx} style={{ borderTop: `1px solid ${styles.cardBorder}` }}>
                    <td className="p-3 font-medium" style={{ color: styles.textMain }}>{idx + 1}</td>
                    <td className="p-3" style={{ color: styles.textMain }}>{item.name}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>{item.quantity}</td>
                    <td className="p-3 font-medium" style={{ color: styles.textMain }}>${item.revenue?.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!salesReport?.topItems || salesReport.topItems.length === 0) && <p className="text-center py-4" style={{ color: styles.textMuted }}>No data available</p>}
          </div>
        </div>
      )}

      {/* Inventory Report */}
      {activeTab === 'inventory' && (
        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <p className="text-2xl font-bold" style={{ color: styles.textMain }}>{inventoryReport?.totalItems || 0}</p>
              <p className="text-sm" style={{ color: styles.textMuted }}>Total Items</p>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <p className="text-2xl font-bold text-red-400">{inventoryReport?.lowStockItems || 0}</p>
              <p className="text-sm" style={{ color: styles.textMuted }}>Low Stock Items</p>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <p className="text-2xl font-bold" style={{ color: styles.textMain }}>${inventoryReport?.totalValue?.toFixed(2) || '0.00'}</p>
              <p className="text-sm" style={{ color: styles.textMuted }}>Total Inventory Value</p>
            </div>
          </div>

          <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
            <h3 className="font-semibold mb-4" style={{ color: styles.textMain }}>Stock Status</h3>
            <table className="table">
              <thead style={{ background: 'rgba(255,255,255,0.05)' }}>
                <tr>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Item</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Current Qty</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Min Level</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Unit Cost</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Total Value</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Status</th>
                </tr>
              </thead>
              <tbody>
                {inventoryReport?.items?.map((item: any) => (
                  <tr key={item.id} style={{ borderTop: `1px solid ${styles.cardBorder}` }}>
                    <td className="p-3 font-medium" style={{ color: styles.textMain }}>{item.name}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>{item.quantity} {item.unit}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>{item.minLevel} {item.unit}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>${item.cost?.toFixed(2)}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>${item.totalValue?.toFixed(2)}</td>
                    <td className="p-3">
                      <span className={`px-2 py-1 rounded text-xs ${item.quantity <= item.minLevel ? 'bg-red-500/20 text-red-400' : 'bg-green-500/20 text-green-400'}`}>
                        {item.quantity <= item.minLevel ? 'Low Stock' : 'OK'}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!inventoryReport?.items || inventoryReport.items.length === 0) && <p className="text-center py-4" style={{ color: styles.textMuted }}>No data available</p>}
          </div>
        </div>
      )}

      {/* Customer Report */}
      {activeTab === 'customers' && (
        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <p className="text-2xl font-bold" style={{ color: styles.textMain }}>{customerReport?.totalCustomers || 0}</p>
              <p className="text-sm" style={{ color: styles.textMuted }}>Total Customers</p>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <p className="text-2xl font-bold text-green-400">{customerReport?.newCustomers || 0}</p>
              <p className="text-sm" style={{ color: styles.textMuted }}>New This Period</p>
            </div>
            <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
              <p className="text-2xl font-bold" style={{ color: styles.textMain }}>{customerReport?.repeatRate?.toFixed(1) || 0}%</p>
              <p className="text-sm" style={{ color: styles.textMuted }}>Repeat Rate</p>
            </div>
          </div>

          <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
            <h3 className="font-semibold mb-4" style={{ color: styles.textMain }}>Top Customers</h3>
            <table className="table">
              <thead style={{ background: 'rgba(255,255,255,0.05)' }}>
                <tr>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>#</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Customer</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Orders</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Total Spent</th>
                  <th className="text-left p-3" style={{ color: styles.textMuted }}>Last Visit</th>
                </tr>
              </thead>
              <tbody>
                {customerReport?.topCustomers?.map((cust: any, idx: number) => (
                  <tr key={cust.id} style={{ borderTop: `1px solid ${styles.cardBorder}` }}>
                    <td className="p-3 font-medium" style={{ color: styles.textMain }}>{idx + 1}</td>
                    <td className="p-3" style={{ color: styles.textMain }}>{cust.name}</td>
                    <td className="p-3" style={{ color: styles.textMuted }}>{cust.orderCount}</td>
                    <td className="p-3 font-medium" style={{ color: styles.textMain }}>${cust.totalSpent?.toFixed(2)}</td>
                    <td className="p-3 text-sm" style={{ color: styles.textMuted }}>{cust.lastVisit ? new Date(cust.lastVisit).toLocaleDateString() : '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {(!customerReport?.topCustomers || customerReport.topCustomers.length === 0) && <p className="text-center py-4" style={{ color: styles.textMuted }}>No data available</p>}
          </div>
        </div>
      )}
    </div>
  )
}
