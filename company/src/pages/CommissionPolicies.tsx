import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { Plus, Edit, Trash2, DollarSign, Percent } from 'lucide-react'

export default function CommissionPolicies() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({
    name: '',
    type: 'percentage',
    value: 0,
    appliesTo: 'all',
    categoryId: '',
    menuItemId: '',
    minSalesAmount: 0,
    isActive: true
  })

  const { data: policies = [], isLoading } = useQuery({ 
    queryKey: ['commission-policies'], 
    queryFn: () => api.get('/api/company/commission-policies').then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: categories = [] } = useQuery({ queryKey: ['categories'], queryFn: () => api.get('/api/company/categories').then(r => Array.isArray(r.data) ? r.data : []) })
  const { data: menuItems = [] } = useQuery({ queryKey: ['menu-items'], queryFn: () => api.get('/api/company/menu-items').then(r => Array.isArray(r.data) ? r.data : []) })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/commission-policies', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['commission-policies'] }); resetForm() }
  })
  
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => api.put(`/api/company/commission-policies/${id}`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['commission-policies'] }); resetForm() }
  })
  
  const deleteMutation = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/commission-policies/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['commission-policies'] })
  })

  const resetForm = () => {
    setShowForm(false)
    setEditingId(null)
    setFormData({ name: '', type: 'percentage', value: 0, appliesTo: 'all', categoryId: '', menuItemId: '', minSalesAmount: 0, isActive: true })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const data = {
      ...formData,
      categoryId: formData.categoryId ? parseInt(formData.categoryId) : null,
      menuItemId: formData.menuItemId ? parseInt(formData.menuItemId) : null
    }
    if (editingId) updateMutation.mutate({ id: editingId, data })
    else createMutation.mutate(data)
  }

  const handleEdit = (policy: any) => {
    setEditingId(policy.id)
    setFormData({
      name: policy.name,
      type: policy.type,
      value: policy.value,
      appliesTo: policy.appliesTo,
      categoryId: policy.categoryId?.toString() || '',
      menuItemId: policy.menuItemId?.toString() || '',
      minSalesAmount: policy.minSalesAmount || 0,
      isActive: policy.isActive
    })
    setShowForm(true)
  }

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><DollarSign size={28} /> {t('commissionPolicies.title')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('commissionPolicies.addPolicy')}</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? t('commissionPolicies.editPolicy') : t('commissionPolicies.addPolicy')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">{t('commissionPolicies.policyName')} *</label>
              <input type="text" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} className="input" required />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('commissionPolicies.commissionType')} *</label>
              <select value={formData.type} onChange={(e) => setFormData({ ...formData, type: e.target.value })} className="input" required>
                <option value="percentage">{t('commissionPolicies.percentageOfSales')}</option>
                <option value="fixed_per_item">{t('commissionPolicies.fixedPerItem')}</option>
                <option value="fixed_per_order">{t('commissionPolicies.fixedPerOrder')}</option>
                <option value="tiered">{t('commissionPolicies.tiered')}</option>
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('commissionPolicies.value')} *</label>
              <div className="flex items-center gap-2">
                <input type="number" step="0.01" value={formData.value} onChange={(e) => setFormData({ ...formData, value: parseFloat(e.target.value) || 0 })} className="input" required />
                <span className="text-gray-500">{formData.type === 'percentage' ? '%' : '$'}</span>
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">{t('commissionPolicies.appliesTo')} *</label>
              <select value={formData.appliesTo} onChange={(e) => setFormData({ ...formData, appliesTo: e.target.value })} className="input" required>
                <option value="all">{t('commissionPolicies.allItems')}</option>
                <option value="category">{t('commissionPolicies.specificCategory')}</option>
                <option value="item">{t('commissionPolicies.specificItem')}</option>
              </select>
            </div>
            {formData.appliesTo === 'category' && (
              <div>
                <label className="block text-sm font-medium mb-1">{t('inventory.category')}</label>
                <select value={formData.categoryId} onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })} className="input">
                  <option value="">{t('commissionPolicies.selectCategory')}</option>
                  {categories?.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>
            )}
            {formData.appliesTo === 'item' && (
              <div>
                <label className="block text-sm font-medium mb-1">{t('commissionPolicies.menuItem')}</label>
                <select value={formData.menuItemId} onChange={(e) => setFormData({ ...formData, menuItemId: e.target.value })} className="input">
                  <option value="">{t('commissionPolicies.selectItem')}</option>
                  {menuItems?.map((i: any) => <option key={i.id} value={i.id}>{i.name}</option>)}
                </select>
              </div>
            )}
            <div>
              <label className="block text-sm font-medium mb-1">{t('commissionPolicies.minSalesAmount')}</label>
              <input type="number" step="0.01" value={formData.minSalesAmount} onChange={(e) => setFormData({ ...formData, minSalesAmount: parseFloat(e.target.value) || 0 })} className="input" />
              <p className="text-xs text-gray-500 mt-1">{t('commissionPolicies.minSalesHint')}</p>
            </div>
            <div className="flex items-center gap-2">
              <input type="checkbox" id="isActive" checked={formData.isActive} onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} />
              <label htmlFor="isActive">{t('common.active')}</label>
            </div>
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">{editingId ? t('common.update') : t('common.create')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">{t('commissionPolicies.policyName')}</th>
              <th className="text-left p-3">{t('common.type')}</th>
              <th className="text-left p-3">{t('commissionPolicies.value')}</th>
              <th className="text-left p-3">{t('commissionPolicies.appliesTo')}</th>
              <th className="text-left p-3">{t('commissionPolicies.minSales')}</th>
              <th className="text-left p-3">{t('common.status')}</th>
              <th className="text-left p-3">{t('common.actions')}</th>
            </tr>
          </thead>
          <tbody>
            {policies?.map((policy: any) => (
              <tr key={policy.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3 font-medium">{policy.name}</td>
                <td className="p-3">
                  <span className="px-2 py-1 bg-blue-100 text-blue-700 rounded text-xs">{policy.type.replace(/_/g, ' ')}</span>
                </td>
                <td className="p-3 flex items-center gap-1">
                  {policy.type === 'percentage' ? <Percent size={14} /> : <DollarSign size={14} />}
                  {policy.value}{policy.type === 'percentage' ? '%' : ''}
                </td>
                <td className="p-3">
                  {policy.appliesTo === 'all' && t('commissionPolicies.allItems')}
                  {policy.appliesTo === 'category' && `${t('inventory.category')}: ${policy.categoryName}`}
                  {policy.appliesTo === 'item' && `${t('commissionPolicies.menuItem')}: ${policy.menuItemName}`}
                </td>
                <td className="p-3">{policy.minSalesAmount > 0 ? `$${policy.minSalesAmount}` : '-'}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${policy.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'}`}>
                    {policy.isActive ? t('common.active') : t('common.inactive')}
                  </span>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => handleEdit(policy)} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(policy.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!policies || policies.length === 0) && <p className="text-center text-gray-500 py-8">{t('commissionPolicies.noPolicies')}</p>}
      </div>
    </div>
  )
}
