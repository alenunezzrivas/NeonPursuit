using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    void Awake()
    {
        // Intenta encontrar el texto automáticamente si no está asignado
        if (healthText == null)
        {
            healthText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = "Vida: " + currentHealth;
        }
        else
        {
            Debug.LogWarning("HealthUI: No hay TextMeshPro asignado.");
        }
    }
}