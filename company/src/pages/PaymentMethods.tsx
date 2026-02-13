import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { paymentMethodsApi } from '../lib/api'
import { Plus, Edit, Trash2, CreditCard } from 'lucide-react'

export default function PaymentMethods() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ name: '', type: 'Cash', requiresReference: false, sortOrder: 0 })

  const { data: methods, isLoading } = useQuery({ queryKey: ['paymentMethods'], queryFn: () => paymentMethodsApi.getAll() })

  const createMutation = useMutation({ mutationFn: paymentMethodsApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['paymentMethods'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => paymentMethodsApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['paymentMethods'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: paymentMethodsApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['paymentMethods'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', type: 'Cash', requiresReference: false, sortOrder: 0 }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{t('admin.paymentMethods')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('paymentMethods.addMethod')}</button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? t('paymentMethods.editMethod') : t('paymentMethods.addMethod')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">{t('common.name')} *</label><input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required /></div>
            <div><label className="label">{t('common.type')}</label>
              <select className="input" value={formData.type} onChange={e => setFormData({...formData, type: e.target.value})}>
                <option value="Cash">Cash</option><option value="Card">Card</option><option value="Online">Online</option><option value="Other">Other</option>
              </select>
            </div>
            <div><label className="label">{t('categories.sortOrder')}</label><input type="number" className="input" value={formData.sortOrder} onChange={e => setFormData({...formData, sortOrder: parseInt(e.target.value) || 0})} /></div>
            <div className="flex items-center gap-2"><input type="checkbox" checked={formData.requiresReference} onChange={e => setFormData({...formData, requiresReference: e.target.checked})} /><label>{t('paymentMethods.requiresReference')}</label></div>
            <div className="md:col-span-2 flex gap-3">
              <button type="submit" className="btn-primary">{editingId ? t('common.update') : t('common.create')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        {isLoading ? <div className="p-8 text-center">{t('common.loading')}</div> : (
          <table className="table">
            <thead><tr><th>{t('common.name')}</th><th>{t('common.type')}</th><th>{t('paymentMethods.requiresRef')}</th><th>{t('common.actions')}</th></tr></thead>
            <tbody>
              {methods?.data?.map((m: any) => (
                <tr key={m.id}>
                  <td className="font-medium flex items-center gap-2"><CreditCard size={18} className="text-primary-600" />{m.name}</td>
                  <td>{m.type}</td>
                  <td>{m.requiresReference ? t('common.yes') : t('common.no')}</td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(m.id); setFormData({ name: m.name, type: m.type, requiresReference: m.requiresReference, sortOrder: m.sortOrder }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm(t('common.confirmDelete')) && deleteMutation.mutate(m.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
