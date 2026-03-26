using UnityEngine;

public class BridgeConsole : MonoBehaviour
{
    public BridgeMovement bridge;
    public Transform player;
    public GameObject messageUI;

    public float activationDistance = 2f;

    private AudioSource audioSource;

    // Referencia global a la consola activa
    private static BridgeConsole currentConsole;

    void Start()
    {
        if (messageUI != null)
        {
            messageUI.SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Si está dentro del rango
        if (distance < activationDistance)
        {
            // Esta consola pasa a ser la activa
            if (currentConsole == null || distance < Vector3.Distance(currentConsole.transform.position, player.position))
            {
                currentConsole = this;
            }
        }

        // SOLO la consola activa controla el UI
        if (currentConsole == this)
        {
            if (distance < activationDistance)
            {
                if (messageUI != null)
                    messageUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    ToggleBridge();
                }
            }
            else
            {
                if (messageUI != null)
                    messageUI.SetActive(false);

                currentConsole = null;
            }
        }
        else
        {
            if (messageUI != null)
                messageUI.SetActive(false);
        }
    }

    void ToggleBridge()
    {
        if (bridge == null) return;

        bridge.ToggleBridge();

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}