using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
	/*
	 * Ein Raycast trifft auf einer Hitbox mit diesem Skript und ruft die funktion "TakeDamage" mit dem entsprechenden
	 * Schaden auf. Das Skript berechnet den tatsächlichen Schaden.
	 * Dabei werden Panzerung und erhöter Schaden bei empfindlichen Körperregionen (z. B. Kopf) beachtet.
	 */

	private OnlinePlayerInfo attachedPlayer;	// Spieler der den Schaden bekommt
	private int armor = 0;						// Panzerung die vor der Lebensenergie abgezogen wird
	public float factor = 1f;					// Damit wird der Schaden multipliziert (z. B. für Kopfschüsse)

	public int Armor
	{
		get { return armor; }
		set
		{
			if (value > 0) armor = value;
			else armor = 0;
		}
	}

	void Start()
	{
		// Initialisierungen
		attachedPlayer = transform.GetComponentInParent<OnlinePlayerInfo>();
	}

	public void TakeDamage(int hitpoints)
	{
		// Eventuellen Schutz vom Damage abziehen
		hitpoints -= Armor;
		Armor -= hitpoints;
		if (hitpoints <= 0) return;

		// Hier Muss der Damage ins Netzwerk gesendet werden + wer ihn bekommt (attachedPlayer.PlayerID)
		//DEMAGE = (int)Mathf.Floor(hitpoints * factor);
		Debug.Log("Damage: " + (int)Mathf.Floor(hitpoints * factor));
	}
}