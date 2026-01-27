import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi, branchesApi } from '../lib/api'
import { Plus, Trash2, FileText, Printer } from 'lucide-react'

export default function Settings() {
  const queryClient = useQueryClient()
  const [activeTab, setActiveTab] = useState<'general' | 'receipts'>('general')
  const [showTemplateForm, setShowTemplateForm] = useState(false)
  const [templateFormData, setTemplateFormData] = useState({ name: '', templateType: 'CustomerReceipt', headerText: '', footerText: '', language: 'en', showLogo: true, showBarcode: false })

  const { data: settings } = useQuery({ queryKey: ['settings'], queryFn: () => settingsApi.getAll() })
  const { data: templates } = useQuery({ queryKey: ['receipt-templates'], queryFn: () => settingsApi.getReceiptTemplates() })
  useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() }) // Reserved for branch-specific settings

  const updateSettingMutation = useMutation({ mutationFn: settingsApi.update, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['settings'] }) })
  const createTemplateMutation = useMutation({ mutationFn: settingsApi.createReceiptTemplate, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['receipt-templates'] }); setShowTemplateForm(false) } })
  const deleteTemplateMutation = useMutation({ mutationFn: settingsApi.deleteReceiptTemplate, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['receipt-templates'] }) })

  const commonSettings = [
    { key: 'DefaultLanguage', label: 'Default Language', type: 'select', options: ['en', 'ar'] },
    { key: 'AllowOfflineMode', label: 'Allow Offline Mode', type: 'boolean' },
    { key: 'MaxOfflineHours', label: 'Max Offline Hours', type: 'number' },
    { key: 'AutoCloseShiftAfterHours', label: 'Auto-Close Shift After Hours', type: 'number' },
    { key: 'RequirePINForVoid', label: 'Require PIN for Void', type: 'boolean' },
    { key: 'AllowNegativeStock', label: 'Allow Negative Stock', type: 'boolean' },
  ]

  const getSetting = (key: string) => settings?.data?.find((s: any) => s.settingKey === key)?.settingValue || ''

  const handleSettingChange = (key: string, value: string, type: string = 'String') => {
    updateSettingMutation.mutate({ settingKey: key, settingValue: value, settingType: type })
  }

  const handleTemplateSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createTemplateMutation.mutate(templateFormData)
  }

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Settings</h1>

      {/* Tabs */}
      <div className="flex gap-4 mb-6 border-b border-gray-700">
        <button onClick={() => setActiveTab('general')} className={`pb-2 px-4 ${activeTab === 'general' ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'}`}>
          General Settings
        </button>
        <button onClick={() => setActiveTab('receipts')} className={`pb-2 px-4 ${activeTab === 'receipts' ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'}`}>
          Receipt Templates
        </button>
      </div>

      {activeTab === 'general' && (
        <div className="card">
          <h2 className="text-lg font-semibold mb-4">General Settings</h2>
          <div className="space-y-4">
            {commonSettings.map((setting) => (
              <div key={setting.key} className="flex items-center justify-between py-2 border-b border-gray-700">
                <label className="font-medium">{setting.label}</label>
                {setting.type === 'boolean' ? (
                  <input type="checkbox" checked={getSetting(setting.key) === 'true'} onChange={(e) => handleSettingChange(setting.key, e.target.checked.toString(), 'Boolean')} className="w-5 h-5" />
                ) : setting.type === 'select' ? (
                  <select value={getSetting(setting.key)} onChange={(e) => handleSettingChange(setting.key, e.target.value)} className="input w-40">
                    {setting.options?.map((opt) => <option key={opt} value={opt}>{opt.toUpperCase()}</option>)}
                  </select>
                ) : (
                  <input type="number" value={getSetting(setting.key)} onChange={(e) => handleSettingChange(setting.key, e.target.value, 'Integer')} className="input w-40" />
                )}
              </div>
            ))}
          </div>
        </div>
      )}

      {activeTab === 'receipts' && (
        <div className="card">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold flex items-center gap-2"><Printer size={20} /> Receipt Templates</h2>
            <button onClick={() => setShowTemplateForm(true)} className="btn-primary flex items-center gap-2">
              <Plus size={20} /> Add Template
            </button>
          </div>

          {showTemplateForm && (
            <form onSubmit={handleTemplateSubmit} className="border rounded p-4 mb-4 bg-gray-800 grid grid-cols-1 md:grid-cols-2 gap-4">
              <input type="text" placeholder="Template Name *" value={templateFormData.name} onChange={(e) => setTemplateFormData({ ...templateFormData, name: e.target.value })} className="input" required />
              <select value={templateFormData.templateType} onChange={(e) => setTemplateFormData({ ...templateFormData, templateType: e.target.value })} className="input">
                <option value="CustomerReceipt">Customer Receipt</option>
                <option value="KitchenTicket">Kitchen Ticket</option>
                <option value="DailyReport">Daily Report</option>
              </select>
              <textarea placeholder="Header Text" value={templateFormData.headerText} onChange={(e) => setTemplateFormData({ ...templateFormData, headerText: e.target.value })} className="input" rows={2} />
              <textarea placeholder="Footer Text" value={templateFormData.footerText} onChange={(e) => setTemplateFormData({ ...templateFormData, footerText: e.target.value })} className="input" rows={2} />
              <select value={templateFormData.language} onChange={(e) => setTemplateFormData({ ...templateFormData, language: e.target.value })} className="input">
                <option value="en">English</option>
                <option value="ar">Arabic</option>
                <option value="both">Both</option>
              </select>
              <div className="flex items-center gap-4">
                <label className="flex items-center gap-2">
                  <input type="checkbox" checked={templateFormData.showLogo} onChange={(e) => setTemplateFormData({ ...templateFormData, showLogo: e.target.checked })} /> Show Logo
                </label>
                <label className="flex items-center gap-2">
                  <input type="checkbox" checked={templateFormData.showBarcode} onChange={(e) => setTemplateFormData({ ...templateFormData, showBarcode: e.target.checked })} /> Show Barcode
                </label>
              </div>
              <div className="md:col-span-2 flex gap-2">
                <button type="submit" className="btn-primary">Save Template</button>
                <button type="button" onClick={() => setShowTemplateForm(false)} className="btn-secondary">Cancel</button>
              </div>
            </form>
          )}

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {templates?.data?.map((template: any) => (
              <div key={template.id} className="border rounded-lg p-4">
                <div className="flex justify-between items-start mb-2">
                  <h3 className="font-semibold flex items-center gap-2"><FileText size={16} /> {template.name}</h3>
                  <button onClick={() => deleteTemplateMutation.mutate(template.id)} className="text-red-600"><Trash2 size={14} /></button>
                </div>
                <p className="text-sm text-gray-600">Type: {template.templateType}</p>
                <p className="text-sm text-gray-600">Language: {template.language.toUpperCase()}</p>
                <div className="flex gap-2 mt-2">
                  {template.showLogo && <span className="text-xs bg-gray-100 px-2 py-1 rounded">Logo</span>}
                  {template.showBarcode && <span className="text-xs bg-gray-100 px-2 py-1 rounded">Barcode</span>}
                  {template.isDefault && <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">Default</span>}
                </div>
              </div>
            ))}
          </div>
          {templates?.data?.length === 0 && <p className="text-center text-gray-500 py-8">No receipt templates configured</p>}
        </div>
      )}
    </div>
  )
}
