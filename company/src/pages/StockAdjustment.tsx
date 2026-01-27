import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/api'
import { Plus, Settings, ArrowUp, ArrowDown } from 'lucide-react'

export default function StockAdjustment() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [formData, setFormData] = useState({ inventoryItemId: '', adjustmentType: 'increase', quantity: 0, reason: '', notes: '' })

  const { data: adjustments = [], isLoading } = useQuery({ 
    queryKey: ['stock-adjustments'], 
    queryFn: () => api.get('/api/company/stock-adjustments').then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: inventoryItems = [] } = useQuery({ 
    queryKey: ['inventory'], 
    queryFn: () => api.get('/api/company/inventory').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/stock-adjustments', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['stock-adjustments'] }); queryClient.invalidateQueries({ queryKey: ['inventory'] }); resetForm() }
  })

  const resetForm = () => { setShowForm(false); setFormData({ inventoryItemId: '', adjustmentType: 'increase', quantity: 0, reason: '', notes: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({ ...formData, inventoryItemId: parseInt(formData.inventoryItemId) })
  }

  const selectedItem = inventoryItems?.find((i: any) => i.id === parseInt(formData.inventoryItemId))

  if (isLoading) return <div>Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><Settings size={28} /> Stock Adjustment</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> New Adjustment</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">Create Stock Adjustment</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Item *</label>
              <select value={formData.inventoryItemId} onChange={(e) => setFormData({ ...formData, inventoryItemId: e.target.value })} className="input" required>
                <option value="">Select Item</option>
                {inventoryItems?.map((item: any) => (
                  <option key={item.id} value={item.id}>{item.name} - Current: {item.quantity} {item.unitOfMeasure}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Adjustment Type *</label>
              <select value={formData.adjustmentType} onChange={(e) => setFormData({ ...formData, adjustmentType: e.target.value })} className="input" required>
                <option value="increase">Increase (Add Stock)</option>
                <option value="decrease">Decrease (Remove Stock)</option>
                <option value="set">Set to Exact Value</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Quantity *</label>
              <div className="flex items-center gap-2">
                <input type="number" step="0.01" min="0" value={formData.quantity} onChange={(e) => setFormData({ ...formData, quantity: parseFloat(e.target.value) || 0 })} className="input" required />
                <span className="text-gray-500">{selectedItem?.unitOfMeasure || 'units'}</span>
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Reason *</label>
              <select value={formData.reason} onChange={(e) => setFormData({ ...formData, reason: e.target.value })} className="input" required>
                <option value="">Select Reason</option>
                <option value="Physical Count">Physical Count Correction</option>
                <option value="Found Stock">Found Stock</option>
                <option value="Data Entry Error">Data Entry Error</option>
                <option value="Return to Stock">Return to Stock</option>
                <option value="System Correction">System Correction</option>
                <option value="Other">Other</option>
              </select>
            </div>
            <div className="md:col-span-2">
              <label className="block text-sm font-medium mb-1">Notes</label>
              <textarea value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} className="input" rows={2} />
            </div>
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">Create Adjustment</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">Date</th>
              <th className="text-left p-3">Item</th>
              <th className="text-left p-3">Type</th>
              <th className="text-left p-3">Quantity</th>
              <th className="text-left p-3">Before</th>
              <th className="text-left p-3">After</th>
              <th className="text-left p-3">Reason</th>
              <th className="text-left p-3">By</th>
            </tr>
          </thead>
          <tbody>
            {adjustments?.map((adj: any) => (
              <tr key={adj.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3">{new Date(adj.createdAt).toLocaleString()}</td>
                <td className="p-3">{adj.itemName}</td>
                <td className="p-3">
                  {adj.adjustmentType === 'increase' && <span className="flex items-center gap-1 text-green-600"><ArrowUp size={14} /> Increase</span>}
                  {adj.adjustmentType === 'decrease' && <span className="flex items-center gap-1 text-red-600"><ArrowDown size={14} /> Decrease</span>}
                  {adj.adjustmentType === 'set' && <span className="text-blue-600">Set Value</span>}
                </td>
                <td className="p-3 font-medium">
                  <span className={adj.adjustmentType === 'increase' ? 'text-green-600' : adj.adjustmentType === 'decrease' ? 'text-red-600' : 'text-blue-600'}>
                    {adj.adjustmentType === 'increase' ? '+' : adj.adjustmentType === 'decrease' ? '-' : ''}{adj.quantity} {adj.unit}
                  </span>
                </td>
                <td className="p-3 text-gray-500">{adj.quantityBefore}</td>
                <td className="p-3 font-medium">{adj.quantityAfter}</td>
                <td className="p-3"><span className="px-2 py-1 bg-gray-100 rounded text-xs">{adj.reason}</span></td>
                <td className="p-3 text-sm text-gray-600">{adj.adjustedBy}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!adjustments || adjustments.length === 0) && <p className="text-center text-gray-500 py-8">No adjustments yet</p>}
      </div>
    </div>
  )
}
