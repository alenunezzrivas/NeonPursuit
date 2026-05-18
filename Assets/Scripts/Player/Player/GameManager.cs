using Fusion;
using System;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Networked] private float timer { get; set; }
    [Networked] private bool gameStarted { get; set; }
    [Networked] private bool gameEnded { get; set; }
    [Networked] private float countdown { get; set; }
    [Networked] private NetworkBool rolesAssigned { get; set; }

    public float matchTime = 120f;
    public float startCountdown = 3f;

    public GameObject resultUI;
    public TMP_Text resultText;

    public TMP_Text timerText;
    public TMP_Text statusText;



    private static GameManager instance;
    private int logTicker;
    private PlayerSpawner spawner;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        timer = matchTime;
        countdown = startCountdown;
        gameStarted = false;
        gameEnded = false;
        rolesAssigned = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (gameEnded) return;

        int playerCount = GetPlayerCount();

        if (Runner != null && Runner.IsRunning && (logTicker++ % 120) == 0)
        {
            string pids = "";
            foreach (var p in Runner.ActivePlayers) pids += p.PlayerId + ",";
            Debug.Log($"[GameManager] Players: {playerCount} [{pids}] started={gameStarted} roles={rolesAssigned}");
        }

        if (!gameStarted)
        {
            if (playerCount < 2)
            {
                Rpc_UpdateStatus("Esperando jugadores...");
                return;
            }

            if (!rolesAssigned)
            {
                if (AreAllPlayersReady())
                {
                    if (spawner == null)
                        spawner = FindObjectOfType<PlayerSpawner>();

                    spawner?.AssignRolesNow();
                    rolesAssigned = true;
                    countdown = startCountdown;
                }
                else
                {
                    int readyCount = CountReadyPlayers();
                    Rpc_UpdateStatus($"Presiona ESPACIO para listo ({readyCount}/{playerCount})");
                }
                return;
            }

            countdown -= Runner.DeltaTime;
            Rpc_UpdateStatus("Empieza en: " + Mathf.Ceil(countdown));

            if (countdown <= 0)
            {
                gameStarted = true;
                Rpc_UpdateStatus("");
            }

            return;
        }

        timer -= Runner.DeltaTime;
        Rpc_UpdateTimer(Mathf.Ceil(timer).ToString());

        CheckWinCondition();

        if (timer <= 0)
        {
            EndGame("Ganan los Runners");
        }
    }

    bool AreAllPlayersReady()
    {
        var roles = FindObjectsOfType<PlayerRole>();
        int activeCount = GetPlayerCount();
        if (roles.Length < activeCount || activeCount < 2) return false;
        foreach (var r in roles)
        {
            if (!r.ready) return false;
        }
        return true;
    }

    int CountReadyPlayers()
    {
        int count = 0;
        var roles = FindObjectsOfType<PlayerRole>();
        foreach (var r in roles)
        {
            if (r.ready) count++;
        }
        return count;
    }

    int GetPlayerCount()
    {
        int count = 0;
        foreach (var p in Runner.ActivePlayers) count++;
        return count;
    }

    void CheckWinCondition()
    {
        if (!gameStarted) return;

        int hunters = 0;
        int runners = 0;

        var players = FindObjectsOfType<PlayerRole>();

        foreach (var p in players)
        {
            if (p.IsHunter()) hunters++;
            else runners++;
        }

        if (hunters > 0 && runners == 0)
        {
            EndGame("Ganan los Hunters");
        }
    }

    void EndGame(string result)
    {
        if (gameEnded) return;

        gameEnded = true;

        Rpc_ShowResult(result);

        Invoke(nameof(ShutdownGame), 4f);
    }

    void ShutdownGame()
    {
        if (Runner != null)
            Runner.Shutdown();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void Rpc_ShowResult(string result)
    {
        resultUI.SetActive(true);
        resultText.text = result;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void Rpc_UpdateTimer(string t)
    {
        timerText.text = t;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void Rpc_UpdateStatus(string s)
    {
        statusText.text = s;
    }
}