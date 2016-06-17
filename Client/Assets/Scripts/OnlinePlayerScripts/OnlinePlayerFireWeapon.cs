using UnityEngine;
using System.Collections;

public class OnlinePlayerFireWeapon : MonoBehaviour
{
	private OnlinePlayerInfo onlinePlayer;
	private Weapon myWeapon;
    public GameObject tracer;

    public Weapon MyWeapon
    {
		get { return myWeapon; }
		set { myWeapon = value; }
	}

	void Start()
	{
		onlinePlayer = GetComponentInParent<OnlinePlayerInfo>();
	}

	public void ShootWeapon(int onlinePlayerID, Vector3 hitpoint)
	{
		Debug.Log(myWeapon.transform.name + "\n" + onlinePlayerID + " | " + onlinePlayer.PlayerID);
		if (myWeapon && onlinePlayerID != onlinePlayer.PlayerID) return;

		GameObject t = Instantiate(tracer, gameObject.transform.position, Quaternion.identity) as GameObject;
		RaycastHit hit = new RaycastHit();
		hit.distance = Vector3.Magnitude(hitpoint - myWeapon.transform.position);
		t.GetComponent<Tracer>().StartTracer(new Ray(myWeapon.transform.position, hitpoint - myWeapon.transform.position), hit);

		myWeapon.PlaySound(myWeapon.shotSound);
	}
}