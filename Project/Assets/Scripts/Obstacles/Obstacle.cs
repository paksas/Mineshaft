using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using rzymskie_cs;

[RequireComponent( typeof( BoxCollider2D ) )]
[RequireComponent( typeof( AudioSource ) )]
public abstract class Obstacle : MonoBehaviour
{
   public int                    m_obstacleTypeIndex;

   public float                  m_minForceToApply;
   public float                  m_maxForceToApply;
   public string                 m_soundOnPassedName;
   public SpriteRenderer         m_tutorialPanel;

   
   protected BoxCollider2D       m_boxCollider;
   protected AudioSource         m_audioSource;

   protected SoundManager        m_soundManager;
   private ObstacleController    m_controller;

   protected Rigidbody2D[]       m_rigidBodies;
   private float                 m_fadeValue = 0.0f;
   private Text                  m_SignLeftText;
   private Text                  m_SignRightText;
   private Text                  m_SignCentralText;
   private DeathMarkerController m_deathMarker;


   /**
    * The host controller.
    */
   public ObstacleController Controller
   {
      get { return m_controller; }
      set 
      { 
         m_controller = value; 
         Init();
      }
   }

   #region Unity callbacks

   void Awake()
   {
      m_boxCollider = GetComponent<BoxCollider2D>();
      m_audioSource = GetComponent<AudioSource>();
      m_deathMarker = GetComponentInChildren<DeathMarkerController>();
      m_SignRightText = transform.FindChild("SignRight").GetComponentInChildren<Text>();
      m_SignLeftText = transform.FindChild("SignLeft").GetComponentInChildren<Text>();
      m_SignCentralText = transform.FindChild("SignCentral").GetComponentInChildren<Text>();
   }

   void Start()
   {
      m_soundManager = GameObject.FindGameObjectWithTag(GameConsts.TAG_SOUND_MANAGER).GetComponent<SoundManager>();
      m_rigidBodies = this.GetComponentsInChildren< Rigidbody2D >();

      OnStart();
   }

   void Update()
   {
      // fade the rigid bodies of a passed obstacle and obstacle is no longer visible
      if ( m_boxCollider.enabled == false && m_fadeValue >= 0.0f)
      {
         m_fadeValue -= Time.deltaTime;

         foreach( Rigidbody2D body in m_rigidBodies )
         {
            if ( !body.isKinematic && body.GetComponent<HingeJoint2D>() == null)
            {
             
               SpriteRenderer currSprite = body.gameObject.GetComponent<SpriteRenderer>();
               currSprite.color = new Color(1f, 1f, 1f, m_fadeValue);
            }
         }
      }
   }

   /**
    * Initialize your implementation here
    */
   protected abstract void OnStart();

   #endregion

   #region Obstacle functionality

   /**
    * Initializes the obstacle after it's been assigned a controller.
    */
   private void Init()
   {
   }

   public abstract void OnMouseUpObs();

   public abstract void OnMouseDownObs();

   /**
    * Called when the obstacle has been passed
    */
   protected void Pass()
   {
      if (m_soundManager != null && m_audioSource != null && m_soundOnPassedName != null )
      {
         m_soundManager.Play(m_audioSource, ESoundType.SoundEffect, m_soundOnPassedName);
      }

      // disable the obstacle
      m_boxCollider.enabled = false;
      m_tutorialPanel.enabled = false;
      m_fadeValue = Controller.BodiesFadeoutTime;

      // let the controller know what happened
      Controller.OnObstaclePassed(this);
   }

   public void SetObstacleSign (string _number, bool _useCentralCounter)
   {
       if (!_useCentralCounter)
       {
           if (m_SignCentralText != null)
           {
               m_SignCentralText.enabled = false;
           }
           if (m_SignRightText != null)
           {
               m_SignRightText.text = _number;
           }
           if (m_SignLeftText != null)
           {
               m_SignLeftText.text = _number;
           }
       }
       else
       {
           if (m_SignCentralText != null)
           {
               m_SignCentralText.text = _number;
           }
           if (m_SignRightText != null)
           {
               m_SignRightText.enabled = false;
           }
           if (m_SignLeftText != null)
           {
               m_SignLeftText.enabled = false;
           }
       }
      
   }

   public int GetObstacleSignValue ()
    {
        return Rzymskie.roman2arabic(m_SignRightText.text);
    }

    public void ShowDeathMarker ()
   {
        if (m_deathMarker != null)
        {
            m_deathMarker.ShowMark();
        }
   }
   /**
    * Called when the user fails to pass the obstacle.
    */
   protected void Fail()
   {
      // let the controller know what happened
      Controller.OnObstacleFailed(this);
   }

   /**
    * Tells if the obstacle has already been successfully passed by the player.
    */
   public bool HasBeenPassed()
   {
      return !m_boxCollider.enabled || transform.position.y > 0.0f;
   }

   #endregion

   #region Tutorial

   public void ShowTutorial(bool show)
   {
      if ( m_tutorialPanel == null )
      {
         return;
      }
      
      if ( !show )
      {
         // disable the tutorial on all obstalces
         m_tutorialPanel.enabled = false;
      }
      else 
      {
         // but enable it only on those that haven't been passed yet
         if ( !HasBeenPassed())
         {
            m_tutorialPanel.enabled = show;
         }
      }
   }

   #endregion

}
