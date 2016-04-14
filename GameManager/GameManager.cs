using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	//keep track of all players in network
	public static Dictionary<string,playerstats> playerlist = new Dictionary<string, playerstats> ();

	public static void RegisterPlayer(string networkid, playerstats player)
	{
		//add to playerlist
		playerlist.Add (networkid, player);

	}


	public static void UnRegisterPlayer(string networkid)
	{
		//remove player from list
		playerlist.Remove (networkid);

	}

	//gets playerstats from passed networkid
	public static playerstats GetPlayerstats(string networkid)
	{

		return playerlist [networkid];

	}

	public static List<playerstats> GetAllPlayerstats()
	{
		List<playerstats> templist = new List<playerstats> ();

		foreach (var item in playerlist) {
			templist.Add (item.Value); 
		}

		return templist;


	}


}
