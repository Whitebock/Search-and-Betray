using UnityEngine;
using System.Collections;

public class PosAdjust_OnlinePlayer_Crouch : PositionAdjust
{
	// Erklärungen sind in der Parrentklasse "PositionAdjust" zu finden.

	private OnlinePlayerInfo info;

	void Start()
	{
		info = GetComponentInParent<OnlinePlayerInfo>();
	}

	void Update()
	{
		if (info.IsCrouching && info.IsGrounded) New();
		else Original();
	}
}