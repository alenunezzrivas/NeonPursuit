using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private List<PlayerRole> players = new List<PlayerRole>();

    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        NetworkObject obj = runner.Spawn(
            playerPrefab,
            spawn.position,
            spawn.rotation,
            player
        );

        PlayerRole role = obj.GetComponent<PlayerRole>();
        players.Add(role);
    }

    public void AssignRolesNow()
    {
        if (players.Count == 0) return;

        int hunterCount = players.Count >= 4 ? 2 : 1;

        foreach (var p in players)
            p.SetRole(Role.Player);

        for (int i = 0; i < hunterCount; i++)
        {
            int index = Random.Range(0, players.Count);
            players[index].SetRole(Role.Hunter);
        }
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }
}
