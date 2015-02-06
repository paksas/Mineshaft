using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EasterEggText : MonoBehaviour
{
	public List	<string>	m_randomTextList = new List<string>() ;
	private GameController  m_gameController;
	private TextMesh	    m_textMesh;
	private bool			m_initalized;


		// Use this for initialization
	void Start ()
	{
		m_textMesh = GetComponent<TextMesh>();
		m_gameController = GameObject.FindGameObjectWithTag ( GameConsts.TAG_GAME_CONTROLLER ).GetComponent<GameController>();
		m_textMesh.renderer.enabled = false;
	}

	// Update is called once per frame
	void Update ()
	{
		if ( m_gameController.GameRunning && !m_initalized )
		{
			Hide ();
			m_initalized = true;
		}
	}

	public void Hide()
	{
		StopAllCoroutines();
		m_textMesh.renderer.enabled = false;
		StartCoroutine ( Co_Show() );
	}
	
	public void Show()
	{
		int rand = 0;

		StopAllCoroutines();
		rand = Random.Range( 0, m_randomTextList.Capacity - 1 );
		m_textMesh.renderer.enabled = true;
		m_textMesh.text = m_randomTextList[rand];
		StartCoroutine ( Co_Hide() );

	}

	IEnumerator Co_Show()
	{
		WaitForSeconds l_yield = new WaitForSeconds( 3 );
		while( true )
		{
			yield return l_yield;
			Show();
		}

	}

	IEnumerator Co_Hide()
	{
		WaitForSeconds l_yield = new WaitForSeconds( 2 );
		while( true )
		{
			yield return l_yield;
			Hide();
		}

	}
}

