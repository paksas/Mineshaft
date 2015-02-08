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
      public float m_initialDistance = 12.0f;
      [SerializeField]
      public float m_decreaseBy = 0.2f;
      [SerializeField]
      public float m_cooldown = 5.0f;
      [SerializeField]
      public float m_minDistance = 4.0f;

   }

   [SerializeField]
   private float m_distanceRandomizationThreshold = 2.0f;
   [SerializeField]
   private float m_slideThreshold = 1.0f;
   [SerializeField]
   private float m_bodiesFadeoutTime = 2.0f;
   [SerializeField]
   private float m_doubleTapTimeLimit = 0.1f;
   [SerializeField]
   private DistanceBetweenObstacles m_distanceBetweenObstacles = new DistanceBetweenObstacles();

   private float m_currDistanceBetweenObstacles = 0.0f;


   private ObstaclesRegistry m_registry;
   private GameController m_gameController;

   private ArrayList m_spawnedObstacles = new ArrayList();
   private int m_incomingObstacleIdx;
   private int m_lastObstacleIdx;
   private bool m_userInputAccepted = false;
   private bool m_continuousObstacleShiftEnabled = false;
   private ObstaclesOccluder m_occluder;

   private ShawlController m_shawlController;

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

   public float ObstacleSpawnDistance
   {
      get { return m_currDistanceBetweenObstacles; }
   }

   public float InitialDistanceToFirstObstacle
   {
      get { return m_distanceBetweenObstacles.m_initialDistance; }
   }

   private int BottomObstacleIdx
   {
      get {
         int bottomObstacleIdx = m_lastObstacleIdx - 1;
         if (bottomObstacleIdx < 0)
         {
            bottomObstacleIdx = m_spawnedObstacles.Count - 1;
         }
         return bottomObstacleIdx;
      }
   }

   private Obstacle TopObstacle
   {
      get { return m_spawnedObstacles[m_lastObstacleIdx] as Obstacle; }
   }

   private Obstacle BottomObstacle
   {
      get { return m_spawnedObstacles[BottomObstacleIdx] as Obstacle; }
   }

   #endregion

   #region Unity Callbacks

   private bool m_mouseWasDown = false;


   void Start()
   {
      m_gameController = GameObject.FindGameObjectWithTag(GameConsts.TAG_GAME_CONTROLLER).GetComponent<GameController>();
      m_gameController.AddListener(this);

      MyPlayer player = GameObject.FindGameObjectWithTag(GameConsts.TAG_PLAYER).GetComponent<MyPlayer>();
      m_shawlController = player.GetComponentInChildren<ShawlController>();

      m_occluder = GetComponentInChildren<ObstaclesOccluder>();
   }

   // Use this for initialization
   void Awake()
   {
   }


   void OnMouseDown()
   {
      if (!m_userInputAccepted)
      {
         // user input is ignored
         return;
      }

      Obstacle incomingObstacles = m_spawnedObstacles[m_incomingObstacleIdx] as Obstacle;

      // start passing the events to the incoming obstacle as soon as it becomes visible
      // on screen, but not sooner
      if (!IsVisibleOnScreen(incomingObstacles))
      {
         return;
      }

      if (!m_mouseWasDown)
      {
         m_mouseWasDown = true;
         incomingObstacles.OnMouseDownObs();
      }
   }

   void OnMouseUp()
   {
      if (m_mouseWasDown)
      {
         m_mouseWasDown = false;

         Obstacle incomingObstacles = m_spawnedObstacles[m_incomingObstacleIdx] as Obstacle;
         incomingObstacles.OnMouseUpObs();
      }
   }

   void Update()
   {
      StepObstacles();

      if (m_continuousObstacleShiftEnabled)
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
      m_registry = GetComponent<ObstaclesRegistry>();
      m_continuousObstacleShiftEnabled = false;
      m_currDistanceBetweenObstacles = m_distanceBetweenObstacles.m_initialDistance;
      int numVisibleObstacles = CalcNumVisibleObstacles();

      SpawnObstacles(numVisibleObstacles, 0.0f);
      ShiftObstaclesDown();
      AcceptUserInput(true);
      StartCoroutine(Co_IncreaseSpeed());
   }

   public void OnGameFinished()
   {
      Obstacle incomingObstacle = m_spawnedObstacles[m_incomingObstacleIdx] as Obstacle;
      PlayerPrefs.SetInt("score", incomingObstacle.GetObstacleSignValue());
      AcceptUserInput(false);
   }

   public void OnGamePaused()
   {
      m_occluder.Activate(true);
      m_continuousObstacleShiftEnabled = true;
      AcceptUserInput(false);
   }

   public void OnGameResumed()
   {
      m_occluder.Activate(false);
      m_continuousObstacleShiftEnabled = false;
      AcceptUserInput(true);
   }

   private void AcceptUserInput(bool flag)
   {
      if (m_userInputAccepted == flag)
      {
         return;
      }
      m_userInputAccepted = flag;
      m_mouseWasDown = false;
   }

   #endregion

   #region Obstacle spawning and movement

   private void SpawnObstacles(int numObstaclesToSpawn, float lastObstacleOffset)
   {
      Vector3 spawnPos = new Vector3(0, lastObstacleOffset - ObstacleSpawnDistance - m_distanceRandomizationThreshold, 1);
      for (int i = 0; i < numObstaclesToSpawn; ++i)
      {
         Obstacle obstacle = m_registry.CreateRandomObstacle();
         obstacle.Controller = this;

         obstacle.transform.position = spawnPos;
         obstacle.transform.parent = this.transform;

         int insertionIndex = BottomObstacleIdx + 1;
         m_spawnedObstacles.Insert( insertionIndex, obstacle);

         spawnPos.y -= ObstacleSpawnDistance + Random.Range(0, m_distanceRandomizationThreshold);
      }

      // update tracked obstacles indices
      float topObstalceHeight = -1000.0f;
      float distToObstacleNearestThePlayer = 1000.0f;
      for (int i = 0; i < m_spawnedObstacles.Count; ++i)
      {
         Obstacle obstacle = m_spawnedObstacles[i] as Obstacle;
         float obstacleVertOffset = obstacle.transform.position.y;

         if ( obstacleVertOffset > topObstalceHeight )
         {
            topObstalceHeight = obstacleVertOffset;
            m_lastObstacleIdx = i;
         }

         if ( obstacleVertOffset < 0.0f && obstacleVertOffset > -distToObstacleNearestThePlayer && !obstacle.HasBeenPassed() )
         {
            distToObstacleNearestThePlayer = -obstacleVertOffset;
            m_incomingObstacleIdx = i;
         }
      }
   }

   private void StepObstacles()
   {
      float yChange = GameController.Instance.ScrollSpeed * Time.deltaTime;
      for (int i = 0; i < m_spawnedObstacles.Count; ++i)
      {
         Obstacle obstacle = m_spawnedObstacles[i] as Obstacle;
         obstacle.transform.Translate(0, yChange, 0);
      }

      // check whether the last segment reached the top of our scrolling area
      // and should be teleported to the bottom
      Obstacle lastSegment = TopObstacle;
      float lowerDespawnBoundary = 2.0f * Camera.main.orthographicSize;
      if (lastSegment.transform.position.y > lowerDespawnBoundary)
      {
         //Destroys passed obstacle behind the screen
         Destroy(lastSegment.gameObject);

         Obstacle bottomObstacle = BottomObstacle;
         Vector3 spawnPosition = bottomObstacle.transform.position;
         spawnPosition.y -= ObstacleSpawnDistance + Random.Range(0, m_distanceRandomizationThreshold);

         Obstacle newObstacle = m_registry.CreateRandomObstacle();
         newObstacle.Controller = this;
         newObstacle.transform.position = spawnPosition;
         m_spawnedObstacles[m_lastObstacleIdx] = newObstacle;

         // Now the next segment from the list becomes the last segment.
         // Keep in mind to loop around the end of the list ( thus the modulo operator % )
         m_lastObstacleIdx = (m_lastObstacleIdx + 1) % m_spawnedObstacles.Count;
      }

      // check if there's no need to spawn additional obstacles
      int numVisibleObstacles = CalcNumVisibleObstacles();
      if (numVisibleObstacles > m_spawnedObstacles.Count)
      {
         // find the obstacle at the very bottom
         float bottomObstacleOffset = BottomObstacle.transform.position.y;      
         SpawnObstacles(numVisibleObstacles - m_spawnedObstacles.Count, bottomObstacleOffset);
      }

   }

   private bool IsVisibleOnScreen(Obstacle obstacle)
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

      Obstacle incomingObstacle = m_spawnedObstacles[m_incomingObstacleIdx] as Obstacle;
      float distToPlayer = -incomingObstacle.transform.position.y;
      float shiftDistance = InitialDistanceToFirstObstacle - distToPlayer;
      foreach (Obstacle obstacle in m_spawnedObstacles)
      {
         if (!obstacle.HasBeenPassed())
         {
            Vector3 newPos = obstacle.transform.position;
            newPos.y -= shiftDistance;
            obstacle.transform.position = newPos;
         }
      }
   }

   private int CalcNumVisibleObstacles()
   {
      float cameraFrustumHeight = Camera.main.orthographicSize * 4.0f;
      int obstaclesCount = Mathf.Max( 1, Mathf.RoundToInt( cameraFrustumHeight/ ObstacleSpawnDistance ) );
      return obstaclesCount;
   }

   #endregion

   #region Obstacle callbacks

   /**
    * Called when the obstacle has been passed
    */
   public void OnObstaclePassed(Obstacle obstacle)
   {
      // the next obstacle in line becomes the active one
      m_incomingObstacleIdx = (m_incomingObstacleIdx + 1) % m_spawnedObstacles.Count;

      m_mouseWasDown = false;

      // show the progress
      m_shawlController.AddSegment();

      m_registry.OnObstaclePassed(obstacle);
   }

   /**
    * Called when the user fails to pass the obstacle.
    */
   public void OnObstacleFailed(Obstacle obstacle)
   {
      m_registry.OnObstacleFailed(obstacle);
   }

   #endregion

   #region Co-routines

   IEnumerator Co_IncreaseSpeed()
   {
      WaitForSeconds yieldDuration = new WaitForSeconds(m_distanceBetweenObstacles.m_cooldown);
      while (m_currDistanceBetweenObstacles > m_distanceBetweenObstacles.m_minDistance)
      {
         yield return yieldDuration;
         if (m_userInputAccepted)
         {
            m_currDistanceBetweenObstacles -= m_distanceBetweenObstacles.m_decreaseBy;
         }
      }
   }

   #endregion
}

