﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CityBlockData
{
	public float[] height;
	public int[] state;
	public MeshRenderer[] ChildrenTowers;
	public float MaxHeight = 0f;
	public BoxCollider bc;

	public float TaxPP = 0.01f;
	public float Pop, Taxes;

	public float CalcPop(){
		Pop = 0;
		for (int i = 0; i < 4; i++) {
			Pop += height [i];
		}
		return Pop;
	}

	public float CalcTaxes(){
		Taxes = TaxPP * Pop;
		return Taxes;
	}

	public void IncreasePopulation(float fac){
		for (int i = 0; i < 4; i++) {
			height [i] += height[i] * fac;
			CheckMaxHeight (height[i]);
		}
		ReAdjustPositioning ();
	}

	public void SpefPopulation(float fac, float num){
		for (int i = 0; i < 4; i++) {
			if(height [i] == 0){height [i] = num;}
			else {height [i] += height[i] * fac;}
			CheckMaxHeight (height[i]);
		}
		ReAdjustPositioning ();
	}

	public void SetPopData(float num){
		for (int i = 0; i < 4; i++) {
			height [i] = num;
			CheckMaxHeight (height[i]);
		}
		ReAdjustPositioning ();
	}

	private void ReAdjustPositioning(){
		for (int i = 0; i < 4; i++) {
			if(RedoMesh(i)){
				float h = GameManager.instance.GetAppropriateHeight (height[i]);

				//ChildrenTowers[i].transform.localScale = new Vector3 (0.38f, height[i], 0.38f);
				//ChildrenTowers[i].transform.localPosition = new Vector3 (ChildrenTowers[i].transform.localPosition.x, height[i]/2f, ChildrenTowers[i].transform.localPosition.z);

				ChildrenTowers[i].transform.localScale = new Vector3 (0.38f, 0.38f,h);
				ChildrenTowers[i].transform.localRotation = Quaternion.Euler (-90, 0, 0);
				ChildrenTowers[i].transform.localPosition = new Vector3 (ChildrenTowers[i].transform.localPosition.x, 0.57f, ChildrenTowers[i].transform.localPosition.z);
			}
		}


		bc.size = new Vector3 (1.25f, (GameManager.instance.GetAppropriateHeight (MaxHeight)+1f)/1f, 1.25f);
		bc.center = new Vector3 (0, (GameManager.instance.GetAppropriateHeight (MaxHeight)+1f)/2f, 0);
	}

	public bool RedoMesh(int i){
		if(GameManager.instance.GetState (height[i]) != state[i]){
			ChildrenTowers [i].GetComponent<MeshFilter> ().mesh = GameManager.instance.GetAppropriateMesh(height[i]);
			state[i] = GameManager.instance.GetState (height[i]);
			return true;
		}
		return false;
	}

	public void CheckMaxHeight(float height){
		if(height > MaxHeight){
			MaxHeight = height;
		}
	}
}

public class CityGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject CopyTower;


	public CityBlockData CBD;

	public float TowerHealth = 50;

	public bool Red;

	public Draggable ActiveToken;

	public AEffect ActiveEffect;

	public int mx, my;

	// Use this for initialization
	void Start () {

	}

	public void GenerateCity(int dx, int dy){
		CBD = new CityBlockData ();
		CBD.ChildrenTowers = new MeshRenderer[4];
		CBD.height = new float[4];
		CBD.state = new int[4];
		int a = 0;
		for (int x = -1; x < 2; x+=2) {
			for (int y = -1; y < 2; y+=2) {
				GameObject Tower = Instantiate<GameObject> (CopyTower);

				Tower.transform.SetParent (this.transform);
				CBD.height[a] = TowerHealth + (Random.value) * 5f;
				if(TowerHealth <= 0.5f){
					CBD.height[a] = 0;
				}

				CBD.CheckMaxHeight (CBD.height[a]);
				float height = GameManager.instance.GetAppropriateHeight (CBD.height[a]);

				Tower.transform.localScale = new Vector3 (0.38f, 0.38f,height);
				Tower.transform.localRotation = Quaternion.Euler (-90, 0, 0);
				Tower.transform.localPosition = new Vector3 (x*0.25f, 0.57f, y*0.25f);
				CBD.ChildrenTowers[a] = Tower.GetComponent<MeshRenderer>();
				CBD.ChildrenTowers [a].GetComponent<MeshFilter> ().mesh = GameManager.instance.GetAppropriateMesh(CBD.height[a]);
				CBD.state[a] = GameManager.instance.GetState (CBD.height[a]);
				a++;
			}
		}

		mx = dx;
		my = dy;

		BoxCollider bc = GetComponent<BoxCollider> ();
		bc.size = new Vector3 (1.25f, (GameManager.instance.GetAppropriateHeight (CBD.MaxHeight)+1f)/1f, 1.25f);
		bc.center = new Vector3 (0, (GameManager.instance.GetAppropriateHeight (CBD.MaxHeight)+1f)/2f, 0);
		CBD.bc = bc;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.BlockSelection != this && Red) {
			TurnWhite ();
		}
			

		if(ActiveToken != null){
			ActiveToken.worldPos = transform.position + new Vector3(0, (TowerHealth*0.1f) + 1.5f );
		}
	}

	public void TurnRed(){
		GetComponent<MeshRenderer> ().material.color = Color.red;
		foreach (MeshRenderer tower in CBD.ChildrenTowers) {
			tower.material.color = Color.red;
		}
		Red = true;
	}

	void TurnWhite(){
		GetComponent<MeshRenderer> ().material.color = new Color(114f/255f, 114f/255f, 114f/255f);
		foreach (MeshRenderer tower in CBD.ChildrenTowers) {
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


}
