using UnityEngine;
using System.Collections;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager instance = null;
    public AdMobPlugin.AdSize m_addSize;
    public string m_addID;
    public string m_interstitialID;
    public bool m_isTopPosition = true;
    public bool m_testMode = true;
    private AdMobPlugin m_admob;
    private bool m_bannerLoaded;
    private bool m_bannerVisible;


    void Awake  ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        m_admob = GetComponent<AdMobPlugin>();
        LoadBanner();
    }
    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadBanner()
    {
        if (!m_bannerLoaded)
        {
            m_bannerLoaded = true;
            m_admob.CreateBanner(m_addID, m_addSize, m_isTopPosition, m_interstitialID, m_testMode);
            m_admob.RequestAd();
        }
       
    }

    public void ShowBanner()
    {
        if (!m_bannerVisible)
        {
            m_admob.ShowBanner();
            m_bannerVisible = true;
            StopCoroutine(Co_ShowBanner());
        }        
    }

    public void HideBanner ()
    {
        if (m_bannerVisible)
        {
            m_admob.HideBanner();
            m_bannerVisible = false;
            StopCoroutine(Co_ShowBanner());
        }    
    }

    public void ShowBannerWithDelay()
    {
        StartCoroutine(Co_ShowBanner());
    }
    IEnumerator Co_ShowBanner()
    {
         yield return new WaitForSeconds(1.5f);
         ShowBanner();

    }


}
