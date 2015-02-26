using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    private ObstacleController m_obstacleController;
    private Obstacle           m_currentObstacle;
    private bool               m_slideDownShown;
    private bool               m_slideRightShown;
    private bool               m_slideLeftShown;
    private bool               m_dubbleTapShown;

    // Use this for initialization
    void Start()
    {
        m_obstacleController = GameObject.FindGameObjectWithTag(GameConsts.TAG_OBSTACLES_CONTROLLER).GetComponent<ObstacleController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_currentObstacle = m_obstacleController.GetIncomingObstacle();

        if (m_currentObstacle is TapObstacle && !m_dubbleTapShown)
        {
            m_dubbleTapShown = true;
            TapObstacle l_obstacle = m_currentObstacle as TapObstacle;
            ShowHint(l_obstacle);
        }
        else if (m_currentObstacle is SlideObstacles )
        {
           SlideObstacles l_obstacle = m_currentObstacle as SlideObstacles;
           EObstacleSlideDirection l_slideDir = l_obstacle.GetObstacleSlideDirection();

            if (!m_slideDownShown && l_slideDir == EObstacleSlideDirection.Down)
            {
                m_slideDownShown = true;
                ShowHint(l_obstacle);
            }
            if (!m_slideLeftShown && l_slideDir == EObstacleSlideDirection.Left)
            {
                m_slideLeftShown = true;
                ShowHint(l_obstacle);
            }
            if (!m_slideRightShown && l_slideDir == EObstacleSlideDirection.Right)
            {
                m_slideRightShown = true;
                ShowHint(l_obstacle);
            }

        }
    }

    private void ShowHint(Obstacle _obstacle)
    {
        if (_obstacle != null)
        {
            Text l_hintText = _obstacle.transform.FindChild("SignCentral").GetComponentInChildren<Text>();
            l_hintText.enabled = true;
        }       
    }
}
