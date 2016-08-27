using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject CopyTower;

	public List<MeshRenderer> ChildrenTowers;

	public float TowerHealth = 50;

	public bool Over;

	// Use this for initialization
	void Start () {
		
	}

	public void GenerateCity(){
		ChildrenTowers = new List<MeshRenderer> ();
		for (int x = -1; x < 2; x+=2) {
			for (int y = -1; y < 2; y+=2) {
				GameObject Tower = Instantiate<GameObject> (CopyTower);

				Tower.transform.SetParent (this.transform);
				float Height = TowerHealth + (Random.value) * 5f;
				Tower.transform.localScale = new Vector3 (0.38f, Height, 0.38f);
				Tower.transform.localPosition = new Vector3 (x*0.25f, Height/2f, y*0.25f);
				ChildrenTowers.Add (Tower.GetComponent<MeshRenderer>());
			}
		}

		BoxCollider bc = GetComponent<BoxCollider> ();
		bc.size = new Vector3 (1.25f, TowerHealth + 5f, 1.25f);
		bc.center = new Vector3 (0, (TowerHealth + 5f)/2f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Over) {
			foreach (MeshRenderer tower in ChildrenTowers) {
				tower.material.color = Color.red;
			}
		} else {
			foreach (MeshRenderer tower in ChildrenTowers) {
				tower.material.color = Color.white;
			}
		}

		Over = false;
	}

	void OnMouseOver(){
		Over = true;
	}
}
