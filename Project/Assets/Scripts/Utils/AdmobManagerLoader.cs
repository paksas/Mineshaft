using UnityEngine;
using System.Collections;

public class AdmobManagerLoader : MonoBehaviour
{
    public GameObject m_admobManager;

    // Use this for initialization
    void Awake()
    {
        if (AdmobManager.instance == null)
        {
            Instantiate(m_admobManager);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
