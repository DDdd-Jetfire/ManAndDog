using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [Header("Player Prefabs")]
    public GameObject playerPrefabA;
    public GameObject playerPrefabB;
    public override void Awake()
    {
        base.Awake();
        Debug.Log($"[NM] Awake -> {GetType().Name}");
    }

    public override void OnStartServer()
    {
        Debug.Log("[NM] OnStartServer");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"[NM] OnServerAddPlayer for conn {conn.connectionId}");

        var chosen = (numPlayers == 0) ? playerPrefabA : playerPrefabB;
        var player = Instantiate(chosen);

        NetworkServer.Spawn(player);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
