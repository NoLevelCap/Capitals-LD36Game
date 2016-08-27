using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject CopyTower;

	public MeshRenderer[] ChildrenTowers;
	public float[] height;

	public float TowerHealth = 50;

	public bool Red;

	public Draggable ActiveToken;

	// Use this for initialization
	void Start () {
		
	}

	public void GenerateCity(){
		ChildrenTowers = new MeshRenderer[4];
		height = new float[4];
		int a = 0;
		for (int x = -1; x < 2; x+=2) {
			for (int y = -1; y < 2; y+=2) {
				GameObject Tower = Instantiate<GameObject> (CopyTower);

				Tower.transform.SetParent (this.transform);
				height[a] = TowerHealth + (Random.value) * 5f;
				if(TowerHealth <= 0.5f){
					height[a] = 0;
				}
				Tower.transform.localScale = new Vector3 (0.38f, height[a], 0.38f);
				Tower.transform.localPosition = new Vector3 (x*0.25f, height[a]/2f, y*0.25f);
				ChildrenTowers[a] = Tower.GetComponent<MeshRenderer>();
				a++;
			}
		}

		BoxCollider bc = GetComponent<BoxCollider> ();
		bc.size = new Vector3 (1.25f, TowerHealth + 5f, 1.25f);
		bc.center = new Vector3 (0, (TowerHealth + 5f)/2f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.BlockSelection != this && Red) {
			TurnWhite ();
		}
			

		if(ActiveToken != null){
			ActiveToken.worldPos = transform.position + new Vector3(0, (TowerHealth*0.1f) + 0.5f );
		}
	}

	void TurnRed(){
		foreach (MeshRenderer tower in ChildrenTowers) {
			tower.material.color = Color.red;
		}
		Red = true;
	}

	void TurnWhite(){
		foreach (MeshRenderer tower in ChildrenTowers) {
			tower.material.color = Color.white;
		}
		Red = false;
	}

	void OnMouseOver(){
	}

	void OnMouseEnter(){
		TurnRed ();
		GameManager.BlockSelection = this;
	}

	void OnMouseExit(){
		TurnWhite ();
		if(GameManager.BlockSelection == this){
			GameManager.BlockSelection = null;
		}
	}

	void OnBecameVisible(){
		if(ActiveToken != null){
			ActiveToken.Show ();
		}
	}

	void OnBecameInvisible(){
		if(ActiveToken != null){
			ActiveToken.Hide ();
		}
		Destroy (gameObject);
	}
}
