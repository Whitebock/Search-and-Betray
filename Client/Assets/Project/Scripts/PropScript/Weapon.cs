using UnityEngine;
using System.Collections;
/*Markus Röse
  Script that handels all weapon related actions
  */

public class Weapon : MonoBehaviour {
    public string name; //Name of Weapon
    public int damage; //Damage of Weapon
    public int shotsPerSecond; //Shots that can be fired per second
    public uint shotsTotal; //Total number of shots that the weapons holds
    public uint shotsPerMag; //Count of shots per magazine
    public float recoilForce; 

    public bool singleShot; //Is the weapon to shoot full automatic

    public float reloadTime;//Time needed for one reload

    public AudioClip shotSound;//Sound played when shot
    public AudioClip reloadSound;//Sound played when reload

    AudioSource myAudioSource;

    uint shotsInMag; //Current bullet count in mag

    bool readyToShoot;
    bool reloading;
	
    // Use this for initialization
	void Start () {
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
            readyToShoot = false;

            if (!singleShot)
            {
                //Reset weapon to limit shots per second
                Invoke("SmallReload", 1.0f / shotsPerSecond);
            }
            return true;
        }
        return false;
    }

    public void TriggerUp()
    {
        if (singleShot && !reloading)
        {
            Invoke("SmallReload", 0.3f);
        }
    }

    public void Reload()
    {
        if (shotsTotal > 0)
        {
            readyToShoot = false;
            reloading = true;
            PlaySound(reloadSound);
            Invoke("ReloadFinish", reloadTime);
        }      
    }

    void ReloadFinish()
    {
        for (shotsInMag = 0; shotsInMag < shotsPerMag && shotsTotal > 0; shotsTotal--)
        {
            shotsInMag++;
        }
        readyToShoot = true;
        reloading = false;

    }

    void SmallReload()
    {
        readyToShoot = true;
    }

    void PlaySound(AudioClip toPlay)
    {
        if (myAudioSource != null)
        {
            if (toPlay != null)
            {
                AudioSource.PlayClipAtPoint(toPlay, transform.position);
            }
            else
            {
                Debug.Log("No \"AudioClip\" found! AudioClip: " + toPlay.name);
            }
        }
        else
        {
            Debug.Log("No \"AudioSource\" found! GameObject: " + gameObject.ToString());
        }
    }

    public string BulletCount()
    {
        return shotsInMag + "/" + shotsTotal;
    }
    
    

}
