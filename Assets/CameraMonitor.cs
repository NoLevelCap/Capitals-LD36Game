using UnityEngine;
using System.Collections;

public class CameraMonitor : MonoBehaviour {

	public Canvas main;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(main.worldCamera == null){
			main.worldCamera = Camera.main;
		}
	}
}
