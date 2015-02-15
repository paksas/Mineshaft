using UnityEngine;
using System.Collections;

public class PlayGamesLoginIndicator : MonoBehaviour
{
    private SpriteRenderer m_indicator;

    // Use this for initialization
    void Awake()
    {
        m_indicator = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_indicator.enabled = PLayGamesController.instance.IsLoggedIn();
    }
}
