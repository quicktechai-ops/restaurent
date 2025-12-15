import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { rolesApi } from '../lib/api'
import { Plus, Edit, Trash2, Shield } from 'lucide-react'

export default function Roles() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ name: '', description: '', permissionIds: [] as number[] })

  const { data: roles, isLoading } = useQuery({ queryKey: ['roles'], queryFn: () => rolesApi.getAll() })
  const { data: permissions } = useQuery({ queryKey: ['permissions'], queryFn: () => rolesApi.getPermissions() })

  const createMutation = useMutation({ mutationFn: rolesApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['roles'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => rolesApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['roles'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: rolesApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['roles'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', description: '', permissionIds: [] }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  // Group permissions by prefix
  const groupedPermissions: Record<string, any[]> = {}
  permissions?.data?.forEach((p: any) => {
    const prefix = p.code.split('.')[0]
    if (!groupedPermissions[prefix]) groupedPermissions[prefix] = []
    groupedPermissions[prefix].push(p)
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Roles & Permissions</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Role</button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Role' : 'Add Role'}</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div><label className="label">Name *</label><input className="input" value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} required /></div>
              <div><label className="label">Description</label><input className="input" value={formData.description} onChange={e => setFormData({...formData, description: e.target.value})} /></div>
            </div>
            <div><label className="label">Permissions</label>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {Object.entries(groupedPermissions).map(([group, perms]) => (
                  <div key={group} className="border rounded p-3">
                    <h4 className="font-medium text-sm text-gray-700 mb-2">{group}</h4>
                    <div className="space-y-1">
                      {perms.map((p: any) => (
                        <label key={p.id} className="flex items-center gap-2 text-sm cursor-pointer">
                          <input type="checkbox" checked={formData.permissionIds.includes(p.id)} onChange={e => setFormData({...formData, permissionIds: e.target.checked ? [...formData.permissionIds, p.id] : formData.permissionIds.filter(id => id !== p.id)})} />
                          {p.code.split('.')[1] || p.code}
                        </label>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            </div>
            <div className="flex gap-3">
              <button type="submit" className="btn-primary">{editingId ? 'Update' : 'Create'}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        {isLoading ? <div className="p-8 text-center">Loading...</div> : (
          <table className="table">
            <thead><tr><th>Name</th><th>Description</th><th>Users</th><th>Permissions</th><th>Actions</th></tr></thead>
            <tbody>
              {roles?.data?.map((role: any) => (
                <tr key={role.id}>
                  <td className="font-medium flex items-center gap-2"><Shield size={18} className="text-primary-600" />{role.name}</td>
                  <td>{role.description || '-'}</td>
                  <td>{role.usersCount}</td>
                  <td><span className="text-sm text-gray-500">{role.permissions?.length || 0} permissions</span></td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(role.id); setFormData({ name: role.name, description: role.description || '', permissionIds: [] }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => confirm('Delete?') && deleteMutation.mutate(role.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
