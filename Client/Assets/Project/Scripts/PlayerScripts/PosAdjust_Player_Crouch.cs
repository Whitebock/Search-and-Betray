using UnityEngine;
using System.Collections;

public class PosAdjust_Player_Crouch : PositionAdjust
{
	// Erklärungen sind in der Parrentklasse "PositionAdjust" zu finden.

	void Update()
	{
		if (PlayerInfo.IsCrouching) New();
		else Original();
	}
}