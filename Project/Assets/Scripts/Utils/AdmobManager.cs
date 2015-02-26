using UnityEngine;
using System.Collections;

public class AdmobManager : MonoBehaviour
{
    private AdMobPlugin m_admob;


    void Awake  ()
    {
        m_admob = GetComponent<AdMobPlugin>();
    }
    // Use this for initialization
    void Start()
    {
        m_admob.CreateBanner("", AdMobPlugin.AdSize.BANNER, true, "", true);
        m_admob.RequestAd();
        m_admob.ShowBanner();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
