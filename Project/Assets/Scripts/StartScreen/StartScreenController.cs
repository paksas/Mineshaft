using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

[RequireComponent( typeof( Animator ) )]
public class StartScreenController : MonoBehaviour 
{
   private Animator        m_animator;
   private bool            triedToLogIn = false;
   private  bool           loggedIn     = false;

	// Use this for initialization
	void Start () {
	   m_animator = GetComponent< Animator >();
       // Activate the Google Play Games platform
        
        if ( PlayerPrefs.GetInt ( GameConsts.PLAY_GAMES_STATE_KEY) == 0 )
        {
            PlayGamesPlatform.Activate();
            PlayerPrefs.SetInt(GameConsts.PLAY_GAMES_STATE_KEY, 1);
        }
       

	}

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {            
            Application.Quit();
            PlayerPrefs.SetInt(GameConsts.PLAY_GAMES_STATE_KEY, 0);
        }

        // authenticate user:
        if (!triedToLogIn)
        {
            triedToLogIn = true;
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    loggedIn = true;
                }
                else
                {
                    triedToLogIn = false;
                }
            });
        }

    }

}
