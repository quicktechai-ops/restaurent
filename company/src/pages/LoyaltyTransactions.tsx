import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { Star, Plus, Minus, ArrowUpDown, Filter, User } from 'lucide-react'

export default function LoyaltyTransactions() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [filters, setFilters] = useState({ customerId: '', type: '', dateFrom: '', dateTo: '' })
  const [showAdjustModal, setShowAdjustModal] = useState(false)
  const [adjustForm, setAdjustForm] = useState({ customerId: '', points: 0, type: 'add', reason: '' })

  const { data: transactions = [], isLoading } = useQuery({ 
    queryKey: ['loyalty-transactions', filters], 
    queryFn: () => api.get('/api/company/loyalty/transactions', { params: filters }).then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: customers = [] } = useQuery({ 
    queryKey: ['customers'], 
    queryFn: () => api.get('/api/company/customers').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const adjustMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/loyalty/adjust', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['loyalty-transactions'] }); setShowAdjustModal(false); setAdjustForm({ customerId: '', points: 0, type: 'add', reason: '' }) }
  })

  const handleAdjust = (e: React.FormEvent) => {
    e.preventDefault()
    adjustMutation.mutate({
      customerId: parseInt(adjustForm.customerId),
      points: adjustForm.type === 'add' ? adjustForm.points : -adjustForm.points,
      reason: adjustForm.reason
    })
  }

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'Earn': return 'bg-green-100 text-green-700'
      case 'Redeem': return 'bg-blue-100 text-blue-700'
      case 'Adjust': return 'bg-yellow-100 text-yellow-700'
      case 'Expire': return 'bg-red-100 text-red-700'
      default: return 'bg-gray-100 text-gray-700'
    }
  }

  if (isLoading) return <div className="text-gray-400">{t('common.loading')}</div>

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><Star size={28} /> {t('loyaltyTransactions.title')}</h1>
        <button onClick={() => setShowAdjustModal(true)} className="btn-primary flex items-center gap-2"><ArrowUpDown size={20} /> {t('loyaltyTransactions.adjustPoints')}</button>
      </div>

      {/* Filters */}
      <div className="card p-4 mb-6">
        <div className="flex items-center gap-2 mb-3">
          <Filter size={18} className="text-gray-500" />
          <span className="font-medium">{t('common.filter')}</span>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <select value={filters.customerId} onChange={(e) => setFilters({ ...filters, customerId: e.target.value })} className="input">
            <option value="">{t('common.all')} {t('loyaltyTransactions.customer')}</option>
            {customers?.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
          <select value={filters.type} onChange={(e) => setFilters({ ...filters, type: e.target.value })} className="input">
            <option value="">{t('common.all')} {t('common.type')}</option>
            <option value="Earn">{t('loyaltyTransactions.earn')}</option>
            <option value="Redeem">{t('loyaltyTransactions.redeem')}</option>
            <option value="Adjust">{t('loyaltyTransactions.adjust')}</option>
            <option value="Expire">{t('loyaltyTransactions.expire')}</option>
          </select>
          <input type="date" value={filters.dateFrom} onChange={(e) => setFilters({ ...filters, dateFrom: e.target.value })} className="input" />
          <input type="date" value={filters.dateTo} onChange={(e) => setFilters({ ...filters, dateTo: e.target.value })} className="input" />
        </div>
      </div>

      {/* Transactions Table */}
      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">{t('reservations.date')}</th>
              <th className="text-left p-3">{t('loyaltyTransactions.customer')}</th>
              <th className="text-left p-3">{t('common.type')}</th>
              <th className="text-left p-3">{t('loyaltyTransactions.points')}</th>
              <th className="text-left p-3">{t('loyaltyTransactions.balanceAfter')}</th>
              <th className="text-left p-3">{t('loyaltyTransactions.reference')}</th>
              <th className="text-left p-3">{t('common.notes')}</th>
            </tr>
          </thead>
          <tbody>
            {transactions?.map((t: any) => (
              <tr key={t.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3">{new Date(t.createdAt).toLocaleString()}</td>
                <td className="p-3 flex items-center gap-2"><User size={16} className="text-gray-400" /> {t.customerName}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${getTypeColor(t.transactionType)}`}>{t.transactionType}</span>
                </td>
                <td className="p-3">
                  <span className={`font-medium flex items-center gap-1 ${t.pointsChange > 0 ? 'text-green-600' : 'text-red-600'}`}>
                    {t.pointsChange > 0 ? <Plus size={14} /> : <Minus size={14} />}
                    {Math.abs(t.pointsChange)}
                  </span>
                </td>
                <td className="p-3 font-medium">{t.pointsAfter}</td>
                <td className="p-3 text-sm text-gray-600">{t.reference || '-'}</td>
                <td className="p-3 text-sm text-gray-600">{t.notes || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!transactions || transactions.length === 0) && <p className="text-center text-gray-500 py-8">{t('loyaltyTransactions.noTransactions')}</p>}
      </div>

      {/* Adjust Points Modal */}
      {showAdjustModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-900 rounded-xl shadow-xl w-full max-w-md p-6">
            <h2 className="text-lg font-semibold mb-4">{t('loyaltyTransactions.adjustPoints')}</h2>
            <form onSubmit={handleAdjust}>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium mb-1">{t('loyaltyTransactions.customer')} *</label>
                  <select value={adjustForm.customerId} onChange={(e) => setAdjustForm({ ...adjustForm, customerId: e.target.value })} className="input" required>
                    <option value="">{t('common.select')} {t('loyaltyTransactions.customer')}</option>
                    {customers?.map((c: any) => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">{t('loyaltyTransactions.adjustmentType')} *</label>
                  <select value={adjustForm.type} onChange={(e) => setAdjustForm({ ...adjustForm, type: e.target.value })} className="input" required>
                    <option value="add">{t('loyaltyTransactions.addPoints')}</option>
                    <option value="subtract">{t('loyaltyTransactions.subtractPoints')}</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">{t('loyaltyTransactions.points')} *</label>
                  <input type="number" min="1" value={adjustForm.points} onChange={(e) => setAdjustForm({ ...adjustForm, points: parseInt(e.target.value) || 0 })} className="input" required />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">{t('loyaltyTransactions.reason')} *</label>
                  <textarea value={adjustForm.reason} onChange={(e) => setAdjustForm({ ...adjustForm, reason: e.target.value })} className="input" rows={2} required />
                </div>
              </div>
              <div className="flex gap-2 mt-6">
                <button type="submit" className="btn-primary">{t('loyaltyTransactions.applyAdjustment')}</button>
                <button type="button" onClick={() => setShowAdjustModal(false)} className="btn-secondary">{t('common.cancel')}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
