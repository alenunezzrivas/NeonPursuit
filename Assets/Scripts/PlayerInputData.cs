using Fusion;

public struct PlayerInputData : INetworkInput
{
    public float horizontal;
    public float vertical;
    public bool jump;
    public bool sprint;

    public float mouseX; // 🔥 NUEVO
}