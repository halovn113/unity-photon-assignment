using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 Move;
    public Vector2 Look;
    public NetworkButtons Buttons;

    public const byte BUTTON_JUMP = 0;
    public const byte BUTTON_RUN = 1;
}