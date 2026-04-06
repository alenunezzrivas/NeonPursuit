using UnityEngine;
using StarterAssets;

public class PlayerRoleController : MonoBehaviour
{
    public ThirdPersonController runnerController;
    public MonoBehaviour zombieController;
    public PlayerVisual visual;

    public void SetRole(PlayerRole role)
    {
        // Activar controladores
        runnerController.enabled = (role == PlayerRole.Runner);
        zombieController.enabled = (role == PlayerRole.Zombie);

        // Cambiar modelo
        visual.SetRole(role);
    }
}