import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { deliveryZonesApi, branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, MapPin } from 'lucide-react'

export default function DeliveryZones() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ branchId: 0, zoneName: '', description: '', baseFee: 0, minOrderAmount: '', extraFeePerKm: '', maxDistanceKm: '' })

  const { data: zones, isLoading } = useQuery({ queryKey: ['delivery-zones'], queryFn: () => deliveryZonesApi.getAll() })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ mutationFn: deliveryZonesApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['delivery-zones'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => deliveryZonesApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['delivery-zones'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: deliveryZonesApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['delivery-zones'] }) })
  const toggleMutation = useMutation({ mutationFn: deliveryZonesApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['delivery-zones'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ branchId: 0, zoneName: '', description: '', baseFee: 0, minOrderAmount: '', extraFeePerKm: '', maxDistanceKm: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const data = {
      ...formData,
      minOrderAmount: formData.minOrderAmount ? parseFloat(formData.minOrderAmount) : null,
      extraFeePerKm: formData.extraFeePerKm ? parseFloat(formData.extraFeePerKm) : null,
      maxDistanceKm: formData.maxDistanceKm ? parseFloat(formData.maxDistanceKm) : null
    }
    if (editingId) updateMutation.mutate({ id: editingId, data })
    else createMutation.mutate(data)
  }

  if (isLoading) return <div className="text-gray-400">Loading...</div>

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Delivery Zones</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Zone
        </button>
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Add'} Delivery Zone</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <select value={formData.branchId} onChange={(e) => setFormData({ ...formData, branchId: parseInt(e.target.value) })} className="input" required>
              <option value="">Select Branch *</option>
              {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
            <input type="text" placeholder="Zone Name *" value={formData.zoneName} onChange={(e) => setFormData({ ...formData, zoneName: e.target.value })} className="input" required />
            <input type="number" step="0.01" placeholder="Base Fee *" value={formData.baseFee} onChange={(e) => setFormData({ ...formData, baseFee: parseFloat(e.target.value) || 0 })} className="input" required />
            <input type="number" step="0.01" placeholder="Min Order Amount" value={formData.minOrderAmount} onChange={(e) => setFormData({ ...formData, minOrderAmount: e.target.value })} className="input" />
            <input type="number" step="0.01" placeholder="Extra Fee per KM" value={formData.extraFeePerKm} onChange={(e) => setFormData({ ...formData, extraFeePerKm: e.target.value })} className="input" />
            <input type="number" step="0.01" placeholder="Max Distance (KM)" value={formData.maxDistanceKm} onChange={(e) => setFormData({ ...formData, maxDistanceKm: e.target.value })} className="input" />
            <textarea placeholder="Description" value={formData.description} onChange={(e) => setFormData({ ...formData, description: e.target.value })} className="input md:col-span-2" rows={2} />
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
              <th className="text-left p-3">Zone Name</th>
              <th className="text-left p-3">Branch</th>
              <th className="text-left p-3">Base Fee</th>
              <th className="text-left p-3">Min Order</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {zones?.data?.map((zone: any) => (
              <tr key={zone.id} className="border-b hover:bg-gray-800/50">
                <td className="p-3 flex items-center gap-2"><MapPin size={16} className="text-gray-400" /> {zone.zoneName}</td>
                <td className="p-3">{zone.branchName}</td>
                <td className="p-3">${zone.baseFee.toFixed(2)}</td>
                <td className="p-3">{zone.minOrderAmount ? `$${zone.minOrderAmount.toFixed(2)}` : '-'}</td>
                <td className="p-3">
                  <button onClick={() => toggleMutation.mutate(zone.id)} className={`px-2 py-1 rounded text-xs ${zone.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {zone.isActive ? 'Active' : 'Inactive'}
                  </button>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => { setEditingId(zone.id); setFormData({ branchId: zone.branchId, zoneName: zone.zoneName, description: zone.description || '', baseFee: zone.baseFee, minOrderAmount: zone.minOrderAmount?.toString() || '', extraFeePerKm: zone.extraFeePerKm?.toString() || '', maxDistanceKm: zone.maxDistanceKm?.toString() || '' }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(zone.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {zones?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No delivery zones found</p>}
      </div>
    </div>
  )
}
