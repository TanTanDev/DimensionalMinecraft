using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingElement : MonoBehaviour
{
    [SerializeField] private Button m_button;
    [SerializeField] private RawImage m_textureImage;
    [SerializeField] private Text m_nameText;
    private Crafting m_crafting;
    private CraftingRecipeScriptable m_recipe;

    public void Setup(CraftingRecipeScriptable a_recipe, Crafting a_crafting)
    {
        m_recipe = a_recipe;
        m_nameText.text = a_recipe.ItemToCraft.Name;
        m_textureImage.texture = a_recipe.ItemToCraft.m_imageIcon;
        m_button.onClick.AddListener(HandleClick);
        m_crafting = a_crafting;
    }

    private void HandleClick()
    {
        // Attempt to craft item
        m_crafting.SetupRequirements(m_recipe);
    }
}
