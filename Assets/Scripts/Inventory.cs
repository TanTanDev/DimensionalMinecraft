using System.Collections.Generic;
using UnityEngine;

namespace Tantan
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] InventoryScriptable m_inventoryScriptable; 
        [SerializeField] ItemScripable[] m_spawnItems; 
        [SerializeField] RectTransform m_contentTransform; 
        [SerializeField] ItemElement m_itemPrefab; 
        private int m_selectedIndex = 0;

        private List<InventoryItem> m_items;

        private void Awake()
        {
            m_inventoryScriptable.Inventory = this;
            m_items = new List<InventoryItem>();
            for(int i = 0; i < m_spawnItems.Length; i++)
            {
                ItemElement itemElement = Instantiate(m_itemPrefab, m_contentTransform);
                itemElement.SetTexture(m_spawnItems[i].m_imageIcon);
                itemElement.SetAmount(0);
                m_items.Add(new InventoryItem(m_spawnItems[i], itemElement));
            }
            if(m_items.Count > 0)
                m_items[0].ItemElement.SetIsSelected(true);
        }

        public void SetSelected(int a_selectedIndex)
        {
            m_items[m_selectedIndex].ItemElement.SetIsSelected(false);
            m_items[a_selectedIndex].ItemElement.SetIsSelected(true);
            m_selectedIndex = a_selectedIndex;
        }
    }
}