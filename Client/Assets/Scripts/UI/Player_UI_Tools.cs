using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player_UI_Tools : MonoBehaviour
{
	/*
	 * Dieses Skript arbeitet im Verbund mit einer "Event Trigger (Script)"-Componente, das auf dem gleichen Objekt liegt.
	 * Diese Componente setzt die Variable -hovers- via -SetHover(bool hovers)-, welche darstellt ob der Spieler angeguckt wird oder nicht.
	 * Alle Objekte die als Referenz in -affectedUIs- abgelegt sind, werden dem entsprechend ein- bzw. ausgeblendet.
	 */

	public OnlinePlayerUI_Manager manager;	// Liefert zusätzliche Informationen ob die UI eingeblendet werden soll oder nicht.
    public float maxDistance = 50f;			// Alles, was weiter entfernt ist, wird nicht mehr angezeigt.
	public bool hideAtStart = true;			// Ob die UI zu beginn aus oder ingeblendet werden soll.
	public CanvasRenderer[] affectedUIs;	// Alle UI-Felder die von diesem Skript gesteuert werden sollen.

	private bool hovers = false;
	private bool toggle = false;

	void Start()
	{
		// Initialisierungen
		if (manager == null)
		{
			Debug.Log("No \"OnlinePlayerUI_Manager\" found!");
			GameObject.Destroy(this);
		}
		if (affectedUIs.Length == 0)
		{
			Debug.Log("No attached UI's found for \"OnlinePlayerUI_Manager\"!");
			GameObject.Destroy(this);
		}

		// Ausblenden der UI zu beginn (Direktes Ausblenden funktioniert irgendwie nicht :/).
		if (hideAtStart) Invoke("Hide", 0.001f);
	}

	void Update()
	{
		// UI ein- und ausblenden
		if (toggle && !(manager.Visible && hovers))
		{
			Hide();
			toggle = false;
		}
		else if (!toggle && manager.Visible && hovers)
		{
			Show();
			toggle = true;
		}
	}

	public void SetHover(bool hovers)
	{
		this.hovers = hovers;
	}

    private void Show()
    {
		// UI einblenden
		if (manager.Visible && manager.Difference.magnitude <= maxDistance)
        {
			foreach (CanvasRenderer item in affectedUIs) item.cull = false;
		}
    }

    private void Hide()
	{
		// UI ausblenden
		foreach (CanvasRenderer item in affectedUIs) item.cull = true;
    }
}