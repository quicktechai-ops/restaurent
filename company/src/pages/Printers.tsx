import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { printersApi, branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, Printer } from 'lucide-react'

export default function Printers() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [branchFilter, setBranchFilter] = useState<number | undefined>()
  const [formData, setFormData] = useState({ branchId: 0, name: '', printerType: 'Receipt', connectionType: 'Network', connectionString: '', paperWidth: 80, isDefault: false })

  const { data: printers, isLoading } = useQuery({ queryKey: ['printers', branchFilter], queryFn: () => printersApi.getAll({ branchId: branchFilter }) })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ mutationFn: printersApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['printers'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => printersApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['printers'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: printersApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['printers'] }) })
  const toggleMutation = useMutation({ mutationFn: printersApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['printers'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ branchId: 0, name: '', printerType: 'Receipt', connectionType: 'Network', connectionString: '', paperWidth: 80, isDefault: false }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Printers</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Printer
        </button>
      </div>

      <div className="mb-4">
        <select value={branchFilter || ''} onChange={(e) => setBranchFilter(e.target.value ? parseInt(e.target.value) : undefined)} className="input-field w-64">
          <option value="">All Branches</option>
          {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Add'} Printer</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <select value={formData.branchId} onChange={(e) => setFormData({ ...formData, branchId: parseInt(e.target.value) })} className="input-field" required>
              <option value="">Select Branch *</option>
              {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
            <input type="text" placeholder="Printer Name *" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} className="input-field" required />
            <select value={formData.printerType} onChange={(e) => setFormData({ ...formData, printerType: e.target.value })} className="input-field">
              <option value="Receipt">Receipt Printer</option>
              <option value="Kitchen">Kitchen Printer</option>
              <option value="Label">Label Printer</option>
            </select>
            <select value={formData.connectionType} onChange={(e) => setFormData({ ...formData, connectionType: e.target.value })} className="input-field">
              <option value="Network">Network (IP)</option>
              <option value="USB">USB</option>
              <option value="Bluetooth">Bluetooth</option>
            </select>
            <input type="text" placeholder="Connection String (IP:Port)" value={formData.connectionString} onChange={(e) => setFormData({ ...formData, connectionString: e.target.value })} className="input-field" />
            <input type="number" placeholder="Paper Width (mm)" value={formData.paperWidth} onChange={(e) => setFormData({ ...formData, paperWidth: parseInt(e.target.value) || 80 })} className="input-field" />
            <div className="flex items-center gap-2">
              <input type="checkbox" id="isDefault" checked={formData.isDefault} onChange={(e) => setFormData({ ...formData, isDefault: e.target.checked })} />
              <label htmlFor="isDefault">Set as Default</label>
            </div>
            <div className="md:col-span-3 flex gap-2">
              <button type="submit" className="btn-primary">Save</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <table className="w-full">
          <thead>
            <tr className="border-b">
              <th className="text-left p-3">Name</th>
              <th className="text-left p-3">Branch</th>
              <th className="text-left p-3">Type</th>
              <th className="text-left p-3">Connection</th>
              <th className="text-left p-3">Paper</th>
              <th className="text-left p-3">Default</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {printers?.data?.map((printer: any) => (
              <tr key={printer.id} className="border-b hover:bg-gray-50">
                <td className="p-3 flex items-center gap-2"><Printer size={16} className="text-gray-400" /> {printer.name}</td>
                <td className="p-3">{printer.branchName}</td>
                <td className="p-3">{printer.printerType}</td>
                <td className="p-3">{printer.connectionType}: {printer.connectionString || '-'}</td>
                <td className="p-3">{printer.paperWidth}mm</td>
                <td className="p-3">{printer.isDefault ? <span className="text-green-600">✓</span> : '-'}</td>
                <td className="p-3">
                  <button onClick={() => toggleMutation.mutate(printer.id)} className={`px-2 py-1 rounded text-xs ${printer.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {printer.isActive ? 'Active' : 'Inactive'}
                  </button>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => { setEditingId(printer.id); setFormData({ branchId: printer.branchId, name: printer.name, printerType: printer.printerType, connectionType: printer.connectionType, connectionString: printer.connectionString || '', paperWidth: printer.paperWidth, isDefault: printer.isDefault }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(printer.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {printers?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No printers configured</p>}
      </div>
    </div>
  )
}
