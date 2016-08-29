using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutputBoard : MonoBehaviour {

	public Text log;

	// Use this for initialization
	void Start () {
		log.text = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddtoLog(string x){
		log.text = x + "\n" + log.text;
	}
}
