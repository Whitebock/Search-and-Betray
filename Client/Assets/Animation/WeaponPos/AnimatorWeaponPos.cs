using UnityEngine;
using System.Collections;

public class AnimatorWeaponPos
{
	// Diese Klasse ist zur erleichterten Ansteuerung des Animator-Controllers der Waffe ("Contr_WeaponPos").
	private Animator anim;
	private int shoot, startedReloading, finishedReloading;

	public AnimatorWeaponPos(Animator anim)
	{
		this.anim = anim;
		shoot = Animator.StringToHash("shoot");
		startedReloading = Animator.StringToHash("startedReloading");
		finishedReloading = Animator.StringToHash("finishedReloading");
	}

	public void StartReload()
	{
		anim.SetTrigger(startedReloading);
	}

	public void FinishReload()
	{
		anim.SetTrigger(finishedReloading);
	}

	public void Shoot()
	{
		anim.SetTrigger(shoot);
	}
}
