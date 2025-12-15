import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, ToggleLeft, ToggleRight, Building2 } from 'lucide-react'

export default function Branches() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({
    name: '', code: '', country: '', city: '', address: '', phone: '',
    currencyCode: 'USD', vatPercent: 0, serviceChargePercent: 0, isActive: true
  })

  const { data: branches, isLoading } = useQuery({
    queryKey: ['branches'],
    queryFn: () => branchesApi.getAll(),
  })

  const createMutation = useMutation({
    mutationFn: branchesApi.create,
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['branches'] }); resetForm() },
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => branchesApi.update(id, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['branches'] }); resetForm() },
  })

  const deleteMutation = useMutation({
    mutationFn: branchesApi.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['branches'] }),
  })

  const toggleMutation = useMutation({
    mutationFn: branchesApi.toggle,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['branches'] }),
  })

  const resetForm = () => {
    setShowForm(false); setEditingId(null)
    setFormData({ name: '', code: '', country: '', city: '', address: '', phone: '', currencyCode: 'USD', vatPercent: 0, serviceChargePercent: 0, isActive: true })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) {
      updateMutation.mutate({ id: editingId, data: formData })
    } else {
      createMutation.mutate(formData)
    }
  }

  const handleEdit = (branch: any) => {
    setEditingId(branch.id)
    setFormData({
      name: branch.name, code: branch.code || '', country: branch.country || '',
      city: branch.city || '', address: branch.address || '', phone: branch.phone || '',
      currencyCode: branch.currencyCode, vatPercent: branch.vatPercent,
      serviceChargePercent: branch.serviceChargePercent, isActive: branch.isActive
    })
    setShowForm(true)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Branches</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Branch
        </button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Branch' : 'Add Branch'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="label">Name *</label>
              <input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required />
            </div>
            <div>
              <label className="label">Code</label>
              <input className="input" value={formData.code} onChange={e => setFormData({...formData, code: e.target.value})} />
            </div>
            <div>
              <label className="label">Country</label>
              <input className="input" value={formData.country} onChange={e => setFormData({...formData, country: e.target.value})} />
            </div>
            <div>
              <label className="label">City</label>
              <input className="input" value={formData.city} onChange={e => setFormData({...formData, city: e.target.value})} />
            </div>
            <div className="md:col-span-2">
              <label className="label">Address</label>
              <input className="input" value={formData.address} onChange={e => setFormData({...formData, address: e.target.value})} />
            </div>
            <div>
              <label className="label">Phone</label>
              <input className="input" value={formData.phone} onChange={e => setFormData({...formData, phone: e.target.value})} />
            </div>
            <div>
              <label className="label">Currency</label>
              <select className="input" value={formData.currencyCode} onChange={e => setFormData({...formData, currencyCode: e.target.value})}>
                <option value="USD">USD</option><option value="EUR">EUR</option><option value="GBP">GBP</option>
                <option value="LBP">LBP</option><option value="AED">AED</option><option value="SAR">SAR</option>
              </select>
            </div>
            <div>
              <label className="label">VAT %</label>
              <input type="number" step="0.01" className="input" value={formData.vatPercent} onChange={e => setFormData({...formData, vatPercent: parseFloat(e.target.value) || 0})} />
            </div>
            <div>
              <label className="label">Service Charge %</label>
              <input type="number" step="0.01" className="input" value={formData.serviceChargePercent} onChange={e => setFormData({...formData, serviceChargePercent: parseFloat(e.target.value) || 0})} />
            </div>
            <div className="md:col-span-2 flex gap-3">
              <button type="submit" className="btn-primary">{editingId ? 'Update' : 'Create'}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        {isLoading ? (
          <div className="p-8 text-center">Loading...</div>
        ) : (
          <table className="table">
            <thead><tr><th>Name</th><th>City</th><th>Currency</th><th>VAT</th><th>Status</th><th>Actions</th></tr></thead>
            <tbody>
              {branches?.data?.map((branch: any) => (
                <tr key={branch.id}>
                  <td className="font-medium flex items-center gap-2"><Building2 size={18} className="text-primary-600" />{branch.name}</td>
                  <td>{branch.city || '-'}</td>
                  <td>{branch.currencyCode}</td>
                  <td>{branch.vatPercent}%</td>
                  <td>
                    <span className={`px-2 py-1 rounded-full text-xs ${branch.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                      {branch.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="flex gap-2">
                    <button onClick={() => handleEdit(branch)} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => toggleMutation.mutate(branch.id)} className="text-gray-600 hover:text-gray-800">
                      {branch.isActive ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                    </button>
                    <button onClick={() => confirm('Delete?') && deleteMutation.mutate(branch.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
