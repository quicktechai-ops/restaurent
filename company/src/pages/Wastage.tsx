import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { Plus, Trash2, AlertTriangle } from 'lucide-react'

export default function Wastage() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [formData, setFormData] = useState({ inventoryItemId: '', quantity: 0, reason: '', notes: '' })

  const { data: wastages = [], isLoading } = useQuery({ 
    queryKey: ['wastages'], 
    queryFn: () => api.get('/api/company/wastages').then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: inventoryItems = [] } = useQuery({ 
    queryKey: ['inventory'], 
    queryFn: () => api.get('/api/company/inventory').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/wastages', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['wastages'] }); resetForm() }
  })

  const resetForm = () => { setShowForm(false); setFormData({ inventoryItemId: '', quantity: 0, reason: '', notes: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createMutation.mutate({ ...formData, inventoryItemId: parseInt(formData.inventoryItemId) })
  }

  const selectedItem = inventoryItems?.find((i: any) => i.id === parseInt(formData.inventoryItemId))

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><AlertTriangle size={28} /> {t('inventory.wastage')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('wastage.record')}</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{t('wastage.record')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">{t('common.item')} *</label>
              <select value={formData.inventoryItemId} onChange={(e) => setFormData({ ...formData, inventoryItemId: e.target.value })} className="input" required>
                <option value="">{t('common.select')} {t('common.item')}</option>
                {inventoryItems?.map((item: any) => <option key={item.id} value={item.id}>{item.name} ({item.unitOfMeasure})</option>)}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('common.quantity')} *</label>
              <div className="flex items-center gap-2">
                <input type="number" step="0.01" value={formData.quantity} onChange={(e) => setFormData({ ...formData, quantity: parseFloat(e.target.value) || 0 })} className="input" required />
                <span className="text-gray-500">{selectedItem?.unitOfMeasure || 'units'}</span>
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('wastage.reason')} *</label>
              <select value={formData.reason} onChange={(e) => setFormData({ ...formData, reason: e.target.value })} className="input" required>
                <option value="">{t('common.select')} {t('wastage.reason')}</option>
                <option value="Expired">{t('wastage.expired')}</option>
                <option value="Damaged">{t('wastage.damaged')}</option>
                <option value="Spoiled">{t('wastage.spoiled')}</option>
                <option value="Spilled">{t('wastage.spilled')}</option>
                <option value="Theft">{t('wastage.theft')}</option>
                <option value="Quality">{t('wastage.qualityIssue')}</option>
                <option value="Other">{t('wastage.other')}</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('common.notes')}</label>
              <input type="text" value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} className="input" />
            </div>
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">{t('wastage.record')}</button>
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
              <th className="text-left p-3">{t('common.quantity')}</th>
              <th className="text-left p-3">{t('wastage.reason')}</th>
              <th className="text-left p-3">{t('wastage.costImpact')}</th>
              <th className="text-left p-3">{t('wastage.recordedBy')}</th>
              <th className="text-left p-3">{t('common.notes')}</th>
            </tr>
          </thead>
          <tbody>
            {wastages?.map((w: any) => (
              <tr key={w.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3">{new Date(w.createdAt).toLocaleString()}</td>
                <td className="p-3 flex items-center gap-2"><Trash2 size={16} className="text-red-400" /> {w.itemName}</td>
                <td className="p-3 text-red-600 font-medium">-{w.quantity} {w.unit}</td>
                <td className="p-3"><span className="px-2 py-1 bg-red-100 text-red-700 rounded text-xs">{w.reason}</span></td>
                <td className="p-3 text-red-600">-${w.costImpact?.toFixed(2) || '0.00'}</td>
                <td className="p-3 text-sm text-gray-600">{w.recordedBy}</td>
                <td className="p-3 text-sm text-gray-600">{w.notes || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!wastages || wastages.length === 0) && <p className="text-center text-gray-500 py-8">{t('wastage.noRecords')}</p>}
      </div>
    </div>
  )
}
