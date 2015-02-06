
using UnityEngine;
using rzymskie_cs;


public class ObstaclesRegistry : MonoBehaviour
{
   [SerializeField]
   private int             m_numObstaclesPerTutorialStage = 4;
   [SerializeField]
   private Obstacle[]      m_obstacles = null;
   private int             m_currentObstacleSignNumber;
   private int             m_score;

   private int             m_numDifferentObstaclesSpawned = 1;
   private int             m_leftToScoreToProceed;

   private string          TUTORIAL_STATE_KEY = "TUTORIAL_STAGE_KEY";
   public bool             m_useCentralCounter;

   void Start()
   {
      m_numDifferentObstaclesSpawned = Mathf.Max( 1, PlayerPrefs.GetInt(TUTORIAL_STATE_KEY) );
      PlayerPrefs.SetInt(TUTORIAL_STATE_KEY, m_numDifferentObstaclesSpawned);
      m_leftToScoreToProceed = m_numObstaclesPerTutorialStage;
   }

   public Obstacle CreateRandomObstacle()
   {
      int upperBoundary = m_obstacles.Length;
      int lowerBoundary = Mathf.Max( 0, upperBoundary - m_numDifferentObstaclesSpawned );
      int obstacleTypeIndex = Random.Range(lowerBoundary, upperBoundary);

      Obstacle obstacleInstance = Instantiate(m_obstacles[obstacleTypeIndex]) as Obstacle;
      obstacleInstance.m_obstacleTypeIndex = obstacleTypeIndex;

      m_currentObstacleSignNumber++;
      obstacleInstance.SetObstacleSign(Rzymskie.arabic2roman(m_currentObstacleSignNumber), m_useCentralCounter);

      m_score = PlayerPrefs.GetInt("score");

      if (m_score > 0 && m_score == m_currentObstacleSignNumber)
      {
         obstacleInstance.ShowDeathMarker();
      }
      return obstacleInstance;
   }

   public void OnObstaclePassed(Obstacle obstacle)
   {
      if ( m_numDifferentObstaclesSpawned >= m_obstacles.Length )
      {
         return;
      }

      if ( obstacle.m_obstacleTypeIndex == m_obstacles.Length - m_numDifferentObstaclesSpawned )
      {
         m_leftToScoreToProceed--;
         if ( m_leftToScoreToProceed <= 0 )
         {
            m_leftToScoreToProceed = m_numObstaclesPerTutorialStage;
            ++m_numDifferentObstaclesSpawned;

            PlayerPrefs.SetInt(TUTORIAL_STATE_KEY, m_numDifferentObstaclesSpawned);
         }
      }
   }

   public void OnObstacleFailed(Obstacle obstacle)
   {
      m_leftToScoreToProceed = 0;
   }


}
