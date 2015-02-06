using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent( typeof( Animator ) )]
public class StartScreenController : MonoBehaviour 
{
   private Animator        m_animator;

	// Use this for initialization
	void Start () {
	   m_animator = GetComponent< Animator >();
	}

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {            
            Application.Quit();
        }
    }

}
