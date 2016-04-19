using UnityEngine;
using System.Collections;

public class Recoil : MonoBehaviour {

    /*Dieses Script generiert den Rückstoß beim Schießen. Bei jedem Schuss wird die Methode "ApplyRecoil" aufgerufen. 
     * Diese generiert nun Zufällig eine Bewegung beim Abschießen
     */
     
   
    public void ApplyRecoil(float recoilForce)
    {
        transform.Rotate(new Vector3(Random.Range(recoilForce * 0.05f , recoilForce), 0, 0));
        transform.Translate(new Vector3(Random.Range(-1 * recoilForce, recoilForce) * 0.01f , 0, 0));
    }
}
