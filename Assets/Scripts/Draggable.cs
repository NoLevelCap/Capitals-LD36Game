using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public RectTransform rect;
	private Vector2 startPos;
	private CanvasGroup cg;
	private Transform parent, superparent;
	public bool Locked, Dragged;
	public Vector3 worldPos;

	public Token token;
	public Image Icon;

	public void Awake()
	{
		rect = GetComponent<RectTransform>();
		cg = GetComponent<CanvasGroup> ();
		Icon = transform.GetChild (0).GetChild(0).GetComponent<Image> ();

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

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(!Locked){
			parent = rect.parent;
			superparent = parent.parent;
			startPos = rect.position;
			rect.localScale = new Vector3 (0.35f, 0.35f, 1f);
			cg.alpha = 0.25f;
			transform.parent = superparent;
			GameManager.TokenSelection = gameObject;
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		//Locked Check
		if (!Locked) {
			if (GameManager.BlockSelection != null) {
				worldPos = GameManager.BlockSelection.transform.position + new Vector3 (0, (GameManager.BlockSelection.TowerHealth * 0.1f) + 0.5f);
				Locked = true;
				if (GameManager.BlockSelection.ActiveToken != null) {
					GameManager.BlockSelection.ActiveEffect.OnDestoy (GameManager.BlockSelection);
					Destroy (GameManager.BlockSelection.ActiveToken.gameObject);
				}
				GameManager.BlockSelection.ActiveToken = this;
				GameManager.BlockSelection.ActiveEffect = Instantiate<AEffect> (token.Effect);
				GameManager.TokenPlayed = true;
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
}