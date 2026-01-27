import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { auditLogsApi, branchesApi, usersApi } from '../lib/api'
import { ChevronLeft, ChevronRight } from 'lucide-react'

export default function AuditLogs() {
  const [branchFilter, setBranchFilter] = useState<number | undefined>()
  const [userFilter, setUserFilter] = useState<number | undefined>()
  const [actionFilter, setActionFilter] = useState('')
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')
  const [page, setPage] = useState(1)

  const { data: logsData, isLoading } = useQuery({ 
    queryKey: ['audit-logs', branchFilter, userFilter, actionFilter, fromDate, toDate, page], 
    queryFn: () => auditLogsApi.getAll({ 
      branchId: branchFilter, 
      userId: userFilter, 
      actionType: actionFilter || undefined,
      fromDate: fromDate || undefined,
      toDate: toDate || undefined,
      page 
    }) 
  })
  const { data: branches } = useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() })
  const { data: users } = useQuery({ queryKey: ['users'], queryFn: () => usersApi.getAll() })
  const { data: actionTypes } = useQuery({ queryKey: ['audit-action-types'], queryFn: () => auditLogsApi.getActionTypes() })

  const logs = logsData?.data?.data || []
  const total = logsData?.data?.total || 0
  const pageSize = 50
  const totalPages = Math.ceil(total / pageSize)

  const formatDate = (date: string) => {
    return new Date(date).toLocaleString()
  }

  const getActionColor = (action: string) => {
    if (action.includes('Create') || action.includes('Add')) return 'bg-green-100 text-green-800'
    if (action.includes('Delete') || action.includes('Remove')) return 'bg-red-100 text-red-800'
    if (action.includes('Update') || action.includes('Edit')) return 'bg-blue-100 text-blue-800'
    if (action.includes('Login')) return 'bg-purple-100 text-purple-800'
    return 'bg-gray-100 text-gray-900'
  }

  if (isLoading) return <div>Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Audit Logs</h1>
        <span className="text-sm text-gray-500">{total} records</span>
      </div>

      {/* Filters */}
      <div className="card mb-6">
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <select value={branchFilter || ''} onChange={(e) => { setBranchFilter(e.target.value ? parseInt(e.target.value) : undefined); setPage(1) }} className="input">
            <option value="">All Branches</option>
            {branches?.data?.map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
          </select>
          <select value={userFilter || ''} onChange={(e) => { setUserFilter(e.target.value ? parseInt(e.target.value) : undefined); setPage(1) }} className="input">
            <option value="">All Users</option>
            {users?.data?.map((u: any) => <option key={u.id} value={u.id}>{u.fullName}</option>)}
          </select>
          <select value={actionFilter} onChange={(e) => { setActionFilter(e.target.value); setPage(1) }} className="input">
            <option value="">All Actions</option>
            {actionTypes?.data?.map((t: string) => <option key={t} value={t}>{t}</option>)}
          </select>
          <input type="date" value={fromDate} onChange={(e) => { setFromDate(e.target.value); setPage(1) }} className="input" placeholder="From Date" />
          <input type="date" value={toDate} onChange={(e) => { setToDate(e.target.value); setPage(1) }} className="input" placeholder="To Date" />
        </div>
      </div>

      {/* Logs Table */}
      <div className="card">
        <table className="table">
          <thead>
            <tr className="border-b border-gray-700">
              <th className="text-left p-3">Timestamp</th>
              <th className="text-left p-3">User</th>
              <th className="text-left p-3">Branch</th>
              <th className="text-left p-3">Action</th>
              <th className="text-left p-3">Entity</th>
              <th className="text-left p-3">IP</th>
            </tr>
          </thead>
          <tbody>
            {logs.map((log: any) => (
              <tr key={log.id} className="border-b hover:bg-gray-800/50">
                <td className="p-3 text-sm">{formatDate(log.timestamp)}</td>
                <td className="p-3">{log.username || '-'}</td>
                <td className="p-3">{log.branchName || '-'}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${getActionColor(log.actionType)}`}>
                    {log.actionType}
                  </span>
                </td>
                <td className="p-3 text-sm">
                  {log.entityName && <span>{log.entityName} #{log.entityId}</span>}
                </td>
                <td className="p-3 text-sm text-gray-500">{log.ipAddress || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
        
        {logs.length === 0 && <p className="text-center text-gray-500 py-8">No audit logs found</p>}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex justify-between items-center mt-4 pt-4 border-t">
            <span className="text-sm text-gray-600">
              Page {page} of {totalPages}
            </span>
            <div className="flex gap-2">
              <button 
                onClick={() => setPage(p => Math.max(1, p - 1))} 
                disabled={page === 1}
                className="btn-secondary flex items-center gap-1 disabled:opacity-50"
              >
                <ChevronLeft size={16} /> Previous
              </button>
              <button 
                onClick={() => setPage(p => Math.min(totalPages, p + 1))} 
                disabled={page === totalPages}
                className="btn-secondary flex items-center gap-1 disabled:opacity-50"
              >
                Next <ChevronRight size={16} />
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
