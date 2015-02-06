using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IGameControllerListener
{
   /**
    * Called when the game starts
    */
   void OnGameStarted();

   /**
    * Called when the game ends
    */
   void OnGameFinished();

   /**
    * Called when the game gets paused
    */
   void OnGamePaused();

   /**
    * Called when the game is resumed.
    */
   void OnGameResumed();
}

public class GameController : MonoBehaviour
{
   #region Member fields

   public static GameController              Instance;

   private bool                              m_gameRunning;
   private Player                            m_player;

   [SerializeField]
   private float                             m_scrollSpeed = 3.0f;
   private int                               m_gamePaused = 0;

   [SerializeField]
   private LevelLoader                       m_gameOverScreenLoader = null;
   [SerializeField]
   private float                             m_levelLoadDelay = 1.0f;
   private float                             m_levelLoadTimer = -1.0f;

   private List<IGameControllerListener>     m_listeners = new List<IGameControllerListener>();

   #endregion

   #region Properties

   public bool GameRunning
   {
      get { return m_gameRunning; }
   }

   public float ScrollSpeed
   {
      get { return m_scrollSpeed; }
   }

   #endregion

   #region Unity callbacks

   // Use this for initialization
   void Awake()
   {
      Instance = this;
   }
  
   void Start()
   {
      m_player = GameObject.FindGameObjectWithTag(GameConsts.TAG_PLAYER).GetComponent<Player>();

      // the game starts automatically
      StartGame();
   }

   // Update is called once per frame
   void Update()
   {
      if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
      {
         if (GameRunning)
         {
            Application.LoadLevel(0);
         }
         else
         {
            Application.Quit();
         }
      }

      if ( m_levelLoadTimer >= 0.0f && m_gameOverScreenLoader != null )
      {
         m_levelLoadTimer -= Time.deltaTime;
         if ( m_levelLoadTimer <= 0.0f )
         {
            m_gameOverScreenLoader.LoadLevel();
         }
      }
   }

   #endregion

   #region Game state management

   public void StartGame()
   {
      m_gameRunning = true;
      m_gamePaused = 0;
      NotifyGameStarted();
      m_player.StartScreaming();
   }

   public void PauseGame()
   {
      m_gamePaused++;
      if ( m_gamePaused == 1 )
      {
         m_player.StopScreaming();
         NotifyGamePaused();
      }
   }

   public void ResumeGame()
   {
      m_gamePaused = Mathf.Max( 0, m_gamePaused - 1 );
      if ( m_gamePaused <= 0 )
      {
         m_player.StartScreaming();
         NotifyGameResumed();
      }
   }

   /**
    * A notification called by the Player class when the player crashes into an obstacle and dies
    */
   public void OnPlayerKilled()
   {
      NotifyGameFinished();

      m_gameRunning = false;
      
      // kickoff the level loading countdown timer
      m_levelLoadTimer = m_levelLoadDelay;
   }

   #endregion

   #region Listeners management

   public void AddListener( IGameControllerListener listener )
   {
      if ( m_listeners.Contains( listener ) )
      {
         return;
      }

      m_listeners.Add( listener );

      // send out the existing notifications
      if ( m_gameRunning )
      {
         listener.OnGameStarted();
      }
      if ( m_gamePaused > 0 )
      {
         listener.OnGamePaused();
      }
   }

   private void NotifyGameStarted()
   {
      foreach ( IGameControllerListener listener in m_listeners )
      {
         listener.OnGameStarted();
      }
   }

   private void NotifyGameFinished()
   {
      foreach ( IGameControllerListener listener in m_listeners )
      {
         listener.OnGameFinished();
      }
   }

   private void NotifyGamePaused()
   {
      foreach ( IGameControllerListener listener in m_listeners )
      {
         listener.OnGamePaused();
      }
   }

   private void NotifyGameResumed()
   {
      foreach ( IGameControllerListener listener in m_listeners )
      {
         listener.OnGameResumed();
      }
   }

   #endregion
}
