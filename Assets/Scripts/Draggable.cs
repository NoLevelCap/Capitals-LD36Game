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

	public void Awake()
	{
		rect = GetComponent<RectTransform>();
		cg = GetComponent<CanvasGroup> ();
		parent = rect.parent;
		superparent = parent.parent;

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
		}
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(!Locked){
			startPos = rect.position;
			rect.localScale = new Vector3 (0.25f, 0.25f, 1f);
			cg.alpha = 0.5f;
			transform.parent = superparent;
			GameManager.TokenSelection = gameObject;
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		//Locked Check

		if(GameManager.BlockSelection != null){
			Debug.Log (GameManager.BlockSelection.TowerHealth);
			worldPos = GameManager.BlockSelection.transform.position + new Vector3(0, (GameManager.BlockSelection.TowerHealth*0.1f) + 0.5f );
			Locked = true;
			if(GameManager.BlockSelection.ActiveToken != null){
				Destroy (GameManager.BlockSelection.ActiveToken.gameObject);
			}
			GameManager.BlockSelection.ActiveToken = this;
		}

		Dragged = true;
		if (!Locked) {
			rect.position = startPos;
			rect.localScale = Vector3.one;
			cg.alpha = 1f;
			transform.parent = parent;
		} else {
			rect.localScale = new Vector3 (0.15f, 0.15f, 1f);
		}

		GameManager.TokenSelection = null;
	}
}