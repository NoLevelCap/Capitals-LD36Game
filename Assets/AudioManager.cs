using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public AudioClip[] audioclips;
	public Dictionary<string, AudioClip> AudioLibrary;
	public AudioSource source;
	private string CurrentSource = "";

	private static AudioManager s_Instance = null;

	public static AudioManager instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (AudioManager)) as AudioManager;
			}

			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AManager");
				s_Instance = obj.AddComponent(typeof (AudioManager)) as AudioManager;
				Debug.Log ("Could not locate an AManager object. AManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}

	// Use this for initialization
	void Start () {
		AudioLibrary = new Dictionary<string, AudioClip> ();
		AddAudioClips ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void AddAudioClips(){
		AudioLibrary.Add ("Pick/Place", audioclips[0]);
		AudioLibrary.Add ("Upgrade", audioclips[1]);
	}

	public void Play(string Key, float volume){

		source.volume = volume;
		if(Key == CurrentSource){
			source.Play ();
			return;
		}
		AudioClip value;
		if (AudioLibrary.TryGetValue (Key, out value)) {
			Debug.Log ("Playing");
			source.clip = value;
			CurrentSource = Key;
			source.Play ();
		} else {
			Debug.LogError ("No AudioClip named " + Key + " in the Library.");
		}	
	}
}
