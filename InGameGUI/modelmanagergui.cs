using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class modelmanagergui : NetworkBehaviour {

	public GameObject scrollviewcontent;

	public GameObject guiprefabmodel;

	public GameObject[] allmodels;

	public GameObject playerprefab;

	void Start()
	{
		SetupScrollMenu();
	}


	void SetupScrollMenu()
	{
		foreach (var item in allmodels) {
			GameObject temp = Instantiate(guiprefabmodel,Vector3.zero,Quaternion.identity) as GameObject;
			temp.GetComponentInChildren<Text>().text = item.transform.name;
			temp.transform.SetParent(scrollviewcontent.transform);
		}

	}


	[Command]
	public void CmdSetModel()
	{
		


	}


	[Command]
	public void CmdSetName()
	{


		
	}

}
