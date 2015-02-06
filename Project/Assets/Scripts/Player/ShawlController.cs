using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MeshFilter))]
public class ShawlController : MonoBehaviour 
{
   [SerializeField] private float               m_segmentLength = 0.05f;
   [SerializeField] private float               m_amplitude = 1.0f;
   [SerializeField] private bool                m_debugMode = true;

   private MeshFilter                           m_meshFilter;
   private Mesh                                 m_mesh;

   private Vector3[]                            m_vertices;
   private int[]                                m_faces;

   private int                                  m_segmentsCount = 0;
   private float                                m_wavePhase = 0.0f;

   private bool                                 m_isActive = false;

   private float                                m_debugCooldown = 1.0f;

   /**
    * Returns the number of segments in the shawl.
    */
   public int SegmentsCount
   {
      get { return m_segmentsCount; }
   }

	// Use this for initialization
	void Start () 
   {
	   m_meshFilter = GetComponent< MeshFilter >();
 
      m_mesh = m_meshFilter.sharedMesh;
      if (m_mesh == null)
      {
         m_meshFilter.mesh = new Mesh();
         m_mesh = m_meshFilter.sharedMesh;
      }     
 
      m_segmentsCount = 0;
      AddSegment();
	}
	
	// Update is called once per frame
	void Update () 
   {
      if ( !m_isActive )
      {
         return;
      }

      if ( m_debugMode )
      {
         m_debugCooldown -= Time.deltaTime;
         if ( m_debugCooldown <= 0.0f )
         {
            m_debugCooldown = 0.2f;
             AddSegment();
         }
      }

      // animate the mesh, shaping it to form a nice wave
      m_wavePhase += Time.deltaTime;
      CalculateSpline();

      UpdateMesh();
	}

   /**
    * Adds a segment to the shawl.
    */
   public void AddSegment()
   {
      if ( !m_isActive )
      {
         return;
      }
      m_segmentsCount++;

      m_mesh.Clear();
      m_vertices = new Vector3[m_segmentsCount * 2 + 2];
      m_faces = new int[m_segmentsCount * 6];

      CalculateSpline();

      // define the triangles
      int indexIdx = 0;
      for ( int segIdx = 0; segIdx < m_segmentsCount; ++segIdx )
      {
         int startVtx = segIdx * 2;
         m_faces[indexIdx++] = startVtx;
         m_faces[indexIdx++] = startVtx + 2;
         m_faces[indexIdx++] = startVtx + 1;
         m_faces[indexIdx++] = startVtx + 1;
         m_faces[indexIdx++] = startVtx + 2;
         m_faces[indexIdx++] = startVtx + 3;
      }
      
      UpdateMesh();
   }

   /**
    * Clears the mesh
    */
   public void Reset()
   {
      m_vertices = null;
      m_faces = null;
      m_segmentsCount = 0;

      m_mesh.Clear();
      UpdateMesh();
   }

   /**
    * Activates the mesh or deactivates it.
    * 
    * A deactivated mesh gets cleared automatically and doesn't react to any AddSegment calls
    */
   public void Activate( bool activate )
   {
      if ( m_isActive == activate )
      {
         return;
      }

      m_isActive = activate;
      if ( m_isActive )
      {
         AddSegment();
      }
      else
      {
         Reset();
      }
   }

   private void UpdateMesh()
   {
      m_mesh.vertices = m_vertices;
      m_mesh.triangles = m_faces;
      m_mesh.RecalculateBounds();
      m_mesh.Optimize();
   }

   private void CalculateSpline()
   {
      int vertexIdx = 0;
      float y = 0.0f;

      // the <= in the for loop condition below is not a coincidence:
      // we need to generate an additional pair of vertices ( the closing pair ), so the number of pairs == m_segmentsCount + 1. 
      // I could either compare against (m_segmentsCount + 1), or use <= ( not sure how this thing optimizes such ops, but I prefer playing it safe )
      for ( int segIdx = 0; segIdx <= m_segmentsCount; ++segIdx, y += m_segmentLength ) 
      {
         float distFactor = y / 6.0f;//(float)segIdx / (float)(m_segmentsCount + 1);
         float offset = Mathf.Sin( m_wavePhase + y ) * distFactor * m_amplitude;
         float halfWidth = Mathf.Abs( Mathf.Sin( m_wavePhase + y * 2.0f ) ) * 0.2f * distFactor + 0.05f;
         m_vertices[vertexIdx++] = new Vector3( offset - halfWidth, y, 0.0f );
         m_vertices[vertexIdx++] = new Vector3( offset + halfWidth, y, 0.0f );
      }
   }
}
