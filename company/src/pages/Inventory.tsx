import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { inventoryApi } from '../lib/api'
import api from '../lib/api'
import { Plus, Edit, Trash2, Package, X, Box, Hash, Scale, FolderOpen, TrendingDown, RotateCcw, Calculator, DollarSign, Layers } from 'lucide-react'

export default function Inventory() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [search, setSearch] = useState('')
  const [formData, setFormData] = useState({ name: '', code: '', unitOfMeasure: '', category: '', minLevel: 0, reorderQty: 0, costMethod: 'Average', quantity: 0, cost: 0, currencyCode: '' })

  const { data: items, isLoading } = useQuery({ queryKey: ['inventory', search], queryFn: () => inventoryApi.getAll({ search: search || undefined }) })
  const { data: categories } = useQuery({ queryKey: ['inventory-categories'], queryFn: () => api.get('/api/company/inventory-settings/categories').then(r => r.data) })
  const { data: units } = useQuery({ queryKey: ['inventory-units'], queryFn: () => api.get('/api/company/inventory-settings/units').then(r => r.data) })
  const { data: currencies } = useQuery({ queryKey: ['currencies'], queryFn: () => api.get('/api/company/currencies').then(r => r.data) })

  const createMutation = useMutation({ mutationFn: inventoryApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => inventoryApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: inventoryApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory'] }) })
  const toggleMutation = useMutation({ mutationFn: inventoryApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', code: '', unitOfMeasure: '', category: '', minLevel: 0, reorderQty: 0, costMethod: 'Average', quantity: 0, cost: 0, currencyCode: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
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

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">Inventory Items</h1>
          <p className="text-sm text-gray-500">Manage your ingredients and stock items</p>
        </div>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Item
        </button>
      </div>

      <div className="mb-4">
        <input type="text" placeholder="Search items..." value={search} onChange={(e) => setSearch(e.target.value)} className="input-field w-full max-w-md" />
      </div>

      {/* Modal Overlay */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4" onClick={resetForm}>
          <div 
            className="bg-white rounded-2xl shadow-2xl w-full max-w-2xl transform transition-all animate-in fade-in zoom-in duration-200"
            onClick={e => e.stopPropagation()}
          >
            {/* Modal Header */}
            <div className="flex items-center justify-between p-6 border-b border-gray-100">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-primary-100 rounded-xl flex items-center justify-center">
                  <Package className="w-5 h-5 text-primary-600" />
                </div>
                <div>
                  <h2 className="text-xl font-semibold text-gray-800">{editingId ? 'Edit' : 'Add New'} Inventory Item</h2>
                  <p className="text-sm text-gray-500">{editingId ? 'Update item details' : 'Add a new ingredient to your inventory'}</p>
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
                    <Box size={14} className="inline mr-2" />Item Name *
                  </label>
                  <input 
                    type="text" 
                    placeholder="e.g., Tomatoes, Olive Oil, Chicken Breast" 
                    value={formData.name} 
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                    required 
                  />
                </div>

                {/* Code Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Hash size={14} className="inline mr-2" />Item Code
                  </label>
                  <input 
                    type="text" 
                    placeholder="e.g., TOM-001" 
                    value={formData.code} 
                    onChange={(e) => setFormData({ ...formData, code: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                </div>

                {/* Unit of Measure Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Scale size={14} className="inline mr-2" />Unit of Measure *
                  </label>
                  <select 
                    value={formData.unitOfMeasure} 
                    onChange={(e) => setFormData({ ...formData, unitOfMeasure: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                    required
                  >
                    <option value="">Select unit...</option>
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
                    <FolderOpen size={14} className="inline mr-2" />Category
                  </label>
                  <select 
                    value={formData.category} 
                    onChange={(e) => setFormData({ ...formData, category: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                  >
                    <option value="">Select category...</option>
                    {categories?.filter((c: any) => c.isActive).map((cat: any) => (
                      <option key={cat.id} value={cat.name}>{cat.name}</option>
                    ))}
                  </select>
                </div>

                {/* Cost Method Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Calculator size={14} className="inline mr-2" />Cost Method
                  </label>
                  <select 
                    value={formData.costMethod} 
                    onChange={(e) => setFormData({ ...formData, costMethod: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                  >
                    <option value="Average">Average Cost</option>
                    <option value="Last">Last Cost (LIFO)</option>
                  </select>
                </div>

                {/* Min Level Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <TrendingDown size={14} className="inline mr-2" />Minimum Level
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.minLevel} 
                    onChange={(e) => setFormData({ ...formData, minLevel: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">Alert when stock falls below this level</p>
                </div>

                {/* Reorder Qty Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <RotateCcw size={14} className="inline mr-2" />Reorder Quantity
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.reorderQty} 
                    onChange={(e) => setFormData({ ...formData, reorderQty: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">Suggested quantity to reorder</p>
                </div>

                {/* Current Quantity Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <Layers size={14} className="inline mr-2" />Current Quantity
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.quantity} 
                    onChange={(e) => setFormData({ ...formData, quantity: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">Current stock on hand</p>
                </div>

                {/* Cost Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <DollarSign size={14} className="inline mr-2" />Unit Cost
                  </label>
                  <input 
                    type="number" 
                    step="0.01" 
                    placeholder="0.00" 
                    value={formData.cost} 
                    onChange={(e) => setFormData({ ...formData, cost: parseFloat(e.target.value) || 0 })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all" 
                  />
                  <p className="text-xs text-gray-400 mt-1">Cost per unit</p>
                </div>

                {/* Currency Field */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    <DollarSign size={14} className="inline mr-2" />Currency
                  </label>
                  <select 
                    value={formData.currencyCode} 
                    onChange={(e) => setFormData({ ...formData, currencyCode: e.target.value })} 
                    className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-all"
                  >
                    <option value="">Select currency...</option>
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
                  Cancel
                </button>
                <button 
                  type="submit" 
                  disabled={createMutation.isPending || updateMutation.isPending}
                  className="px-6 py-2.5 bg-primary-600 hover:bg-primary-700 text-white rounded-xl font-medium transition-colors flex items-center gap-2 disabled:opacity-50"
                >
                  {(createMutation.isPending || updateMutation.isPending) && (
                    <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  )}
                  {editingId ? 'Update Item' : 'Add Item'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="card">
        <table className="w-full">
          <thead>
            <tr className="border-b">
              <th className="text-left p-3">Name</th>
              <th className="text-left p-3">Code</th>
              <th className="text-left p-3">Unit</th>
              <th className="text-left p-3">Category</th>
              <th className="text-left p-3">Min Level</th>
              <th className="text-left p-3">Reorder Qty</th>
              <th className="text-left p-3">Cost Method</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items?.data?.map((item: any) => (
              <tr key={item.id} className="border-b hover:bg-gray-50">
                <td className="p-3 flex items-center gap-2"><Package size={16} className="text-gray-400" /> {item.name}</td>
                <td className="p-3">{item.code || '-'}</td>
                <td className="p-3">{item.unitOfMeasure}</td>
                <td className="p-3">{item.category || '-'}</td>
                <td className="p-3">{item.minLevel}</td>
                <td className="p-3">{item.reorderQty}</td>
                <td className="p-3">{item.costMethod}</td>
                <td className="p-3">
                  <button onClick={() => toggleMutation.mutate(item.id)} className={`px-2 py-1 rounded text-xs ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {item.isActive ? 'Active' : 'Inactive'}
                  </button>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => openEditModal(item)} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(item.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {items?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No inventory items found</p>}
      </div>
    </div>
  )
}
