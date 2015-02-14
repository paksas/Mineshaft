using UnityEngine;
using System.Collections;

public class PlayGamesControllerLoader : MonoBehaviour
{
    public GameObject m_playGamesController;


    void Awake()
    {
        if (PLayGamesController.instance == null)
        {
            Instantiate(m_playGamesController);
        }
    }

    public void ShowLeaderboards()
    {
        PLayGamesController.instance.ShowLeaderboards();
    }

}
