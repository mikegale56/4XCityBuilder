using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingDropdownCreator : MonoBehaviour
{

    public RectTransform panel;
    public BuildingDropdown buildingDropdown;
    public BuildingManager buildingManager;

    // Use this for initialization
    void Start()
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
    void Update()
    {

    }

    public void CreateDropdown(ResourceQuantityQualityList choices)
    {

        // Create the button
        buildingDropdown = DropdownUtilities.NewButton("Building Dropdown", "Select Building", panel.transform, 160F, 32F).gameObject.AddComponent<BuildingDropdown>();
        buildingDropdown.transform.localPosition = new Vector3(101F, 0F);
        buildingDropdown.childHeight = 30;
        buildingDropdown.mainText = buildingDropdown.transform.Find("Text").GetComponent<Text>();
        buildingDropdown.mainText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buildingDropdown.mainText.color = Color.black;

        int ind = 0;
        foreach (string category in buildingManager.buildingCategories)
        {
            buildingDropdown.AddChild(category);
            buildingDropdown.children[ind].childText.text = category;
            ind++;
        }


    }

}
