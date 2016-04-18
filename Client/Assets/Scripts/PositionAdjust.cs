using UnityEngine;
using System.Collections;

public abstract class PositionAdjust : MonoBehaviour
{
	/*
	 * Skripte, die von diese Klasse erben, bewegen das Objekt auf dem es liegt in die angegebene Richtung ("New()")
	 * und wieder zurück ("Original()").
	 * Die Methoden werden von den erbenden Klassen aufgerufen und heißen "PosAdjust_...".
	 */

	// Original Position
	private Vector3 pos, scl;
	private Quaternion rot;

	// Neue Position
	public Vector3 move, rotate, scale;

	// Smoothing
	public float interpolation = 1f;

	void Awake()
	{
		pos = transform.localPosition;
		rot = transform.localRotation;
		scl = transform.localScale;
	}

	internal void Original()
	{
		transform.localPosition = Vector3.Lerp(transform.localPosition, pos, interpolation);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, interpolation);
		transform.localScale = Vector3.Lerp(transform.localScale, scl, interpolation);
	}

	internal void New()
	{
		transform.localPosition = Vector3.Lerp(transform.localPosition, pos + move, interpolation);
		transform.Rotate(Vector3.Lerp(rot.eulerAngles, rot.eulerAngles + rotate, interpolation));
		transform.localScale = Vector3.Lerp(transform.localScale, scl + scale, interpolation);
	}
}