using UnityEngine;
using System.Collections;

public class CollectableWeapon : CollectableBase
{
	public Vector3 dropforce;
	public float pickUpCooldown = 2f;

	void Start()
	{
		type = ItemType.Weapon;
	}

	public override void PickUp()
	{
		phy.isKinematic = true;
		if (col_item) col_item.enabled = false;
		col_triggerzone.enabled = false;
		this.enabled = false;
	}

	public void Drop()
	{
		phy.isKinematic = false;
		if (col_item) col_item.enabled = true;
		this.enabled = true;
		phy.velocity = PlayerInfo.Phy.velocity;
		phy.AddForce(transform.localToWorldMatrix * dropforce, ForceMode.Impulse);
		Invoke("ActivatePickUp", pickUpCooldown);
	}

	private void ActivatePickUp()
	{
		col_triggerzone.enabled = true;
	}
}