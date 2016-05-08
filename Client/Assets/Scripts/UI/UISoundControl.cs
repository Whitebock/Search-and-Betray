using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler

public class UISoundControl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler {

	public AudioSource menuFX;
	public AudioClip hover,click;


	public void OnPointerEnter (PointerEventData eventData)
	{
		if (hover != null) 
		{
			menuFX.clip = hover;
			menuFX.Play();
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
