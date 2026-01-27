import { useState, useEffect } from 'react'
import { useAuth } from '../contexts/AuthContext'
import axios from 'axios'
import { 
  ShoppingCart, Plus, Minus, Trash2, Search, User, 
  UtensilsCrossed, Package, Truck, CreditCard, X,
  ChefHat, Receipt, Percent, DollarSign
} from 'lucide-react'

const API_URL = 'http://localhost:5000/api'

interface Category {
  id: number
  name: string
  parentCategoryId: number | null
  isActive: boolean
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
}

interface Branch {
  id: number
  name: string
  vatPercent: number
  serviceChargePercent: number
}

export default function POS() {
  const { token } = useAuth()
  const [categories, setCategories] = useState<Category[]>([])
  const [menuItems, setMenuItems] = useState<MenuItem[]>([])
  const [tables, setTables] = useState<Table[]>([])
  const [customers, setCustomers] = useState<Customer[]>([])
  const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([])
  const [branches, setBranches] = useState<Branch[]>([])
  const [modifiers, setModifiers] = useState<Modifier[]>([])
  
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

  useEffect(() => {
    fetchData()
  }, [])

  const fetchData = async () => {
    try {
      const headers = { Authorization: `Bearer ${token}` }
      const [catRes, itemsRes, tablesRes, custRes, pmRes, branchRes, modRes] = await Promise.all([
        axios.get(`${API_URL}/company/categories`, { headers }),
        axios.get(`${API_URL}/company/menu-items`, { headers }),
        axios.get(`${API_URL}/company/tables`, { headers }),
        axios.get(`${API_URL}/company/customers`, { headers }),
        axios.get(`${API_URL}/company/payment-methods`, { headers }),
        axios.get(`${API_URL}/company/branches`, { headers }),
        axios.get(`${API_URL}/company/modifiers`, { headers })
      ])
      
      setCategories(catRes.data.filter((c: Category) => c.isActive))
      setMenuItems(itemsRes.data.filter((i: MenuItem) => i.isActive))
      setTables(tablesRes.data)
      setCustomers(custRes.data)
      setPaymentMethods(pmRes.data.filter((p: PaymentMethod) => p.isActive))
      setBranches(branchRes.data)
      setModifiers(modRes.data.filter((m: Modifier) => m.isActive))
      
      if (branchRes.data.length > 0) {
        setSelectedBranch(branchRes.data[0])
      }
      if (catRes.data.length > 0) {
        setSelectedCategory(catRes.data[0].id)
      }
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
    // Always open modifier modal to allow adding modifiers, quantity, and notes
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

  const [processing, setProcessing] = useState(false)

  const processPayment = async (paymentMethodId: number | null, paymentMethodName: string) => {
    if (!selectedBranch || orderLines.length === 0) return

    setProcessing(true)
    const headers = { Authorization: `Bearer ${token}` }

    try {
      console.log('Starting payment process...')
      console.log('Grand total:', grandTotal)
      
      // 1. Create order
      const orderRes = await axios.post(`${API_URL}/company/orders`, {
        branchId: selectedBranch.id,
        orderType: orderType,
        tableId: selectedTable,
        customerId: selectedCustomer?.id,
        notes: ''
      }, { headers })

      const orderId = orderRes.data.orderId

      // 2. Add order lines
      for (const line of orderLines) {
        await axios.post(`${API_URL}/company/orders/${orderId}/lines`, {
          menuItemId: line.menuItemId,
          menuItemSizeId: line.menuItemSizeId,
          quantity: line.quantity,
          discountPercent: line.discountPercent,
          notes: line.notes,
          modifiers: line.modifiers.map(m => ({
            modifierId: m.modifierId,
            quantity: m.quantity
          }))
        }, { headers })
      }

      // 3. Apply bill discount if any
      if (billDiscount > 0) {
        await axios.post(`${API_URL}/company/orders/${orderId}/discount`, {
          discountPercent: billDiscount
        }, { headers })
      }

      // 4. Process payment
      console.log('Processing payment for order:', orderId, 'Amount:', grandTotal)
      const payRes = await axios.post(`${API_URL}/company/orders/${orderId}/pay`, {
        payments: [{
          paymentMethodId: paymentMethodId || 1,
          amount: grandTotal,
          currencyCode: 'USD'
        }]
      }, { headers })
      console.log('Payment response:', payRes.data)

      alert(`✓ Order #${orderRes.data.orderNumber} paid via ${paymentMethodName}\nStatus: ${payRes.data.paymentStatus}`)
      setShowPaymentModal(false)
      clearOrder()

    } catch (error: any) {
      console.error('Payment error:', error)
      console.error('Error response data:', error.response?.data)
      alert(`Payment failed: ${error.response?.data?.message || error.message}\n${error.response?.data?.error || ''}\n${error.response?.data?.inner || ''}`)
    } finally {
      setProcessing(false)
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    )
  }

  return (
    <div className="h-screen flex flex-col bg-gray-100">
      {/* Header */}
      <div className="bg-gray-900 shadow-sm px-4 py-2 flex items-center justify-between">
        <div className="flex items-center gap-4">
          <h1 className="text-xl font-bold text-gray-900">POS</h1>
          <select 
            className="border rounded-lg px-3 py-1.5 text-sm"
            value={selectedBranch?.id || ''}
            onChange={(e) => {
              const branch = branches.find(b => b.id === parseInt(e.target.value))
              setSelectedBranch(branch || null)
            }}
          >
            {branches.map(b => (
              <option key={b.id} value={b.id}>{b.name}</option>
            ))}
          </select>
        </div>
        <div className="flex items-center gap-2">
          <span className="text-sm text-gray-500">
            VAT: {selectedBranch?.vatPercent || 0}% | Service: {selectedBranch?.serviceChargePercent || 0}%
          </span>
        </div>
      </div>

      <div className="flex-1 flex overflow-hidden">
        {/* Left Panel - Categories & Items */}
        <div className="flex-1 flex flex-col overflow-hidden">
          {/* Order Type Tabs */}
          <div className="bg-gray-900 px-4 py-2 flex gap-2 border-b border-gray-700">
            <button 
              onClick={() => setOrderType('DineIn')}
              className={`flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition ${
                orderType === 'DineIn' 
                  ? 'bg-primary-600 text-white' 
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              <UtensilsCrossed size={18} />
              Dine In
            </button>
            <button 
              onClick={() => setOrderType('Takeaway')}
              className={`flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition ${
                orderType === 'Takeaway' 
                  ? 'bg-orange-500 text-white' 
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              <Package size={18} />
              Takeaway
            </button>
            <button 
              onClick={() => setOrderType('Delivery')}
              className={`flex items-center gap-2 px-4 py-2 rounded-lg font-medium transition ${
                orderType === 'Delivery' 
                  ? 'bg-green-600 text-white' 
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              <Truck size={18} />
              Delivery
            </button>

            {orderType === 'DineIn' && (
              <select 
                className="ml-4 border rounded-lg px-3 py-1.5"
                value={selectedTable || ''}
                onChange={(e) => setSelectedTable(parseInt(e.target.value) || null)}
              >
                <option value="">Select Table</option>
                {tables.map(t => (
                  <option key={t.id} value={t.id}>
                    {t.tableName} ({t.zone})
                  </option>
                ))}
              </select>
            )}

            <button 
              onClick={() => setShowCustomerModal(true)}
              className="ml-auto flex items-center gap-2 px-3 py-1.5 border rounded-lg hover:bg-gray-800/50"
            >
              <User size={18} />
              {selectedCustomer ? selectedCustomer.name : 'Add Customer'}
            </button>
          </div>

          {/* Search */}
          <div className="px-4 py-2 bg-gray-900 border-b border-gray-700">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
              <input
                type="text"
                placeholder="Search items..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border rounded-lg"
              />
            </div>
          </div>

          {/* Categories */}
          <div className="bg-gray-900 px-4 py-2 border-b overflow-x-auto">
            <div className="flex gap-2">
              {categories.map(cat => (
                <button
                  key={cat.id}
                  onClick={() => setSelectedCategory(cat.id)}
                  className={`px-4 py-2 rounded-lg font-medium whitespace-nowrap transition ${
                    selectedCategory === cat.id
                      ? 'bg-primary-600 text-white'
                      : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                  }`}
                >
                  {cat.name}
                </button>
              ))}
            </div>
          </div>

          {/* Menu Items Grid */}
          <div className="flex-1 overflow-auto p-4">
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-3">
              {filteredItems.map(item => (
                <button
                  key={item.id}
                  onClick={() => handleItemClick(item)}
                  className="bg-gray-900 rounded-xl shadow-sm hover:shadow-md transition p-4 text-left border border-gray-100 hover:border-primary-300"
                >
                  <div className="font-semibold text-gray-900 mb-1 line-clamp-2">{item.name}</div>
                  {item.code && (
                    <div className="text-xs text-gray-400 mb-2">{item.code}</div>
                  )}
                  <div className="text-primary-600 font-bold">
                    ${item.defaultPrice.toFixed(2)}
                  </div>
                  {item.allowSizes && (
                    <div className="text-xs text-gray-500 mt-1">Multiple sizes</div>
                  )}
                </button>
              ))}
            </div>
            {filteredItems.length === 0 && (
              <div className="text-center text-gray-500 py-8">
                No items found
              </div>
            )}
          </div>
        </div>

        {/* Right Panel - Order Summary */}
        <div className="w-96 bg-gray-900 shadow-lg flex flex-col">
          <div className="p-4 border-b border-gray-700">
            <div className="flex items-center justify-between">
              <h2 className="font-bold text-lg flex items-center gap-2">
                <ShoppingCart size={20} />
                Current Order
              </h2>
              <span className={`px-2 py-1 rounded text-xs font-medium ${
                orderType === 'DineIn' ? 'bg-primary-100 text-primary-700' :
                orderType === 'Takeaway' ? 'bg-orange-100 text-orange-700' :
                'bg-green-100 text-green-700'
              }`}>
                {orderType}
              </span>
            </div>
            {selectedTable && (
              <div className="text-sm text-gray-500 mt-1">
                Table: {tables.find(t => t.id === selectedTable)?.tableName}
              </div>
            )}
            {selectedCustomer && (
              <div className="text-sm text-gray-500">
                Customer: {selectedCustomer.name}
              </div>
            )}
          </div>

          {/* Order Lines */}
          <div className="flex-1 overflow-auto p-4">
            {orderLines.length === 0 ? (
              <div className="text-center text-gray-400 py-8">
                <ShoppingCart size={48} className="mx-auto mb-2 opacity-50" />
                <p>No items added</p>
              </div>
            ) : (
              <div className="space-y-3">
                {orderLines.map(line => (
                  <div key={line.id} className="bg-gray-800 rounded-lg p-3">
                    <div className="flex justify-between items-start mb-2">
                      <div className="flex-1">
                        <div className="font-medium text-gray-900">{line.name}</div>
                        {line.sizeName && (
                          <div className="text-xs text-gray-500">{line.sizeName}</div>
                        )}
                        {line.modifiers.length > 0 && (
                          <div className="text-xs text-primary-600 mt-1">
                            {line.modifiers.map(m => m.name).join(', ')}
                          </div>
                        )}
                        {line.notes && (
                          <div className="text-xs text-gray-400 italic mt-1">{line.notes}</div>
                        )}
                      </div>
                      <button 
                        onClick={() => removeLine(line.id)}
                        className="text-red-500 hover:text-red-700 p-1"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <button 
                          onClick={() => updateLineQuantity(line.id, -1)}
                          className="w-7 h-7 rounded-full bg-gray-200 flex items-center justify-center hover:bg-gray-300"
                        >
                          <Minus size={14} />
                        </button>
                        <span className="w-8 text-center font-medium">{line.quantity}</span>
                        <button 
                          onClick={() => updateLineQuantity(line.id, 1)}
                          className="w-7 h-7 rounded-full bg-gray-200 flex items-center justify-center hover:bg-gray-300"
                        >
                          <Plus size={14} />
                        </button>
                      </div>
                      <div 
                        className="text-right cursor-pointer hover:bg-gray-100 rounded p-1 -m-1"
                        onClick={() => openLineDiscount(line.id)}
                        title="Click to apply discount"
                      >
                        <div className="font-semibold">${line.lineNet.toFixed(2)}</div>
                        <div className="text-xs text-gray-500">
                          ${line.effectivePrice.toFixed(2)} x {line.quantity}
                        </div>
                        {line.discountPercent > 0 && (
                          <div className="text-xs text-green-600">-{line.discountPercent}% off</div>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Totals */}
          <div className="border-t p-4 space-y-2 bg-gray-800">
            <div className="flex justify-between text-sm">
              <span className="text-gray-600">Subtotal</span>
              <span>${subtotal.toFixed(2)}</span>
            </div>
            {totalLineDiscount > 0 && (
              <div className="flex justify-between text-sm text-green-600">
                <span>Line Discounts</span>
                <span>-${totalLineDiscount.toFixed(2)}</span>
              </div>
            )}
            <div className="flex justify-between text-sm items-center">
              <span className="text-gray-600 flex items-center gap-1">
                <Percent size={14} />
                Bill Discount
              </span>
              <div className="flex items-center gap-1">
                <input
                  type="number"
                  min="0"
                  max="100"
                  value={billDiscount}
                  onChange={(e) => setBillDiscount(parseFloat(e.target.value) || 0)}
                  className="w-16 text-right border rounded px-2 py-0.5 text-sm"
                />
                <span>%</span>
              </div>
            </div>
            {billDiscountAmount > 0 && (
              <div className="flex justify-between text-sm text-green-600">
                <span></span>
                <span>-${billDiscountAmount.toFixed(2)}</span>
              </div>
            )}
            {serviceCharge > 0 && (
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">Service ({selectedBranch?.serviceChargePercent}%)</span>
                <span>${serviceCharge.toFixed(2)}</span>
              </div>
            )}
            {tax > 0 && (
              <div className="flex justify-between text-sm">
                <span className="text-gray-600">VAT ({selectedBranch?.vatPercent}%)</span>
                <span>${tax.toFixed(2)}</span>
              </div>
            )}
            <div className="flex justify-between font-bold text-lg pt-2 border-t">
              <span>Total</span>
              <span className="text-primary-600">${grandTotal.toFixed(2)}</span>
            </div>
          </div>

          {/* Actions */}
          <div className="p-4 border-t space-y-2">
            <div className="grid grid-cols-2 gap-2">
              <button 
                onClick={() => {/* Send to kitchen */}}
                disabled={orderLines.length === 0}
                className="flex items-center justify-center gap-2 px-4 py-3 bg-orange-500 text-white rounded-lg font-medium hover:bg-orange-600 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <ChefHat size={18} />
                Kitchen
              </button>
              <button 
                onClick={() => {/* Print bill */}}
                disabled={orderLines.length === 0}
                className="flex items-center justify-center gap-2 px-4 py-3 bg-gray-600 text-white rounded-lg font-medium hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Receipt size={18} />
                Print
              </button>
            </div>
            <button 
              onClick={() => setShowPaymentModal(true)}
              disabled={orderLines.length === 0}
              className="w-full flex items-center justify-center gap-2 px-4 py-3 bg-green-600 text-white rounded-lg font-bold text-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <CreditCard size={20} />
              Pay ${grandTotal.toFixed(2)}
            </button>
            <button 
              onClick={clearOrder}
              className="w-full px-4 py-2 text-red-600 hover:bg-red-50 rounded-lg font-medium"
            >
              Clear Order
            </button>
          </div>
        </div>
      </div>

      {/* Modifier/Size Modal */}
      {showModifierModal && selectedItemForModifier && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-900 rounded-xl shadow-xl w-full max-w-md mx-4">
            <div className="p-4 border-b flex justify-between items-center">
              <h3 className="font-bold text-lg">{selectedItemForModifier.name}</h3>
              <button onClick={() => setShowModifierModal(false)} className="text-gray-500 hover:text-gray-700">
                <X size={20} />
              </button>
            </div>
            <div className="p-4 space-y-4 max-h-96 overflow-auto">
              {/* Sizes */}
              {selectedItemForModifier.sizes && selectedItemForModifier.sizes.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Size</label>
                  <div className="grid grid-cols-3 gap-2">
                    {selectedItemForModifier.sizes.map(size => (
                      <button
                        key={size.id}
                        onClick={() => setSelectedSize(size.id)}
                        className={`p-2 rounded-lg border text-center ${
                          selectedSize === size.id
                            ? 'border-primary-500 bg-primary-50 text-primary-700'
                            : 'border-gray-200 hover:border-gray-300'
                        }`}
                      >
                        <div className="font-medium">{size.sizeName}</div>
                        <div className="text-sm text-gray-500">${size.price.toFixed(2)}</div>
                      </button>
                    ))}
                  </div>
                </div>
              )}

              {/* Quantity */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Quantity</label>
                <div className="flex items-center gap-3">
                  <button 
                    onClick={() => setItemQuantity(q => Math.max(1, q - 1))}
                    className="w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center hover:bg-gray-200"
                  >
                    <Minus size={18} />
                  </button>
                  <span className="text-xl font-bold w-12 text-center">{itemQuantity}</span>
                  <button 
                    onClick={() => setItemQuantity(q => q + 1)}
                    className="w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center hover:bg-gray-200"
                  >
                    <Plus size={18} />
                  </button>
                </div>
              </div>

              {/* Modifiers */}
              {modifiers.length > 0 && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Add-ons</label>
                  <div className="space-y-2">
                    {modifiers.map(mod => {
                      const selected = selectedModifiers.find(m => m.modifierId === mod.id)
                      return (
                        <div key={mod.id} className="flex items-center justify-between p-2 border rounded-lg">
                          <div>
                            <span className="font-medium">{mod.name}</span>
                            <span className="text-sm text-gray-500 ml-2">+${mod.extraPrice.toFixed(2)}</span>
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
                                  className="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center"
                                >
                                  <Minus size={12} />
                                </button>
                                <span className="w-6 text-center">{selected.quantity}</span>
                                <button 
                                  onClick={() => {
                                    setSelectedModifiers(mods => 
                                      mods.map(m => m.modifierId === mod.id 
                                        ? {...m, quantity: m.quantity + 1} 
                                        : m
                                      )
                                    )
                                  }}
                                  className="w-6 h-6 rounded-full bg-primary-500 text-white flex items-center justify-center"
                                >
                                  <Plus size={12} />
                                </button>
                              </>
                            ) : (
                              <button 
                                onClick={() => setSelectedModifiers(mods => [...mods, {modifierId: mod.id, quantity: 1}])}
                                className="px-3 py-1 text-sm bg-primary-100 text-primary-700 rounded-lg hover:bg-primary-200"
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
                <label className="block text-sm font-medium text-gray-700 mb-2">Special Instructions</label>
                <textarea
                  value={itemNotes}
                  onChange={(e) => setItemNotes(e.target.value)}
                  placeholder="Any special requests..."
                  className="w-full border rounded-lg p-2 text-sm"
                  rows={2}
                />
              </div>
            </div>
            <div className="p-4 border-t">
              <button
                onClick={confirmAddItem}
                className="w-full py-3 bg-primary-600 text-white rounded-lg font-bold hover:bg-primary-700"
              >
                Add to Order
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Payment Modal */}
      {showPaymentModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-900 rounded-xl shadow-xl w-full max-w-md mx-4">
            <div className="p-4 border-b flex justify-between items-center">
              <h3 className="font-bold text-lg">Payment</h3>
              <button onClick={() => setShowPaymentModal(false)} className="text-gray-500 hover:text-gray-700">
                <X size={20} />
              </button>
            </div>
            <div className="p-4">
              <div className="text-center mb-6">
                <div className="text-sm text-gray-500">Total Amount</div>
                <div className="text-4xl font-bold text-primary-600">${grandTotal.toFixed(2)}</div>
              </div>
              {processing && (
                <div className="text-center py-4">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600 mx-auto mb-2"></div>
                  <div className="text-sm text-gray-500">Processing payment...</div>
                </div>
              )}
              {!processing && (
                <div className="grid grid-cols-2 gap-3">
                  {paymentMethods.length > 0 ? (
                    paymentMethods.map(pm => (
                      <button
                        key={pm.id}
                        onClick={() => processPayment(pm.id, pm.name)}
                        className="p-4 border-2 rounded-xl hover:border-primary-500 hover:bg-primary-50 transition"
                      >
                        <div className="flex items-center justify-center gap-2">
                          {pm.type === 'Cash' ? <DollarSign size={20} /> : <CreditCard size={20} />}
                          <span className="font-medium">{pm.name}</span>
                        </div>
                      </button>
                    ))
                  ) : (
                    <>
                      <button
                        onClick={() => processPayment(null, 'Cash')}
                        className="p-4 border-2 rounded-xl hover:border-primary-500 hover:bg-primary-50 transition"
                      >
                        <div className="flex items-center justify-center gap-2">
                          <DollarSign size={20} />
                          <span className="font-medium">Cash</span>
                        </div>
                      </button>
                      <button
                        onClick={() => processPayment(null, 'Card')}
                        className="p-4 border-2 rounded-xl hover:border-primary-500 hover:bg-primary-50 transition"
                      >
                        <div className="flex items-center justify-center gap-2">
                          <CreditCard size={20} />
                          <span className="font-medium">Card</span>
                        </div>
                      </button>
                    </>
                  )}
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Customer Modal */}
      {showCustomerModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-900 rounded-xl shadow-xl w-full max-w-md mx-4">
            <div className="p-4 border-b flex justify-between items-center">
              <h3 className="font-bold text-lg">Select Customer</h3>
              <button onClick={() => setShowCustomerModal(false)} className="text-gray-500 hover:text-gray-700">
                <X size={20} />
              </button>
            </div>
            <div className="p-4 max-h-96 overflow-auto">
              <button
                onClick={() => {
                  setSelectedCustomer(null)
                  setShowCustomerModal(false)
                }}
                className="w-full p-3 text-left hover:bg-gray-800/50 rounded-lg mb-2 text-gray-500"
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
                  className={`w-full p-3 text-left hover:bg-gray-800/50 rounded-lg border mb-2 ${
                    selectedCustomer?.id === c.id ? 'border-primary-500 bg-primary-50' : 'border-gray-100'
                  }`}
                >
                  <div className="font-medium">{c.name}</div>
                  <div className="text-sm text-gray-500">{c.phone}</div>
                </button>
              ))}
            </div>
          </div>
        </div>
      )}

      {/* Line Discount Modal */}
      {showLineDiscountModal && selectedLineForDiscount && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-900 rounded-xl shadow-xl w-full max-w-sm mx-4">
            <div className="p-4 border-b flex justify-between items-center">
              <h3 className="font-bold text-lg">Line Discount</h3>
              <button onClick={() => setShowLineDiscountModal(false)} className="text-gray-500 hover:text-gray-700">
                <X size={20} />
              </button>
            </div>
            <div className="p-4">
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">Discount Percentage</label>
                <div className="flex items-center gap-2">
                  <input
                    type="number"
                    min="0"
                    max="100"
                    value={lineDiscountInput}
                    onChange={(e) => setLineDiscountInput(parseFloat(e.target.value) || 0)}
                    className="flex-1 border rounded-lg px-3 py-2 text-lg"
                    autoFocus
                  />
                  <span className="text-lg">%</span>
                </div>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => setShowLineDiscountModal(false)}
                  className="flex-1 py-2 border rounded-lg hover:bg-gray-800/50"
                >
                  Cancel
                </button>
                <button
                  onClick={applyLineDiscount}
                  className="flex-1 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700"
                >
                  Apply
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
