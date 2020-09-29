namespace Tantan
{
    public struct InventoryItem  
    {
        public InventoryItem(ItemScriptable a_itemScriptable, ItemElement a_itemElement)
        {
            Item = a_itemScriptable;
            ItemElement = a_itemElement;
            Count = 0;
        }

        public static InventoryItem CreateWithDeltaAmount(InventoryItem a_inventoryItem, int a_amount)
        {
            InventoryItem inventoryItem = new InventoryItem(a_inventoryItem.Item, a_inventoryItem.ItemElement);
            inventoryItem.Count = a_inventoryItem.Count;
            inventoryItem.Count += a_amount;
            return inventoryItem;
        }
        public ItemScriptable Item; 
        public ItemElement ItemElement; 
        public int Count;
    }
}