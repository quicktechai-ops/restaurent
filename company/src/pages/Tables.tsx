import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { tablesApi, branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, Grid3X3 } from 'lucide-react'

export default function Tables() {
  const queryClient = useQueryClient()
  const [branchFilter, setBranchFilter] = useState<number | null>(null)
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ branchId: 0, tableName: '', zone: '', capacity: 4, floorNumber: 1 })

  const { data: tables, isLoading } = useQuery({ queryKey: ['tables', branchFilter], queryFn: () => tablesApi.getAll(branchFilter || undefined) })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ mutationFn: tablesApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['tables'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => tablesApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['tables'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: tablesApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['tables'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ branchId: 0, tableName: '', zone: '', capacity: 4, floorNumber: 1 }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Tables</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Table</button>
      </div>

      <div className="card p-4 mb-6">
        <select className="input w-48" value={branchFilter || ''} onChange={e => setBranchFilter(e.target.value ? parseInt(e.target.value) : null)}>
          <option value="">All Branches</option>
          {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Table' : 'Add Table'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">Branch *</label>
              <select className="input" value={formData.branchId} onChange={e => setFormData({...formData, branchId: parseInt(e.target.value)})} required>
                <option value="">-- Select --</option>
                {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
            <div><label className="label">Table Name *</label><input className="input" value={formData.tableName} onChange={e => setFormData({...formData, tableName: e.target.value})} required /></div>
            <div><label className="label">Zone</label><input className="input" value={formData.zone} onChange={e => setFormData({...formData, zone: e.target.value})} placeholder="Hall, Terrace..." /></div>
            <div><label className="label">Capacity</label><input type="number" className="input" value={formData.capacity} onChange={e => setFormData({...formData, capacity: parseInt(e.target.value) || 4})} /></div>
            <div><label className="label">Floor</label><input type="number" className="input" value={formData.floorNumber} onChange={e => setFormData({...formData, floorNumber: parseInt(e.target.value) || 1})} /></div>
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
            <thead><tr><th>Name</th><th>Branch</th><th>Zone</th><th>Capacity</th><th>Floor</th><th>Actions</th></tr></thead>
            <tbody>
              {tables?.data?.map((table: any) => (
                <tr key={table.id}>
                  <td className="font-medium flex items-center gap-2"><Grid3X3 size={18} className="text-primary-600" />{table.tableName}</td>
                  <td>{table.branchName}</td>
                  <td>{table.zone || '-'}</td>
                  <td>{table.capacity}</td>
                  <td>{table.floorNumber}</td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(table.id); setFormData({ branchId: table.branchId, tableName: table.tableName, zone: table.zone || '', capacity: table.capacity, floorNumber: table.floorNumber }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm('Delete?') && deleteMutation.mutate(table.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
