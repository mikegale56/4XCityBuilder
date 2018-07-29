using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceDropdownCreator : MonoBehaviour
{

    public RectTransform panel;
    public List<DropdownBase> resourceDropdown;
    public ResourceManager resourceManager;
    public float imageSize = 64F;

    // Use this for initialization
    void Start ()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            EventSystem e = new GameObject().AddComponent<EventSystem>();
            e.name = "Event System";
            e.gameObject.AddComponent<StandaloneInputModule>();

        }

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

    public void CreateResourceChoiceDropdown(Vector3 localPosition, ResourceQuantityQualityList choices)
    {
        // Delete the old ones if they exist
        if (resourceDropdown == null)
            resourceDropdown = new List<DropdownBase>();
        else
            ClearResourceList();

        int resInd = 0;
        foreach (ResourceQuantityQuality rqq in choices.rqqList)
        {
            Dictionary<string, Sprite> stringSprite = rqq.GetImageOptions(resourceManager, rqq.minTier);
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
                }
                resourceDropdown[resInd].AddChild();
                resourceDropdown[resInd].children[ind].tooltipData = entry.Key;
                resourceDropdown[resInd].children[ind].imageGo.sprite = entry.Value;
                
                if (!nqq.CheckResource(resourceManager.domainResources))
                {
                    if (ind == 0)
                        resourceDropdown[resInd].imageGo.color = Color.red;
                    resourceDropdown[resInd].children[ind].imageGo.color = Color.red;
                    resourceDropdown[resInd].children[ind].button.interactable = false;
                }

                resourceDropdown[resInd].children[ind].textGo.text = "";
                ind++;
            }
            resInd++;   
        }
    }

}
