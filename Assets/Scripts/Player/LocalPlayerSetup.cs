using Fusion;
using System;
using UnityEngine;

public class LocalPlayerSetup : NetworkBehaviour
{
    public Camera playerCamera;
    public AudioListener audioListener;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            playerCamera.gameObject.SetActive(true);

            if (audioListener != null)
                audioListener.enabled = true;
        }
        else
        {
            playerCamera.gameObject.SetActive(false);

            if (audioListener != null)
                audioListener.enabled = false;
        }
    }
}