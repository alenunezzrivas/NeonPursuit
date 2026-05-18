using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerFusion : MonoBehaviour, INetworkRunnerCallbacks
{
    public GameObject runnerPrefab;
    public PlayerSpawner playerSpawner;
    public GameObject mainMenuUI;

    private NetworkRunner runner;

    public async void StartGame()
    {
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        runner = FindObjectOfType<NetworkRunner>();

        if (runner == null && runnerPrefab != null)
        {
            GameObject obj = Instantiate(runnerPrefab);
            obj.name = "NetworkRunner";
            runner = obj.GetComponent<NetworkRunner>();
        }

        if (runner == null)
        {
            Debug.LogError("No NetworkRunner found or could not be created. Assign runnerPrefab in the inspector.");
            return;
        }

        if (runner.IsRunning)
        {
            Debug.Log("Fusion is already running");
            return;
        }

        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        if (playerSpawner == null)
            playerSpawner = FindObjectOfType<PlayerSpawner>();

        if (playerSpawner == null)
        {
            Debug.LogError("PlayerSpawner not found in scene");
            return;
        }

        NetworkSceneManagerDefault sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestRoom",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = sceneManager
        });

        if (!result.Ok)
        {
            Debug.LogError($"Fusion failed to start: {result.ShutdownReason}");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
            playerSpawner.SpawnPlayer(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerInputData data = new PlayerInputData();

        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            var mouse = UnityEngine.InputSystem.Mouse.current;

            Vector2 moveDir = Vector2.zero;
            if (keyboard.wKey.isPressed) moveDir.y += 1f;
            if (keyboard.sKey.isPressed) moveDir.y -= 1f;
            if (keyboard.aKey.isPressed) moveDir.x -= 1f;
            if (keyboard.dKey.isPressed) moveDir.x += 1f;
            if (moveDir.magnitude > 1f) moveDir.Normalize();
            data.move = moveDir;

            if (mouse != null)
                data.look = mouse.delta.ReadValue() * 0.01f;

            data.jump = keyboard.qKey.isPressed;
            data.ready = keyboard.spaceKey.wasPressedThisFrame;
        }
        else
        {
            data.move = new Vector2(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical")
            );
            data.jump = UnityEngine.Input.GetKey(KeyCode.Q);
            data.ready = UnityEngine.Input.GetKeyDown(KeyCode.Space);
        }

        input.Set(data);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogError("Disconnected from server: " + reason);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError("Connection failed: " + reason);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning("Fusion shut down: " + shutdownReason);
    }

    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
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
