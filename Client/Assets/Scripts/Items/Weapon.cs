using UnityEngine;
using System.Collections;
/*
 * Markus Röse: Script that handels all weapon related actions
 */

public class Weapon : MonoBehaviour
{
    public string weaponName;			//Name of Weapon
    public int damage;					//Damage of Weapon
    public int shotsPerSecond;			//Shots that can be fired per second
    public uint shotsTotal;				//Total number of shots that the weapons holds
    public uint shotsPerMag;			//Count of shots per magazine
    public float recoilForce;
    public bool singleShot;				//Is the weapon to shoot full automatic
    public float reloadTime;			//Time needed for one reload
    public AudioClip shotSound;			//Sound played when shooting
	public AudioClip reloadStartSound;	//Sound played when started reloading
	public AudioClip reloadFinishSound;	//Sound played when finished reload
	public AudioClip emptySound;		//Sound played when magazine is empty
    AudioSource myAudioSource;
	AnimatorWeaponPos anim;

    public uint shotsInMag;					//Current bullet count in mag
    bool readyToShoot;
    bool reloading;
    bool smallReloading;

	void Start()
	{
        readyToShoot = true;
        smallReloading = reloading = false;
        myAudioSource = GetComponent<AudioSource>();
        shotsInMag = shotsPerMag;
		anim = new AnimatorWeaponPos(GameObject.Find("PlayerWeaponPos").GetComponent<Animator>());
	}

    /// <summary>
    /// Emulates trigger down event from real weapon.
    /// </summary>
    public bool TriggerDown()
	{
        
        if (readyToShoot && shotsInMag > 0 && !reloading && !smallReloading)
		{
            NotReadyToShoot();
            PlaySound(shotSound);
			anim.Shoot();
            shotsInMag--;
            return true;
        }
		else if (readyToShoot && shotsInMag <= 0 && !reloading && !smallReloading)
		{
            NotReadyToShoot();
            PlaySound(emptySound);
			return false;
		}
        return false;
    }

    /// <summary>
    /// Emulates trigger up event from real weapon.
    /// </summary>
    public void TriggerUp()
    {
        if (singleShot && !smallReloading)
        {
            smallReloading = true;
            Invoke("SmallReload", 1.0f);
            Debug.Log((1.0f / shotsPerSecond).ToString());
        }
       
    }

    /// <summary>
    /// Reloads weapon
    /// </summary>
    /// <returns>Bool which indicates if reload was successfull</returns>
    public bool Reload()
    {
        if (!reloading)
        {
            if (shotsTotal > 0 && shotsInMag < shotsPerMag)
            {
                readyToShoot = false;
                reloading = true;
                PlaySound(reloadStartSound);
                anim.StartReload();
                Invoke("ReloadAlmostFinished", reloadTime - 0.5f);
                Invoke("ReloadFinish", reloadTime);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Disables shoot function on weapon
    /// </summary>
	void NotReadyToShoot()
	{
		readyToShoot = false;
		if (!singleShot) Invoke("SmallReload", 1.0f / shotsPerSecond); //Reset weapon to limit shots per second
	}

    /// <summary>
    /// Called after a single shot has been fired.
    /// </summary>
	void SmallReload()
	{
		readyToShoot = true;
        smallReloading = false;
    }

    /// <summary>
    /// Plays reload sound.
    /// </summary>
	void ReloadAlmostFinished()
	{
		PlaySound(reloadFinishSound);
		anim.FinishReload();
	}

    /// <summary>
    /// Method which is called when reloading is finished. Refills magazine.
    /// </summary>
    void ReloadFinish()
    {
        for (shotsInMag = 0; shotsInMag < shotsPerMag && shotsTotal > 0; shotsTotal--) shotsInMag++;
        readyToShoot = true;
        reloading = false;
    }

    /// <summary>
    /// Method to play sound with weapon as source.
    /// </summary>
    /// <param name="toPlay">Audioclip which shall be played</param>
    public void PlaySound(AudioClip toPlay)
    {
        if (myAudioSource != null)
        {
            if (toPlay != null) AudioSource.PlayClipAtPoint(toPlay, transform.position);
			else Debug.Log("No \"AudioClip\" found! GameObject: " + gameObject.ToString());
        }
        else Debug.Log("No \"AudioSource\" found! GameObject: " + gameObject.ToString());
    }
}