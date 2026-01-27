import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { loyaltyApi } from '../lib/api'
import { Plus, Edit, Trash2, Star, Award, Settings } from 'lucide-react'

export default function Loyalty() {
  const queryClient = useQueryClient()
  const [showTierForm, setShowTierForm] = useState(false)
  const [editingTierId, setEditingTierId] = useState<number | null>(null)
  const [settingsData, setSettingsData] = useState({ pointsPerAmount: 1, amountUnit: 10, pointsRedeemValue: 0.1, pointsExpiryMonths: '', earnOnNetBeforeTax: true, isActive: true })
  const [tierFormData, setTierFormData] = useState({ name: '', minTotalSpent: 0, minTotalPoints: 0, tierDiscountPercent: 0, benefits: '', sortOrder: 0 })

  const { data: settings } = useQuery({ queryKey: ['loyalty-settings'], queryFn: () => loyaltyApi.getSettings() })
  const { data: tiers, isLoading } = useQuery({ queryKey: ['loyalty-tiers'], queryFn: () => loyaltyApi.getTiers() })

  const updateSettingsMutation = useMutation({ mutationFn: loyaltyApi.updateSettings, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['loyalty-settings'] }) })
  const createTierMutation = useMutation({ mutationFn: loyaltyApi.createTier, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['loyalty-tiers'] }); resetTierForm() } })
  const updateTierMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => loyaltyApi.updateTier(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['loyalty-tiers'] }); resetTierForm() } })
  const deleteTierMutation = useMutation({ mutationFn: loyaltyApi.deleteTier, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['loyalty-tiers'] }) })

  const resetTierForm = () => { setShowTierForm(false); setEditingTierId(null); setTierFormData({ name: '', minTotalSpent: 0, minTotalPoints: 0, tierDiscountPercent: 0, benefits: '', sortOrder: 0 }) }

  const handleSaveSettings = () => {
    const data = { ...settingsData, pointsExpiryMonths: settingsData.pointsExpiryMonths ? parseInt(settingsData.pointsExpiryMonths as string) : null }
    updateSettingsMutation.mutate(data)
  }

  const handleTierSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingTierId) updateTierMutation.mutate({ id: editingTierId, data: tierFormData })
    else createTierMutation.mutate(tierFormData)
  }

  // Initialize settings form when data loads
  const currentSettings = settings?.data?.[0]

  if (isLoading) return <div className="text-gray-400">Loading...</div>

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Loyalty Program</h1>

      {/* Settings Card */}
      <div className="card mb-6">
        <h2 className="text-lg font-semibold mb-4 flex items-center gap-2"><Settings size={20} /> Loyalty Settings</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm text-gray-600 mb-1">Points per Amount</label>
            <input type="number" step="0.01" value={currentSettings?.pointsPerAmount || settingsData.pointsPerAmount} onChange={(e) => setSettingsData({ ...settingsData, pointsPerAmount: parseFloat(e.target.value) || 1 })} className="input" />
          </div>
          <div>
            <label className="block text-sm text-gray-600 mb-1">Per Amount Unit ($)</label>
            <input type="number" step="0.01" value={currentSettings?.amountUnit || settingsData.amountUnit} onChange={(e) => setSettingsData({ ...settingsData, amountUnit: parseFloat(e.target.value) || 10 })} className="input" />
          </div>
          <div>
            <label className="block text-sm text-gray-600 mb-1">Point Redeem Value ($)</label>
            <input type="number" step="0.01" value={currentSettings?.pointsRedeemValue || settingsData.pointsRedeemValue} onChange={(e) => setSettingsData({ ...settingsData, pointsRedeemValue: parseFloat(e.target.value) || 0.1 })} className="input" />
          </div>
          <div>
            <label className="block text-sm text-gray-600 mb-1">Points Expiry (months)</label>
            <input type="number" placeholder="Never" value={currentSettings?.pointsExpiryMonths || settingsData.pointsExpiryMonths} onChange={(e) => setSettingsData({ ...settingsData, pointsExpiryMonths: e.target.value })} className="input" />
          </div>
          <div className="flex items-center gap-2">
            <input type="checkbox" id="earnOnNet" checked={currentSettings?.earnOnNetBeforeTax ?? settingsData.earnOnNetBeforeTax} onChange={(e) => setSettingsData({ ...settingsData, earnOnNetBeforeTax: e.target.checked })} />
            <label htmlFor="earnOnNet" className="text-sm">Earn on Net Before Tax</label>
          </div>
          <div>
            <button onClick={handleSaveSettings} className="btn-primary">Save Settings</button>
          </div>
        </div>
        <p className="text-sm text-gray-500 mt-4">
          Example: Earn {settingsData.pointsPerAmount} point(s) per ${settingsData.amountUnit} spent. Each point = ${settingsData.pointsRedeemValue}
        </p>
      </div>

      {/* Tiers */}
      <div className="card">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-lg font-semibold flex items-center gap-2"><Award size={20} /> Loyalty Tiers</h2>
          <button onClick={() => setShowTierForm(true)} className="btn-primary flex items-center gap-2">
            <Plus size={20} /> Add Tier
          </button>
        </div>

        {showTierForm && (
          <form onSubmit={handleTierSubmit} className="border rounded p-4 mb-4 bg-gray-800 grid grid-cols-1 md:grid-cols-3 gap-4">
            <input type="text" placeholder="Tier Name *" value={tierFormData.name} onChange={(e) => setTierFormData({ ...tierFormData, name: e.target.value })} className="input" required />
            <input type="number" step="0.01" placeholder="Min Spent ($)" value={tierFormData.minTotalSpent} onChange={(e) => setTierFormData({ ...tierFormData, minTotalSpent: parseFloat(e.target.value) || 0 })} className="input" />
            <input type="number" step="0.01" placeholder="Discount %" value={tierFormData.tierDiscountPercent} onChange={(e) => setTierFormData({ ...tierFormData, tierDiscountPercent: parseFloat(e.target.value) || 0 })} className="input" />
            <input type="text" placeholder="Benefits" value={tierFormData.benefits} onChange={(e) => setTierFormData({ ...tierFormData, benefits: e.target.value })} className="input md:col-span-2" />
            <input type="number" placeholder="Sort Order" value={tierFormData.sortOrder} onChange={(e) => setTierFormData({ ...tierFormData, sortOrder: parseInt(e.target.value) || 0 })} className="input" />
            <div className="md:col-span-3 flex gap-2">
              <button type="submit" className="btn-primary">Save Tier</button>
              <button type="button" onClick={resetTierForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        )}

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {tiers?.data?.map((tier: any) => (
            <div key={tier.id} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
              <div className="flex justify-between items-start mb-2">
                <h3 className="font-semibold text-lg flex items-center gap-2"><Star size={18} className="text-yellow-500" /> {tier.name}</h3>
                <div className="flex gap-1">
                  <button onClick={() => { setEditingTierId(tier.id); setTierFormData(tier); setShowTierForm(true) }} className="text-blue-600"><Edit size={14} /></button>
                  <button onClick={() => deleteTierMutation.mutate(tier.id)} className="text-red-600"><Trash2 size={14} /></button>
                </div>
              </div>
              <p className="text-sm text-gray-600">Min Spent: ${tier.minTotalSpent}</p>
              <p className="text-sm text-gray-600">Discount: {tier.tierDiscountPercent}%</p>
              {tier.benefits && <p className="text-sm text-gray-500 mt-2">{tier.benefits}</p>}
              <p className="text-xs text-gray-400 mt-2">{tier.customersCount} customers</p>
            </div>
          ))}
        </div>
        {tiers?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No loyalty tiers configured</p>}
      </div>
    </div>
  )
}
