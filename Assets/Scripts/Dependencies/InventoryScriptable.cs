using UnityEngine;

namespace Tantan
{
    [CreateAssetMenu(menuName = "Tantan/InventoryScriptable", fileName = "InventoryScriptable")]
    public class InventoryScriptable : ScriptableObject
    {
        public Inventory Inventory;
    }
}