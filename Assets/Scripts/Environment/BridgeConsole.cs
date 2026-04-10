using Fusion;
using UnityEngine;

public class BridgeConsole : NetworkBehaviour
{
    public BridgeMovement bridge;
    public GameObject messageUI;

    public float activationDistance = 2f;

    private Transform player;
    private AudioSource audioSource;

    private static BridgeConsole currentConsole;

    public override void Spawned()
    {
        audioSource = GetComponent<AudioSource>();

        if (messageUI != null)
            messageUI.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        // 👇 SOLO el jugador local ejecuta esto
        if (!Object.HasInputAuthority) return;

        // 🔍 Buscar player local
        if (player == null)
        {
            var players = FindObjectsOfType<PlayerRole>();

            foreach (var p in players)
            {
                if (p.Object != null && p.Object.HasInputAuthority)
                {
                    player = p.transform;
                    break;
                }
            }
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 🎯 Seleccionar consola más cercana
        if (distance <= activationDistance)
        {
            if (currentConsole == null ||
                distance < Vector3.Distance(currentConsole.transform.position, player.position))
            {
                currentConsole = this;
            }
        }

        // 🎮 UI + INPUT
        if (currentConsole == this && distance <= activationDistance)
        {
            if (messageUI != null && !messageUI.activeSelf)
                messageUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                RPC_RequestToggle();
            }
        }
        else
        {
            if (messageUI != null && messageUI.activeSelf)
                messageUI.SetActive(false);

            if (currentConsole == this)
                currentConsole = null;
        }
    }

    // 🔥 CLIENT → SERVER
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestToggle()
    {
        if (bridge != null)
        {
            bridge.ToggleBridge();

            if (audioSource != null)
                audioSource.Play();
        }
    }
}