using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public GameObject runnerModel;
    public GameObject zombieModel;

    public void SetRole(PlayerRole role)
    {
        runnerModel.SetActive(role == PlayerRole.Runner);
        zombieModel.SetActive(role == PlayerRole.Zombie);
    }
}