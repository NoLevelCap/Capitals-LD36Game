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

	public int HandSize;

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
			if(Day > 0){
				HandSize -= 1;
			}
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

			if(HandSize < 4){
				FillHand (1);
			}


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
					gj.transform.GetChild (0).GetComponent<Text> ().text = "<color=" + GetTypeColor(child.Type) + ">" + child.Name + "</color>";
					gj.transform.GetChild (1).GetChild (0).GetComponent<Image> ().sprite = TokenIcons [child.TokenID];
					gj.transform.GetChild (3).GetComponent<Text> ().text = child.Desc;
					gj.transform.GetChild (4).GetComponent<Text> ().text = "<color=" + GetTypeColor(tech.Type) + ">" + tech.Name + "</color>";
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

	string GetTypeColor(Type type){
		switch (type) {
		case Type.Buisness:
			return "#c98c06";
		case Type.Science:
			return "#3ac2f0";
		default:
			return "#ffffff";
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

	public void FillHand(int amount){
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
		HandSize += amount;
	}

	public bool ChargeACost(string name, int amount){
		if(Money - amount >= 0){
			Money -= amount;
			if(amount != 0){
				Man.AddValue (name, -amount);
			}
			return true;
		} else {
			return false;
		}
	}

	Token getRandomToken(){
		float rarity = 0f;
		int id;
		do {
			id = Random.Range (0, UnlockedTokens.Count);
			rarity = UnlockedTokens.ToArray()[id].Rarity;
		} while (Random.value > rarity);
		return UnlockedTokens.ToArray()[id];
	}
		
	void initTokens(){
		AEffect nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{15f, 0.01f, 0.02f});
		Token child = new Token (Type.Science, "The Wheel", Morality.Neutral, nEffect, 10, 0, 0, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		nEffect.setData (new float[]{0.02f, 0.04f, 0.01f});
		child = new Token (Type.Science, "Writing", Morality.Neutral, nEffect, 10, 0, 1, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		//PopIncreaseGroup
		nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{50f, 0.02f, 0.04f});
		child = new Token(Type.Science, "Horse & Cart", Morality.Neutral, nEffect, 10, 10, 2, 1f, "");
		tokens.Add (child.Name, child);
		Token par; if (tokens.TryGetValue ("The Wheel", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

		nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{200f, 0.04f, 0.08f});
		child = new Token(Type.Science, "Bike", Morality.Neutral, nEffect, 10, 10, 2, 1f, "");
		tokens.Add (child.Name, child);
		if (tokens.TryGetValue ("Horse & Cart", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

		nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{1000f, 0.08f, 0.16f});
		child = new Token(Type.Science, "The Car", Morality.Neutral, nEffect, 10, 10, 2, 1f, "");
		tokens.Add (child.Name, child);
		if (tokens.TryGetValue ("Bike", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

		//TaxesIncreaseGroup
		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		nEffect.setData (new float[]{0.04f, 0.08f, 0.02f});
		child = new Token(Type.Science, "The Printing Press", Morality.Neutral, nEffect, 20, 10, 2, 1f, "");
		tokens.Add (child.Name, child);
		if (tokens.TryGetValue ("Writing", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

		nEffect = ScriptableObject.CreateInstance<AddCardsEffect>();
		nEffect.setData (new float[]{10f, 2f});
		child = new Token (Type.Science, "Metalworking", Morality.Neutral, nEffect, 40, 0, 1, 0.1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<IncreasePopEffect>();
		nEffect.setData (new float[]{10f, 2f, 1f});
		child = new Token (Type.Science, "Farming", Morality.Neutral, nEffect, 25, 0, 1, 0.75f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<IncreaseMoney>();
		nEffect.setData (new float[]{200f});
		child = new Token (Type.Science, "Abacus", Morality.Neutral, nEffect, 30, 0, 1, 0.75f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<ClearTilesEffect>();
		nEffect.setData (new float[]{5f, 1f});
		child = new Token (Type.Science, "Archeology", Morality.Neutral, nEffect, 2, 0, 1, 0.75f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		
	}

	public static CityGenerator GetBlock(int mx, int my){
		if(Blocks != null){
			return Blocks [(mx * width * 2) + my ];
		}
		return null;
	}
}
