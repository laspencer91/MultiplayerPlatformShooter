using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public struct PlayerInput : INetworkSerializable
{
    public ChangeWeaponAction changeWeaponAction;
    public bool Fire1Button;
    public bool JumpButtonDown;
    public bool JumpButtonReleased;
    public float VerticalAxisInput;
    public float HorizontalAxisInput;

    public void Serialize(NetworkWriter writer)
    {
        writer.Write((byte)changeWeaponAction);
        writer.Write(Fire1Button);
        writer.Write(JumpButtonDown);
        writer.Write(JumpButtonReleased);
        writer.Write(VerticalAxisInput);
        writer.Write(HorizontalAxisInput);
    }

    public void Deserialize(NetworkReader reader)
    {
        changeWeaponAction  = (ChangeWeaponAction)reader.ReadByte();
        Fire1Button         = reader.ReadBoolean();
        JumpButtonDown      = reader.ReadBoolean();
        JumpButtonReleased  = reader.ReadBoolean();
        VerticalAxisInput   = reader.ReadSingle();
        HorizontalAxisInput = reader.ReadSingle();
    }

    public bool Equals(PlayerInput other)
    {
        return changeWeaponAction  == other.changeWeaponAction &&
               Fire1Button         == other.Fire1Button &&
               JumpButtonDown      == other.JumpButtonDown &&
               JumpButtonReleased  == other.JumpButtonReleased &&
               VerticalAxisInput   == other.VerticalAxisInput &&
               HorizontalAxisInput == other.HorizontalAxisInput;
    }

    public override string ToString()
    {
        return "Jump Button Down: " + JumpButtonDown + "\nJumpButtonReleased: " + JumpButtonReleased;
    }
}

public class LocalPlayerInputEngine : MonoBehaviour
{
    PlayerInput    input             = new PlayerInput();
    PlayerInput    previousInput     = new PlayerInput();
    Player         player;
    NetworkManager netManager;

    Queue<PlayerInput> networkInputs = new Queue<PlayerInput>();

    bool isOnlineGameSession;

    private void Awake()
    {
        player = GetComponent<Player>();
        isOnlineGameSession = GameSession.type == GameSessionType.online;

        if (isOnlineGameSession)
            netManager = FindObjectOfType<NetworkManager>();
    }

    void Update ()
    {
        if (player.isLocalPlayer)
        {
            HandleLocalInput();
        }
        else if (netManager.isServer())
        {
            HandleServerInput();
        }

        // Reset Inputs needed to reset every frame.. no matter netwoked or not
        input.JumpButtonDown     = false;
        input.JumpButtonReleased = false;
    }

    private void HandleLocalInput()
    {
        if (CrossPlatformInputManager.GetButtonDown("ChangeToPrimary"))
            input.changeWeaponAction = ChangeWeaponAction.Primary;
        else if (CrossPlatformInputManager.GetButtonDown("ChangeToSecondary"))
            input.changeWeaponAction = ChangeWeaponAction.Secondary;
        else
            input.changeWeaponAction = ChangeWeaponAction.None;

        input.Fire1Button = CrossPlatformInputManager.GetButton("Fire1");
        input.JumpButtonDown = CrossPlatformInputManager.GetButtonDown("Jump");
        input.JumpButtonReleased = CrossPlatformInputManager.GetButtonUp("Jump");
        input.VerticalAxisInput = CrossPlatformInputManager.GetAxis("Vertical");
        input.HorizontalAxisInput = CrossPlatformInputManager.GetAxis("Horizontal");

        if (isOnlineGameSession)
        {
            if (!netManager.isServer() && !previousInput.Equals(input))
            {
                ((ClientNetworkManager)netManager).SendPlayerInput(input);
            }
        }

        previousInput = input;
        player.ExecuteUpdate(input);
    }

    private void HandleServerInput()
    {
        bool updatedThisFrame = false;

        while (networkInputs.Count > 0)
        {
            player.ExecuteUpdate(networkInputs.Dequeue());
            updatedThisFrame = true;
        }

        if (!updatedThisFrame)
            player.ExecuteUpdate(input);
    }

    public void SetInput(PlayerInput input)
    {
        this.input = input;
        networkInputs.Enqueue(input);
        Debug.Log(input.ToString());
    }
}
