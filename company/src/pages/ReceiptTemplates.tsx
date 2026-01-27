import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import api from '../lib/api'
import { Plus, Edit, Trash2, FileText, Eye } from 'lucide-react'

export default function ReceiptTemplates() {
  const queryClient = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [editingId, setEditingId] = useState<number | null>(null)
  const [previewTemplate, setPreviewTemplate] = useState<any>(null)
  const [formData, setFormData] = useState({
    name: '',
    type: 'customer',
    paperSize: '80mm',
    language: 'en',
    showLogo: true,
    showAddress: true,
    showPhone: true,
    showTaxNumber: true,
    showItemCode: false,
    showModifiers: true,
    showDiscountDetails: true,
    showPaymentDetails: true,
    showTips: true,
    footerText: 'Thank you for your visit!',
    footerTextAr: '',
    isDefault: false,
    isActive: true
  })

  const { data: templates = [], isLoading } = useQuery({ 
    queryKey: ['receipt-templates'], 
    queryFn: () => api.get('/api/company/receipt-templates').then(r => Array.isArray(r.data) ? r.data : []) 
  })

  const createMutation = useMutation({
    mutationFn: (data: any) => api.post('/api/company/receipt-templates', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['receipt-templates'] }); resetForm() }
  })
  
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => api.put(`/api/company/receipt-templates/${id}`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['receipt-templates'] }); resetForm() }
  })
  
  const deleteMutation = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/receipt-templates/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['receipt-templates'] })
  })

  const resetForm = () => {
    setShowForm(false)
    setEditingId(null)
    setFormData({
      name: '', type: 'customer', paperSize: '80mm', language: 'en',
      showLogo: true, showAddress: true, showPhone: true, showTaxNumber: true,
      showItemCode: false, showModifiers: true, showDiscountDetails: true,
      showPaymentDetails: true, showTips: true, footerText: 'Thank you for your visit!',
      footerTextAr: '', isDefault: false, isActive: true
    })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  const handleEdit = (template: any) => {
    setEditingId(template.id)
    setFormData({ ...template })
    setShowForm(true)
  }

  if (isLoading) return <div>Loading...</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><FileText size={28} /> Receipt Templates</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> Add Template</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit' : 'Create'} Receipt Template</h2>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
              <div>
                <label className="block text-sm font-medium mb-1">Template Name *</label>
                <input type="text" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} className="input" required />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Type *</label>
                <select value={formData.type} onChange={(e) => setFormData({ ...formData, type: e.target.value })} className="input" required>
                  <option value="customer">Customer Receipt</option>
                  <option value="kitchen">Kitchen Ticket</option>
                  <option value="label">Food Label</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Paper Size</label>
                <select value={formData.paperSize} onChange={(e) => setFormData({ ...formData, paperSize: e.target.value })} className="input">
                  <option value="58mm">58mm</option>
                  <option value="80mm">80mm</option>
                  <option value="A4">A4</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Language</label>
                <select value={formData.language} onChange={(e) => setFormData({ ...formData, language: e.target.value })} className="input">
                  <option value="en">English</option>
                  <option value="ar">Arabic</option>
                  <option value="both">Bilingual (EN + AR)</option>
                </select>
              </div>
            </div>

            <h3 className="font-medium mb-3">Display Options</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-6">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showLogo} onChange={(e) => setFormData({ ...formData, showLogo: e.target.checked })} /> Show Logo</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showAddress} onChange={(e) => setFormData({ ...formData, showAddress: e.target.checked })} /> Show Address</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showPhone} onChange={(e) => setFormData({ ...formData, showPhone: e.target.checked })} /> Show Phone</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showTaxNumber} onChange={(e) => setFormData({ ...formData, showTaxNumber: e.target.checked })} /> Show Tax Number</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showItemCode} onChange={(e) => setFormData({ ...formData, showItemCode: e.target.checked })} /> Show Item Codes</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showModifiers} onChange={(e) => setFormData({ ...formData, showModifiers: e.target.checked })} /> Show Modifiers</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showDiscountDetails} onChange={(e) => setFormData({ ...formData, showDiscountDetails: e.target.checked })} /> Show Discounts</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showPaymentDetails} onChange={(e) => setFormData({ ...formData, showPaymentDetails: e.target.checked })} /> Show Payment Details</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showTips} onChange={(e) => setFormData({ ...formData, showTips: e.target.checked })} /> Show Tips</label>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
              <div>
                <label className="block text-sm font-medium mb-1">Footer Text (English)</label>
                <textarea value={formData.footerText} onChange={(e) => setFormData({ ...formData, footerText: e.target.value })} className="input" rows={2} />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Footer Text (Arabic)</label>
                <textarea value={formData.footerTextAr} onChange={(e) => setFormData({ ...formData, footerTextAr: e.target.value })} className="input" rows={2} dir="rtl" />
              </div>
            </div>

            <div className="flex items-center gap-4 mb-6">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.isDefault} onChange={(e) => setFormData({ ...formData, isDefault: e.target.checked })} /> Set as Default</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.isActive} onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} /> Active</label>
            </div>

            <div className="flex gap-2">
              <button type="submit" className="btn-primary">{editingId ? 'Update' : 'Create'} Template</button>
              <button type="button" onClick={resetForm} className="btn-secondary">Cancel</button>
            </div>
          </form>
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {templates?.map((template: any) => (
          <div key={template.id} className="card p-4">
            <div className="flex justify-between items-start mb-3">
              <div>
                <h3 className="font-semibold flex items-center gap-2">
                  <FileText size={18} className="text-primary-600" />
                  {template.name}
                  {template.isDefault && <span className="text-xs bg-primary-100 text-primary-700 px-2 py-0.5 rounded">Default</span>}
                </h3>
                <p className="text-sm text-gray-500 capitalize">{template.type} • {template.paperSize} • {template.language}</p>
              </div>
              <span className={`px-2 py-1 rounded text-xs ${template.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'}`}>
                {template.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>
            <div className="text-sm text-gray-600 mb-3">
              <p>Options: {[
                template.showLogo && 'Logo',
                template.showAddress && 'Address',
                template.showModifiers && 'Modifiers',
                template.showDiscountDetails && 'Discounts'
              ].filter(Boolean).join(', ')}</p>
            </div>
            <div className="flex gap-2">
              <button onClick={() => setPreviewTemplate(template)} className="flex-1 py-1.5 bg-gray-100 text-gray-700 rounded text-sm flex items-center justify-center gap-1 hover:bg-gray-200">
                <Eye size={14} /> Preview
              </button>
              <button onClick={() => handleEdit(template)} className="flex-1 py-1.5 bg-blue-100 text-blue-700 rounded text-sm flex items-center justify-center gap-1 hover:bg-blue-200">
                <Edit size={14} /> Edit
              </button>
              <button onClick={() => deleteMutation.mutate(template.id)} className="py-1.5 px-3 bg-red-100 text-red-700 rounded text-sm hover:bg-red-200">
                <Trash2 size={14} />
              </button>
            </div>
          </div>
        ))}
      </div>
      {(!templates || templates.length === 0) && <p className="text-center text-gray-500 py-8">No receipt templates defined</p>}

      {/* Preview Modal */}
      {previewTemplate && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50" onClick={() => setPreviewTemplate(null)}>
          <div className="bg-gray-900 rounded-xl shadow-xl p-6 max-w-sm" onClick={e => e.stopPropagation()}>
            <h2 className="text-center font-bold mb-2">RECEIPT PREVIEW</h2>
            <div className="border-2 border-dashed border-gray-300 p-4 font-mono text-xs">
              {previewTemplate.showLogo && <div className="text-center mb-2">[LOGO]</div>}
              <div className="text-center font-bold">Restaurant Name</div>
              {previewTemplate.showAddress && <div className="text-center text-gray-600">123 Main Street</div>}
              {previewTemplate.showPhone && <div className="text-center text-gray-600">Tel: +1 234 567 890</div>}
              {previewTemplate.showTaxNumber && <div className="text-center text-gray-600 mb-2">Tax#: 12345678</div>}
              <div className="border-t border-dashed my-2"></div>
              <div>Order #1234 • Table 5</div>
              <div className="text-gray-600">Dec 7, 2025 7:30 PM</div>
              <div className="border-t border-dashed my-2"></div>
              <div className="flex justify-between"><span>2x Burger</span><span>$24.00</span></div>
              {previewTemplate.showModifiers && <div className="text-gray-500 ml-2">- Extra cheese</div>}
              <div className="flex justify-between"><span>1x Cola</span><span>$3.00</span></div>
              <div className="border-t border-dashed my-2"></div>
              <div className="flex justify-between"><span>Subtotal</span><span>$27.00</span></div>
              {previewTemplate.showDiscountDetails && <div className="flex justify-between text-green-600"><span>Discount 10%</span><span>-$2.70</span></div>}
              <div className="flex justify-between"><span>Tax</span><span>$2.43</span></div>
              <div className="flex justify-between font-bold"><span>TOTAL</span><span>$26.73</span></div>
              {previewTemplate.showPaymentDetails && <><div className="border-t border-dashed my-2"></div><div>Paid: Cash $30.00</div><div>Change: $3.27</div></>}
              <div className="border-t border-dashed my-2"></div>
              <div className="text-center text-gray-600">{previewTemplate.footerText}</div>
            </div>
            <button onClick={() => setPreviewTemplate(null)} className="btn-secondary w-full mt-4">Close Preview</button>
          </div>
        </div>
      )}
    </div>
  )
}
