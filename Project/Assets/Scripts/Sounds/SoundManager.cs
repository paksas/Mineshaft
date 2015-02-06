using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ESoundType
{
	SoundEffect,
	Music
}

[System.Serializable]
public class AudioClipDef
{
	public string 	m_audioClipName;
	public AudioClip 	m_audioClip;
		
}

public class SoundManager : MonoBehaviour 
{
	private const string  MUSIC_ENABLED = "MusicEnabled";
	private const string  SOUNDS_ENABLED = "SoundsEnabled";

	public bool m_soundsEnabled = true;
	public bool m_musicEnabled = true;
	public AudioSource m_ambientPlayer;

	public List <AudioClipDef> m_audioClips; 
		

	void Awake ()
	{
        if ( m_ambientPlayer == null )
        {
            m_ambientPlayer = GameObject.Find("AmbientPlayer").GetComponent<AudioSource>();
        }

		if( PlayerPrefs.HasKey( MUSIC_ENABLED ) )
		{
			EnableMusic( PlayerPrefs.GetInt( MUSIC_ENABLED, 0 ) == 1 );
		}
		if( PlayerPrefs.HasKey( SOUNDS_ENABLED ) )
		{
			EnableSounds( PlayerPrefs.GetInt( SOUNDS_ENABLED, 0 ) == 1 );
		}
	}
	// Use this for initialization
	void Start () 
	{
		Play ( m_ambientPlayer, ESoundType.Music, "Ambient" );
	}


    public void ToogleMusicAndSounds ()
    {
        EnableMusic(!m_musicEnabled);
        EnableSounds(!m_soundsEnabled);
    }
		
	public void Play ( AudioSource _audioSource, ESoundType _soundType )
	{
		if ( _soundType == ESoundType.SoundEffect && m_soundsEnabled )
		{			
			_audioSource.Play();
		}
		if ( _soundType == ESoundType.Music && m_musicEnabled )
		{			
			_audioSource.Play();					
		}
	}

	public void Play ( AudioSource _audioSource, ESoundType _soundType, bool _force )
	{
		if ( _soundType == ESoundType.SoundEffect && m_soundsEnabled )
		{
			if ( _force )
			{
				_audioSource.Play();
			}
			else
			{
				if ( !_audioSource.isPlaying )
				{
					_audioSource.Play();
				}
			}
		}
		if ( _soundType == ESoundType.Music && m_musicEnabled )
		{
			if ( _force )
			{
				_audioSource.Play();
			}
			else
			{
				if ( !_audioSource.isPlaying )
				{
					_audioSource.Play();
				}
			}
		}
	}

	public void Play ( AudioSource _audioSource, ESoundType _soundType, string _clipName, bool _force )
	{
		if ( _soundType == ESoundType.SoundEffect && m_soundsEnabled )
		{	
			if ( _force )
			{
				_audioSource.clip = GetAudioClip ( _clipName );
				_audioSource.Play();
			}
			else
			{
				if ( !_audioSource.isPlaying )
				{
					_audioSource.clip = GetAudioClip ( _clipName );
					_audioSource.Play();
				}
			}
		}
		if ( _soundType == ESoundType.Music && m_musicEnabled )
		{
			if ( _force )
			{
				_audioSource.clip = GetAudioClip ( _clipName );
				_audioSource.Play();
			}
			else
			{
				if ( !_audioSource.isPlaying )
				{
					_audioSource.clip = GetAudioClip ( _clipName );
					_audioSource.Play();
				}
			}
		}
	}

	public void Play ( AudioSource _audioSource, ESoundType _soundType, string _clipName )
	{
		if ( _soundType == ESoundType.SoundEffect && m_soundsEnabled )
		{
			_audioSource.clip = GetAudioClip ( _clipName );
			_audioSource.Play();
			
		}
		if ( _soundType == ESoundType.Music && m_musicEnabled )
		{							
			_audioSource.clip = GetAudioClip ( _clipName );
			_audioSource.Play();			
		}
	}

    public void Stop ( AudioSource _audioSource )
    {
        _audioSource.Stop();
    }

	public void StopMusic ()
	{
		m_ambientPlayer.Stop();
	}

    public void PlayMusic()
    {
        Play(m_ambientPlayer, ESoundType.Music, "Ambient");
    }
	public void EnableMusic ( bool _isEnabled )
	{
		m_musicEnabled = _isEnabled;

		if ( !_isEnabled )
		{
			StopMusic ();
		}
        else
        {
            PlayMusic();
        }
		PlayerPrefs.SetInt( MUSIC_ENABLED, _isEnabled ? 1 : 0 ) ;
	}

	public void EnableSounds ( bool _isEnabled )
	{
		m_soundsEnabled = _isEnabled;
		PlayerPrefs.SetInt( SOUNDS_ENABLED, _isEnabled ? 1 : 0 ) ;
	}

	private AudioClip GetAudioClip ( string _clipName )
	{
		for ( int i = 0; i < m_audioClips.Count; i++ )
		{
			if ( m_audioClips[i].m_audioClipName == _clipName )
			{
				return m_audioClips[i].m_audioClip;
			}
		}
		return null;
	}
}
