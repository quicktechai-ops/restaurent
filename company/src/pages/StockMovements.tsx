import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { ArrowUpDown, ArrowUp, ArrowDown, Package, Filter } from 'lucide-react'

export default function StockMovements() {
  const { t } = useTranslation()
  const [filters, setFilters] = useState({ itemId: '', type: '', dateFrom: '', dateTo: '' })

  const { data: movements = [], isLoading } = useQuery({ 
    queryKey: ['stock-movements', filters], 
    queryFn: () => api.get('/api/company/stock-movements', { params: filters }).then(r => Array.isArray(r.data) ? r.data : []) 
  })
  
  const { data: inventoryItems = [] } = useQuery({ 
    queryKey: ['inventory'], 
    queryFn: () => api.get('/api/company/inventory').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const getTypeIcon = (type: string) => {
    if (type.includes('IN')) return <ArrowDown className="text-green-600" size={16} />
    if (type.includes('OUT')) return <ArrowUp className="text-red-600" size={16} />
    return <ArrowUpDown className="text-blue-600" size={16} />
  }

  const getTypeColor = (type: string) => {
    if (type.includes('IN')) return 'bg-green-100 text-green-700'
    if (type.includes('OUT')) return 'bg-red-100 text-red-700'
    return 'bg-blue-100 text-blue-700'
  }

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><ArrowUpDown size={28} /> {t('inventory.stockMovements')}</h1>
      </div>

      {/* Filters */}
      <div className="card p-4 mb-6">
        <div className="flex items-center gap-2 mb-3">
          <Filter size={18} className="text-gray-500" />
          <span className="font-medium">{t('common.filter')}</span>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <select value={filters.itemId} onChange={(e) => setFilters({ ...filters, itemId: e.target.value })} className="input">
            <option value="">{t('common.all')} {t('common.items')}</option>
            {inventoryItems?.map((item: any) => <option key={item.id} value={item.id}>{item.name}</option>)}
          </select>
          <select value={filters.type} onChange={(e) => setFilters({ ...filters, type: e.target.value })} className="input">
            <option value="">{t('stockMovements.allTypes')}</option>
            <option value="IN-Purchase">{t('stockMovements.inPurchase')}</option>
            <option value="IN-Adjustment">{t('stockMovements.inAdjustment')}</option>
            <option value="IN-Transfer">{t('stockMovements.inTransfer')}</option>
            <option value="OUT-Sales">{t('stockMovements.outSales')}</option>
            <option value="OUT-Waste">{t('stockMovements.outWaste')}</option>
            <option value="OUT-Adjustment">{t('stockMovements.outAdjustment')}</option>
            <option value="OUT-Transfer">{t('stockMovements.outTransfer')}</option>
          </select>
          <input type="date" value={filters.dateFrom} onChange={(e) => setFilters({ ...filters, dateFrom: e.target.value })} className="input" placeholder={t('stockMovements.fromDate')} />
          <input type="date" value={filters.dateTo} onChange={(e) => setFilters({ ...filters, dateTo: e.target.value })} className="input" placeholder={t('stockMovements.toDate')} />
        </div>
      </div>

      {/* Movements Table */}
      <div className="card overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="text-left p-3">{t('stockMovements.date')}</th>
              <th className="text-left p-3">{t('stockMovements.item')}</th>
              <th className="text-left p-3">{t('common.type')}</th>
              <th className="text-left p-3">{t('common.quantity')}</th>
              <th className="text-left p-3">{t('stockMovements.unitCost')}</th>
              <th className="text-left p-3">{t('stockMovements.reference')}</th>
              <th className="text-left p-3">{t('stockMovements.notes')}</th>
            </tr>
          </thead>
          <tbody>
            {movements?.map((m: any) => (
              <tr key={m.id} className="border-t hover:bg-gray-800/50">
                <td className="p-3">{new Date(m.createdAt).toLocaleString()}</td>
                <td className="p-3 flex items-center gap-2"><Package size={16} className="text-gray-400" /> {m.itemName}</td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs flex items-center gap-1 w-fit ${getTypeColor(m.movementType)}`}>
                    {getTypeIcon(m.movementType)} {m.movementType}
                  </span>
                </td>
                <td className="p-3 font-medium">
                  <span className={m.quantity > 0 ? 'text-green-600' : 'text-red-600'}>
                    {m.quantity > 0 ? '+' : ''}{m.quantity} {m.unit}
                  </span>
                </td>
                <td className="p-3">${m.unitCost?.toFixed(2) || '0.00'}</td>
                <td className="p-3 text-sm text-gray-600">{m.reference || '-'}</td>
                <td className="p-3 text-sm text-gray-600">{m.notes || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {(!movements || movements.length === 0) && <p className="text-center text-gray-500 py-8">{t('stockMovements.noMovements')}</p>}
      </div>
    </div>
  )
}
