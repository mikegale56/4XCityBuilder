using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceDropdownCreator : MonoBehaviour
{

    public RectTransform panel;
    public ResourceDropdown resourceDropdown;
    public ResourceManager resourceManager;

    // Use this for initialization
    void Start ()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            EventSystem e = new GameObject().AddComponent<EventSystem>();
            e.name = "Event System";
            e.gameObject.AddComponent<StandaloneInputModule>();

        }
        // Test Code
        ResourceQuantityQualityList choices = new ResourceQuantityQualityList();
        choices.rqqList.Add(new ResourceNameQuantityQuality("Cow", QualityEnum.any, 10));
        choices.rqqList.Add(new ResourceTypeQuantityQuality("Wood", QualityEnum.any, 10));
        choices.rqqList.Add(new ResourceTypeQuantityQuality("Metal", QualityEnum.any, 10));
        CreateDropdown(choices);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CreateDropdown(ResourceQuantityQualityList choices)
    {

        Dictionary<string, Sprite> stringSprite = choices.rqqList[2].GetImageOptions(resourceManager);

        // Create the button
        float imageSize = 64F;
        resourceDropdown = DropdownUtilities.NewButton("Resource 1 Dropdown", "Need text here", panel.transform, imageSize, imageSize).gameObject.AddComponent<ResourceDropdown>();
        resourceDropdown.childHeight = imageSize;
        resourceDropdown.mainText = resourceDropdown.transform.Find("Text").GetComponent<Text>();
        resourceDropdown.mainText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        resourceDropdown.mainText.color = Color.red;

        int ind = 0;
        foreach (KeyValuePair<string, Sprite> entry in stringSprite)
        {
            // do something with entry.Value or entry.Key
            if (ind == 0)
            {
                resourceDropdown.image.sprite = entry.Value;
                resourceDropdown.mainText.text = "";
            }
            resourceDropdown.AddChild(entry.Key);
            resourceDropdown.children[ind].image.sprite = entry.Value;
            resourceDropdown.children[ind].childText.text = "";
            ind++;
        }
        
        
    }

}
