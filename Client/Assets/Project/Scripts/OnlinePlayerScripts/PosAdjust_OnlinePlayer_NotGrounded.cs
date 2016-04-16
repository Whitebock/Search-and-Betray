using UnityEngine;
using System.Collections;

public class PosAdjust_OnlinePlayer_NotGrounded : PositionAdjust
{
	// Erklärungen sind in der Parrentklasse "PositionAdjust" zu finden.

	private OnlinePlayerInfo info;

	void Start()
	{
		info = GetComponentInParent<OnlinePlayerInfo>();
	}

	void Update()
	{
		if (!info.IsGrounded) New();
		else Original();
	}
}
