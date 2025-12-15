import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, Edit, Trash2, X, ToggleLeft, ToggleRight } from 'lucide-react'
import { plansApi } from '../lib/api'

interface Plan {
  id: number
  name: string
  description?: string
  price: number
  currencyCode: string
  billingCycle: string
  durationDays: number
  maxBranches: number
  maxUsers: number
  maxOrdersPerMonth?: number
  features?: string
  isActive: boolean
  sortOrder: number
}

interface PlanForm {
  name: string
  description: string
  price: number
  currencyCode: string
  billingCycle: string
  durationDays: number
  maxBranches: number
  maxUsers: number
  maxOrdersPerMonth: number | null
  features: string
  isActive: boolean
  sortOrder: number
}

const initialForm: PlanForm = {
  name: '',
  description: '',
  price: 0,
  currencyCode: 'USD',
  billingCycle: 'Monthly',
  durationDays: 30,
  maxBranches: 1,
  maxUsers: 5,
  maxOrdersPerMonth: null,
  features: '',
  isActive: true,
  sortOrder: 0
}

export default function Plans() {
  const [showModal, setShowModal] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [form, setForm] = useState<PlanForm>(initialForm)
  const queryClient = useQueryClient()

  const { data: plans = [], isLoading } = useQuery<Plan[]>({
    queryKey: ['plans-all'],
    queryFn: async () => {
      const res = await plansApi.getAll()
      return res.data
    }
  })

  const createMutation = useMutation({
    mutationFn: (data: PlanForm) => plansApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plans-all'] })
      setShowModal(false)
      setForm(initialForm)
    }
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: PlanForm }) => plansApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plans-all'] })
      setShowModal(false)
      setEditingId(null)
      setForm(initialForm)
    }
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => plansApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plans-all'] })
    }
  })

  const toggleMutation = useMutation({
    mutationFn: (id: number) => plansApi.toggle(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plans-all'] })
    }
  })

  const handleEdit = (plan: Plan) => {
    setForm({
      name: plan.name,
      description: plan.description || '',
      price: plan.price,
      currencyCode: plan.currencyCode,
      billingCycle: plan.billingCycle,
      durationDays: plan.durationDays,
      maxBranches: plan.maxBranches,
      maxUsers: plan.maxUsers,
      maxOrdersPerMonth: plan.maxOrdersPerMonth || null,
      features: plan.features || '',
      isActive: plan.isActive,
      sortOrder: plan.sortOrder
    })
    setEditingId(plan.id)
    setShowModal(true)
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) {
      updateMutation.mutate({ id: editingId, data: form })
    } else {
      createMutation.mutate(form)
    }
  }

  const handleDelete = (id: number, name: string) => {
    if (confirm(`Are you sure you want to delete "${name}"?`)) {
      deleteMutation.mutate(id)
    }
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-white">Subscription Plans</h1>
        <button 
          onClick={() => { setForm(initialForm); setEditingId(null); setShowModal(true) }}
          className="btn-primary flex items-center gap-2"
        >
          <Plus size={20} />
          Add Plan
        </button>
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {plans.map((plan) => (
            <div key={plan.id} className={`card ${!plan.isActive ? 'opacity-60' : ''}`}>
              <div className="flex items-start justify-between mb-4">
                <div>
                  <h3 className="text-xl font-bold text-white">{plan.name}</h3>
                  <p className="text-gray-400 text-sm">{plan.description}</p>
                </div>
                <button
                  onClick={() => toggleMutation.mutate(plan.id)}
                  className={`p-1 ${plan.isActive ? 'text-green-400' : 'text-gray-500'}`}
                >
                  {plan.isActive ? <ToggleRight size={24} /> : <ToggleLeft size={24} />}
                </button>
              </div>

              <div className="mb-4">
                <span className="text-3xl font-bold text-white">${plan.price}</span>
                <span className="text-gray-400">/{plan.billingCycle.toLowerCase()}</span>
              </div>

              <div className="space-y-2 mb-4 text-sm text-gray-300">
                <p>Duration: {plan.durationDays} days</p>
                <p>Max Branches: {plan.maxBranches}</p>
                <p>Max Users: {plan.maxUsers}</p>
                {plan.maxOrdersPerMonth && <p>Max Orders/Month: {plan.maxOrdersPerMonth}</p>}
              </div>

              <div className="flex gap-2 pt-4 border-t border-gray-700">
                <button onClick={() => handleEdit(plan)} className="btn-secondary flex-1 flex items-center justify-center gap-2">
                  <Edit size={16} /> Edit
                </button>
                <button onClick={() => handleDelete(plan.id, plan.name)} className="btn-danger flex-1 flex items-center justify-center gap-2">
                  <Trash2 size={16} /> Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-800 rounded-xl p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-xl font-bold text-white">
                {editingId ? 'Edit Plan' : 'Add Plan'}
              </h2>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-white">
                <X size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">Plan Name *</label>
                <input
                  type="text"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  className="input"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">Description</label>
                <input
                  type="text"
                  value={form.description}
                  onChange={(e) => setForm({ ...form, description: e.target.value })}
                  className="input"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Price *</label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.price}
                    onChange={(e) => setForm({ ...form, price: parseFloat(e.target.value) })}
                    className="input"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Billing Cycle</label>
                  <select
                    value={form.billingCycle}
                    onChange={(e) => setForm({ ...form, billingCycle: e.target.value })}
                    className="input"
                  >
                    <option value="Monthly">Monthly</option>
                    <option value="Yearly">Yearly</option>
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Duration (days) *</label>
                  <input
                    type="number"
                    value={form.durationDays}
                    onChange={(e) => setForm({ ...form, durationDays: parseInt(e.target.value) })}
                    className="input"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Sort Order</label>
                  <input
                    type="number"
                    value={form.sortOrder}
                    onChange={(e) => setForm({ ...form, sortOrder: parseInt(e.target.value) })}
                    className="input"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Max Branches *</label>
                  <input
                    type="number"
                    value={form.maxBranches}
                    onChange={(e) => setForm({ ...form, maxBranches: parseInt(e.target.value) })}
                    className="input"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Max Users *</label>
                  <input
                    type="number"
                    value={form.maxUsers}
                    onChange={(e) => setForm({ ...form, maxUsers: parseInt(e.target.value) })}
                    className="input"
                    required
                  />
                </div>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="isActive"
                  checked={form.isActive}
                  onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
                  className="w-4 h-4"
                />
                <label htmlFor="isActive" className="text-sm text-gray-300">Active</label>
              </div>

              <div className="flex gap-3 pt-4">
                <button type="submit" className="btn-primary flex-1" disabled={createMutation.isPending || updateMutation.isPending}>
                  {(createMutation.isPending || updateMutation.isPending) ? 'Saving...' : (editingId ? 'Update' : 'Create')}
                </button>
                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary flex-1">
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
