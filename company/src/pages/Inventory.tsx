import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { inventoryApi } from '../lib/api'
import api from '../lib/api'
import { Plus, Edit, Trash2, Package, X, Box, Hash, Scale, FolderOpen, TrendingDown, RotateCcw, Calculator, DollarSign, Layers, ArrowUpDown } from 'lucide-react'

export default function Inventory() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [search, setSearch] = useState('')
  const [formData, setFormData] = useState({ name: '', code: '', unitOfMeasure: '', category: '', minLevel: 0, reorderQty: 0, costMethod: 'Average', quantity: 0, cost: 0, currencyCode: '' })
  const [showCategoryForm, setShowCategoryForm] = useState(false)
  const [newCategoryName, setNewCategoryName] = useState('')
  const [wastePct, setWastePct] = useState(0)
  const [adjustItem, setAdjustItem] = useState<any>(null)
  const [adjustData, setAdjustData] = useState({ type: 'increase', quantity: '', reason: '' })

  const { data: items, isLoading } = useQuery({ queryKey: ['inventory', search], queryFn: () => inventoryApi.getAll({ search: search || undefined }) })
  const { data: categories } = useQuery({ queryKey: ['inventory-categories'], queryFn: () => api.get('/api/company/inventory-settings/categories').then(r => r.data) })
  const { data: units } = useQuery({ queryKey: ['inventory-units'], queryFn: () => api.get('/api/company/inventory-settings/units').then(r => r.data) })
  const { data: currencies } = useQuery({ queryKey: ['currencies'], queryFn: () => api.get('/api/company/currencies').then(r => r.data) })

  const createMutation = useMutation({ mutationFn: inventoryApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => inventoryApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: inventoryApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory'] }) })
  const toggleMutation = useMutation({ mutationFn: inventoryApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory'] }) })
  const adjustMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/stock-adjustments', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory'] }); setAdjustItem(null); setAdjustData({ type: 'increase', quantity: '', reason: '' }) }
  })
  const createCategoryMutation = useMutation({ 
    mutationFn: (name: string) => api.post('/api/company/inventory-settings/categories', { name, isActive: true }), 
    onSuccess: () => { 
      queryClient.invalidateQueries({ queryKey: ['inventory-categories'] })
      setShowCategoryForm(false)
      setNewCategoryName('')
    } 
  })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', code: '', unitOfMeasure: '', category: '', minLevel: 0, reorderQty: 0, costMethod: 'Average', quantity: 0, cost: 0, currencyCode: '' }); setWastePct(0) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const submitData = { ...formData }
    if (wastePct > 0) {
      submitData.quantity = formData.quantity * (1 - wastePct / 100)
    }
    if (editingId) updateMutation.mutate({ id: editingId, data: submitData })
    else createMutation.mutate(submitData)
  }

  const openEditModal = (item: any) => {
    setEditingId(item.id)
    setFormData({
      name: item.name,
      code: item.code || '',
      unitOfMeasure: item.unitOfMeasure,
      category: item.category || '',
      minLevel: item.minLevel,
      reorderQty: item.reorderQty,
      costMethod: item.costMethod,
      quantity: item.quantity || 0,
      cost: item.cost || 0,
      currencyCode: item.currencyCode || ''
    })
    setShowForm(true)
  }

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{t('inventory.title')}</h1>
          <p className="text-sm text-gray-500">{t('inventory.manageItems')}</p>
        </div>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> {t('inventory.addItem')}
        </button>
      </div>

      <div className="mb-4">
        <input type="text" placeholder={t('common.search')} value={search} onChange={(e) => setSearch(e.target.value)} className="input w-full max-w-md" />
      </div>

      {/* Modal Overlay */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4" onClick={resetForm}>
          <div 
            className="bg-gray-900 rounded-2xl shadow-2xl w-full max-w-2xl transform transition-all animate-in fade-in zoom-in duration-200"
            onClick={e => e.stopPropagation()}
          >
            {/* Modal Header */}
            <div className="flex items-center justify-between p-6 border-b border-gray-700">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-primary-100 rounded-xl flex items-center justify-center">
                  <Package className="w-5 h-5 text-primary-600" />
                </div>
                <div>
                  <h2 className="text-xl font-semibold text-gray-900">{editingId ? t('inventory.editItem') : t('inventory.addItem')}</h2>
                  <p className="text-sm text-gray-500">{editingId ? t('inventory.updateDetails') : t('inventory.addNewIngredient')}</p>
                </div>
              </div>
              <button onClick={resetForm} className="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <X size={20} className="text-gray-500" />
              </button>
            </div>

            {/* Modal Body */}
            <form onSubmit={handleSubmit} className="p-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
                {/* Name Field */}
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Box size={14} className="inline mr-2" />{t('inventory.itemName')} *
                  </label>
                  <input 
                    type="text" 
                    placeholder={t('inventory.itemNameExample')} 
                    value={formData.name} 
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                    required 
                  />
                </div>

                {/* Code Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Hash size={14} className="inline mr-2" />{t('inventory.itemCode')}
                  </label>
                  <input 
                    type="text" 
                    placeholder={t('inventory.itemCodeExample')} 
                    value={formData.code} 
                    onChange={(e) => setFormData({ ...formData, code: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                </div>

                {/* Unit of Measure Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Scale size={14} className="inline mr-2" />{t('inventory.unitOfMeasure')} *
                  </label>
                  <select 
                    value={formData.unitOfMeasure} 
                    onChange={(e) => setFormData({ ...formData, unitOfMeasure: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                    required
                  >
                    <option value="">{t('inventory.selectUnit')}</option>
                    {units?.filter((u: any) => u.isActive).map((unit: any) => (
                      <option key={unit.id} value={unit.code}>{unit.name} ({unit.code})</option>
                    ))}
                    {(!units || units.length === 0) && (
                      <>
                        <option value="kg">Kilogram (kg)</option>
                        <option value="g">Gram (g)</option>
                        <option value="liter">Liter (L)</option>
                        <option value="ml">Milliliter (ml)</option>
                        <option value="pcs">Pieces (pcs)</option>
                      </>
                    )}
                  </select>
                </div>

                {/* Category Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <FolderOpen size={14} className="inline mr-2" />{t('inventory.category')}
                  </label>
                  {showCategoryForm ? (
                    <div className="flex gap-2">
                      <input
                        type="text"
                        placeholder={t('inventory.newCategoryName')}
                        value={newCategoryName}
                        onChange={(e) => setNewCategoryName(e.target.value)}
                        className="flex-1 px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                        autoFocus
                      />
                      <button
                        type="button"
                        onClick={() => {
                          if (newCategoryName.trim()) {
                            createCategoryMutation.mutate(newCategoryName.trim())
                          }
                        }}
                        disabled={createCategoryMutation.isPending}
                        className="px-3 py-2 bg-green-600 text-white rounded-xl hover:bg-green-700 transition-colors"
                      >
                        {createCategoryMutation.isPending ? '...' : '✓'}
                      </button>
                      <button
                        type="button"
                        onClick={() => { setShowCategoryForm(false); setNewCategoryName('') }}
                        className="px-3 py-2 bg-gray-200 text-gray-700 rounded-xl hover:bg-gray-300 transition-colors"
                      >
                        ✕
                      </button>
                    </div>
                  ) : (
                    <div className="flex gap-2">
                      <select 
                        value={formData.category} 
                        onChange={(e) => setFormData({ ...formData, category: e.target.value })} 
                        className="flex-1 px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                      >
                        <option value="">{t('inventory.selectCategory')}</option>
                        {categories?.filter((c: any) => c.isActive).map((cat: any) => (
                          <option key={cat.id} value={cat.name}>{cat.name}</option>
                        ))}
                      </select>
                      <button
                        type="button"
                        onClick={() => setShowCategoryForm(true)}
                        className="px-3 py-2 bg-primary-600 text-white rounded-xl hover:bg-primary-700 transition-colors"
                        title={t('menu.addNewCategory')}
                      >
                        <Plus size={20} />
                      </button>
                    </div>
                  )}
                </div>

                {/* Cost Method Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Calculator size={14} className="inline mr-2" />{t('inventory.costMethod')}
                  </label>
                  <select 
                    value={formData.costMethod} 
                    onChange={(e) => setFormData({ ...formData, costMethod: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                  >
                    <option value="Average">{t('inventory.averageCost')}</option>
                    <option value="Last">{t('inventory.lastCost')}</option>
                  </select>
                </div>

                {/* Min Level Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <TrendingDown size={14} className="inline mr-2" />{t('inventory.minimumLevel')}
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.minLevel} 
                    onChange={(e) => setFormData({ ...formData, minLevel: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">{t('inventory.minLevelHint')}</p>
                </div>

                {/* Reorder Qty Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <RotateCcw size={14} className="inline mr-2" />{t('inventory.reorderQuantity')}
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.reorderQty} 
                    onChange={(e) => setFormData({ ...formData, reorderQty: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">{t('inventory.reorderHint')}</p>
                </div>

                {/* Current Quantity Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Layers size={14} className="inline mr-2" />{t('inventory.currentQuantity')}
                  </label>
                  <div className="flex gap-2">
                    <input 
                      type="number" 
                      step="0.01" 
                      placeholder="0.00" 
                      value={formData.quantity} 
                      onChange={(e) => setFormData({ ...formData, quantity: parseFloat(e.target.value) || 0 })} 
                      className="flex-1 px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                    />
                    <select
                      value={wastePct}
                      onChange={(e) => setWastePct(parseFloat(e.target.value))}
                      className="w-28 px-2 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all text-sm"
                    >
                      <option value={0}>{t('inventory.noLoss')}</option>
                      <option value={1}>-1%</option>
                      <option value={2}>-2%</option>
                      <option value={3}>-3%</option>
                      <option value={5}>-5%</option>
                      <option value={8}>-8%</option>
                      <option value={10}>-10%</option>
                      <option value={15}>-15%</option>
                      <option value={20}>-20%</option>
                      <option value={25}>-25%</option>
                      <option value={30}>-30%</option>
                    </select>
                  </div>
                  <p className="text-xs text-gray-400 mt-1">{t('inventory.currentStockHint')}</p>
                  {wastePct > 0 && formData.quantity > 0 && (
                    <p className="text-xs text-yellow-400 mt-1">
                      {t('inventory.afterLoss')}: <span className="font-semibold">{(formData.quantity * (1 - wastePct / 100)).toFixed(2)}</span> ({t('inventory.loss')}: {(formData.quantity * wastePct / 100).toFixed(2)})
                    </p>
                  )}
                </div>

                {/* Cost Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <DollarSign size={14} className="inline mr-2" />{t('inventory.unitCost')}
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.cost} 
                    onChange={(e) => setFormData({ ...formData, cost: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">{t('inventory.costPerUnit')}</p>
                </div>

                {/* Currency Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <DollarSign size={14} className="inline mr-2" />{t('inventory.currency')}
                  </label>
                  <select 
                    value={formData.currencyCode} 
                    onChange={(e) => setFormData({ ...formData, currencyCode: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                  >
                    <option value="">{t('inventory.selectCurrency')}</option>
                    {currencies?.filter((c: any) => c.isActive).map((currency: any) => (
                      <option key={currency.currencyCode} value={currency.currencyCode}>
                        {currency.currencyCode} - {currency.name} {currency.isDefault && '(Default)'}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              {/* Modal Footer */}
              <div className="flex items-center justify-end gap-3 mt-8 pt-6 border-t border-gray-100">
                <button 
                  type="button" 
                  onClick={resetForm} 
                  className="px-6 py-2.5 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-xl font-medium transition-colors"
                >
                  {t('common.cancel')}
                </button>
                <button 
                  type="submit" 
                  disabled={createMutation.isPending || updateMutation.isPending}
                  className="px-6 py-2.5 bg-primary-600 hover:bg-primary-700 text-white rounded-xl font-medium transition-colors flex items-center gap-2 disabled:opacity-50"
                >
                  {(createMutation.isPending || updateMutation.isPending) && (
                    <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  )}
                  {editingId ? t('inventory.updateItem') : t('inventory.addItem')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="card">
        <table className="table">
          <thead>
            <tr className="border-b border-gray-700">
              <th className="text-left p-3">{t('common.name')}</th>
              <th className="text-left p-3">{t('inventory.code')}</th>
              <th className="text-left p-3">{t('inventory.unit')}</th>
              <th className="text-left p-3">{t('inventory.category')}</th>
              <th className="text-right p-3">{t('inventory.qty')}</th>
              <th className="text-right p-3">{t('inventory.cost')}</th>
              <th className="text-right p-3">{t('inventory.minLevel')}</th>
              <th className="text-right p-3">{t('inventory.reorder')}</th>
              <th className="text-left p-3">{t('common.status')}</th>
              <th className="text-left p-3">{t('common.actions')}</th>
            </tr>
          </thead>
          <tbody>
            {items?.data?.map((item: any) => (
              <tr key={item.id} className={`border-b hover:bg-gray-800/50 ${item.quantity <= item.minLevel ? 'bg-red-900/20' : ''}`}>
                <td className="p-3 flex items-center gap-2"><Package size={16} className="text-gray-400" /> {item.name}</td>
                <td className="p-3">{item.code || '-'}</td>
                <td className="p-3">{item.unitOfMeasure}</td>
                <td className="p-3">{item.category || '-'}</td>
                <td className={`p-3 text-right font-medium ${item.quantity <= item.minLevel ? 'text-red-600' : ''}`}>{item.quantity ?? 0}</td>
                <td className="p-3 text-right">{item.cost ? `$${item.cost.toFixed(2)}` : '-'}</td>
                <td className="p-3 text-right">{item.minLevel}</td>
                <td className="p-3 text-right">{item.reorderQty}</td>
                <td className="p-3">
                  <button onClick={() => toggleMutation.mutate(item.id)} className={`px-2 py-1 rounded text-xs ${item.isActive ? 'bg-green-900/30 text-green-400' : 'bg-red-900/30 text-red-400'}`}>
                    {item.isActive ? t('common.active') : t('common.inactive')}
                  </button>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => { setAdjustItem(item); setAdjustData({ type: 'increase', quantity: '', reason: '' }) }} className="text-yellow-500 hover:text-yellow-400" title={t('inventory.quickAdjust')}><ArrowUpDown size={16} /></button>
                    <button onClick={() => openEditModal(item)} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(item.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {items?.data?.length === 0 && <p className="text-center text-gray-500 py-8">{t('inventory.noItems')}</p>}
      </div>

      {/* Quick Stock Adjustment Dialog */}
      {adjustItem && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4" onClick={() => setAdjustItem(null)}>
          <div className="bg-gray-900 rounded-2xl shadow-2xl w-full max-w-sm" onClick={e => e.stopPropagation()}>
            <div className="flex items-center justify-between p-4 border-b border-gray-700">
              <div>
                <h3 className="font-semibold">{t('inventory.quickAdjust')}</h3>
                <p className="text-sm text-gray-400">{adjustItem.name} ({adjustItem.quantity} {adjustItem.unitOfMeasure})</p>
              </div>
              <button onClick={() => setAdjustItem(null)} className="p-1 hover:bg-gray-800 rounded-lg"><X size={18} /></button>
            </div>
            <form onSubmit={(e) => { e.preventDefault(); const qty = parseFloat(adjustData.quantity); if (!qty || qty <= 0) return; adjustMutation.mutate({ inventoryItemId: adjustItem.id, adjustmentType: adjustData.type, quantity: qty, reason: adjustData.reason || 'Quick adjustment' }) }} className="p-4 space-y-4">
              <div className="flex gap-2">
                <button type="button" onClick={() => setAdjustData({ ...adjustData, type: 'increase' })} className={`flex-1 py-2 rounded-xl font-medium text-sm transition-colors ${adjustData.type === 'increase' ? 'bg-green-600 text-white' : 'bg-gray-800 text-gray-400'}`}>
                  + {t('inventory.addStock')}
                </button>
                <button type="button" onClick={() => setAdjustData({ ...adjustData, type: 'decrease' })} className={`flex-1 py-2 rounded-xl font-medium text-sm transition-colors ${adjustData.type === 'decrease' ? 'bg-red-600 text-white' : 'bg-gray-800 text-gray-400'}`}>
                  - {t('inventory.deductStock')}
                </button>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('inventory.qty')}</label>
                <input type="number" step="0.01" min="0.01" value={adjustData.quantity} onChange={(e) => setAdjustData({ ...adjustData, quantity: e.target.value })} className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" placeholder="0.00" required autoFocus />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('inventory.reason')}</label>
                <input type="text" value={adjustData.reason} onChange={(e) => setAdjustData({ ...adjustData, reason: e.target.value })} className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" placeholder={t('inventory.reasonPlaceholder')} />
              </div>
              {adjustData.quantity && parseFloat(adjustData.quantity) > 0 && (
                <p className="text-sm text-gray-400">
                  {adjustItem.quantity} {adjustData.type === 'increase' ? '+' : '-'} {adjustData.quantity} = <span className="font-semibold text-white">{adjustData.type === 'increase' ? (adjustItem.quantity + parseFloat(adjustData.quantity)).toFixed(2) : (adjustItem.quantity - parseFloat(adjustData.quantity)).toFixed(2)} {adjustItem.unitOfMeasure}</span>
                </p>
              )}
              <button type="submit" disabled={adjustMutation.isPending} className={`w-full py-2.5 rounded-xl font-medium transition-colors ${adjustData.type === 'increase' ? 'bg-green-600 hover:bg-green-700' : 'bg-red-600 hover:bg-red-700'} text-white disabled:opacity-50`}>
                {adjustMutation.isPending ? '...' : adjustData.type === 'increase' ? `+ ${t('inventory.addStock')}` : `- ${t('inventory.deductStock')}`}
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
