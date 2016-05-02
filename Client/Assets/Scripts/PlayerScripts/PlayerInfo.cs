using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    /*
	 * In dieser Klasse laufen alle Informationen, Events und Zustände des Spielers zusammen und werden statisch zur
	 * Verfügung gestellt. Physikalische Abfragen (z. B. -isGrounded-) und Spielerinput wird auch hier gehändelt.
	 * Alle Propertys (Setter) und -Update()- können für Netzwerkübertragungen genutzt werden.
	 */

    #region Felder
    private static int playerID;
	private static string playerName;
	private static int lifeEnergy;

	private static Rigidbody phy;
	private static float inp_horizontal, inp_vertical;
	private static bool isGrounded, isCrouchingInp, isCrouching, crouchToggle, unconscious;

    private CCC_Client client;
    #endregion

    #region Events
    /*
	 * Erklärung:
	 * Delegates fangen mit "Del_" an. Events mit "On_".
	 * Danach folg ein "Inp_" oder "Player_".
	 * 		- "Inp_" sind Events, die bei Button-Inputs gesendet werden, die andere Skripte dann auffangen.
	 * 		- "Player_" sind tatsächliche Bewegungen die der Spieler macht, auf die Skripte reagieren können. (Noch gibts keine)
	 */
    public delegate void Del_Inp_Jump();
	public static event Del_Inp_Jump On_Inp_Jump;
	public delegate void Del_Crouch(bool isCrouching);
	public static event Del_Crouch On_Inp_Crouch;
    #endregion

    #region Properties
    public static int PlayerID														// Spieler-ID
	{ get { return playerID; } set { playerID = value; } }
	public static string PlayerName													// Spielername
	{ get { return playerName; } set { playerName = value; } }
	public static int LifeEnergy													// Leben
	{ get { return lifeEnergy; } set { if (value <= 100) lifeEnergy = value; } }
	public static Rigidbody Phy														// Rigitbody des Spielers
	{ get { return phy; } }
	public static float Inp_horizontal												// Input "Seitwärts"
	{ get { return inp_horizontal; } }
	public static float Inp_vertical												// Input "Vorwärts/Rückwärts"
	{ get { return inp_vertical; } }
	public static bool IsGrounded													// Hat bodenkontakt
	{ get { return isGrounded; } }
	public static bool CrouchToggle													// Ob die Crouch-Taste nicht gedrück gehalten werden muss
	{ get { return crouchToggle; } set { crouchToggle = value; } }
	public static bool IsCrouchingInp												// Möchte geduckt sein
	{ get { return isCrouchingInp; } }
	public static bool IsCrouching													// Ist tatsächlich geduckt
	{ 
		get { return isCrouching; }
		set
		{
			isCrouching = value;
			// ------------------ Simulierte Netzwerkschnittstelle ------------------
			// Ob Ich geduckt bin senden
			if (value) Netzwerk_Simulator.Senden(playerID, -1, PackageType.Crouch, "true");
			else Netzwerk_Simulator.Senden(playerID, -1, PackageType.Crouch, "false");
			// ----------------------------------------------------------------------
		}
	}
	public static bool Unconscious													// Bewegungen sollen blockiert werden
	{ get { return unconscious; } set { unconscious = value; } }
    #endregion
    void Start()
	{
		// Initialisierungen
		phy = GetComponent<Rigidbody>();
		LifeEnergy = 100;
		isCrouchingInp = isCrouching = false;
        client = CCC_Client.CreateInstance();
	}

	void Update()
	{
		// Input registrieren
		inp_horizontal = Input.GetAxisRaw("Horizontal");
		inp_vertical = Input.GetAxisRaw("Vertical");

		// Springen Event
		if (Input.GetButtonDown("Jump") && On_Inp_Jump != null) On_Inp_Jump();

		// Ducken Event
		if (On_Inp_Crouch != null)
		{
			// Ducken gedrückt halten oder toggeln
			if (crouchToggle)
			{
				// Ducken toggeln
				if (Input.GetButtonDown("Crouch"))
				{
					if (isCrouchingInp)
					{
						On_Inp_Crouch(false);
						isCrouchingInp = false;
					}
					else
					{
						On_Inp_Crouch(true);
						isCrouchingInp = true;
					}
				}
			}
			else
			{
				// Ducken gedrückt halten
				if (Input.GetButtonDown("Crouch"))
				{
					isCrouchingInp = true;
					On_Inp_Crouch(true);
				}
				else if (Input.GetButtonUp("Crouch"))
				{
					isCrouchingInp = false;
					On_Inp_Crouch(false);
				}
			}
		}

        // ------------------ Netzwerkschnittstelle ------------------
        
		// Position und Velocity senden
		Vector3 hilf = transform.worldToLocalMatrix * phy.velocity;

        client.SendTransform(transform, hilf);

		// Granatenwurf senden
		//if (Input.GetButtonDown("Fire1")) Netzwerk_Simulator.Senden(playerID, -1, PackageType.Granade, "");

		// ----------------------------------------------------------------------
	}
	
	void FixedUpdate()
	{
		// Bodenkontakt
		isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.05f);
	}

    public void Disconnect()
    {
        client.Disconnect();
        SceneManager.LoadScene("Menu");
    }

	/*/ Bewustlosigkeit (Blockiert Bewegungen. Gut für AddForce.)
	public void ThempUnconsciousnes(float sec)
	{
		Unconscious = true;
		Invoke("SetUnconscious", sec);
	}
	void SetUnconscious()
	{
		Unconscious = false;
	}*/
}