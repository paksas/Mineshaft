using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Animator ) ) ]
public class TooltipController : MonoBehaviour 
{
   [SerializeField]int     m_tooltipIdx = 0;

	// Use this for initialization
	void Start () 
   {
	   Animator animator = GetComponent<Animator>();
      animator.SetInteger( "tooltipIdx", m_tooltipIdx );
	}
	
}
