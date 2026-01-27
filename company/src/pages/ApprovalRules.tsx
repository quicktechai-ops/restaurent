import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/api'
import { Plus, Edit, Trash2, Shield, Lock, Key } from 'lucide-react'

export default function ApprovalRules() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({
    ruleType: 'max_discount',
    roleId: '',
    threshold: 0,
    requireManagerPin: true,
    requireReason: true,
    isActive: true
  })

  const { data: rules = [], isLoading } = useQuery({ 
    queryKey: ['approval-rules'], 
    queryFn: () => api.get('/api/company/approval-rules').then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: roles = [] } = useQuery({ 
    queryKey: ['roles'], 
    queryFn: () => api.get('/api/company/roles').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/approval-rules', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['approval-rules'] }); resetForm() }
  })
  
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => api.put(`/api/company/approval-rules/${id}`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['approval-rules'] }); resetForm() }
  })
  
  const deleteMutation = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/approval-rules/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['approval-rules'] })
  })

  const resetForm = () => {
    setShowForm(false)
    setEditingId(null)
    setFormData({ ruleType: 'max_discount', roleId: '', threshold: 0, requireManagerPin: true, requireReason: true, isActive: true })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const data = { ...formData, roleId: formData.roleId ? parseInt(formData.roleId) : null }
    if (editingId) updateMutation.mutate({ id: editingId, data })
    else createMutation.mutate(data)
  }

  const handleEdit = (rule: any) => {
    setEditingId(rule.id)
    setFormData({
      ruleType: rule.ruleType,
      roleId: rule.roleId?.toString() || '',
      threshold: rule.threshold,
      requireManagerPin: rule.requireManagerPin,
      requireReason: rule.requireReason,
      isActive: rule.isActive
    })
    setShowForm(true)
  }

  const getRuleTypeLabel = (type: string) => {
    switch (type) {
      case 'max_discount': return 'Maximum Discount'
      case 'price_change': return 'Price Change in Order'
      case 'void_paid': return 'Void Paid Invoice'
      case 'refund': return 'Process Refund'
      case 'delete_order': return 'Delete Order'
      default: return type
    }
  }

  const getRuleTypeDescription = (type: string) => {
    switch (type) {
      case 'max_discount': return 'Maximum discount percentage allowed without manager approval'
      case 'price_change': return 'Changing item prices during order requires approval'
      case 'void_paid': return 'Voiding already paid invoices requires approval'
      case 'refund': return 'Processing refunds requires approval'
      case 'delete_order': return 'Deleting orders requires approval'
      default: return ''
    }
  }

  if (isLoading) return <div>Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><Shield size={28} /> Approval Rules</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Rule</button>
      </div>

      <div className="card p-4 mb-6 bg-blue-50 border-blue-200">
        <p className="text-sm text-blue-800">
          <strong>Approval rules</strong> control which actions require manager authorization. When a user attempts a restricted action that exceeds their limit, they'll need to enter a manager PIN.
        </p>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Create'} Approval Rule</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Rule Type *</label>
              <select value={formData.ruleType} onChange={(e) => setFormData({ ...formData, ruleType: e.target.value })} className="input" required>
                <option value="max_discount">Maximum Discount Without Approval</option>
                <option value="price_change">Price Change in Order</option>
                <option value="void_paid">Void Paid Invoice</option>
                <option value="refund">Process Refund</option>
                <option value="delete_order">Delete Order</option>
              </select>
              <p className="text-xs text-gray-500 mt-1">{getRuleTypeDescription(formData.ruleType)}</p>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Apply to Role</label>
              <select value={formData.roleId} onChange={(e) => setFormData({ ...formData, roleId: e.target.value })} className="input">
                <option value="">All Roles</option>
                {roles?.map((r: any) => <option key={r.id} value={r.id}>{r.name}</option>)}
              </select>
            </div>
            {formData.ruleType === 'max_discount' && (
              <div>
                <label className="block text-sm font-medium mb-1">Threshold (%) *</label>
                <input type="number" step="0.1" min="0" max="100" value={formData.threshold} onChange={(e) => setFormData({ ...formData, threshold: parseFloat(e.target.value) || 0 })} className="input" required />
                <p className="text-xs text-gray-500 mt-1">Discounts above this % require approval</p>
              </div>
            )}
            <div className="flex flex-col gap-3">
              <label className="flex items-center gap-2">
                <input type="checkbox" checked={formData.requireManagerPin} onChange={(e) => setFormData({ ...formData, requireManagerPin: e.target.checked })} />
                <Key size={16} /> Require Manager PIN
              </label>
              <label className="flex items-center gap-2">
                <input type="checkbox" checked={formData.requireReason} onChange={(e) => setFormData({ ...formData, requireReason: e.target.checked })} />
                Require Reason/Notes
              </label>
              <label className="flex items-center gap-2">
                <input type="checkbox" checked={formData.isActive} onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} />
                Active
              </label>
            </div>
            <div className="md:col-span-2 flex gap-2">
              <button type="submit" className="btn-primary">{editingId ? 'Update' : 'Create'} Rule</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">Rule Type</th>
              <th className="text-left p-3">Role</th>
              <th className="text-left p-3">Threshold</th>
              <th className="text-left p-3">Manager PIN</th>
              <th className="text-left p-3">Requires Reason</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {rules?.map((rule: any) => (
              <tr key={rule.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3">
                  <div className="flex items-center gap-2">
                    <Lock size={16} className="text-gray-400" />
                    <div>
                      <span className="font-medium">{getRuleTypeLabel(rule.ruleType)}</span>
                      <p className="text-xs text-gray-500">{getRuleTypeDescription(rule.ruleType)}</p>
                    </div>
                  </div>
                </td>
                <td className="p-3">{rule.roleName || 'All Roles'}</td>
                <td className="p-3">{rule.ruleType === 'max_discount' ? `${rule.threshold}%` : '-'}</td>
                <td className="p-3">{rule.requireManagerPin ? '✓ Yes' : '✗ No'}</td>
                <td className="p-3">{rule.requireReason ? '✓ Yes' : '✗ No'}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${rule.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'}`}>
                    {rule.isActive ? 'Active' : 'Inactive'}
                  </span>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => handleEdit(rule)} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(rule.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!rules || rules.length === 0) && <p className="text-center text-gray-500 py-8">No approval rules defined</p>}
      </div>
    </div>
  )
}
