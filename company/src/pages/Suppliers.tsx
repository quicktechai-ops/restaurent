import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { suppliersApi } from '../lib/api'
import { Plus, Edit, Trash2, Truck } from 'lucide-react'

export default function Suppliers() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [search, setSearch] = useState('')
  const [formData, setFormData] = useState({ name: '', contactPerson: '', phone: '', email: '', address: '', paymentTerms: '' })

  const { data: suppliers, isLoading } = useQuery({ queryKey: ['suppliers', search], queryFn: () => suppliersApi.getAll({ search: search || undefined }) })

  const createMutation = useMutation({ mutationFn: suppliersApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['suppliers'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => suppliersApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['suppliers'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: suppliersApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['suppliers'] }) })
  const toggleMutation = useMutation({ mutationFn: suppliersApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['suppliers'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', contactPerson: '', phone: '', email: '', address: '', paymentTerms: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  if (isLoading) return <div>Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Suppliers</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Supplier
        </button>
      </div>

      <div className="mb-4">
        <input type="text" placeholder="Search suppliers..." value={search} onChange={(e) => setSearch(e.target.value)} className="input w-full max-w-md" />
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Add'} Supplier</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <input type="text" placeholder="Name *" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} className="input" required />
            <input type="text" placeholder="Contact Person" value={formData.contactPerson} onChange={(e) => setFormData({ ...formData, contactPerson: e.target.value })} className="input" />
            <input type="tel" placeholder="Phone" value={formData.phone} onChange={(e) => setFormData({ ...formData, phone: e.target.value })} className="input" />
            <input type="email" placeholder="Email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} className="input" />
            <input type="text" placeholder="Payment Terms" value={formData.paymentTerms} onChange={(e) => setFormData({ ...formData, paymentTerms: e.target.value })} className="input" />
            <input type="text" placeholder="Address" value={formData.address} onChange={(e) => setFormData({ ...formData, address: e.target.value })} className="input" />
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">Save</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <table className="table">
          <thead>
            <tr className="border-b border-gray-700">
              <th className="text-left p-3">Name</th>
              <th className="text-left p-3">Contact</th>
              <th className="text-left p-3">Phone</th>
              <th className="text-left p-3">Email</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {suppliers?.data?.map((supplier: any) => (
              <tr key={supplier.id} className="border-b hover:bg-gray-800/50">
                <td className="p-3 flex items-center gap-2"><Truck size={16} className="text-gray-400" /> {supplier.name}</td>
                <td className="p-3">{supplier.contactPerson || '-'}</td>
                <td className="p-3">{supplier.phone || '-'}</td>
                <td className="p-3">{supplier.email || '-'}</td>
                <td className="p-3">
                  <button onClick={() => toggleMutation.mutate(supplier.id)} className={`px-2 py-1 rounded text-xs ${supplier.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {supplier.isActive ? 'Active' : 'Inactive'}
                  </button>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => { setEditingId(supplier.id); setFormData(supplier); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(supplier.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {suppliers?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No suppliers found</p>}
      </div>
    </div>
  )
}
