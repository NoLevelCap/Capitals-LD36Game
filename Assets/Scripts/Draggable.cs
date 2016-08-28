using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
			Duration = transform.Find ("Info").FindChild ("Duration").GetChild (0).GetChild (0).GetComponent<Text> ();
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
			transform.parent = board;
			GameManager.TokenSelection = gameObject;
		}
	}

	public void SetIcons(){
		int[] types = token.Effect.GetIconTypes ();
		Transform IconPanel = transform.Find ("Info").GetChild (0);
		foreach (int icon in types) {
			GameObject gm = Instantiate<GameObject> (GameManager.instance.Icon);
			gm.transform.SetParent (IconPanel);
			gm.transform.GetChild (0).GetComponent<Image> ().sprite = GameManager.instance.TokenIcons [icon];
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		//Locked Check
		if (!Locked) {
			if (GameManager.BlockSelection != null && GameManager.BlockSelection.ActiveToken == null) {
				worldPos = GameManager.BlockSelection.transform.position + new Vector3 (0, (GameManager.BlockSelection.TowerHealth * 0.1f) + 0.5f);
				Locked = true;
				ExpDate = GameManager.Day+token.Lifespan;
				GameManager.BlockSelection.ActiveToken = this;
				GameManager.BlockSelection.ActiveEffect = Instantiate<AEffect> (token.Effect);
				GameManager.BlockSelection.ActiveEffect.SetParent (token);
				GameManager.BlockSelection.ActiveEffect.OnDown(GameManager.BlockSelection);
				GameManager.TokenPlayed = true;
				UpdateDuration ();
				Dragged = true;
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
}