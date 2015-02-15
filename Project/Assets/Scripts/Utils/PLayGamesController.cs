using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class PLayGamesController : MonoBehaviour
{
    public static PLayGamesController instance = null;

    private bool m_isLoggingIn = false;
    private bool m_isPostingScore = false;
    private bool m_isShowingLeaderBoards = false;
    private static bool m_playGamesActive = false;
    public static bool m_forceLoginAttemptProcessed = false;
   


    private bool m_triedToLogIn = false;
    private bool m_loggedIn = false;
    private bool m_triedToPostScore = false;
    private bool m_scorePosted = false;
  
    

    // Use this for initialization
    void Start()
    {

    }

    void Awake()
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
        Activate();
    }


    // Update is called once per frame
    void Update()
    {
        if ( !Social.Active.localUser.authenticated)
        {
            m_loggedIn = false;
        }
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
                    m_triedToLogIn = false;
                }
                else
                {
                    m_isLoggingIn = false;
                    m_triedToLogIn = false;
                }
            });
        }

        if (m_isPostingScore && !m_triedToPostScore)
        {
                         
            m_triedToPostScore = true;
            Social.ReportScore(PlayerPrefs.GetInt(GameConsts.PLAYER_SCORE_KEY), "CgkIusvHhscWEAIQAA", (bool success) =>
            {
                if (success)
                {
                    m_scorePosted = true;
                    m_isPostingScore = false;
                    m_triedToPostScore = false;
                }
                else
                {
                    m_isPostingScore = false;
                    m_triedToPostScore = false;
                }
            });    
            

        }
        if (m_isShowingLeaderBoards )
        {         
            m_isShowingLeaderBoards = false;
            PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIusvHhscWEAIQAA");                                             
        }
    }

    public void Activate()
    {
        // Activate the Google Play Games platform

        if (!m_playGamesActive)
        {
            m_playGamesActive = true;
            PlayGamesPlatform.Activate();
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
        if (!m_loggedIn)
        {
            LogIn();

        }
        PostScore();
        m_isShowingLeaderBoards = true;
    }
    public bool IsLoggedIn()
    {
        return m_loggedIn;
    }

}
