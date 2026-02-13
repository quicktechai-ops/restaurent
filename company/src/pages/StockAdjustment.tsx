import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { Plus, Settings, ArrowUp, ArrowDown } from 'lucide-react'

export default function StockAdjustment() {
  const { t } = useTranslation()
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

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><Settings size={28} /> {t('inventory.stockAdjustment')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('stockAdjustment.newAdjustment')}</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{t('stockAdjustment.createAdjustment')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">{t('common.item')} *</label>
              <select value={formData.inventoryItemId} onChange={(e) => setFormData({ ...formData, inventoryItemId: e.target.value })} className="input" required>
                <option value="">{t('common.select')} {t('common.item')}</option>
                {inventoryItems?.map((item: any) => (
                  <option key={item.id} value={item.id}>{item.name} - Current: {item.quantity} {item.unitOfMeasure}</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('stockAdjustment.adjustmentType')} *</label>
              <select value={formData.adjustmentType} onChange={(e) => setFormData({ ...formData, adjustmentType: e.target.value })} className="input" required>
                <option value="increase">{t('stockAdjustment.increase')}</option>
                <option value="decrease">{t('stockAdjustment.decrease')}</option>
                <option value="set">{t('stockAdjustment.setExact')}</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('common.quantity')} *</label>
              <div className="flex items-center gap-2">
                <input type="number" step="0.01" min="0" value={formData.quantity} onChange={(e) => setFormData({ ...formData, quantity: parseFloat(e.target.value) || 0 })} className="input" required />
                <span className="text-gray-500">{selectedItem?.unitOfMeasure || 'units'}</span>
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('stockAdjustment.reason')} *</label>
              <select value={formData.reason} onChange={(e) => setFormData({ ...formData, reason: e.target.value })} className="input" required>
                <option value="">{t('common.select')} {t('stockAdjustment.reason')}</option>
                <option value="Physical Count">{t('stockAdjustment.physicalCount')}</option>
                <option value="Found Stock">{t('stockAdjustment.foundStock')}</option>
                <option value="Data Entry Error">{t('stockAdjustment.dataEntryError')}</option>
                <option value="Return to Stock">{t('stockAdjustment.returnToStock')}</option>
                <option value="System Correction">{t('stockAdjustment.systemCorrection')}</option>
                <option value="Other">{t('stockAdjustment.other')}</option>
              </select>
            </div>
            <div className="md:col-span-2">
              <label className="block text-sm font-medium mb-1">{t('common.notes')}</label>
              <textarea value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} className="input" rows={2} />
            </div>
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">{t('stockAdjustment.createAdjustment')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">{t('reservations.date')}</th>
              <th className="text-left p-3">{t('common.item')}</th>
              <th className="text-left p-3">{t('common.type')}</th>
              <th className="text-left p-3">{t('common.quantity')}</th>
              <th className="text-left p-3">{t('stockAdjustment.before')}</th>
              <th className="text-left p-3">{t('stockAdjustment.after')}</th>
              <th className="text-left p-3">{t('stockAdjustment.reason')}</th>
              <th className="text-left p-3">{t('stockAdjustment.by')}</th>
            </tr>
          </thead>
          <tbody>
            {adjustments?.map((adj: any) => (
              <tr key={adj.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3">{new Date(adj.createdAt).toLocaleString()}</td>
                <td className="p-3">{adj.itemName}</td>
                <td className="p-3">
                  {adj.adjustmentType === 'increase' && <span className="flex items-center gap-1 text-green-600"><ArrowUp size={14} /> {t('stockAdjustment.increase')}</span>}
                  {adj.adjustmentType === 'decrease' && <span className="flex items-center gap-1 text-red-600"><ArrowDown size={14} /> {t('stockAdjustment.decrease')}</span>}
                  {adj.adjustmentType === 'set' && <span className="text-blue-600">{t('stockAdjustment.setExact')}</span>}
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
        {(!adjustments || adjustments.length === 0) && <p className="text-center text-gray-500 py-8">{t('stockAdjustment.noAdjustments')}</p>}
      </div>
    </div>
  )
}
