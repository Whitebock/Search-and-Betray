using UnityEngine;
using System.Collections;

public class ShiftMovement : MonoBehaviour
{
	public Vector3 direction;
	public bool canJump = true;
	public float exitYVelocity = 0f;

	private Movement movement;
	private Jumping jumping;
	private int playerLayer;
	private bool ignoreEYV;

	void Start()
	{
		playerLayer = LayerMask.NameToLayer("Player");
		movement = GameObject.Find("Player").GetComponent<Movement>();
		jumping = movement.GetComponent<Jumping>();
		PlayerInfo.On_Player_Disabled += PlayerDisabled;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != playerLayer) return;
		movement.ShiftMovement(transform, direction);
		jumping.canJump = canJump;
		PlayerInfo.On_Player_isGrounded += PlayerGrounded;
		PlayerInfo.On_Inp_Jump += PlayerJumpedOff;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer != playerLayer) return;
		Vector3 exitVelocity = PlayerInfo.Phy.velocity;
		if (!ignoreEYV) exitVelocity.y *= exitYVelocity;
		movement.NormalMovement(exitVelocity);
		jumping.canJump = false;
		PlayerInfo.On_Player_isGrounded -= PlayerGrounded;
		PlayerInfo.On_Inp_Jump -= PlayerJumpedOff;
	}

	void PlayerGrounded(bool grounded)
	{
		if (!grounded) return;
		movement.NormalMovement();
		jumping.canJump = false;
	}

	void PlayerJumpedOff()
	{
		if (!canJump) return;
		movement.NormalMovement();
		jumping.canJump = false;
		ignoreEYV = true;
	}

	void PlayerDisabled()
	{
		PlayerInfo.On_Player_isGrounded -= PlayerGrounded;
		PlayerInfo.On_Inp_Jump -= PlayerJumpedOff;
	}

	void OnDisabled()
	{
		PlayerInfo.On_Player_Disabled -= PlayerDisabled;
		PlayerInfo.On_Player_isGrounded -= PlayerGrounded;
		PlayerInfo.On_Inp_Jump -= PlayerJumpedOff;
	}
}