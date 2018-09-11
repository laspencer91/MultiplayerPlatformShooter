using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum NetMessageId
{
    ConnectionRequest,
    ClientInitialization,
    ClientDisconnected,
    ClientConnected,
    PlayerSpawn,
    AllPlayerSpawnData,
    PlayerInputUpdate
}
