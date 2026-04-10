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

    public float matchTime = 120f;
    public float startCountdown = 3f;

    public GameObject resultUI;
    public TMP_Text resultText;

    public TMP_Text timerText;
    public TMP_Text statusText;

    private static GameManager instance;

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
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (gameEnded) return;

        int playerCount = GetPlayerCount();

        // ESPERANDO JUGADORES
        if (!gameStarted)
        {
            if (playerCount < 2)
            {
                Rpc_UpdateStatus("Esperando jugadores...");
                return;
            }

            // COUNTDOWN
            countdown -= Runner.DeltaTime;
            Rpc_UpdateStatus("Empieza en: " + Mathf.Ceil(countdown));

            if (countdown <= 0)
            {
                gameStarted = true;
                Rpc_UpdateStatus("");
            }

            return;
        }

        // PARTIDA
        timer -= Runner.DeltaTime;
        Rpc_UpdateTimer(Mathf.Ceil(timer).ToString());

        CheckWinCondition();

        if (timer <= 0)
        {
            EndGame("Ganan los Runners");
        }
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