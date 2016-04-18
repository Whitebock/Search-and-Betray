using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{
	/*
	 * Dieses Skript gibt dem Objekt die gleiche Rotation wie der MainCamera.
	 */

	Transform myCamera;

	void Start()
	{
		myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}

	void Update()
	{
		transform.rotation = myCamera.rotation;
	}
}