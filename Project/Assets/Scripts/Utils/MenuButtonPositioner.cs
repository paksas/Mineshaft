using UnityEngine;
using System.Collections;

public class MenuButtonPositioner : MonoBehaviour 
{
   public Vector2         m_offset = new Vector2( 0.0f, 0.0f );

	void Update()
   {
      // keep the toolbar in the upper left corner of the screen
      Camera cam = Camera.main;
      Vector3 newPos = new Vector3(-cam.orthographicSize * cam.aspect + m_offset.x, Camera.main.orthographicSize - m_offset.y, transform.localPosition.z);
      transform.localPosition = newPos;
   }
}
