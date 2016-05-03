using UnityEngine;
using System.Collections;
using System;
/*
 * Markus Röse: Script zum steuern der Waffe
 */

public class FireWeapon : MonoBehaviour
{
	private Weapon myWeapon;
	private AnimatorWeaponPos anim;
	public Weapon MyWeapon
	{
		get { return myWeapon; }
		set { myWeapon = value; }
	}

	void Start()
	{
		anim = new AnimatorWeaponPos(GetComponentInChildren<Animator>());
	}

	void Update()
	{
		if (!myWeapon) return;

		if (Input.GetButton ("Fire1"))
		{
			if (myWeapon.TriggerDown()) // If weapon is actually shot recoil is applied
			{
				CheckHit();
				anim.Shoot();
				ApplyRecoil();
			}
		}
		else if (Input.GetButtonUp ("Fire1")) myWeapon.TriggerUp();
		else if (Input.GetKeyDown (KeyCode.R))
		{
			if (myWeapon.Reload()) anim.Reload();
		}
	}

    private void ApplyRecoil()
    {
        transform.Rotate(new Vector3(UnityEngine.Random.Range(0.2f, 0.4f) * -1,0,0));
    }

    void CheckHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2 + UnityEngine.Random.Range(-myWeapon.recoilForce, myWeapon.recoilForce),
            Screen.height / 2 + UnityEngine.Random.Range(-myWeapon.recoilForce, myWeapon.recoilForce)));
		
        if (Physics.Raycast(ray, out hit))
        {
			if (hit.transform.gameObject.GetComponent<DamageHandler>())
            {
                hit.transform.gameObject.GetComponent<DamageHandler>().TakeDamage(myWeapon.damage);
            }
        }
    }
}