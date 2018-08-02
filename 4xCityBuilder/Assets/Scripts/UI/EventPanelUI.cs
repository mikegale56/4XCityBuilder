using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelUI : MonoBehaviour {

    private List<EventPanelButton> uiElements;
    public RectTransform eventVlgRect;

    public int jobEventHeight = 25;
    public Color jobColor = new Color(0.8F, 1.0F, 0.8F);
    public float jobEventPersist = 30; // Seconds

    public int constructionEventHeight = 25;
    public Color constructionColor = new Color(0.8F, 0.8F, 1.0F);
    public float constructionEventPersist = 60; // Seconds

    void Start ()
    {
        uiElements = new List<EventPanelButton>();

        // Register a Listener for domain events
        DomainEventHandler pjc = new DomainEventHandler(ProcessJobComplete);
        DomainEventHandler pcc = new DomainEventHandler(ProcessConstructionComplete);
        ManagerBase.domain.eventManager.AddListener(domainEventChannels.job, jobChannelEvents.jobComplete, pjc);
        ManagerBase.domain.eventManager.AddListener(domainEventChannels.job, jobChannelEvents.constructionComplete, pcc);

        // Set PruneList to invoke starting in 0 seconds (now), once a second
        InvokeRepeating("PruneList", 0, 1.0F);

    }

    public void ProcessJobComplete(DomainEventArg de)
    {
        CustomUIElement newEvent = UIElementFunctions.ButtonImageText(eventVlgRect.transform, null, de.message, new Vector3(0, 0, 0), new Vector2(0, 0));
        LayoutElement element = newEvent.thisGo.AddComponent<LayoutElement>();
        element.minHeight = this.jobEventHeight;
        uiElements.Add(new EventPanelButton(newEvent, jobEventPersist));
    }

    public void ProcessConstructionComplete(DomainEventArg de)
    {
        CustomUIElement newEvent = UIElementFunctions.ButtonImageText(eventVlgRect.transform, null, de.message, new Vector3(0, 0, 0), new Vector2(0, 0));
        LayoutElement element = newEvent.thisGo.AddComponent<LayoutElement>();
        element.minHeight = this.constructionEventHeight;
        newEvent.imageGo.color = this.constructionColor;
        uiElements.Add(new EventPanelButton(newEvent, constructionEventPersist));
    }

    // Update is called once per frame
    void PruneList ()
    {
        uiElements.Sort();
        float now = Time.time;
        int ind = 0;
        while (uiElements.Count > 0 && now >= uiElements[ind].removeAtTime)
        {
            Destroy(uiElements[ind].uiElement.thisGo);
            uiElements.RemoveAt(ind);
        }
	}
}

public class EventPanelButton : IComparable<EventPanelButton>
{
    public CustomUIElement uiElement;
    public float removeAtTime;

    public EventPanelButton(CustomUIElement uie, float persistDuration)
    {
        this.uiElement = uie;
        this.removeAtTime = Time.time + persistDuration;
    }

    public int CompareTo(EventPanelButton epb)
    {       // A null value means that this object is greater.
        if (epb == null)
            return 1;
        else
            return this.removeAtTime.CompareTo(epb.removeAtTime);
    }

}

