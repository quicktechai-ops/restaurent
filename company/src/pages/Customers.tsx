import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { customersApi } from '../lib/api'
import api from '../lib/api'
import { Plus, Edit, Trash2, User, MapPin, X, Eye } from 'lucide-react'
import { Link } from 'react-router-dom'

export default function Customers() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [search, setSearch] = useState('')
  const [formData, setFormData] = useState({ name: '', phone: '', email: '', customerCode: '', notes: '' })
  
  // Address modal state
  const [showAddressModal, setShowAddressModal] = useState(false)
  const [selectedCustomerId, setSelectedCustomerId] = useState<number | null>(null)
  const [addressForm, setAddressForm] = useState({ label: '', addressLine1: '', addressLine2: '', city: '', area: '', notes: '', deliveryZoneId: '' })

  const { data: customers, isLoading } = useQuery({ 
    queryKey: ['customers', search], 
    queryFn: () => customersApi.getAll({ search: search || undefined }) 
  })

  // Delivery zones for address dropdown
  const { data: deliveryZones } = useQuery({ queryKey: ['delivery-zones'], queryFn: () => api.get('/api/company/delivery-zones').then(r => r.data) })
  
  // Customer addresses query
  const { data: customerAddresses } = useQuery({ 
    queryKey: ['customer-addresses', selectedCustomerId], 
    queryFn: () => api.get(`/api/company/customers/${selectedCustomerId}/addresses`).then(r => r.data),
    enabled: !!selectedCustomerId
  })

  const createMutation = useMutation({ mutationFn: customersApi.create, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['customers'] }); resetForm() } })
  const updateMutation = useMutation({ mutationFn: ({ id, data }: { id: number; data: any }) => customersApi.update(id, data), onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['customers'] }); resetForm() } })
  const deleteMutation = useMutation({ mutationFn: customersApi.delete, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['customers'] }) })
  
  // Address mutations
  const createAddressMutation = useMutation({ 
    mutationFn: (data: any) => api.post(`/api/company/customers/${selectedCustomerId}/addresses`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['customer-addresses', selectedCustomerId] }); queryClient.invalidateQueries({ queryKey: ['customers'] }); resetAddressForm() }
  })
  const deleteAddressMutation = useMutation({ 
    mutationFn: (addressId: number) => api.delete(`/api/company/customers/${selectedCustomerId}/addresses/${addressId}`),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['customer-addresses', selectedCustomerId] }); queryClient.invalidateQueries({ queryKey: ['customers'] }) }
  })

  const resetForm = () => { setShowForm(false); setEditingId(null); setFormData({ name: '', phone: '', email: '', customerCode: '', notes: '' }) }
  const resetAddressForm = () => { setAddressForm({ label: '', addressLine1: '', addressLine2: '', city: '', area: '', notes: '', deliveryZoneId: '' }) }
  
  const openAddressModal = (customerId: number) => { setSelectedCustomerId(customerId); setShowAddressModal(true) }
  const closeAddressModal = () => { setShowAddressModal(false); setSelectedCustomerId(null); resetAddressForm() }
  
  const handleAddressSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createAddressMutation.mutate({ ...addressForm, deliveryZoneId: addressForm.deliveryZoneId ? parseInt(addressForm.deliveryZoneId) : null })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  const handleEdit = (customer: any) => {
    setEditingId(customer.id)
    setFormData({ name: customer.name, phone: customer.phone || '', email: customer.email || '', customerCode: customer.customerCode || '', notes: '' })
    setShowForm(true)
  }

  if (isLoading) return <div className="p-6">Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Customers</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2">
          <Plus size={20} /> Add Customer
        </button>
      </div>

      <div className="mb-4">
        <input type="text" placeholder="Search customers..." value={search} onChange={(e) => setSearch(e.target.value)}
          className="input-field w-full max-w-md" />
      </div>

      {showForm && (
        <div className="card mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Add'} Customer</h2>
          <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <input type="text" placeholder="Name *" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} className="input-field" required />
            <input type="text" placeholder="Customer Code" value={formData.customerCode} onChange={(e) => setFormData({ ...formData, customerCode: e.target.value })} className="input-field" />
            <input type="tel" placeholder="Phone" value={formData.phone} onChange={(e) => setFormData({ ...formData, phone: e.target.value })} className="input-field" />
            <input type="email" placeholder="Email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} className="input-field" />
            <textarea placeholder="Notes" value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} className="input-field md:col-span-2" rows={2} />
            <div className="md:col-span-2 flex gap-2">
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
              <th className="text-left p-3">Code</th>
              <th className="text-left p-3">Phone</th>
              <th className="text-left p-3">Email</th>
              <th className="text-left p-3">Addresses</th>
              <th className="text-left p-3">Status</th>
              <th className="text-left p-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {customers?.data?.map((customer: any) => (
              <tr key={customer.id} className="border-b hover:bg-gray-50">
                <td className="p-3 flex items-center gap-2"><User size={16} className="text-gray-400" /> {customer.name}</td>
                <td className="p-3">{customer.customerCode || '-'}</td>
                <td className="p-3">{customer.phone || '-'}</td>
                <td className="p-3">{customer.email || '-'}</td>
                <td className="p-3">
                  <button onClick={() => openAddressModal(customer.id)} className="text-primary-600 hover:text-primary-800 flex items-center gap-1">
                    <MapPin size={14} /> {customer.addressesCount || 0}
                  </button>
                </td>
                <td className="p-3">
                  <span className={`px-2 py-1 rounded text-xs ${customer.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                    {customer.isActive ? 'Active' : 'Inactive'}
                  </span>
                </td>
                <td className="p-3">
                  <div className="flex gap-2">
                    <Link to={`/customers/${customer.id}`} className="text-green-600 hover:text-green-800"><Eye size={16} /></Link>
                    <button onClick={() => handleEdit(customer)} className="text-blue-600 hover:text-blue-800"><Edit size={16} /></button>
                    <button onClick={() => deleteMutation.mutate(customer.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {customers?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No customers found</p>}
      </div>

      {/* Address Modal */}
      {showAddressModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-2xl max-h-[90vh] overflow-hidden">
            <div className="flex justify-between items-center p-4 border-b">
              <h2 className="text-lg font-semibold">Customer Addresses</h2>
              <button onClick={closeAddressModal} className="text-gray-500 hover:text-gray-700"><X size={20} /></button>
            </div>
            
            <div className="p-4 max-h-[60vh] overflow-y-auto">
              {/* Add Address Form */}
              <form onSubmit={handleAddressSubmit} className="bg-gray-50 rounded-lg p-4 mb-4">
                <h3 className="font-medium mb-3">Add New Address</h3>
                <div className="grid grid-cols-2 gap-3">
                  <input type="text" placeholder="Label (Home, Work...)" value={addressForm.label} onChange={(e) => setAddressForm({ ...addressForm, label: e.target.value })} className="input-field" required />
                  <select value={addressForm.deliveryZoneId} onChange={(e) => setAddressForm({ ...addressForm, deliveryZoneId: e.target.value })} className="input-field">
                    <option value="">Select Delivery Zone</option>
                    {deliveryZones?.map((zone: any) => (
                      <option key={zone.id} value={zone.id}>{zone.zoneName}</option>
                    ))}
                  </select>
                  <input type="text" placeholder="Address Line 1 *" value={addressForm.addressLine1} onChange={(e) => setAddressForm({ ...addressForm, addressLine1: e.target.value })} className="input-field col-span-2" required />
                  <input type="text" placeholder="Address Line 2" value={addressForm.addressLine2} onChange={(e) => setAddressForm({ ...addressForm, addressLine2: e.target.value })} className="input-field col-span-2" />
                  <input type="text" placeholder="City" value={addressForm.city} onChange={(e) => setAddressForm({ ...addressForm, city: e.target.value })} className="input-field" />
                  <input type="text" placeholder="Area/Neighborhood" value={addressForm.area} onChange={(e) => setAddressForm({ ...addressForm, area: e.target.value })} className="input-field" />
                  <textarea placeholder="Delivery Notes" value={addressForm.notes} onChange={(e) => setAddressForm({ ...addressForm, notes: e.target.value })} className="input-field col-span-2" rows={2} />
                </div>
                <button type="submit" className="btn-primary mt-3">Add Address</button>
              </form>

              {/* Address List */}
              <div className="space-y-3">
                {customerAddresses?.map((addr: any) => (
                  <div key={addr.id} className="border rounded-lg p-3 flex justify-between items-start">
                    <div>
                      <span className="font-medium text-primary-600">{addr.label}</span>
                      <p className="text-sm text-gray-600">{addr.addressLine1}</p>
                      {addr.addressLine2 && <p className="text-sm text-gray-600">{addr.addressLine2}</p>}
                      <p className="text-sm text-gray-500">{[addr.area, addr.city].filter(Boolean).join(', ')}</p>
                      {addr.deliveryZoneName && <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded mt-1 inline-block">{addr.deliveryZoneName}</span>}
                    </div>
                    <button onClick={() => deleteAddressMutation.mutate(addr.id)} className="text-red-600 hover:text-red-800"><Trash2 size={16} /></button>
                  </div>
                ))}
                {customerAddresses?.length === 0 && <p className="text-center text-gray-500 py-4">No addresses yet</p>}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
