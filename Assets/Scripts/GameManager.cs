using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject[] TokenObjects;

	public static GameObject TokenSelection;
	public static CityGenerator BlockSelection;

	public static CityGenerator[] Blocks;

	public Dictionary<string, Token> tokens;

	public GameObject TokenManager;
	public Queue<Token> ListOfComingTokens;

	public int Population, Money, Day;
	public Text PopulationTXT, DateTXT, MoneyTXT;

	public float MPP = 0.05f;

	private float TimeSinceLC;

	// Use this for initialization
	void Start () {
		tokens = new Dictionary<string, Token> ();
		initTokens ();
	}
	
	// Update is called once per frame
	void Update () {
		PopulationTXT.text = Population + " ";
		MoneyTXT.text = "<color=green>£" + Money + "</color>" + " ";
		DateTXT.text = "Day: " + Day + " ";
	}

	//Day Update
	void FixedUpdate(){
		if(Blocks != null){
			Population = 0;
			foreach (CityGenerator block in Blocks) {
				foreach (float height in block.height) {
					Population += Mathf.RoundToInt(height);
				}
			}
			Money += Mathf.RoundToInt(Population * MPP);
			Day++;

		}
	}
		
	void initTokens(){
		Effect nEffect = new Effect (0,0,0,0,0);
		tokens.Add ("The Wheel", new Token(Type.Science, "The Wheel", Morality.Neutral, nEffect, 0, "no clue"));
		Token par; if (tokens.TryGetValue ("The Wheel", out par)) {par.Unlocked = true;}

		nEffect = new Effect (0,0,0,0,0);
		tokens.Add ("Writing", new Token(Type.Science, "Writing", Morality.Neutral, nEffect, 0, "no clue"));
		if (tokens.TryGetValue ("The Wheel", out par)) {par.Unlocked = true;}

		nEffect = new Effect (0,0,0,0,0);
		tokens.Add ("Basic Tools", new Token(Type.Science, "Basic Tools", Morality.Neutral, nEffect, 0, "no clue"));
		if (tokens.TryGetValue ("The Wheel", out par)) {par.Unlocked = true;}
	}
}
