using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    public float damage;
    public int shotsPerSecond;
    public int shotsLeft;
    public int shotsPerMag;
    public bool singleShot;
    public AudioClip shotSound;
    public AudioClip reloadSound;
    AudioSource myAudioSource;

    bool readyToShoot;
	// Use this for initialization
	void Start () {
        myAudioSource = GetComponent<AudioSource>();
	}
	
    public bool Shoot()
    {
        if (readyToShoot && shotsInMag() > 0)
        {
            shotsLeft--;

            //Reset weapon to limit shots per second
            Invoke("SmallReload", 1 / shotsPerSecond);

            return true;
        }
        return false;
    }

    public int shotsInMag()
    {
        return shotsLeft % shotsPerMag;
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

}
