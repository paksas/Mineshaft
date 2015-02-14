using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent( typeof( Animator ) )]
public class StartScreenController : MonoBehaviour 
{
   public bool                 m_forceLogin;
   private Animator            m_animator;
 
	// Use this for initialization
	void Start () 
    {
	   m_animator = GetComponent< Animator >();

       if (m_forceLogin)
       {
           PLayGamesController.instance.LogIn();
       }  
       
	}

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            Application.Quit();
        }   
    }

}
