using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
    public float damage;
    public int shotsPerSecond;
    public int shotsLeft;
    public int shotsPerMag;
    public float recoilForce;
    public bool singleShot;
    public AudioClip shotSound;
    public AudioClip reloadSound;
    AudioSource myAudioSource;

    bool readyToShoot;
	// Use this for initialization
	void Start () {
        readyToShoot = true;
        myAudioSource = GetComponent<AudioSource>();
	}
	
    public bool TriggerDown()
    {
        if (readyToShoot && shotsInMag() > 0)
        {
            shotsLeft--;
            PlaySound(shotSound);

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
        if (singleShot)
        {
            Invoke("SmallReload", 0.3f);
        }
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
