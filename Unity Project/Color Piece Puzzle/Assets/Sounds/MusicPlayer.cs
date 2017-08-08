using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

	private static MusicPlayer instance = null;
	public static MusicPlayer Instance {
		get { return instance; }
	}
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);
	}
	public static void pause(){
		instance.GetComponent<AudioSource> ().Pause ();
	}
	public static void play(){
		instance.gameObject.GetComponent<AudioSource> ().Play ();
	}

	public static void setMute(bool mute){
		instance.gameObject.GetComponent<AudioSource> ().mute = mute;
	}

	public static bool getMute(){
		return instance.gameObject.GetComponent<AudioSource> ().mute;
	}
}