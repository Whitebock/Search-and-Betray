using UnityEngine;
using System.Collections;

public class AnimatorPlayer
{
	// Diese Klasse ist zur erleichterten Ansteuerung des Animator-Controllers der Spieler ("Contr_Player").

	private Animator anim;
	private int speed_straight, speed_sideways, isGrounded, isCrouching, isFiring, gunType, throws;
	
	public float Speed_straight
	{ get { return anim.GetFloat(speed_straight); } set { anim.SetFloat(speed_straight, value); } }
	public float Speed_sideways
	{ get { return anim.GetFloat(speed_sideways); } set { anim.SetFloat(speed_sideways, value); } }
	public bool IsGrounded
	{ set { anim.SetBool(isGrounded, value); } }
	public bool IsCrouching
	{ set { anim.SetBool(isCrouching, value); } }
	public bool IsFiring
	{ set { anim.SetBool(isFiring, value); } }
	public int GunType
	{ set { anim.SetInteger(gunType, value); } }
	
	public AnimatorPlayer(Animator anim)
	{
		this.anim = anim;
		speed_straight = Animator.StringToHash("speed_straight");
		speed_sideways = Animator.StringToHash("speed_sideways");
		isGrounded = Animator.StringToHash("isGrounded");
		isCrouching = Animator.StringToHash("isCrouching");
		isFiring = Animator.StringToHash("isFiring");
		gunType = Animator.StringToHash("gunType");
		throws = Animator.StringToHash("throws");
	}

	public void Throw()
	{
		anim.SetTrigger(throws);
	}
}