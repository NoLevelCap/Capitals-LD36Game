using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 move = new Vector3 ();
		if (Input.GetKey(KeyCode.A)) {
			transform.RotateAround (Vector3.zero, Vector3.up, 60f * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.D)) {
			transform.RotateAround (Vector3.zero, Vector3.up, -60f * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.Q)) {
			move -= new Vector3 (0, 20f * Time.deltaTime, 0);
		}
		if (Input.GetKey(KeyCode.E)) {
			move += new Vector3 (0, 20f * Time.deltaTime, 0);
		}

		if (Input.GetKey(KeyCode.S)) {
			move -= new Vector3 (0, 0, 20f * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.W)) {
			move += new Vector3 (0, 0, 20f * Time.deltaTime);
		}

		transform.Translate (move);
		transform.LookAt (Vector3.zero);

	}
}
