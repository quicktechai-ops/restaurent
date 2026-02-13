import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { giftCardsApi, branchesApi } from '../lib/api'
import { Plus, Gift, Ban, CheckCircle } from 'lucide-react'

export default function GiftCards() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [statusFilter, setStatusFilter] = useState('')
  const [formData, setFormData] = useState({ branchIssuedId: 0, initialValue: 0, currencyCode: 'USD', expiryDate: '' })

  const { data: giftCards, isLoading } = useQuery({ 
    queryKey: ['gift-cards', statusFilter], 
    queryFn: () => giftCardsApi.getAll({ status: statusFilter || undefined }) 
  })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ mutationFn: giftCardsApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['gift-cards'] }); resetForm() } })
  const blockMutation = useMutation({ mutationFn: giftCardsApi.block, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['gift-cards'] }) })
  const activateMutation = useMutation({ mutationFn: giftCardsApi.activate, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['gift-cards'] }) })

  const resetForm = () => { setShowForm(false); setFormData({ branchIssuedId: 0, initialValue: 0, currencyCode: 'USD', expiryDate: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const data = { ...formData, expiryDate: formData.expiryDate || undefined }
    createMutation.mutate(data)
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'bg-green-100 text-green-800'
      case 'UsedUp': return 'bg-gray-100 text-gray-900'
      case 'Blocked': return 'bg-red-100 text-red-800'
      case 'Expired': return 'bg-yellow-100 text-yellow-800'
      default: return 'bg-gray-100 text-gray-900'
    }
  }

  if (isLoading) return <div className="text-gray-400">{t('common.loading')}</div>

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{t('giftCards.title')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> {t('giftCards.issueCard')}
        </button>
      </div>

      <div className="mb-4">
        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)} className="input w-48">
          <option value="">{t('common.all')} {t('common.status')}</option>
          <option value="Active">{t('common.active')}</option>
          <option value="UsedUp">{t('giftCards.usedUp')}</option>
          <option value="Blocked">{t('giftCards.blocked')}</option>
          <option value="Expired">{t('giftCards.expired')}</option>
        </select>
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{t('giftCards.issueCard')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <select value={formData.branchIssuedId} onChange={(e) => setFormData({ ...formData, branchIssuedId: parseInt(e.target.value) })} className="input" required>
              <option value="">{t('common.select')} {t('staff.branch')} *</option>
              {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
            <input type="number" step="0.01" placeholder={`${t('giftCards.initialValue')} *`} value={formData.initialValue || ''} onChange={(e) => setFormData({ ...formData, initialValue: parseFloat(e.target.value) || 0 })} className="input" required />
            <select value={formData.currencyCode} onChange={(e) => setFormData({ ...formData, currencyCode: e.target.value })} className="input">
              <option value="USD">USD</option>
              <option value="EUR">EUR</option>
              <option value="LBP">LBP</option>
            </select>
            <input type="date" placeholder={t('giftCards.expiryDate')} value={formData.expiryDate} onChange={(e) => setFormData({ ...formData, expiryDate: e.target.value })} className="input" />
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">{t('giftCards.issueCard')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <table className="table">
          <thead>
            <tr className="border-b border-gray-700">
              <th className="text-left p-3">{t('giftCards.cardNumber')}</th>
              <th className="text-left p-3">{t('employees.branch')}</th>
              <th className="text-left p-3">{t('giftCards.initial')}</th>
              <th className="text-left p-3">{t('giftCards.balance')}</th>
              <th className="text-left p-3">{t('giftCards.expiry')}</th>
              <th className="text-left p-3">{t('common.status')}</th>
              <th className="text-left p-3">{t('common.actions')}</th>
            </tr>
          </thead>
          <tbody>
            {giftCards?.data?.map((card: any) => (
              <tr key={card.id} className="border-b hover:bg-gray-800/50">
                <td className="p-3 flex items-center gap-2"><Gift size={16} className="text-purple-500" /> {card.giftCardNumber}</td>
                <td className="p-3">{card.branchIssuedName}</td>
                <td className="p-3">{card.currencyCode} {card.initialValue.toFixed(2)}</td>
                <td className="p-3 font-semibold">{card.currencyCode} {card.currentBalance.toFixed(2)}</td>
                <td className="p-3">{card.expiryDate ? new Date(card.expiryDate).toLocaleDateString() : 'Never'}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${getStatusColor(card.status)}`}>{card.status}</span>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    {card.status === 'Active' && (
                      <button onClick={() => blockMutation.mutate(card.id)} className="text-red-600 hover:text-red-800" title={t('giftCards.block')}><Ban size={16} /></button>
                    )}
                    {card.status === 'Blocked' && (
                      <button onClick={() => activateMutation.mutate(card.id)} className="text-green-600 hover:text-green-800" title={t('giftCards.activate')}><CheckCircle size={16} /></button>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {giftCards?.data?.length === 0 && <p className="text-center text-gray-500 py-8">{t('giftCards.noCards')}</p>}
      </div>
    </div>
  )
}
