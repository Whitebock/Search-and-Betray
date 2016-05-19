using UnityEngine;
using System.Collections;
using Assets.Scripts.Network;

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
       CCC_Client.Instance.OnPlayerShoot += ShootWeaponTemp;
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

    void ShootWeaponTemp(int onlinePlayerID, Vector3 hitpoint)
	{
		if (onlinePlayerID == onlinePlayer.PlayerID)
        {
            Debug.Log("tracer");
            Dispatcher.Instance.Invoke(delegate {
                /*GameObject t = Instantiate(tracer, gameObject.transform.position, Quaternion.identity) as GameObject;
                RaycastHit hit = new RaycastHit();
                hit.distance = Vector3.Distance(transform.position, hitpoint);
                t.GetComponent<Tracer>().StartTracer(new Ray(transform.position, (hitpoint - transform.position).normalized), hit);*/
                AudioSource.PlayClipAtPoint(defaultShot, transform.position);
            });
            

           
        }

	}
}