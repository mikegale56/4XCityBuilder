using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingDropdownCreator : MonoBehaviour
{

    public RectTransform panel;
    public DropdownBase buildingDropdown;
    public DropdownBase demolishDropdown;
    public BuildingManager buildingManager;
    public BuildingButtonCallback bbcb;

    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            EventSystem e = new GameObject().AddComponent<EventSystem>();
            e.name = "Event System";
            e.gameObject.AddComponent<StandaloneInputModule>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateDropdown(Vector3 localPosition, float w, float h)
    {

        // Create the button
        buildingDropdown = DropdownUtilities.NewButton("Building Dropdown", "Select Building", panel.transform, w, h).gameObject.AddComponent<DropdownBase>();
        buildingDropdown.transform.localPosition = localPosition;
        buildingDropdown.childHeight = 30;
        buildingDropdown.childFontSize = 16;
        buildingDropdown.CloseButton();

        int ind = 0;
        foreach (string category in buildingManager.buildingCategories)
        {
            buildingDropdown.AddChild();
            buildingDropdown.children[ind].textGo.text = category;
            buildingDropdown.children[ind].CloseButton();
            int subInd = 0;
            IEnumerable<BuildingDef> theseBuildingDefs = BuildingQueries.ByCategoryNoParent(buildingManager.buildingDefinitions, category);
            foreach (BuildingDef def in theseBuildingDefs)
            {
                buildingDropdown.children[ind].AddChild();
                buildingDropdown.children[ind].children[subInd].textGo.text = def.name + " (Tier " + def.tier + ")";
                buildingDropdown.children[ind].children[subInd].button.onClick.AddListener(() => bbcb(def.name));
                buildingDropdown.children[ind].children[subInd].CloseButton();
                subInd++;
            }
            ind++;
        }

    }

    public void CreateDemolishDropdown(Vector3 localPosition, float w, float h)
    {

        // Create the button
        demolishDropdown = DropdownUtilities.NewButton("Demolish Dropdown", "Demolish", panel.transform, w, h).gameObject.AddComponent<DropdownBase>();
        demolishDropdown.transform.localPosition = localPosition;
        demolishDropdown.childHeight = 30;
        demolishDropdown.childFontSize = 16;

    }

}
