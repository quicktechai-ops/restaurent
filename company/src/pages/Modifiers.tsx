import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { modifiersApi } from '../lib/api'
import { Plus, Edit, Trash2, Sliders } from 'lucide-react'

export default function Modifiers() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ name: '', nameAr: '', description: '', extraPrice: 0, currencyCode: 'USD' })

  const { data: modifiers, isLoading } = useQuery({ queryKey: ['modifiers'], queryFn: () => modifiersApi.getAll() })

  const createMutation = useMutation({ mutationFn: modifiersApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['modifiers'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => modifiersApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['modifiers'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: modifiersApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['modifiers'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', nameAr: '', description: '', extraPrice: 0, currencyCode: 'USD' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Modifiers</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Modifier</button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Modifier' : 'Add Modifier'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">Name *</label><input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required placeholder="Extra Cheese, No Onions..." /></div>
            <div><label className="label">Arabic Name</label><input className="input" value={formData.nameAr} onChange={e => setFormData({...formData, nameAr: e.target.value})} /></div>
            <div><label className="label">Extra Price</label><input type="number" step="0.01" className="input" value={formData.extraPrice} onChange={e => setFormData({...formData, extraPrice: parseFloat(e.target.value) || 0})} /></div>
            <div><label className="label">Currency</label>
              <select className="input" value={formData.currencyCode} onChange={e => setFormData({...formData, currencyCode: e.target.value})}>
                <option value="USD">USD</option><option value="EUR">EUR</option><option value="LBP">LBP</option>
              </select>
            </div>
            <div className="md:col-span-2"><label className="label">Description</label><input className="input" value={formData.description} onChange={e => setFormData({...formData, description: e.target.value})} /></div>
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
            <thead><tr><th>Name</th><th>Arabic</th><th>Extra Price</th><th>Actions</th></tr></thead>
            <tbody>
              {modifiers?.data?.map((mod: any) => (
                <tr key={mod.id}>
                  <td className="font-medium flex items-center gap-2"><Sliders size={18} className="text-primary-600" />{mod.name}</td>
                  <td>{mod.nameAr || '-'}</td>
                  <td>{mod.extraPrice > 0 ? `${mod.currencyCode} ${mod.extraPrice.toFixed(2)}` : 'Free'}</td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(mod.id); setFormData({ name: mod.name, nameAr: mod.nameAr || '', description: mod.description || '', extraPrice: mod.extraPrice, currencyCode: mod.currencyCode }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm('Delete?') && deleteMutation.mutate(mod.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
