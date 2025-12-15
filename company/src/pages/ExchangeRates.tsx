import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { exchangeRatesApi, currenciesApi } from '../lib/api';
import { Plus, Edit, Trash2 } from 'lucide-react';

export default function ExchangeRates() {
  const queryClient = useQueryClient();
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [formData, setFormData] = useState({
    baseCurrencyCode: '',
    targetCurrencyCode: '',
    rate: 1,
    validFrom: new Date().toISOString().split('T')[0],
    validTo: ''
  });

  const { data: rates = [], isLoading } = useQuery({
    queryKey: ['exchange-rates'],
    queryFn: exchangeRatesApi.getAll
  });

  const { data: currencies = [] } = useQuery({
    queryKey: ['currencies'],
    queryFn: currenciesApi.getAll
  });

  const createMutation = useMutation({
    mutationFn: exchangeRatesApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['exchange-rates'] });
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => exchangeRatesApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['exchange-rates'] });
      resetForm();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: exchangeRatesApi.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['exchange-rates'] })
  });

  const resetForm = () => {
    setShowForm(false);
    setEditingId(null);
    setFormData({
      baseCurrencyCode: '',
      targetCurrencyCode: '',
      rate: 1,
      validFrom: new Date().toISOString().split('T')[0],
      validTo: ''
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const data = {
      ...formData,
      validTo: formData.validTo || null
    };
    if (editingId) {
      updateMutation.mutate({ id: editingId, data });
    } else {
      createMutation.mutate(data);
    }
  };

  const handleEdit = (rate: any) => {
    setEditingId(rate.id);
    setFormData({
      baseCurrencyCode: rate.baseCurrencyCode,
      targetCurrencyCode: rate.targetCurrencyCode,
      rate: rate.rate,
      validFrom: rate.validFrom?.split('T')[0] || '',
      validTo: rate.validTo?.split('T')[0] || ''
    });
    setShowForm(true);
  };

  const handleDelete = (id: number) => {
    if (confirm('Are you sure you want to delete this exchange rate?')) {
      deleteMutation.mutate(id);
    }
  };

  if (isLoading) return <div className="p-6">Loading...</div>;

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Exchange Rates</h1>
        <button
          onClick={() => setShowForm(true)}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
        >
          <Plus className="w-4 h-4" /> Add Rate
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Exchange Rate' : 'Add Exchange Rate'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Base Currency *</label>
              <select
                value={formData.baseCurrencyCode}
                onChange={(e) => setFormData({ ...formData, baseCurrencyCode: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              >
                <option value="">Select currency</option>
                {currencies.map((c: any) => (
                  <option key={c.currencyCode} value={c.currencyCode}>
                    {c.currencyCode} - {c.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Target Currency *</label>
              <select
                value={formData.targetCurrencyCode}
                onChange={(e) => setFormData({ ...formData, targetCurrencyCode: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              >
                <option value="">Select currency</option>
                {currencies.filter((c: any) => c.currencyCode !== formData.baseCurrencyCode).map((c: any) => (
                  <option key={c.currencyCode} value={c.currencyCode}>
                    {c.currencyCode} - {c.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Rate *</label>
              <input
                type="number"
                step="0.0001"
                value={formData.rate}
                onChange={(e) => setFormData({ ...formData, rate: parseFloat(e.target.value) })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
              <p className="text-xs text-gray-500 mt-1">
                1 {formData.baseCurrencyCode || 'BASE'} = {formData.rate} {formData.targetCurrencyCode || 'TARGET'}
              </p>
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Valid From *</label>
              <input
                type="date"
                value={formData.validFrom}
                onChange={(e) => setFormData({ ...formData, validFrom: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Valid To (optional)</label>
              <input
                type="date"
                value={formData.validTo}
                onChange={(e) => setFormData({ ...formData, validTo: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
              />
            </div>
            <div className="md:col-span-2 flex gap-4">
              <button type="submit" className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700">
                {editingId ? 'Update' : 'Create'}
              </button>
              <button type="button" onClick={resetForm} className="bg-gray-300 px-6 py-2 rounded-lg hover:bg-gray-400">
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Base</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Target</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Rate</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Valid From</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Valid To</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {rates.map((rate: any) => (
              <tr key={rate.id}>
                <td className="px-6 py-4 font-medium">{rate.baseCurrencyCode}</td>
                <td className="px-6 py-4">{rate.targetCurrencyCode}</td>
                <td className="px-6 py-4">{rate.rate.toFixed(4)}</td>
                <td className="px-6 py-4">{new Date(rate.validFrom).toLocaleDateString()}</td>
                <td className="px-6 py-4">{rate.validTo ? new Date(rate.validTo).toLocaleDateString() : '-'}</td>
                <td className="px-6 py-4">
                  <div className="flex gap-2">
                    <button onClick={() => handleEdit(rate)} className="text-blue-600 hover:text-blue-800">
                      <Edit className="w-4 h-4" />
                    </button>
                    <button onClick={() => handleDelete(rate.id)} className="text-red-600 hover:text-red-800">
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {rates.length === 0 && (
              <tr>
                <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                  No exchange rates configured
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
