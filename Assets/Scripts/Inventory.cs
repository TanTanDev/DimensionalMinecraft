using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Tantan
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] InventoryScriptable m_inventoryScriptable; 
        [SerializeField] ItemScriptable[] m_spawnItems; 
        [SerializeField] RectTransform m_contentTransform; 
        [SerializeField] ItemElement m_itemPrefab; 
        [SerializeField] ReactiveEventItemScriptable m_onPerformItemPickup; 
        private CompositeDisposable m_disposables;
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
                int index = i;
                itemElement.Button.onClick.AddListener(()=>{this.SetSelected(index);});
                m_items.Add(new InventoryItem(m_spawnItems[i], itemElement));
            }
            if(m_items.Count > 0)
                m_items[0].ItemElement.SetIsSelected(true);
            m_disposables = new CompositeDisposable();
            m_onPerformItemPickup.Subscribe(HandleItemPickup).AddTo(m_disposables);
        }

        private void Update()
        {
            if(Input.mouseScrollDelta.y != 0.0f)
            {
                m_items[m_selectedIndex].ItemElement.SetIsSelected(false);
                if(Input.mouseScrollDelta.y > 0f)
                    m_selectedIndex++;
                else if(Input.mouseScrollDelta.y < 0f)
                    m_selectedIndex--;
                // clamp selected index
                if(m_selectedIndex < 0)
                    m_selectedIndex = m_items.Count - 1;
                if(m_selectedIndex > m_items.Count - 1)
                    m_selectedIndex = 0;
                m_items[m_selectedIndex].ItemElement.SetIsSelected(true);
            }
        }

        private void HandleItemPickup(ItemScriptable a_item)
        {
            AddItem(a_item, 1);
        }

        public void RemoveItem(ItemScriptable a_item, int a_amount)
        {
            AddItem(a_item, -a_amount);
        }

        public void AddItem(ItemScriptable a_item, int a_amount)
        {
            for(int i = 0; i < m_items.Count; i++)
            {
                InventoryItem inventoryItem = m_items[i];
                if(inventoryItem.Item == a_item)
                {
                    // Add 1
                    m_items[i] = InventoryItem.CreateWithDeltaAmount(inventoryItem, a_amount);
                    m_items[i].ItemElement.SetAmount(m_items[i].Count);
                    break;
                }
            }
        }

        public void SetSelected(int a_selectedIndex)
        {
            m_items[m_selectedIndex].ItemElement.SetIsSelected(false);
            m_items[a_selectedIndex].ItemElement.SetIsSelected(true);
            m_selectedIndex = a_selectedIndex;
        }

        public bool CanPlace()
        {
            return m_items[m_selectedIndex].Count > 0;
        }

        public int GetAmount(ItemScriptable a_item)
        {
            for(int i = 0 ; i < m_items.Count; i++)
            {
                if(m_items[i].Item == a_item)
                    return m_items[i].Count;
            }
            return 0;
        }

        public ItemScriptable GetHoldingItem()
        {
            return m_items[m_selectedIndex].Item;
        }

        public void DecreaseHoldingItem()
        {
            m_items[m_selectedIndex] = InventoryItem.CreateWithDeltaAmount(m_items[m_selectedIndex], -1);
            m_items[m_selectedIndex].ItemElement.SetAmount(m_items[m_selectedIndex].Count);
        }
    }
}