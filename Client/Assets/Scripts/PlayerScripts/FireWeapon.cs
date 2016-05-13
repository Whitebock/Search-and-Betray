using UnityEngine;
using System.Collections;
using System;
/*Markus Röse
Script zum steuern der Waffe*/

public class FireWeapon : MonoBehaviour
{
    private Weapon myWeapon;
    public GameObject tracer;

    public Weapon MyWeapon
    { get { return myWeapon; } set { myWeapon = value; } }

    void Start ()
	{
        FindWeapon();
	}

	void Update ()
	{
		if (myWeapon == null) return;

        if (Input.GetButton("Fire1"))
        {
            //If weapon is actually shot recoil is applied
            if(MyWeapon.TriggerDown())
			{
                CheckHit();
                ApplyRecoil();
            }

        }
        else if (Input.GetButtonUp("Fire1"))
        {
            MyWeapon.TriggerUp();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            MyWeapon.Reload();
        }
        HUDManagment.SetWeaponInfo(myWeapon.weaponName, (int)myWeapon.shotsInMag, (int)myWeapon.shotsTotal);
        HUDManagment.SetFireMode(myWeapon.singleShot ? FireMode.Single : FireMode.Automatic);
        HUDManagment.SetCrosshair(true);
	}

    private void ApplyRecoil()
    {
        transform.Rotate(new Vector3(UnityEngine.Random.Range(0.2f, 0.4f) * -1,0,0));
    }

    void FindWeapon()
    {
        foreach (Transform item in transform)
        {
            if (item.gameObject.tag == "Weapon")
            {
                if (item.GetComponent<Weapon>())
                {
                    if(item.GetComponent<Weapon>().enabled)
                        MyWeapon = item.GetComponent<Weapon>();
                }
            }
        }
    }

    void CheckHit()
	{
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2 + UnityEngine.Random.Range(-1 * MyWeapon.recoilForce, MyWeapon.recoilForce),
            Screen.height / 2 + UnityEngine.Random.Range(-1 * MyWeapon.recoilForce, MyWeapon.recoilForce)));

		RaycastHit hit;
		Physics.Raycast(ray, out hit);

		//Setting ray origin to weapon
		ray.origin = myWeapon.transform.position;

		GameObject t = Instantiate(tracer, gameObject.transform.position, Quaternion.identity) as GameObject;
		t.GetComponent<Tracer>().StartTracer(ray, hit);

		DamageHandler hitPlayer = null;

		if (hit.transform != null) hitPlayer = hit.transform.GetComponent<DamageHandler>();

		if (hitPlayer)
        {
            Debug.Log("SEND_SHOT_DAMAGE");
			CCC_Client.Instance.SendShot(hit.point, hitPlayer.AttachedPlayer.PlayerID, MyWeapon.damage);
        }
        else
        {
            Debug.Log("SEND_SHOT");
            //CCC_Client.Instance.SendShot(hit.point);
		}
    }
}