import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { currenciesApi } from '../lib/api';
import { Plus, Edit, ToggleLeft, ToggleRight, Star } from 'lucide-react';

export default function Currencies() {
  const queryClient = useQueryClient();
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    currencyCode: '',
    name: '',
    symbol: '',
    decimalPlaces: 2,
    isDefault: false
  });

  const { data: currencies = [], isLoading } = useQuery({
    queryKey: ['currencies'],
    queryFn: currenciesApi.getAll
  });

  const createMutation = useMutation({
    mutationFn: currenciesApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['currencies'] });
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: any }) => currenciesApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['currencies'] });
      resetForm();
    }
  });

  const toggleMutation = useMutation({
    mutationFn: currenciesApi.toggle,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['currencies'] })
  });

  const setDefaultMutation = useMutation({
    mutationFn: currenciesApi.setDefault,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['currencies'] })
  });

  const resetForm = () => {
    setShowForm(false);
    setEditingId(null);
    setFormData({ currencyCode: '', name: '', symbol: '', decimalPlaces: 2, isDefault: false });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (editingId) {
      updateMutation.mutate({ id: editingId, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleEdit = (currency: any) => {
    setEditingId(currency.currencyCode);
    setFormData({
      currencyCode: currency.currencyCode,
      name: currency.name,
      symbol: currency.symbol,
      decimalPlaces: currency.decimalPlaces,
      isDefault: currency.isDefault
    });
    setShowForm(true);
  };

  if (isLoading) return <div className="p-6">Loading...</div>;

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Currencies</h1>
        <button
          onClick={() => setShowForm(true)}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
        >
          <Plus className="w-4 h-4" /> Add Currency
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Currency' : 'Add Currency'}</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Currency Code *</label>
              <input
                type="text"
                value={formData.currencyCode}
                onChange={(e) => setFormData({ ...formData, currencyCode: e.target.value.toUpperCase() })}
                className="w-full border rounded-lg px-3 py-2"
                maxLength={3}
                required
                disabled={!!editingId}
                placeholder="USD"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Name *</label>
              <input
                type="text"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
                placeholder="US Dollar"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Symbol *</label>
              <input
                type="text"
                value={formData.symbol}
                onChange={(e) => setFormData({ ...formData, symbol: e.target.value })}
                className="w-full border rounded-lg px-3 py-2"
                required
                placeholder="$"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Decimal Places</label>
              <input
                type="number"
                value={formData.decimalPlaces}
                onChange={(e) => setFormData({ ...formData, decimalPlaces: parseInt(e.target.value) })}
                className="w-full border rounded-lg px-3 py-2"
                min={0}
                max={4}
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
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Code</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Name</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Symbol</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Decimals</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Default</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {currencies.map((currency: any) => (
              <tr key={currency.currencyCode}>
                <td className="px-6 py-4 font-medium">{currency.currencyCode}</td>
                <td className="px-6 py-4">{currency.name}</td>
                <td className="px-6 py-4">{currency.symbol}</td>
                <td className="px-6 py-4">{currency.decimalPlaces}</td>
                <td className="px-6 py-4">
                  {currency.isDefault ? (
                    <Star className="w-5 h-5 text-yellow-500 fill-yellow-500" />
                  ) : (
                    <button
                      onClick={() => setDefaultMutation.mutate(currency.currencyCode)}
                      className="text-gray-400 hover:text-yellow-500"
                      title="Set as default"
                    >
                      <Star className="w-5 h-5" />
                    </button>
                  )}
                </td>
                <td className="px-6 py-4">
                  <span className={`px-2 py-1 rounded-full text-xs ${currency.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {currency.isActive ? 'Active' : 'Inactive'}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <div className="flex gap-2">
                    <button onClick={() => handleEdit(currency)} className="text-blue-600 hover:text-blue-800">
                      <Edit className="w-4 h-4" />
                    </button>
                    <button onClick={() => toggleMutation.mutate(currency.currencyCode)} className="text-gray-600 hover:text-gray-800">
                      {currency.isActive ? <ToggleRight className="w-5 h-5 text-green-600" /> : <ToggleLeft className="w-5 h-5" />}
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
