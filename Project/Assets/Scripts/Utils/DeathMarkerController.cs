using UnityEngine;
using System.Collections;

public class DeathMarkerController : MonoBehaviour
{

    private SpriteRenderer m_skullRight;
    private SpriteRenderer m_skullLeft;

    // Use this for initialization
    void Start()
    {

    }

    void Awake ()
    {
        m_skullRight = transform.FindChild("skull_2").GetComponentInChildren<SpriteRenderer>();
        m_skullLeft = transform.FindChild("skull_1").GetComponentInChildren<SpriteRenderer>();
        HideMark();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideMark ()
    {
        if (m_skullRight != null)
        m_skullRight.renderer.enabled = false;

        if (m_skullLeft != null)
        m_skullLeft.renderer.enabled = false;
    }

    public void ShowMark()
    {
        if (m_skullRight != null)
        m_skullRight.renderer.enabled = true;

        if (m_skullLeft != null)
        m_skullLeft.renderer.enabled = true;
    }
}
