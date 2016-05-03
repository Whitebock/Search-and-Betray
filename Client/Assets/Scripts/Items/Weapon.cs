using UnityEngine;
using System.Collections;
/*
 * Markus Röse: Script that handels all weapon related actions
 */

public class Weapon : MonoBehaviour
{
    public string name;				//Name of Weapon
    public int damage;				//Damage of Weapon
    public int shotsPerSecond;		//Shots that can be fired per second
    public uint shotsTotal;			//Total number of shots that the weapons holds
    public uint shotsPerMag;		//Count of shots per magazine
    public float recoilForce;
    public bool singleShot;			//Is the weapon to shoot full automatic
    public float reloadTime;		//Time needed for one reload
    public AudioClip shotSound;		//Sound played when shooting
	public AudioClip reloadSound;	//Sound played when reload
	public AudioClip emptySound;	//Sound played when magazine is empty
    AudioSource myAudioSource;

    uint shotsInMag;				//Current bullet count in mag
    bool readyToShoot;
    bool reloading;

	void Start()
	{
        readyToShoot = true;
        reloading = false;
        myAudioSource = GetComponent<AudioSource>();
        shotsInMag = shotsPerMag;
	}
	
    public bool TriggerDown()
	{
        if (readyToShoot && shotsInMag > 0 && !reloading)
		{
            PlaySound(shotSound);
            shotsInMag--;
			NotReadyToShoot();
            return true;
        }
		else if (readyToShoot && shotsInMag <= 0 && !reloading)
		{
			PlaySound(emptySound);
			NotReadyToShoot();
			return false;
		}
        return false;
    }

    public void TriggerUp()
    {
        if (singleShot && !reloading) Invoke("SmallReload", 0.3f);
    }

    public bool Reload()
    {
		if (shotsTotal > 0 && shotsInMag < shotsPerMag)
        {
            readyToShoot = false;
            reloading = true;
            PlaySound(reloadSound);
            Invoke("ReloadFinish", reloadTime);
			return true;
        }
		else return false;
	}

	void NotReadyToShoot()
	{
		readyToShoot = false;
		if (!singleShot) Invoke("SmallReload", 1.0f / shotsPerSecond); //Reset weapon to limit shots per second
	}

	void SmallReload()
	{
		readyToShoot = true;
	}

    void ReloadFinish()
    {
        for (shotsInMag = 0; shotsInMag < shotsPerMag && shotsTotal > 0; shotsTotal--) shotsInMag++;
        readyToShoot = true;
        reloading = false;
    }

    void PlaySound(AudioClip toPlay)
    {
        if (myAudioSource != null)
        {
            if (toPlay != null) AudioSource.PlayClipAtPoint(toPlay, transform.position);
			else Debug.Log("No \"AudioClip\" found! GameObject: " + gameObject.ToString());
        }
        else Debug.Log("No \"AudioSource\" found! GameObject: " + gameObject.ToString());
    }
}