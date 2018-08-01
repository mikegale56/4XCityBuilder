using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ResourceDropdownCreator
{
	public static float imageSize = 64F;
	
	
	public static ResourceDropdown CreateResourceStaticView(Transform parent, Vector3 localPosition, ResourceQuantityQualityList choiceRqqList, string taskName, Domain domain)
    {
		return Create(parent, localPosition, choiceRqqList, taskName, domain, true);
	}
	
	public static ResourceDropdown CreateResourceChoiceDropdown(Transform parent, Vector3 localPosition, ResourceQuantityQualityList choiceRqqList, string taskName, Domain domain)
	{
		return Create(parent, localPosition, choiceRqqList, taskName, domain, false);
	}

    private static ResourceDropdown Create(Transform parent, Vector3 localPosition, ResourceQuantityQualityList choiceRqqList, string taskName, Domain domain, bool isStatic)
    {
		GameObject newGo = new GameObject("Resource Dropdown");
		ResourceDropdown resourceDropdown = newGo.AddComponent<ResourceDropdown>();
		
        resourceDropdown.choiceRqqList = choiceRqqList;
        resourceDropdown.taskName      = taskName;
		resourceDropdown.domain        = domain;

        int resInd = 0;
        foreach (ResourceQuantityQuality rqq in choiceRqqList.rqqList)
        {
            Dictionary<string, Sprite> stringSprite = rqq.GetImageOptions(rqq.minTier);
            
			// Create the button
			resourceDropdown.elements.Add(UIElementFunctions.Dropdown(parent, null, "", localPosition, new Vector2(64, 64)));
			//resourceDropdown[resInd].transform.localPosition = localPosition;
            
			localPosition.x += imageSize * 3 / 2;
            resourceDropdown.elements[resInd].thisGo.name = "Resource Dropdown Item";
            resourceDropdown.elements[resInd].childHeight = imageSize;

            int ind = 0;
            foreach (KeyValuePair<string, Sprite> entry in stringSprite)
            {
                ResourceNameQuantityQuality nqq = new ResourceNameQuantityQuality(entry.Key, QualityEnum.any, rqq.quantity);

                // do something with entry.Value or entry.Key
                if (ind == 0)
                {
                    resourceDropdown.elements[resInd].imageGo.sprite = entry.Value;
                    resourceDropdown.elements[resInd].textGo.text = "";
                    resourceDropdown.elements[resInd].defName = entry.Key;
                }
				if (!isStatic)
				{
                    resourceDropdown.elements[resInd].AddChild();
                    resourceDropdown.elements[resInd].children[ind].tooltipData = entry.Key;
                    resourceDropdown.elements[resInd].children[ind].defName = entry.Key;
                    resourceDropdown.elements[resInd].children[ind].imageGo.sprite = entry.Value;
				}
                
                if (!nqq.CheckResource(domain.stock))
                {
                    if (ind == 0)
                    {
                        resourceDropdown.elements[resInd].imageGo.color = Color.red;
                        resourceDropdown.elements[resInd].allowed = false;
                    }
					if (!isStatic)
					{
                        resourceDropdown.elements[resInd].children[ind].imageGo.color = Color.red;
                        resourceDropdown.elements[resInd].children[ind].buttonGo.interactable = false;
                        resourceDropdown.elements[resInd].children[ind].allowed = false;
					}
                }
				if (!isStatic)
                    resourceDropdown.elements[resInd].children[ind].textGo.text = "";
					
                ind++;
            }
            resInd++;   
        }
        return resourceDropdown;
    }
}