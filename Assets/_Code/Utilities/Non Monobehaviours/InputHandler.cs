using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityStandardAssets.CrossPlatformInput;

class InputHandler
{
    public static PlayerInput PollLocalPlayerInput()
    {
        PlayerInput input = new PlayerInput();

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

        return input;
    }
}

public struct PlayerInput
{
    public ChangeWeaponAction changeWeaponAction;
    public bool Fire1Button;
    public bool JumpButtonDown;
    public bool JumpButtonReleased;
    public float VerticalAxisInput;
    public float HorizontalAxisInput;

    public override string ToString()
    {
        return "Jump Button Down: " + JumpButtonDown + "\nJumpButtonReleased: " + JumpButtonReleased;
    }
}