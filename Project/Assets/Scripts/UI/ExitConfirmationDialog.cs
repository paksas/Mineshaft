using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) )]
public class ExitConfirmationDialog : MonoBehaviour 
{
   private Animator m_animator;

	// Use this for initialization
	void Start () 
   {
	   m_animator = GetComponent< Animator >();
	}
	
	// Update is called once per frame
	void Update () 
   {
	
	}

   public void ShowDialog()
   {
      if ( m_animator.GetBool( "Visible" ) == false )
      {
         m_animator.SetBool( "Visible", true );
         GameController.Instance.PauseGame();
      }
   }

   public void HideDialog()
   {
      m_animator.SetBool( "Visible", false );
      GameController.Instance.ResumeGame();
   }
}
