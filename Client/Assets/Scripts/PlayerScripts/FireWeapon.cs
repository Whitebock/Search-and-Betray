using UnityEngine;
using System.Collections;
using System;
/*Markus Röse
Script zum steuern der Waffe*/

public class FireWeapon : MonoBehaviour {
    private Weapon myWeapon;
    public GameObject tracer;

    public Weapon MyWeapon
    {
        get
        {
            return myWeapon;
        }

        set
        {
            myWeapon = value;
        }
    }

    // Use this for initialization
    void Start () {
        FindWeapon();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            //If weapon is actually shot recoil is applied
            if(MyWeapon.TriggerDown())
            {
                CheckHit();
                // myRecoil.ApplyRecoil(myWeapon.recoilForce); Currently recoil is not working properly
                ApplyRecoil();
                
            }

        }
        else if (Input.GetButtonUp("Fire1"))
        {
            MyWeapon.TriggerUp();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            MyWeapon.Reload();
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
                        MyWeapon = item.GetComponent<Weapon>();
                }
            }
        }
    }
    void CheckHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2 + UnityEngine.Random.Range(-1 * MyWeapon.recoilForce, MyWeapon.recoilForce),
            Screen.height / 2 + UnityEngine.Random.Range(-1 * MyWeapon.recoilForce, MyWeapon.recoilForce)));
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<DamageHandler>())
            {
                hit.transform.gameObject.GetComponent<DamageHandler>().TakeDamage(MyWeapon.damage);
            }
        }
        else
        {
            hit = new RaycastHit();
            hit.point =  ray.GetPoint(300);
            hit.distance = 300;
            
        }

        //Setting ray origin to weapon
        ray.origin = myWeapon.transform.position;

        GameObject t = Instantiate(tracer, gameObject.transform.position, Quaternion.identity) as GameObject;
        t.GetComponent<Tracer>().StartTracer(ray, hit);
    } 
}
