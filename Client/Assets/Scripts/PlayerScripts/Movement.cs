using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
	/*
	 * "W, A, S, D"-Steuerung wird als Bewegung umgesetzt.
	 */

	public float speed_horizontal = 5f, speed_vertical = 5f, crouchFactor = 0.6f;	// Faktoren zum Anpassen der Geschwindigkeit
	private float inp_horizontal, inp_vertical;										// Hilfsvariablen (nicht tatsächlicher Input)
	private float speed_horizontal_air = 5f, speed_vertical_air = 5f;				// Geschwindigkeit in der Luft
	private Transform newMovement = null;											// Rotation der Bewegung (z. B. für Treppen und Leitern)

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
		if (PlayerInfo.IsGrounded || newMovement)
		{
			// Geduckte geschwindigkeit
			if (PlayerInfo.IsCrouching)
			{
				inp_horizontal *= crouchFactor;
				inp_vertical *= crouchFactor;
			}

			// Am Bodem
			Vector3 move = transform.localToWorldMatrix * new Vector3(inp_horizontal * speed_horizontal, 0f, inp_vertical * speed_vertical);
			if (newMovement) PlayerInfo.Phy.velocity = Quaternion.FromToRotation(transform.up, newMovement.up) * move;
			else PlayerInfo.Phy.velocity = new Vector3(0f, PlayerInfo.Phy.velocity.y, 0f) + move;

		}
		else
		{
			// In der Luft
			PlayerInfo.Phy.AddRelativeForce(inp_horizontal * speed_horizontal_air, 0f, inp_vertical * speed_vertical_air, ForceMode.Acceleration);
			Vector2 hilf = Vector2.ClampMagnitude(new Vector2(PlayerInfo.Phy.velocity.x, PlayerInfo.Phy.velocity.z), speed_vertical);
			PlayerInfo.Phy.velocity = new Vector3(hilf.x, PlayerInfo.Phy.velocity.y, hilf.y);
		}
	}

	void OnDisable()
	{
		NormalMovement();
	}

	public void NormalMovement()
	{
		try { Destroy(GetComponent<ConstantForce>()); } catch {}
		PlayerInfo.Phy.useGravity = true;
		newMovement = null;
	}
	public void NormalMovement(Vector3 vel)
	{
		try { Destroy(GetComponent<ConstantForce>()); } catch {}
		PlayerInfo.Phy.useGravity = true;
		newMovement = null;
		PlayerInfo.Phy.velocity = new Vector3(vel.x, vel.y, vel.z);
	}

	public void ShiftMovement(Transform newMovement, Vector3 gravityDirection)
	{
		// Schwerkraft verschieben um Abrutschen zu vermeiden
		PlayerInfo.Phy.useGravity = false;
		ConstantForce newGravity = gameObject.AddComponent<ConstantForce>();
		gravityDirection = (newMovement.localToWorldMatrix * gravityDirection).normalized;
		newGravity.force = new Vector3(gravityDirection.x * Physics.gravity.x, gravityDirection.y * Physics.gravity.y, gravityDirection.z * Physics.gravity.z);

		// Bewegung anpassen
		this.newMovement = newMovement;
	}
}