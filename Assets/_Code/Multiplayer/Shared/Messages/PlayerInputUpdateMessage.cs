using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

class PlayerInputUpdateMessage : MessageBase
{
    public PlayerInput input;

    public PlayerInputUpdateMessage() { }

    public PlayerInputUpdateMessage(PlayerInput input)
    {
        this.input = input;
    }

    public override void Serialize(NetworkWriter writer)
    {
        input.Serialize(writer);
    }

    public override void Deserialize(NetworkReader reader)
    {
        input.Deserialize(reader);
    }
}
