using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject[] TokenObjects;
	public Sprite[] TokenIcons;

	public static GameObject TokenSelection;
	public static CityGenerator BlockSelection;

	public static CityGenerator[] Blocks;

	public Dictionary<string, Token> tokens;

	public GameObject TokenManager;
	public Queue<Token> ListOfComingTokens;
	private List<Token> UnlockedTokens;

	public int Population, Money, Day;
	public Text PopulationTXT, DateTXT, MoneyTXT;

	public float MPP = 0.05f;

	private float TimeSinceLC;

	// Use this for initialization
	void Start () {
		tokens = new Dictionary<string, Token> ();
		ListOfComingTokens = new Queue<Token> ();
		UnlockedTokens = new List<Token> ();
		initTokens ();
		CheckUnlockedTokens ();
		FillHand (5);
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

	void CheckUnlockedTokens(){
		UnlockedTokens.Clear ();
		foreach (Token token in tokens.Values) {
			if(token.Unlocked){
				UnlockedTokens.Add (token);
			}
			
		}
	}

	void FillHand(int amount){
		for (int i = 0; i < amount; i++) {
			Token random = getRandomToken ();
			GameObject TokenType;
			switch (random.Type) {
				case Type.Buisness:
					TokenType = TokenObjects [0];
					break;
				case Type.Science:
					TokenType = TokenObjects [1];
					break;
				default:
					TokenType = TokenObjects [2];
					break;
			}
			GameObject Token = Instantiate<GameObject> (TokenType);
			Token.transform.parent = TokenManager.transform;
			Token.GetComponent<Draggable> ().token = random;
			Token.GetComponentInChildren<Text> ().text = random.Name;
			Token.GetComponent<Draggable> ().Icon.sprite =  TokenIcons [random.TokenID];

		}
	}

	Token getRandomToken(){
		return UnlockedTokens.ToArray()[Random.Range(0, UnlockedTokens.Count)];
	}
		
	void initTokens(){
		Effect nEffect = new Effect (0,0,0,0,0);
		tokens.Add ("The Wheel", new Token(Type.Science, "The Wheel", Morality.Neutral, nEffect, 0, 0, "no clue"));
		Token par; if (tokens.TryGetValue ("The Wheel", out par)) {par.Unlocked = true;}

		nEffect = new Effect (0,0,0,0,0);
		tokens.Add ("Writing", new Token(Type.Science, "Writing", Morality.Neutral, nEffect, 0, 1, "no clue"));
		if (tokens.TryGetValue ("Writing", out par)) {par.Unlocked = true;}

		nEffect = new Effect (0,0,0,0,0);
		tokens.Add ("Basic Tools", new Token(Type.Science, "Basic Tools", Morality.Neutral, nEffect, 0,  0, "no clue"));
		if (tokens.TryGetValue ("Basic Tools", out par)) {par.Unlocked = true;}
	}
}
