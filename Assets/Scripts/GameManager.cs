using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GameObject[] TokenObjects;
	public Sprite[] TokenIcons;

	public GameObject TechData, TechTree;
	public MoneyManager Man;

	public GameObject Icon;

	public static GameObject TokenSelection;
	public static CityGenerator BlockSelection;

	public static bool NewDayFrame;
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

	private static GameManager s_Instance = null;

	public static GameManager instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (GameManager)) as GameManager;
			}

			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AManager");
				s_Instance = obj.AddComponent(typeof (GameManager)) as GameManager;
				Debug.Log ("Could not locate an AManager object. AManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}

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
		if (TokenPlayed || Day == 0) {
			NewDayFrame = true;
			DayUpdate ();
			TokenPlayed = false;
		} else {
			NewDayFrame = false;
		}
		PopulationTXT.text = Population + " ";
		MoneyTXT.text = "<color=green>£" + Money + "</color>" + " ";
		DateTXT.text = "Day: " + Day + " ";
	}

	//Day Update
	void DayUpdate(){
		if(Blocks != null){
			ShowAvailableTech ();

			foreach (Transform obj in TokenBoard.transform) {
				Draggable drg = obj.GetComponent<Draggable> ();
				drg.UpdateDuration ();
				drg.CheckForDestroy ();
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
			Man.AddValue ("Day " + (Day+1) + " taxes", Mathf.RoundToInt (Taxes));
			Day++;

			FillHand (1);


			CheckUnlockedTokens ();
		}
	}

	void ShowAvailableTech(){
		foreach (Transform item in TechTree.transform) {
			Destroy (item.gameObject);
		}
		foreach (Token tech in UnlockedTokens) {
			foreach (Token child in tech.children) {
				if(!child.Unlocked){
					GameObject gj = Instantiate<GameObject> (TechData);
					gj.transform.parent = TechTree.transform;
					gj.transform.GetChild (0).GetComponent<Text> ().text = child.Name;
					gj.transform.GetChild (1).GetChild (0).GetComponent<Image> ().sprite = TokenIcons [child.TokenID];
					gj.transform.GetChild (3).GetComponent<Text> ().text = child.Desc;
					gj.transform.GetChild (4).GetComponent<Text> ().text = tech.Name;
					gj.transform.GetChild (5).GetChild (1).GetComponent<Image> ().fillAmount = (float) child.Progress / (float) child.UpPoints;

					if (child.Progress < child.UpPoints) {
						gj.transform.GetChild (5).GetChild (0).GetComponent<Text> ().text = child.Progress + "/" + child.UpPoints;
						gj.transform.GetChild (5).GetChild (1).GetChild (0).GetComponent<Text> ().text = child.Progress + "/" + child.UpPoints;
					} else {
						gj.transform.GetChild (5).GetChild (0).GetComponent<Text> ().text = "Unlocked Tommorow";
						gj.transform.GetChild (5).GetChild (1).GetChild (0).GetComponent<Text> ().text = "Unlocked Tommorow";
					}
				}
			}
		}
	}
		

	void CheckUnlockedTokens(){
		UnlockedTokens.Clear ();
		foreach (Token token in tokens.Values) {
			foreach (Token child in token.children) {
				child.CheckForUnlock ();
			}
		}
		foreach (Token token in tokens.Values) {
			if (token.Unlocked) {
				UnlockedTokens.Add (token);
			} else {
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
			Token.transform.SetParent(TokenManager.transform, false);
			Token.GetComponent<Draggable> ().token = random;
			Token.GetComponentInChildren<Text> ().text = random.Name;
			Token.GetComponent<Draggable> ().Icon.sprite =  TokenIcons [random.TokenID];
			Token.GetComponent<Draggable> ().UpdateDuration ();
			Token.GetComponent<Draggable> ().SetIcons ();

		}
	}

	Token getRandomToken(){
		return UnlockedTokens.ToArray()[Random.Range(0, UnlockedTokens.Count)];
	}
		
	void initTokens(){
		AEffect nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		tokens.Add ("The Wheel", new Token(Type.Science, "The Wheel", Morality.Neutral, nEffect, 3, 0, 0, "no clue"));
		Token par; if (tokens.TryGetValue ("The Wheel", out par)) {par.Unlocked = true;}

		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		tokens.Add ("Writing", new Token(Type.Buisness, "Writing", Morality.Neutral, nEffect, 3, 0, 1, "no clue"));
		if (tokens.TryGetValue ("Writing", out par)) {par.Unlocked = true;}

		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		Token child = new Token(Type.Science, "Example", Morality.Neutral, nEffect, 3, 10, 1, "no clue");
		tokens.Add (child.Name, child);
		if (tokens.TryGetValue ("Writing", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		child = new Token(Type.Science, "Example2", Morality.Neutral, nEffect, 3, 10, 1, "no clue");
		tokens.Add (child.Name, child);
		if (tokens.TryGetValue ("Writing", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		child = new Token(Type.Science, "ExampleChild", Morality.Neutral, nEffect, 3, 10, 1, "no clue");
		tokens.Add (child.Name, child);
		if (tokens.TryGetValue ("Example", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}


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
