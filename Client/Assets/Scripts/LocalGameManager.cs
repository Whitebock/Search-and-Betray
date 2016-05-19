using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Network;
using System;

public class LocalGameManager : MonoBehaviour
{
    public Transform onlinePlayerPrefab;                                            // Prefab mit dem neue OnlineSpieler instanziiert werden
    private List<OnlinePlayerInfo> onlinePlayer = new List<OnlinePlayerInfo>();     // Liste aller OnlineSpieler
    private PlayerInfo player;                                                      // Referenz auf den Spieler
    private MainCameraManager cameraManager;                                        // Referenz auf den KameraManager
    public Transform[] spawns;		
    void Awake()
    {
        // Initialisierungen
        cameraManager = GameObject.Find("MainCamera").GetComponent<MainCameraManager>();
        player = GameObject.Find("Player").GetComponent<PlayerInfo>();
        spawns = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) spawns[i] = transform.GetChild(i);
        CCC_Client.Instance.OnPlayerJoin += ConnectOnlinePlayer;
        CCC_Client.Instance.OnSync += OnSync;
    }

    private void OnSync(Dictionary<int, string> players)
    {
        foreach (KeyValuePair<int, string> s in players)
        {
            bool found = false;
            foreach (OnlinePlayerInfo op in onlinePlayer)
            {
                if (s.Key == op.PlayerID)
                { found = true; break; }
            }
            if (!found && s.Key != PlayerInfo.PlayerID)
            {
                ConnectOnlinePlayer(s.Key, s.Value);
                
            }
        }
    }

    void Update()
    {
        Dispatcher.Instance.InvokePending();
    }

    // Der Spieler in der Scene ist deaktiviert und die Kameraposition ist fest
    public void StopPlayer()
    {
        cameraManager.FrezzeCamera();
        player.gameObject.SetActive(false);
    }

    // OnlinePlayer nach PlayerID finden
    private OnlinePlayerInfo GetOnlinePlayer(int id)
    {
        foreach (OnlinePlayerInfo item in onlinePlayer) if (item.PlayerID == id) return item;
        return null;
    }

    // Spawnindex überprüfen. In einem Fehlerfall wird eine Meldung ausgegeben und statt dessen Spawnindex 0 zurückgegeben.
    private int CheckSpawnIndex(int spawnIndex)
    {
        if (spawnIndex < 0 || spawnIndex >= spawns.Length)
        {
            Debug.LogError("[GAME] Could not spawn the object at the given index (" + spawnIndex + ")!\nThere are only " + spawns.Length + " spawn points available.");
            return 0;
        }
        else if (spawns[spawnIndex] == null)
        {
            Debug.LogError("[GAME] Could not spawn the object at given the index (" + spawnIndex + ")!\nThe reference to this spawn point is missing.");
            return 0;
        }
        return spawnIndex;
    }

    // OnlinePlayer nach PlayerID löschen
    private OnlinePlayerInfo RemoveOnlinePlayer(int id)
    {
        for (int i = 0; i < onlinePlayer.Count; i++)
        {
            if (onlinePlayer[i].PlayerID == id)
            {
                OnlinePlayerInfo x = onlinePlayer[i];
                onlinePlayer.RemoveAt(i);
                return x;
            }
        }
        return null;
    }

    // ------------------------------------------------- Spieler -------------------------------------------------
    public void SpawnPlayer(int spawnIndex)
    {
        int x = CheckSpawnIndex(spawnIndex);                // Spawnpoint checken
        player.transform.position = spawns[x].position;     // positionieren
        player.transform.rotation = spawns[x].rotation;     // ausrichten
        player.gameObject.SetActive(true);                  // aktivieren
        cameraManager.UnfrezzeCamera(false);                // Spieler wieder in Egoperspektive bringen
    }
    public void SpawnPlayer()
    {
        int x = CheckSpawnIndex((int)UnityEngine.Random.Range(0, spawns.Length)); // Zufälligen Spawnpoint setzen und checken
        player.transform.position = spawns[x].position;
        player.transform.rotation = spawns[x].rotation;
        Debug.Log(x);
        player.gameObject.SetActive(true);
        cameraManager.UnfrezzeCamera(false);
    }
    // ---------------------------------------------- OnlineSpieler ----------------------------------------------
    private void ConnectOnlinePlayer(int id, string name)
    {
        Dispatcher.Instance.Invoke(delegate ()
        {
            // OnlinePlayer instanziieren
                OnlinePlayerInfo newPlayer = Instantiate(onlinePlayerPrefab).GetComponent<OnlinePlayerInfo>();

            // OnlinePlayer initialisieren
            newPlayer.playerID = id;
            newPlayer.PlayerName = name;
            newPlayer.TeamID = 0;

            // Referenz merken
            onlinePlayer.Add(newPlayer);

            // Spieler inaktiv setzen
            //newPlayer.gameObject.SetActive(false);

            // Benachrichtigung ausgeben
            Debug.Log("[SERVER] \"" + name + "\" (ID: " + id + ") connected");

        });
        

    }
    private void DisconnectOnlinePlayer(int id)
    {
        // OnlinePlayer aus des Scene und aus der Liste löschen
        GameObject.Destroy(RemoveOnlinePlayer(id).transform);
        Debug.Log("[SERVER] \"" + name + "\" (ID: " + id + ") disconnected");
    }
    private void ShowOnlinePlayer(int id)
    {
        // Zu spawnenden OnlinePlayer identifizieren
        Transform onlPlayer = GetOnlinePlayer(id).transform;

        // Sollte kein OnlinePlayer gefunden werden eine Benachrichtigung senden
        if (onlPlayer == null)
        {
            Debug.LogError("[GAME] Could not spawn the online player with the PlayerID " + id + "!\nAn online player with this PlayerID could not be found in the Scene and won't be visible if it is connected.");
            return;
        }

        // OnlinePlayer aktivieren
        onlPlayer.gameObject.SetActive(true);
    }
    private void HideOnlinePlayer(int id)
    {
        // Zu spawnenden OnlinePlayer identifizieren
        Transform onlPlayer = GetOnlinePlayer(id).transform;

        // Sollte kein OnlinePlayer gefunden werden eine Benachrichtigung senden
        if (onlPlayer == null)
        {
            Debug.LogError("[GAME] Could not hide the online player with the PlayerID " + id + "!\nAn online player with this PlayerID could not be found in the Scene and will still be visible.");
            return;
        }

        // OnlinePlayer aktivieren
        onlPlayer.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        CCC_Client.Instance.OnPlayerJoin -= ConnectOnlinePlayer;
    }
}