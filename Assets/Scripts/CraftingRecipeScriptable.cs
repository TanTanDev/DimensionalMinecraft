using UnityEngine;

using Tantan;

[System.Serializable]
public class ItemAndAmount
{
    public ItemScriptable Item;
    public int Amount;
}

[CreateAssetMenu(menuName = "Tantan/Recipe", fileName = "recipe")]
public class CraftingRecipeScriptable : ScriptableObject
{
    public ItemScriptable ItemToCraft;
    public int ItemsToGet = 1;
    public ItemAndAmount[] RequiredItems;
}
