using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerFusion : MonoBehaviour, INetworkRunnerCallbacks
{
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

        // Buscar NetworkRunner YA EXISTENTE EN ESCENA
        runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            Debug.LogError("NO se encontró NetworkRunner en escena");
            Debug.LogError("Arrastra el prefab Runner a la Hierarchy antes de ejecutar.");
            return;
        }

        Debug.Log("NetworkRunner encontrado correctamente");

        // Evitar iniciar Fusion dos veces
        if (runner.IsRunning)
        {
            Debug.LogWarning("Fusion ya está ejecutándose");
            return;
        }

        // Activar input
        runner.ProvideInput = true;

        Debug.Log("ProvideInput activado");

        // Registrar callbacks
        runner.AddCallbacks(this);

        Debug.Log("Callbacks registrados");

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

        // Verificar SceneManager
        NetworkSceneManagerDefault sceneManager =
            runner.GetComponent<NetworkSceneManagerDefault>();

        if (sceneManager == null)
        {
            sceneManager =
                runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            Debug.Log("NetworkSceneManagerDefault añadido automáticamente");
        }

        Debug.Log("INTENTANDO INICIAR FUSION...");

        try
        {
            var result = await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                SessionName = "TestRoom",
                Scene = SceneRef.FromIndex(
                    SceneManager.GetActiveScene().buildIndex),
                SceneManager = sceneManager
            });

            Debug.Log("========== RESULTADO STARTGAME ==========");
            Debug.Log("RESULT OK: " + result.Ok);
            Debug.Log("SHUTDOWN: " + result.ShutdownReason);

            if (result.Ok)
            {
                Debug.Log("FUSION INICIADO CORRECTAMENTE");

                if (runner.IsServer)
                {
                    Debug.Log("ESTE PEER ES HOST");
                }
                else
                {
                    Debug.Log("ESTE PEER ES CLIENT");
                }
            }
            else
            {
                Debug.LogError("Fusion NO inició");
                Debug.LogError("ShutdownReason: " + result.ShutdownReason);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("========== EXCEPCIÓN AL INICIAR FUSION ==========");
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

    public void OnDisconnectedFromServer(
        NetworkRunner runner,
        NetDisconnectReason reason)
    {
        Debug.LogError("Desconectado del servidor: " + reason);
    }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason)
    {
        Debug.LogError("Conexión fallida: " + reason);
    }

    public void OnShutdown(
        NetworkRunner runner,
        ShutdownReason shutdownReason)
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

    public void OnInputMissing(
        NetworkRunner runner,
        PlayerRef player,
        NetworkInput input)
    {
    }

    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token)
    {
    }

    public void OnUserSimulationMessage(
        NetworkRunner runner,
        SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(
        NetworkRunner runner,
        List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(
        NetworkRunner runner,
        HostMigrationToken hostMigrationToken)
    {
    }

    public void OnObjectEnterAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    {
    }

    public void OnObjectExitAOI(
        NetworkRunner runner,
        NetworkObject obj,
        PlayerRef player)
    {
    }

    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        float progress)
    {
    }
}