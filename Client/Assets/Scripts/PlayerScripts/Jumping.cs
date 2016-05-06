using UnityEngine;
using System.Collections;

public class Jumping : MonoBehaviour
{
	public float jumpforce = 7f;					// Sprungkraft
	public bool canJump = false;					// Ob man Springen kann, egal ob man Bodenkantakt hat
	private float jumpFactor, jumpFactorIncrease;	// Zur Berechnung der Anlaufs

	void Start()
	{
		// Input einfangen
		PlayerInfo.On_Inp_Jump += Jump;

		// Initialisierung
		jumpFactor = 0.5f;
		jumpFactorIncrease = 0.01f;
	}

	void FixedUpdate()
	{
		// Geschwindigkeit berechnen (Ohne Y-Achse)
		float planeVel = Mathf.Sqrt(Mathf.Pow(PlayerInfo.Phy.velocity.x, 2) + Mathf.Pow(PlayerInfo.Phy.velocity.z, 2));

		// Anlaufboost für den Sprung raufzählen
		if (planeVel > 0.4f) jumpFactor += jumpFactorIncrease;
		else jumpFactor = 0f;
		jumpFactor = Mathf.Clamp(jumpFactor, 0.7f, 1f);
	}

	void Jump()
	{
		// Springen blockieren
		if (!PlayerInfo.IsGrounded && !canJump || PlayerInfo.IsCrouching || PlayerInfo.Unconscious) return;

		// Springen
		PlayerInfo.Phy.AddForce(0f, jumpforce * jumpFactor, 0f, ForceMode.VelocityChange);
	}

	public void OnDisable()
	{
		canJump = false;
	}
}