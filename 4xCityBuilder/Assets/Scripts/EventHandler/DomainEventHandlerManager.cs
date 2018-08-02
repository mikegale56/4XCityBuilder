using System;
using System.Collections.Generic;
using UnityEngine;

public class DomainEventHandlerManager
{
    public Dictionary<domainEventChannels, Dictionary<Enum, DomainEventHandler>> ListenerFunctions = initializeDicts();

    public void Broadcast(domainEventChannels channel, Enum ev, DomainEventArg e)
    {
        ListenerFunctions[channel][ev](e);
    }

    public void AddListener(domainEventChannels evType, Enum ev, DomainEventHandler eventListener)
    {
        ListenerFunctions[evType][ev] += eventListener;
    }
    public void RemoveListener(domainEventChannels evType, Enum ev, DomainEventHandler eventListener)
    {
        ListenerFunctions[evType][ev] -= eventListener;
    }

    public void OnDestroy()
    {
        ListenerFunctions = initializeDicts();
    }

    static Dictionary<domainEventChannels, Dictionary<Enum, DomainEventHandler>> initializeDicts()
    {
        Dictionary<domainEventChannels, Array> enumChannelEventList = DomainChannelEnums.getChannelEnumList();
        Dictionary<domainEventChannels, Dictionary<Enum, DomainEventHandler>> result = new Dictionary<domainEventChannels, Dictionary<Enum, DomainEventHandler>>();
        foreach (var val in (domainEventChannels[])Enum.GetValues(typeof(domainEventChannels)))
        {
            result.Add(val, new Dictionary<Enum, DomainEventHandler>());
            foreach (var ev in enumChannelEventList[val])
            {
                result[val].Add((Enum)ev, new DomainEventHandler(delegate (DomainEventArg e) { }));
            }
        }
        return result;
    }
}

public enum domainEventChannels
{
    job
}

public enum jobChannelEvents
{
    constructionComplete,
    jobComplete
}

public class DomainEventArg : System.EventArgs
{
    public Vector3Int location;
    public string message;
    public DomainEventArg(string mes, Vector3Int loc)
    {
        this.message = mes;
        this.location = loc;
    }

    public DomainEventArg(string mes, int i, int j)
    {
        this.message = mes;
        this.location = new Vector3Int(i, j, 0);
    }
}

public delegate void DomainEventHandler(DomainEventArg e);

public class DomainChannelEnums
{
    public static Dictionary<domainEventChannels, System.Array> getChannelEnumList()
    {

        Dictionary<domainEventChannels, System.Array> enumChannelEventList = new Dictionary<domainEventChannels, System.Array>();
        enumChannelEventList.Add(domainEventChannels.job, System.Enum.GetValues(typeof(jobChannelEvents)));
        return enumChannelEventList;
    }
}