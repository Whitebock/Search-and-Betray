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
    private static int playerID = 1;
	private static string playerName = "Test-Player";
	private static int lifeEnergy;

	private static Rigidbody phy;
	private static float inp_horizontal, inp_vertical;
	private static bool isGrounded, isCrouchingInp, isCrouching, crouchToggle, unconscious;
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
	public delegate void Del_Inp_Crouch(bool isCrouching);
	public static event Del_Inp_Crouch On_Inp_Crouch;
	public delegate void Del_Player_Disabled();
	public static event Del_Player_Disabled On_Player_Disabled;
	public delegate void Del_Player_isGrounded(bool isGrounded);
	public static event Del_Player_isGrounded On_Player_isGrounded;
    #endregion

    #region Properties
    public static int PlayerID { get; set; }										// Spieler-ID
	public static string PlayerName { get; set; }									// Spielername
	public static int LifeEnergy													// Leben
	{ get { return lifeEnergy; } set { if (value <= 100) lifeEnergy = value; } }
	public static Rigidbody Phy                                                     // Rigitbody des Spielers
    { get { return phy; } }
    public static float Inp_horizontal                                              // Input "Seitwärts"
    { get { return inp_horizontal; } }
    public static float Inp_vertical                                                // Input "Vorwärts/Rückwärts"
    { get { return inp_vertical; } }
    public static bool IsGrounded													// Hat bodenkontakt
	{
		get { return isGrounded; }
		set
		{
			// isGrounded-Event abfeuern
			if (On_Player_isGrounded != null && isGrounded != value) On_Player_isGrounded(value);
			isGrounded = value;
		}
	}
	public static bool CrouchToggle { get; set; } // Ob die Crouch-Taste nicht gedrück gehalten werden muss.

    public static bool IsCrouchingInp // Möchte geduckt sein.
	{
		get { return isCrouchingInp; }
		set
		{
			// isCrouching-Event abfeuern
			if (On_Inp_Crouch != null) On_Inp_Crouch(value);
			isCrouchingInp = value;
		}
	}
    public static bool IsCrouching               // Ist tatsächlich geduckt.
    {
        get { return isCrouching; }
        set
        {
            isCrouching = value;

            // Send crouch info.
            CCC_Client.Instance.SendCrouch(value);
        }
    }
    public static bool Unconscious { get; set; } // Bewegungen sollen blockiert werden.
    #endregion

    void Start()
	{
		// Initialisierungen
		phy = GetComponent<Rigidbody>();
		LifeEnergy = 100;
		isCrouchingInp = isCrouching = false;
    }

	void Update()
	{
		// Input registrieren
		inp_horizontal = Input.GetAxisRaw("Horizontal");
		inp_vertical = Input.GetAxisRaw("Vertical");

		// Springen Event
		if (Input.GetButtonDown("Jump") && On_Inp_Jump != null) On_Inp_Jump();

		// Ducken Event | Ducken gedrückt halten oder toggeln
		if (crouchToggle && Input.GetButtonDown("Crouch")) IsCrouchingInp = !isCrouchingInp;
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


        // --------------------- Netzwerkschnittstelle ---------------------

        Vector3 hilf = transform.worldToLocalMatrix * phy.velocity;

        CCC_Client.Instance.SendTransform(transform, hilf);

        // Granatenwurf senden
        //if (Input.GetButtonDown("Fire1")) Netzwerk_Simulator.Senden(playerID, -1, PackageType.Granade, "");

        // ------------------------------------------------------------------
    }

    void FixedUpdate()
	{
		// Bodenkontakt
		IsGrounded = Physics.Raycast(transform.position, Vector3.down, 1.05f);
	}

	// Todessequenz
	public void Kill()
	{
		gameObject.SetActive(false);
	}

	void OnDisabled()
	{
		// Inaktiv-Event abfeuern
		if (On_Player_Disabled != null) On_Player_Disabled();
	}

	// ------------------------- Netzwerk -------------------------

    public void Disconnect()
    {
        CCC_Client.Instance.Disconnect();
        SceneManager.LoadScene("MainMenu");
	}
	// ------------------------------------------------------------
}

/*/ ------------------ Simulierte Netzwerkschnittstelle ------------------
// Position senden
Netzwerk_Simulator.Senden(playerID, -1, PackageType.Position, transform.position.x + "," + transform.position.y + "," + transform.position.z);

// Geschwindigkeit (für den Animator) senden
Vector3 hilf = transform.worldToLocalMatrix * phy.velocity;
Netzwerk_Simulator.Senden(playerID, -1, PackageType.Velocity, hilf.x + "," + hilf.y + "," + hilf.z);

// Ausrichtung senden
Netzwerk_Simulator.Senden(playerID, -1, PackageType.Rotation, transform.rotation.x + "," + transform.rotation.y + "," + transform.rotation.z + "," + transform.rotation.w);

// Granatenwurf senden
if (Input.GetButtonDown("Fire1")) Netzwerk_Simulator.Senden(playerID, -1, PackageType.Granade, "");
// ----------------------------------------------------------------------*/

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