import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || '/api'

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('superadmin_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('superadmin_token')
      localStorage.removeItem('superadmin_user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api

// SuperAdmin Auth
export const authApi = {
  login: (username: string, password: string) =>
    api.post('/auth/superadmin/login', { username, password }),
  getMe: () => api.get('/auth/me'),
}

// Companies
export const companiesApi = {
  getAll: (params?: Record<string, unknown>) => api.get('/superadmin/companies', { params }),
  getById: (id: number) => api.get(`/superadmin/companies/${id}`),
  create: (data: unknown) => api.post('/superadmin/companies', data),
  update: (id: number, data: unknown) => api.put(`/superadmin/companies/${id}`, data),
  delete: (id: number) => api.delete(`/superadmin/companies/${id}`),
  resetPassword: (id: number, newPassword: string) => api.patch(`/superadmin/companies/${id}/reset-password`, { newPassword }),
  toggleStatus: (id: number) => api.put(`/superadmin/companies/${id}/toggle-status`),
}

// Plans
export const plansApi = {
  getAll: (params?: Record<string, unknown>) => api.get('/superadmin/plans', { params }),
  getById: (id: number) => api.get(`/superadmin/plans/${id}`),
  create: (data: unknown) => api.post('/superadmin/plans', data),
  update: (id: number, data: unknown) => api.put(`/superadmin/plans/${id}`, data),
  delete: (id: number) => api.delete(`/superadmin/plans/${id}`),
  toggle: (id: number) => api.patch(`/superadmin/plans/${id}/toggle`),
}

// Billing
export const billingApi = {
  getAll: (params?: Record<string, unknown>) => api.get('/superadmin/billing', { params }),
  getById: (id: number) => api.get(`/superadmin/billing/${id}`),
  create: (data: unknown) => api.post('/superadmin/billing', data),
  update: (id: number, data: unknown) => api.put(`/superadmin/billing/${id}`, data),
  delete: (id: number) => api.delete(`/superadmin/billing/${id}`),
}

// Dashboard Stats
export const statsApi = {
  getDashboard: () => api.get('/superadmin/dashboard'),
}
