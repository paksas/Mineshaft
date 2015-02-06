using UnityEngine;
using System.Collections;

[ ExecuteInEditMode ]
public class CameraAdjustor : MonoBehaviour 
{
	[ SerializeField ] private float	m_displayWidth = 5;

	void Update () 
	{
		Camera l_cam = Camera.main;
		l_cam.orthographicSize = ( m_displayWidth / l_cam.aspect ) / 2;
	}
}
