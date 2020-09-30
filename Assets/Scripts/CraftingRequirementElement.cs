using UnityEngine;
using UnityEngine.UI;

public class CraftingRequirementElement : MonoBehaviour
{
    [SerializeField] private Text AmountText;
    [SerializeField] private Text NameText;
    [SerializeField] private RawImage IconRawImage;

    public void Setup(ItemAndAmount a_itemAndAmount)
    {
        AmountText.text = a_itemAndAmount.Amount.ToString();
        NameText.text = a_itemAndAmount.Item.Name;
        IconRawImage.texture = a_itemAndAmount.Item.m_imageIcon;
    }
}
