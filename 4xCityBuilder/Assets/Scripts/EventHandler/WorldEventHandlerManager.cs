using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventHandlerManager
{
    public static Dictionary<worldEventChannels, Dictionary<Enum, WorldEventHandler>> ListenerFunctions = initializeDicts();

    public static void Broadcast(worldEventChannels channel, Enum ev, WorldEventArg e)
    {
        ListenerFunctions[channel][ev](e);
    }

    public static void AddListener(worldEventChannels evType, Enum ev, WorldEventHandler eventListener)
    {
        ListenerFunctions[evType][ev] += eventListener;
    }
    public static void RemoveListener(worldEventChannels evType, Enum ev, WorldEventHandler eventListener)
    {
        ListenerFunctions[evType][ev] -= eventListener;
    }

    public void OnDestroy()
    {
        ListenerFunctions = initializeDicts();
    }

    static Dictionary<worldEventChannels, Dictionary<Enum, WorldEventHandler>> initializeDicts()
    {
        Dictionary<worldEventChannels, Array> enumChannelEventList = WorldChannelEnums.getChannelEnumList();
        Dictionary<worldEventChannels, Dictionary<Enum, WorldEventHandler>> result = new Dictionary<worldEventChannels, Dictionary<Enum, WorldEventHandler>>();
        foreach (var val in (worldEventChannels[])Enum.GetValues(typeof(worldEventChannels)))
        {
            result.Add(val, new Dictionary<Enum, WorldEventHandler>());
            foreach (var ev in enumChannelEventList[val])
            {
                result[val].Add((Enum)ev, new WorldEventHandler(delegate (WorldEventArg e) { }));
            }
        }
        return result;
    }
}

public enum worldEventChannels
{
    map
}

public enum mapChannelEvents
{
    change
}
 
public class WorldEventArg : System.EventArgs
{
    public Vector3Int location;
    public WorldEventArg(Vector3Int loc)
    {
        this.location = loc;
    }

    public WorldEventArg(int i, int j)
    {
        this.location = new Vector3Int(i, j, 0);
    }
}

public delegate void WorldEventHandler(WorldEventArg e);

public class WorldChannelEnums
{
    public static Dictionary<worldEventChannels, System.Array> getChannelEnumList()
    {

        Dictionary<worldEventChannels, System.Array> enumChannelEventList = new Dictionary<worldEventChannels, System.Array>();
        enumChannelEventList.Add(worldEventChannels.map, System.Enum.GetValues(typeof(mapChannelEvents)));
        return enumChannelEventList;
    }
}