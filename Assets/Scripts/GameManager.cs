using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject[] TokenObjects;
	public Sprite[] TokenIcons;

	public static GameObject TokenSelection;
	public static CityGenerator BlockSelection;

	public static int width = 10, height = 10;

	public static CityGenerator[] Blocks;

	public Dictionary<string, Token> tokens;

	public GameObject TokenManager, TokenBoard;
	public Queue<Token> ListOfComingTokens;
	private List<Token> UnlockedTokens;

	public static int Population, Money, Day;
	public Text PopulationTXT, DateTXT, MoneyTXT;

	public float MPP = 0.00f;

	public static bool TokenPlayed;

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
		if(TokenPlayed || Day == 0){
			DayUpdate ();
			TokenPlayed = false;
		}
		PopulationTXT.text = Population + " ";
		MoneyTXT.text = "<color=green>£" + Money + "</color>" + " ";
		DateTXT.text = "Day: " + Day + " ";
	}

	//Day Update
	void DayUpdate(){
		if(Blocks != null){
			foreach (Transform obj in TokenBoard.transform) {
				obj.GetComponent<Draggable> ().CheckForDestroy ();
			}

			foreach (CityGenerator block in Blocks) {
				if(block.ActiveToken != null){
					block.ActiveEffect.TriggerEffect (block);
				}
			}

			float Pop = 0;
			float Taxes = 0;
			foreach (CityGenerator block in Blocks) {
				CityBlockData CBD = block.CBD;
				Pop += CBD.CalcPop();
				Taxes += CBD.CalcTaxes();
			}
			Population = Mathf.RoundToInt (Pop); Money += Mathf.RoundToInt (Taxes);
			Day++;

			FillHand (1);
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
		AEffect nEffect = new PopulationIncreaseEffect ();
		tokens.Add ("The Wheel", new Token(Type.Science, "The Wheel", Morality.Neutral, nEffect, 3, 0, 0, "no clue"));
		Token par; if (tokens.TryGetValue ("The Wheel", out par)) {par.Unlocked = true;}

		Debug.Log (new PopulationIncreaseEffect());

		nEffect = new LocalTaxesIncreaseEffect ();
		tokens.Add ("Writing", new Token(Type.Buisness, "Writing", Morality.Neutral, nEffect, 3, 0, 1, "no clue"));
		if (tokens.TryGetValue ("Writing", out par)) {par.Unlocked = true;}
		/*
		nEffect = new Effect (SpecialMode.PopIncrease, 0, 100f);
		tokens.Add ("Basic Tools", new Token(Type.Science, "Basic Tools", Morality.Neutral, nEffect, 0,  0, "no clue"));
		if (tokens.TryGetValue ("Basic Tools", out par)) {par.Unlocked = true;}*/
	}

	public static CityGenerator GetBlock(int mx, int my){
		if(Blocks != null){
			return Blocks [(mx * width * 2) + my ];
		}
		return null;
	}
}
