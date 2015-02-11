using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class PLayGamesController : MonoBehaviour
{

    private bool m_isLoggingIn = false;
    private bool m_isPostingScore = false;
   


    private bool m_triedToLogIn = false;
    private bool m_loggedIn = false;
    private bool m_triedToPostScore = false;
    private bool m_scorePosted = false;
    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // authenticate user:
        if (m_isLoggingIn && !m_triedToLogIn)
        {
            m_triedToLogIn = true;
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    m_loggedIn = true;
                    m_isLoggingIn = false;
                }
                else
                {
                    m_isLoggingIn = false;
                }
            });
        }

        if (m_isPostingScore && !m_triedToPostScore)
        {
            if (!m_loggedIn)
            {
                LogIn();
            }
            else 
            {                
                m_triedToPostScore = true;
                Social.ReportScore(PlayerPrefs.GetInt(GameConsts.PLAYER_SCORE_KEY), "Cfji293fjsie_QA", (bool success) =>
                {
                    if (success)
                    {
                        m_scorePosted = true;
                        m_isPostingScore = false;
                    }
                    else
                    {
                        m_isPostingScore = false;
                    }
                });    
            }

        }
    }

    public void Activate()
    {
        // Activate the Google Play Games platform

        if (PlayerPrefs.GetInt(GameConsts.PLAY_GAMES_STATE_KEY) == 0)
        {
            PlayGamesPlatform.Activate();
            PlayerPrefs.SetInt(GameConsts.PLAY_GAMES_STATE_KEY, 1);
        }
    }

    public void LogIn()
    {
        m_isLoggingIn = true;
    }

    public void PostScore ()
    {
        m_isPostingScore = true;
    }

    public void ShowLeaderboards ()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI("Cfji293fjsie_QA"); 
    }

}
