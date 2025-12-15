import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriesApi } from '../lib/api'
import { Plus, Edit, Trash2, FolderTree } from 'lucide-react'

export default function Categories() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ name: '', nameAr: '', parentCategoryId: null as number | null, sortOrder: 0 })

  const { data: categories, isLoading } = useQuery({ queryKey: ['categories'], queryFn: () => categoriesApi.getAll() })

  const createMutation = useMutation({ mutationFn: categoriesApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['categories'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => categoriesApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['categories'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: categoriesApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['categories'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', nameAr: '', parentCategoryId: null, sortOrder: 0 }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Categories</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Category</button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Category' : 'Add Category'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">Name *</label><input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required /></div>
            <div><label className="label">Arabic Name</label><input className="input" value={formData.nameAr} onChange={e => setFormData({...formData, nameAr: e.target.value})} /></div>
            <div><label className="label">Parent Category</label>
              <select className="input" value={formData.parentCategoryId || ''} onChange={e => setFormData({...formData, parentCategoryId: e.target.value ? parseInt(e.target.value) : null})}>
                <option value="">-- None --</option>
                {categories?.data?.filter((c: any) => c.id !== editingId).map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div><label className="label">Sort Order</label><input type="number" className="input" value={formData.sortOrder} onChange={e => setFormData({...formData, sortOrder: parseInt(e.target.value) || 0})} /></div>
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
            <thead><tr><th>Name</th><th>Arabic</th><th>Parent</th><th>Items</th><th>Actions</th></tr></thead>
            <tbody>
              {categories?.data?.map((cat: any) => (
                <tr key={cat.id}>
                  <td className="font-medium flex items-center gap-2"><FolderTree size={18} className="text-primary-600" />{cat.name}</td>
                  <td>{cat.nameAr || '-'}</td>
                  <td>{cat.parentCategoryName || '-'}</td>
                  <td>{cat.itemsCount}</td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(cat.id); setFormData({ name: cat.name, nameAr: cat.nameAr || '', parentCategoryId: cat.parentCategoryId, sortOrder: cat.sortOrder }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm('Delete?') && deleteMutation.mutate(cat.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
