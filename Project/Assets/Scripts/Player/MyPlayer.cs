using UnityEngine;
using System.Collections;

public class MyPlayer : MonoBehaviour
{
   public enum State
   {
      Idle,
      Falling,
      Dead,

      Invalid
   };

   public ParticleSystem   m_bloodParticle;
   public string           m_soundOnFalling;
   public string           m_soundOnDeath;
   private SoundManager    m_soundManager;
   private AudioSource     m_audioSource;
   private Animator        m_animator;

   private State           m_currentState = State.Invalid;

   private ShawlController m_shawlController;


   // Use this for initialization
   void Start()
   {
      SetState( State.Invalid );
      m_soundManager = GameObject.FindGameObjectWithTag(GameConsts.TAG_SOUND_MANAGER).GetComponent<SoundManager>();
   }

   void Awake()
   {
      m_audioSource = GetComponent<AudioSource>();
      m_animator = GetComponentInChildren<Animator>();
      m_shawlController = GetComponentInChildren< ShawlController >();
   }


   public void StartScreaming()
   {
      SetState(State.Falling);
   }

   public void StopScreaming()
   {
      SetState(State.Idle);
   }

   void OnCollisionEnter2D(Collision2D _collision)
   {
      Debug.Log("Collision enter");
      if (_collision.rigidbody.GetComponent<Obstacle>() != null)
      {
          if ( IsAlive())
         SetState(State.Dead);
      }
   }

   void OnTriggerEnter2D(Collider2D _collision)
   {
      Debug.Log("Trigger enter");
      if (_collision.GetComponent<Obstacle>() != null)
      {
         if (IsAlive())
         SetState(State.Dead);
      }
   }

   public bool IsAlive()
   {
      return m_currentState == State.Falling;
   }

   private void SetState( State state )
   {
      if ( m_currentState == state )
      {
         // nothing changes
         return;
      }

      m_currentState = state;
      m_animator.SetInteger("StateIdx", (int)m_currentState);

      switch ( state )
      {
         case State.Idle:
            {
               OnIdle();
               break;
            }

         case State.Falling:
            {
               onFalling();
               break;
            }

         case State.Dead:
            {
               OnKill();
               break;
            }
      }
   }

   // -------------------------------------------------------------------------
   // State callbacks 
   // -------------------------------------------------------------------------
   private void OnIdle()
   {     
       m_soundManager.Stop(m_audioSource);      
   }

   private void onFalling()
   {
      m_shawlController.Activate( true );
      m_soundManager.Play(m_audioSource, ESoundType.SoundEffect, m_soundOnFalling);
   }

   private void OnKill()
   {
      // memorize the player's score
      PlayerPrefs.SetInt(GameConsts.PLAYER_SCORE_KEY, m_shawlController.SegmentsCount - 1);

      m_shawlController.Activate( false );
      m_soundManager.Stop(m_audioSource);
      m_audioSource.loop = false;
      m_soundManager.Play(m_audioSource, ESoundType.SoundEffect, m_soundOnDeath,true );

      GameController gameController = GameObject.FindGameObjectWithTag(GameConsts.TAG_GAME_CONTROLLER).GetComponent<GameController>();
      gameController.OnPlayerKilled();

      if (m_bloodParticle)
      {
         Instantiate(m_bloodParticle, transform.position + new Vector3(0, -1.0f, -0.5f), Quaternion.AngleAxis( -90.0f, new Vector3( 1.0f, 0.0f, 0.0f ) ) );
      }
   }
}
