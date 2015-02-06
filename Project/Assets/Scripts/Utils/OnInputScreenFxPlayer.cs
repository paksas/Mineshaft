using UnityEngine;
using System.Collections;

public class OnInputScreenFxPlayer : MonoBehaviour
{
   public ParticleSystem m_fx;
   public float m_fxUpdateTime = 0.2f;

   private ParticleSystem m_fxObject;

   // Use this for initialization
   void Start()
    {
         m_fxObject = Instantiate(m_fx, new Vector3(0,0,0), Quaternion.AngleAxis(90.0f, new Vector3(1.0f, 0.0f, 0.0f))) as ParticleSystem;
         m_fxObject.enableEmission = false;
    }

   // Update is called once per frame
   void Update()
   {
      Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      newPos.z = 0;
      m_fxObject.transform.localPosition = newPos;
   }

   void OnMouseUp()
   {
      m_fxObject.enableEmission = false;
   }

   void OnMouseDown()
   {
     m_fxObject.enableEmission = true;
   }

}
