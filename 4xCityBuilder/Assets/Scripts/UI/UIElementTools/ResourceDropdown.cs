using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceDropdown : MonoBehaviour
{

    public List<DropdownUIElement> elements;
    public string taskName;
    public Domain domain;
    public ResourceQuantityQualityList choiceRqqList;
    public List<CustomUIElement> uiElements;
    public CustomUIElement jobStartButton;

    private bool selectable;

    public void Awake()
    {
        elements = new List<DropdownUIElement>();
        uiElements = new List<CustomUIElement>();
    }

    public void Update()
    {
        // Check if the go button should be active or not
        if (jobStartButton != null)
            jobStartButton.buttonGo.interactable = CheckResources();
    }

    public void ClearResourceList()
    {
        foreach (DropdownUIElement db in elements)
        {
            db.Hide();
            Destroy(db.thisGo);
        }
        foreach (CustomUIElement ui in uiElements)
        {
            ui.Hide();
            Destroy(ui.thisGo);
        }
        if (jobStartButton != null)
        {
            jobStartButton.Hide();
            Destroy(jobStartButton.thisGo);
        }
    }

    public bool CheckResources()
    {
        bool checkIsOk = true;
        foreach (DropdownUIElement rd in elements)
            checkIsOk = checkIsOk && rd.allowed;
        return checkIsOk;
    }

    public ResourceQuantityQualityList GetCurrentChoices()
    {
        ResourceQuantityQualityList currentChoices = new ResourceQuantityQualityList();
        int ind = 0;
        foreach (DropdownUIElement rd in elements)
        {
            currentChoices.rqqList.Add(new ResourceNameQuantityQuality(rd.defName, QualityEnum.any, choiceRqqList.rqqList[ind].quantity));
            ind++;
        }
        return currentChoices;
    }
}