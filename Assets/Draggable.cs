using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private RectTransform rect;
	private Vector2 startPos;
	private CanvasGroup cg;
	private Transform parent, superparent;
	public bool Locked, Dragged;
	private Vector3 worldPos;

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
		startPos = rect.position;
		rect.localScale = new Vector3 (0.25f, 0.25f, 1f);
		cg.alpha = 0.5f;
		transform.parent = superparent;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		Dragged = true;
		if(!Locked){
			rect.position = startPos;
			rect.localScale = Vector3.one;
			cg.alpha = 1f;
			transform.parent = parent;
		}
		worldPos = Camera.main.ScreenToWorldPoint (rect.position);
	}
}