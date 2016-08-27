using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityManager : MonoBehaviour {

	public GameObject CityBlock;

	// Use this for initialization
	void Start () {
		int width = GameManager.width;
		int height = GameManager.height;
		GameManager.Blocks = new CityGenerator[width * height * 2 * 2];
		int i = 0;
		for (float x = -width; x < width; x++) {
			for (float y = -height; y < height; y++) {
				
				GameObject CityObject = Instantiate<GameObject> (CityBlock);
				CityObject.transform.SetParent (transform);
				CityObject.transform.localPosition = new Vector3 (2f*x, 0f, 2f*y);
				CityGenerator CG = CityObject.GetComponent<CityGenerator> ();
				//City Design 1st Num is Spread
				//CG.TowerHealth = 45f*Mathf.Abs(((1f-(Mathf.Abs(x)/width))+(1f-(Mathf.Abs(y)/height)))/4f);
				CG.TowerHealth = 2f*Mathf.Abs(((1f-((Mathf.Abs(x)+1)/width))+(1f-((Mathf.Abs(y)+1)/height)))/4f);
				CG.GenerateCity((int)x+(int)width, (int)y+(int)height);
				GameManager.Blocks[i] = CG;
				i++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
