import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { usersApi, rolesApi, branchesApi } from '../lib/api'
import { Plus, Edit, Trash2, ToggleLeft, ToggleRight, Key, Users as UsersIcon } from 'lucide-react'

export default function Users() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [formData, setFormData] = useState({ username: '', password: '', fullName: '', email: '', phone: '', position: '', defaultBranchId: null as number | null, roleIds: [] as number[] })

  const { data: users, isLoading } = useQuery({ queryKey: ['users'], queryFn: () => usersApi.getAll() })
  const { data: roles } = useQuery({ queryKey: ['roles'], queryFn: () => rolesApi.getAll() })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })

  const createMutation = useMutation({ mutationFn: usersApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['users'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => usersApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['users'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: usersApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }) })
  const toggleMutation = useMutation({ mutationFn: usersApi.toggle, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['users'] }) })
  const resetPasswordMutation = useMutation({ mutationFn: ({ id, password }: { id: number; password: string }) => usersApi.resetPassword(id, password) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ username: '', password: '', fullName: '', email: '', phone: '', position: '', defaultBranchId: null, roleIds: [] }) }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  const handleResetPassword = (id: number) => {
    const password = prompt('Enter new password:')
    if (password) resetPasswordMutation.mutate({ id, password })
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{t('users.title')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('users.addUser')}</button>
      </div>

      {showForm && (
        <div className="card p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? t('users.editUser') : t('users.addUser')}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div><label className="label">{t('users.username')} *</label><input className="input" value={formData.username} onChange={e => setFormData({...formData, username: e.target.value})} required disabled={!!editingId} /></div>
            {!editingId && <div><label className="label">{t('users.password')} *</label><input type="password" className="input" value={formData.password} onChange={e => setFormData({...formData, password: e.target.value})} required /></div>}
            <div><label className="label">{t('users.fullName')} *</label><input className="input" value={formData.fullName} onChange={e => setFormData({...formData, fullName: e.target.value})} required /></div>
            <div><label className="label">{t('common.email')}</label><input type="email" className="input" value={formData.email} onChange={e => setFormData({...formData, email: e.target.value})} /></div>
            <div><label className="label">{t('common.phone')}</label><input className="input" value={formData.phone} onChange={e => setFormData({...formData, phone: e.target.value})} /></div>
            <div><label className="label">{t('users.role')} *</label>
              <select className="input" value={formData.position} onChange={e => setFormData({...formData, position: e.target.value})} required>
                <option value="">-- {t('common.select')} --</option>
                {roles?.data?.map((r: any) => <option key={r.id} value={r.name}>{r.name}</option>)}
              </select>
            </div>
            <div><label className="label">{t('users.defaultBranch')}</label>
              <select className="input" value={formData.defaultBranchId || ''} onChange={e => setFormData({...formData, defaultBranchId: e.target.value ? parseInt(e.target.value) : null})}>
                <option value="">-- {t('common.select')} --</option>
                {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
              </select>
            </div>
            <div className="md:col-span-2 flex gap-3">
              <button type="submit" className="btn-primary">{editingId ? t('common.update') : t('common.create')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
            </div>
          </form>
        </div>
      )}

      <div className="card overflow-hidden">
        {isLoading ? <div className="p-8 text-center">{t('common.loading')}</div> : (
          <table className="table">
            <thead><tr><th>{t('users.username')}</th><th>{t('common.name')}</th><th>{t('users.role')}</th><th>{t('users.defaultBranch')}</th><th>{t('common.status')}</th><th>{t('common.actions')}</th></tr></thead>
            <tbody>
              {users?.data?.map((user: any) => (
                <tr key={user.id}>
                  <td className="font-medium flex items-center gap-2"><UsersIcon size={18} className="text-primary-600" />{user.username}</td>
                  <td>{user.fullName}</td>
                  <td>{user.position || '-'}</td>
                  <td>{user.defaultBranchName || '-'}</td>
                  <td><span className={`px-2 py-1 rounded-full text-xs ${user.isActive ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>{user.isActive ? t('common.active') : t('common.inactive')}</span></td>
                  <td className="flex gap-2">
                    <button onClick={() => { setEditingId(user.id); setFormData({ username: user.username, password: '', fullName: user.fullName, email: user.email || '', phone: user.phone || '', position: user.position || '', defaultBranchId: user.defaultBranchId, roleIds: [] }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={18} /></button>
                    <button onClick={() => handleResetPassword(user.id)} className="text-yellow-600 hover:text-yellow-800"><Key size={18} /></button>
                    <button onClick={() => toggleMutation.mutate(user.id)} className="text-gray-600 hover:text-gray-900">{user.isActive ? <ToggleRight size={18} /> : <ToggleLeft size={18} />}</button>
                    <button onClick={() => confirm(t('common.confirmDelete')) && deleteMutation.mutate(user.id)} className="text-red-600 hover:text-red-800"><Trash2 size={18} /></button>
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
