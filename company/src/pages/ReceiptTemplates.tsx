import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { Plus, Edit, Trash2, FileText, Eye } from 'lucide-react'

export default function ReceiptTemplates() {
  const { t } = useTranslation()
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
    showOrderNumber: true,
    showDate: true,
    showOrderType: true,
    showTable: true,
    showCustomer: true,
    showPaymentMethod: true,
    showItemCode: false,
    showModifiers: true,
    showDiscountDetails: true,
    showPaymentDetails: true,
    showTips: true,
    footerText: 'Thank you for your visit!',
    footerText2: 'Please come again',
    footerTextAr: '',
    footerTextAr2: '',
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
      showOrderNumber: true, showDate: true, showOrderType: true, showTable: true,
      showCustomer: true, showPaymentMethod: true, showItemCode: false, showModifiers: true,
      showDiscountDetails: true, showPaymentDetails: true, showTips: true,
      footerText: 'Thank you for your visit!', footerText2: 'Please come again',
      footerTextAr: '', footerTextAr2: '', isDefault: false, isActive: true
    })
  }

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingId) updateMutation.mutate({ id: editingId, data: formData })
    else createMutation.mutate(formData)
  }

  const handleEdit = (template: any) => {
    setEditingId(template.id)
    setFormData({
      name: template.name || '',
      type: template.type || 'customer',
      paperSize: template.paperSize || '80mm',
      language: template.language || 'en',
      showLogo: template.showLogo ?? true,
      showAddress: template.showAddress ?? true,
      showPhone: template.showPhone ?? true,
      showTaxNumber: template.showTaxNumber ?? true,
      showOrderNumber: template.showOrderNumber ?? true,
      showDate: template.showDate ?? true,
      showOrderType: template.showOrderType ?? true,
      showTable: template.showTable ?? true,
      showCustomer: template.showCustomer ?? true,
      showPaymentMethod: template.showPaymentMethod ?? true,
      showItemCode: template.showItemCode ?? false,
      showModifiers: template.showModifiers ?? true,
      showDiscountDetails: template.showDiscountDetails ?? true,
      showPaymentDetails: template.showPaymentDetails ?? true,
      showTips: template.showTips ?? true,
      footerText: template.footerText || 'Thank you for your visit!',
      footerText2: template.footerText2 || 'Please come again',
      footerTextAr: template.footerTextAr || '',
      footerTextAr2: template.footerTextAr2 || '',
      isDefault: template.isDefault ?? false,
      isActive: template.isActive ?? true
    })
    setShowForm(true)
  }

  if (isLoading) return <div>{t('common.loading')}</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2"><FileText size={28} /> {t('receiptTemplates.title')}</h1>
        <button onClick={() => setShowForm(true)} className="btn-primary flex items-center gap-2"><Plus size={20} /> {t('receiptTemplates.addTemplate')}</button>
      </div>

      {showForm && (
        <div className="card mb-6 p-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? t('receiptTemplates.editTemplate') : t('receiptTemplates.addTemplate')}</h2>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.templateName')} *</label>
                <input type="text" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} className="input" required />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('common.type')} *</label>
                <select value={formData.type} onChange={(e) => setFormData({ ...formData, type: e.target.value })} className="input" required>
                  <option value="customer">{t('receiptTemplates.customerReceipt')}</option>
                  <option value="kitchen">{t('receiptTemplates.kitchenTicket')}</option>
                  <option value="label">{t('receiptTemplates.foodLabel')}</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.paperSize')}</label>
                <select value={formData.paperSize} onChange={(e) => setFormData({ ...formData, paperSize: e.target.value })} className="input">
                  <option value="58mm">58mm</option>
                  <option value="80mm">80mm</option>
                  <option value="A4">A4</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.language')}</label>
                <select value={formData.language} onChange={(e) => setFormData({ ...formData, language: e.target.value })} className="input">
                  <option value="en">{t('receiptTemplates.english')}</option>
                  <option value="ar">{t('receiptTemplates.arabic')}</option>
                  <option value="both">{t('receiptTemplates.bilingual')}</option>
                </select>
              </div>
            </div>

            <h3 className="font-medium mb-3">{t('receiptTemplates.headerOptions')}</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-4">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showLogo} onChange={(e) => setFormData({ ...formData, showLogo: e.target.checked })} /> {t('receiptTemplates.showLogo')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showAddress} onChange={(e) => setFormData({ ...formData, showAddress: e.target.checked })} /> {t('receiptTemplates.showAddress')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showPhone} onChange={(e) => setFormData({ ...formData, showPhone: e.target.checked })} /> {t('receiptTemplates.showPhone')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showTaxNumber} onChange={(e) => setFormData({ ...formData, showTaxNumber: e.target.checked })} /> {t('receiptTemplates.showTaxNumber')}</label>
            </div>

            <h3 className="font-medium mb-3">{t('receiptTemplates.orderInfoOptions')}</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-4">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showOrderNumber} onChange={(e) => setFormData({ ...formData, showOrderNumber: e.target.checked })} /> {t('receiptTemplates.showOrderNumber')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showDate} onChange={(e) => setFormData({ ...formData, showDate: e.target.checked })} /> {t('receiptTemplates.showDate')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showOrderType} onChange={(e) => setFormData({ ...formData, showOrderType: e.target.checked })} /> {t('receiptTemplates.showOrderType')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showTable} onChange={(e) => setFormData({ ...formData, showTable: e.target.checked })} /> {t('receiptTemplates.showTable')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showCustomer} onChange={(e) => setFormData({ ...formData, showCustomer: e.target.checked })} /> {t('receiptTemplates.showCustomer')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showPaymentMethod} onChange={(e) => setFormData({ ...formData, showPaymentMethod: e.target.checked })} /> {t('receiptTemplates.showPaymentMethod')}</label>
            </div>

            <h3 className="font-medium mb-3">{t('receiptTemplates.itemOptions')}</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mb-6">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showItemCode} onChange={(e) => setFormData({ ...formData, showItemCode: e.target.checked })} /> {t('receiptTemplates.showItemCodes')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showModifiers} onChange={(e) => setFormData({ ...formData, showModifiers: e.target.checked })} /> {t('receiptTemplates.showModifiers')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showDiscountDetails} onChange={(e) => setFormData({ ...formData, showDiscountDetails: e.target.checked })} /> {t('receiptTemplates.showDiscounts')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showPaymentDetails} onChange={(e) => setFormData({ ...formData, showPaymentDetails: e.target.checked })} /> {t('receiptTemplates.showPaymentDetails')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.showTips} onChange={(e) => setFormData({ ...formData, showTips: e.target.checked })} /> {t('receiptTemplates.showTips')}</label>
            </div>

            <h3 className="font-medium mb-3">{t('receiptTemplates.footerSection')}</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.footerLine1En')}</label>
                <input type="text" value={formData.footerText} onChange={(e) => setFormData({ ...formData, footerText: e.target.value })} className="input" placeholder="Thank you for your visit!" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.footerLine1Ar')}</label>
                <input type="text" value={formData.footerTextAr} onChange={(e) => setFormData({ ...formData, footerTextAr: e.target.value })} className="input" dir="rtl" placeholder="شكراً لزيارتكم" />
              </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.footerLine2En')}</label>
                <input type="text" value={formData.footerText2} onChange={(e) => setFormData({ ...formData, footerText2: e.target.value })} className="input" placeholder="Please come again" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">{t('receiptTemplates.footerLine2Ar')}</label>
                <input type="text" value={formData.footerTextAr2} onChange={(e) => setFormData({ ...formData, footerTextAr2: e.target.value })} className="input" dir="rtl" placeholder="نتطلع لرؤيتكم مجدداً" />
              </div>
            </div>

            <div className="flex items-center gap-4 mb-6">
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.isDefault} onChange={(e) => setFormData({ ...formData, isDefault: e.target.checked })} /> {t('receiptTemplates.setAsDefault')}</label>
              <label className="flex items-center gap-2"><input type="checkbox" checked={formData.isActive} onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })} /> {t('common.active')}</label>
            </div>

            <div className="flex gap-2">
              <button type="submit" className="btn-primary">{editingId ? t('common.update') : t('common.create')}</button>
              <button type="button" onClick={resetForm} className="btn-secondary">{t('common.cancel')}</button>
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
                  {template.isDefault && <span className="text-xs bg-primary-100 text-primary-700 px-2 py-0.5 rounded">{t('receiptTemplates.defaultLabel')}</span>}
                </h3>
                <p className="text-sm text-gray-500 capitalize">{template.type} • {template.paperSize} • {template.language}</p>
              </div>
              <span className={`px-2 py-1 rounded text-xs ${template.isActive ? 'bg-green-100 text-green-700' : 'bg-gray-100 text-gray-700'}`}>
                {template.isActive ? t('common.active') : t('common.inactive')}
              </span>
            </div>
            <div className="text-sm text-gray-600 mb-3">
              <p>{t('receiptTemplates.options')}: {[
                template.showLogo && t('receiptTemplates.showLogo'),
                template.showAddress && t('receiptTemplates.showAddress'),
                template.showModifiers && t('receiptTemplates.showModifiers'),
                template.showDiscountDetails && t('receiptTemplates.showDiscounts')
              ].filter(Boolean).join(', ')}</p>
            </div>
            <div className="flex gap-2">
              <button onClick={() => setPreviewTemplate(template)} className="flex-1 py-1.5 bg-gray-100 text-gray-700 rounded text-sm flex items-center justify-center gap-1 hover:bg-gray-200">
                <Eye size={14} /> {t('receiptTemplates.preview')}
              </button>
              <button onClick={() => handleEdit(template)} className="flex-1 py-1.5 bg-blue-100 text-blue-700 rounded text-sm flex items-center justify-center gap-1 hover:bg-blue-200">
                <Edit size={14} /> {t('common.edit')}
              </button>
              <button onClick={() => deleteMutation.mutate(template.id)} className="py-1.5 px-3 bg-red-100 text-red-700 rounded text-sm hover:bg-red-200">
                <Trash2 size={14} />
              </button>
            </div>
          </div>
        ))}
      </div>
      {(!templates || templates.length === 0) && <p className="text-center text-gray-500 py-8">{t('receiptTemplates.noTemplates')}</p>}

      {/* Preview Modal */}
      {previewTemplate && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50" onClick={() => setPreviewTemplate(null)}>
          <div className="bg-gray-900 rounded-xl shadow-xl p-6 max-w-sm" onClick={e => e.stopPropagation()}>
            <h2 className="text-center font-bold mb-2">RECEIPT PREVIEW</h2>
            <div className="border-2 border-dashed border-gray-300 p-4 font-mono text-xs bg-white text-black">
              {previewTemplate.showLogo && <div className="text-center mb-2">[LOGO]</div>}
              <div className="text-center font-bold">Restaurant Name</div>
              {previewTemplate.showAddress && <div className="text-center text-gray-600">123 Main Street</div>}
              {previewTemplate.showPhone && <div className="text-center text-gray-600">Tel: +1 234 567 890</div>}
              {previewTemplate.showTaxNumber && <div className="text-center text-gray-600 mb-2">Tax#: 12345678</div>}
              <div className="border-t border-dashed border-black my-2"></div>
              {previewTemplate.showOrderNumber && <div className="flex justify-between"><span>Order #</span><span>#1234</span></div>}
              {previewTemplate.showDate && <div className="flex justify-between"><span>Date</span><span>Dec 7, 2025 7:30 PM</span></div>}
              {previewTemplate.showOrderType && <div className="flex justify-between"><span>Type</span><span>Dine In</span></div>}
              {previewTemplate.showTable && <div className="flex justify-between"><span>Table</span><span>Table 5</span></div>}
              {previewTemplate.showCustomer && <div className="flex justify-between"><span>Customer</span><span>John Doe</span></div>}
              {previewTemplate.showPaymentMethod && <div className="flex justify-between"><span>Payment</span><span>Cash</span></div>}
              <div className="border-t border-dashed border-black my-2"></div>
              <div className="flex justify-between"><span>2x Burger</span><span>$24.00</span></div>
              {previewTemplate.showModifiers && <div className="text-gray-500 ml-2">+ Extra cheese</div>}
              <div className="flex justify-between"><span>1x Cola</span><span>$3.00</span></div>
              <div className="border-t border-dashed border-black my-2"></div>
              <div className="flex justify-between"><span>Subtotal</span><span>$27.00</span></div>
              {previewTemplate.showDiscountDetails && <div className="flex justify-between text-red-600"><span>Discount 10%</span><span>-$2.70</span></div>}
              <div className="flex justify-between"><span>Tax</span><span>$2.43</span></div>
              <div className="flex justify-between font-bold border-t-2 border-b-2 border-double border-black py-1 my-1"><span>TOTAL</span><span>$26.73</span></div>
              {previewTemplate.showPaymentDetails && <><div className="text-gray-600">Paid: $30.00</div><div className="text-gray-600">Change: $3.27</div></>}
              <div className="border-t border-dashed border-black my-2"></div>
              <div className="text-center font-bold">{previewTemplate.footerText || 'Thank you!'}</div>
              {previewTemplate.footerText2 && <div className="text-center text-gray-600">{previewTemplate.footerText2}</div>}
            </div>
            <button onClick={() => setPreviewTemplate(null)} className="btn-secondary w-full mt-4">{t('receiptTemplates.closePreview')}</button>
          </div>
        </div>
      )}
    </div>
  )
}
