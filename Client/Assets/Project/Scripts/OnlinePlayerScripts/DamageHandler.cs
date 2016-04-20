using UnityEngine;
using System.Collections;

public class DamageHandler : MonoBehaviour
{
	/*
	 * Ein Raycast trifft auf einer Hitbox mit diesem Skript und ruft die funktion "TakeDemage" mit dem entsprechenden
	 * Schaden auf. Das Skript berechnet den tatsächlichen Schaden.
	 * Dabei werden Panzerung und erhöter Schaden bei empfindlichen Körperregionen (z. B. Kopf) beachtet.
	 */

	private OnlinePlayerInfo attachedPlayer;	// Spieler der den Schaden bekommt
	private int armor = 0;						// Panzerung die vor der Lebensenergie abgezogen wird
	public float factor = 1f;					// Damit wird der Schaden multipliziert (z. B. für Kopfschüsse)

	public int Armor
	{ get { return armor; } set { armor = value; } }

	void Start()
	{
		// Initialisierungen
		attachedPlayer = transform.GetComponentInParent<OnlinePlayerInfo>();
	}

	public void TakeDamage(int hitpoints, int myPlayerID)
	{
		hitpoints -= armor;

        int damage = (int)Mathf.Floor(hitpoints * factor); //Berechnung des Schadens
        // Hier Muss der Demage ins Netzwerk gesendet werden + wer ihn bekommt (attachedPlayer.PlayerID)
        //DEMAGE = (int)Mathf.Floor(hitpoints * factor);
        Debug.Log(attachedPlayer.PlayerID + " got hit by" + myPlayerID + " and recevied " + damage + " damage!"); //Da momentan noch kein Server existiert hier die Ausgabe des Schadens
		armor = 0;
	}
}