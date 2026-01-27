import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { recipesApi } from '../lib/api';
import api from '../lib/api';
import { Plus, Edit, Trash2, ChefHat } from 'lucide-react';

export default function Recipes() {
  const queryClient = useQueryClient();
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [formData, setFormData] = useState({
    menuItemId: 0,
    yield: 1,
    ingredients: [] as { inventoryItemId: number; quantity: number; unit: string }[]
  });
  const [newIngredient, setNewIngredient] = useState({ inventoryItemId: 0, quantity: 0, unit: '' });

  const { data: recipes = [], isLoading } = useQuery({
    queryKey: ['recipes'],
    queryFn: recipesApi.getAll
  });

  const { data: menuItems = [] } = useQuery({
    queryKey: ['menu-items'],
    queryFn: () => api.get('/api/company/menu-items').then(r => r.data)
  });

  const { data: inventoryItems = [] } = useQuery({
    queryKey: ['inventory'],
    queryFn: () => api.get('/api/company/inventory').then(r => r.data)
  });

  const createMutation = useMutation({
    mutationFn: recipesApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['recipes'] });
      resetForm();
    }
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => recipesApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['recipes'] });
      resetForm();
    }
  });

  const deleteMutation = useMutation({
    mutationFn: recipesApi.delete,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['recipes'] })
  });

  const resetForm = () => {
    setShowForm(false);
    setEditingId(null);
    setFormData({ menuItemId: 0, yield: 1, ingredients: [] });
    setNewIngredient({ inventoryItemId: 0, quantity: 0, unit: '' });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log('Submitting formData:', formData);
    if (editingId) {
      updateMutation.mutate({ id: editingId, data: formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleEdit = async (recipe: any) => {
    // Fetch full recipe details including ingredients
    const fullRecipe = await recipesApi.getById(recipe.id);
    setEditingId(recipe.id);
    setFormData({
      menuItemId: fullRecipe.menuItemId,
      yield: fullRecipe.yield,
      ingredients: fullRecipe.ingredients?.map((ing: any) => ({
        inventoryItemId: ing.inventoryItemId,
        quantity: ing.quantity,
        unit: ing.unit
      })) || []
    });
    setShowForm(true);
  };

  const handleDelete = (id: number) => {
    if (confirm('Are you sure you want to delete this recipe?')) {
      deleteMutation.mutate(id);
    }
  };

  const addIngredient = () => {
    console.log('Adding ingredient:', newIngredient, 'inventoryItems:', inventoryItems);
    if (newIngredient.inventoryItemId && newIngredient.quantity > 0) {
      const item = inventoryItems.find((i: any) => i.id === newIngredient.inventoryItemId);
      console.log('Found item:', item);
      const newIng = { ...newIngredient, unit: item?.unitOfMeasure || newIngredient.unit };
      console.log('New ingredient to add:', newIng);
      setFormData({
        ...formData,
        ingredients: [...formData.ingredients, newIng]
      });
      setNewIngredient({ inventoryItemId: 0, quantity: 0, unit: '' });
    } else {
      console.log('Validation failed - inventoryItemId:', newIngredient.inventoryItemId, 'quantity:', newIngredient.quantity);
    }
  };

  const removeIngredient = (index: number) => {
    setFormData({
      ...formData,
      ingredients: formData.ingredients.filter((_, i) => i !== index)
    });
  };

  const getInventoryItemName = (id: number) => {
    const item = inventoryItems.find((i: any) => i.id === id);
    return item?.name || 'Unknown';
  };

  const getMenuItemName = (id: number) => {
    const item = menuItems.find((i: any) => i.id === id);
    return item?.name || 'Unknown';
  };

  if (isLoading) return <div>Loading...</div>;

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold flex items-center gap-2">
          <ChefHat className="w-6 h-6" /> Recipes
        </h1>
        <button
          onClick={() => setShowForm(true)}
          className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
        >
          <Plus className="w-4 h-4" /> Add Recipe
        </button>
      </div>

      {showForm && (
        <div className="bg-gray-900 rounded-lg shadow p-6 mb-6">
          <h2 className="text-lg font-semibold mb-4">{editingId ? 'Edit Recipe' : 'Add Recipe'}</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium mb-1">Menu Item *</label>
                <select
                  value={formData.menuItemId}
                  onChange={(e) => setFormData({ ...formData, menuItemId: parseInt(e.target.value) })}
                  className="w-full border rounded-lg px-3 py-2"
                  required
                >
                  <option value={0}>Select menu item</option>
                  {menuItems.map((item: any) => (
                    <option key={item.id} value={item.id}>
                      {item.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Yield (portions)</label>
                <input
                  type="number"
                  value={formData.yield}
                  onChange={(e) => setFormData({ ...formData, yield: parseInt(e.target.value) })}
                  className="w-full border rounded-lg px-3 py-2"
                  min={1}
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Ingredients</label>
              <div className="border rounded-lg p-4 bg-gray-800">
                <div className="flex gap-2 mb-4">
                  <select
                    value={newIngredient.inventoryItemId}
                    onChange={(e) => setNewIngredient({ ...newIngredient, inventoryItemId: parseInt(e.target.value) })}
                    className="flex-1 border rounded-lg px-3 py-2"
                  >
                    <option value={0}>Select ingredient</option>
                    {inventoryItems.map((item: any) => (
                      <option key={item.id} value={item.id}>
                        {item.name} ({item.unitOfMeasure})
                      </option>
                    ))}
                  </select>
                  <input
                    type="number"
                    step="0.001"
                    placeholder="Qty"
                    value={newIngredient.quantity || ''}
                    onChange={(e) => setNewIngredient({ ...newIngredient, quantity: parseFloat(e.target.value) })}
                    className="w-24 border rounded-lg px-3 py-2"
                  />
                  <button
                    type="button"
                    onClick={addIngredient}
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700"
                  >
                    <Plus className="w-4 h-4" />
                  </button>
                </div>

                {formData.ingredients.length > 0 ? (
                  <table className="table">
                    <thead>
                      <tr className="text-left text-sm text-gray-500">
                        <th className="pb-2">Ingredient</th>
                        <th className="pb-2">Quantity</th>
                        <th className="pb-2">Unit</th>
                        <th className="pb-2"></th>
                      </tr>
                    </thead>
                    <tbody>
                      {formData.ingredients.map((ing, index) => (
                        <tr key={index} className="border-t">
                          <td className="py-2">{getInventoryItemName(ing.inventoryItemId)}</td>
                          <td className="py-2">{ing.quantity}</td>
                          <td className="py-2">{ing.unit}</td>
                          <td className="py-2">
                            <button
                              type="button"
                              onClick={() => removeIngredient(index)}
                              className="text-red-600 hover:text-red-800"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                ) : (
                  <p className="text-gray-500 text-sm">No ingredients added yet</p>
                )}
              </div>
            </div>

            <div className="flex gap-4">
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

      <div className="bg-gray-900 rounded-lg shadow overflow-hidden">
        <table className="table">
          <thead className="bg-gray-800">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Menu Item</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Yield</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ingredients</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Est. Cost</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {recipes.map((recipe: any) => (
              <tr key={recipe.id}>
                <td className="px-6 py-4 font-medium">{recipe.menuItemName || getMenuItemName(recipe.menuItemId)}</td>
                <td className="px-6 py-4">{recipe.yield}</td>
                <td className="px-6 py-4">{recipe.ingredientCount || recipe.ingredients?.length || 0} items</td>
                <td className="px-6 py-4">${recipe.estimatedCost?.toFixed(2) || '0.00'}</td>
                <td className="px-6 py-4">
                  <div className="flex gap-2">
                    <button onClick={() => handleEdit(recipe)} className="text-blue-600 hover:text-blue-800">
                      <Edit className="w-4 h-4" />
                    </button>
                    <button onClick={() => handleDelete(recipe.id)} className="text-red-600 hover:text-red-800">
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {recipes.length === 0 && (
              <tr>
                <td colSpan={5} className="px-6 py-8 text-center text-gray-500">
                  No recipes defined yet
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
