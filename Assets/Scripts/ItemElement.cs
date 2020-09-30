using UnityEngine;
using UnityEngine.UI;

public class ItemElement : MonoBehaviour
{
    public Button Button; 
    [SerializeField] private Text m_text; 
    [SerializeField] private RawImage m_image; 
    [SerializeField] private GameObject m_selectedImage; 

    private void Awake()
    {
        SetIsSelected(false);
    }

    public void SetTexture(Texture a_texture)
    {
        m_image.texture = a_texture;
    }

    public void SetAmount(int a_amount)
    {
        m_text.text = a_amount.ToString();
    }

    public void SetIsSelected(bool a_isSelected)
    {
        m_selectedImage.SetActive(a_isSelected);
    }
}
