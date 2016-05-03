using UnityEngine;
using System.Collections;

public class WeaponHinge : MonoBehaviour
{
	private FireWeapon fireing;		// Referenz auf das Schießen-Skript
	private Transform weapon;		// Attribut dass das Waffen-GameObject speichert

	public Transform Weapon
	{ get { return weapon; } }

	void Start()
	{
		// Initialisierungen
		fireing = GetComponentInParent<FireWeapon>();
	}

	void Update()
	{
		// ---- Testzeile ----
		if (Input.GetKeyDown(KeyCode.Q)) DropWeapon();
		// -------------------
	}

	//Methode die vom Spielercharackter aufgreufen wird wenn er eine Waffe aufnimmt
	public void DrawWeapon(Transform weapon)
	{
		// Aktuelle Waffe löschen
		if (transform.childCount > 0) DropWeapon();

		// Waffe aufheben
		this.weapon = weapon;								// Referenz auf neue Waffe
		this.weapon.SetParent(transform);					// Waffe an den Waffenanker hängen
		this.weapon.localPosition = new Vector3();			// Am Anker positionieren
		this.weapon.localRotation = Quaternion.identity;	// Am Anker rotieren

		// Schießenskript mitteilen mit welcher Waffe geschossen wird
		fireing.MyWeapon = this.weapon.GetComponent<Weapon>();
	}

	public void DropWeapon()
	{
		if (weapon == null) return;							// Fehler abfangen
		weapon.SetParent(null);								// Waffe vom Waffenanker lösen
		weapon.GetComponent<CollectableWeapon>().Drop();	// Waffe fallen lassen
		weapon = null;										// Referenz auf die Waffe löschen
		fireing.MyWeapon = null;							// Referenz auf die Waffe löschen
	}

	public void DeleteWeapon()
	{
		if (weapon == null) return; 		// Fehler abfangen
		GameObject.Destroy(weapon);			// Waffe löschen
		weapon = null;						// Referenz auf die Waffe löschen
		fireing.MyWeapon = null;			// Referenz auf die Waffe löschen
	}
}
