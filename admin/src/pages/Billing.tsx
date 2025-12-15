import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Plus, X } from 'lucide-react'
import { billingApi, companiesApi } from '../lib/api'

interface Billing {
  id: number
  companyId: number
  companyName: string
  amount: number
  currencyCode: string
  paymentMethod: string
  paymentReference?: string
  paymentDate: string
  status: string
  notes?: string
}

interface Company {
  id: number
  name: string
}

interface BillingForm {
  companyId: number | null
  amount: number
  currencyCode: string
  paymentMethod: string
  paymentReference: string
  paymentDate: string
  status: string
  notes: string
}

const initialForm: BillingForm = {
  companyId: null,
  amount: 0,
  currencyCode: 'USD',
  paymentMethod: 'Cash',
  paymentReference: '',
  paymentDate: new Date().toISOString().split('T')[0],
  status: 'completed',
  notes: ''
}

export default function Billing() {
  const [showModal, setShowModal] = useState(false)
  const [form, setForm] = useState<BillingForm>(initialForm)
  const queryClient = useQueryClient()

  const { data: billings = [], isLoading } = useQuery<Billing[]>({
    queryKey: ['billings'],
    queryFn: async () => {
      const res = await billingApi.getAll()
      return res.data
    }
  })

  const { data: companies = [] } = useQuery<Company[]>({
    queryKey: ['companies-list'],
    queryFn: async () => {
      const res = await companiesApi.getAll()
      return res.data
    }
  })

  const createMutation = useMutation({
    mutationFn: (data: BillingForm) => billingApi.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['billings'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
      setShowModal(false)
      setForm(initialForm)
    }
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.companyId) return
    createMutation.mutate(form)
  }

  const totalRevenue = billings.filter(b => b.status === 'completed').reduce((sum, b) => sum + b.amount, 0)

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Billing & Payments</h1>
          <p className="text-gray-400">Total Revenue: <span className="text-green-400 font-semibold">${totalRevenue.toLocaleString()}</span></p>
        </div>
        <button 
          onClick={() => { setForm(initialForm); setShowModal(true) }}
          className="btn-primary flex items-center gap-2"
        >
          <Plus size={20} />
          Record Payment
        </button>
      </div>

      {isLoading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500"></div>
        </div>
      ) : (
        <div className="table-container">
          <table className="table">
            <thead>
              <tr>
                <th>Company</th>
                <th>Amount</th>
                <th>Method</th>
                <th>Reference</th>
                <th>Date</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-700">
              {billings.length === 0 ? (
                <tr>
                  <td colSpan={6} className="text-center text-gray-400 py-8">
                    No payments recorded
                  </td>
                </tr>
              ) : (
                billings.map((billing) => (
                  <tr key={billing.id}>
                    <td className="font-medium text-white">{billing.companyName}</td>
                    <td className="text-green-400 font-semibold">
                      ${billing.amount.toLocaleString()}
                    </td>
                    <td>{billing.paymentMethod}</td>
                    <td className="text-gray-400">{billing.paymentReference || '-'}</td>
                    <td>{new Date(billing.paymentDate).toLocaleDateString()}</td>
                    <td>
                      <span className={`px-2 py-1 rounded-full text-xs ${
                        billing.status === 'completed' ? 'bg-green-900 text-green-300' : 
                        billing.status === 'pending' ? 'bg-yellow-900 text-yellow-300' :
                        'bg-red-900 text-red-300'
                      }`}>
                        {billing.status}
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-gray-800 rounded-xl p-6 w-full max-w-lg">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-xl font-bold text-white">Record Payment</h2>
              <button onClick={() => setShowModal(false)} className="text-gray-400 hover:text-white">
                <X size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">Company *</label>
                <select
                  value={form.companyId || ''}
                  onChange={(e) => setForm({ ...form, companyId: e.target.value ? Number(e.target.value) : null })}
                  className="input"
                  required
                >
                  <option value="">Select Company</option>
                  {companies.map((company) => (
                    <option key={company.id} value={company.id}>{company.name}</option>
                  ))}
                </select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Amount *</label>
                  <input
                    type="number"
                    step="0.01"
                    value={form.amount}
                    onChange={(e) => setForm({ ...form, amount: parseFloat(e.target.value) })}
                    className="input"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Payment Method</label>
                  <select
                    value={form.paymentMethod}
                    onChange={(e) => setForm({ ...form, paymentMethod: e.target.value })}
                    className="input"
                  >
                    <option value="Cash">Cash</option>
                    <option value="BankTransfer">Bank Transfer</option>
                    <option value="CreditCard">Credit Card</option>
                    <option value="PayPal">PayPal</option>
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Payment Date</label>
                  <input
                    type="date"
                    value={form.paymentDate}
                    onChange={(e) => setForm({ ...form, paymentDate: e.target.value })}
                    className="input"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-1">Reference</label>
                  <input
                    type="text"
                    value={form.paymentReference}
                    onChange={(e) => setForm({ ...form, paymentReference: e.target.value })}
                    className="input"
                    placeholder="Transaction ID"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-300 mb-1">Notes</label>
                <textarea
                  value={form.notes}
                  onChange={(e) => setForm({ ...form, notes: e.target.value })}
                  className="input"
                  rows={2}
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button type="submit" className="btn-primary flex-1" disabled={createMutation.isPending}>
                  {createMutation.isPending ? 'Saving...' : 'Record Payment'}
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
