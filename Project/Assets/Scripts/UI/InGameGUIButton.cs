using UnityEngine;
using System.Collections;
using System.Reflection;


[RequireComponent(typeof(BoxCollider2D))]
public class InGameGUIButton : MonoBehaviour
{
   public MonoBehaviour m_operatedObject;
   public string m_methodName;

   private SoundManager m_soundManager = null;
   private AudioSource m_audioSource = null;
   private InGameGUIController m_menuController = null;
   private bool m_isActive = false;

   // Use this for initialization
   void Start()
   {
      m_audioSource = GetComponentInParent<AudioSource>();

      GameObject soundManagerObj = GameObject.FindGameObjectWithTag(GameConsts.TAG_SOUND_MANAGER);
      if ( soundManagerObj != null )
      {
         m_soundManager = soundManagerObj.GetComponent<SoundManager>();
      }

      m_menuController = GetComponentInParent<InGameGUIController>();
      SetActive( m_menuController == null );
   }

   // Called in response to the button being released
   void OnMouseUp()
   {
      if (!m_isActive)
      {
         // inactive button - nothing to do here
         return;
      }

      if (m_operatedObject == null || m_methodName.Length <= 0)
      {
         // no object or method was assigned
         return;
      }

      MethodInfo method = m_operatedObject.GetType().GetMethod(m_methodName);
      if ( m_soundManager != null && m_audioSource != null )
      {
         m_soundManager.Play(m_audioSource, ESoundType.SoundEffect, "Button", true);
      }
      if (method != null)
      {
         method.Invoke(m_operatedObject, new object[] { });
      }

      // method's been invoked - automatically hide the menu, provided it's not a callback button,
      // in which case we need to wait a bit longer
      if ( m_menuController != null )
      {
         m_menuController.TransitionToNextState();
      }
   }

   /**
    * Activates/deactivates the button.
    * 
    * @param active
    */
   public void SetActive(bool active)
   {
      m_isActive = active;
   }

   public int GetMethodIdx(string[] methods)
   {
      int idx = 0;
      foreach (string methodName in methods)
      {
         if (methodName == m_methodName)
         {
            return idx;
         }
         ++idx;
      }

      return -1;
   }
}
