using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	[SerializeField] private AudioSource sfxAudioSource;

	void Awake ()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (instance);
	}

	public void PlaySFX (AudioClip clip)
	{
		sfxAudioSource.clip = clip;
		sfxAudioSource.Play ();
	}

}
