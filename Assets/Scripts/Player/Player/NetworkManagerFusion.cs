using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerFusion : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runnerPrefab;

    private NetworkRunner runner;

    public PlayerSpawner playerSpawner;

    public GameObject mainMenuUI;

    public async void StartGame()
    {
        Debug.Log("========== START GAME ==========");

        // Ocultar menú
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }

        // Evitar duplicados
        if (FindObjectOfType<NetworkRunner>() != null)
        {
            Debug.LogWarning("Ya existe un NetworkRunner en escena");
            return;
        }

        // Prefab asignado?
        if (runnerPrefab == null)
        {
            Debug.LogError("Runner Prefab NO asignado");
            return;
        }

        Debug.Log("Instanciando Runner...");

        runner = Instantiate(runnerPrefab);

        if (runner == null)
        {
            Debug.LogError("Runner NO pudo instanciarse");
            return;
        }

        runner.name = "NetworkRunner";

        Debug.Log("Runner instanciado correctamente");

        runner.ProvideInput = true;

        Debug.Log("ProvideInput activado");

        runner.AddCallbacks(this);

        Debug.Log("Callbacks añadidos");

        // Buscar PlayerSpawner
        if (playerSpawner == null)
        {
            Debug.Log("Buscando PlayerSpawner...");

            playerSpawner = FindObjectOfType<PlayerSpawner>();

            if (playerSpawner == null)
            {
                Debug.LogError("NO se encontró PlayerSpawner");
                return;
            }

            Debug.Log("PlayerSpawner encontrado");
        }

        Debug.Log("Iniciando Fusion...");

        try
        {
            var result = await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "TestRoom"
            });

            if (result.Ok)
            {
                Debug.Log("FUSION INICIADO CORRECTAMENTE");
            }
            else
            {
                Debug.LogError("Fusion NO inició");
                Debug.LogError("ShutdownReason: " + result.ShutdownReason);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("EXCEPCIÓN AL INICIAR FUSION");
            Debug.LogError(e);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Jugador unido: " + player);

        if (runner.IsServer)
        {
            Debug.Log("Servidor spawneando jugador...");

            playerSpawner.SpawnPlayer(runner, player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Jugador salió: " + player);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerInputData data = new PlayerInputData();

        data.move = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        data.jump = Input.GetKey(KeyCode.Space);

        input.Set(data);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Conectado al servidor");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogError("Desconectado del servidor: " + reason);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError("Conexión fallida: " + reason);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning("Fusion cerrado: " + shutdownReason);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene Load DONE");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene Load START");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}