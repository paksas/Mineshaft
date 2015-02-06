using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TapObstacle : Obstacle
{
   [SerializeField] private Rigidbody2D[]    m_bodiesToDropOnTap = null;

   public string                             m_soundOnTapName;

   private int                               m_droppedBodyIdx;
   private float                             m_lastTapTime;


   protected override void OnStart()
   {
      m_droppedBodyIdx = 0;
      m_lastTapTime = -1.0f;
   }

   public override void OnMouseDownObs()
   {
      ActivateRigidBodyOnTap();
      PlayTapSound();

      // if there user double tapped the screen, deactivate all remaining bodies in one swoop
      bool doubleTapDetected = ( m_lastTapTime >= 0.0f ) && ( ( Time.time - m_lastTapTime ) <= Controller.DoubleTapTimeLimit );
      if ( doubleTapDetected )
      {
         ActivateRemainingBodies();
      }
      m_lastTapTime = Time.time;

      // if all bodies were dropped, score the obstacle as passed
      if ( m_droppedBodyIdx >= m_bodiesToDropOnTap.Length )
      {
         Pass();
      }
   }

   public override void OnMouseUpObs()
   {
      // nothing to do here
   }

   private void ActivateRigidBodyOnTap()
   {
      if ( m_droppedBodyIdx >= m_bodiesToDropOnTap.Length )
      {
         return;
      }

      Rigidbody2D body = m_bodiesToDropOnTap[m_droppedBodyIdx];
      m_droppedBodyIdx++;

      // turn the body into a dynamic one
      body.isKinematic = false;
         
      Vector2 forceVec = new Vector2( Random.Range( -0.2f, 0.2f ), -Random.Range( m_minForceToApply, m_maxForceToApply ) );
      body.AddForce(forceVec, ForceMode2D.Impulse);
   }

   private void ActivateRemainingBodies()
   {
      while ( m_droppedBodyIdx < m_bodiesToDropOnTap.Length )
      {
         ActivateRigidBodyOnTap();
      }
   }

   private void PlayTapSound()
   {
      if (m_soundManager != null && m_audioSource != null && m_soundOnPassedName != "")
      {
         m_soundManager.Play(m_audioSource, ESoundType.SoundEffect, m_soundOnTapName);
      }
   }
}
