import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { kitchenStationsApi, branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, ChefHat } from 'lucide-react'

export default function KitchenStations() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ branchId: 0, name: '', color: '#FF5722', averagePrepTime: 10, displayOrder: 0 })

  const { data: stations, isLoading } = useQuery({ queryKey: ['kitchenStations'], queryFn: () => kitchenStationsApi.getAll() })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ mutationFn: kitchenStationsApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['kitchenStations'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => kitchenStationsApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['kitchenStations'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: kitchenStationsApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['kitchenStations'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ branchId: 0, name: '', color: '#FF5722', averagePrepTime: 10, displayOrder: 0 }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{t('menu.kitchenStations')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('kitchenStations.addStation')}</button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? t('kitchenStations.editStation') : t('kitchenStations.addStation')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">{t('staff.branch')} *</label>
              <select className="input" value={formData.branchId} onChange={e => setFormData({...formData, branchId: parseInt(e.target.value)})} required>
                <option value="">-- {t('common.select')} --</option>
                {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
            <div><label className="label">{t('common.name')} *</label><input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required placeholder={t('kitchenStations.namePlaceholder')} /></div>
            <div><label className="label">{t('kitchenStations.color')}</label><input type="color" className="input h-10" value={formData.color} onChange={e => setFormData({...formData, color: e.target.value})} /></div>
            <div><label className="label">{t('kitchenStations.avgPrepTime')}</label><input type="number" className="input" value={formData.averagePrepTime} onChange={e => setFormData({...formData, averagePrepTime: parseInt(e.target.value) || 10})} /></div>
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
            <thead><tr><th>{t('common.name')}</th><th>{t('staff.branch')}</th><th>{t('kitchenStations.color')}</th><th>{t('menu.prepTime')}</th><th>{t('common.items')}</th><th>{t('common.actions')}</th></tr></thead>
            <tbody>
              {stations?.data?.map((station: any) => (
                <tr key={station.id}>
                  <td className="font-medium flex items-center gap-2"><ChefHat size={18} className="text-primary-600" />{station.name}</td>
                  <td>{station.branchName}</td>
                  <td><div className="w-6 h-6 rounded" style={{ backgroundColor: station.color || '#999' }}></div></td>
                  <td>{station.averagePrepTime} min</td>
                  <td>{station.menuItemsCount}</td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(station.id); setFormData({ branchId: station.branchId, name: station.name, color: station.color || '#FF5722', averagePrepTime: station.averagePrepTime, displayOrder: station.displayOrder }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm(t('common.confirmDelete')) && deleteMutation.mutate(station.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
