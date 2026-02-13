import { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { useAuth } from '../contexts/AuthContext'
import api from '../lib/api'
import { 
  ShoppingBasket, Plus, Minus, Trash2, Search, User, 
  UtensilsCrossed, Package, Truck, CreditCard, X,
  Percent, DollarSign, Clock, Tag, Home, LogOut
} from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import { printReceipt } from '../components/ReceiptPrint'

const API_URL = '/api'

interface Category {
  id: number
  name: string
  parentCategoryId: number | null
  isActive: boolean
  image?: string
}

interface MenuItem {
  id: number
  name: string
  code: string
  categoryId: number
  defaultPrice: number
  description: string
  isActive: boolean
  allowSizes: boolean
  imageUrl?: string
  sizes?: MenuItemSize[]
}

interface MenuItemSize {
  id: number
  sizeName: string
  price: number
}

interface Modifier {
  id: number
  name: string
  extraPrice: number
  isActive?: boolean
}

interface OrderLine {
  id: string
  menuItemId: number
  menuItemSizeId?: number
  name: string
  sizeName?: string
  quantity: number
  basePrice: number
  modifiersExtra: number
  effectivePrice: number
  lineTotal: number
  discountPercent: number
  discountAmount: number
  lineNet: number
  notes?: string
  modifiers: { modifierId: number; name: string; quantity: number; price: number }[]
}

interface Table {
  id: number
  tableName: string
  zone: string
  capacity: number
}

interface Customer {
  id: number
  name: string
  phone: string
}

interface PaymentMethod {
  id: number
  name: string
  type: string
  isActive?: boolean
}

interface Branch {
  id: number
  name: string
  vatPercent: number
  serviceChargePercent: number
}

// Dark theme styles — soft pro POS palette
const styles = {
  bgDark: '#151820',
  panelBg: '#1c2028',
  glass: 'rgba(255, 255, 255, 0.04)',
  glassBorder: 'rgba(255, 255, 255, 0.07)',
  accent: '#4da6e8',
  success: '#63d9a0',
  danger: '#f28b8b',
  warning: '#f0c850',
  textMain: '#e8eaed',
  textMuted: '#717a88',
}

interface Reservation {
  id: number
  customerName: string
  customerPhone: string
  reservationDate: string
  startTime: string
  partySize: number
  tableName?: string
  status: string
}

export default function POS() {
  const { t } = useTranslation()
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [categories, setCategories] = useState<Category[]>([])
  const [menuItems, setMenuItems] = useState<MenuItem[]>([])
  const [tables, setTables] = useState<Table[]>([])
  const [customers, setCustomers] = useState<Customer[]>([])
  const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([])
  const [branches, setBranches] = useState<Branch[]>([])
  const [modifiers, setModifiers] = useState<Modifier[]>([])
  const [upcomingReservations, setUpcomingReservations] = useState<Reservation[]>([])
  
  const [selectedBranch, setSelectedBranch] = useState<Branch | null>(null)
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null)
  const [orderType, setOrderType] = useState<'DineIn' | 'Takeaway' | 'Delivery'>('DineIn')
  const [selectedTable, setSelectedTable] = useState<number | null>(null)
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null)
  const [orderLines, setOrderLines] = useState<OrderLine[]>([])
  const [searchTerm, setSearchTerm] = useState('')
  const [billDiscount, setBillDiscount] = useState(0)
  
  const [showPaymentModal, setShowPaymentModal] = useState(false)
  const [showCustomerModal, setShowCustomerModal] = useState(false)
  const [showModifierModal, setShowModifierModal] = useState(false)
  const [selectedItemForModifier, setSelectedItemForModifier] = useState<MenuItem | null>(null)
  const [selectedModifiers, setSelectedModifiers] = useState<{modifierId: number; quantity: number}[]>([])
  const [selectedSize, setSelectedSize] = useState<number | null>(null)
  const [itemQuantity, setItemQuantity] = useState(1)
  const [itemNotes, setItemNotes] = useState('')
  
  const [showLineDiscountModal, setShowLineDiscountModal] = useState(false)
  const [selectedLineForDiscount, setSelectedLineForDiscount] = useState<string | null>(null)
  const [lineDiscountInput, setLineDiscountInput] = useState(0)

  const [loading, setLoading] = useState(true)
  const [processing, setProcessing] = useState(false)
  const [currentTime, setCurrentTime] = useState(new Date())



  useEffect(() => {
    fetchData()
    const timer = setInterval(() => setCurrentTime(new Date()), 1000)
    return () => clearInterval(timer)
  }, [])

  const fetchData = async () => {
    try {
      // Get date range for next 3 days
      const today = new Date()
      const endDate = new Date(today)
      endDate.setDate(today.getDate() + 3)
      const startDateStr = today.toISOString().split('T')[0]
      const endDateStr = endDate.toISOString().split('T')[0]

      const [catRes, itemsRes, tablesRes, custRes, pmRes, branchRes, modRes, reservationsRes] = await Promise.all([
        api.get(`${API_URL}/company/categories`),
        api.get(`${API_URL}/company/menu-items`),
        api.get(`${API_URL}/company/tables`),
        api.get(`${API_URL}/company/customers`),
        api.get(`${API_URL}/company/payment-methods`),
        api.get(`${API_URL}/company/branches`),
        api.get(`${API_URL}/company/modifiers`),
        api.get(`${API_URL}/company/reservations?startDate=${startDateStr}&endDate=${endDateStr}`)
      ])
      
      setCategories(catRes.data.filter((c: Category) => c.isActive))
      setMenuItems(itemsRes.data.filter((i: MenuItem) => i.isActive))
      setTables(tablesRes.data)
      setCustomers(custRes.data)
      setPaymentMethods(pmRes.data.filter((p: PaymentMethod) => p.isActive))
      setBranches(branchRes.data)
      setModifiers(modRes.data.filter((m: Modifier) => m.isActive))
      setUpcomingReservations(reservationsRes.data.filter((r: Reservation) => r.status !== 'Canceled' && r.status !== 'NoShow'))
      
      if (branchRes.data.length > 0) {
        setSelectedBranch(branchRes.data[0])
      }
      // Don't auto-select category - show categories grid first
    } catch (error) {
      console.error('Error fetching data:', error)
    } finally {
      setLoading(false)
    }
  }

  const filteredItems = menuItems.filter(item => {
    const matchesCategory = selectedCategory ? item.categoryId === selectedCategory : true
    const matchesSearch = searchTerm 
      ? item.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.code?.toLowerCase().includes(searchTerm.toLowerCase())
      : true
    return matchesCategory && matchesSearch
  })

  const addToOrder = (item: MenuItem, sizeId?: number, mods?: {modifierId: number; quantity: number}[], qty: number = 1, notes?: string) => {
    const size = sizeId ? item.sizes?.find(s => s.id === sizeId) : null
    const basePrice = size ? size.price : item.defaultPrice
    
    let modifiersExtra = 0
    const modifierDetails: OrderLine['modifiers'] = []
    
    if (mods && mods.length > 0) {
      mods.forEach(m => {
        const modifier = modifiers.find(mod => mod.id === m.modifierId)
        if (modifier) {
          const price = modifier.extraPrice * m.quantity
          modifiersExtra += price
          modifierDetails.push({
            modifierId: m.modifierId,
            name: modifier.name,
            quantity: m.quantity,
            price
          })
        }
      })
    }

    const effectivePrice = basePrice + modifiersExtra
    const lineTotal = effectivePrice * qty

    const newLine: OrderLine = {
      id: `${Date.now()}-${Math.random()}`,
      menuItemId: item.id,
      menuItemSizeId: sizeId,
      name: item.name,
      sizeName: size?.sizeName,
      quantity: qty,
      basePrice,
      modifiersExtra,
      effectivePrice,
      lineTotal,
      discountPercent: 0,
      discountAmount: 0,
      lineNet: lineTotal,
      notes,
      modifiers: modifierDetails
    }

    setOrderLines([...orderLines, newLine])
  }

  const handleItemClick = (item: MenuItem) => {
    setSelectedItemForModifier(item)
    setSelectedSize(item.allowSizes && item.sizes && item.sizes.length > 0 ? item.sizes[0].id : null)
    setSelectedModifiers([])
    setItemQuantity(1)
    setItemNotes('')
    setShowModifierModal(true)
  }

  const confirmAddItem = () => {
    if (selectedItemForModifier) {
      addToOrder(
        selectedItemForModifier, 
        selectedSize || undefined, 
        selectedModifiers, 
        itemQuantity,
        itemNotes || undefined
      )
      setShowModifierModal(false)
      setSelectedItemForModifier(null)
    }
  }

  const updateLineQuantity = (lineId: string, delta: number) => {
    setOrderLines(lines => 
      lines.map(line => {
        if (line.id === lineId) {
          const newQty = Math.max(1, line.quantity + delta)
          const lineTotal = line.effectivePrice * newQty
          const discountAmount = lineTotal * line.discountPercent / 100
          return { 
            ...line, 
            quantity: newQty,
            lineTotal,
            discountAmount,
            lineNet: lineTotal - discountAmount
          }
        }
        return line
      })
    )
  }

  const removeLine = (lineId: string) => {
    setOrderLines(lines => lines.filter(l => l.id !== lineId))
  }

  const openLineDiscount = (lineId: string) => {
    const line = orderLines.find(l => l.id === lineId)
    if (line) {
      setSelectedLineForDiscount(lineId)
      setLineDiscountInput(line.discountPercent)
      setShowLineDiscountModal(true)
    }
  }

  const applyLineDiscount = () => {
    if (!selectedLineForDiscount) return
    setOrderLines(lines =>
      lines.map(line => {
        if (line.id === selectedLineForDiscount) {
          const discountAmount = line.lineTotal * lineDiscountInput / 100
          return {
            ...line,
            discountPercent: lineDiscountInput,
            discountAmount,
            lineNet: line.lineTotal - discountAmount
          }
        }
        return line
      })
    )
    setShowLineDiscountModal(false)
    setSelectedLineForDiscount(null)
  }

  const subtotal = orderLines.reduce((sum, l) => sum + l.lineTotal, 0)
  const totalLineDiscount = orderLines.reduce((sum, l) => sum + l.discountAmount, 0)
  const netAfterLineDiscount = subtotal - totalLineDiscount
  const billDiscountAmount = netAfterLineDiscount * billDiscount / 100
  const netAfterBillDiscount = netAfterLineDiscount - billDiscountAmount
  const serviceCharge = selectedBranch ? netAfterBillDiscount * selectedBranch.serviceChargePercent / 100 : 0
  const netBeforeTax = netAfterBillDiscount + serviceCharge
  const tax = selectedBranch ? netBeforeTax * selectedBranch.vatPercent / 100 : 0
  const grandTotal = Math.round((netBeforeTax + tax) * 100) / 100

  const clearOrder = () => {
    setOrderLines([])
    setSelectedTable(null)
    setSelectedCustomer(null)
    setBillDiscount(0)
  }

  const processPayment = async (paymentMethodId: number | null, paymentMethodName: string) => {
    if (!selectedBranch || orderLines.length === 0) return

    setProcessing(true)

    try {
      const orderRes = await api.post(`${API_URL}/company/orders`, {
        branchId: selectedBranch.id,
        orderType: orderType,
        tableId: selectedTable,
        customerId: selectedCustomer?.id,
        notes: ''
      })

      const orderId = orderRes.data.orderId

      for (const line of orderLines) {
        await api.post(`${API_URL}/company/orders/${orderId}/lines`, {
          menuItemId: line.menuItemId,
          menuItemSizeId: line.menuItemSizeId,
          quantity: line.quantity,
          discountPercent: line.discountPercent,
          notes: line.notes,
          modifiers: line.modifiers.map(m => ({
            modifierId: m.modifierId,
            quantity: m.quantity
          }))
        })
      }

      if (billDiscount > 0) {
        await api.post(`${API_URL}/company/orders/${orderId}/discount`, {
          discountPercent: billDiscount
        })
      }

      await api.post(`${API_URL}/company/orders/${orderId}/pay`, {
        payments: [{
          paymentMethodId: paymentMethodId || 1,
          amount: grandTotal,
          currencyCode: 'USD'
        }]
      })

      // Build receipt data before clearing order
      const receipt = {
        orderNumber: orderRes.data.orderNumber || orderId.toString(),
        orderType: orderType === 'DineIn' ? t('pos.dineIn') : orderType === 'Takeaway' ? t('pos.takeaway') : t('pos.delivery'),
        branchName: selectedBranch.name,
        tableName: selectedTable ? tables.find(tb => tb.id === selectedTable)?.tableName : undefined,
        customerName: selectedCustomer?.name,
        lines: orderLines.map(l => ({
          name: l.name,
          sizeName: l.sizeName,
          quantity: l.quantity,
          effectivePrice: l.effectivePrice,
          lineNet: l.lineNet,
          discountPercent: l.discountPercent,
          modifiers: l.modifiers,
          notes: l.notes
        })),
        subtotal,
        totalLineDiscount,
        billDiscountPercent: billDiscount,
        billDiscountAmount,
        serviceChargePercent: selectedBranch.serviceChargePercent,
        serviceChargeAmount: serviceCharge,
        vatPercent: selectedBranch.vatPercent,
        vatAmount: tax,
        grandTotal,
        paymentMethod: paymentMethodName,
        companyName: user?.companyName || '',
        dateTime: new Date()
      }

      setShowPaymentModal(false)
      printReceipt(receipt, t)
      clearOrder()

    } catch (error: any) {
      console.error('Payment error:', error)
      alert(`Payment failed: ${error.response?.data?.message || error.message}`)
    } finally {
      setProcessing(false)
    }
  }

  const money = (n: number) => `$${n.toFixed(2)}`

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen" style={{ background: styles.bgDark }}>
        <div className="w-8 h-8 border-3 border-t-blue-500 border-white/20 rounded-full animate-spin"></div>
      </div>
    )
  }

  return (
    <div className="fixed inset-0 flex flex-col overflow-hidden" style={{ 
      background: styles.bgDark,
      color: styles.textMain,
      fontFamily: "'Inter', 'Segoe UI', system-ui, -apple-system, sans-serif",
      zIndex: 50
    }}>
      {/* Header */}
      <header className="h-[48px] flex items-center justify-between px-5 shrink-0 z-50" style={{
        background: '#111419',
        borderBottom: `1px solid ${styles.glassBorder}`
      }}>
        {/* Left: Nav + Brand */}
        <div className="flex items-center gap-4">
          <button 
            onClick={() => navigate('/')}
            className="w-8 h-8 rounded-lg flex items-center justify-center transition-colors"
            style={{ background: styles.glass, border: `1px solid ${styles.glassBorder}` }}
          >
            <Home size={16} style={{ color: styles.textMuted }} />
          </button>
          <div className="flex items-center gap-2">
            <ShoppingBasket size={20} style={{ color: styles.accent }} />
            <span className="text-sm font-semibold tracking-wide" style={{ color: styles.textMain }}>
              POS <span style={{ color: styles.accent }}>PRO</span>
            </span>
          </div>
          <div className="h-5 w-px mx-1" style={{ background: styles.glassBorder }} />
          <select 
            className="px-2.5 py-1.5 rounded-lg outline-none text-xs"
            style={{ 
              background: styles.glass, 
              border: `1px solid ${styles.glassBorder}`,
              color: styles.textMain
            }}
            value={selectedBranch?.id || ''}
            onChange={(e) => {
              const branch = branches.find(b => b.id === parseInt(e.target.value))
              setSelectedBranch(branch || null)
            }}
          >
            {branches.map(b => (
              <option key={b.id} value={b.id} style={{ background: '#111419' }}>
                {b.name}
              </option>
            ))}
          </select>
        </div>

        {/* Right: Info + Logout */}
        <div className="flex items-center gap-3 text-xs" style={{ color: styles.textMuted }}>
          <span>{t('pos.vat')} {selectedBranch?.vatPercent || 0}%</span>
          <span className="opacity-30">|</span>
          <span>{t('pos.serviceCharge')} {selectedBranch?.serviceChargePercent || 0}%</span>
          <div className="h-5 w-px mx-1" style={{ background: styles.glassBorder }} />
          <span className="flex items-center gap-1.5 font-medium" style={{ color: styles.textMain }}>
            <Clock size={13} /> {currentTime.toLocaleTimeString()}
          </span>
          <button 
            onClick={logout}
            className="w-8 h-8 rounded-lg flex items-center justify-center transition-colors hover:bg-red-500/10"
            style={{ border: `1px solid ${styles.glassBorder}` }}
            title={t('common.logout')}
          >
            <LogOut size={14} style={{ color: styles.danger }} />
          </button>
        </div>
      </header>

      <div className="flex flex-1 overflow-hidden">
        {/* Menu Section */}
        <main className="flex-1 flex flex-col overflow-hidden">
          {/* Controls Bar */}
          <div className="flex items-center gap-3 px-5 py-3 shrink-0" style={{ 
            background: '#1a1e26',
            borderBottom: `1px solid ${styles.glassBorder}` 
          }}>
            {/* Order Type Tabs */}
            <div className="flex rounded-lg overflow-hidden" style={{ border: `1px solid ${styles.glassBorder}` }}>
              {(['DineIn', 'Takeaway', 'Delivery'] as const).map(type => (
                <button
                  key={type}
                  onClick={() => setOrderType(type)}
                  className="flex items-center gap-1.5 px-3.5 py-2 text-xs font-semibold transition-all select-none"
                  style={{
                    background: orderType === type 
                      ? type === 'DineIn' ? 'rgba(77,166,232,0.15)' 
                      : type === 'Takeaway' ? 'rgba(240,200,80,0.15)' 
                      : 'rgba(99,217,160,0.15)'
                      : 'transparent',
                    color: orderType === type 
                      ? type === 'DineIn' ? styles.accent 
                      : type === 'Takeaway' ? styles.warning 
                      : styles.success
                      : styles.textMuted,
                    borderRight: `1px solid ${styles.glassBorder}`
                  }}
                >
                  {type === 'DineIn' && <UtensilsCrossed size={14} />}
                  {type === 'Takeaway' && <Package size={14} />}
                  {type === 'Delivery' && <Truck size={14} />}
                  {type === 'DineIn' ? t('pos.dineIn') : type === 'Takeaway' ? t('pos.takeaway') : t('pos.delivery')}
                </button>
              ))}
            </div>

            {orderType === 'DineIn' && (
              <select 
                className="px-3 py-2 rounded-lg outline-none text-xs"
                style={{ 
                  background: styles.glass, 
                  border: `1px solid ${styles.glassBorder}`,
                  color: styles.textMain
                }}
                value={selectedTable || ''}
                onChange={(e) => setSelectedTable(parseInt(e.target.value) || null)}
              >
                <option value="" style={{ background: '#1a1e26' }}>{t('pos.selectTable')}</option>
                {tables.map(t => (
                  <option key={t.id} value={t.id} style={{ background: '#1a1e26' }}>
                    {t.tableName} ({t.zone})
                  </option>
                ))}
              </select>
            )}

            <div className="flex-1" />

            {/* Search */}
            <div className="relative w-[240px]">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2" size={14} style={{ color: styles.textMuted }} />
              <input
                type="text"
                placeholder={t('pos.searchItems')}
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-9 pr-3 py-2 rounded-lg outline-none text-xs"
                style={{ 
                  background: styles.glass, 
                  border: `1px solid ${styles.glassBorder}`,
                  color: styles.textMain
                }}
              />
            </div>

            {/* Customer */}
            <button 
              onClick={() => setShowCustomerModal(true)}
              className="flex items-center gap-1.5 px-3 py-2 rounded-lg transition-all select-none text-xs"
              style={{
                background: selectedCustomer ? 'rgba(77,166,232,0.1)' : styles.glass,
                border: `1px solid ${selectedCustomer ? 'rgba(77,166,232,0.3)' : styles.glassBorder}`,
                color: selectedCustomer ? styles.accent : styles.textMuted
              }}
            >
              <User size={14} />
              {selectedCustomer ? selectedCustomer.name : t('pos.addCustomer')}
            </button>
          </div>

          {/* Main Content Area */}
          <div className="flex-1 overflow-y-auto p-5">
            {/* Categories Grid */}
            {!selectedCategory ? (
              <div className="grid gap-3" style={{ gridTemplateColumns: 'repeat(auto-fill, minmax(140px, 1fr))' }}>
                {categories.map(cat => {
                  const itemCount = menuItems.filter(i => i.categoryId === cat.id).length
                  return (
                    <button
                      key={cat.id}
                      onClick={() => setSelectedCategory(cat.id)}
                      className="rounded-xl overflow-hidden transition-all cursor-pointer select-none hover:-translate-y-1 hover:shadow-xl relative group"
                      style={{
                        background: styles.panelBg,
                        border: `1px solid ${styles.glassBorder}`,
                        aspectRatio: '4/3'
                      }}
                    >
                      {cat.image && (
                        <img 
                          src={cat.image} 
                          alt={cat.name}
                          className="absolute inset-0 w-full h-full object-cover opacity-30 group-hover:opacity-50 transition-opacity"
                          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none' }}
                        />
                      )}
                      <div className="absolute inset-0 bg-gradient-to-t from-black/70 to-transparent" />
                      <div className="absolute bottom-0 left-0 right-0 p-3">
                        <h3 className="font-bold text-sm">{cat.name}</h3>
                        <span className="text-[11px]" style={{ color: styles.textMuted }}>{itemCount} {t('common.items')}</span>
                      </div>
                    </button>
                  )
                })}
              </div>
            ) : (
              <>
                {/* Category Header */}
                <div className="flex items-center gap-3 mb-4">
                  <button
                    onClick={() => {
                      setSelectedCategory(null)
                      setSearchTerm('')
                    }}
                    className="w-8 h-8 rounded-lg flex items-center justify-center transition-all"
                    style={{ background: styles.glass, border: `1px solid ${styles.glassBorder}` }}
                  >
                    <span className="text-sm">←</span>
                  </button>
                  <span className="font-semibold text-sm">{categories.find(c => c.id === selectedCategory)?.name}</span>
                  <span className="text-xs px-2 py-0.5 rounded-full" style={{ background: styles.glass, color: styles.textMuted }}>
                    {filteredItems.length} {t('common.items')}
                  </span>
                </div>

                {/* Items Grid */}
                <div className="grid gap-3" style={{ gridTemplateColumns: 'repeat(auto-fill, minmax(140px, 1fr))' }}>
                  {filteredItems.map(item => (
                    <button
                      key={item.id}
                      onClick={() => handleItemClick(item)}
                      className="rounded-xl overflow-hidden transition-all cursor-pointer select-none hover:-translate-y-1 hover:shadow-lg flex flex-col text-left"
                      style={{
                        background: styles.panelBg,
                        border: `1px solid ${styles.glassBorder}`
                      }}
                    >
                      <div className="w-full h-24 overflow-hidden" style={{ background: '#1e222b' }}>
                        <img 
                          src={item.imageUrl || `https://placehold.co/200x120/1e222b/555?text=${encodeURIComponent(item.name.substring(0,8))}`} 
                          alt={item.name}
                          className="w-full h-full object-cover"
                          onError={(e) => { (e.target as HTMLImageElement).src = `https://placehold.co/200x120/1e222b/555?text=${encodeURIComponent(item.name.substring(0,8))}` }}
                        />
                      </div>
                      <div className="p-2.5 flex flex-col gap-0.5">
                        <h4 className="font-semibold text-xs leading-tight line-clamp-2">{item.name}</h4>
                        <div className="flex items-center justify-between gap-1 mt-1">
                          <span className="font-bold text-sm" style={{ color: styles.success }}>{money(item.defaultPrice)}</span>
                          {item.allowSizes && (
                            <span className="text-[9px] px-1.5 py-0.5 rounded" style={{ 
                              background: 'rgba(77,166,232,0.12)',
                              color: styles.accent
                            }}>{t('pos.sizes')}</span>
                          )}
                        </div>
                      </div>
                    </button>
                  ))}
                </div>
                {filteredItems.length === 0 && (
                  <div className="text-center py-12 text-sm" style={{ color: styles.textMuted }}>
                    {t('pos.noItemsFound')}
                  </div>
                )}
              </>
            )}
          </div>
        </main>

        {/* Receipt Sidebar */}
        <aside className="w-[360px] flex flex-col shrink-0 overflow-hidden" style={{
          background: '#111419',
          borderLeft: `1px solid ${styles.glassBorder}`
        }}>
          {/* Sidebar Header */}
          <div className="flex items-center justify-between px-4 py-3 shrink-0" style={{ borderBottom: `1px solid ${styles.glassBorder}` }}>
            <div className="flex items-center gap-2">
              <ShoppingBasket size={16} style={{ color: styles.accent }} />
              <span className="font-semibold text-sm">{t('pos.currentOrder')}</span>
            </div>
            <span className="text-[10px] font-semibold px-2 py-0.5 rounded" style={{
              background: orderType === 'DineIn' ? 'rgba(77,166,232,0.12)' 
                : orderType === 'Takeaway' ? 'rgba(240,200,80,0.12)' 
                : 'rgba(99,217,160,0.12)',
              color: orderType === 'DineIn' ? styles.accent 
                : orderType === 'Takeaway' ? styles.warning 
                : styles.success
            }}>
              {orderType === 'DineIn' ? t('pos.dineIn') : orderType === 'Takeaway' ? t('pos.takeaway') : t('pos.delivery')}
            </span>
          </div>

          {/* Meta Info */}
          {(selectedTable || selectedCustomer) && (
            <div className="flex items-center gap-3 px-4 py-2 text-xs shrink-0" style={{ 
              color: styles.textMuted,
              borderBottom: `1px solid ${styles.glassBorder}` 
            }}>
              {selectedTable && <span>{t('reservations.table')}: <strong style={{ color: styles.textMain }}>{tables.find(t => t.id === selectedTable)?.tableName}</strong></span>}
              {selectedTable && selectedCustomer && <span className="opacity-30">|</span>}
              {selectedCustomer && <span>{t('loyaltyTransactions.customer')}: <strong style={{ color: styles.textMain }}>{selectedCustomer.name}</strong></span>}
            </div>
          )}

          {/* Upcoming Reservations */}
          {upcomingReservations.length > 0 && (
            <div className="mx-4 mt-3 mb-1 rounded-lg p-2.5 shrink-0" style={{
              background: 'rgba(240,200,80,0.06)',
              border: '1px solid rgba(240,200,80,0.15)'
            }}>
              <div className="flex items-center gap-2 mb-1.5" style={{ color: styles.warning }}>
                <Clock size={12} />
                <span className="font-semibold text-[10px] uppercase tracking-wider">{t('pos.upcomingReservations')}</span>
                <span className="ml-auto text-[10px] font-bold px-1.5 py-0.5 rounded" style={{ background: 'rgba(240,200,80,0.12)' }}>
                  {upcomingReservations.length}
                </span>
              </div>
              <div className="space-y-1 max-h-20 overflow-y-auto">
                {upcomingReservations.slice(0, 4).map(res => {
                  const resDate = new Date(res.reservationDate)
                  const today = new Date()
                  const isToday = resDate.toDateString() === today.toDateString()
                  const tomorrow = new Date(today)
                  tomorrow.setDate(today.getDate() + 1)
                  const isTomorrow = resDate.toDateString() === tomorrow.toDateString()
                  const dateLabel = isToday ? 'Today' : isTomorrow ? 'Tomorrow' : resDate.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
                  
                  return (
                    <div key={res.id} className="flex items-center gap-2 text-[11px]">
                      <span className={`px-1 py-0.5 rounded text-[9px] font-bold ${isToday ? 'bg-red-500/15 text-red-400' : 'bg-gray-500/15 text-gray-400'}`}>
                        {dateLabel}
                      </span>
                      <span className="font-medium" style={{ color: styles.textMain }}>{res.startTime?.substring(0, 5)}</span>
                      <span className="truncate flex-1" style={{ color: styles.textMuted }}>{res.customerName}</span>
                      <span className="flex items-center gap-0.5" style={{ color: styles.textMuted }}>
                        <User size={9} /> {res.partySize}
                      </span>
                    </div>
                  )
                })}
              </div>
            </div>
          )}

          {/* Order Lines */}
          <div className="flex-1 overflow-y-auto px-4 py-3">
            {orderLines.length === 0 ? (
              <div className="flex flex-col items-center justify-center h-full text-center" style={{ color: styles.textMuted }}>
                <ShoppingBasket size={28} className="mb-2 opacity-40" />
                <div className="font-semibold text-sm" style={{ color: styles.textMain }}>{t('pos.noItemsAdded')}</div>
                <div className="text-xs mt-1">{t('pos.tapToAdd')}</div>
              </div>
            ) : (
              <div className="space-y-1.5">
                {orderLines.map(line => (
                  <div key={line.id} className="rounded-lg p-2.5 flex gap-2.5" style={{
                    background: 'rgba(255,255,255,0.02)',
                    border: `1px solid rgba(255,255,255,0.05)`
                  }}>
                    {/* Qty Badge */}
                    <span className="w-6 h-6 rounded flex items-center justify-center text-[10px] font-bold shrink-0 mt-0.5" style={{ 
                      background: 'rgba(77,166,232,0.12)', color: styles.accent 
                    }}>
                      {line.quantity}
                    </span>

                    {/* Item Details */}
                    <div className="flex-1 min-w-0">
                      <div className="font-semibold text-xs truncate">{line.name}</div>
                      {line.sizeName && <div className="text-[10px] mt-0.5" style={{ color: styles.textMuted }}>{line.sizeName}</div>}
                      {line.modifiers.length > 0 && (
                        <div className="text-[10px] mt-0.5" style={{ color: styles.accent }}>
                          {line.modifiers.map(m => m.name).join(', ')}
                        </div>
                      )}
                      {line.notes && (
                        <div className="text-[10px] mt-0.5 italic" style={{ color: styles.textMuted }}>{line.notes}</div>
                      )}
                      {/* Qty Controls */}
                      <div className="flex items-center gap-1.5 mt-1.5">
                        <button 
                          onClick={() => updateLineQuantity(line.id, -1)}
                          className="w-5 h-5 rounded flex items-center justify-center"
                          style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.glassBorder}` }}
                        >
                          <Minus size={10} />
                        </button>
                        <span className="text-[10px] font-bold w-4 text-center">{line.quantity}</span>
                        <button 
                          onClick={() => updateLineQuantity(line.id, 1)}
                          className="w-5 h-5 rounded flex items-center justify-center"
                          style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.glassBorder}` }}
                        >
                          <Plus size={10} />
                        </button>
                      </div>
                    </div>

                    {/* Price + Actions */}
                    <div className="flex flex-col items-end gap-1 shrink-0">
                      <div 
                        className="text-right cursor-pointer"
                        onClick={() => openLineDiscount(line.id)}
                        title={t('pos.clickToApplyDiscount')}
                      >
                        <div className="font-bold text-xs">{money(line.lineNet)}</div>
                        <div className="text-[10px]" style={{ color: styles.textMuted }}>{money(line.effectivePrice)} × {line.quantity}</div>
                        {line.discountPercent > 0 && (
                          <div className="text-[10px]" style={{ color: styles.success }}>-{line.discountPercent}%</div>
                        )}
                      </div>
                      <button onClick={() => removeLine(line.id)} className="mt-auto">
                        <Trash2 size={12} style={{ color: styles.danger, opacity: 0.6 }} />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Totals & Actions */}
          <div className="px-4 py-3 space-y-1.5 shrink-0" style={{ 
            borderTop: `1px solid ${styles.glassBorder}`,
            background: '#0e1116'
          }}>
            <div className="flex justify-between text-xs" style={{ color: styles.textMuted }}>
              <span>{t('pos.subtotal')}</span>
              <span>{money(subtotal)}</span>
            </div>

            {totalLineDiscount > 0 && (
              <div className="flex justify-between text-xs" style={{ color: styles.success }}>
                <span>{t('pos.lineDiscounts')}</span>
                <span>-{money(totalLineDiscount)}</span>
              </div>
            )}

            <div className="flex justify-between text-xs items-center" style={{ color: styles.textMuted }}>
              <span className="flex items-center gap-1">
                <Percent size={11} /> {t('pos.billDiscount')}
              </span>
              <span className="flex items-center gap-1">
                <input
                  type="number"
                  min="0"
                  max="100"
                  value={billDiscount}
                  onChange={(e) => setBillDiscount(parseFloat(e.target.value) || 0)}
                  className="w-12 text-right rounded px-1.5 py-0.5 outline-none font-semibold text-xs"
                  style={{ 
                    background: 'rgba(255,255,255,0.04)', 
                    border: `1px solid ${styles.glassBorder}`,
                    color: styles.textMain
                  }}
                />
                <span className="font-semibold">%</span>
              </span>
            </div>

            {(serviceCharge > 0 || tax > 0) && (
              <div className="flex justify-between text-xs" style={{ color: styles.textMuted }}>
                <span>{t('pos.serviceCharge')} + {t('pos.vat')}</span>
                <span>{money(serviceCharge + tax)}</span>
              </div>
            )}

            <div className="flex justify-between items-baseline pt-1.5 pb-1" style={{ borderTop: `1px solid ${styles.glassBorder}` }}>
              <span className="text-sm font-semibold">{t('pos.grandTotal')}</span>
              <span className="text-xl font-bold" style={{ color: styles.success }}>{money(grandTotal)}</span>
            </div>

            <div className="flex gap-2 pt-1">
              <button 
                onClick={clearOrder}
                className="px-4 py-2.5 rounded-lg font-semibold text-xs select-none transition-all"
                style={{
                  border: `1px solid rgba(242,139,139,0.2)`,
                  background: 'rgba(242,139,139,0.06)',
                  color: styles.danger
                }}
              >
                {t('pos.clearOrder')}
              </button>
              <button 
                onClick={() => setShowPaymentModal(true)}
                disabled={orderLines.length === 0}
                className="flex-1 py-2.5 rounded-lg font-bold text-sm flex justify-between items-center select-none disabled:opacity-40 disabled:cursor-not-allowed transition-all"
                style={{
                  background: orderLines.length > 0 ? 'rgba(99,217,160,0.15)' : styles.glass,
                  border: `1px solid ${orderLines.length > 0 ? 'rgba(99,217,160,0.3)' : styles.glassBorder}`,
                  color: orderLines.length > 0 ? styles.success : styles.textMuted
                }}
              >
                <span className="flex items-center gap-1.5">
                  <CreditCard size={14} /> {t('pos.pay')}
                </span>
                <span className="font-bold">{money(grandTotal)}</span>
              </button>
            </div>
          </div>
        </aside>
      </div>

      {/* Item Modal */}
      {showModifierModal && selectedItemForModifier && (
        <div className="fixed inset-0 flex items-center justify-center z-[999] p-4" style={{
          background: 'rgba(0,0,0,0.55)',
          backdropFilter: 'blur(6px)'
        }}>
          <div className="w-full max-w-[560px] rounded-2xl overflow-hidden" style={{
            background: 'rgba(15,15,18,0.92)',
            border: `1px solid ${styles.glassBorder}`,
            boxShadow: '0 20px 40px rgba(0,0,0,0.45)'
          }}>
            <div className="px-4 py-3.5 flex items-center justify-between" style={{ borderBottom: `1px solid rgba(255,255,255,0.08)` }}>
              <div className="font-black text-base flex items-center gap-2.5">
                <UtensilsCrossed size={18} style={{ color: styles.accent }} />
                {selectedItemForModifier.name}
              </div>
              <button 
                onClick={() => setShowModifierModal(false)}
                className="w-9 h-9 rounded-xl flex items-center justify-center"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                <X size={16} />
              </button>
            </div>

            <div className="p-4 max-h-[60vh] overflow-auto space-y-4">
              {/* Sizes */}
              {selectedItemForModifier.sizes && selectedItemForModifier.sizes.length > 0 && (
                <div>
                  <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('pos.size')}</div>
                  <div className="grid grid-cols-3 gap-2.5">
                    {selectedItemForModifier.sizes.map(size => (
                      <button
                        key={size.id}
                        onClick={() => setSelectedSize(size.id)}
                        className={`p-2.5 rounded-xl text-left transition-all select-none hover:-translate-y-0.5`}
                        style={{
                          background: selectedSize === size.id ? 'rgba(0,120,212,0.14)' : 'rgba(255,255,255,0.03)',
                          border: `1px solid ${selectedSize === size.id ? styles.accent : 'rgba(255,255,255,0.14)'}`
                        }}
                      >
                        <div className="font-black">{size.sizeName}</div>
                        <div className="text-sm mt-1" style={{ color: styles.textMuted }}>{money(size.price)}</div>
                      </button>
                    ))}
                  </div>
                </div>
              )}

              {/* Quantity */}
              <div>
                <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('common.quantity')}</div>
                <div className="flex items-center gap-3">
                  <button 
                    onClick={() => setItemQuantity(q => Math.max(1, q - 1))}
                    className="w-7 h-7 rounded-full flex items-center justify-center transition-all"
                    style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.glassBorder}` }}
                  >
                    <Minus size={14} />
                  </button>
                  <div className="text-xl font-black w-11 text-center">{itemQuantity}</div>
                  <button 
                    onClick={() => setItemQuantity(q => q + 1)}
                    className="w-7 h-7 rounded-full flex items-center justify-center transition-all"
                    style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.glassBorder}` }}
                  >
                    <Plus size={14} />
                  </button>
                </div>
              </div>

              {/* Modifiers */}
              {modifiers.length > 0 && (
                <div>
                  <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('pos.addOns')}</div>
                  <div className="space-y-2">
                    {modifiers.map(mod => {
                      const selected = selectedModifiers.find(m => m.modifierId === mod.id)
                      return (
                        <div key={mod.id} className="flex items-center justify-between gap-2.5 p-2.5 rounded-xl" style={{
                          background: 'rgba(255,255,255,0.03)',
                          border: `1px solid rgba(255,255,255,0.10)`
                        }}>
                          <div>
                            <div className="font-black">{mod.name}</div>
                            <div className="text-sm" style={{ color: styles.textMuted }}>+{money(mod.extraPrice)}</div>
                          </div>
                          <div className="flex items-center gap-2">
                            {selected ? (
                              <>
                                <button 
                                  onClick={() => {
                                    if (selected.quantity === 1) {
                                      setSelectedModifiers(mods => mods.filter(m => m.modifierId !== mod.id))
                                    } else {
                                      setSelectedModifiers(mods => 
                                        mods.map(m => m.modifierId === mod.id 
                                          ? {...m, quantity: m.quantity - 1} 
                                          : m
                                        )
                                      )
                                    }
                                  }}
                                  className="w-7 h-7 rounded-full flex items-center justify-center"
                                  style={{ background: 'rgba(231,76,60,0.12)', border: `1px solid rgba(231,76,60,0.5)` }}
                                >
                                  <Minus size={12} />
                                </button>
                                <div className="w-5 text-center font-black">{selected.quantity}</div>
                                <button 
                                  onClick={() => {
                                    setSelectedModifiers(mods => 
                                      mods.map(m => m.modifierId === mod.id 
                                        ? {...m, quantity: m.quantity + 1} 
                                        : m
                                      )
                                    )
                                  }}
                                  className="w-7 h-7 rounded-full flex items-center justify-center"
                                  style={{ background: 'rgba(0,120,212,0.15)', border: `1px solid rgba(0,120,212,0.5)` }}
                                >
                                  <Plus size={12} />
                                </button>
                              </>
                            ) : (
                              <button 
                                onClick={() => setSelectedModifiers(mods => [...mods, {modifierId: mod.id, quantity: 1}])}
                                className="px-2.5 py-2 rounded-lg font-black"
                                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
                              >
                                {t('common.add')}
                              </button>
                            )}
                          </div>
                        </div>
                      )
                    })}
                  </div>
                </div>
              )}

              {/* Notes */}
              <div>
                <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('pos.specialInstructions')}</div>
                <textarea
                  value={itemNotes}
                  onChange={(e) => setItemNotes(e.target.value)}
                  placeholder={t('pos.anySpecialRequests')}
                  className="w-full rounded-xl p-2.5 outline-none resize-y min-h-[70px]"
                  style={{ 
                    background: 'rgba(255,255,255,0.04)', 
                    border: `1px solid rgba(255,255,255,0.14)`,
                    color: styles.textMain
                  }}
                />
              </div>
            </div>

            <div className="px-4 py-3.5 flex gap-2.5 justify-end" style={{ borderTop: `1px solid rgba(255,255,255,0.08)` }}>
              <button 
                onClick={() => setShowModifierModal(false)}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                {t('common.cancel')}
              </button>
              <button 
                onClick={confirmAddItem}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(0,120,212,0.18)', border: `1px solid rgba(0,120,212,0.6)` }}
              >
                {t('pos.addToOrder')}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Payment Modal */}
      {showPaymentModal && (
        <div className="fixed inset-0 flex items-center justify-center z-[999] p-4" style={{
          background: 'rgba(0,0,0,0.55)',
          backdropFilter: 'blur(6px)'
        }}>
          <div className="w-full max-w-[420px] rounded-2xl overflow-hidden" style={{
            background: 'rgba(15,15,18,0.92)',
            border: `1px solid ${styles.glassBorder}`,
            boxShadow: '0 20px 40px rgba(0,0,0,0.45)'
          }}>
            <div className="px-4 py-3.5 flex items-center justify-between" style={{ borderBottom: `1px solid rgba(255,255,255,0.08)` }}>
              <div className="font-black text-base flex items-center gap-2.5">
                <CreditCard size={18} style={{ color: styles.success }} /> {t('pos.payment')}
              </div>
              <button 
                onClick={() => setShowPaymentModal(false)}
                className="w-9 h-9 rounded-xl flex items-center justify-center"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                <X size={16} />
              </button>
            </div>

            <div className="p-4 space-y-4">
              <div className="text-center">
                <div className="text-sm" style={{ color: styles.textMuted }}>{t('pos.totalAmount')}</div>
                <div className="text-4xl font-black mt-1.5" style={{ color: styles.accent }}>{money(grandTotal)}</div>
              </div>

              {processing ? (
                <div className="text-center py-2.5">
                  <div className="w-6 h-6 border-3 rounded-full mx-auto animate-spin" style={{
                    borderColor: 'rgba(255,255,255,0.15)',
                    borderTopColor: styles.accent
                  }}></div>
                  <div className="mt-2.5 text-sm" style={{ color: styles.textMuted }}>{t('pos.processingPayment')}</div>
                </div>
              ) : (
                <div className="grid grid-cols-2 gap-2.5">
                  {paymentMethods.length > 0 ? (
                    paymentMethods.map(pm => (
                      <button
                        key={pm.id}
                        onClick={() => processPayment(pm.id, pm.name)}
                        className="p-3.5 rounded-xl font-black flex items-center justify-center gap-2.5 transition-all select-none hover:-translate-y-0.5"
                        style={{
                          background: 'rgba(255,255,255,0.03)',
                          border: `1px solid rgba(255,255,255,0.14)`
                        }}
                      >
                        {pm.type === 'Cash' ? <DollarSign size={18} /> : <CreditCard size={18} />}
                        {pm.name}
                      </button>
                    ))
                  ) : (
                    <>
                      <button
                        onClick={() => processPayment(null, 'Cash')}
                        className="p-3.5 rounded-xl font-black flex items-center justify-center gap-2.5 transition-all select-none hover:-translate-y-0.5"
                        style={{
                          background: 'rgba(255,255,255,0.03)',
                          border: `1px solid rgba(255,255,255,0.14)`
                        }}
                      >
                        <DollarSign size={18} /> {t('pos.cash')}
                      </button>
                    </>
                  )}
                </div>
              )}
            </div>

            <div className="px-4 py-3.5 flex justify-end" style={{ borderTop: `1px solid rgba(255,255,255,0.08)` }}>
              <button 
                onClick={() => setShowPaymentModal(false)}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                {t('common.close')}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Customer Modal */}
      {showCustomerModal && (
        <div className="fixed inset-0 flex items-center justify-center z-[999] p-4" style={{
          background: 'rgba(0,0,0,0.55)',
          backdropFilter: 'blur(6px)'
        }}>
          <div className="w-full max-w-[560px] rounded-2xl overflow-hidden" style={{
            background: 'rgba(15,15,18,0.92)',
            border: `1px solid ${styles.glassBorder}`,
            boxShadow: '0 20px 40px rgba(0,0,0,0.45)'
          }}>
            <div className="px-4 py-3.5 flex items-center justify-between" style={{ borderBottom: `1px solid rgba(255,255,255,0.08)` }}>
              <div className="font-black text-base flex items-center gap-2.5">
                <User size={18} style={{ color: styles.accent }} /> {t('pos.selectCustomer')}
              </div>
              <button 
                onClick={() => setShowCustomerModal(false)}
                className="w-9 h-9 rounded-xl flex items-center justify-center"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                <X size={16} />
              </button>
            </div>

            <div className="p-4 max-h-[60vh] overflow-auto space-y-2">
              <button
                onClick={() => {
                  setSelectedCustomer(null)
                  setShowCustomerModal(false)
                }}
                className="w-full p-3 text-left rounded-xl transition-all hover:bg-gray-900/5"
                style={{ color: styles.textMuted }}
              >
                {t('pos.walkIn')}
              </button>
              {customers.map(c => (
                <button
                  key={c.id}
                  onClick={() => {
                    setSelectedCustomer(c)
                    setShowCustomerModal(false)
                  }}
                  className="w-full p-3 text-left rounded-xl transition-all"
                  style={{
                    background: selectedCustomer?.id === c.id ? 'rgba(0,120,212,0.14)' : 'rgba(255,255,255,0.03)',
                    border: `1px solid ${selectedCustomer?.id === c.id ? styles.accent : 'rgba(255,255,255,0.10)'}`
                  }}
                >
                  <div className="font-black">{c.name}</div>
                  <div className="text-sm" style={{ color: styles.textMuted }}>{c.phone}</div>
                </button>
              ))}
            </div>

            <div className="px-4 py-3.5 flex justify-end" style={{ borderTop: `1px solid rgba(255,255,255,0.08)` }}>
              <button 
                onClick={() => setShowCustomerModal(false)}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                {t('common.close')}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Line Discount Modal */}
      {showLineDiscountModal && selectedLineForDiscount && (
        <div className="fixed inset-0 flex items-center justify-center z-[999] p-4" style={{
          background: 'rgba(0,0,0,0.55)',
          backdropFilter: 'blur(6px)'
        }}>
          <div className="w-full max-w-[420px] rounded-2xl overflow-hidden" style={{
            background: 'rgba(15,15,18,0.92)',
            border: `1px solid ${styles.glassBorder}`,
            boxShadow: '0 20px 40px rgba(0,0,0,0.45)'
          }}>
            <div className="px-4 py-3.5 flex items-center justify-between" style={{ borderBottom: `1px solid rgba(255,255,255,0.08)` }}>
              <div className="font-black text-base flex items-center gap-2.5">
                <Tag size={18} style={{ color: styles.success }} /> {t('pos.lineDiscount')}
              </div>
              <button 
                onClick={() => setShowLineDiscountModal(false)}
                className="w-9 h-9 rounded-xl flex items-center justify-center"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                <X size={16} />
              </button>
            </div>

            <div className="p-4 space-y-4">
              <div>
                <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>{t('pos.discountPercentage')}</div>
                <div className="flex items-center gap-2.5">
                  <input
                    type="number"
                    min="0"
                    max="100"
                    value={lineDiscountInput}
                    onChange={(e) => setLineDiscountInput(parseFloat(e.target.value) || 0)}
                    className="w-[120px] rounded-xl px-2.5 py-2 outline-none font-black"
                    style={{ 
                      background: 'rgba(255,255,255,0.04)', 
                      border: `1px solid ${styles.glassBorder}`,
                      color: styles.textMain
                    }}
                    autoFocus
                  />
                  <div className="font-black">%</div>
                </div>
              </div>
              <div className="text-sm" style={{ color: styles.textMuted }}>
                Tip: click the price box on a line to open this.
              </div>
            </div>

            <div className="px-4 py-3.5 flex gap-2.5 justify-end" style={{ borderTop: `1px solid rgba(255,255,255,0.08)` }}>
              <button 
                onClick={() => setShowLineDiscountModal(false)}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid rgba(255,255,255,0.16)` }}
              >
                {t('common.cancel')}
              </button>
              <button 
                onClick={applyLineDiscount}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(46,204,113,0.18)', border: `1px solid rgba(46,204,113,0.6)` }}
              >
                {t('pos.apply')}
              </button>
            </div>
          </div>
        </div>
      )}

    </div>
  )
}
