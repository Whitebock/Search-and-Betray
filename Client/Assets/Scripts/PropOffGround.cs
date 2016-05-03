using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropOffGround : MonoBehaviour
{
	/*
	 * Diese Klasse kümmert sich um alles, was von der Map fallen kann (Rigidbodys).
	 * Was passieren soll wird nach tag's entschiden, unten in der Switchcase-Anweisung.
	 */

	public float minHeight = -10;			// Minimalhöhe nach der geprüft werden soll

	private List<Rigidbody> props;			// Liste aller props die behandelt werden (Child-Objekte)
	private bool notification;				// Weitere Notifications verhindern (Hilfsvariable)

	void Start()
	{
		// Initialisierungen
		props = new List<Rigidbody>(30);
		notification = false;

		// Schleife vorbereiten
		int i = 0;
		Rigidbody hilf = null;
		try { hilf = transform.GetChild(i).GetComponent<Rigidbody>(); } catch {};
		if (hilf == null) return;

		// Alle Child-Objekte mit einem Rigitbody als Referenz in -props- ablegen
		while(hilf != null)
		{
			props.Add(hilf);
			i++;
			try { hilf = transform.GetChild(i).GetComponent<Rigidbody>(); }
			catch { break; }
			if (hilf == null) break;
		}
	}

	void Update()
	{
		// jedes Prop durchgehen
		foreach(Rigidbody item in props)
		{
			// Nullreference abfangen
			if (item == null) continue;

			// Prop Positionen nach -minHeight- prüfen
			if (item.position.y < minHeight)
			{
				switch(item.tag)
				{
					// Props mit diesen Tags werden gelöscht
					case "Collectable":
					case "Untagged":
						GameObject.Destroy(item.gameObject);
						break;

					// Props mit diesen Tags werden umgebracht
					case "Enemy":
					case "Player":
						item.gameObject.SetActive(false);
						break;

					// Benachrichtigung für unbehandelte tags (Nur einmalig)
					default:
						if (notification) break;
						notification = true;
						Debug.LogWarning("PROPMENAGER: Ein Prop hat die Minimalhöhe unterschritten! | name = " + item.name +
					          " | tag = " + item.tag + "\nVerlassen an position: " +
					          item.position.x + ", " + item.position.y + ", " + item.position.z);
						break;
				}
			}
		}
	}
}