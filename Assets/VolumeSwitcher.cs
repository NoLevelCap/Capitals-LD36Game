using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSwitcher : MonoBehaviour {

	public Image Button;
	public Sprite On, Off;
	public AudioSource group;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (group.mute) {
			Button.sprite = Off;
		} else {
			Button.sprite = On;
		}
	}

	public void InvertMuteState(){
		group.mute = !group.mute;
	}
}
