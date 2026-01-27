import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { usersApi, rolesApi, branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, ToggleLeft, ToggleRight, Key, UserCheck } from 'lucide-react'

export default function Staff() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [branchFilter, setBranchFilter] = useState<number | undefined>()
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    fullName: '',
    email: '',
    phone: '',
    position: '',
    defaultBranchId: null as number | null,
    baseSalary: '',
    hireDate: ''
  })
  const [showRoleForm, setShowRoleForm] = useState(false)
  const [newRoleName, setNewRoleName] = useState('')
  const [showBranchForm, setShowBranchForm] = useState(false)
  const [newBranchName, setNewBranchName] = useState('')

  const { data: users, isLoading } = useQuery({ 
    queryKey: ['users', branchFilter], 
    queryFn: () => usersApi.getAll({ branchId: branchFilter }) 
  })
  const { data: roles } = useQuery({ queryKey: ['roles'], queryFn: () => rolesApi.getAll() })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ 
    mutationFn: usersApi.create, 
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['users'] }); resetForm() } 
  })
  const updateMutation = useMutation({ 
    mutationFn: ({ id, data }: { id: number; data: any }) => usersApi.update(id, data), 
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['users'] }); resetForm() } 
  })
  const deleteMutation = useMutation({ 
    mutationFn: usersApi.delete, 
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }) 
  })
  const toggleMutation = useMutation({ 
    mutationFn: usersApi.toggle, 
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }) 
  })
  const resetPasswordMutation = useMutation({ 
    mutationFn: ({ id, password }: { id: number; password: string }) => usersApi.resetPassword(id, password) 
  })
  const createRoleMutation = useMutation({
    mutationFn: (name: string) => rolesApi.create({ name, isActive: true }),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['roles'] }); setShowRoleForm(false); setNewRoleName('') }
  })
  const createBranchMutation = useMutation({
    mutationFn: (name: string) => branchesApi.create({ name, isActive: true }),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['branches'] }); setShowBranchForm(false); setNewBranchName('') }
  })

  const resetForm = () => { 
    setShowForm(false)
    setEditingId(null)
    setFormData({ 
      username: '', 
      password: '', 
      fullName: '', 
      email: '', 
      phone: '', 
      position: '', 
      defaultBranchId: null,
      baseSalary: '',
      hireDate: ''
    }) 
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    const data = {
      ...formData,
      baseSalary: formData.baseSalary ? parseFloat(formData.baseSalary) : null
    }
    if (editingId) updateMutation.mutate({ id: editingId, data })
    else createMutation.mutate(data)
  }

  const handleEdit = (user: any) => {
    setEditingId(user.id)
    setFormData({ 
      username: user.username, 
      password: '', 
      fullName: user.fullName, 
      email: user.email || '', 
      phone: user.phone || '', 
      position: user.position || '', 
      defaultBranchId: user.defaultBranchId,
      baseSalary: user.baseSalary?.toString() || '',
      hireDate: user.hireDate?.split('T')[0] || ''
    })
    setShowForm(true)
  }

  const handleResetPassword = (id: number) => {
    const password = prompt('Enter new password:')
    if (password) resetPasswordMutation.mutate({ id, password })
  }

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Staff Management</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Staff
        </button>
      </div>

      <div className="mb-4">
        <select 
          value={branchFilter || ''} 
          onChange={(e) => setBranchFilter(e.target.value ? parseInt(e.target.value) : undefined)} 
          className="input w-64"
        >
          <option value="">All Branches</option>
          {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Staff' : 'Add Staff'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="label">Username *</label>
              <input 
                className="input" 
                value={formData.username} 
                onChange={e => setFormData({...formData, username: e.target.value})} 
                required 
                disabled={!!editingId} 
                placeholder="Login username"
              />
            </div>
            {!editingId && (
              <div>
                <label className="label">Password *</label>
                <input 
                  type="password" 
                  className="input" 
                  value={formData.password} 
                  onChange={e => setFormData({...formData, password: e.target.value})} 
                  required 
                  placeholder="Login password"
                />
              </div>
            )}
            <div>
              <label className="label">Full Name *</label>
              <input 
                className="input" 
                value={formData.fullName} 
                onChange={e => setFormData({...formData, fullName: e.target.value})} 
                required 
              />
            </div>
            <div>
              <label className="label">Role *</label>
              {showRoleForm ? (
                <div className="flex gap-2">
                  <input type="text" placeholder="New role name..." value={newRoleName} onChange={e => setNewRoleName(e.target.value)} className="input flex-1" autoFocus />
                  <button type="button" onClick={() => newRoleName.trim() && createRoleMutation.mutate(newRoleName.trim())} disabled={createRoleMutation.isPending} className="px-3 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">{createRoleMutation.isPending ? '...' : '✓'}</button>
                  <button type="button" onClick={() => { setShowRoleForm(false); setNewRoleName('') }} className="px-3 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300">✕</button>
                </div>
              ) : (
                <div className="flex gap-2">
                  <select className="input flex-1" value={formData.position} onChange={e => setFormData({...formData, position: e.target.value})} required>
                    <option value="">-- Select Role --</option>
                    {roles?.data?.map((r: any) => <option key={r.id} value={r.name}>{r.name}</option>)}
                  </select>
                  <button type="button" onClick={() => setShowRoleForm(true)} className="px-3 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700" title="Add new role"><Plus size={20} /></button>
                </div>
              )}
            </div>
            <div>
              <label className="label">Branch *</label>
              {showBranchForm ? (
                <div className="flex gap-2">
                  <input type="text" placeholder="New branch name..." value={newBranchName} onChange={e => setNewBranchName(e.target.value)} className="input flex-1" autoFocus />
                  <button type="button" onClick={() => newBranchName.trim() && createBranchMutation.mutate(newBranchName.trim())} disabled={createBranchMutation.isPending} className="px-3 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">{createBranchMutation.isPending ? '...' : '✓'}</button>
                  <button type="button" onClick={() => { setShowBranchForm(false); setNewBranchName('') }} className="px-3 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300">✕</button>
                </div>
              ) : (
                <div className="flex gap-2">
                  <select className="input flex-1" value={formData.defaultBranchId || ''} onChange={e => setFormData({...formData, defaultBranchId: e.target.value ? parseInt(e.target.value) : null})} required>
                    <option value="">-- Select Branch --</option>
                    {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
                  </select>
                  <button type="button" onClick={() => setShowBranchForm(true)} className="px-3 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700" title="Add new branch"><Plus size={20} /></button>
                </div>
              )}
            </div>
            <div>
              <label className="label">Phone</label>
              <input 
                className="input" 
                value={formData.phone} 
                onChange={e => setFormData({...formData, phone: e.target.value})} 
              />
            </div>
            <div>
              <label className="label">Email</label>
              <input 
                type="email" 
                className="input" 
                value={formData.email} 
                onChange={e => setFormData({...formData, email: e.target.value})} 
              />
            </div>
            <div>
              <label className="label">Salary</label>
              <input 
                type="number" 
                className="input" 
                value={formData.baseSalary} 
                onChange={e => setFormData({...formData, baseSalary: e.target.value})} 
                placeholder="Monthly salary"
              />
            </div>
            <div>
              <label className="label">Hire Date</label>
              <input 
                type="date" 
                className="input" 
                value={formData.hireDate} 
                onChange={e => setFormData({...formData, hireDate: e.target.value})} 
              />
            </div>
            <div className="md:col-span-3 flex gap-3 mt-2">
              <button type="submit" className="btn-primary">{editingId ? 'Update' : 'Save'}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        {isLoading ? <div className="p-8 text-center">Loading...</div> : (
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Username</th>
                <th>Role</th>
                <th>Branch</th>
                <th>Phone</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users?.data?.map((user: any) => (
                <tr key={user.id}>
                  <td className="font-medium flex items-center gap-2">
                    <UserCheck size={18} className="text-primary-600" />
                    {user.fullName}
                  </td>
                  <td className="text-gray-500">{user.username}</td>
                  <td>{user.position || '-'}</td>
                  <td>{user.defaultBranchName || '-'}</td>
                  <td>{user.phone || '-'}</td>
                  <td>
                    <span className={`px-2 py-1 rounded-full text-xs ${user.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                      {user.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="flex gap-2">
                    <button onClick={() => handleEdit(user)} className="text-blue-600 hover:text-blue-800" title="Edit">
                      <Edit size={18} />
                    </button>
                    <button onClick={() => handleResetPassword(user.id)} className="text-yellow-600 hover:text-yellow-800" title="Reset Password">
                      <Key size={18} />
                    </button>
                    <button onClick={() => toggleMutation.mutate(user.id)} className="text-gray-600 hover:text-gray-900" title="Toggle Status">
                      {user.isActive ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}
                    </button>
                    <button onClick={() => confirm('Delete this staff member?') && deleteMutation.mutate(user.id)} className="text-red-600 hover:text-red-800" title="Delete">
                      <Trash2 size={18} />
                    </button>
                  </td>
                </tr>
              ))}
              {users?.data?.length === 0 && (
                <tr>
                  <td colSpan={7} className="text-center text-gray-500 py-8">No staff members found</td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
