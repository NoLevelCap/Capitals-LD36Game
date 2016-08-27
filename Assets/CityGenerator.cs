using UnityEngine;
using System.Collections;

public class CityGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject CopyTower;

	public GameObject[] ChildrenTowers;

	public float TowerHealth = 50;

	// Use this for initialization
	void Start () {
		
	}

	public void GenerateCity(){
		for (int x = -1; x < 2; x+=2) {
			for (int y = -1; y < 2; y+=2) {
				GameObject Tower = Instantiate<GameObject> (CopyTower);

				Tower.transform.SetParent (this.transform);
				float Height = TowerHealth + (Random.value) * 5f;
				Tower.transform.localScale = new Vector3 (0.38f, Height, 0.38f);
				Tower.transform.localPosition = new Vector3 (x*0.25f, Height/2f, y*0.25f);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
