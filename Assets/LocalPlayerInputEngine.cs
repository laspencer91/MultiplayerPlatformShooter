using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public struct PlayerInput
{
    public ChangeWeaponAction changeWeaponAction;
    public bool Fire1Button;
    public bool JumpButtonDown;
    public bool JumpButtonReleased;
    public float VerticalAxisInput;
    public float HorizontalAxisInput;
}

public class LocalPlayerInputEngine : MonoBehaviour
{
    PlayerInput input = new PlayerInput();
    Player      player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update ()
    {
        if (CrossPlatformInputManager.GetButtonDown("ChangeToPrimary"))
            input.changeWeaponAction = ChangeWeaponAction.Primary;
        else if (CrossPlatformInputManager.GetButtonDown("ChangeToSecondary"))
            input.changeWeaponAction = ChangeWeaponAction.Secondary;
        else
            input.changeWeaponAction = ChangeWeaponAction.None;

        input.Fire1Button             = CrossPlatformInputManager.GetButton("Fire1");
        input.JumpButtonDown          = CrossPlatformInputManager.GetButtonDown("Jump");
        input.JumpButtonReleased      = CrossPlatformInputManager.GetButtonUp("Jump");
        input.VerticalAxisInput       = CrossPlatformInputManager.GetAxis("Vertical");
        input.HorizontalAxisInput     = CrossPlatformInputManager.GetAxis("Horizontal");

        player.ExecuteUpdate(input);
    }
}
