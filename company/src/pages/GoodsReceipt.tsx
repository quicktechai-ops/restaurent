import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/api'
import { Package, X, Check, Truck } from 'lucide-react'

interface ReceiptLine { inventoryItemId: number; itemName: string; orderedQty: number; receivedQty: number; unitCost: number; unit: string }

export default function GoodsReceipt() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [selectedPO, setSelectedPO] = useState<any>(null)
  const [lines, setLines] = useState<ReceiptLine[]>([])
  const [notes, setNotes] = useState('')

  const { data: pendingPOs = [], isLoading } = useQuery({ 
    queryKey: ['pending-pos'], 
    queryFn: () => api.get('/api/company/purchase-orders?status=Approved').then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: receipts = [] } = useQuery({ 
    queryKey: ['goods-receipts'], 
    queryFn: () => api.get('/api/company/goods-receipts').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/goods-receipts', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['goods-receipts'] }); queryClient.invalidateQueries({ queryKey: ['pending-pos'] }); resetForm() }
  })

  const resetForm = () => { setShowForm(false); setSelectedPO(null); setLines([]); setNotes('') }

  const loadPOLines = async (poId: number) => {
    const res = await api.get(`/api/company/purchase-orders/${poId}`)
    setSelectedPO(res.data)
    setLines(res.data.lines?.map((l: any) => ({
      inventoryItemId: l.inventoryItemId,
      itemName: l.itemName,
      orderedQty: l.quantity,
      receivedQty: l.quantity,
      unitCost: l.unitPrice,
      unit: l.unit
    })) || [])
    setShowForm(true)
  }

  const updateReceivedQty = (index: number, qty: number) => {
    const updated = [...lines]
    updated[index].receivedQty = qty
    setLines(updated)
  }

  const updateUnitCost = (index: number, cost: number) => {
    const updated = [...lines]
    updated[index].unitCost = cost
    setLines(updated)
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({
      purchaseOrderId: selectedPO.id,
      notes,
      lines: lines.map(l => ({
        inventoryItemId: l.inventoryItemId,
        receivedQuantity: l.receivedQty,
        unitCost: l.unitCost
      }))
    })
  }

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800 flex items-center gap-2"><Truck size={28} /> Goods Receipt</h1>
      </div>

      {/* Pending POs to Receive */}
      <div className="card mb-6">
        <h2 className="font-semibold p-4 border-b">Pending Purchase Orders</h2>
        <div className="p-4">
          {pendingPOs?.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {pendingPOs.map((po: any) => (
                <div key={po.id} className="border rounded-lg p-4 hover:bg-gray-50">
                  <div className="flex justify-between items-start mb-2">
                    <span className="font-medium">PO-{po.id.toString().padStart(5, '0')}</span>
                    <span className="text-sm text-gray-500">{new Date(po.createdAt).toLocaleDateString()}</span>
                  </div>
                  <p className="text-sm text-gray-600 mb-2">{po.supplierName}</p>
                  <p className="text-sm mb-3">{po.lineCount} items • ${po.totalAmount?.toFixed(2)}</p>
                  <button onClick={() => loadPOLines(po.id)} className="btn-primary w-full flex items-center justify-center gap-2">
                    <Package size={16} /> Receive
                  </button>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-center text-gray-500 py-4">No pending purchase orders to receive</p>
          )}
        </div>
      </div>

      {/* Receipt Form Modal */}
      {showForm && selectedPO && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-4xl max-h-[90vh] overflow-hidden">
            <div className="flex justify-between items-center p-4 border-b">
              <h2 className="text-lg font-semibold">Receive PO-{selectedPO.id.toString().padStart(5, '0')} from {selectedPO.supplierName}</h2>
              <button onClick={resetForm} className="text-gray-500 hover:text-gray-700"><X size={20} /></button>
            </div>
            
            <form onSubmit={handleSubmit} className="p-4 max-h-[70vh] overflow-y-auto">
              <table className="w-full mb-4 border rounded-lg overflow-hidden">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="text-left p-3">Item</th>
                    <th className="text-left p-3">Ordered</th>
                    <th className="text-left p-3">Received</th>
                    <th className="text-left p-3">Unit Cost</th>
                    <th className="text-left p-3">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {lines.map((line, idx) => (
                    <tr key={idx} className="border-t">
                      <td className="p-3">{line.itemName}</td>
                      <td className="p-3">{line.orderedQty} {line.unit}</td>
                      <td className="p-3">
                        <input type="number" step="0.01" value={line.receivedQty} onChange={(e) => updateReceivedQty(idx, parseFloat(e.target.value) || 0)} className="input-field w-24" />
                      </td>
                      <td className="p-3">
                        <input type="number" step="0.01" value={line.unitCost} onChange={(e) => updateUnitCost(idx, parseFloat(e.target.value) || 0)} className="input-field w-24" />
                      </td>
                      <td className="p-3 font-medium">${(line.receivedQty * line.unitCost).toFixed(2)}</td>
                    </tr>
                  ))}
                  <tr className="border-t bg-gray-50">
                    <td colSpan={4} className="p-3 text-right font-medium">Total:</td>
                    <td className="p-3 font-bold">${lines.reduce((sum, l) => sum + l.receivedQty * l.unitCost, 0).toFixed(2)}</td>
                  </tr>
                </tbody>
              </table>

              <div className="mb-4">
                <label className="block text-sm font-medium mb-1">Notes</label>
                <textarea value={notes} onChange={(e) => setNotes(e.target.value)} className="input-field w-full" rows={2} />
              </div>

              <div className="flex gap-2">
                <button type="submit" className="btn-primary flex items-center gap-2"><Check size={16} /> Confirm Receipt</button>
                <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Receipt History */}
      <div className="card overflow-hidden">
        <h2 className="font-semibold p-4 border-b">Receipt History</h2>
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="text-left p-3">Receipt #</th>
              <th className="text-left p-3">PO #</th>
              <th className="text-left p-3">Supplier</th>
              <th className="text-left p-3">Date</th>
              <th className="text-left p-3">Items</th>
              <th className="text-left p-3">Total</th>
            </tr>
          </thead>
          <tbody>
            {receipts?.map((r: any) => (
              <tr key={r.id} className="border-t hover:bg-gray-50">
                <td className="p-3 font-medium">GR-{r.id.toString().padStart(5, '0')}</td>
                <td className="p-3">PO-{r.purchaseOrderId?.toString().padStart(5, '0')}</td>
                <td className="p-3">{r.supplierName}</td>
                <td className="p-3">{new Date(r.createdAt).toLocaleDateString()}</td>
                <td className="p-3">{r.lineCount} items</td>
                <td className="p-3 font-medium">${r.totalAmount?.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!receipts || receipts.length === 0) && <p className="text-center text-gray-500 py-8">No receipts yet</p>}
      </div>
    </div>
  )
}
