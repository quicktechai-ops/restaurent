import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/api'
import { Clock, LogIn, LogOut, Calendar, User } from 'lucide-react'

export default function Attendance() {
  const queryClient = useQueryClient()
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0])
  const [selectedEmployee, setSelectedEmployee] = useState('')

  const { data: attendance = [], isLoading } = useQuery({ 
    queryKey: ['attendance', selectedDate, selectedEmployee], 
    queryFn: () => api.get('/api/company/attendance', { params: { date: selectedDate, employeeId: selectedEmployee || undefined } }).then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: employees = [] } = useQuery({ 
    queryKey: ['employees'], 
    queryFn: () => api.get('/api/company/employees').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const clockInMutation = useMutation({
    mutationFn: (employeeId: number) => api.post(`/api/company/attendance/clock-in`, { employeeId }),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['attendance'] })
  })

  const clockOutMutation = useMutation({
    mutationFn: (attendanceId: number) => api.patch(`/api/company/attendance/${attendanceId}/clock-out`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['attendance'] })
  })

  const formatTime = (dateStr: string | null) => {
    if (!dateStr) return '-'
    return new Date(dateStr).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  }

  const calculateHours = (clockIn: string, clockOut: string | null) => {
    if (!clockOut) return '-'
    const diff = new Date(clockOut).getTime() - new Date(clockIn).getTime()
    const hours = Math.floor(diff / (1000 * 60 * 60))
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
    return `${hours}h ${minutes}m`
  }

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800 flex items-center gap-2"><Clock size={28} /> Attendance</h1>
      </div>

      {/* Quick Clock In/Out */}
      <div className="card p-4 mb-6">
        <h2 className="font-semibold mb-4">Quick Clock In/Out</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-3">
          {employees?.filter((e: any) => e.isActive).map((emp: any) => {
            const todayRecord = attendance?.find((a: any) => a.employeeId === emp.id && !a.clockOut)
            return (
              <div key={emp.id} className="border rounded-lg p-3 text-center">
                <div className="w-10 h-10 bg-primary-100 rounded-full flex items-center justify-center mx-auto mb-2">
                  <User size={20} className="text-primary-600" />
                </div>
                <p className="font-medium text-sm mb-2">{emp.fullName}</p>
                {todayRecord ? (
                  <button onClick={() => clockOutMutation.mutate(todayRecord.id)} className="w-full py-1.5 bg-red-100 text-red-700 rounded text-sm flex items-center justify-center gap-1 hover:bg-red-200">
                    <LogOut size={14} /> Clock Out
                  </button>
                ) : (
                  <button onClick={() => clockInMutation.mutate(emp.id)} className="w-full py-1.5 bg-green-100 text-green-700 rounded text-sm flex items-center justify-center gap-1 hover:bg-green-200">
                    <LogIn size={14} /> Clock In
                  </button>
                )}
              </div>
            )
          })}
        </div>
      </div>

      {/* Filters */}
      <div className="card p-4 mb-6">
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2">
            <Calendar size={18} className="text-gray-500" />
            <input type="date" value={selectedDate} onChange={(e) => setSelectedDate(e.target.value)} className="input-field" />
          </div>
          <select value={selectedEmployee} onChange={(e) => setSelectedEmployee(e.target.value)} className="input-field">
            <option value="">All Employees</option>
            {employees?.map((e: any) => <option key={e.id} value={e.id}>{e.fullName}</option>)}
          </select>
        </div>
      </div>

      {/* Attendance Table */}
      <div className="card overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="text-left p-3">Employee</th>
              <th className="text-left p-3">Date</th>
              <th className="text-left p-3">Clock In</th>
              <th className="text-left p-3">Clock Out</th>
              <th className="text-left p-3">Hours Worked</th>
              <th className="text-left p-3">Status</th>
            </tr>
          </thead>
          <tbody>
            {attendance?.map((record: any) => (
              <tr key={record.id} className="border-t hover:bg-gray-50">
                <td className="p-3 flex items-center gap-2"><User size={16} className="text-gray-400" /> {record.employeeName}</td>
                <td className="p-3">{new Date(record.date).toLocaleDateString()}</td>
                <td className="p-3">
                  <span className="flex items-center gap-1 text-green-600"><LogIn size={14} /> {formatTime(record.clockIn)}</span>
                </td>
                <td className="p-3">
                  {record.clockOut ? (
                    <span className="flex items-center gap-1 text-red-600"><LogOut size={14} /> {formatTime(record.clockOut)}</span>
                  ) : (
                    <span className="text-yellow-600">Still Working</span>
                  )}
                </td>
                <td className="p-3 font-medium">{calculateHours(record.clockIn, record.clockOut)}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${record.clockOut ? 'bg-gray-100 text-gray-700' : 'bg-green-100 text-green-700'}`}>
                    {record.clockOut ? 'Completed' : 'Active'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!attendance || attendance.length === 0) && <p className="text-center text-gray-500 py-8">No attendance records for this date</p>}
      </div>

      {/* Summary */}
      {attendance && attendance.length > 0 && (
        <div className="card p-4 mt-6">
          <h3 className="font-semibold mb-3">Day Summary</h3>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-center">
            <div>
              <p className="text-2xl font-bold text-primary-600">{attendance.length}</p>
              <p className="text-sm text-gray-500">Total Records</p>
            </div>
            <div>
              <p className="text-2xl font-bold text-green-600">{attendance.filter((a: any) => !a.clockOut).length}</p>
              <p className="text-sm text-gray-500">Currently Working</p>
            </div>
            <div>
              <p className="text-2xl font-bold text-gray-600">{attendance.filter((a: any) => a.clockOut).length}</p>
              <p className="text-sm text-gray-500">Completed Shifts</p>
            </div>
            <div>
              <p className="text-2xl font-bold text-blue-600">
                {attendance.reduce((total: number, a: any) => {
                  if (!a.clockOut) return total
                  const diff = new Date(a.clockOut).getTime() - new Date(a.clockIn).getTime()
                  return total + diff / (1000 * 60 * 60)
                }, 0).toFixed(1)}h
              </p>
              <p className="text-sm text-gray-500">Total Hours</p>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
