import { useState, useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { settingsApi, branchesApi } from '../lib/api'
import { Plus, Trash2, FileText, Printer } from 'lucide-react'

export default function Settings() {
  const queryClient = useQueryClient()
  const { t, i18n } = useTranslation()
  const [activeTab, setActiveTab] = useState<'general' | 'receipts'>('general')
  const [showTemplateForm, setShowTemplateForm] = useState(false)
  const [templateFormData, setTemplateFormData] = useState({ name: '', templateType: 'CustomerReceipt', headerText: '', footerText: '', language: 'en', showLogo: true, showBarcode: false })

  const { data: settings } = useQuery({ queryKey: ['settings'], queryFn: () => settingsApi.getAll() })

  // Sync language setting with i18n when settings load
  useEffect(() => {
    const langSetting = settings?.data?.find((s: any) => s.settingKey === 'DefaultLanguage')?.settingValue
    if (langSetting && langSetting !== i18n.language) {
      i18n.changeLanguage(langSetting)
      document.documentElement.dir = langSetting === 'ar' ? 'rtl' : 'ltr'
      document.documentElement.lang = langSetting
    }
  }, [settings, i18n])
  const { data: templates } = useQuery({ queryKey: ['receipt-templates'], queryFn: () => settingsApi.getReceiptTemplates() })
  useQuery({ queryKey: ['branches'], queryFn: () => branchesApi.getAll() }) // Reserved for branch-specific settings

  const updateSettingMutation = useMutation({ mutationFn: settingsApi.update, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['settings'] }) })
  const createTemplateMutation = useMutation({ mutationFn: settingsApi.createReceiptTemplate, onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['receipt-templates'] }); setShowTemplateForm(false) } })
  const deleteTemplateMutation = useMutation({ mutationFn: settingsApi.deleteReceiptTemplate, onSuccess: () => queryClient.invalidateQueries({ queryKey: ['receipt-templates'] }) })

  const commonSettings = [
    { key: 'DefaultLanguage', labelKey: 'settings.defaultLanguage', type: 'select', options: ['en', 'ar'] },
    { key: 'AllowOfflineMode', labelKey: 'settings.allowOfflineMode', type: 'boolean' },
    { key: 'MaxOfflineHours', labelKey: 'settings.maxOfflineHours', type: 'number' },
    { key: 'AutoCloseShiftAfterHours', labelKey: 'settings.autoCloseShift', type: 'number' },
    { key: 'RequirePINForVoid', labelKey: 'settings.requirePinVoid', type: 'boolean' },
    { key: 'AllowNegativeStock', labelKey: 'settings.allowNegativeStock', type: 'boolean' },
  ]

  const getSetting = (key: string) => settings?.data?.find((s: any) => s.settingKey === key)?.settingValue || ''

  const handleSettingChange = (key: string, value: string, type: string = 'String') => {
    updateSettingMutation.mutate({ settingKey: key, settingValue: value, settingType: type })
    // If language is changed, update i18n immediately
    if (key === 'DefaultLanguage') {
      i18n.changeLanguage(value)
      document.documentElement.dir = value === 'ar' ? 'rtl' : 'ltr'
      document.documentElement.lang = value
    }
  }

  const handleTemplateSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    createTemplateMutation.mutate(templateFormData)
  }

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">{t('common.settings')}</h1>

      {/* Tabs */}
      <div className="flex gap-4 mb-6 border-b border-gray-700">
        <button onClick={() => setActiveTab('general')} className={`pb-2 px-4 ${activeTab === 'general' ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'}`}>
          {t('settings.general')}
        </button>
        <button onClick={() => setActiveTab('receipts')} className={`pb-2 px-4 ${activeTab === 'receipts' ? 'border-b-2 border-blue-600 text-blue-600' : 'text-gray-500'}`}>
          {t('settings.receiptTemplates')}
        </button>
      </div>

      {activeTab === 'general' && (
        <div className="card">
          <h2 className="text-lg font-semibold mb-4">{t('settings.general')}</h2>
          <div className="space-y-4">
            {commonSettings.map((setting) => (
              <div key={setting.key} className="flex items-center justify-between py-2 border-b border-gray-700">
                <label className="font-medium">{t(setting.labelKey)}</label>
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
            <h2 className="text-lg font-semibold flex items-center gap-2"><Printer size={20} /> {t('settings.receiptTemplates')}</h2>
            <button onClick={() => setShowTemplateForm(true)} className="btn-primary flex items-center gap-2">
              <Plus size={20} /> {t('settings.addTemplate')}
            </button>
          </div>

          {showTemplateForm && (
            <form onSubmit={handleTemplateSubmit} className="border rounded p-4 mb-4 bg-gray-800 grid grid-cols-1 md:grid-cols-2 gap-4">
              <input type="text" placeholder={`${t('settings.templateName')} *`} value={templateFormData.name} onChange={(e) => setTemplateFormData({ ...templateFormData, name: e.target.value })} className="input" required />
              <select value={templateFormData.templateType} onChange={(e) => setTemplateFormData({ ...templateFormData, templateType: e.target.value })} className="input">
                <option value="CustomerReceipt">{t('receiptTemplates.customerReceipt')}</option>
                <option value="KitchenTicket">{t('receiptTemplates.kitchenTicket')}</option>
                <option value="DailyReport">{t('settings.dailyReport')}</option>
              </select>
              <textarea placeholder={t('settings.headerText')} value={templateFormData.headerText} onChange={(e) => setTemplateFormData({ ...templateFormData, headerText: e.target.value })} className="input" rows={2} />
              <textarea placeholder={t('settings.footerText')} value={templateFormData.footerText} onChange={(e) => setTemplateFormData({ ...templateFormData, footerText: e.target.value })} className="input" rows={2} />
              <select value={templateFormData.language} onChange={(e) => setTemplateFormData({ ...templateFormData, language: e.target.value })} className="input">
                <option value="en">{t('receiptTemplates.english')}</option>
                <option value="ar">{t('receiptTemplates.arabic')}</option>
                <option value="both">{t('receiptTemplates.bilingual')}</option>
              </select>
              <div className="flex items-center gap-4">
                <label className="flex items-center gap-2">
                  <input type="checkbox" checked={templateFormData.showLogo} onChange={(e) => setTemplateFormData({ ...templateFormData, showLogo: e.target.checked })} /> {t('settings.showLogo')}
                </label>
                <label className="flex items-center gap-2">
                  <input type="checkbox" checked={templateFormData.showBarcode} onChange={(e) => setTemplateFormData({ ...templateFormData, showBarcode: e.target.checked })} /> {t('settings.showBarcode')}
                </label>
              </div>
              <div className="md:col-span-2 flex gap-2">
                <button type="submit" className="btn-primary">{t('settings.saveTemplate')}</button>
                <button type="button" onClick={() => setShowTemplateForm(false)} className="btn-secondary">{t('common.cancel')}</button>
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
                <p className="text-sm text-gray-600">{t('common.type')}: {template.templateType}</p>
                <p className="text-sm text-gray-600">{t('receiptTemplates.language')}: {template.language.toUpperCase()}</p>
                <div className="flex gap-2 mt-2">
                  {template.showLogo && <span className="text-xs bg-gray-100 px-2 py-1 rounded">{t('settings.showLogo')}</span>}
                  {template.showBarcode && <span className="text-xs bg-gray-100 px-2 py-1 rounded">{t('settings.showBarcode')}</span>}
                  {template.isDefault && <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">{t('receiptTemplates.defaultLabel')}</span>}
                </div>
              </div>
            ))}
          </div>
          {templates?.data?.length === 0 && <p className="text-center text-gray-500 py-8">{t('receiptTemplates.noTemplates')}</p>}
        </div>
      )}
    </div>
  )
}
