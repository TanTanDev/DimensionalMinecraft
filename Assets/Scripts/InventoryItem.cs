namespace Tantan
{
    public struct InventoryItem  
    {
        public InventoryItem(ItemScripable a_itemScriptable, ItemElement a_itemElement)
        {
            Item = a_itemScriptable;
            ItemElement = a_itemElement;
            Count = 0;
        }
        public ItemScripable Item; 
        public ItemElement ItemElement; 
        public int Count;
    }
}