using System;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
	/*
	 * (Angepasstes Unity-Skript)
	 * Dieses Skript übersetzt Mausbewegungen auf zwei Objekte.
	 * Horizontale Bewegungen werden auf das Objekt "tr_horizontal" übertragen;
	 * Vertikale Bewegungen werden auf das Objekt "tr_vertical" übertragen;
	 * 
	 * Für den fertigen Build die beiden auskommentierten Zeilen in -Start()- einkommentieren!
	 */

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;

	public Transform tr_horizontal, tr_vertical;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    void Start()
    {
		PlayerPos();
		m_CharacterTargetRot = tr_horizontal.localRotation;
		m_CameraTargetRot = tr_vertical.localRotation;
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
    }

	public void PlayerPos()
	{
		tr_horizontal = GameObject.Find("Player").transform;
		tr_vertical = GameObject.Find("PlayerCameraPos").transform;
	}

    void Update()
    {
		if (tr_horizontal == null || tr_vertical == null) return;

		float yRot = PlayerInfo.Inp_yRot * XSensitivity;
		float xRot = PlayerInfo.Inp_xRot * YSensitivity;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if(clampVerticalRotation) m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        if(smooth)
        {
			tr_horizontal.localRotation = Quaternion.Slerp(tr_horizontal.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
			tr_vertical.localRotation = Quaternion.Slerp(tr_vertical.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
        }
        else
        {
			if (object.ReferenceEquals(tr_horizontal, tr_vertical))
			{
				tr_horizontal.localRotation = Quaternion.Euler(m_CharacterTargetRot.eulerAngles + m_CameraTargetRot.eulerAngles);
			}
			else
			{
				tr_horizontal.localRotation = m_CharacterTargetRot;
				tr_vertical.localRotation = m_CameraTargetRot;
			}
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
        angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}