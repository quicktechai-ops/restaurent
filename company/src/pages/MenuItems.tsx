import { useState, useRef } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { menuItemsApi, categoriesApi, uploadApi } from '../lib/api'
import api from '../lib/api'
import { Plus, Edit, Trash2, UtensilsCrossed, Search, X, Upload, Loader2 } from 'lucide-react'

interface ItemSize { name: string; price: number; cost: number }

export default function MenuItems() {
  const queryClient = useQueryClient()
  const [search, setSearch] = useState('')
  const [categoryFilter, setCategoryFilter] = useState<number | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ 
    name: '', nameAr: '', code: '', description: '', categoryId: 0, 
    defaultPrice: 0, currencyCode: 'USD', taxIncluded: false, 
    allowSizes: false, sizes: [] as ItemSize[], modifierIds: [] as number[],
    kitchenStationId: '', commissionPerUnit: 0, imageUrl: ''
  })
  const [newSize, setNewSize] = useState({ name: '', price: 0, cost: 0 })
  const [uploading, setUploading] = useState(false)
  const fileInputRef = useRef<HTMLInputElement>(null)
  const [showCategoryForm, setShowCategoryForm] = useState(false)
  const [newCategoryName, setNewCategoryName] = useState('')
  const [showStationForm, setShowStationForm] = useState(false)
  const [newStationName, setNewStationName] = useState('')

  const { data: items, isLoading } = useQuery({ queryKey: ['menuItems', search, categoryFilter], queryFn: () => menuItemsApi.getAll({ search, categoryId: categoryFilter || undefined }) })
  const { data: categories } = useQuery({ queryKey: ['categories'], queryFn: () => categoriesApi.getAll(true) })
  const { data: kitchenStations } = useQuery({ queryKey: ['kitchen-stations'], queryFn: () => api.get('/api/company/kitchen-stations').then(r => r.data) })

  const createMutation = useMutation({ mutationFn: menuItemsApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['menuItems'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => menuItemsApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['menuItems'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: menuItemsApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['menuItems'] }) })
  const createCategoryMutation = useMutation({ 
    mutationFn: (name: string) => categoriesApi.create({ name, isActive: true }), 
    onSuccess: () => { 
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      setShowCategoryForm(false)
      setNewCategoryName('')
    } 
  })
  const createStationMutation = useMutation({ 
    mutationFn: (name: string) => api.post('/api/company/kitchen-stations', { name, isActive: true }), 
    onSuccess: () => { 
      queryClient.invalidateQueries({ queryKey: ['kitchen-stations'] })
      setShowStationForm(false)
      setNewStationName('')
    } 
  })

  const resetForm = () => { 
    setShowForm(false); setEditingId(null); setNewSize({ name: '', price: 0, cost: 0 })
    setFormData({ name: '', nameAr: '', code: '', description: '', categoryId: 0, defaultPrice: 0, currencyCode: 'USD', taxIncluded: false, allowSizes: false, sizes: [], modifierIds: [], kitchenStationId: '', commissionPerUnit: 0, imageUrl: '' }) 
  }

  const addSize = () => {
    if (!newSize.name) return
    setFormData({ ...formData, sizes: [...formData.sizes, newSize] })
    setNewSize({ name: '', price: 0, cost: 0 })
  }

  const removeSize = (index: number) => {
    setFormData({ ...formData, sizes: formData.sizes.filter((_, i) => i !== index) })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const payload = {
      ...formData,
      kitchenStationId: formData.kitchenStationId ? parseInt(formData.kitchenStationId) : null,
      sizes: formData.sizes.map(s => ({
        sizeName: s.name,
        price: s.price,
        cost: s.cost,
        currencyCode: formData.currencyCode
      })),
      imageUrl: formData.imageUrl || null
    }
    if (editingId) updateMutation.mutate({ id: editingId, data: payload })
    else createMutation.mutate(payload)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Menu Items</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Item</button>
      </div>

      <div className="card p-4 mb-6 flex gap-4">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          <input className="input pl-10" placeholder="Search items..." value={search} onChange={e => setSearch(e.target.value)} />
        </div>
        <select className="input w-48" value={categoryFilter || ''} onChange={e => setCategoryFilter(e.target.value ? parseInt(e.target.value) : null)}>
          <option value="">All Categories</option>
          {categories?.data?.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Item' : 'Add Item'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">Name *</label><input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required /></div>
            <div><label className="label">Arabic Name</label><input className="input" value={formData.nameAr} onChange={e => setFormData({...formData, nameAr: e.target.value})} /></div>
            <div><label className="label">Code</label><input className="input" value={formData.code} onChange={e => setFormData({...formData, code: e.target.value})} /></div>
            <div><label className="label">Category *</label>
              {showCategoryForm ? (
                <div className="flex gap-2">
                  <input type="text" placeholder="New category name..." value={newCategoryName} onChange={e => setNewCategoryName(e.target.value)} className="input flex-1" autoFocus />
                  <button type="button" onClick={() => newCategoryName.trim() && createCategoryMutation.mutate(newCategoryName.trim())} disabled={createCategoryMutation.isPending} className="px-3 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">{createCategoryMutation.isPending ? '...' : '✓'}</button>
                  <button type="button" onClick={() => { setShowCategoryForm(false); setNewCategoryName('') }} className="px-3 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300">✕</button>
                </div>
              ) : (
                <div className="flex gap-2">
                  <select className="input flex-1" value={formData.categoryId} onChange={e => setFormData({...formData, categoryId: parseInt(e.target.value)})} required>
                    <option value="">-- Select --</option>
                    {categories?.data?.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                  <button type="button" onClick={() => setShowCategoryForm(true)} className="px-3 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700" title="Add new category"><Plus size={20} /></button>
                </div>
              )}
            </div>
            <div><label className="label">Price *</label><input type="number" step="0.01" className="input" value={formData.defaultPrice} onChange={e => setFormData({...formData, defaultPrice: parseFloat(e.target.value) || 0})} required /></div>
            <div><label className="label">Currency</label>
              <select className="input" value={formData.currencyCode} onChange={e => setFormData({...formData, currencyCode: e.target.value})}>
                <option value="USD">USD</option><option value="EUR">EUR</option><option value="LBP">LBP</option>
              </select>
            </div>
            <div><label className="label">Kitchen Station</label>
              {showStationForm ? (
                <div className="flex gap-2">
                  <input type="text" placeholder="New station name..." value={newStationName} onChange={e => setNewStationName(e.target.value)} className="input flex-1" autoFocus />
                  <button type="button" onClick={() => newStationName.trim() && createStationMutation.mutate(newStationName.trim())} disabled={createStationMutation.isPending} className="px-3 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">{createStationMutation.isPending ? '...' : '✓'}</button>
                  <button type="button" onClick={() => { setShowStationForm(false); setNewStationName('') }} className="px-3 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300">✕</button>
                </div>
              ) : (
                <div className="flex gap-2">
                  <select className="input flex-1" value={formData.kitchenStationId} onChange={e => setFormData({...formData, kitchenStationId: e.target.value})}>
                    <option value="">-- None --</option>
                    {kitchenStations?.map((s: any) => <option key={s.id} value={s.id}>{s.name}</option>)}
                  </select>
                  <button type="button" onClick={() => setShowStationForm(true)} className="px-3 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700" title="Add new station"><Plus size={20} /></button>
                </div>
              )}
            </div>
            <div><label className="label">Commission per Unit ($)</label>
              <input type="number" step="0.01" className="input" value={formData.commissionPerUnit} onChange={e => setFormData({...formData, commissionPerUnit: parseFloat(e.target.value) || 0})} />
            </div>
            <div className="md:col-span-2"><label className="label">Description</label><textarea className="input" rows={2} value={formData.description} onChange={e => setFormData({...formData, description: e.target.value})} /></div>
            
            {/* Image Upload */}
            <div className="md:col-span-2">
              <label className="label flex items-center gap-2"><Upload size={16} /> Item Image</label>
              <div className="flex items-center gap-4">
                <input
                  type="file"
                  ref={fileInputRef}
                  accept="image/jpeg,image/png,image/gif,image/webp"
                  className="hidden"
                  onChange={async (e) => {
                    const file = e.target.files?.[0]
                    if (!file) return
                    setUploading(true)
                    try {
                      const res = await uploadApi.uploadImage(file)
                      setFormData({...formData, imageUrl: res.data.url})
                    } catch (err: any) {
                      alert(err.response?.data?.message || 'Failed to upload image')
                    } finally {
                      setUploading(false)
                    }
                  }}
                />
                <button
                  type="button"
                  onClick={() => fileInputRef.current?.click()}
                  disabled={uploading}
                  className="btn-secondary flex items-center gap-2"
                >
                  {uploading ? <Loader2 size={16} className="animate-spin" /> : <Upload size={16} />}
                  {uploading ? 'Uploading...' : 'Upload Image'}
                </button>
                {formData.imageUrl && (
                  <div className="relative">
                    <img src={formData.imageUrl} alt="Preview" className="h-16 w-16 object-cover rounded-lg border" />
                    <button
                      type="button"
                      onClick={() => setFormData({...formData, imageUrl: ''})}
                      className="absolute -top-2 -right-2 w-5 h-5 bg-red-500 text-white rounded-full flex items-center justify-center hover:bg-red-600"
                    >
                      <X size={12} />
                    </button>
                  </div>
                )}
              </div>
            </div>

            <div className="flex items-center gap-4">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.taxIncluded} onChange={e => setFormData({...formData, taxIncluded: e.target.checked})} /> Tax Included</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.allowSizes} onChange={e => setFormData({...formData, allowSizes: e.target.checked})} /> Allow Sizes</label>
            </div>
            
            {/* Sizes Section */}
            {formData.allowSizes && (
              <div className="md:col-span-2 border rounded-lg p-4 bg-gray-800">
                <h3 className="font-medium mb-3">Item Sizes</h3>
                <div className="flex gap-2 mb-3">
                  <input type="text" placeholder="Size Name (e.g. Small)" className="input flex-1" value={newSize.name} onChange={e => setNewSize({...newSize, name: e.target.value})} />
                  <input type="number" step="0.01" placeholder="Price" className="input w-24" value={newSize.price || ''} onChange={e => setNewSize({...newSize, price: parseFloat(e.target.value) || 0})} />
                  <input type="number" step="0.01" placeholder="Cost" className="input w-24" value={newSize.cost || ''} onChange={e => setNewSize({...newSize, cost: parseFloat(e.target.value) || 0})} />
                  <button type="button" onClick={addSize} className="btn-primary px-3"><Plus size={18} /></button>
                </div>
                {formData.sizes.length > 0 && (
                  <table className="w-full text-sm">
                    <thead><tr className="border-b border-gray-700"><th className="text-left py-2">Size</th><th className="text-left py-2">Price</th><th className="text-left py-2">Cost</th><th></th></tr></thead>
                    <tbody>
                      {formData.sizes.map((size, idx) => (
                        <tr key={idx} className="border-b border-gray-700">
                          <td className="py-2">{size.name}</td>
                          <td className="py-2">${size.price.toFixed(2)}</td>
                          <td className="py-2">${size.cost.toFixed(2)}</td>
                          <td className="py-2"><button type="button" onClick={() => removeSize(idx)} className="text-red-600"><X size={16} /></button></td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
                {formData.sizes.length === 0 && <p className="text-sm text-gray-500">No sizes added yet</p>}
              </div>
            )}
            
            <div className="md:col-span-2 flex gap-3">
              <button type="submit" className="btn-primary">{editingId ? 'Update' : 'Create'}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        {isLoading ? <div className="p-8 text-center">Loading...</div> : (
          <table className="table">
            <thead><tr><th>Name</th><th>Code</th><th>Category</th><th>Price</th><th>Status</th><th>Actions</th></tr></thead>
            <tbody>
              {items?.data?.map((item: any) => (
                <tr key={item.id}>
                  <td className="font-medium flex items-center gap-2"><UtensilsCrossed size={18} className="text-primary-600" />{item.name}</td>
                  <td>{item.code || '-'}</td>
                  <td>{item.categoryName}</td>
                  <td>{item.currencyCode} {item.defaultPrice.toFixed(2)}</td>
                  <td><span className={`px-2 py-1 rounded-full text-xs ${item.isAvailable ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>{item.isAvailable ? 'Available' : '86\'d'}</span></td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(item.id); setFormData({ name: item.name, nameAr: item.nameAr || '', code: item.code || '', description: item.description || '', categoryId: item.categoryId, defaultPrice: item.defaultPrice, currencyCode: item.currencyCode, taxIncluded: item.taxIncluded, allowSizes: item.allowSizes, sizes: item.sizes || [], modifierIds: [], kitchenStationId: item.kitchenStationId?.toString() || '', commissionPerUnit: item.commissionPerUnit || 0, imageUrl: item.imageUrl || '' }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm('Delete?') && deleteMutation.mutate(item.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
