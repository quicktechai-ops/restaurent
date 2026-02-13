import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import api from '../lib/api'
import { Plus, Edit, Trash2, X, FolderOpen, Scale, GripVertical, Tag, FileText, Hash, Type, ArrowRightLeft } from 'lucide-react'

type TabType = 'categories' | 'units' | 'conversions'

interface Category {
  id: number
  name: string
  description?: string
  parentCategoryId?: number | null
  parentCategoryName?: string | null
  sortOrder: number
  isActive: boolean
}

interface UnitOfMeasure {
  id: number
  code: string
  name: string
  symbol?: string
  sortOrder: number
  isActive: boolean
}

interface UnitConversion {
  id: number
  fromUnitCode: string
  toUnitCode: string
  conversionFactor: number
  isActive: boolean
}

export default function InventorySettings() {
  const { t } = useTranslation()
  const queryClient = useQueryClient()
  const [activeTab, setActiveTab] = useState<TabType>('categories')
  
  // Category state
  const [showCategoryModal, setShowCategoryModal] = useState(false)
  const [editingCategory, setEditingCategory] = useState<Category | null>(null)
  const [categoryForm, setCategoryForm] = useState({ name: '', description: '', parentCategoryId: null as number | null, sortOrder: 0 })
  
  // Unit state
  const [showUnitModal, setShowUnitModal] = useState(false)
  const [editingUnit, setEditingUnit] = useState<UnitOfMeasure | null>(null)
  const [unitForm, setUnitForm] = useState({ code: '', name: '', symbol: '', sortOrder: 0 })

  // Conversion state
  const [showConversionModal, setShowConversionModal] = useState(false)
  const [editingConversion, setEditingConversion] = useState<UnitConversion | null>(null)
  const [conversionForm, setConversionForm] = useState({ fromUnitCode: '', toUnitCode: '', conversionFactor: 1 })

  // Queries
  const { data: categories = [], isLoading: loadingCategories } = useQuery({
    queryKey: ['inventory-categories'],
    queryFn: () => api.get('/api/company/inventory-settings/categories').then(r => r.data)
  })

  const { data: units = [], isLoading: loadingUnits } = useQuery({
    queryKey: ['inventory-units'],
    queryFn: () => api.get('/api/company/inventory-settings/units').then(r => r.data)
  })

  // Category mutations
  const createCategory = useMutation({
    mutationFn: (data: any) => api.post('/api/company/inventory-settings/categories', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory-categories'] }); closeCategoryModal() }
  })
  const updateCategory = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => api.put(`/api/company/inventory-settings/categories/${id}`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory-categories'] }); closeCategoryModal() }
  })
  const toggleCategory = useMutation({
    mutationFn: (id: number) => api.patch(`/api/company/inventory-settings/categories/${id}/toggle`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory-categories'] })
  })
  const deleteCategory = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/inventory-settings/categories/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory-categories'] })
  })

  // Unit mutations
  const createUnit = useMutation({
    mutationFn: (data: any) => api.post('/api/company/inventory-settings/units', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory-units'] }); closeUnitModal() }
  })
  const updateUnit = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => api.put(`/api/company/inventory-settings/units/${id}`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['inventory-units'] }); closeUnitModal() }
  })
  const toggleUnit = useMutation({
    mutationFn: (id: number) => api.patch(`/api/company/inventory-settings/units/${id}/toggle`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory-units'] })
  })
  const deleteUnit = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/inventory-settings/units/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['inventory-units'] })
  })

  // Conversions query
  const { data: conversions = [], isLoading: loadingConversions } = useQuery({
    queryKey: ['unit-conversions'],
    queryFn: () => api.get('/api/company/inventory-settings/conversions').then(r => r.data)
  })

  // Conversion mutations
  const createConversion = useMutation({
    mutationFn: (data: any) => api.post('/api/company/inventory-settings/conversions', data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['unit-conversions'] }); closeConversionModal() }
  })
  const updateConversion = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => api.put(`/api/company/inventory-settings/conversions/${id}`, data),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['unit-conversions'] }); closeConversionModal() }
  })
  const deleteConversion = useMutation({
    mutationFn: (id: number) => api.delete(`/api/company/inventory-settings/conversions/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['unit-conversions'] })
  })

  // Category handlers
  const openCategoryModal = (category?: Category) => {
    if (category) {
      setEditingCategory(category)
      setCategoryForm({ name: category.name, description: category.description || '', parentCategoryId: category.parentCategoryId || null, sortOrder: category.sortOrder })
    } else {
      setEditingCategory(null)
      setCategoryForm({ name: '', description: '', parentCategoryId: null, sortOrder: 0 })
    }
    setShowCategoryModal(true)
  }

  const closeCategoryModal = () => {
    setShowCategoryModal(false)
    setEditingCategory(null)
    setCategoryForm({ name: '', description: '', parentCategoryId: null, sortOrder: 0 })
  }

  // Get parent categories (categories without parent)
  const parentCategories = categories.filter((c: Category) => !c.parentCategoryId)

  const handleCategorySubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingCategory) {
      updateCategory.mutate({ id: editingCategory.id, data: { ...categoryForm, isActive: editingCategory.isActive } })
    } else {
      createCategory.mutate(categoryForm)
    }
  }

  // Unit handlers
  const openUnitModal = (unit?: UnitOfMeasure) => {
    if (unit) {
      setEditingUnit(unit)
      setUnitForm({ code: unit.code, name: unit.name, symbol: unit.symbol || '', sortOrder: unit.sortOrder })
    } else {
      setEditingUnit(null)
      setUnitForm({ code: '', name: '', symbol: '', sortOrder: 0 })
    }
    setShowUnitModal(true)
  }

  const closeUnitModal = () => {
    setShowUnitModal(false)
    setEditingUnit(null)
    setUnitForm({ code: '', name: '', symbol: '', sortOrder: 0 })
  }

  const handleUnitSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingUnit) {
      updateUnit.mutate({ id: editingUnit.id, data: { ...unitForm, isActive: editingUnit.isActive } })
    } else {
      createUnit.mutate(unitForm)
    }
  }

  // Conversion handlers
  const openConversionModal = (conversion?: UnitConversion) => {
    if (conversion) {
      setEditingConversion(conversion)
      setConversionForm({ 
        fromUnitCode: conversion.fromUnitCode, 
        toUnitCode: conversion.toUnitCode, 
        conversionFactor: conversion.conversionFactor 
      })
    } else {
      setEditingConversion(null)
      setConversionForm({ fromUnitCode: '', toUnitCode: '', conversionFactor: 1 })
    }
    setShowConversionModal(true)
  }

  const closeConversionModal = () => {
    setShowConversionModal(false)
    setEditingConversion(null)
    setConversionForm({ fromUnitCode: '', toUnitCode: '', conversionFactor: 1 })
  }

  const handleConversionSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (editingConversion) {
      updateConversion.mutate({ id: editingConversion.id, data: { conversionFactor: conversionForm.conversionFactor, isActive: editingConversion.isActive } })
    } else {
      createConversion.mutate(conversionForm)
    }
  }

  // Get unique conversions (only show from→to, not reverse)
  const uniqueConversions = conversions.filter((c: UnitConversion, index: number, arr: UnitConversion[]) => {
    const reverseIndex = arr.findIndex((r: UnitConversion) => r.fromUnitCode === c.toUnitCode && r.toUnitCode === c.fromUnitCode)
    return reverseIndex === -1 || index < reverseIndex
  })

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">{t('inventorySettings.title')}</h1>
        <p className="text-gray-500">{t('inventorySettings.subtitle')}</p>
      </div>

      {/* Tabs */}
      <div className="flex gap-1 p-1 bg-gray-100 rounded-xl w-fit mb-6">
        <button
          onClick={() => setActiveTab('categories')}
          className={`flex items-center gap-2 px-4 py-2.5 rounded-lg font-medium transition-all ${
            activeTab === 'categories' 
              ? 'bg-gray-900 text-primary-600 shadow-sm' 
              : 'text-gray-600 hover:text-gray-900'
          }`}
        >
          <FolderOpen size={18} />
          {t('inventorySettings.categories')}
          <span className="bg-gray-200 text-gray-600 text-xs px-2 py-0.5 rounded-full">{categories.length}</span>
        </button>
        <button
          onClick={() => setActiveTab('units')}
          className={`flex items-center gap-2 px-4 py-2.5 rounded-lg font-medium transition-all ${
            activeTab === 'units' 
              ? 'bg-gray-900 text-primary-600 shadow-sm' 
              : 'text-gray-600 hover:text-gray-900'
          }`}
        >
          <Scale size={18} />
          {t('inventorySettings.unitsOfMeasure')}
          <span className="bg-gray-200 text-gray-600 text-xs px-2 py-0.5 rounded-full">{units.length}</span>
        </button>
        <button
          onClick={() => setActiveTab('conversions')}
          className={`flex items-center gap-2 px-4 py-2.5 rounded-lg font-medium transition-all ${
            activeTab === 'conversions' 
              ? 'bg-gray-900 text-primary-600 shadow-sm' 
              : 'text-gray-600 hover:text-gray-900'
          }`}
        >
          <ArrowRightLeft size={18} />
          {t('inventorySettings.conversions')}
          <span className="bg-gray-200 text-gray-600 text-xs px-2 py-0.5 rounded-full">{uniqueConversions.length}</span>
        </button>
      </div>

      {/* Categories Tab */}
      {activeTab === 'categories' && (
        <div className="card">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900">{t('inventorySettings.ingredientCategories')}</h2>
            <button onClick={() => openCategoryModal()} className="btn-primary flex items-center gap-2">
              <Plus size={18} /> {t('inventorySettings.addCategory')}
            </button>
          </div>

          {loadingCategories ? (
            <div className="text-center py-8 text-gray-500">{t('common.loading')}</div>
          ) : categories.length === 0 ? (
            <div className="text-center py-12">
              <FolderOpen size={48} className="mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500 mb-4">{t('inventorySettings.noCategories')}</p>
              <button onClick={() => openCategoryModal()} className="btn-primary">
                {t('inventorySettings.createFirstCategory')}
              </button>
            </div>
          ) : (
            <div className="grid gap-3">
              {categories.map((cat: Category) => (
                <div 
                  key={cat.id} 
                  className={`flex items-center justify-between p-4 rounded-xl border ${
                    cat.isActive ? 'bg-gray-900 border-gray-200' : 'bg-gray-800 border-gray-100'
                  }`}
                >
                  <div className="flex items-center gap-4">
                    {cat.parentCategoryId && <div className="w-6" />}
                    <div className="text-gray-300 cursor-move">
                      <GripVertical size={20} />
                    </div>
                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${
                      cat.isActive ? (cat.parentCategoryId ? 'bg-blue-100' : 'bg-primary-100') : 'bg-gray-200'
                    }`}>
                      <FolderOpen size={20} className={cat.isActive ? (cat.parentCategoryId ? 'text-blue-600' : 'text-primary-600') : 'text-gray-400'} />
                    </div>
                    <div>
                      <div className="flex items-center gap-2">
                        <h3 className={`font-medium ${cat.isActive ? 'text-gray-900' : 'text-gray-400'}`}>
                          {cat.name}
                        </h3>
                        {cat.parentCategoryName && (
                          <span className="text-xs bg-gray-100 text-gray-500 px-2 py-0.5 rounded">
                            {t('inventorySettings.in')} {cat.parentCategoryName}
                          </span>
                        )}
                      </div>
                      {cat.description && (
                        <p className="text-sm text-gray-400">{cat.description}</p>
                      )}
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <button
                      onClick={() => toggleCategory.mutate(cat.id)}
                      className={`px-3 py-1 rounded-lg text-xs font-medium transition-colors ${
                        cat.isActive 
                          ? 'bg-green-100 text-green-700 hover:bg-green-200' 
                          : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
                      }`}
                    >
                      {cat.isActive ? t('common.active') : t('common.inactive')}
                    </button>
                    <button onClick={() => openCategoryModal(cat)} className="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                      <Edit size={16} />
                    </button>
                    <button onClick={() => deleteCategory.mutate(cat.id)} className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Units Tab */}
      {activeTab === 'units' && (
        <div className="card">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900">{t('inventorySettings.unitsOfMeasure')}</h2>
            <button onClick={() => openUnitModal()} className="btn-primary flex items-center gap-2">
              <Plus size={18} /> {t('inventorySettings.addUnit')}
            </button>
          </div>

          {loadingUnits ? (
            <div className="text-center py-8 text-gray-500">{t('common.loading')}</div>
          ) : units.length === 0 ? (
            <div className="text-center py-12">
              <Scale size={48} className="mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500 mb-4">{t('inventorySettings.noUnits')}</p>
              <button onClick={() => openUnitModal()} className="btn-primary">
                {t('inventorySettings.createFirstUnit')}
              </button>
            </div>
          ) : (
            <div className="grid gap-3">
              {units.map((unit: UnitOfMeasure) => (
                <div 
                  key={unit.id} 
                  className={`flex items-center justify-between p-4 rounded-xl border ${
                    unit.isActive ? 'bg-gray-900 border-gray-200' : 'bg-gray-800 border-gray-100'
                  }`}
                >
                  <div className="flex items-center gap-4">
                    <div className="text-gray-300 cursor-move">
                      <GripVertical size={20} />
                    </div>
                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${
                      unit.isActive ? 'bg-blue-100' : 'bg-gray-200'
                    }`}>
                      <Scale size={20} className={unit.isActive ? 'text-blue-600' : 'text-gray-400'} />
                    </div>
                    <div>
                      <div className="flex items-center gap-2">
                        <h3 className={`font-medium ${unit.isActive ? 'text-gray-900' : 'text-gray-400'}`}>
                          {unit.name}
                        </h3>
                        <span className="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded font-mono">
                          {unit.code}
                        </span>
                      </div>
                      {unit.symbol && (
                        <p className="text-sm text-gray-400">{t('inventorySettings.symbol')}: {unit.symbol}</p>
                      )}
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <button
                      onClick={() => toggleUnit.mutate(unit.id)}
                      className={`px-3 py-1 rounded-lg text-xs font-medium transition-colors ${
                        unit.isActive 
                          ? 'bg-green-100 text-green-700 hover:bg-green-200' 
                          : 'bg-gray-100 text-gray-500 hover:bg-gray-200'
                      }`}
                    >
                      {unit.isActive ? t('common.active') : t('common.inactive')}
                    </button>
                    <button onClick={() => openUnitModal(unit)} className="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                      <Edit size={16} />
                    </button>
                    <button onClick={() => deleteUnit.mutate(unit.id)} className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Conversions Tab */}
      {activeTab === 'conversions' && (
        <div className="card">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-lg font-semibold text-gray-900">{t('inventorySettings.unitConversions')}</h2>
            <button onClick={() => openConversionModal()} className="btn-primary flex items-center gap-2">
              <Plus size={18} /> {t('inventorySettings.addConversion')}
            </button>
          </div>

          {loadingConversions ? (
            <div className="text-center py-8 text-gray-500">{t('common.loading')}</div>
          ) : uniqueConversions.length === 0 ? (
            <div className="text-center py-12">
              <ArrowRightLeft size={48} className="mx-auto text-gray-300 mb-4" />
              <p className="text-gray-500 mb-4">{t('inventorySettings.noConversions')}</p>
              <p className="text-sm text-gray-400 mb-4">{t('inventorySettings.conversionHint')}</p>
              <button onClick={() => openConversionModal()} className="btn-primary">
                {t('inventorySettings.createFirstConversion')}
              </button>
            </div>
          ) : (
            <div className="grid gap-3">
              {uniqueConversions.map((conv: UnitConversion) => (
                <div 
                  key={conv.id} 
                  className={`flex items-center justify-between p-4 rounded-xl border ${
                    conv.isActive ? 'bg-gray-900 border-gray-200' : 'bg-gray-800 border-gray-100'
                  }`}
                >
                  <div className="flex items-center gap-4">
                    <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${
                      conv.isActive ? 'bg-purple-100' : 'bg-gray-200'
                    }`}>
                      <ArrowRightLeft size={20} className={conv.isActive ? 'text-purple-600' : 'text-gray-400'} />
                    </div>
                    <div>
                      <div className="flex items-center gap-2">
                        <span className="font-mono bg-gray-100 px-2 py-0.5 rounded text-sm">{conv.fromUnitCode}</span>
                        <ArrowRightLeft size={14} className="text-gray-400" />
                        <span className="font-mono bg-gray-100 px-2 py-0.5 rounded text-sm">{conv.toUnitCode}</span>
                      </div>
                      <p className="text-sm text-gray-500 mt-1">
                        1 {conv.fromUnitCode} = {conv.conversionFactor} {conv.toUnitCode}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <button onClick={() => openConversionModal(conv)} className="p-2 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                      <Edit size={16} />
                    </button>
                    <button onClick={() => deleteConversion.mutate(conv.id)} className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Category Modal */}
      {showCategoryModal && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4" onClick={closeCategoryModal}>
          <div className="bg-gray-900 rounded-2xl shadow-2xl w-full max-w-md" onClick={e => e.stopPropagation()}>
            <div className="flex items-center justify-between p-6 border-b border-gray-700">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-primary-100 rounded-xl flex items-center justify-center">
                  <FolderOpen className="w-5 h-5 text-primary-600" />
                </div>
                <div>
                  <h2 className="text-xl font-semibold text-gray-900">{editingCategory ? t('inventorySettings.editCategory') : t('inventorySettings.addCategory')}</h2>
                  <p className="text-sm text-gray-500">{t('inventorySettings.organizeItems')}</p>
                </div>
              </div>
              <button onClick={closeCategoryModal} className="p-2 hover:bg-gray-100 rounded-lg">
                <X size={20} className="text-gray-500" />
              </button>
            </div>

            <form onSubmit={handleCategorySubmit} className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Tag size={14} className="inline mr-2" />{t('inventorySettings.categoryName')} *
                </label>
                <input
                  type="text"
                  placeholder={t('inventorySettings.categoryExample')}
                  value={categoryForm.name}
                  onChange={e => setCategoryForm({ ...categoryForm, name: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <FolderOpen size={14} className="inline mr-2" />{t('inventorySettings.parentCategory')}
                </label>
                <select
                  value={categoryForm.parentCategoryId || ''}
                  onChange={e => setCategoryForm({ ...categoryForm, parentCategoryId: e.target.value ? parseInt(e.target.value) : null })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">{t('inventorySettings.noneTopLevel')}</option>
                  {parentCategories
                    .filter((c: Category) => c.id !== editingCategory?.id)
                    .map((cat: Category) => (
                      <option key={cat.id} value={cat.id}>{cat.name}</option>
                    ))}
                </select>
                <p className="text-xs text-gray-400 mt-1">{t('inventorySettings.parentCategoryHint')}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <FileText size={14} className="inline mr-2" />{t('inventorySettings.description')}
                </label>
                <textarea
                  placeholder={t('inventorySettings.optionalDescription')}
                  value={categoryForm.description}
                  onChange={e => setCategoryForm({ ...categoryForm, description: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                  rows={2}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <GripVertical size={14} className="inline mr-2" />{t('inventorySettings.sortOrder')}
                </label>
                <input
                  type="number"
                  value={categoryForm.sortOrder}
                  onChange={e => setCategoryForm({ ...categoryForm, sortOrder: parseInt(e.target.value) || 0 })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="flex gap-3 pt-4 border-t border-gray-100">
                <button type="button" onClick={closeCategoryModal} className="flex-1 px-4 py-2.5 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium">
                  {t('common.cancel')}
                </button>
                <button type="submit" disabled={createCategory.isPending || updateCategory.isPending} className="flex-1 px-4 py-2.5 bg-primary-600 hover:bg-primary-700 text-white rounded-xl font-medium disabled:opacity-50">
                  {editingCategory ? t('common.update') : t('common.create')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Unit Modal */}
      {showUnitModal && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4" onClick={closeUnitModal}>
          <div className="bg-gray-900 rounded-2xl shadow-2xl w-full max-w-md" onClick={e => e.stopPropagation()}>
            <div className="flex items-center justify-between p-6 border-b border-gray-700">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-blue-100 rounded-xl flex items-center justify-center">
                  <Scale className="w-5 h-5 text-blue-600" />
                </div>
                <div>
                  <h2 className="text-xl font-semibold text-gray-900">{editingUnit ? t('inventorySettings.editUnit') : t('inventorySettings.addUnit')}</h2>
                  <p className="text-sm text-gray-500">{t('inventorySettings.defineMeasurementUnits')}</p>
                </div>
              </div>
              <button onClick={closeUnitModal} className="p-2 hover:bg-gray-100 rounded-lg">
                <X size={20} className="text-gray-500" />
              </button>
            </div>

            <form onSubmit={handleUnitSubmit} className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Hash size={14} className="inline mr-2" />{t('inventorySettings.unitCode')} *
                </label>
                <input
                  type="text"
                  placeholder={t('inventorySettings.unitCodeExample')}
                  value={unitForm.code}
                  onChange={e => setUnitForm({ ...unitForm, code: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500 font-mono"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Type size={14} className="inline mr-2" />{t('inventorySettings.fullName')} *
                </label>
                <input
                  type="text"
                  placeholder={t('inventorySettings.unitNameExample')}
                  value={unitForm.name}
                  onChange={e => setUnitForm({ ...unitForm, name: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Tag size={14} className="inline mr-2" />{t('inventorySettings.symbol')}
                </label>
                <input
                  type="text"
                  placeholder={t('inventorySettings.unitSymbolExample')}
                  value={unitForm.symbol}
                  onChange={e => setUnitForm({ ...unitForm, symbol: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <GripVertical size={14} className="inline mr-2" />{t('inventorySettings.sortOrder')}
                </label>
                <input
                  type="number"
                  value={unitForm.sortOrder}
                  onChange={e => setUnitForm({ ...unitForm, sortOrder: parseInt(e.target.value) || 0 })}
                  className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="flex gap-3 pt-4 border-t border-gray-100">
                <button type="button" onClick={closeUnitModal} className="flex-1 px-4 py-2.5 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium">
                  {t('common.cancel')}
                </button>
                <button type="submit" disabled={createUnit.isPending || updateUnit.isPending} className="flex-1 px-4 py-2.5 bg-primary-600 hover:bg-primary-700 text-white rounded-xl font-medium disabled:opacity-50">
                  {editingUnit ? t('common.update') : t('common.create')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Conversion Modal */}
      {showConversionModal && (
        <div className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4" onClick={closeConversionModal}>
          <div className="bg-gray-900 rounded-2xl shadow-2xl w-full max-w-md" onClick={e => e.stopPropagation()}>
            <div className="flex items-center justify-between p-6 border-b border-gray-700">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-purple-100 rounded-xl flex items-center justify-center">
                  <ArrowRightLeft className="w-5 h-5 text-purple-600" />
                </div>
                <div>
                  <h2 className="text-xl font-semibold text-gray-900">{editingConversion ? t('inventorySettings.editConversion') : t('inventorySettings.addConversion')}</h2>
                  <p className="text-sm text-gray-500">{t('inventorySettings.defineConversionRatio')}</p>
                </div>
              </div>
              <button onClick={closeConversionModal} className="p-2 hover:bg-gray-100 rounded-lg">
                <X size={20} className="text-gray-500" />
              </button>
            </div>

            <form onSubmit={handleConversionSubmit} className="p-6 space-y-5">
              {!editingConversion && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      {t('inventorySettings.fromUnit')} *
                    </label>
                    <select
                      value={conversionForm.fromUnitCode}
                      onChange={e => setConversionForm({ ...conversionForm, fromUnitCode: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                      required
                    >
                      <option value="">{t('inventorySettings.selectUnit')}</option>
                      {units.filter((u: UnitOfMeasure) => u.isActive).map((unit: UnitOfMeasure) => (
                        <option key={unit.id} value={unit.code}>{unit.name} ({unit.code})</option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      {t('inventorySettings.toUnit')} *
                    </label>
                    <select
                      value={conversionForm.toUnitCode}
                      onChange={e => setConversionForm({ ...conversionForm, toUnitCode: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                      required
                    >
                      <option value="">{t('inventorySettings.selectUnit')}</option>
                      {units.filter((u: UnitOfMeasure) => u.isActive && u.code !== conversionForm.fromUnitCode).map((unit: UnitOfMeasure) => (
                        <option key={unit.id} value={unit.code}>{unit.name} ({unit.code})</option>
                      ))}
                    </select>
                  </div>
                </>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  {t('inventorySettings.conversionFactor')} *
                </label>
                <div className="flex items-center gap-3">
                  <span className="text-gray-600 whitespace-nowrap">1 {conversionForm.fromUnitCode || '?'} =</span>
                  <input
                    type="number"
                    step="0.000001"
                    min="0.000001"
                    placeholder={t('inventorySettings.conversionExample')}
                    value={conversionForm.conversionFactor}
                    onChange={e => setConversionForm({ ...conversionForm, conversionFactor: parseFloat(e.target.value) || 0 })}
                    className="flex-1 px-4 py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                    required
                  />
                  <span className="text-gray-600 whitespace-nowrap">{conversionForm.toUnitCode || '?'}</span>
                </div>
                <p className="text-xs text-gray-400 mt-2">
                  {t('inventorySettings.conversionExampleHint')}
                </p>
              </div>

              <div className="bg-blue-50 p-4 rounded-xl">
                <p className="text-sm text-blue-700">
                  <strong>{t('common.note')}:</strong> {t('inventorySettings.reverseConversionNote')}
                  {conversionForm.fromUnitCode && conversionForm.toUnitCode && conversionForm.conversionFactor > 0 && (
                    <span className="block mt-1">
                      1 {conversionForm.toUnitCode} = {(1 / conversionForm.conversionFactor).toFixed(6)} {conversionForm.fromUnitCode}
                    </span>
                  )}
                </p>
              </div>

              <div className="flex gap-3 pt-4 border-t border-gray-100">
                <button type="button" onClick={closeConversionModal} className="flex-1 px-4 py-2.5 bg-gray-100 hover:bg-gray-200 text-gray-700 rounded-xl font-medium">
                  {t('common.cancel')}
                </button>
                <button type="submit" disabled={createConversion.isPending || updateConversion.isPending} className="flex-1 px-4 py-2.5 bg-primary-600 hover:bg-primary-700 text-white rounded-xl font-medium disabled:opacity-50">
                  {editingConversion ? t('common.update') : t('common.create')}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
