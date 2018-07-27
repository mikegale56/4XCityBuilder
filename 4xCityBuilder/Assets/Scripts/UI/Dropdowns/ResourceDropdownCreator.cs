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

        // Create the button
        resourceDropdown = DropdownUtilities.NewButton("Resource Dropdown", "Resource Dropdown", panel.transform).gameObject.AddComponent<ResourceDropdown>();
        resourceDropdown.mainText = resourceDropdown.transform.Find("Text").GetComponent<Text>();
        resourceDropdown.mainText.text = "Enter Text Here";
        resourceDropdown.mainText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        resourceDropdown.mainText.color = Color.red;
        resourceDropdown.AddChild();
        resourceDropdown.AddChild();
    }

}
