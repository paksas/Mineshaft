using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ObstacleController : MonoBehaviour, IGameControllerListener
{
   #region Member fields

   
   [System.Serializable]
   public class DistanceBetweenObstacles
   {
      [SerializeField]
      public float                                       m_initialDistance = 12.0f;
      [SerializeField]
      public float                                       m_decreaseBy = 0.2f;
      [SerializeField]
      public float                                       m_cooldown = 5.0f;
      [SerializeField]
      public float                                       m_minDistance = 4.0f;

   }

   private const int                                     SPAWNED_OBSTACLES_LIMIT = 8;

   [SerializeField] private float                        m_distanceRandomizationThreshold = 2.0f;
   [SerializeField] private float                        m_slideThreshold = 1.0f;
   [SerializeField] private float                        m_bodiesFadeoutTime = 2.0f;
   [SerializeField] private float                        m_doubleTapTimeLimit = 0.1f;
   [SerializeField] private DistanceBetweenObstacles     m_distanceBetweenObstacles = new DistanceBetweenObstacles();
   
   private float                                         m_currDistanceBetweenObstacles = 0.0f;


   private ObstaclesRegistry                             m_registry;
   private GameController                                m_gameController;

   private Obstacle[]                                    m_spawnedObstacles;
   private int                                           m_incomingObstacleIdx;
   private int                                           m_lastObstacleIdx;
   private bool                                          m_userInputAccepted = false;
   private ObstaclesOccluder                             m_occluder;

   private ShawlController                               m_shawlController;
   public bool                                           m_tutorialEnabled = true;
   private const string                                  TUTORIAL_STATE_KEY = "TutorialState";
   private const string                                  TUTORIAL_STATE_INITIALIZED_KEY = "TutorialStateInitialized";

   #endregion

   #region Properties

   /**
    * Tells how long should the finger slide be to score a slide obstacle.
    */
   public float SlideThreshold
   {
      get { return m_slideThreshold; }
   }

   /**
    * Tells how long it takes for an activated body to fade out.
    */
   public float BodiesFadeoutTime
   {
      get { return m_bodiesFadeoutTime; }
   }

   /**
    * The maximum time elapsed between two consecutive taps when they can be considered as a double tap
    */
   public float DoubleTapTimeLimit
   {
      get { return m_doubleTapTimeLimit; }
   }

   /**
    * Is the tutorial enabled.
    */
   public bool TutorialEnabled
   {
      get { return m_tutorialEnabled; }
   }

   /**
    * Can the tutorial be displayed
    */
   public bool CanDisplayTutorial
   {
      get { return m_tutorialEnabled && ( m_gameController == null || m_gameController.GameRunning ); }
   }

   public float ObstacleSpawnDistance
   {
      get { return m_currDistanceBetweenObstacles; }
   }

   public float InitialDistanceToFirstObstacle
   {
      get { return m_distanceBetweenObstacles.m_initialDistance; }
   }

   #endregion

   #region Unity Callbacks

   private bool m_mouseWasDown = false;


   void Start()
   {
      m_gameController = GameObject.FindGameObjectWithTag(GameConsts.TAG_GAME_CONTROLLER).GetComponent<GameController>();
      m_gameController.AddListener( this );

      Player player = GameObject.FindGameObjectWithTag(GameConsts.TAG_PLAYER).GetComponent<Player>();
      m_shawlController = player.GetComponentInChildren<ShawlController>();

      m_occluder = GetComponentInChildren<ObstaclesOccluder>();
   }

   // Use this for initialization
   void Awake()
   {
       if (IsTutorialStateInitialized())
       {
           m_tutorialEnabled = GetTutorialState();
       }
       else
       {
           InitTutorialState();
       }
   }


   void OnMouseDown()
   {
      if ( !m_userInputAccepted )
      {
         // user input is ignored
         return;
      }

      // start passing the events to the incoming obstacle as soon as it becomes visible
      // on screen, but not sooner
      if ( !IsVisibleOnScreen( m_spawnedObstacles[m_incomingObstacleIdx] ) )
      {
         return;
      }

      if ( !m_mouseWasDown )
      {
         m_mouseWasDown = true;
         m_spawnedObstacles[m_incomingObstacleIdx].OnMouseDownObs();
      }
   }

   void OnMouseUp()
   {
      if ( m_mouseWasDown )
      {
         m_mouseWasDown = false;
         m_spawnedObstacles[m_incomingObstacleIdx].OnMouseUpObs();
      }
   }

   void Update()
   {
      StepObstacles();

      if ( !m_userInputAccepted )
      {
         // if the user doesn't have control of the input, we need to give him room to breathe
         // until he regains that control - so we're gonna be keeping all obstacles at
         // a distance from him until then
         ShiftObstaclesDown();
      }
   }

   #endregion

   #region IGameControllerListener implementation

   public void OnGameStarted()
   {
      SpawnObstacles();
      ShiftObstaclesDown();
      ShowTutorial( m_tutorialEnabled );
      AcceptUserInput( true );
      StartCoroutine(Co_IncreaseSpeed());
   }

   public void OnGameFinished()
   {
       PlayerPrefs.SetInt("score", m_spawnedObstacles[m_incomingObstacleIdx].GetObstacleSignValue());
      ShowTutorial( false );
      AcceptUserInput( false );
   }

   public void OnGamePaused()
   {
      m_occluder.Activate( true );
      ShiftObstaclesDown();
      AcceptUserInput( false );
   }

   public void OnGameResumed()
   {
      m_occluder.Activate( false );
      AcceptUserInput( true );
   }

   private void AcceptUserInput( bool flag )
   {
      if ( m_userInputAccepted == flag )
      {
         return;
      }
      m_userInputAccepted = flag;
      m_mouseWasDown = false;
   }

   #endregion

   #region Tutorial

   public void ToggleTutorial()
   {
      m_tutorialEnabled = !m_tutorialEnabled;
      ShowTutorial( m_tutorialEnabled );
      SaveTutorialState();

   }

   private void ShowTutorial( bool show )
   {
      foreach ( Obstacle obstacle in m_spawnedObstacles )
      {
         obstacle.ShowTutorial( show );
      }
   }

      void InitTutorialState()
   {
       SaveTutorialState();
       PlayerPrefs.SetInt(TUTORIAL_STATE_INITIALIZED_KEY, 1);
   }

   public void SaveTutorialState()
   {
       if (m_tutorialEnabled)
       {
           PlayerPrefs.SetInt(TUTORIAL_STATE_KEY, 1);
       }
       else
       {
           PlayerPrefs.SetInt(TUTORIAL_STATE_KEY, 0);
       }
   }

   private bool IsTutorialStateInitialized()
   {
       return PlayerPrefs.GetInt(TUTORIAL_STATE_INITIALIZED_KEY) == 1;
   }

   public bool GetTutorialState()
   {
       return PlayerPrefs.GetInt(TUTORIAL_STATE_KEY) == 1;
   }

   #endregion

   #region Obstacle spawning and movement

   private void SpawnObstacles()
   {
      m_registry = GetComponent<ObstaclesRegistry>();
      m_spawnedObstacles = new Obstacle[SPAWNED_OBSTACLES_LIMIT];

      m_currDistanceBetweenObstacles = m_distanceBetweenObstacles.m_initialDistance;
      Vector3 spawnPos = new Vector3(0, -ObstacleSpawnDistance - m_distanceRandomizationThreshold, 1);

      for (int i = 0; i < SPAWNED_OBSTACLES_LIMIT; ++i)
      {
         Obstacle obstacle = m_registry.CreateRandomObstacle();
         obstacle.Controller = this;

         obstacle.transform.position = spawnPos;
         obstacle.transform.parent = this.transform;
         m_spawnedObstacles[i] = obstacle;

         spawnPos.y -= ObstacleSpawnDistance + Random.Range(0, m_distanceRandomizationThreshold);
      }

      m_lastObstacleIdx = 0;
      m_incomingObstacleIdx = 0;
   }

   private void StepObstacles()
   {
      float yChange = GameController.Instance.ScrollSpeed * Time.deltaTime;
		for( int i = 0; i < SPAWNED_OBSTACLES_LIMIT; ++i )
		{
			m_spawnedObstacles[ i ].transform.Translate( 0, yChange, 0 );
		}

      // check whether the last segment reached the top of our scrolling area
      // and should be teleported to the bottom
      Obstacle lastSegment = m_spawnedObstacles[ m_lastObstacleIdx ];
      float lowerDespawnBoundary = 2.0f * Camera.main.orthographicSize;
		if( lastSegment.transform.position.y > lowerDespawnBoundary  )
		{
         //Destroys passed obstacle behind the screen
         Destroy(lastSegment.gameObject);

         // replace the old obstacle with a new, random one
         int prevObstacleIdx = m_lastObstacleIdx - 1;
         if ( prevObstacleIdx < 0 )
         {
            prevObstacleIdx = SPAWNED_OBSTACLES_LIMIT - 1;
         }

         Vector3 spawnPosition = m_spawnedObstacles[ prevObstacleIdx ].transform.position;
         spawnPosition.y -= ObstacleSpawnDistance + Random.Range(0, m_distanceRandomizationThreshold);

         Obstacle newObstacle = m_registry.CreateRandomObstacle();
         newObstacle.Controller = this;
         newObstacle.transform.position = spawnPosition;
         m_spawnedObstacles[m_lastObstacleIdx] = newObstacle;

         // Now the next segment from the list becomes the last segment.
         // Keep in mind to loop around the end of the list ( thus the modulo operator % )
         m_lastObstacleIdx = (m_lastObstacleIdx + 1) % SPAWNED_OBSTACLES_LIMIT;
		}
   }

   private bool IsVisibleOnScreen( Obstacle obstacle )
   {
       return obstacle.renderer.isVisible;
   }

   /**
    * Call this method if you want to shift all active obstacles down to give the player
    * some room to breathe.
    */
   private void ShiftObstaclesDown()
   {
      // Calculate the shift offset.
      // We want to give the user at least 10 m of space, and that's why we're gonna
      // grab the position of the next active obstacle, see how far it is away from
      // the player ( which is essentially located at the origin ) now, and that way
      // we'll learn how far do we need to move the obstacle so that it's 10 m away.

      float distToPlayer = -m_spawnedObstacles[m_incomingObstacleIdx].transform.position.y;
      float shiftDistance = InitialDistanceToFirstObstacle - distToPlayer;
      foreach ( Obstacle obstacle in m_spawnedObstacles )
      {
         if ( !obstacle.HasBeenPassed() )
         {
            Vector3 newPos = obstacle.transform.position;
            newPos.y -= shiftDistance;
            obstacle.transform.position = newPos;
         }
      }
   }

   #endregion

   #region Obstacle callbacks

   /**
    * Called when the obstacle has been passed
    */
   public void OnObstaclePassed( Obstacle obstacle )
   {
      // the next obstacle in line becomes the active one
      m_incomingObstacleIdx = ( m_incomingObstacleIdx + 1) % SPAWNED_OBSTACLES_LIMIT;

      m_mouseWasDown = false;

      // show the progress
      m_shawlController.AddSegment();

      m_registry.OnObstaclePassed( obstacle );
   }

   /**
    * Called when the user fails to pass the obstacle.
    */
   public void OnObstacleFailed( Obstacle obstacle )
   {
      m_registry.OnObstacleFailed( obstacle );
   }

   #endregion
   
   #region Co-routines

   IEnumerator Co_IncreaseSpeed()
   {
      WaitForSeconds yieldDuration = new WaitForSeconds(m_distanceBetweenObstacles.m_cooldown);
      while ( m_currDistanceBetweenObstacles > m_distanceBetweenObstacles.m_minDistance )
      {
         yield return yieldDuration;
         if ( m_userInputAccepted )
         {
            m_currDistanceBetweenObstacles -= m_distanceBetweenObstacles.m_decreaseBy;
         }
      }
   }

   #endregion
}

