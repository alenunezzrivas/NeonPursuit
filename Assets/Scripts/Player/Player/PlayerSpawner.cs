using Fusion;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject runnerPrefab;

    public Transform[] spawnPoints;

    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        int playerCount = 0;

        foreach (var p in runner.ActivePlayers)
        {
            playerCount++;
        }

        bool isZombie = playerCount == 1;

        GameObject prefabToSpawn = isZombie ? zombiePrefab : runnerPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogError("❌ Prefab no asignado");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("❌ No hay spawn points");
            return;
        }

        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        NetworkObject netObj = prefabToSpawn.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("❌ El prefab no tiene NetworkObject");
            return;
        }

        runner.Spawn(
            netObj,
            spawnPoint.position,
            spawnPoint.rotation,
            player
        );

        Debug.Log("✅ Jugador spawneado correctamente");
    }
}