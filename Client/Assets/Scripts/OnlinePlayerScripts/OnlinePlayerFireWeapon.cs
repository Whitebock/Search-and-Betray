using UnityEngine;
using System.Collections;

public class OnlinePlayerFireWeapon : MonoBehaviour
{
	private OnlinePlayerInfo onlinePlayer;
	private Weapon myWeapon;
    public GameObject tracer;
	public AudioClip defaultShot;

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
		if (myWeapon && onlinePlayerID != onlinePlayer.PlayerID) return;

		GameObject t = Instantiate(tracer, gameObject.transform.position, Quaternion.identity) as GameObject;
		RaycastHit hit = new RaycastHit();
		hit.distance = Vector3.Magnitude(myWeapon.transform.position - hitpoint);
		t.GetComponent<Tracer>().StartTracer(new Ray(myWeapon.transform.position, myWeapon.transform.position - hitpoint), hit);

		myWeapon.PlaySound(myWeapon.shotSound);
	}

	public void ShootWeaponThemp(int onlinePlayerID, Vector3 hitpoint)
	{
		if (myWeapon && onlinePlayerID != onlinePlayer.PlayerID) return;

		GameObject t = Instantiate(tracer, gameObject.transform.position, Quaternion.identity) as GameObject;
		RaycastHit hit = new RaycastHit();
		hit.distance = Vector3.Magnitude(transform.position - hitpoint);
		t.GetComponent<Tracer>().StartTracer(new Ray(transform.position, myWeapon.transform.position - hitpoint), hit);

		AudioSource.PlayClipAtPoint(defaultShot, transform.position);
	}
}