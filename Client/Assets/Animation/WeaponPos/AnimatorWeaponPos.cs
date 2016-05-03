using UnityEngine;
using System.Collections;

public class AnimatorWeaponPos
{
	// Diese Klasse ist zur erleichterten Ansteuerung des Animator-Controllers der Waffe ("Contr_WeaponPos").
	private Animator anim;
	private int shoot, reload;

	public AnimatorWeaponPos(Animator anim)
	{
		this.anim = anim;
		shoot = Animator.StringToHash("shoot");
		reload = Animator.StringToHash("reload");
	}

	public void Reload()
	{
		anim.SetTrigger(reload);
	}

	public void Shoot()
	{
		anim.SetTrigger(shoot);
	}
}
