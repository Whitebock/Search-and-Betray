using UnityEngine;
using System.Collections;

public class CharacterPickUp : MonoBehaviour
{
	private WeaponHinge hinge;

	void Start()
	{
		hinge = GetComponentInChildren<WeaponHinge>();
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Collectable")
		{
			CollectableBase item = other.GetComponent<CollectableBase>();
			switch (item.type)
			{
				case ItemType.Weapon:
					if (hinge.Weapon == null)
					{
						item.PickUp();
						hinge.DrawWeapon(item.transform);
					}
					break;

				default:
					Debug.Log("The following Itemtype is not implemented: " + item.type.ToString());
					break;
			}
		}
	}
}