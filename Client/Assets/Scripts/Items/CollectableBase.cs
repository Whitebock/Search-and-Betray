using UnityEngine;
using System.Collections;

public enum ItemType {Ammo, Armor, Health, Speed, Weapon};

//Basisklasse für PowerUps/Waffe
public abstract class CollectableBase : MonoBehaviour
{
	public ItemType type;
	public Collider col_item;
	internal Collider col_triggerzone;
	internal Rigidbody phy;

	void Awake()
	{
		if (!col_item) Debug.Log("No Collider attached to this script!\nItem won't have collision after it is droped.");
		col_triggerzone = GetComponent<Collider>();
		phy = GetComponent<Rigidbody>();
	}

	public abstract void PickUp();
}