using UnityEngine;
using System.Collections;

public class OnlinePlayerWeaponHinge : MonoBehaviour
{
	private OnlinePlayerInfo onlinePlayer;		// Referenz auf diesen Online Spieler
	private CollectableWeapon weapon;			// Attribut dass das Waffen-GameObject speichert
	private Transform propsManager;				// Parrent-Objekt des Items, wenn es noch nicht aufgesammelt wurde
	private Network_SendTransform network;		// Referenz auf die Netzwerkkomponente des Objektes
	private OnlinePlayerFireWeapon fireWeapon;	// Referenz auf das Schießen-Skript

	public CollectableWeapon Weapon
	{ get { return weapon; } }

	void Start()
	{
		// Initialisierungen
		try { propsManager = GameObject.Find("PropsManager").transform; } catch {}
		onlinePlayer = GetComponentInParent<OnlinePlayerInfo>();
		fireWeapon = GetComponent<OnlinePlayerFireWeapon>();

		// ---------------------- Netzwerkschnittstelle ----------------------
		Netzwerk_Simulator.NetzwerkStream += Stream;
		// -------------------------------------------------------------------
	}

	// Methode die vom Spielercharackter aufgreufen wird wenn er eine Waffe aufnimmt
	public void DrawWeapon(CollectableWeapon weapon)
	{
		if (!weapon || GameObject.ReferenceEquals(this.weapon, weapon)) return;

		// Aktuelle Waffe löschen
		DropWeapon();

		// Waffe aufheben
		this.weapon = weapon;
		this.weapon.PickUp();

		network = this.weapon.GetComponent<Network_SendTransform>(); 	// Referenz auf die Netzwerkkomponente merken
		network.Mode = PropMode.inactive; 								// Waffenposition nicht mehr über das Netzwerk senden
		network.On_Network_DataRecieved += DropWeapon;					// Sobald Netzwerkdaten ankommen, soll die Waffe wieder fallen gelassen werden.

		this.weapon.transform.SetParent(transform);						// Waffe an den Waffenanker hängen
		this.weapon.transform.localPosition = new Vector3();			// Am Anker positionieren
		this.weapon.transform.localRotation = Quaternion.identity;		// Am Anker rotieren
		fireWeapon.MyWeapon = this.weapon.GetComponent<Weapon>();		// Dem Schießen-Skript die Waffe übergeben
	}

	public void DropWeapon()
	{
		if (fireWeapon != null) fireWeapon.MyWeapon = null;
		if (weapon != null)
		{
			weapon.transform.SetParent(propsManager);	// Waffe vom Waffenanker lösen
			weapon.Drop();								// Waffe fallen lassen
			weapon = null;								// Referenz auf die Waffe löschen
		}
		if (network != null)
		{
			network.Mode = PropMode.auto;					// Waffenposition wieder vom Netzwerk verarbeiten lassen
			network.On_Network_DataRecieved -= DropWeapon;
			network = null;									// Referenz auf die Netzwerkkomponente löschen
		}
	}

	// ---------------------- Netzwerkschnittstelle ----------------------
	void Stream(int sender, int empfaenger, PackageType typ, string info)
	{
		if (empfaenger != onlinePlayer.PlayerID || typ != PackageType.pickedUp) return;

		string[] msg = info.Split(new char[]{ ',' });
		int[] hilf = new int[msg.Length];
		for(int i = 0; i < msg.Length; i++) int.TryParse(msg[i], out hilf[i]);

		Network_SendTransform x = Network_SendTransform.GetItem(hilf[0]);
		if (x != null) DrawWeapon(x.transform.GetComponent<CollectableWeapon>());
	}
	void OnDestroy()
	{
		Netzwerk_Simulator.NetzwerkStream -= Stream;
	}
	// -------------------------------------------------------------------
}