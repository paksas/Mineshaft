using UnityEngine;
using System.Collections;
using System;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

[RequireComponent( typeof( Animator ) )]
public class ScoreDisplay : MonoBehaviour 
{
   public SpriteRenderer[]          m_digitSprites = null;
 


	// Use this for initialization
   void Start () 
   {
      if ( m_digitSprites == null || m_digitSprites.Length != 10 )
      {
         Debug.LogError( "ScoreDisplay: please define 10 sprites corresponding to successive numbers in range 0-9" );
         return;
      }

      // run a check to see if all digit sprites were initialized
      for ( int i = 0; i < m_digitSprites.Length; ++i )
      {
         if ( m_digitSprites[i] == null )
         {
            Debug.LogError( "ScoreDisplay: the sprite for digit '" + i + "' has not been set" );
            return;
         }
      }

      // get the serialized player score
	  int  playerScore = PlayerPrefs.GetInt( GameConsts.PLAYER_SCORE_KEY );


      // output the player score on screen
      string scoreStr = playerScore.ToString();
      char[] scoreDigitsArr = scoreStr.ToCharArray();
      SpriteRenderer[] instantiatedSprites = new SpriteRenderer[scoreDigitsArr.Length];

      float offset = 0;
      float drawingWidth = 0.0f;
      float prevDigitWidth = 0.0f;
      for( int charIdx = 0; charIdx < scoreDigitsArr.Length; ++charIdx )
      {
         int digitIdx = Uri.FromHex( scoreDigitsArr[charIdx] );

         SpriteRenderer digitSprite = Instantiate( m_digitSprites[digitIdx], new Vector3( offset, 0.0f, 0.0f ), Quaternion.identity ) as SpriteRenderer;
         digitSprite.transform.parent = transform;
         instantiatedSprites[charIdx] = digitSprite;

         // position the digit so that it's at the correct distance from the previous digit
         float digitWidth = digitSprite.renderer.bounds.extents.x * 2.0f;
         if ( charIdx > 0 )
         {
            offset += ( prevDigitWidth + digitWidth ) * 0.5f;
         }
         digitSprite.transform.localPosition = new Vector3( offset, 0.0f, 0.0f );

         // increase the offset
         drawingWidth += charIdx == 0 ? digitWidth * 0.5f : digitWidth;
         prevDigitWidth = digitWidth;
      }

      // center the text at the origin
      float textAlignmentOffset = drawingWidth * 0.5f;
      for( int charIdx = 0; charIdx < instantiatedSprites.Length; ++charIdx )
      {
         Transform spriteTransform = instantiatedSprites[charIdx].transform;
         spriteTransform.localPosition = new Vector3( spriteTransform.localPosition.x - textAlignmentOffset, 0.0f, 0.0f );
      }


      PLayGamesController.instance.PostScore();
      
	}
	
}
