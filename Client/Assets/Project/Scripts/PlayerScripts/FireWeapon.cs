using UnityEngine;
using System.Collections;
using System;

public class FireWeapon : MonoBehaviour {
    public Weapon myWeapon;
	// Use this for initialization
	void Start () {
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
                // myRecoil.ApplyRecoil(myWeapon.recoilForce); Currently recoil is not working properly
                ApplyRecoil();
                
            }

        }
        else if (Input.GetButtonUp("Fire1"))
        {
            myWeapon.TriggerUp();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            myWeapon.Reload();
        }
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
                        myWeapon = item.GetComponent<Weapon>();
                }
            }
        }
    }
    void CheckHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2 + UnityEngine.Random.Range(-1 * myWeapon.recoilForce, myWeapon.recoilForce),
            Screen.height / 2 + UnityEngine.Random.Range(-1 * myWeapon.recoilForce, myWeapon.recoilForce)));
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<DamageHandler>())
            {
                hit.transform.gameObject.GetComponent<DamageHandler>().TakeDamage(myWeapon.damage, PlayerInfo.PlayerID);
            }
        }
            
    } 
}
