import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/api'
import { Plus, ClipboardList, Check, AlertCircle } from 'lucide-react'

interface CountLine { inventoryItemId: number; itemName: string; systemQty: number; countedQty: number; unit: string; variance: number }

export default function StockCount() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [countName, setCountName] = useState('')
  const [lines, setLines] = useState<CountLine[]>([])

  const { data: counts = [], isLoading } = useQuery({ 
    queryKey: ['stock-counts'], 
    queryFn: () => api.get('/api/company/stock-counts').then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: inventoryItems = [] } = useQuery({ 
    queryKey: ['inventory'], 
    queryFn: () => api.get('/api/company/inventory').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/stock-counts', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['stock-counts'] }); queryClient.invalidateQueries({ queryKey: ['inventory'] }); resetForm() }
  })

  const resetForm = () => { setShowForm(false); setCountName(''); setLines([]) }

  const startNewCount = () => {
    setLines(inventoryItems?.map((item: any) => ({
      inventoryItemId: item.id,
      itemName: item.name,
      systemQty: item.quantity || 0,
      countedQty: item.quantity || 0,
      unit: item.unitOfMeasure,
      variance: 0
    })) || [])
    setShowForm(true)
  }

  const updateCountedQty = (index: number, qty: number) => {
    const updated = [...lines]
    updated[index].countedQty = qty
    updated[index].variance = qty - updated[index].systemQty
    setLines(updated)
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({
      name: countName || `Stock Count ${new Date().toLocaleDateString()}`,
      lines: lines.map(l => ({
        inventoryItemId: l.inventoryItemId,
        systemQuantity: l.systemQty,
        countedQuantity: l.countedQty
      }))
    })
  }

  const totalVariance = lines.reduce((sum, l) => sum + Math.abs(l.variance), 0)
  const itemsWithVariance = lines.filter(l => l.variance !== 0).length

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800 flex items-center gap-2"><ClipboardList size={28} /> Stock Count</h1>
        <button onClick={startNewCount} className="btn-primary flex items-center gap-2"><Plus size={20} /> New Stock Count</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold">Physical Stock Count</h2>
            <div className="flex items-center gap-4 text-sm">
              <span className="text-gray-600">Items with variance: <strong className="text-orange-600">{itemsWithVariance}</strong></span>
              <span className="text-gray-600">Total variance: <strong className={totalVariance > 0 ? 'text-red-600' : 'text-green-600'}>{totalVariance.toFixed(2)}</strong></span>
            </div>
          </div>
          
          <form onSubmit={handleSubmit}>
            <div className="mb-4">
              <label className="block text-sm font-medium mb-1">Count Name</label>
              <input type="text" value={countName} onChange={(e) => setCountName(e.target.value)} className="input-field w-full max-w-md" placeholder={`Stock Count ${new Date().toLocaleDateString()}`} />
            </div>

            <div className="border rounded-lg overflow-hidden max-h-[60vh] overflow-y-auto">
              <table className="w-full">
                <thead className="bg-gray-50 sticky top-0">
                  <tr>
                    <th className="text-left p-3">Item</th>
                    <th className="text-left p-3">Unit</th>
                    <th className="text-left p-3">System Qty</th>
                    <th className="text-left p-3">Counted Qty</th>
                    <th className="text-left p-3">Variance</th>
                  </tr>
                </thead>
                <tbody>
                  {lines.map((line, idx) => (
                    <tr key={idx} className={`border-t ${line.variance !== 0 ? 'bg-orange-50' : ''}`}>
                      <td className="p-3">{line.itemName}</td>
                      <td className="p-3 text-gray-500">{line.unit}</td>
                      <td className="p-3">{line.systemQty}</td>
                      <td className="p-3">
                        <input type="number" step="0.01" value={line.countedQty} onChange={(e) => updateCountedQty(idx, parseFloat(e.target.value) || 0)} className="input-field w-24" />
                      </td>
                      <td className="p-3">
                        {line.variance !== 0 ? (
                          <span className={`font-medium flex items-center gap-1 ${line.variance > 0 ? 'text-green-600' : 'text-red-600'}`}>
                            <AlertCircle size={14} />
                            {line.variance > 0 ? '+' : ''}{line.variance.toFixed(2)}
                          </span>
                        ) : (
                          <span className="text-green-600 flex items-center gap-1"><Check size={14} /> Match</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="flex gap-2 mt-4">
              <button type="submit" className="btn-primary flex items-center gap-2"><Check size={16} /> Complete Count & Apply Adjustments</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      {/* Count History */}
      <div className="card overflow-hidden">
        <h2 className="font-semibold p-4 border-b">Count History</h2>
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="text-left p-3">Date</th>
              <th className="text-left p-3">Name</th>
              <th className="text-left p-3">Items Counted</th>
              <th className="text-left p-3">Variances</th>
              <th className="text-left p-3">Counted By</th>
              <th className="text-left p-3">Status</th>
            </tr>
          </thead>
          <tbody>
            {counts?.map((count: any) => (
              <tr key={count.id} className="border-t hover:bg-gray-50">
                <td className="p-3">{new Date(count.createdAt).toLocaleString()}</td>
                <td className="p-3 font-medium">{count.name}</td>
                <td className="p-3">{count.itemCount} items</td>
                <td className="p-3">
                  {count.varianceCount > 0 ? (
                    <span className="text-orange-600">{count.varianceCount} items</span>
                  ) : (
                    <span className="text-green-600">All matched</span>
                  )}
                </td>
                <td className="p-3 text-sm text-gray-600">{count.countedBy}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${count.status === 'Completed' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'}`}>
                    {count.status}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!counts || counts.length === 0) && <p className="text-center text-gray-500 py-8">No stock counts yet</p>}
      </div>
    </div>
  )
}
