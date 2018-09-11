using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

interface INetworkSerializable
{
    void Serialize(NetworkWriter writer);
    void Deserialize(NetworkReader reader);
}

class StaticINetworkSerializable
{
    public static T Serialize<T>(NetworkWriter writer) where T : INetworkSerializable
    {
        T newObj = (T)Activator.CreateInstance(typeof(T));
        newObj.Serialize(writer);
        return newObj;
    }
    public static T Deserialize<T>(NetworkReader reader) where T : INetworkSerializable
    {
        T newObj = (T)Activator.CreateInstance(typeof(T));
        newObj.Deserialize(reader);
        return newObj;
    }
}