using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] InventoryScriptable m_inventoryScriptable; 
        [SerializeField] ItemScripable[] m_spawnItems; 

        private List<InventoryItem> m_items;

        private void Awake()
        {
            m_inventoryScriptable.Inventory = this;
            m_items = new List<InventoryItem>();
        }

    }
}