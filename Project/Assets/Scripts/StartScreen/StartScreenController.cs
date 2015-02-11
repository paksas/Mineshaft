using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent( typeof( Animator ) )]
public class StartScreenController : MonoBehaviour 
{
   private Animator        m_animator;
   private PLayGamesController m_playGamesController;   
 

	// Use this for initialization
	void Start () 
    {
	   m_animator = GetComponent< Animator >();
       m_playGamesController = GameObject.FindGameObjectWithTag(GameConsts.PLAY_GAMES_CONTROLLERY).GetComponent<PLayGamesController>();
       
	}

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {            
            Application.Quit();
            PlayerPrefs.SetInt(GameConsts.PLAY_GAMES_STATE_KEY, 0);
        }

        m_playGamesController.Activate();
        m_playGamesController.LogIn();
    }

}
