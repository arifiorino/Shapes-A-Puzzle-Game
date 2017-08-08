using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour {

	public AudioClip click;
	public AudioClip snap;
	public AudioClip win;

	public static AudioClip clickSound;
	public static AudioClip snapSound;
	public static AudioClip winSound;


	private static SoundEffectPlayer instance = null;
	public static SoundEffectPlayer Instance {
		get { return instance; }
	}
	void Awake() {

		clickSound = click;
		snapSound = snap;
		winSound = win;

		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public static void playSound(AudioClip sound, float withDelay=0){
		if (withDelay == 0)
			instance.gameObject.GetComponent<AudioSource> ().PlayOneShot (sound);
		else
			instance.gameObject.GetComponent<AudioSource> ().PlayDelayed (withDelay);
	}

	public static void setMute(bool mute){
		instance.gameObject.GetComponent<AudioSource> ().mute = mute;
	}
}
