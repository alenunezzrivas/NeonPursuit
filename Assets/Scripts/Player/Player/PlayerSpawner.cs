using UnityEngine;
using Fusion;
using System.Collections.Generic;

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

        Debug.Log("Jugador añadido a lista");

        if (runner.IsServer)
        {
            AssignRoles();
        }
    }

    void AssignRoles()
    {
        if (players.Count == 0) return;

        int hunterCount = players.Count >= 4 ? 2 : 1;

        // Reset todos
        foreach (var p in players)
        {
            p.SetRole(Role.Player);
        }

        // Elegir hunters
        for (int i = 0; i < hunterCount; i++)
        {
            int index = Random.Range(0, players.Count);
            players[index].SetRole(Role.Hunter);
        }

        Debug.Log("Roles asignados correctamente");
    }
}