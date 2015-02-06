using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class InGameGUIController : MonoBehaviour, IGameControllerListener
{
   private enum State
   {
      Inactive,
      SlidingOut,
      SlidingIn,
      Active,
   }

   private InGameGUIButton[]  m_buttons;
   private Animator           m_animationController;
   private SoundManager       m_soundManager;
   private AudioSource        m_audioSource;
   private State              m_currentState = State.Inactive;
   private bool               m_userInputAccepted = false;


   // Use this for initialization
   void Start()
   {
      m_audioSource = GetComponent<AudioSource>();
      m_soundManager = GameObject.FindGameObjectWithTag(GameConsts.TAG_SOUND_MANAGER).GetComponent<SoundManager>();
      m_buttons = GetComponentsInChildren<InGameGUIButton>();
      m_animationController = GetComponent<Animator>();

      GameController.Instance.AddListener( this );
   }

   // Called in response to the button being released
   void OnMouseUp()
   {
      if ( !m_userInputAccepted )
      {
         return;
      }

      TransitionToNextState();
      m_soundManager.Play(m_audioSource, ESoundType.SoundEffect, "Button", true);
   }

   /**
    * Transitions the menu to the next state.
    */
   public void TransitionToNextState()
   {
      State[] subsequentState = new State[] { State.SlidingOut, State.SlidingIn, State.SlidingOut, State.SlidingIn };
      m_currentState = subsequentState[(int)m_currentState];

      m_animationController.SetInteger("StateIdx", (int)m_currentState);

      // slow the time as the menu is sliding out
      if (m_currentState == State.SlidingOut)
      {
         GameController.Instance.PauseGame();
      }
   }

   // An animation callback invoked when the slide animation ends.
   void OnSlideComplete()
   {
      switch (m_currentState)
      {
         case State.SlidingOut:
            {
               m_currentState = State.Active;
               break;
            }

         case State.SlidingIn:
            {
               // bring back the regular time scale as the menu has fully slid in
               GameController.Instance.ResumeGame();
               m_currentState = State.Inactive;
               break;
            }
      }

      foreach (InGameGUIButton button in m_buttons)
      {
         button.SetActive(m_currentState == State.Active);
      }
   }

   #region IGameControllerListener

   public void OnGameStarted()
   {
      m_userInputAccepted = true;
   }


   public void OnGameFinished()
   {
      m_userInputAccepted = false;
   }

   public void OnGamePaused() 
   {
   }

   public void OnGameResumed() {}

   #endregion
}
