using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	/*
	 * "W, A, S, D"-Steuerung wird als Bewegung umgesetzt.
	 */

	private float inp_horizontal, inp_vertical;										// Hilfsvariablen (nicht tatsächlicher Input)
	public float speed_horizontal = 5f, speed_vertical = 5f, crouchFactor = 0.6f;	// Faktoren zum Anpassen der Geschwindigkeit 
	private float speed_horizontal_air = 5f, speed_vertical_air = 5f;				// Geschwindigkeit in der Luft

	void Update()
	{
		// Input registrieren
		inp_horizontal = PlayerInfo.Inp_horizontal;
		inp_vertical = PlayerInfo.Inp_vertical;

		// Diagonales Bewegen
		if (inp_horizontal != 0 && inp_vertical != 0)
		{
			inp_horizontal *= 0.7071f;
			inp_vertical *= 0.7071f;
		}
	}

	void FixedUpdate()
	{
		// Bewegung blockieren
		if (PlayerInfo.Unconscious) return;

		// Bewegung
		if (PlayerInfo.IsGrounded)
		{
			// Geduckte geschwindigkeit
			if (PlayerInfo.IsCrouching)
			{
				inp_horizontal *= crouchFactor;
				inp_vertical *= crouchFactor;
			}

			// Am Bodem
			PlayerInfo.Phy.velocity = new Vector3(inp_horizontal * speed_horizontal, PlayerInfo.Phy.velocity.y, PlayerInfo.Phy.velocity.z);
			PlayerInfo.Phy.velocity = transform.localToWorldMatrix * new Vector3(PlayerInfo.Phy.velocity.x, PlayerInfo.Phy.velocity.y, inp_vertical * speed_vertical);
		}
		else
		{
			// in der Luft
			PlayerInfo.Phy.AddRelativeForce(inp_horizontal * speed_horizontal_air, 0f, 0f, ForceMode.Acceleration);
			PlayerInfo.Phy.AddRelativeForce(0f, 0f, inp_vertical * speed_vertical_air, ForceMode.Acceleration);
			Vector2 hilf = Vector2.ClampMagnitude(new Vector2(PlayerInfo.Phy.velocity.x, PlayerInfo.Phy.velocity.z), speed_vertical);
			PlayerInfo.Phy.velocity = new Vector3(hilf.x, PlayerInfo.Phy.velocity.y, hilf.y);
		}
	}
}