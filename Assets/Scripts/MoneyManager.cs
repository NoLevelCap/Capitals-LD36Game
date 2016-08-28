using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MoneyManager : MonoBehaviour {

	public Text AmountsTXT, NamesTXT;
	private string Amounts, Names;
	private List<int> AmountsList;
	private List<string> NamesList;
	private List<string> CompareList;

	// Use this for initialization
	void Start () {
		AmountsTXT.text = "";
		NamesTXT.text = "";

		AmountsList = new List<int> ();
		NamesList = new List<string> ();
	}

	public void AddValue(string Name, int Amount){
		AmountsList.Add (Amount);
		NamesList.Add (Name);
	}
	
	// Update is called once per frame
	void Update () {
		Names = "";
		Amounts = "";
		for (int i = 0; i < AmountsList.ToArray().Length; i++) {
			if(i>5){
				AmountsList.RemoveAt(0);
				NamesList.RemoveAt(0);
				break;
			}
			int amount = AmountsList.ToArray () [i];
			string name = NamesList.ToArray () [i];
			if (amount < 0) {
				Amounts = "<color=#FF2E2E" + ("") + ">£" + -amount + "</color>" + "\n" + Amounts;
			} else {
				Amounts = "<color=#2EFF2E" + ("") + ">£" + amount + "</color>" + "\n" + Amounts;
			}

			Names = "<color=#000000>" + name + "</color>:" + "\n" + Names;
		}
		AmountsTXT.text = Amounts;
		NamesTXT.text = Names;
	}
}
