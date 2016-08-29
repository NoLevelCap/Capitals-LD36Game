using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public RectTransform rect;
	private Vector2 startPos;
	private CanvasGroup cg;
	public Transform parent, board;
	public bool Locked, Dragged;
	public Vector3 worldPos;

	private Text Duration;

	public Token token;
	private AEffect activeeffect;
	public Image Icon;

	public int ExpDate;

	public void Awake()
	{
		rect = GetComponent<RectTransform>();
		cg = GetComponent<CanvasGroup> ();
		Icon = transform.GetChild (0).GetChild(0).GetComponent<Image> ();
		board = GameObject.Find ("BoardTokens").transform;
		parent = GameObject.Find ("TokenManager").transform;
	}

	public void OnDrag(PointerEventData eventData)
	{
		PointerEventData pointerData = eventData;
		if (pointerData == null) { return; }

		rect.position = Input.mousePosition;
	}

	public void Update(){
		if(Locked && Dragged){
			rect.position = Camera.main.WorldToScreenPoint (worldPos);
			float Mag = Vector3.Magnitude (Camera.main.transform.position - worldPos);
			rect.localScale = (Vector3.one / Mag) * 7.5f;
			if (Camera.main.WorldToViewportPoint (worldPos).z < 0) {
				Hide ();
			} else {
				Show ();
			}
			int top = activeeffect.getOutput ().Length;
			foreach (Transform item in transform.FindChild ("Floaters")) {
				Destroy (item.gameObject);
			}
			for (int i = 0; i < top; i++) {
				GameObject text = Instantiate <GameObject> (GameManager.instance.Floater);
				text.transform.SetParent (transform.FindChild ("Floaters"), false);
				text.name = "Floater";
				text.GetComponent<Text> ().text = activeeffect.getOutput()[i];
			}


		}
	}

	public void Hide(){
		cg.alpha = 0f;
	}

	public void Show(){
		cg.alpha = 0.75f;
	}

	public void UpdateDuration(){
		if (Duration == null) {
			Duration = transform.Find ("Info").FindChild ("Duration").GetChild (0).GetChild (1).GetComponent<Text> ();
		} 
		if (Locked && Dragged) {
			Duration.text = ((ExpDate - GameManager.Day) + 1) + "";
		} else {
			Duration.text = ((token.Lifespan) + 1) + "";
		}


	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(!Locked){
			
			startPos = rect.position;
			rect.localScale = new Vector3 (0.35f, 0.35f, 1f);
			cg.alpha = 0.25f;
			transform.SetParent(board, false);
			GameManager.TokenSelection = gameObject;
		}
	}

	public void SetIcons(){
		int[] types = token.Effect.GetIconTypes ();
		Transform IconPanel = transform.Find ("Info").GetChild (0);
		foreach (int icon in types) {
			GameObject gm = Instantiate<GameObject> (GameManager.instance.Icon);
			gm.transform.SetParent (IconPanel, false);
			gm.transform.GetChild(0).GetComponent<Image> ().sprite = GameManager.instance.EffectIcons [icon];
		}

		if (token.Effect.getCost () > 0) {
			transform.FindChild ("CostPanel").GetComponentInChildren<Text> ().text = token.Effect.getCost () + "";
		} else {
			transform.FindChild ("CostPanel").gameObject.SetActive (false);
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		//Locked Check
		if (!Locked) {
			if (GameManager.BlockSelection != null ){
				if (GameManager.instance.ChargeACost (token.Name, token.Effect.getCost ())) {
					if (GameManager.BlockSelection.ActiveToken == null || token.Effect.getType () == 1) {
						if (token.Effect.getType () == 1 && GameManager.BlockSelection.ActiveToken != null) {
							GameManager.BlockSelection.ActiveToken.ForceDestroy ();
						}
								
						worldPos = GameManager.BlockSelection.transform.position + new Vector3 (0, (GameManager.BlockSelection.TowerHealth * 0.1f) + 3.5f);
						Locked = true;
						ExpDate = GameManager.Day + token.Lifespan;
						GameManager.BlockSelection.ActiveToken = this;
						GameManager.BlockSelection.ActiveEffect = Instantiate<AEffect> (token.Effect);
						activeeffect = GameManager.BlockSelection.ActiveEffect;
						GameManager.BlockSelection.ActiveEffect.SetParent (token);
						GameManager.BlockSelection.ActiveEffect.setData (token.Effect.getData ());
						GameManager.BlockSelection.ActiveEffect.OnDown (GameManager.BlockSelection);
						GameManager.TokenPlayed = true;
						UpdateDuration ();
						Dragged = true;

						transform.FindChild ("Floaters").localPosition = new Vector3 (0, 50, 0);
						for(int i=0; i<5; i++) {
							GameObject text = Instantiate <GameObject> (GameManager.instance.Floater);
							text.transform.SetParent (transform.FindChild ("Floaters"), false);
							text.name = "Floater";
						}
					} else {
						GameManager.instance.Alert.text = "<color=red>That block has a token.</color>";
						GameManager.instance.Alert.CrossFadeAlpha (1f, 0.2f, true);
					}
				} else {
					GameManager.instance.Alert.text = "<color=red>You can not afford that token.</color>";
					GameManager.instance.Alert.CrossFadeAlpha (1f, 0.2f, true);
				}
			}

			
			if (!Locked) {
				rect.position = startPos;
				rect.localScale = Vector3.one;
				cg.alpha = 1f;
				transform.parent = parent;
			} else {
				rect.localScale = new Vector3 (0.25f, 0.25f, 1f);
				cg.alpha = 0.75f;
			}

			GameManager.TokenSelection = null;
		}
	}

	public void CheckForDestroy(){
		if(GameManager.Day > ExpDate){
			Destroy (gameObject);
		}
	}

	public void ForceDestroy(){
		Destroy (gameObject);
	}
}