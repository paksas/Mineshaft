using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) )]
public class LevelLoader : MonoBehaviour 
{
   public string        m_levelName;

   private Animator     m_animator;

   void Start()
   {
      m_animator = GetComponent<Animator>();
      m_animator.SetBool( "Visible", false );
   }

	public void LoadLevel()
   {
      m_animator.SetBool( "Visible", true );
   }

   public void OnScreenFadedIn()
   {
   }

   public void OnScreenFadedOut()
   {
      if ( m_levelName.Length > 0 )
      {
         Application.LoadLevel(m_levelName);
      }
   }

}
