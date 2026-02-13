import { useState, useEffect, useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { printReceipt, ReceiptData } from '../components/ReceiptPrint'
import { useAuth } from '../contexts/AuthContext'
import {
  FileText, Search, Filter, ChevronLeft, ChevronRight,
  X, Printer, Ban, Eye, Calendar, TrendingUp,
  DollarSign, ShoppingBag, Download
} from 'lucide-react'

const styles = {
  cardBg: 'rgba(255, 255, 255, 0.05)',
  cardBorder: 'rgba(255, 255, 255, 0.08)',
  textMain: '#ffffff',
  textMuted: '#a0a0a0',
  inputBg: 'rgba(255, 255, 255, 0.06)',
  success: '#63d9a0',
  danger: '#f28b8b',
  warning: '#f5c542',
  info: '#4da6e8',
}

interface OrderSummary {
  orderId: number
  orderNumber: string
  orderType: string
  orderStatus: string
  branchId: number
  branchName: string
  tableId?: number
  tableName?: string
  customerId?: number
  customerName?: string
  waiterName?: string
  subTotal: number
  grandTotal: number
  paymentStatus: string
  createdAt: string
}

interface OrderDetail {
  orderId: number
  orderNumber: string
  orderType: string
  orderStatus: string
  branchId: number
  branchName: string
  tableId?: number
  tableName?: string
  customerId?: number
  customerName?: string
  customerPhone?: string
  waiterName?: string
  cashierName?: string
  currencyCode: string
  subTotal: number
  totalLineDiscount: number
  billDiscountPercent: number
  billDiscountAmount: number
  serviceChargePercent: number
  serviceChargeAmount: number
  taxPercent: number
  taxAmount: number
  deliveryFee: number
  tipsAmount: number
  grandTotal: number
  totalPaid: number
  balanceDue: number
  paymentStatus: string
  loyaltyPointsEarned: number
  loyaltyPointsRedeemed: number
  notes?: string
  createdAt: string
  paidAt?: string
  voidedAt?: string
  voidReason?: string
  voidByName?: string
  lines: {
    orderLineId: number
    menuItemId: number
    menuItemName: string
    menuItemSizeId?: number
    sizeName?: string
    quantity: number
    baseUnitPrice: number
    modifiersExtraPrice: number
    effectiveUnitPrice: number
    lineGross: number
    discountPercent: number
    discountAmount: number
    lineNet: number
    notes?: string
    kitchenStatus: string
    modifiers: {
      modifierId: number
      modifierName: string
      quantity: number
      extraPrice: number
      totalPrice: number
    }[]
  }[]
  payments: {
    orderPaymentId: number
    paymentMethodId: number
    paymentMethodName: string
    amount: number
    currencyCode: string
    reference?: string
    createdAt: string
  }[]
  deliveryDetails?: {
    addressLine: string
    city: string
    area: string
    phone: string
    driverName?: string
    deliveryFeeCalculated: number
  }
}

export default function OrdersHistory() {
  const { t } = useTranslation()
  const { user } = useAuth()

  // Filters
  const [searchQuery, setSearchQuery] = useState('')
  const [branchFilter, setBranchFilter] = useState('')
  const [typeFilter, setTypeFilter] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [showVoided, setShowVoided] = useState(false)
  const [dateFrom, setDateFrom] = useState(() => {
    const d = new Date()
    d.setDate(d.getDate() - 30)
    return d.toISOString().split('T')[0]
  })
  const [dateTo, setDateTo] = useState(() => new Date().toISOString().split('T')[0])

  // Data
  const [orders, setOrders] = useState<OrderSummary[]>([])
  const [branches, setBranches] = useState<{ id: number; name: string }[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [pageSize] = useState(25)
  const [loading, setLoading] = useState(true)

  // Detail modal
  const [selectedOrder, setSelectedOrder] = useState<OrderDetail | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)

  // Void modal
  const [showVoidModal, setShowVoidModal] = useState(false)
  const [voidReason, setVoidReason] = useState('')
  const [voiding, setVoiding] = useState(false)

  useEffect(() => {
    api.get('/api/company/branches').then(r => {
      const data = Array.isArray(r.data) ? r.data : r.data.branches || []
      setBranches(data)
    }).catch(() => {})
  }, [])

  useEffect(() => {
    fetchOrders()
  }, [page, branchFilter, typeFilter, statusFilter, showVoided, dateFrom, dateTo])

  const fetchOrders = async () => {
    setLoading(true)
    try {
      const params: any = { page, pageSize }
      if (branchFilter) params.branchId = branchFilter
      if (typeFilter) params.orderType = typeFilter
      if (statusFilter) params.status = statusFilter
      else if (!showVoided) params.status = 'notVoided'
      if (dateFrom) params.fromDate = dateFrom + 'T00:00:00'
      if (dateTo) params.toDate = dateTo + 'T23:59:59'

      const res = await api.get('/api/company/orders', { params })
      setOrders(res.data.orders || [])
      setTotal(res.data.total || 0)
    } catch {
      setOrders([])
      setTotal(0)
    } finally {
      setLoading(false)
    }
  }

  const filteredOrders = useMemo(() => {
    if (!searchQuery.trim()) return orders
    const q = searchQuery.toLowerCase()
    return orders.filter(o =>
      o.orderNumber.toLowerCase().includes(q) ||
      (o.customerName && o.customerName.toLowerCase().includes(q)) ||
      (o.waiterName && o.waiterName.toLowerCase().includes(q)) ||
      (o.tableName && o.tableName.toLowerCase().includes(q))
    )
  }, [orders, searchQuery])

  // Summary stats for current filtered data
  const summaryStats = useMemo(() => {
    const nonVoided = orders.filter(o => o.orderStatus !== 'Voided')
    const paidOrders = nonVoided.filter(o => o.paymentStatus === 'Paid')
    return {
      totalRevenue: paidOrders.reduce((s, o) => s + o.grandTotal, 0),
      totalOrders: total,
      paidCount: paidOrders.length,
      avgTicket: paidOrders.length > 0
        ? paidOrders.reduce((s, o) => s + o.grandTotal, 0) / paidOrders.length
        : 0
    }
  }, [orders, total])

  const totalPages = Math.ceil(total / pageSize)

  const openDetail = async (orderId: number) => {
    setDetailLoading(true)
    try {
      const res = await api.get(`/api/company/orders/${orderId}`)
      setSelectedOrder(res.data)
    } catch {
      alert('Failed to load order details')
    } finally {
      setDetailLoading(false)
    }
  }

  const handleVoid = async () => {
    if (!selectedOrder || !voidReason.trim()) return
    setVoiding(true)
    try {
      await api.post(`/api/company/orders/${selectedOrder.orderId}/void`, { reason: voidReason })
      setShowVoidModal(false)
      setVoidReason('')
      setSelectedOrder(null)
      fetchOrders()
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to void order')
    } finally {
      setVoiding(false)
    }
  }

  const handleReprint = () => {
    if (!selectedOrder) return
    const receipt: ReceiptData = {
      orderNumber: selectedOrder.orderNumber,
      orderType: selectedOrder.orderType,
      branchName: selectedOrder.branchName,
      tableName: selectedOrder.tableName,
      customerName: selectedOrder.customerName,
      lines: selectedOrder.lines.map(l => ({
        name: l.menuItemName,
        sizeName: l.sizeName,
        quantity: l.quantity,
        effectivePrice: l.effectiveUnitPrice,
        lineNet: l.lineNet,
        discountPercent: l.discountPercent,
        modifiers: l.modifiers.map(m => ({
          name: m.modifierName,
          quantity: m.quantity,
          price: m.extraPrice
        })),
        notes: l.notes
      })),
      subtotal: selectedOrder.subTotal,
      totalLineDiscount: selectedOrder.totalLineDiscount,
      billDiscountPercent: selectedOrder.billDiscountPercent,
      billDiscountAmount: selectedOrder.billDiscountAmount,
      serviceChargePercent: selectedOrder.serviceChargePercent,
      serviceChargeAmount: selectedOrder.serviceChargeAmount,
      vatPercent: selectedOrder.taxPercent,
      vatAmount: selectedOrder.taxAmount,
      grandTotal: selectedOrder.grandTotal,
      paymentMethod: selectedOrder.payments.map(p => p.paymentMethodName).join(', ') || '-',
      companyName: user?.companyName || '',
      dateTime: new Date(selectedOrder.createdAt)
    }
    printReceipt(receipt, t)
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Paid': return { bg: 'rgba(99,217,160,0.15)', text: styles.success }
      case 'Voided': return { bg: 'rgba(242,139,139,0.15)', text: styles.danger }
      case 'Draft': return { bg: 'rgba(160,160,160,0.15)', text: styles.textMuted }
      case 'SentToKitchen': return { bg: 'rgba(77,166,232,0.15)', text: styles.info }
      default: return { bg: 'rgba(245,197,66,0.15)', text: styles.warning }
    }
  }

  const getPaymentColor = (status: string) => {
    switch (status) {
      case 'Paid': return { bg: 'rgba(99,217,160,0.15)', text: styles.success }
      case 'PartiallyPaid': return { bg: 'rgba(245,197,66,0.15)', text: styles.warning }
      case 'Unpaid': return { bg: 'rgba(242,139,139,0.15)', text: styles.danger }
      default: return { bg: 'rgba(160,160,160,0.15)', text: styles.textMuted }
    }
  }

  const money = (n: number) => `$${(n || 0).toFixed(2)}`

  const handleExportCSV = () => {
    if (!orders.length) return
    const headers = ['Order #', 'Date', 'Branch', 'Type', 'Customer', 'Status', 'Payment', 'Total']
    const rows = orders.map(o => [
      o.orderNumber,
      new Date(o.createdAt).toLocaleString(),
      o.branchName,
      o.orderType,
      o.customerName || '-',
      o.orderStatus,
      o.paymentStatus,
      o.grandTotal.toFixed(2)
    ])
    const csv = [headers, ...rows].map(r => r.join(',')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `orders_${dateFrom}_${dateTo}.csv`
    a.click()
    URL.revokeObjectURL(url)
  }

  return (
    <div className="p-6" style={{ color: styles.textMain }}>
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold flex items-center gap-2">
            <FileText size={28} /> {t('ordersHistory.title')}
          </h1>
          <p className="text-sm mt-1" style={{ color: styles.textMuted }}>{t('ordersHistory.description')}</p>
        </div>
        <button
          onClick={handleExportCSV}
          className="px-4 py-2 rounded-lg flex items-center gap-2 transition-all hover:-translate-y-0.5"
          style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
        >
          <Download size={18} /> CSV
        </button>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
        <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg flex items-center justify-center" style={{ background: 'rgba(99,217,160,0.15)' }}>
              <DollarSign size={20} style={{ color: styles.success }} />
            </div>
            <div>
              <p className="text-xl font-bold">{money(summaryStats.totalRevenue)}</p>
              <p className="text-xs" style={{ color: styles.textMuted }}>{t('ordersHistory.totalRevenue')}</p>
            </div>
          </div>
        </div>
        <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg flex items-center justify-center" style={{ background: 'rgba(77,166,232,0.15)' }}>
              <ShoppingBag size={20} style={{ color: styles.info }} />
            </div>
            <div>
              <p className="text-xl font-bold">{summaryStats.totalOrders}</p>
              <p className="text-xs" style={{ color: styles.textMuted }}>{t('ordersHistory.totalOrders')}</p>
            </div>
          </div>
        </div>
        <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg flex items-center justify-center" style={{ background: 'rgba(245,197,66,0.15)' }}>
              <TrendingUp size={20} style={{ color: styles.warning }} />
            </div>
            <div>
              <p className="text-xl font-bold">{money(summaryStats.avgTicket)}</p>
              <p className="text-xs" style={{ color: styles.textMuted }}>{t('ordersHistory.avgTicket')}</p>
            </div>
          </div>
        </div>
        <div className="p-4 rounded-xl" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg flex items-center justify-center" style={{ background: 'rgba(99,217,160,0.08)' }}>
              <FileText size={20} style={{ color: styles.success }} />
            </div>
            <div>
              <p className="text-xl font-bold">{summaryStats.paidCount}</p>
              <p className="text-xs" style={{ color: styles.textMuted }}>{t('ordersHistory.paidOrders')}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Filters */}
      <div className="p-4 rounded-xl mb-5" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
        <div className="flex items-center gap-2 mb-3">
          <Filter size={16} style={{ color: styles.textMuted }} />
          <span className="text-sm font-semibold">{t('common.filter')}</span>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-7 gap-3">
          {/* Search */}
          <div className="relative col-span-2 md:col-span-1">
            <Search size={15} className="absolute top-1/2 -translate-y-1/2 ltr:left-3 rtl:right-3" style={{ color: styles.textMuted }} />
            <input
              type="text"
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
              placeholder={t('ordersHistory.searchPlaceholder')}
              className="w-full rounded-lg py-2 ltr:pl-9 rtl:pr-9 ltr:pr-3 rtl:pl-3 text-sm outline-none"
              style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
            />
          </div>

          {/* Branch */}
          <select
            value={branchFilter}
            onChange={e => { setBranchFilter(e.target.value); setPage(1) }}
            className="rounded-lg py-2 px-3 text-sm outline-none"
            style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
          >
            <option value="">{t('ordersHistory.allBranches')}</option>
            {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
          </select>

          {/* Type */}
          <select
            value={typeFilter}
            onChange={e => { setTypeFilter(e.target.value); setPage(1) }}
            className="rounded-lg py-2 px-3 text-sm outline-none"
            style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
          >
            <option value="">{t('ordersHistory.allTypes')}</option>
            <option value="DineIn">{t('ordersHistory.dineIn')}</option>
            <option value="Takeaway">{t('ordersHistory.takeaway')}</option>
            <option value="Delivery">{t('ordersHistory.delivery')}</option>
          </select>

          {/* Status */}
          <select
            value={statusFilter}
            onChange={e => { setStatusFilter(e.target.value); setPage(1) }}
            className="rounded-lg py-2 px-3 text-sm outline-none"
            style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
          >
            <option value="">{t('ordersHistory.allStatuses')}</option>
            <option value="Draft">{t('ordersHistory.draft')}</option>
            <option value="SentToKitchen">{t('ordersHistory.sentToKitchen')}</option>
            <option value="Paid">{t('ordersHistory.paid')}</option>
            <option value="Voided">{t('ordersHistory.voided')}</option>
          </select>

          {/* Date from */}
          <div className="flex items-center gap-2">
            <Calendar size={14} style={{ color: styles.textMuted, flexShrink: 0 }} />
            <input
              type="date"
              value={dateFrom}
              onChange={e => { setDateFrom(e.target.value); setPage(1) }}
              className="w-full rounded-lg py-2 px-2 text-sm outline-none"
              style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
            />
          </div>

          {/* Date to */}
          <div className="flex items-center gap-2">
            <span className="text-xs" style={{ color: styles.textMuted, flexShrink: 0 }}>{t('ordersHistory.to')}</span>
            <input
              type="date"
              value={dateTo}
              onChange={e => { setDateTo(e.target.value); setPage(1) }}
              className="w-full rounded-lg py-2 px-2 text-sm outline-none"
              style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
            />
          </div>
          {/* Show Voided toggle */}
          <label className="flex items-center gap-2 cursor-pointer whitespace-nowrap">
            <input
              type="checkbox"
              checked={showVoided}
              onChange={e => { setShowVoided(e.target.checked); setPage(1) }}
              className="w-4 h-4 rounded accent-red-500"
            />
            <span className="text-xs" style={{ color: styles.danger }}>{t('ordersHistory.showVoided')}</span>
          </label>
        </div>
      </div>

      {/* Orders Table */}
      <div className="rounded-xl overflow-hidden" style={{ background: styles.cardBg, border: `1px solid ${styles.cardBorder}` }}>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr style={{ borderBottom: `1px solid ${styles.cardBorder}` }}>
                <th className="text-left p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.orderNumber')}</th>
                <th className="text-left p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.date')}</th>
                <th className="text-left p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.branch')}</th>
                <th className="text-left p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.type')}</th>
                <th className="text-left p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.customer')}</th>
                <th className="text-left p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.table')}</th>
                <th className="text-right p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.grandTotal')}</th>
                <th className="text-center p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.orderStatus')}</th>
                <th className="text-center p-3 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.paymentStatus')}</th>
                <th className="text-center p-3 font-semibold" style={{ color: styles.textMuted }}>{t('common.actions')}</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={10} className="text-center py-12" style={{ color: styles.textMuted }}>{t('common.loading')}</td></tr>
              ) : filteredOrders.length === 0 ? (
                <tr><td colSpan={10} className="text-center py-12" style={{ color: styles.textMuted }}>{t('ordersHistory.noOrders')}</td></tr>
              ) : (
                filteredOrders.map(order => {
                  const sc = getStatusColor(order.orderStatus)
                  const pc = getPaymentColor(order.paymentStatus)
                  return (
                    <tr
                      key={order.orderId}
                      className="cursor-pointer transition-colors"
                      style={{ borderBottom: `1px solid ${styles.cardBorder}` }}
                      onMouseEnter={e => (e.currentTarget.style.background = 'rgba(255,255,255,0.03)')}
                      onMouseLeave={e => (e.currentTarget.style.background = 'transparent')}
                      onClick={() => openDetail(order.orderId)}
                    >
                      <td className="p-3 font-semibold" style={{ color: styles.info }}>#{order.orderNumber}</td>
                      <td className="p-3 whitespace-nowrap">{new Date(order.createdAt).toLocaleDateString()} <span style={{ color: styles.textMuted }}>{new Date(order.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span></td>
                      <td className="p-3">{order.branchName}</td>
                      <td className="p-3">{order.orderType}</td>
                      <td className="p-3">{order.customerName || <span style={{ color: styles.textMuted }}>-</span>}</td>
                      <td className="p-3">{order.tableName || <span style={{ color: styles.textMuted }}>-</span>}</td>
                      <td className="p-3 text-right font-semibold">{money(order.grandTotal)}</td>
                      <td className="p-3 text-center">
                        <span className="px-2 py-1 rounded-md text-xs font-medium" style={{ background: sc.bg, color: sc.text }}>{order.orderStatus}</span>
                      </td>
                      <td className="p-3 text-center">
                        <span className="px-2 py-1 rounded-md text-xs font-medium" style={{ background: pc.bg, color: pc.text }}>{order.paymentStatus}</span>
                      </td>
                      <td className="p-3 text-center">
                        <button
                          onClick={e => { e.stopPropagation(); openDetail(order.orderId) }}
                          className="p-1.5 rounded-lg transition-colors"
                          style={{ background: 'rgba(77,166,232,0.1)' }}
                          title="View"
                        >
                          <Eye size={15} style={{ color: styles.info }} />
                        </button>
                      </td>
                    </tr>
                  )
                })
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex items-center justify-between px-4 py-3" style={{ borderTop: `1px solid ${styles.cardBorder}` }}>
            <span className="text-xs" style={{ color: styles.textMuted }}>
              {t('ordersHistory.showing')} {((page - 1) * pageSize) + 1}-{Math.min(page * pageSize, total)} {t('ordersHistory.of')} {total} {t('ordersHistory.orders')}
            </span>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page <= 1}
                className="p-1.5 rounded-lg transition-colors disabled:opacity-30"
                style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}` }}
              >
                <ChevronLeft size={16} />
              </button>
              <span className="text-xs px-2" style={{ color: styles.textMuted }}>
                {t('ordersHistory.page')} {page} {t('ordersHistory.of')} {totalPages}
              </span>
              <button
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page >= totalPages}
                className="p-1.5 rounded-lg transition-colors disabled:opacity-30"
                style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}` }}
              >
                <ChevronRight size={16} />
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Loading overlay for detail */}
      {detailLoading && (
        <div className="fixed inset-0 flex items-center justify-center z-[9998]" style={{ background: 'rgba(0,0,0,0.5)' }}>
          <div className="w-8 h-8 border-3 border-t-blue-500 border-white/20 rounded-full animate-spin" />
        </div>
      )}

      {/* Order Detail Modal */}
      {selectedOrder && !detailLoading && (
        <div className="fixed inset-0 flex items-center justify-center z-[9999] p-4" style={{ background: 'rgba(0,0,0,0.6)', backdropFilter: 'blur(6px)' }}>
          <div className="w-full max-w-[700px] rounded-2xl overflow-hidden flex flex-col max-h-[90vh]" style={{ background: '#111419', border: `1px solid ${styles.cardBorder}`, boxShadow: '0 24px 48px rgba(0,0,0,0.5)' }}>
            {/* Modal Header */}
            <div className="flex items-center justify-between px-5 py-3 shrink-0" style={{ borderBottom: `1px solid ${styles.cardBorder}` }}>
              <div className="flex items-center gap-2">
                <FileText size={18} style={{ color: styles.info }} />
                <span className="font-semibold text-sm">{t('ordersHistory.orderDetails')}</span>
                <span className="font-bold" style={{ color: styles.info }}>#{selectedOrder.orderNumber}</span>
              </div>
              <button onClick={() => setSelectedOrder(null)} className="w-8 h-8 rounded-lg flex items-center justify-center" style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.cardBorder}` }}>
                <X size={14} />
              </button>
            </div>

            {/* Modal Body */}
            <div className="flex-1 overflow-y-auto p-5 space-y-5">
              {/* Order Info Grid */}
              <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                {[
                  { label: t('ordersHistory.date'), value: new Date(selectedOrder.createdAt).toLocaleString() },
                  { label: t('ordersHistory.type'), value: selectedOrder.orderType },
                  { label: t('ordersHistory.branch'), value: selectedOrder.branchName },
                  { label: t('ordersHistory.table'), value: selectedOrder.tableName || '-' },
                  { label: t('ordersHistory.customer'), value: selectedOrder.customerName || '-' },
                  { label: t('ordersHistory.waiter'), value: selectedOrder.waiterName || '-' },
                  { label: t('ordersHistory.cashier'), value: selectedOrder.cashierName || '-' },
                  { label: t('ordersHistory.orderStatus'), value: selectedOrder.orderStatus },
                  { label: t('ordersHistory.paymentStatus'), value: selectedOrder.paymentStatus },
                ].map((item, i) => (
                  <div key={i} className="p-2.5 rounded-lg" style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}>
                    <p className="text-[10px] uppercase tracking-wider mb-0.5" style={{ color: styles.textMuted }}>{item.label}</p>
                    <p className="text-sm font-medium">{item.value}</p>
                  </div>
                ))}
              </div>

              {/* Order Lines */}
              <div>
                <h3 className="text-xs font-bold uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('ordersHistory.orderLines')}</h3>
                <div className="rounded-lg overflow-hidden" style={{ border: `1px solid ${styles.cardBorder}` }}>
                  <table className="w-full text-xs">
                    <thead>
                      <tr style={{ background: 'rgba(255,255,255,0.03)', borderBottom: `1px solid ${styles.cardBorder}` }}>
                        <th className="text-left p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.itemName')}</th>
                        <th className="text-center p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.qty')}</th>
                        <th className="text-right p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.unitPrice')}</th>
                        <th className="text-right p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.discount')}</th>
                        <th className="text-right p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.lineTotal')}</th>
                      </tr>
                    </thead>
                    <tbody>
                      {selectedOrder.lines.map(line => (
                        <tr key={line.orderLineId} style={{ borderBottom: `1px solid ${styles.cardBorder}` }}>
                          <td className="p-2.5">
                            <span className="font-medium">{line.menuItemName}</span>
                            {line.sizeName && <span className="text-[10px] ml-1" style={{ color: styles.textMuted }}>({line.sizeName})</span>}
                            {line.modifiers.length > 0 && (
                              <div className="text-[10px] mt-0.5" style={{ color: styles.textMuted }}>
                                + {line.modifiers.map(m => `${m.modifierName}${m.quantity > 1 ? ` x${m.quantity}` : ''}`).join(', ')}
                              </div>
                            )}
                            {line.notes && <div className="text-[10px] italic mt-0.5" style={{ color: styles.textMuted }}>{line.notes}</div>}
                          </td>
                          <td className="p-2.5 text-center">{line.quantity}</td>
                          <td className="p-2.5 text-right">{money(line.effectiveUnitPrice)}</td>
                          <td className="p-2.5 text-right" style={{ color: line.discountPercent > 0 ? styles.danger : styles.textMuted }}>
                            {line.discountPercent > 0 ? `-${line.discountPercent}%` : '-'}
                          </td>
                          <td className="p-2.5 text-right font-semibold">{money(line.lineNet)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* Totals Breakdown */}
              <div className="rounded-lg p-4" style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}>
                <div className="space-y-1.5 text-sm">
                  <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.subtotal')}</span><span>{money(selectedOrder.subTotal)}</span></div>
                  {selectedOrder.totalLineDiscount > 0 && (
                    <div className="flex justify-between" style={{ color: styles.danger }}><span>{t('ordersHistory.lineDiscounts')}</span><span>-{money(selectedOrder.totalLineDiscount)}</span></div>
                  )}
                  {selectedOrder.billDiscountPercent > 0 && (
                    <div className="flex justify-between" style={{ color: styles.danger }}><span>{t('ordersHistory.billDiscount')} ({selectedOrder.billDiscountPercent}%)</span><span>-{money(selectedOrder.billDiscountAmount)}</span></div>
                  )}
                  {selectedOrder.serviceChargeAmount > 0 && (
                    <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.serviceCharge')} ({selectedOrder.serviceChargePercent}%)</span><span>{money(selectedOrder.serviceChargeAmount)}</span></div>
                  )}
                  {selectedOrder.taxAmount > 0 && (
                    <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.vat')} ({selectedOrder.taxPercent}%)</span><span>{money(selectedOrder.taxAmount)}</span></div>
                  )}
                  {selectedOrder.deliveryFee > 0 && (
                    <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.deliveryFee')}</span><span>{money(selectedOrder.deliveryFee)}</span></div>
                  )}
                  {selectedOrder.tipsAmount > 0 && (
                    <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.tips')}</span><span>{money(selectedOrder.tipsAmount)}</span></div>
                  )}
                  <div className="flex justify-between font-bold text-base pt-2 mt-2" style={{ borderTop: `2px solid ${styles.cardBorder}` }}>
                    <span>{t('ordersHistory.grandTotal')}</span><span>{money(selectedOrder.grandTotal)}</span>
                  </div>
                  {selectedOrder.totalPaid > 0 && (
                    <div className="flex justify-between text-xs"><span style={{ color: styles.textMuted }}>{t('ordersHistory.totalPaid')}</span><span style={{ color: styles.success }}>{money(selectedOrder.totalPaid)}</span></div>
                  )}
                  {selectedOrder.balanceDue > 0.01 && (
                    <div className="flex justify-between text-xs"><span style={{ color: styles.textMuted }}>{t('ordersHistory.balanceDue')}</span><span style={{ color: styles.danger }}>{money(selectedOrder.balanceDue)}</span></div>
                  )}
                </div>
              </div>

              {/* Payments */}
              {selectedOrder.payments.length > 0 && (
                <div>
                  <h3 className="text-xs font-bold uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('ordersHistory.paymentDetails')}</h3>
                  <div className="rounded-lg overflow-hidden" style={{ border: `1px solid ${styles.cardBorder}` }}>
                    <table className="w-full text-xs">
                      <thead>
                        <tr style={{ background: 'rgba(255,255,255,0.03)', borderBottom: `1px solid ${styles.cardBorder}` }}>
                          <th className="text-left p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.method')}</th>
                          <th className="text-right p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.amount')}</th>
                          <th className="text-left p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.reference')}</th>
                          <th className="text-left p-2.5 font-semibold" style={{ color: styles.textMuted }}>{t('ordersHistory.date')}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {selectedOrder.payments.map(p => (
                          <tr key={p.orderPaymentId} style={{ borderBottom: `1px solid ${styles.cardBorder}` }}>
                            <td className="p-2.5 font-medium">{p.paymentMethodName}</td>
                            <td className="p-2.5 text-right font-semibold">{money(p.amount)}</td>
                            <td className="p-2.5" style={{ color: styles.textMuted }}>{p.reference || '-'}</td>
                            <td className="p-2.5" style={{ color: styles.textMuted }}>{new Date(p.createdAt).toLocaleString()}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              )}

              {/* Delivery Details */}
              {selectedOrder.deliveryDetails && (
                <div>
                  <h3 className="text-xs font-bold uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('ordersHistory.deliveryDetails')}</h3>
                  <div className="rounded-lg p-3 space-y-1.5 text-sm" style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}>
                    <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.address')}</span><span>{selectedOrder.deliveryDetails.addressLine}, {selectedOrder.deliveryDetails.area}, {selectedOrder.deliveryDetails.city}</span></div>
                    <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.phone')}</span><span>{selectedOrder.deliveryDetails.phone}</span></div>
                    {selectedOrder.deliveryDetails.driverName && (
                      <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.driver')}</span><span>{selectedOrder.deliveryDetails.driverName}</span></div>
                    )}
                  </div>
                </div>
              )}

              {/* Void Info */}
              {selectedOrder.orderStatus === 'Voided' && (
                <div className="rounded-lg p-3 text-sm" style={{ background: 'rgba(242,139,139,0.08)', border: `1px solid rgba(242,139,139,0.25)` }}>
                  <div className="flex items-center gap-2 mb-2">
                    <Ban size={14} style={{ color: styles.danger }} />
                    <span className="text-xs font-bold uppercase tracking-wider" style={{ color: styles.danger }}>{t('ordersHistory.voided')}</span>
                  </div>
                  <div className="space-y-1">
                    {selectedOrder.voidByName && (
                      <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.voidedBy')}</span><span className="font-semibold">{selectedOrder.voidByName}</span></div>
                    )}
                    {selectedOrder.voidedAt && (
                      <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.voidedAt')}</span><span>{new Date(selectedOrder.voidedAt).toLocaleString()}</span></div>
                    )}
                    {selectedOrder.voidReason && (
                      <div className="flex justify-between"><span style={{ color: styles.textMuted }}>{t('ordersHistory.voidReason')}</span><span>{selectedOrder.voidReason}</span></div>
                    )}
                  </div>
                </div>
              )}

              {/* Notes */}
              {selectedOrder.notes && (
                <div className="rounded-lg p-3 text-sm" style={{ background: 'rgba(255,255,255,0.03)', border: `1px solid ${styles.cardBorder}` }}>
                  <p className="text-[10px] uppercase tracking-wider mb-1" style={{ color: styles.textMuted }}>{t('ordersHistory.notes')}</p>
                  <p>{selectedOrder.notes}</p>
                </div>
              )}
            </div>

            {/* Modal Actions */}
            <div className="flex gap-2 px-5 py-3 shrink-0" style={{ borderTop: `1px solid ${styles.cardBorder}`, background: '#0e1116' }}>
              {selectedOrder.orderStatus === 'Paid' && (
                <button
                  onClick={handleReprint}
                  className="flex items-center gap-2 px-4 py-2 rounded-lg text-xs font-semibold transition-colors"
                  style={{ background: 'rgba(77,166,232,0.12)', border: '1px solid rgba(77,166,232,0.3)', color: styles.info }}
                >
                  <Printer size={15} /> {t('ordersHistory.reprintReceipt')}
                </button>
              )}
              {selectedOrder.orderStatus !== 'Voided' && (
                <button
                  onClick={() => setShowVoidModal(true)}
                  className="flex items-center gap-2 px-4 py-2 rounded-lg text-xs font-semibold transition-colors"
                  style={{ background: 'rgba(242,139,139,0.12)', border: '1px solid rgba(242,139,139,0.3)', color: styles.danger }}
                >
                  <Ban size={15} /> {t('ordersHistory.voidOrder')}
                </button>
              )}
              <div className="flex-1" />
              <button
                onClick={() => setSelectedOrder(null)}
                className="px-4 py-2 rounded-lg text-xs font-semibold"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.cardBorder}`, color: styles.textMuted }}
              >
                {t('common.close')}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Void Confirmation Modal */}
      {showVoidModal && selectedOrder && (
        <div className="fixed inset-0 flex items-center justify-center z-[10000] p-4" style={{ background: 'rgba(0,0,0,0.7)' }}>
          <div className="w-full max-w-[400px] rounded-xl p-5" style={{ background: '#111419', border: `1px solid ${styles.cardBorder}` }}>
            <h3 className="font-semibold text-base mb-3 flex items-center gap-2">
              <Ban size={18} style={{ color: styles.danger }} /> {t('ordersHistory.voidOrder')}
            </h3>
            <p className="text-sm mb-4" style={{ color: styles.textMuted }}>{t('ordersHistory.voidConfirm')}</p>
            <label className="block text-xs font-medium mb-1" style={{ color: styles.textMuted }}>{t('ordersHistory.voidReason')} *</label>
            <textarea
              value={voidReason}
              onChange={e => setVoidReason(e.target.value)}
              rows={3}
              className="w-full rounded-lg p-3 text-sm outline-none mb-4 resize-none"
              style={{ background: styles.inputBg, border: `1px solid ${styles.cardBorder}`, color: styles.textMain }}
              placeholder={t('ordersHistory.voidReason')}
            />
            <div className="flex gap-2 justify-end">
              <button
                onClick={() => { setShowVoidModal(false); setVoidReason('') }}
                className="px-4 py-2 rounded-lg text-xs font-semibold"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.cardBorder}`, color: styles.textMuted }}
              >
                {t('common.cancel')}
              </button>
              <button
                onClick={handleVoid}
                disabled={voiding || !voidReason.trim()}
                className="px-4 py-2 rounded-lg text-xs font-bold disabled:opacity-40"
                style={{ background: 'rgba(242,139,139,0.2)', border: '1px solid rgba(242,139,139,0.4)', color: styles.danger }}
              >
                {voiding ? t('common.loading') : t('ordersHistory.voidOrder')}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
