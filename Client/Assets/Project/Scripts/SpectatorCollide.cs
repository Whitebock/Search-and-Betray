using UnityEngine;
using System.Collections;

public class SpectatorCollide : MonoBehaviour
{
	public float maxDistance = 2f;

	/*void Update()
	{
		Ray x = new Ray(transform.parent.position, transform.parent.localToWorldMatrix * transform.localPosition);
		RaycastHit hitInfo;

		if (Physics.Raycast(x, out hitInfo, maxDistance)) { } // transform.localPosition = new Vector3 (0f, -hitInfo.distance, 0f);
		//else transform.localPosition = new Vector3 (0f, -maxDistance, 0f);

		Debug.DrawRay(x.origin, x.direction, Color.yellow, maxDistance);
		if (hitInfo.transform != null) Debug.Log(hitInfo.distance + ", " + hitInfo.transform.name);

		//Debug.DrawLine(transform.parent.position, transform.parent.position + new Vector3(0f, -maxDistance, 0f));
	}*/
}
