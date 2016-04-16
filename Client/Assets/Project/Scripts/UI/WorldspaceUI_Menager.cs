using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldspaceUI_Menager : MonoBehaviour
{
	/*
	 * Dieses Skript erzeugt eine UI für den Spieler, der, beim Aufruf der Methode "MakeUI(Transform newPlayer)",
	 * als Parameter übergeben wird. Auf dem Objekt muss das Skript "OnlinePlayerInfo" vorhanden sein. Aus diesem Skript
	 * holt sich die UI alle Informationen, die es Anzeigen wird (z. B. ".PlayerName").
	 */

	// UI-Prefab das erzeugt wird.
	public Transform onlinePlayerUI;

	public void MakeUI(Transform newPlayer)
	{
		if (onlinePlayerUI == null)
		{
			Debug.Log("Kein PlayerUI-Prefab gefunden!");
			return;
		}

		// UI erzeugen
		Transform newUI = GameObject.Instantiate(onlinePlayerUI);

		// UI diesem Objekt unterordnen (für Ordnung in der "Hirachie")
		newUI.SetParent(transform, false);

		// UI dem Spieler zuweisen, dessen Informationen angezeigt werden sollen.
		newUI.GetComponentInChildren<OnlinePlayerUI_Menager>().OnlinePlayer = newPlayer;
	}
}