using UnityEngine;
using System.Collections;

public class WeaponHinge : MonoBehaviour
{
	private FireWeapon fireing;		// Referenz auf das Schießen-Skript
	private Transform weapon;		// Attribut dass das Waffen-GameObject speichert
	private Transform propsManager;

	public Transform Weapon
	{ get { return weapon; } }

	void Start()
	{
		// Initialisierungen
		fireing = GetComponentInParent<FireWeapon>();
		try { propsManager = GameObject.Find("PropsManager").transform; } catch {}
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
		if (!weapon || GameObject.ReferenceEquals(this.weapon, weapon)) return;

		// Aktuelle Waffe löschen
		DropWeapon();

		// Waffe aufheben
		this.weapon = weapon;								// Referenz auf neue Waffe

		// Waffenposition nicht mehr über das Netzwerk senden
		Network_SendTransform network = this.weapon.GetComponent<Network_SendTransform>();
		network.Mode = PropMode.inactive;

		this.weapon.SetParent(transform);					// Waffe an den Waffenanker hängen
		this.weapon.localPosition = new Vector3();			// Am Anker positionieren
		this.weapon.localRotation = Quaternion.identity;	// Am Anker rotieren

		// Schießenskript mitteilen mit welcher Waffe geschossen wird
		fireing.MyWeapon = this.weapon.GetComponent<Weapon>();

		// ---------------------- Netzwerkschnittstelle ----------------------
		Netzwerk_Simulator.Senden(1, PlayerInfo.PlayerID, PackageType.pickedUp, network.ObjectID.ToString());
		// -------------------------------------------------------------------
	}

	public void DropWeapon()
	{
		if (weapon == null) return;											// Fehler abfangen
		weapon.SetParent(propsManager);										// Waffe vom Waffenanker lösen
		weapon.GetComponent<Network_SendTransform>().Mode = PropMode.auto;	// Waffenposition wieder vom Netzwerk verarbeiten lassen
		weapon.GetComponent<CollectableWeapon>().Drop();					// Waffe fallen lassen
		weapon = null;														// Referenz auf die Waffe löschen
		fireing.MyWeapon = null;											// Referenz auf die Waffe löschen
	}

	public void DeleteWeapon()
	{
		if (weapon == null) return; 		// Fehler abfangen
		GameObject.Destroy(weapon);			// Waffe löschen
		weapon = null;						// Referenz auf die Waffe löschen
		fireing.MyWeapon = null;			// Referenz auf die Waffe löschen
	}
}