using UnityEngine;
using System.Collections;


public class ScrollController : MonoBehaviour
{
   [SerializeField] private Transform[]         m_prefabs = null;
   [SerializeField] private int                 m_spawnedSegmentsCount = 0;
   [SerializeField] private float               m_scrollSpeed = 1.0f;
   [SerializeField] private bool                m_useInversion = true;

 	private Transform[ ] 	                     m_segments;
   private float                                m_segmentSize = 5.0f;
   private float                                m_despawnHeigth = 0.0f;
   private int                                  m_lastSegIdx = 0;

   private GameController                       m_gameController;

   public void Start()
   {
      m_gameController = GameObject.FindGameObjectWithTag ( GameConsts.TAG_GAME_CONTROLLER ).GetComponent<GameController>();

      SpawnSegments();
   }

   public void Update()
   {
      if ( m_gameController.GameRunning )
      {
         Step();
      }
   }

   /**
    * Spawns the initial batch of segments
    */
   private void SpawnSegments()
   {
      if ( m_spawnedSegmentsCount <= 0 )
      {
         return;
      }

      // spawn the batch of segments
		m_segments = new Transform[ m_spawnedSegmentsCount ];

      // calculate the height which when reached by one segment will cause it's teleportation
      // to the bottom of the queue
      m_despawnHeigth = m_segmentSize * m_spawnedSegmentsCount * 0.5f;

      // we want the level to span around the origin in both directions evenly,
      // and that will be the upper boundary of the parallax we've just created
		Vector3 spawnPos = new Vector3( 0, m_despawnHeigth, transform.position.z );

		for( int i = 0; i < m_spawnedSegmentsCount; ++i )
		{
         // randomize the spawned prefab
         int prefabIdx = Mathf.Min( Random.Range( 0, m_prefabs.Length ), m_prefabs.Length - 1 );
         Transform segmentInstance = GameObject.Instantiate( m_prefabs[prefabIdx], spawnPos, Quaternion.identity ) as Transform;

         // every now and then flip the prefab vertically to create a more interesting pattern
         if ( m_useInversion && Random.value > 0.5f )
         {
            segmentInstance.transform.localScale = new Vector3( 1, -1, 1 );
         }
         
         // make the spawned segment a child of this object
			segmentInstance.parent = this.transform;

			m_segments[ i ] = segmentInstance;
			spawnPos.y -= m_segmentSize;
		}
   }

   /**
    * Scrolls the segments.
    */
   private void Step()
   {
      if ( m_spawnedSegmentsCount <= 0 )
      {
         return;
      }

      float yChange = GameController.Instance.ScrollSpeed * Time.deltaTime * m_scrollSpeed;
		for( int i = 0; i < m_spawnedSegmentsCount; ++i )
		{
			m_segments[ i ].Translate( 0, yChange, 0 );
		}

      // check whether the last segment reached the top of our scrolling area
      // and should be teleported to the bottom
      Transform lastSegment = m_segments[m_lastSegIdx];
		if( lastSegment.position.y > m_despawnHeigth )
		{
         int bottomSegIdx = m_lastSegIdx - 1;
         if ( bottomSegIdx < 0 )
         {
            bottomSegIdx = m_segments.Length - 1;
         }
         Transform bottomSegPos = m_segments[bottomSegIdx];

         lastSegment.position = new Vector3( 0, bottomSegPos.position.y - m_segmentSize, transform.position.z );

         // Now the next segment from the list becomes the last segment.
         // Keep in mind to loop around the end of the list ( thus the modulo operator % )
         m_lastSegIdx = (m_lastSegIdx + 1) % m_spawnedSegmentsCount;
		}
   }
}
