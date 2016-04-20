using UnityEngine;
using System.Collections;

public class FireWeapon : MonoBehaviour {
    public Weapon myWeapon;
    Recoil myRecoil;
	// Use this for initialization
	void Start () {
        myRecoil = GetComponent<Recoil>();
        FindWeapon();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            //If weapon is actually shot recoil is applied
            if(myWeapon.TriggerDown())
            {
                CheckHit();
                myRecoil.ApplyRecoil(myWeapon.recoilForce);
                
            }

        }
        if (Input.GetButtonUp("Fire1"))
        {
            myWeapon.TriggerUp();
        }

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
                        myWeapon = item.GetComponent<Weapon>();
                }
            }
        }
    }
    void CheckHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<DamageHandler>())
            {
                hit.transform.gameObject.GetComponent<DamageHandler>().TakeDamage(myWeapon.damage, PlayerInfo.PlayerID);
            }
        }
            
    } 
}
