using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler

public class UISoundControl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public AudioSource menuFX;
	public AudioClip hover,click;
	public bool invisibleWithoutHover;

	public void Awake()
	{
		if (invisibleWithoutHover) 
		{
			this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
		}
	}


	public void OnPointerEnter (PointerEventData eventData)
	{
		if (invisibleWithoutHover) 
		{
			this.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
		}

		if (hover != null) 
		{
			menuFX.clip = hover;
			menuFX.Play();
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (invisibleWithoutHover) 
		{
			this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (click != null) 
		{
			menuFX.clip = click;
			menuFX.Play();
		}
	}
}
