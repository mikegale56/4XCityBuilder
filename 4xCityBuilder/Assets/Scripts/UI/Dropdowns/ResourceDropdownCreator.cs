using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceDropdownCreator : MonoBehaviour
{

    public RectTransform panel;
    public List<DropdownBase> resourceDropdown;
    public Domain domain;
    public string buildingName;
    public float imageSize = 64F;


    private ResourceQuantityQualityList choiceRqqList;

    // Use this for initialization
    void Start ()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            EventSystem e = new GameObject().AddComponent<EventSystem>();
            e.name = "Event System";
            e.gameObject.AddComponent<StandaloneInputModule>();
        }
        domain = ManagerBase.domain;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ClearResourceList()
    {
        foreach (DropdownBase db in resourceDropdown)
        {
            db.HideAll();
            Destroy(db.thisGo);
        }
        resourceDropdown = new List<DropdownBase>();
    }

    public bool CheckResources()
    {
        bool checkIsOk = true;
        foreach (DropdownBase rd in resourceDropdown)
            checkIsOk = checkIsOk && rd.allowed;
        return checkIsOk;
    }

    public ResourceQuantityQualityList GetCurrentChoices()
    {
        ResourceQuantityQualityList currentChoices = new ResourceQuantityQualityList();
        int ind = 0;
        foreach (DropdownBase rd in resourceDropdown)
        {
            currentChoices.rqqList.Add(new ResourceNameQuantityQuality(rd.ddName, QualityEnum.any, choiceRqqList.rqqList[ind].quantity));
            ind++;
        }
        return currentChoices;
    }

    public void CreateResourceChoiceDropdown(Vector3 localPosition, ResourceQuantityQualityList choices, string buildingName)
    {
        this.choiceRqqList = choices;
        this.buildingName = buildingName;

        // Delete the old ones if they exist
        if (resourceDropdown == null)
            resourceDropdown = new List<DropdownBase>();
        else
            ClearResourceList();

        int resInd = 0;
        foreach (ResourceQuantityQuality rqq in choices.rqqList)
        {
            Dictionary<string, Sprite> stringSprite = rqq.GetImageOptions(rqq.minTier);
            // Create the button
            
            resourceDropdown.Add(DropdownUtilities.NewButton("Resource " + resInd.ToString() + " Dropdown", "", panel.transform, imageSize, imageSize).gameObject.AddComponent<DropdownBase>());
            resourceDropdown[resInd].transform.localPosition = localPosition;
            localPosition.x += imageSize * 3 / 2;
            resourceDropdown[resInd].childHeight = imageSize;

            int ind = 0;
            foreach (KeyValuePair<string, Sprite> entry in stringSprite)
            {
                ResourceNameQuantityQuality nqq = new ResourceNameQuantityQuality(entry.Key, QualityEnum.any, rqq.quantity);

                // do something with entry.Value or entry.Key
                if (ind == 0)
                {
                    resourceDropdown[resInd].imageGo.sprite = entry.Value;
                    resourceDropdown[resInd].textGo.text = "";
                    resourceDropdown[resInd].ddName = entry.Key;
                }
                resourceDropdown[resInd].AddChild();
                resourceDropdown[resInd].children[ind].tooltipData = entry.Key;
                resourceDropdown[resInd].children[ind].ddName = entry.Key;
                resourceDropdown[resInd].children[ind].imageGo.sprite = entry.Value;
                
                if (!nqq.CheckResource(domain.stock))
                {
                    if (ind == 0)
                    {
                        resourceDropdown[resInd].imageGo.color = Color.red;
                        resourceDropdown[resInd].allowed = false;
                    }
                    resourceDropdown[resInd].children[ind].imageGo.color = Color.red;
                    resourceDropdown[resInd].children[ind].button.interactable = false;
                    resourceDropdown[resInd].children[ind].allowed = false;
                }

                resourceDropdown[resInd].children[ind].textGo.text = "";
                ind++;
            }
            resInd++;   
        }
    }

}
