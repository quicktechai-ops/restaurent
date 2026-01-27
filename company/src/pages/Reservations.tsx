import { useState, useMemo } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { reservationsApi, branchesApi, tablesApi } from '../lib/api'
import { Plus, Edit, Trash2, Clock, Users } from 'lucide-react'

type DateRangeType = 'day' | 'week' | 'month'

const getDateRange = (type: DateRangeType, baseDate: string) => {
  const date = new Date(baseDate)
  let startDate: Date, endDate: Date
  
  switch (type) {
    case 'week':
      const dayOfWeek = date.getDay()
      startDate = new Date(date)
      startDate.setDate(date.getDate() - dayOfWeek)
      endDate = new Date(startDate)
      endDate.setDate(startDate.getDate() + 6)
      break
    case 'month':
      startDate = new Date(date.getFullYear(), date.getMonth(), 1)
      endDate = new Date(date.getFullYear(), date.getMonth() + 1, 0)
      break
    default: // day
      startDate = date
      endDate = date
  }
  
  return {
    startDate: startDate.toISOString().split('T')[0],
    endDate: endDate.toISOString().split('T')[0]
  }
}

export default function Reservations() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [dateFilter, setDateFilter] = useState(new Date().toISOString().split('T')[0])
  const [dateRangeType, setDateRangeType] = useState<DateRangeType>('day')
  const [branchFilter, setBranchFilter] = useState<number | undefined>()
  const [formData, setFormData] = useState({ branchId: 0, customerName: '', customerPhone: '', reservationDate: '', startTime: '', durationMinutes: 90, partySize: 2, tableId: '', channel: 'Phone', notes: '' })

  const dateRange = useMemo(() => getDateRange(dateRangeType, dateFilter), [dateRangeType, dateFilter])

  const { data: reservations, isLoading } = useQuery({ 
    queryKey: ['reservations', dateRange.startDate, dateRange.endDate, branchFilter], 
    queryFn: () => reservationsApi.getAll({ 
      startDate: dateRange.startDate, 
      endDate: dateRange.endDate, 
      branchId: branchFilter 
    }) 
  })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })
  const { data: tables } = useQuery({ queryKey: ['tables', formData.branchId], queryFn: () => tablesApi.getAll(formData.branchId || undefined), enabled: !!formData.branchId })

  const createMutation = useMutation({ 
    mutationFn: reservationsApi.create, 
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['reservations'] }); resetForm() },
    onError: (err: any) => { alert(err?.response?.data?.message || err.message || 'Error creating reservation') }
  })
  const updateMutation = useMutation({ 
    mutationFn: ({ id, data }: { id: number; data: any }) => reservationsApi.update(id, data), 
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['reservations'] }); resetForm() },
    onError: (err: any) => { alert(err?.response?.data?.message || err.message || 'Error updating reservation') }
  })
  const deleteMutation = useMutation({ mutationFn: reservationsApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['reservations'] }) })
  const statusMutation = useMutation({ mutationFn: ({ id, status }: { id: number; status: string }) => reservationsApi.updateStatus(id, status), onSuccess: () => queryClient.invalidateQueries({ queryKey: ['reservations'] }) })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ branchId: 0, customerName: '', customerPhone: '', reservationDate: '', startTime: '', durationMinutes: 90, partySize: 2, tableId: '', channel: 'Phone', notes: '' }) }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const data = { 
      ...formData, 
      tableId: formData.tableId ? parseInt(formData.tableId) : null,
      startTime: formData.startTime + ':00' // Convert "19:00" to "19:00:00" for TimeSpan
    }
    try {
      if (editingId) {
        updateMutation.mutate({ id: editingId, data })
      } else {
        await reservationsApi.create(data)
        queryClient.invalidateQueries({ queryKey: ['reservations'] })
        setDateFilter(formData.reservationDate) // Switch to the reservation's date
        resetForm()
      }
    } catch (err: any) {
      alert(err?.response?.data?.message || err?.response?.data?.inner || err.message || 'Error creating reservation')
    }
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Confirmed': return 'bg-green-100 text-green-800'
      case 'Seated': return 'bg-blue-100 text-blue-800'
      case 'Canceled': return 'bg-red-100 text-red-800'
      case 'NoShow': return 'bg-gray-100 text-gray-900'
      default: return 'bg-yellow-100 text-yellow-800'
    }
  }

  if (isLoading) return <div className="text-gray-400">Loading...</div>

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Reservations</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> New Reservation
        </button>
      </div>

      <div className="flex gap-4 mb-4 flex-wrap">
        <div className="flex rounded-lg overflow-hidden border border-gray-600">
          <button 
            onClick={() => setDateRangeType('day')} 
            className={`px-3 py-2 text-sm ${dateRangeType === 'day' ? 'bg-primary-600 text-white' : 'bg-gray-800 text-gray-300 hover:bg-gray-700'}`}
          >
            Day
          </button>
          <button 
            onClick={() => setDateRangeType('week')} 
            className={`px-3 py-2 text-sm ${dateRangeType === 'week' ? 'bg-primary-600 text-white' : 'bg-gray-800 text-gray-300 hover:bg-gray-700'}`}
          >
            Week
          </button>
          <button 
            onClick={() => setDateRangeType('month')} 
            className={`px-3 py-2 text-sm ${dateRangeType === 'month' ? 'bg-primary-600 text-white' : 'bg-gray-800 text-gray-300 hover:bg-gray-700'}`}
          >
            Month
          </button>
        </div>
        <input type="date" value={dateFilter} onChange={(e) => setDateFilter(e.target.value)} className="input" />
        {dateRangeType !== 'day' && (
          <span className="text-gray-400 text-sm self-center">
            {dateRange.startDate} to {dateRange.endDate}
          </span>
        )}
        <select value={branchFilter || ''} onChange={(e) => setBranchFilter(e.target.value ? parseInt(e.target.value) : undefined)} className="input">
          <option value="">All Branches</option>
          {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'New'} Reservation</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <select value={formData.branchId} onChange={(e) => setFormData({ ...formData, branchId: parseInt(e.target.value) })} className="input" required>
              <option value="">Select Branch *</option>
              {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
            </select>
            <input type="text" placeholder="Customer Name *" value={formData.customerName} onChange={(e) => setFormData({ ...formData, customerName: e.target.value })} className="input" required />
            <input type="tel" placeholder="Phone *" value={formData.customerPhone} onChange={(e) => setFormData({ ...formData, customerPhone: e.target.value })} className="input" required />
            <input type="date" value={formData.reservationDate} onChange={(e) => setFormData({ ...formData, reservationDate: e.target.value })} className="input" required />
            <input type="time" value={formData.startTime} onChange={(e) => setFormData({ ...formData, startTime: e.target.value })} className="input" required />
            <input type="number" placeholder="Duration (mins)" value={formData.durationMinutes} onChange={(e) => setFormData({ ...formData, durationMinutes: parseInt(e.target.value) || 90 })} className="input" />
            <input type="number" placeholder="Party Size" value={formData.partySize} onChange={(e) => setFormData({ ...formData, partySize: parseInt(e.target.value) || 2 })} className="input" required />
            <select value={formData.tableId} onChange={(e) => setFormData({ ...formData, tableId: e.target.value })} className="input">
              <option value="">Select Table (Optional)</option>
              {tables?.data?.map((t: any) => <option key={t.id} value={t.id}>{t.tableName} ({t.capacity} seats)</option>)}
            </select>
            <select value={formData.channel} onChange={(e) => setFormData({ ...formData, channel: e.target.value })} className="input">
              <option value="Phone">Phone</option>
              <option value="WalkIn">Walk-In</option>
              <option value="Online">Online</option>
            </select>
            <textarea placeholder="Notes" value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} className="input md:col-span-3" rows={2} />
            <div className="md:col-span-3 flex gap-2">
              <button type="submit" className="btn-primary" disabled={createMutation.isPending || updateMutation.isPending}>
                {createMutation.isPending || updateMutation.isPending ? 'Saving...' : 'Save'}
              </button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="card">
        <table className="table">
          <thead>
            <tr className="border-b border-gray-700">
              <th className="text-left p-3">Time</th>
              <th className="text-left p-3">Customer</th>
              <th className="text-left p-3">Party</th>
              <th className="text-left p-3">Table</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {reservations?.data?.map((res: any) => (
              <tr key={res.id} className="border-b hover:bg-gray-800/50">
                <td className="p-3 flex items-center gap-2"><Clock size={16} className="text-gray-400" /> {res.startTime?.substring(0, 5)}</td>
                <td className="p-3">{res.customerName} <br /><span className="text-sm text-gray-500">{res.customerPhone}</span></td>
                <td className="p-3 flex items-center gap-1"><Users size={14} /> {res.partySize}</td>
                <td className="p-3">{res.tableName || '-'}</td>
                <td className="p-3">
                  <select value={res.status} onChange={(e) => statusMutation.mutate({ id: res.id, status: e.target.value })} className={`px-2 py-1 rounded text-xs border-0 ${getStatusColor(res.status)}`}>
                    <option value="Pending">Pending</option>
                    <option value="Confirmed">Confirmed</option>
                    <option value="Seated">Seated</option>
                    <option value="Canceled">Canceled</option>
                    <option value="NoShow">No Show</option>
                  </select>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <button onClick={() => { setEditingId(res.id); setFormData({ branchId: res.branchId, customerName: res.customerName || '', customerPhone: res.customerPhone || '', reservationDate: res.reservationDate?.split('T')[0] || '', startTime: res.startTime?.substring(0, 5) || '', durationMinutes: res.durationMinutes, partySize: res.partySize, tableId: res.tableId?.toString() || '', channel: res.channel, notes: res.notes || '' }); setShowForm(true) }} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(res.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {reservations?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No reservations for this date</p>}
      </div>
    </div>
  )
}
