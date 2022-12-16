using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Unity.Multiplayer.Tools.NetStatsMonitor;

public class GameData : NetworkBehaviour {
    private static GameData _instance;
    public static GameData Instance {
        get {
            return _instance;
        }
    }

    private int colorIndex = 0;
    private Color[] playerColors = new Color[] {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    public NetworkList<PlayerInfo> allPlayers;
    public RuntimeNetStatsMonitor netMontior;

    public void Start()
    {
        netMontior = NetworkManager.GetComponent<RuntimeNetStatsMonitor>();
    }

    // --------------------------
    // Initialization
    // --------------------------
    public void Awake() {
        // allPlayers must be initialized even though we might be destroying
        // this instance.  Errors occur if we do not.
        allPlayers = new NetworkList<PlayerInfo>();

        // This isn't working as expected.  If you place another GameData in a
        // later scene, it causes an error.  I suspect this has something to
        // do with the NetworkList but I have not verified that yet.  It causes
        // Network related errors.
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        } else if(_instance != this) {
            Destroy(this);
        }
    }


    public override void OnNetworkSpawn() {
        if (IsHost) {
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;
            AddPlayerToList(NetworkManager.LocalClientId);
        }
    }


    // --------------------------
    // Private
    // --------------------------
    private Color NextColor() {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1) {
            colorIndex = 0;
        }
        return newColor;
    }


    // --------------------------
    // Events
    // --------------------------
    private void HostOnClientConnected(ulong clientId) {
        Debug.Log($"[GameData] client connected {clientId}");
        AddPlayerToList(clientId);
    }

    private void HostOnClientDisconnected(ulong clientId) {
        Debug.Log($"[GameData] client disconnected {clientId}");
        RemovePlayerFromList(clientId);
        }
    }


    // --------------------------
    // Public
    // --------------------------
    public void AddPlayerToList(ulong clientId) {
        allPlayers.Add(new PlayerInfo(clientId, NextColor(), false));
    }


    public int FindPlayerIndex(ulong clientId) {
        var idx = 0;
        var found = false;

        while (idx < allPlayers.Count && !found) {
            if (allPlayers[idx].clientId == clientId) {
                found = true;
            } else {
                idx += 1;
            }
        }

        if (!found) {
            idx = -1;
        }

        return idx;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (netMontior)
            {
                netMontior.Visible = !netMontior.Visible;
                netMontior.enabled = netMontior.Visible;
            }
        }
    }
}