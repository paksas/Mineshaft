using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) )]
public class ObstaclesOccluder : MonoBehaviour 
{
   private Animator        m_animator;

	// Use this for initialization
	void Start () 
   {
	   m_animator = GetComponent< Animator >();
	}
	
   /**
    * Activates/deactivates the effect
    */
   public void Activate( bool activate )
   {
      m_animator.SetBool( "Active", activate );
   }
}
