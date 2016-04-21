using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MagCount : MonoBehaviour {
    public GameObject myWeapon;

	
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = myWeapon.GetComponent<Weapon>().BulletCount();
	}
}
