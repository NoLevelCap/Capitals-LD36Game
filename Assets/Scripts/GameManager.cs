using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


	public GameObject[] TokenObjects;

	public Mesh[] SmallProperties, MidSizedProperties, LargeProperties;
	public Sprite[] EffectIcons, TokenIcons;

	public GameObject TechData, TechTree;
	public MoneyManager Man;
	public OutputBoard OB;

	public GameObject Icon, Floater;
	public Text Alert;

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

		if(Input.GetMouseButtonDown(0)){
			Alert.CrossFadeAlpha (0f, 0.5f, false); 
		}
	}

	void UpdateFloaters(){
		
	}

	//Day Update
	void DayUpdate(){
		if(Blocks != null){
			ShowAvailableTech ();

			foreach (Transform obj in TokenBoard.transform) {
				if (obj.gameObject.tag == "Token") {
					Draggable drg = obj.GetComponent<Draggable> ();
					drg.UpdateDuration ();
					drg.CheckForDestroy ();
				} 
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

			if(HandSize < 5){
				FillHand (1);
			}


			CheckUnlockedTokens ();
		}
	}

	void ShowAvailableTech(){
		foreach (Transform item in TechTree.transform) {
			Destroy (item.gameObject);
		}
		int a = 0;
		foreach (Token tech in UnlockedTokens) {
			foreach (Token child in tech.children) {
				if(!child.Unlocked){
					GameObject gj = Instantiate<GameObject> (TechData);
					gj.transform.SetParent(TechTree.transform, false);
					gj.transform.GetChild (1).GetComponent<Text> ().text = "<color=" + GetTypeColor(child.Type) + ">" + child.Name + "</color>";
					gj.transform.GetChild (2).GetChild (0).GetComponent<Image> ().sprite = TokenIcons [child.TokenID];
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
					a++;
				}
			}
		}

		if (a == 0) {
			TechTree.transform.parent.parent.gameObject.SetActive (false);
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
				if(!child.Unlocked){
					if (child.CheckForUnlock ()) {
						OB.AddtoLog (child.Name + " Unlocked");
					}
				}
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
		Token token;
		do {
			id = Random.Range (0, UnlockedTokens.Count);
			rarity = UnlockedTokens.ToArray()[id].Rarity;
		} while (Random.value > rarity);
		return UnlockedTokens.ToArray()[id];
	}
		
	void initTokens(){
		AEffect nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{15f, 0.01f, 0.02f});
		Token child = new Token (Type.Buisness, "Tourism Board", Morality.Neutral, nEffect, 10, 10, 4, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		nEffect.setData (new float[]{0.02f, 0.04f, 0.01f});
		child = new Token (Type.Buisness, "Tax Office", Morality.Neutral, nEffect, 10, 10, 5, 1f, "");
		tokens.Add (child.Name, child);

		//PopIncreaseGroup
		nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{50f, 0.02f, 0.04f});
		child = new Token(Type.Buisness, "Bus Station", Morality.Neutral, nEffect, 10, 10, 12, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{200f, 0.04f, 0.08f});
		child = new Token(Type.Buisness, "Railway", Morality.Neutral, nEffect, 10, 10, 13, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<PopulationIncreaseEffect>();
		nEffect.setData (new float[]{1000f, 0.08f, 0.16f});
		child = new Token(Type.Buisness, "Airport", Morality.Neutral, nEffect, 10, 10, 14, 1f, "");
		tokens.Add (child.Name, child);

		//TaxesIncreaseGroup
		nEffect = ScriptableObject.CreateInstance<LocalTaxesIncreaseEffect>();
		nEffect.setData (new float[]{0.04f, 0.08f, 0.02f});
		child = new Token(Type.Buisness, "The Printing Press", Morality.Neutral, nEffect, 20, 10, 15, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<AddCardsEffect>();
		nEffect.setData (new float[]{10f, 2f});
		child = new Token (Type.Buisness, "General Store", Morality.Neutral, nEffect, 40, 10, 16, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<IncreasePopEffect>();
		nEffect.setData (new float[]{10f, 2f, 1f});
		child = new Token (Type.Buisness, "Farms", Morality.Neutral, nEffect, 25, 10, 17, 0.75f, "");
		tokens.Add (child.Name, child);

		/*nEffect = ScriptableObject.CreateInstance<IncreasePopEffect>();
		nEffect.setData (new float[]{0f, 80f, 1f});
		child = new Token (Type.Buisness, "SuperBOON", Morality.Neutral, nEffect, 1, 0, 1, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;*/

		nEffect = ScriptableObject.CreateInstance<IncreaseMoney>();
		nEffect.setData (new float[]{200f});
		child = new Token (Type.Buisness, "A Front", Morality.Neutral, nEffect, 30, 10, 18, 0.75f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<ClearTilesEffect>();
		nEffect.setData (new float[]{5f, 1f});
		child = new Token (Type.Buisness, "A Park", Morality.Neutral, nEffect, 2, 10, 19, 0.75f, "");
		tokens.Add (child.Name, child);

		//Sciences

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Writing", Morality.Neutral, nEffect, 10, 0, 1, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "The Wheel", Morality.Neutral, nEffect, 10, 0, 2, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Metalworking", Morality.Neutral, nEffect, 10, 0, 3, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Horse & Cart", Morality.Neutral, nEffect, 40, 10, 6, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Engine", Morality.Neutral, nEffect, 40, 10, 7, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Flight", Morality.Neutral, nEffect, 40, 10, 8, 1f, "");
		tokens.Add (child.Name, child);

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Farming", Morality.Neutral, nEffect, 40, 10, 9, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Abacus", Morality.Neutral, nEffect, 40, 10, 10, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		nEffect = ScriptableObject.CreateInstance<BasicScienceIncreaserEffect>();
		nEffect.setData (new float[]{1f});
		child = new Token (Type.Science, "Archeology", Morality.Neutral, nEffect, 40, 10, 11, 1f, "");
		tokens.Add (child.Name, child);
		child.Unlocked = true;

		//Inheritance
		Token par; 
		if (tokens.TryGetValue ("Horse & Cart", out child) && tokens.TryGetValue ("The Wheel", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Engine", out child) && tokens.TryGetValue ("Horse & Cart", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Flight", out child) && tokens.TryGetValue ("Engine", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Tax Office", out child) && tokens.TryGetValue ("Writing", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Tourism Board", out child) && tokens.TryGetValue ("The Wheel", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("General Store", out child) && tokens.TryGetValue ("Metalworking", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Bus Station", out child) && tokens.TryGetValue ("Horse & Cart", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Railway", out child) && tokens.TryGetValue ("Engine", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Farms", out child) && tokens.TryGetValue ("Farming", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("Airport", out child) && tokens.TryGetValue ("Flight", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("A Front", out child) && tokens.TryGetValue ("Abacus", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}
		if (tokens.TryGetValue ("A Park", out child) && tokens.TryGetValue ("Archeology", out par)) {child.setParent (par);} else {Debug.Log ("ERR");}

	}

	public static CityGenerator GetBlock(int mx, int my){
		if(Blocks != null){
			return Blocks [(mx * width * 2) + my ];
		}
		return null;
	}

	public Mesh GetAppropriateMesh(float height){
		if(height > 20){
			return LargeProperties [Random.Range(0, LargeProperties.Length)];
		} else if(height > 10){
			return MidSizedProperties [Random.Range(0, MidSizedProperties.Length)];
		} else if(height > 0){
			return SmallProperties [Random.Range(0, SmallProperties.Length)];
		}  
		return null;
	}

	public float GetAppropriateHeight(float height){
		if(height > 20){
			return 5f;
		} else if(height > 10){
			return 5f;
		} else if(height > 0){
			return 5f;
		}  
		return 0f;
	}

	public int GetState(float height){
		if(height > 20){
			return 3;
		} else if(height > 10){
			return 2;
		} else if(height > 0){
			return 1;
		}  
		return 0;
	}
}
