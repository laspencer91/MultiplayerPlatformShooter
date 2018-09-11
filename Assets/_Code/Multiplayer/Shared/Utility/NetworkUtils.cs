using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

class NetworkUtils
{
    /// <summary>
    /// Will return a List of the type given. The type given must implement INetworkSerializable.
    /// </summary>
    /// <typeparam name="T">Type of INetworkSerializable To Create A List From</typeparam>
    /// <param name="count">Number of T to read from the reasder</param>
    /// <param name="reader">Network reader with position set ready to read the first T</param>
    /// <returns>List of T as read from reader</returns>
    public static List<T> ReadList<T>(int count, NetworkReader reader) where T : INetworkSerializable
    {
        List<T> list = new List<T>();

        for (int i = 0; i < count; i++)
        {
            T newObj = StaticINetworkSerializable.Deserialize<T>(reader);
            list.Add(newObj);
        }

        return list;
    }

    /// <summary>
    /// Will return a List of the type given. The type given must implement INetworkSerializable.
    /// This will read a single byte as the Count of the expected list and then deserialize the list
    /// </summary>
    /// <typeparam name="T">Type of INetworkSerializable To Create A List From</typeparam>
    /// <param name="reader">Network reader with position set ready to read the first T</param>
    /// <returns>List of T as read from reader</returns>
    public static List<T> ReadByteCountAndList<T>(NetworkReader reader) where T : INetworkSerializable
    {
        byte count = reader.ReadByte();
        List<T> list = new List<T>();

        for (int i = 0; i < count; i++)
        {
            T newObj = StaticINetworkSerializable.Deserialize<T>(reader);
            list.Add(newObj);
        }

        return list;
    }

    /// <summary>
    /// Writes a small list (Count less than or equal to size of a byte) to a NetworkWriter.
    /// The Count of the list is stored as the Byte directly before the list entries start.
    /// </summary>
    /// <typeparam name="T">Class that implements INetworkSerializable</typeparam>
    /// <param name="list">List of INetworkSerializable implementers</param>
    /// <param name="writer">NetworkWriter to write the list to</param>
    public static void WriteByteCountAndList<T>(List<T> list, NetworkWriter writer) where T : INetworkSerializable
    {
        if (list.Count > byte.MaxValue)
            throw new NotSupportedException("This method cannot write a list with size greater than " + byte.MaxValue);

        writer.Write((byte)list.Count);          // Write how many in the list

        foreach (T entry in list)
        {
            entry.Serialize(writer);
        }
    }
}
