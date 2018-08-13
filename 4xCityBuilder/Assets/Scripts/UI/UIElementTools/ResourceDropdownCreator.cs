using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ResourceDropdownCreator
{
	public static float imageSize = 64F;

    private static Sprite rightArrow;


    public static ResourceDropdown CreateResourceStaticView(Transform parent, Vector3 localPosition, ResourceQuantityQualityList choiceRqqList, string taskName, Domain domain, Sprite resultSprite)
    {
        ResourceDropdown rd = Create(parent, localPosition, choiceRqqList, taskName, domain, true);
        AddUiElements(rd, parent, resultSprite, true);
        return rd;
	}
	
	public static ResourceDropdown CreateResourceChoiceDropdown(Transform parent, Vector3 localPosition, ResourceQuantityQualityList choiceRqqList, string taskName, Domain domain, Sprite resultSprite)
	{
        ResourceDropdown rd = Create(parent, localPosition, choiceRqqList, taskName, domain, false);
        AddUiElements(rd, parent, resultSprite, false);
        return rd;
    }

    public static ResourceDropdown CreateDemolishStaticView(Transform parent, Vector3 localPosition, string taskName, Domain domain, Sprite resultSprite)
    {
        ResourceDropdown rd = CreateDemolish(parent, localPosition, taskName, domain, resultSprite, true);
        JustUiElements(rd, parent, resultSprite, true, localPosition);
        return rd;
    }

    public static ResourceDropdown CreateDemolishChoiceDropdown(Transform parent, Vector3 localPosition, string taskName, Domain domain, Sprite resultSprite)
    {
        ResourceDropdown rd = CreateDemolish(parent, localPosition, taskName, domain, resultSprite, false);
        JustUiElements(rd, parent, resultSprite, false, localPosition);
        return rd;
    }

    private static ResourceDropdown CreateDemolish(Transform parent, Vector3 localPosition, string taskName, Domain domain, Sprite resultSprite, bool isStatic)
    {
        GameObject newGo = new GameObject("Resource Dropdown");
        ResourceDropdown resourceDropdown = newGo.AddComponent<ResourceDropdown>();
        resourceDropdown.taskName = taskName;
        resourceDropdown.domain = domain;
        JustUiElements(resourceDropdown, parent, resultSprite, isStatic, localPosition);
        return resourceDropdown;
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

                // Add a quantity text
                string quantityString = rqq.quantity.ToString();
                CustomUIElement temp = UIElementFunctions.TextOnly(resourceDropdown.elements[resInd].thisGo.transform, quantityString, new Vector3(0, imageSize - 20), new Vector2(imageSize, 20F));
                temp.textGo.alignment = TextAnchor.MiddleCenter;

                ind++;
            }
            resInd++;   
        }
        return resourceDropdown;
    }

    private static void AddUiElements(ResourceDropdown resourceDropdown, Transform parent, Sprite resultSprite, bool isStatic)
    {
        // Add stuff to the right of the resources
        Vector3 localPosition = resourceDropdown.elements[resourceDropdown.elements.Count - 1].transform.localPosition;
        JustUiElements(resourceDropdown, parent, resultSprite, isStatic, localPosition);
    }

    private static void JustUiElements(ResourceDropdown resourceDropdown, Transform parent, Sprite resultSprite, bool isStatic, Vector3 localPosition)
    {
        if (rightArrow == null)
        {
            Texture2D rightArrowTex = Resources.Load("Textures/RightArrow") as Texture2D;
            rightArrow = Sprite.Create(rightArrowTex,
                new Rect(0, 0, rightArrowTex.width, rightArrowTex.height),
                new Vector2(0.5f, 0.5f), rightArrowTex.width);
        }
        // Arrow 
        localPosition.x += ResourceDropdownCreator.imageSize * 4 / 2;
        resourceDropdown.uiElements.Add(UIElementFunctions.ImageOnly(parent, rightArrow, localPosition, new Vector2(imageSize, imageSize)));
        resourceDropdown.uiElements[resourceDropdown.uiElements.Count - 1].gameObject.name = "Job Arrow";
        // Result
        localPosition.x += ResourceDropdownCreator.imageSize * 3 / 2;
        resourceDropdown.uiElements.Add(UIElementFunctions.ImageOnly(parent, resultSprite, localPosition, new Vector2(imageSize, imageSize)));
        resourceDropdown.uiElements[resourceDropdown.uiElements.Count - 1].gameObject.name = "Job Result";

        if (!isStatic)
        {
            localPosition.x += ResourceDropdownCreator.imageSize * 3 / 2;
            resourceDropdown.jobStartButton = UIElementFunctions.ButtonTextColor(parent, "Go", Color.green, localPosition, new Vector2(imageSize, imageSize));
            resourceDropdown.jobStartButton.textGo.fontSize = 22;
            resourceDropdown.jobStartButton.buttonGo.interactable = false;
            resourceDropdown.jobStartButton.gameObject.name = "Job Start Button";
        }
    }
}