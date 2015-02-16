using UnityEngine;
using System.Collections;

public enum EObstacleSlideDirection
{
	Left,
	Right,
	Down
}

public class SlideObstacles : Obstacle
{
   private const float                                ANGLE_TOLERANCE = 45.0f;

	[SerializeField] private EObstacleSlideDirection   m_slideDirection = EObstacleSlideDirection.Left;
   [SerializeField] private Rigidbody2D[]             m_bodiesToLeaveInactive = null;

	private Vector3                                    m_mouseDownPos;

   protected override void OnStart()
   {
   }

	public override void OnMouseDownObs()
	{
		m_mouseDownPos = Input.mousePosition;
	}

	public override void OnMouseUpObs()
	{
		Vector3 mouseUpPos = Input.mousePosition;
		Vector3 mouseDelta = mouseUpPos - m_mouseDownPos;

      if ( mouseDelta.magnitude < 50.0f )
      {
         // the move was too short
         return;
      }

      Vector3 dir = mouseDelta.normalized;
      float angle = Mathf.Acos( Vector3.Dot( dir, new Vector3( 1, 0, 0 ) ) ) * Mathf.Rad2Deg;

      bool passed = false;
		switch( m_slideDirection )
		{
		case EObstacleSlideDirection.Down: 
         {
            passed = mouseDelta.y < Controller.SlideThreshold && Mathf.Abs( angle - 90.0f ) <= ANGLE_TOLERANCE;
			   break; 
         }

		case EObstacleSlideDirection.Left: 
         {
            passed = mouseDelta.x < Controller.SlideThreshold && Mathf.Abs( angle - 180.0f ) <= ANGLE_TOLERANCE;
			   break; 
         }

		case EObstacleSlideDirection.Right: 
         {
            passed = mouseDelta.x > Controller.SlideThreshold && angle <= ANGLE_TOLERANCE;
			   break; 
         }
		}

		if (passed) 
		{
			Pass();
         ActivateRigidBodies();
		}
	}

   private void ActivateRigidBodies()
   {
      foreach( Rigidbody2D body in m_rigidBodies )
      {   
         bool activate = CanBeActivated( body );
         if ( !activate )
         {
            continue;
         }

         body.isKinematic = false;

         Vector2 forceVec = new Vector2( transform.position.x, 0 );
         body.AddForce(forceVec, ForceMode2D.Impulse);
      }
   }

   private bool CanBeActivated( Rigidbody2D body )
   {
      foreach ( Rigidbody2D inactiveBody in m_bodiesToLeaveInactive )
      {
         if ( inactiveBody == body )
         {
            return false;
         }
      }

      return true;
   }
}
