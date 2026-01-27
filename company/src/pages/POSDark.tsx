import { useState, useEffect } from 'react'
import { useAuth } from '../contexts/AuthContext'
import api from '../lib/api'
import { 
  ShoppingBasket, Plus, Minus, Trash2, Search, User, 
  UtensilsCrossed, Package, Truck, CreditCard, X,
  Percent, DollarSign, MapPin, Clock, Tag, Home, ArrowLeft, LogOut
} from 'lucide-react'
import { useNavigate } from 'react-router-dom'

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

// Dark theme styles
const styles = {
  bgDark: '#0c0c0e',
  panelBg: '#1c1c22',
  glass: 'rgba(255, 255, 255, 0.05)',
  glassBorder: 'rgba(255, 255, 255, 0.1)',
  accent: '#0078d4',
  success: '#2ecc71',
  danger: '#e74c3c',
  warning: '#f39c12',
  textMain: '#ffffff',
  textMuted: '#a0a0a0',
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
  const { logout } = useAuth()
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

      const payRes = await api.post(`${API_URL}/company/orders/${orderId}/pay`, {
        payments: [{
          paymentMethodId: paymentMethodId || 1,
          amount: grandTotal,
          currencyCode: 'USD'
        }]
      })

      alert(`✓ Order #${orderRes.data.orderNumber} paid via ${paymentMethodName}\nStatus: ${payRes.data.paymentStatus}`)
      setShowPaymentModal(false)
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
      background: `radial-gradient(circle at top left, #1a1a2e 0%, ${styles.bgDark} 100%)`,
      color: styles.textMain,
      fontFamily: "'Segoe UI', system-ui, -apple-system, sans-serif",
      zIndex: 50
    }}>
      {/* Header */}
      <header className="h-[50px] flex items-center justify-between px-4 border-b shrink-0 z-50" style={{
        background: 'rgba(0,0,0,0.3)',
        backdropFilter: 'blur(15px)',
        borderColor: styles.glassBorder
      }}>
        <div className="flex items-center gap-3">
          <button 
            onClick={() => navigate('/')}
            className="flex items-center gap-2 pr-3 mr-2 hover:text-blue-400 transition-colors"
            style={{ borderRight: `1px solid ${styles.glassBorder}` }}
          >
            <ArrowLeft size={18} />
            <Home size={18} />
          </button>
          <div style={{ color: styles.accent }}>
            <ShoppingBasket size={24} />
          </div>
          <h2 className="text-sm tracking-wide">
            RETAIL POS <span className="font-black" style={{ color: styles.accent }}>PRO</span>
          </h2>
        </div>

        <div className="flex items-center gap-4 text-sm" style={{ color: styles.textMuted }}>
          <select 
            className="px-3 py-2 rounded-lg outline-none max-w-[260px]"
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
              <option key={b.id} value={b.id} style={{ background: '#111118' }}>
                {b.name} (VAT {b.vatPercent}% • SVC {b.serviceChargePercent}%)
              </option>
            ))}
          </select>
          <span className="flex items-center gap-1">
            <MapPin size={14} /> {selectedBranch?.name}
          </span>
          <span>VAT: {selectedBranch?.vatPercent || 0}% | Service: {selectedBranch?.serviceChargePercent || 0}%</span>
          <span className="flex items-center gap-1">
            <Clock size={14} /> {currentTime.toLocaleTimeString()}
          </span>
          <button 
            onClick={logout}
            className="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-red-600/20 transition-colors"
            style={{ color: styles.danger }}
            title="Logout"
          >
            <LogOut size={18} />
          </button>
        </div>
      </header>

      <div className="flex flex-1 overflow-hidden">
        {/* Menu Section */}
        <main className="flex-1 flex flex-col p-4 overflow-hidden gap-3">
          {/* Top Controls */}
          <div className="flex gap-2 items-center flex-wrap">
            <div className="flex gap-2 items-center flex-wrap">
              {(['DineIn', 'Takeaway', 'Delivery'] as const).map(type => (
                <button
                  key={type}
                  onClick={() => setOrderType(type)}
                  className={`flex items-center gap-2 px-3 py-2.5 rounded-xl font-bold transition-all select-none ${
                    orderType === type ? 'scale-[0.98]' : ''
                  }`}
                  style={{
                    background: orderType === type 
                      ? type === 'DineIn' ? 'rgba(0,120,212,0.18)' 
                      : type === 'Takeaway' ? 'rgba(243,156,18,0.18)' 
                      : 'rgba(46,204,113,0.18)'
                      : 'rgba(255,255,255,0.03)',
                    border: `1px solid ${orderType === type 
                      ? type === 'DineIn' ? styles.accent 
                      : type === 'Takeaway' ? styles.warning 
                      : styles.success
                      : styles.glassBorder}`,
                    color: styles.textMain
                  }}
                >
                  {type === 'DineIn' && <UtensilsCrossed size={16} />}
                  {type === 'Takeaway' && <Package size={16} />}
                  {type === 'Delivery' && <Truck size={16} />}
                  {type === 'DineIn' ? 'Dine In' : type}
                </button>
              ))}
            </div>

            {orderType === 'DineIn' && (
              <select 
                className="px-3 py-2.5 rounded-xl outline-none min-w-[220px]"
                style={{ 
                  background: styles.glass, 
                  border: `1px solid ${styles.glassBorder}`,
                  color: styles.textMain
                }}
                value={selectedTable || ''}
                onChange={(e) => setSelectedTable(parseInt(e.target.value) || null)}
              >
                <option value="" style={{ background: '#111118' }}>Select Table</option>
                {tables.map(t => (
                  <option key={t.id} value={t.id} style={{ background: '#111118' }}>
                    {t.tableName} ({t.zone})
                  </option>
                ))}
              </select>
            )}

            <button 
              onClick={() => setShowCustomerModal(true)}
              className="flex items-center gap-2 px-3 py-2.5 rounded-xl transition-all ml-auto select-none hover:-translate-y-0.5"
              style={{
                background: 'rgba(255,255,255,0.03)',
                border: `1px solid ${styles.glassBorder}`,
                color: styles.textMain
              }}
            >
              <User size={16} />
              {selectedCustomer ? selectedCustomer.name : 'Add Customer'}
            </button>
          </div>

          {/* Search - only show when viewing items */}
          {selectedCategory && (
            <div className="relative">
              <Search className="absolute left-4 top-1/2 -translate-y-1/2" size={18} style={{ color: styles.textMuted }} />
              <input
                type="text"
                placeholder="Search item name or code..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-12 pr-4 py-3 rounded-xl outline-none"
                style={{ 
                  background: styles.glass, 
                  border: `1px solid ${styles.glassBorder}`,
                  color: styles.textMain
                }}
              />
            </div>
          )}

          {/* Main Content Area */}
          <div className="flex-1 overflow-y-auto pr-1">
            {/* Categories Grid - Show when no category selected */}
            {!selectedCategory ? (
              <div className="grid gap-4" style={{ gridTemplateColumns: 'repeat(auto-fill, minmax(160px, 1fr))' }}>
                {categories.map(cat => {
                  const itemCount = menuItems.filter(i => i.categoryId === cat.id).length
                  return (
                    <button
                      key={cat.id}
                      onClick={() => setSelectedCategory(cat.id)}
                      className="aspect-square rounded-2xl overflow-hidden transition-all cursor-pointer select-none hover:-translate-y-2 hover:shadow-2xl relative group"
                      style={{
                        background: 'linear-gradient(135deg, #2a2a35 0%, #1a1a22 100%)',
                        border: `1px solid ${styles.glassBorder}`
                      }}
                    >
                      {cat.image && (
                        <img 
                          src={cat.image} 
                          alt={cat.name}
                          className="absolute inset-0 w-full h-full object-cover opacity-40 group-hover:opacity-60 transition-opacity"
                          onError={(e) => { (e.target as HTMLImageElement).style.display = 'none' }}
                        />
                      )}
                      <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent" />
                      <div className="absolute bottom-0 left-0 right-0 p-4">
                        <h3 className="font-black text-lg text-white">{cat.name}</h3>
                        <span className="text-sm" style={{ color: styles.textMuted }}>{itemCount} items</span>
                      </div>
                      <div className="absolute top-3 right-3 w-8 h-8 rounded-full flex items-center justify-center" 
                        style={{ background: 'rgba(255,255,255,0.1)' }}>
                        <span className="text-xs font-bold">{itemCount}</span>
                      </div>
                    </button>
                  )
                })}
              </div>
            ) : (
              /* Items Grid - Show when category is selected */
              <>
                {/* Back to Categories Button */}
                <button
                  onClick={() => {
                    setSelectedCategory(null)
                    setSearchTerm('')
                  }}
                  className="flex items-center gap-2 mb-4 px-4 py-2 rounded-xl transition-all hover:-translate-x-1"
                  style={{
                    background: 'rgba(255,255,255,0.05)',
                    border: `1px solid ${styles.glassBorder}`,
                    color: styles.textMain
                  }}
                >
                  <span style={{ fontSize: '18px' }}>←</span>
                  <span className="font-bold">Back to Categories</span>
                  <span className="ml-2 px-2 py-0.5 rounded text-sm" style={{ background: styles.accent }}>
                    {categories.find(c => c.id === selectedCategory)?.name}
                  </span>
                </button>

                <div className="grid gap-3" style={{ gridTemplateColumns: 'repeat(auto-fill, minmax(150px, 1fr))' }}>
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
                      <div className="w-full h-28 overflow-hidden" style={{ background: '#2a2a30' }}>
                        <img 
                          src={item.imageUrl || `https://placehold.co/200x120/2a2a30/666?text=${encodeURIComponent(item.name.substring(0,10))}`} 
                          alt={item.name}
                          className="w-full h-full object-cover"
                          onError={(e) => { (e.target as HTMLImageElement).src = `https://placehold.co/200x120/2a2a30/666?text=${encodeURIComponent(item.name.substring(0,10))}` }}
                        />
                      </div>
                      <div className="p-3 flex flex-col gap-1">
                        <h4 className="font-bold text-sm leading-tight line-clamp-2">{item.name}</h4>
                        {item.code && <span className="text-xs" style={{ color: styles.textMuted }}>{item.code}</span>}
                        <div className="flex items-center justify-between gap-1 mt-1">
                          <span className="font-black text-lg" style={{ color: styles.success }}>{money(item.defaultPrice)}</span>
                          {item.allowSizes && (
                            <span className="text-[10px] px-2 py-0.5 rounded" style={{ 
                              background: 'rgba(0,120,212,0.2)',
                              color: styles.accent
                            }}>Sizes</span>
                          )}
                        </div>
                      </div>
                    </button>
                  ))}
                </div>
                {filteredItems.length === 0 && (
                  <div className="text-center py-8" style={{ color: styles.textMuted }}>
                    No items found in this category
                  </div>
                )}
              </>
            )}
          </div>
        </main>

        {/* Receipt Sidebar */}
        <aside className="w-[340px] flex flex-col p-4 shrink-0 overflow-hidden gap-2" style={{
          background: 'rgba(0,0,0,0.4)',
          backdropFilter: 'blur(25px)',
          borderLeft: `1px solid ${styles.glassBorder}`
        }}>
          {/* Header */}
          <div className="flex items-center justify-between pb-3" style={{ borderBottom: `1px solid ${styles.glassBorder}` }}>
            <h3 className="font-bold text-base flex items-center gap-2" style={{ color: styles.accent }}>
              <ShoppingBasket size={18} /> CURRENT ORDER
            </h3>
            <span className="text-xs font-black px-2.5 py-1 rounded-full" style={{
              background: orderType === 'DineIn' ? 'rgba(0,120,212,0.15)' 
                : orderType === 'Takeaway' ? 'rgba(243,156,18,0.15)' 
                : 'rgba(46,204,113,0.15)',
              border: `1px solid ${orderType === 'DineIn' ? 'rgba(0,120,212,0.5)' 
                : orderType === 'Takeaway' ? 'rgba(243,156,18,0.5)' 
                : 'rgba(46,204,113,0.5)'}`
            }}>
              {orderType}
            </span>
          </div>

          {/* Meta */}
          <div className="text-sm flex flex-col gap-1 pb-1" style={{ color: styles.textMuted }}>
            {selectedTable && <div>Table: {tables.find(t => t.id === selectedTable)?.tableName}</div>}
            {selectedCustomer && <div>Customer: {selectedCustomer.name}</div>}
          </div>

          {/* Upcoming Reservations */}
          {upcomingReservations.length > 0 && (
            <div className="mb-3 rounded-xl p-3" style={{
              background: 'rgba(243,156,18,0.1)',
              border: '1px solid rgba(243,156,18,0.3)'
            }}>
              <div className="flex items-center gap-2 mb-2" style={{ color: styles.warning }}>
                <Clock size={14} />
                <span className="font-bold text-xs">UPCOMING RESERVATIONS</span>
                <span className="ml-auto text-xs px-2 py-0.5 rounded-full" style={{ background: 'rgba(243,156,18,0.2)' }}>
                  {upcomingReservations.length}
                </span>
              </div>
              <div className="space-y-1.5 max-h-24 overflow-y-auto">
                {upcomingReservations.slice(0, 5).map(res => {
                  const resDate = new Date(res.reservationDate)
                  const today = new Date()
                  const isToday = resDate.toDateString() === today.toDateString()
                  const tomorrow = new Date(today)
                  tomorrow.setDate(today.getDate() + 1)
                  const isTomorrow = resDate.toDateString() === tomorrow.toDateString()
                  const dateLabel = isToday ? 'Today' : isTomorrow ? 'Tomorrow' : resDate.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
                  
                  return (
                    <div key={res.id} className="flex items-center gap-2 text-xs" style={{ color: styles.textMain }}>
                      <span className={`px-1.5 py-0.5 rounded text-[10px] font-bold ${isToday ? 'bg-red-500/20 text-red-400' : 'bg-gray-500/20 text-gray-400'}`}>
                        {dateLabel}
                      </span>
                      <span className="font-medium">{res.startTime?.substring(0, 5)}</span>
                      <span className="truncate flex-1">{res.customerName}</span>
                      <span className="flex items-center gap-1" style={{ color: styles.textMuted }}>
                        <User size={10} /> {res.partySize}
                      </span>
                    </div>
                  )
                })}
              </div>
            </div>
          )}

          {/* Order List */}
          <div className="flex-1 overflow-y-auto pr-1">
            {orderLines.length === 0 ? (
              <div className="rounded-xl p-5 text-center" style={{
                border: `1px dashed rgba(255,255,255,0.18)`,
                background: 'rgba(255,255,255,0.03)',
                color: styles.textMuted
              }}>
                <ShoppingBasket size={34} className="mx-auto mb-2 opacity-70" />
                <div className="font-black text-white mt-1">No items added</div>
                <div className="mt-1">Tap a menu item to add it.</div>
              </div>
            ) : (
              <div className="space-y-2.5">
                {orderLines.map(line => (
                  <div key={line.id} className="rounded-xl p-3 flex justify-between items-start gap-2.5" style={{
                    background: 'rgba(255,255,255,0.03)',
                    border: `1px solid rgba(255,255,255,0.08)`
                  }}>
                    <div className="flex items-start gap-2.5 flex-1 min-w-0">
                      <span className="px-2 py-0.5 rounded-md text-xs font-black text-white" style={{ background: styles.accent }}>
                        {line.quantity}
                      </span>
                      <div className="min-w-0">
                        <div className="font-extrabold text-sm truncate">{line.name}</div>
                        {line.sizeName && <div className="text-xs mt-0.5" style={{ color: styles.textMuted }}>{line.sizeName}</div>}
                        {line.modifiers.length > 0 && (
                          <div className="text-xs mt-1" style={{ color: 'rgba(0,120,212,0.9)' }}>
                            {line.modifiers.map(m => m.name).join(', ')}
                          </div>
                        )}
                        {line.notes && (
                          <div className="text-xs mt-1 italic" style={{ color: 'rgba(255,255,255,0.55)' }}>{line.notes}</div>
                        )}
                        <div className="flex items-center gap-2 mt-2.5">
                          <button 
                            onClick={() => updateLineQuantity(line.id, -1)}
                            className="w-7 h-7 rounded-full flex items-center justify-center transition-all hover:-translate-y-0.5"
                            style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.glassBorder}` }}
                          >
                            <Minus size={12} />
                          </button>
                          <div className="w-6 text-center font-black">{line.quantity}</div>
                          <button 
                            onClick={() => updateLineQuantity(line.id, 1)}
                            className="w-7 h-7 rounded-full flex items-center justify-center transition-all hover:-translate-y-0.5"
                            style={{ background: 'rgba(255,255,255,0.04)', border: `1px solid ${styles.glassBorder}` }}
                          >
                            <Plus size={12} />
                          </button>
                        </div>
                      </div>
                    </div>

                    <div className="flex flex-col items-end gap-2">
                      <div className="flex items-center gap-2.5">
                        <div 
                          className="text-right cursor-pointer rounded-lg p-1.5 -m-1.5 transition-all hover:bg-gray-900/5"
                          onClick={() => openLineDiscount(line.id)}
                          title="Click to apply discount"
                        >
                          <div className="font-black">{money(line.lineNet)}</div>
                          <div className="text-xs" style={{ color: styles.textMuted }}>{money(line.effectivePrice)} x {line.quantity}</div>
                          {line.discountPercent > 0 && (
                            <div className="text-xs mt-0.5" style={{ color: styles.success }}>-{line.discountPercent}% off</div>
                          )}
                        </div>
                        <Trash2 
                          size={14} 
                          className="cursor-pointer" 
                          style={{ color: styles.danger }}
                          onClick={() => removeLine(line.id)}
                        />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Totals - Fixed at bottom */}
          <div className="pt-2 space-y-1 shrink-0" style={{ borderTop: `2px solid ${styles.glassBorder}` }}>
            <div className="flex justify-between text-xs" style={{ color: styles.textMuted }}>
              <span>Subtotal</span>
              <span>{money(subtotal)}</span>
            </div>

            {totalLineDiscount > 0 && (
              <div className="flex justify-between text-xs" style={{ color: styles.success }}>
                <span>Discounts</span>
                <span>-{money(totalLineDiscount)}</span>
              </div>
            )}

            <div className="flex justify-between text-xs items-center" style={{ color: styles.textMuted }}>
              <span className="flex items-center gap-1">
                <Percent size={12} /> Bill %
              </span>
              <span className="flex items-center gap-1">
                <input
                  type="number"
                  min="0"
                  max="100"
                  value={billDiscount}
                  onChange={(e) => setBillDiscount(parseFloat(e.target.value) || 0)}
                  className="w-14 text-right rounded px-2 py-1 outline-none font-bold text-xs"
                  style={{ 
                    background: 'rgba(255,255,255,0.04)', 
                    border: `1px solid ${styles.glassBorder}`,
                    color: styles.textMain
                  }}
                />
                <span className="font-bold">%</span>
              </span>
            </div>

            {(serviceCharge > 0 || tax > 0) && (
              <div className="flex justify-between text-xs" style={{ color: styles.textMuted }}>
                <span>SVC + VAT</span>
                <span>{money(serviceCharge + tax)}</span>
              </div>
            )}

            <div className="flex justify-between items-baseline text-2xl font-black pt-1">
              <span>TOTAL</span>
              <span style={{ color: styles.success }}>{money(grandTotal)}</span>
            </div>

            <button 
              onClick={() => setShowPaymentModal(true)}
              disabled={orderLines.length === 0}
              className="w-full py-3 rounded-xl font-black text-base flex justify-between items-center select-none disabled:opacity-50 disabled:cursor-not-allowed"
              style={{
                background: 'linear-gradient(135deg, #11998e 0%, #38ef7d 100%)',
                color: '#000',
                boxShadow: '0 8px 16px rgba(0,0,0,0.2)'
              }}
            >
              <span>PAY</span>
              <span className="flex items-center gap-2">
                {money(grandTotal)}
                <CreditCard size={16} />
              </span>
            </button>

            <button 
              onClick={clearOrder}
              className="w-full py-2 rounded-lg font-bold text-sm select-none transition-all hover:bg-red-500/10"
              style={{
                border: `1px solid rgba(231,76,60,0.3)`,
                background: 'rgba(231,76,60,0.06)',
                color: '#ffb3ac'
              }}
            >
              Clear Order
            </button>
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
                  <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>Size</div>
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
                <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>Quantity</div>
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
                  <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>Add-ons</div>
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
                                Add
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
                <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>Special Instructions</div>
                <textarea
                  value={itemNotes}
                  onChange={(e) => setItemNotes(e.target.value)}
                  placeholder="Any special requests..."
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
                Cancel
              </button>
              <button 
                onClick={confirmAddItem}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(0,120,212,0.18)', border: `1px solid rgba(0,120,212,0.6)` }}
              >
                Add to Order
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
                <CreditCard size={18} style={{ color: styles.success }} /> Payment
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
                <div className="text-sm" style={{ color: styles.textMuted }}>Total Amount</div>
                <div className="text-4xl font-black mt-1.5" style={{ color: styles.accent }}>{money(grandTotal)}</div>
              </div>

              {processing ? (
                <div className="text-center py-2.5">
                  <div className="w-6 h-6 border-3 rounded-full mx-auto animate-spin" style={{
                    borderColor: 'rgba(255,255,255,0.15)',
                    borderTopColor: styles.accent
                  }}></div>
                  <div className="mt-2.5 text-sm" style={{ color: styles.textMuted }}>Processing payment...</div>
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
                        <DollarSign size={18} /> Cash
                      </button>
                      <button
                        onClick={() => processPayment(null, 'Card')}
                        className="p-3.5 rounded-xl font-black flex items-center justify-center gap-2.5 transition-all select-none hover:-translate-y-0.5"
                        style={{
                          background: 'rgba(255,255,255,0.03)',
                          border: `1px solid rgba(255,255,255,0.14)`
                        }}
                      >
                        <CreditCard size={18} /> Card
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
                Close
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
                <User size={18} style={{ color: styles.accent }} /> Select Customer
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
                No Customer (Walk-in)
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
                Close
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
                <Tag size={18} style={{ color: styles.success }} /> Line Discount
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
                <div className="text-xs font-black uppercase tracking-wider mb-2" style={{ color: styles.textMuted }}>Discount Percentage</div>
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
                Cancel
              </button>
              <button 
                onClick={applyLineDiscount}
                className="px-3.5 py-3 rounded-xl font-black"
                style={{ background: 'rgba(46,204,113,0.18)', border: `1px solid rgba(46,204,113,0.6)` }}
              >
                Apply
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
