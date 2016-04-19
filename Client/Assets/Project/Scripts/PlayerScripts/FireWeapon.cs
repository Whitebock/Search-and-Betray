using UnityEngine;
using System.Collections;

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
            Debug.Log("FIRE");
            myWeapon.TriggerDown();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Trigger up");
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
}
