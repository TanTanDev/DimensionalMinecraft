using UnityEngine;
using UnityEngine.UI;

public class SkinSelect : MonoBehaviour
{
    [SerializeField] private GameObject m_holder;
    [SerializeField] private GetSkin m_getSkin;
    [SerializeField] private InputField m_inputField;
    private bool m_visible;

    private void Awake()
    {
        m_visible = false;
        m_holder.SetActive(m_visible);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F2))
        {
            m_visible = ! m_visible;
            m_holder.SetActive(m_visible);
        }
    }

    // button event set up from edit
    public void RefreshSkin()
    {
        m_getSkin.Set(m_inputField.text);
    }
}
