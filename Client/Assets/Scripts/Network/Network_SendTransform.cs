using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PropStatus
{
	/*
	 * In welchem Status sich das Objekt befindet
	 * "inactive"	Das Objekt bewegt sich nicht
	 * "active"		Das Objekt wird vom Client (vom Spieler) bewegt und sendet an alle anderen seine Positionsdaten
	 * "passive"	Das Objekt wird von einem OnlineSpieler (vom Netzwerk) bewegt
	 * 				und synchronisiert seine Positionsdaten mit den empfangenen daten.
	*/
	active = 1, passive, inactive
};
public enum PropMode
{
	/*
	 * Der Modus bestimmt in welchem Status das Objekt kann
	 * "auto" 		Der Status wird von diesem Skript gesteuert und ist damit variabel.
	 * "inactive"	Die Position des Objekts wird nicht über das Netzwerk synchronisiert.
	 * "active"		Positionsdaten werden ständig übermittelt und keine empfangen.
	 * 				(VORSICHT: Wenn mehr als ein Objekt mit der gleichen ID sendet,
	 * 				führt das dazu, dass alle Empfänger versuchen an allen Stellen gleichzeitig zu sein)
	 * "passive"	Ist unbeweglich und empfäng ständig Positionsdaten über das Netzwerk.
	 */
	auto = 0, active, passive, inactive
};

public class Network_SendTransform : MonoBehaviour
{
	/*
	 * Dieses Skript sorgt dafür, das alle bweglichen Objekte im Level über das Netzwerk synchronisiert werden.
	 * Damit bleiben sie für jeden Spieler an der gleichen Position, auch wenn sie von einem Spieler bewegt werden.
	 * Dafür werden Objekt-IDs verwendet. Damit weiß das Netzwerk welche Objekt synchronisiert werden sollen.
	 * Sie kann entweder manuell gesetzt werden (mit "initialObjectID") oder die ID wird vom Netzwerk vergeben, sollte
	 * keine gültige ID (wie z. B. "0" oder "-1" etc.) vergeben worden sein.
	 */

	public int initialObjectID = 0;					// Voreingestellte Objekt-ID mit der der Server das Objekt identifizieren soll
	public float triggerActiveState = 0.001f;		// Ab welcher Differenz in den Aktiv-Status geschaltet wird
	public PropMode mode = PropMode.auto;			// In welchem Modus das Objekt über das Netzwerk kommunizieren soll
	private int objectID = 0;						// Tatsächliche Objekt-ID
	public PropStatus status;						// Ob das Objekt sendet, empfängt oder inaktiv ist
	private Rigidbody phy;							// Referenz auf die Physik des Objektes
	private Vector3 lastPosition, lastRotation;
	public static List<Network_SendTransform> allProps = new List<Network_SendTransform>(10);

	public delegate void Del_Network_DataRecieved();
	public event Del_Network_DataRecieved On_Network_DataRecieved;

	public PropStatus Status
	{
		get { return status; }
		set
		{
			// Wenn eingestellt (mit "mode"), ein Umschalten des Statuses verhindern
			if (mode == PropMode.auto)
			{
				// Phsikalische Eigenschaften des Obejkts, je nach Status, aktivieren und deaktivieren
				switch (value)
				{
				case PropStatus.active:
					phy.isKinematic = false;
					break;
				case PropStatus.passive:
					phy.isKinematic = true;
					break;
				case PropStatus.inactive:
					phy.isKinematic = false;
					break;
				}
				status = value;
			}
			else status = (PropStatus)mode;
		}
	}
	public PropMode Mode
	{
		get { return mode; }
		set { mode = value; if (mode != PropMode.auto) status = (PropStatus)mode; }
	}

	public int ObjectID { get { return objectID; } }

	void Start()
	{
		// Initialisierungen
		phy = GetComponent<Rigidbody>();
		Status = PropStatus.inactive; 		// Initialen Status des Objekts setzen
		enabled = false;

		// ObjektID setzen
		if (initialObjectID < 1)
		{
			Debug.LogError("[SERVER] The NetworkID-System is not implemented yet!\nSet the object-ID manualy.");
			//Debug.LogError("[SERVER] Object \"" + transform.name + "\" recieved an invalid ID \"" + ObjectID + "\" !");
			return;
		}
		else ConnectToNetwork(initialObjectID);
	}

	public void ConnectToNetwork(int id)
	{
		if (id < 1)
		{
			Debug.LogError("[SERVER] Object \"" + transform.name + "\" recieved an invalid ID \"" + id + "\" !");
			return;
		}
		//Debug.Log("[SERVER] Object \"" + transform.name + "\" with ID \"" + id + "\" connected to the Server.");
		objectID = id;
		enabled = true;
		Network_SendTransform.allProps.Add(this);
	}

	public void DisconnectFromNetwork()
	{
		//Debug.Log("[SERVER] Object \"" + transform.name + "\" with ID \"" + ObjectID + "\" disconnected from the Server.\nThis Object will not be visible for the other online players.");
		enabled = false;
		objectID = 0;
		Network_SendTransform.allProps.Remove(this);
	}

	void FixedUpdate()
	{
		// Positionsdaten senden wenn das Objekt im Aktiv-Status ist
		if (status == PropStatus.active)
		{
			// ------------------ Simulierte Netzwerkschnittstelle ------------------
			// TODO: Position + Rotation senden
			// ----------------------------------------------------------------------
		}

		// Überprüfen ob sich das Objekt bewegt hat
		if (Mathf.Abs((transform.position - lastPosition).magnitude) > triggerActiveState ||
			Mathf.Abs((transform.rotation.eulerAngles - lastRotation).magnitude) > triggerActiveState)
		{
			// Den Status auf Aktiv setzen, wenn das Objekt nicht vom Netzwerk bewegt wird
			if (status == PropStatus.inactive) Status = PropStatus.active;
		}
		// Den Status auf Inaktiv setzen, wenn das Objekt sich nicht mehr bewegt
		else Status = PropStatus.inactive;

		// Letzte synchronisierte Position merken.
		// Damit wird im nächsten Durchgang überprüft ob sich das Objekt bewegt hat.
		if (status != PropStatus.inactive)
		{
			lastPosition = transform.position;
			lastRotation = transform.rotation.eulerAngles;
		}
	}

	// ------------------ Simulierte Netzwerkschnittstelle ------------------
	void GetPositionData()
	{
		// --------------- !!! WICHTIG !!! ---------------
		if (On_Network_DataRecieved != null && status != PropStatus.passive) On_Network_DataRecieved();
		Status = PropStatus.passive;
		if (status != PropStatus.passive) return;
		// -----------------------------------------------
	}
	// ----------------------------------------------------------------------

	void OnDestroy()
	{
		DisconnectFromNetwork();
	}

	public static Network_SendTransform GetItem(int id)
	{
		// Alle Child-Objekte mit einem Network_SendTransform als Referenz durchsuchen
		foreach (Network_SendTransform item in allProps) if (item.ObjectID == id) return item;
		return null;
	}
}