using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Tantan;
public class Crafting : MonoBehaviour
{
    [SerializeField] private GameObject m_toggleGameObject;
    [SerializeField] private RectTransform m_contentRect;
    [SerializeField] private RectTransform m_requirementContentRect;
    [SerializeField] private CraftingRecipeScriptable[] m_craftingRecipies;
    [SerializeField] private CraftingElement m_craftingElementPrefab;
    [SerializeField] private CraftingRequirementElement m_craftingRequirementPrefab;
    [SerializeField] private Button m_craftButton;
    [SerializeField] private InventoryScriptable m_inventoryScriptable;
    private List<CraftingRequirementElement> m_requirementElements;

    private CraftingRecipeScriptable m_currentRecipe;
    private bool m_isOpen;

    private void Awake()
    {
        m_toggleGameObject.SetActive(false);
        m_requirementElements = new List<CraftingRequirementElement>(2);
        for(int i = 0 ; i < m_craftingRecipies.Length; i++) 
        {
            CraftingElement craftingElement = Instantiate(m_craftingElementPrefab, m_contentRect);
            craftingElement.Setup(m_craftingRecipies[i], this);
        }
        m_craftButton.onClick.AddListener(OnCraft);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            m_isOpen = !m_isOpen;
            m_toggleGameObject.SetActive(m_isOpen);
        }
    }

    private void OnCraft()
    {
        if(m_currentRecipe == null)
            return;
        for(int i = 0; i < m_currentRecipe.RequiredItems.Length; i++)
        {
            var requirement = m_currentRecipe.RequiredItems[i];
            int itemAmount = m_inventoryScriptable.Inventory.GetAmount(requirement.Item);
            if(itemAmount < requirement.Amount)
                return;
        }
        m_inventoryScriptable.Inventory.AddItem(m_currentRecipe.ItemToCraft, m_currentRecipe.ItemsToGet);
        for(int i = 0; i < m_currentRecipe.RequiredItems.Length; i++)
        {
            var requirement = m_currentRecipe.RequiredItems[i];
            int itemAmount = m_inventoryScriptable.Inventory.GetAmount(requirement.Item);
            m_inventoryScriptable.Inventory.RemoveItem(requirement.Item, requirement.Amount);
        }
    }

    public void SetupRequirements(CraftingRecipeScriptable a_recipe)
    {
        for(int i = 0; i < m_requirementElements.Count; i++)
        {
            Destroy(m_requirementElements[i].gameObject);
        }
        m_requirementElements.Clear();
        for(int i = 0; i < a_recipe.RequiredItems.Length; i++)
        {
            CraftingRequirementElement reqElement = Instantiate(m_craftingRequirementPrefab, m_requirementContentRect);
            var requirement = a_recipe.RequiredItems[i];
            reqElement.Setup(requirement);
            m_requirementElements.Add(reqElement);
        }
        m_currentRecipe = a_recipe;
    }
}
