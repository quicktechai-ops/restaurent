import axios from 'axios'

const api = axios.create({
  baseURL: '',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('company_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle 401 errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('company_token')
      localStorage.removeItem('company_user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// API services
export const dashboardApi = {
  getStats: () => api.get('/api/company/dashboard'),
}

export const branchesApi = {
  getAll: () => api.get('/api/company/branches'),
  getById: (id: number) => api.get(`/api/company/branches/${id}`),
  create: (data: any) => api.post('/api/company/branches', data),
  update: (id: number, data: any) => api.put(`/api/company/branches/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/branches/${id}`),
  toggle: (id: number) => api.patch(`/api/company/branches/${id}/toggle`),
}

export const usersApi = {
  getAll: (branchId?: number) => api.get('/api/company/users', { params: { branchId } }),
  getById: (id: number) => api.get(`/api/company/users/${id}`),
  create: (data: any) => api.post('/api/company/users', data),
  update: (id: number, data: any) => api.put(`/api/company/users/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/users/${id}`),
  toggle: (id: number) => api.patch(`/api/company/users/${id}/toggle`),
  resetPassword: (id: number, password: string) => 
    api.patch(`/api/company/users/${id}/reset-password`, { newPassword: password }),
}

export const rolesApi = {
  getAll: () => api.get('/api/company/roles'),
  getPermissions: () => api.get('/api/company/roles/permissions'),
  getById: (id: number) => api.get(`/api/company/roles/${id}`),
  create: (data: any) => api.post('/api/company/roles', data),
  update: (id: number, data: any) => api.put(`/api/company/roles/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/roles/${id}`),
  toggle: (id: number) => api.patch(`/api/company/roles/${id}/toggle`),
}

export const categoriesApi = {
  getAll: (isActive?: boolean) => api.get('/api/company/categories', { params: { isActive } }),
  getById: (id: number) => api.get(`/api/company/categories/${id}`),
  create: (data: any) => api.post('/api/company/categories', data),
  update: (id: number, data: any) => api.put(`/api/company/categories/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/categories/${id}`),
  toggle: (id: number) => api.patch(`/api/company/categories/${id}/toggle`),
}

export const menuItemsApi = {
  getAll: (params?: { categoryId?: number; isActive?: boolean; search?: string }) => 
    api.get('/api/company/menu-items', { params }),
  getById: (id: number) => api.get(`/api/company/menu-items/${id}`),
  create: (data: any) => api.post('/api/company/menu-items', data),
  update: (id: number, data: any) => api.put(`/api/company/menu-items/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/menu-items/${id}`),
  toggle: (id: number) => api.patch(`/api/company/menu-items/${id}/toggle`),
  toggleAvailability: (id: number) => api.patch(`/api/company/menu-items/${id}/availability`),
}

export const tablesApi = {
  getAll: (branchId?: number) => api.get('/api/company/tables', { params: { branchId } }),
  getById: (id: number) => api.get(`/api/company/tables/${id}`),
  create: (data: any) => api.post('/api/company/tables', data),
  update: (id: number, data: any) => api.put(`/api/company/tables/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/tables/${id}`),
  updateStatus: (id: number, status: string) => 
    api.patch(`/api/company/tables/${id}/status?status=${status}`),
}

export const kitchenStationsApi = {
  getAll: (branchId?: number) => api.get('/api/company/kitchen-stations', { params: { branchId } }),
  getById: (id: number) => api.get(`/api/company/kitchen-stations/${id}`),
  create: (data: any) => api.post('/api/company/kitchen-stations', data),
  update: (id: number, data: any) => api.put(`/api/company/kitchen-stations/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/kitchen-stations/${id}`),
  toggle: (id: number) => api.patch(`/api/company/kitchen-stations/${id}/toggle`),
}

export const modifiersApi = {
  getAll: (isActive?: boolean) => api.get('/api/company/modifiers', { params: { isActive } }),
  getById: (id: number) => api.get(`/api/company/modifiers/${id}`),
  create: (data: any) => api.post('/api/company/modifiers', data),
  update: (id: number, data: any) => api.put(`/api/company/modifiers/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/modifiers/${id}`),
  toggle: (id: number) => api.patch(`/api/company/modifiers/${id}/toggle`),
}

export const paymentMethodsApi = {
  getAll: (isActive?: boolean) => api.get('/api/company/payment-methods', { params: { isActive } }),
  getById: (id: number) => api.get(`/api/company/payment-methods/${id}`),
  create: (data: any) => api.post('/api/company/payment-methods', data),
  update: (id: number, data: any) => api.put(`/api/company/payment-methods/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/payment-methods/${id}`),
  toggle: (id: number) => api.patch(`/api/company/payment-methods/${id}/toggle`),
}

export const currenciesApi = {
  getAll: () => api.get('/api/company/currencies').then(r => r.data),
  create: (data: any) => api.post('/api/company/currencies', data),
  update: (id: string, data: any) => api.put(`/api/company/currencies/${id}`, data),
  toggle: (id: string) => api.patch(`/api/company/currencies/${id}/toggle`),
  setDefault: (id: string) => api.patch(`/api/company/currencies/${id}/set-default`),
}

export const customersApi = {
  getAll: (params?: { search?: string; isActive?: boolean }) => 
    api.get('/api/company/customers', { params }),
  getById: (id: number) => api.get(`/api/company/customers/${id}`),
  create: (data: any) => api.post('/api/company/customers', data),
  update: (id: number, data: any) => api.put(`/api/company/customers/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/customers/${id}`),
  getAddresses: (customerId: number) => api.get(`/api/company/customers/${customerId}/addresses`),
  addAddress: (customerId: number, data: any) => api.post(`/api/company/customers/${customerId}/addresses`, data),
  deleteAddress: (addressId: number) => api.delete(`/api/company/customers/addresses/${addressId}`),
}

export const deliveryZonesApi = {
  getAll: (branchId?: number) => api.get('/api/company/delivery-zones', { params: { branchId } }),
  getById: (id: number) => api.get(`/api/company/delivery-zones/${id}`),
  create: (data: any) => api.post('/api/company/delivery-zones', data),
  update: (id: number, data: any) => api.put(`/api/company/delivery-zones/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/delivery-zones/${id}`),
  toggle: (id: number) => api.patch(`/api/company/delivery-zones/${id}/toggle`),
}

export const inventoryApi = {
  getAll: (params?: { search?: string; category?: string }) => 
    api.get('/api/company/inventory', { params }),
  getById: (id: number) => api.get(`/api/company/inventory/${id}`),
  create: (data: any) => api.post('/api/company/inventory', data),
  update: (id: number, data: any) => api.put(`/api/company/inventory/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/inventory/${id}`),
  toggle: (id: number) => api.patch(`/api/company/inventory/${id}/toggle`),
  getCategories: () => api.get('/api/company/inventory/categories'),
}

export const suppliersApi = {
  getAll: (params?: { search?: string; isActive?: boolean }) => 
    api.get('/api/company/suppliers', { params }),
  getById: (id: number) => api.get(`/api/company/suppliers/${id}`),
  create: (data: any) => api.post('/api/company/suppliers', data),
  update: (id: number, data: any) => api.put(`/api/company/suppliers/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/suppliers/${id}`),
  toggle: (id: number) => api.patch(`/api/company/suppliers/${id}/toggle`),
}

export const employeesApi = {
  getAll: (params?: { branchId?: number; position?: string }) => 
    api.get('/api/company/employees', { params }),
  getById: (id: number) => api.get(`/api/company/employees/${id}`),
  create: (data: any) => api.post('/api/company/employees', data),
  update: (id: number, data: any) => api.put(`/api/company/employees/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/employees/${id}`),
  toggle: (id: number) => api.patch(`/api/company/employees/${id}/toggle`),
  getPositions: () => api.get('/api/company/employees/positions'),
}

export const loyaltyApi = {
  getSettings: () => api.get('/api/company/loyalty/settings'),
  updateSettings: (data: any) => api.post('/api/company/loyalty/settings', data),
  getTiers: () => api.get('/api/company/loyalty/tiers'),
  createTier: (data: any) => api.post('/api/company/loyalty/tiers', data),
  updateTier: (id: number, data: any) => api.put(`/api/company/loyalty/tiers/${id}`, data),
  deleteTier: (id: number) => api.delete(`/api/company/loyalty/tiers/${id}`),
}

export const giftCardsApi = {
  getAll: (params?: { status?: string; branchId?: number }) => 
    api.get('/api/company/gift-cards', { params }),
  getById: (id: number) => api.get(`/api/company/gift-cards/${id}`),
  lookup: (number: string) => api.get(`/api/company/gift-cards/lookup/${number}`),
  create: (data: any) => api.post('/api/company/gift-cards', data),
  getTransactions: (id: number) => api.get(`/api/company/gift-cards/${id}/transactions`),
  block: (id: number) => api.patch(`/api/company/gift-cards/${id}/block`),
  activate: (id: number) => api.patch(`/api/company/gift-cards/${id}/activate`),
}

export const reservationsApi = {
  getAll: (params?: { branchId?: number; date?: string; status?: string }) => 
    api.get('/api/company/reservations', { params }),
  getById: (id: number) => api.get(`/api/company/reservations/${id}`),
  create: (data: any) => api.post('/api/company/reservations', data),
  update: (id: number, data: any) => api.put(`/api/company/reservations/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/reservations/${id}`),
  updateStatus: (id: number, status: string) => 
    api.patch(`/api/company/reservations/${id}/status`, { status }),
}

export const printersApi = {
  getAll: (params?: { branchId?: number; type?: string }) => 
    api.get('/api/company/printers', { params }),
  getById: (id: number) => api.get(`/api/company/printers/${id}`),
  create: (data: any) => api.post('/api/company/printers', data),
  update: (id: number, data: any) => api.put(`/api/company/printers/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/printers/${id}`),
  toggle: (id: number) => api.patch(`/api/company/printers/${id}/toggle`),
}

export const settingsApi = {
  getAll: (branchId?: number) => api.get('/api/company/settings', { params: { branchId } }),
  getByKey: (key: string, branchId?: number) => 
    api.get(`/api/company/settings/${key}`, { params: { branchId } }),
  update: (data: any) => api.post('/api/company/settings', data),
  bulkUpdate: (data: any[]) => api.post('/api/company/settings/bulk', data),
  delete: (id: number) => api.delete(`/api/company/settings/${id}`),
  getReceiptTemplates: (branchId?: number) => 
    api.get('/api/company/settings/receipt-templates', { params: { branchId } }),
  createReceiptTemplate: (data: any) => api.post('/api/company/settings/receipt-templates', data),
  updateReceiptTemplate: (id: number, data: any) => 
    api.put(`/api/company/settings/receipt-templates/${id}`, data),
  deleteReceiptTemplate: (id: number) => api.delete(`/api/company/settings/receipt-templates/${id}`),
}

export const auditLogsApi = {
  getAll: (params?: { branchId?: number; userId?: number; actionType?: string; fromDate?: string; toDate?: string; page?: number }) => 
    api.get('/api/company/audit-logs', { params }),
  getActionTypes: () => api.get('/api/company/audit-logs/action-types'),
}

export const exchangeRatesApi = {
  getAll: () => api.get('/api/company/exchange-rates').then(r => r.data),
  create: (data: any) => api.post('/api/company/exchange-rates', data),
  update: (id: number, data: any) => api.put(`/api/company/exchange-rates/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/exchange-rates/${id}`),
}

export const recipesApi = {
  getAll: () => api.get('/api/company/recipes').then(r => r.data),
  getById: (id: number) => api.get(`/api/company/recipes/${id}`).then(r => r.data),
  create: (data: any) => api.post('/api/company/recipes', data),
  update: (id: number, data: any) => api.put(`/api/company/recipes/${id}`, data),
  delete: (id: number) => api.delete(`/api/company/recipes/${id}`),
}

export default api
