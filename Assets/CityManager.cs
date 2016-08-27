using UnityEngine;
using System.Collections;

public class CityManager : MonoBehaviour {

	public GameObject CityBlock;
	public float width, height;

	// Use this for initialization
	void Start () {
		for (float x = -width; x < width; x++) {
			for (float y = -height; y < height; y++) {
				GameObject CityObject = Instantiate<GameObject> (CityBlock);
				CityObject.transform.SetParent (transform);
				CityObject.transform.localPosition = new Vector3 (2f*x, 0f, 2f*y);
				CityGenerator CG = CityObject.GetComponent<CityGenerator> ();
				//City Design 1st Num is Spread
				//CG.TowerHealth = 45f*Mathf.Abs(((1f-(Mathf.Abs(x)/width))+(1f-(Mathf.Abs(y)/height)))/4f);
				CG.TowerHealth = 10f*Mathf.Abs(((1f-((Mathf.Abs(x)+1)/width))+(1f-((Mathf.Abs(y)+1)/height)))/4f);
				CG.GenerateCity();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
