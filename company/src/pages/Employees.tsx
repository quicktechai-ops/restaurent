import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { employeesApi, branchesApi, usersApi } from '../lib/api'
import { Plus, Edit, Trash2, UserCheck } from 'lucide-react'

export default function Employees() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [branchFilter, setBranchFilter] = useState<number | undefined>()
  const [formData, setFormData] = useState({ branchId: 0, fullName: '', position: '', phone: '', email: '', baseSalary: '', hireDate: '', userId: '' })

  const { data: employees, isLoading } = useQuery({ queryKey: ['employees', branchFilter], queryFn: () => employeesApi.getAll({ branchId: branchFilter }) })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })
  const { data: users } = useQuery({ queryKey: ['users'], queryFn: () => usersApi.getAll() })

  const createMutation = useMutation({ mutationFn: employeesApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['employees'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => employeesApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['employees'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: employeesApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['employees'] }) })
  const toggleMutation = useMutation({ mutationFn: employeesApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['employees'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ branchId: 0, fullName: '', position: '', phone: '', email: '', baseSalary: '', hireDate: '', userId: '' }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const data = { ...formData, baseSalary: formData.baseSalary ? parseFloat(formData.baseSalary) : null, userId: formData.userId ? parseInt(formData.userId) : null }
    if (editingId) updateMutation.mutate({ id: editingId, data })
    else createMutation.mutate(data)
  }

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Employees</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Employee
        </button>
      </div>

      <div className="mb-4">
        <select value={branchFilter || ''} onChange={(e) => setBranchFilter(e.target.value ? parseInt(e.target.value) : undefined)} className="input-field w-64">
          <option value="">All Branches</option>
          {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Add'} Employee</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <select value={formData.branchId} onChange={(e) => setFormData({ ...formData, branchId: parseInt(e.target.value) })} className="input-field" required>
              <option value="">Select Branch *</option>
              {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
            <input type="text" placeholder="Full Name *" value={formData.fullName} onChange={(e) => setFormData({ ...formData, fullName: e.target.value })} className="input-field" required />
            <input type="text" placeholder="Position *" value={formData.position} onChange={(e) => setFormData({ ...formData, position: e.target.value })} className="input-field" required />
            <input type="tel" placeholder="Phone" value={formData.phone} onChange={(e) => setFormData({ ...formData, phone: e.target.value })} className="input-field" />
            <input type="email" placeholder="Email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} className="input-field" />
            <input type="number" step="0.01" placeholder="Base Salary" value={formData.baseSalary} onChange={(e) => setFormData({ ...formData, baseSalary: e.target.value })} className="input-field" />
            <input type="date" placeholder="Hire Date" value={formData.hireDate} onChange={(e) => setFormData({ ...formData, hireDate: e.target.value })} className="input-field" />
            <select value={formData.userId} onChange={(e) => setFormData({ ...formData, userId: e.target.value })} className="input-field">
              <option value="">Link to User (Optional)</option>
              {users?.data?.map((u: any) => <option key={u.id} value={u.id}>{u.fullName}</option>)}
            </select>
            <div className="md:col-span-3 flex gap-2">
              <button type="submit" className="btn-primary">Save</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <table className="w-full">
          <thead>
            <tr className="border-b">
              <th className="text-left p-3">Name</th>
              <th className="text-left p-3">Position</th>
              <th className="text-left p-3">Branch</th>
              <th className="text-left p-3">Phone</th>
              <th className="text-left p-3">Salary</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {employees?.data?.map((emp: any) => (
              <tr key={emp.id} className="border-b hover:bg-gray-50">
                <td className="p-3 flex items-center gap-2"><UserCheck size={16} className="text-gray-400" /> {emp.fullName}</td>
                <td className="p-3">{emp.position}</td>
                <td className="p-3">{emp.branchName}</td>
                <td className="p-3">{emp.phone || '-'}</td>
                <td className="p-3">{emp.baseSalary ? `$${emp.baseSalary.toFixed(2)}` : '-'}</td>
                <td className="p-3">
                  <button onClick={() => toggleMutation.mutate(emp.id)} className={`px-2 py-1 rounded text-xs ${emp.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {emp.isActive ? 'Active' : 'Inactive'}
                  </button>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => { setEditingId(emp.id); setFormData({ branchId: emp.branchId, fullName: emp.fullName, position: emp.position, phone: emp.phone || '', email: emp.email || '', baseSalary: emp.baseSalary?.toString() || '', hireDate: emp.hireDate?.split('T')[0] || '', userId: emp.userId?.toString() || '' }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(emp.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {employees?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No employees found</p>}
      </div>
    </div>
  )
}
