import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { suppliersApi } from '../lib/api'
import { Plus, Trash2, ShoppingCart, X, Check, FileText } from 'lucide-react'

interface POLine { inventoryItemId: number; itemName: string; quantity: number; unitPrice: number; unit: string }

export default function PurchaseOrders() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ supplierId: '', expectedDate: '', notes: '' })
  const [lines, setLines] = useState<POLine[]>([])
  const [newLine, setNewLine] = useState({ inventoryItemId: '', quantity: '1', unitPrice: '0' })
  const [showSupplierForm, setShowSupplierForm] = useState(false)
  const [newSupplierName, setNewSupplierName] = useState('')

  const { data: orders = [], isLoading } = useQuery({ queryKey: ['purchase-orders'], queryFn: () => api.get('/api/company/purchase-orders').then(r => Array.isArray(r.data) ? r.data : []) })
  const { data: suppliers = [] } = useQuery({ queryKey: ['suppliers'], queryFn: () => api.get('/api/company/suppliers').then(r => Array.isArray(r.data) ? r.data : []) })
  const { data: inventoryItems = [] } = useQuery({ queryKey: ['inventory'], queryFn: () => api.get('/api/company/inventory').then(r => Array.isArray(r.data) ? r.data : []) })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/purchase-orders', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['purchase-orders'] }); resetForm() }
  })
  const approveMutation = useMutation({
    mutationFn: (id: number) => api.patch(`/api/company/purchase-orders/${id}/approve`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['purchase-orders'] })
  })
  const deleteMutation = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/purchase-orders/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['purchase-orders'] })
  })
  const createSupplierMutation = useMutation({
    mutationFn: (name: string) => suppliersApi.create({ name }),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['suppliers'] }); setShowSupplierForm(false); setNewSupplierName('') }
  })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ supplierId: '', expectedDate: '', notes: '' }); setLines([]) }

  const addLine = () => {
    if (!newLine.inventoryItemId) return
    const item = inventoryItems?.find((i: any) => i.id === parseInt(newLine.inventoryItemId))
    if (item) {
      const qty = parseFloat(newLine.quantity) || 0
      const price = parseFloat(newLine.unitPrice) || 0
      setLines([...lines, { inventoryItemId: item.id, itemName: item.name, quantity: qty, unitPrice: price, unit: item.unitOfMeasure }])
      setNewLine({ inventoryItemId: '', quantity: '1', unitPrice: '0' })
    }
  }

  const removeLine = (index: number) => setLines(lines.filter((_, i) => i !== index))

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (lines.length === 0) return alert(t('purchaseOrders.addAtLeastOneItem'))
    createMutation.mutate({
      supplierId: parseInt(formData.supplierId),
      expectedDate: formData.expectedDate || null,
      lines: lines.map(l => ({ inventoryItemId: l.inventoryItemId, quantity: l.quantity, unitPrice: l.unitPrice }))
    })
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Draft': return 'bg-gray-100 text-gray-700'
      case 'Approved': return 'bg-blue-100 text-blue-700'
      case 'Received': return 'bg-green-100 text-green-700'
      case 'PartiallyReceived': return 'bg-yellow-100 text-yellow-700'
      case 'Cancelled': return 'bg-red-100 text-red-700'
      default: return 'bg-gray-100 text-gray-700'
    }
  }

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><ShoppingCart size={28} /> {t('inventory.purchaseOrders')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('purchaseOrders.newPO')}</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? t('common.edit') : t('common.create')} {t('purchaseOrders.title')}</h2>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
              <div>
                <label className="block text-sm font-medium mb-1">{t('inventory.supplier')} *</label>
                {showSupplierForm ? (
                  <div className="flex gap-2">
                    <input type="text" placeholder={t('purchaseOrders.newSupplierName')} value={newSupplierName} onChange={e => setNewSupplierName(e.target.value)} className="input flex-1" autoFocus />
                    <button type="button" onClick={() => newSupplierName.trim() && createSupplierMutation.mutate(newSupplierName.trim())} disabled={createSupplierMutation.isPending} className="px-3 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">{createSupplierMutation.isPending ? '...' : '✓'}</button>
                    <button type="button" onClick={() => { setShowSupplierForm(false); setNewSupplierName('') }} className="px-3 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300">✕</button>
                  </div>
                ) : (
                  <div className="flex gap-2">
                    <select value={formData.supplierId} onChange={(e) => setFormData({ ...formData, supplierId: e.target.value })} className="input flex-1" required>
                      <option value="">{t('common.select')} {t('inventory.supplier')}</option>
                      {suppliers?.map((s: any) => <option key={s.id} value={s.id}>{s.name}</option>)}
                    </select>
                    <button type="button" onClick={() => setShowSupplierForm(true)} className="px-3 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700" title={t('purchaseOrders.addNewSupplier')}><Plus size={20} /></button>
                  </div>
                )}
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('purchaseOrders.expectedDate')}</label>
                <input type="date" value={formData.expectedDate} onChange={(e) => setFormData({ ...formData, expectedDate: e.target.value })} className="input" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('common.notes')}</label>
                <input type="text" value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} className="input" />
              </div>
            </div>

            {/* Add Line */}
            <div className="bg-gray-800 rounded-lg p-4 mb-4">
              <h3 className="font-medium mb-3">{t('purchaseOrders.addItems')}</h3>
              <div className="flex gap-3 items-end">
                <div className="flex-1">
                  <label className="block text-sm mb-1">{t('common.item')}</label>
                  <select value={newLine.inventoryItemId} onChange={(e) => setNewLine({ ...newLine, inventoryItemId: e.target.value })} className="input">
                    <option value="">{t('common.select')} {t('common.item')}</option>
                    {inventoryItems?.map((i: any) => <option key={i.id} value={i.id}>{i.name} ({i.unitOfMeasure})</option>)}
                  </select>
                </div>
                <div className="w-32">
                  <label className="block text-sm mb-1">{t('purchaseOrders.qty')}</label>
                  <input type="number" step="0.01" value={newLine.quantity} onChange={(e) => setNewLine({ ...newLine, quantity: e.target.value })} className="input" />
                </div>
                <div className="w-32">
                  <label className="block text-sm mb-1">{t('purchaseOrders.unitPrice')}</label>
                  <input type="number" step="0.01" value={newLine.unitPrice} onChange={(e) => setNewLine({ ...newLine, unitPrice: e.target.value })} className="input" />
                </div>
                <button type="button" onClick={addLine} className="btn-primary"><Plus size={20} /></button>
              </div>
            </div>

            {/* Lines Table */}
            {lines.length > 0 && (
              <table className="w-full mb-4 border rounded-lg overflow-hidden">
                <thead className="bg-gray-800">
                  <tr>
                    <th className="text-left p-3">{t('common.item')}</th>
                    <th className="text-left p-3">{t('purchaseOrders.qty')}</th>
                    <th className="text-left p-3">{t('purchaseOrders.unitPrice')}</th>
                    <th className="text-left p-3">{t('common.total')}</th>
                    <th className="p-3"></th>
                  </tr>
                </thead>
                <tbody>
                  {lines.map((line, idx) => (
                    <tr key={idx} className="border-t">
                      <td className="p-3">{line.itemName}</td>
                      <td className="p-3">{line.quantity} {line.unit}</td>
                      <td className="p-3">${line.unitPrice.toFixed(2)}</td>
                      <td className="p-3 font-medium">${(line.quantity * line.unitPrice).toFixed(2)}</td>
                      <td className="p-3"><button type="button" onClick={() => removeLine(idx)} className="text-red-600"><X size={16} /></button></td>
                    </tr>
                  ))}
                  <tr className="border-t bg-gray-800">
                    <td colSpan={3} className="p-3 text-right font-medium">{t('common.total')}:</td>
                    <td className="p-3 font-bold">${lines.reduce((sum, l) => sum + l.quantity * l.unitPrice, 0).toFixed(2)}</td>
                    <td></td>
                  </tr>
                </tbody>
              </table>
            )}

            <div className="flex gap-2">
              <button type="submit" className="btn-primary">{t('purchaseOrders.createPO')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">{t('purchaseOrders.poNumber')}</th>
              <th className="text-left p-3">{t('inventory.supplier')}</th>
              <th className="text-left p-3">{t('reservations.date')}</th>
              <th className="text-left p-3">{t('purchaseOrders.expectedDate')}</th>
              <th className="text-left p-3">{t('common.items')}</th>
              <th className="text-left p-3">{t('common.total')}</th>
              <th className="text-left p-3">{t('common.status')}</th>
              <th className="text-left p-3">{t('common.actions')}</th>
            </tr>
          </thead>
          <tbody>
            {orders?.map((order: any) => (
              <tr key={order.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3 font-medium">PO-{order.id.toString().padStart(5, '0')}</td>
                <td className="p-3">{order.supplierName}</td>
                <td className="p-3">{new Date(order.createdAt).toLocaleDateString()}</td>
                <td className="p-3">{order.expectedDate ? new Date(order.expectedDate).toLocaleDateString() : '-'}</td>
                <td className="p-3">{order.lineCount} items</td>
                <td className="p-3 font-medium">${order.totalAmount?.toFixed(2)}</td>
                <td className="p-3"><span className={`px-2 py-1 rounded text-xs ${getStatusColor(order.status)}`}>{order.status}</span></td>
                <td className="p-3">
                  <div className="flex gap-2">
                    {order.status === 'Draft' && (
                      <button onClick={() => approveMutation.mutate(order.id)} className="text-green-600 hover:text-green-800" title={t('purchaseOrders.approve')}><Check size={16} /></button>
                    )}
                    {order.status === 'Draft' && (
                      <button onClick={() => deleteMutation.mutate(order.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                    )}
                    <button className="text-blue-600 hover:text-blue-800" title={t('common.view')}><FileText size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!orders || orders.length === 0) && <p className="text-center text-gray-500 py-8">{t('purchaseOrders.noPOs')}</p>}
      </div>
    </div>
  )
}
