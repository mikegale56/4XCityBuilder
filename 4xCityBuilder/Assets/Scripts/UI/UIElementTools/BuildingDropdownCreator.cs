using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingDropdownCreator : MonoBehaviour
{

    public RectTransform panel;
    public DropdownUIElement buildingDropdown;
    public DropdownUIElement demolishDropdown;
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
		buildingDropdown = UIElementFunctions.Dropdown(panel, null, "Select Building", localPosition, new Vector2(w, h));
        buildingDropdown.thisGo.name = "Building Dropdown Button";
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
            IEnumerable<BuildingDef> theseBuildingDefs = BuildingQueries.ByCategoryNoParent(ManagerBase.buildingDefinitions, category);
            foreach (BuildingDef def in theseBuildingDefs)
            {
                buildingDropdown.children[ind].AddChild();
                buildingDropdown.children[ind].children[subInd].textGo.text = def.name + " (Tier " + def.tier + ")";
                buildingDropdown.children[ind].children[subInd].buttonGo.onClick.AddListener(() => bbcb(def.name));
                buildingDropdown.children[ind].children[subInd].CloseButton();
                subInd++;
            }
            ind++;
        }
    }

    public void CreateDemolishOrUpgradeDropdown(Vector3 localPosition, float w, float h, string surfaceType)
    {

        // Create the button
		demolishDropdown = UIElementFunctions.Dropdown(panel, null, "Surface Actions", localPosition, new Vector2(w, h));
        demolishDropdown.childHeight = 30;
        demolishDropdown.childFontSize = 16;
		
		demolishDropdown.AddChild();
		demolishDropdown.children[0].textGo.text = "Demolish";
		demolishDropdown.children[0].CloseButton();

        IEnumerable<BuildingDef> upgrades = BuildingQueries.ByParent(ManagerBase.buildingDefinitions, surfaceType);
        Debug.Log(upgrades);
        int ind = 0;
		foreach (BuildingDef def in upgrades)
        {
            Debug.Log(def.name);
            demolishDropdown.AddChild();
            demolishDropdown.children[ind].textGo.text = def.name + " (Tier " + def.tier + ")";
			demolishDropdown.children[ind].buttonGo.onClick.AddListener(() => bbcb(def.name));
            demolishDropdown.children[ind].CloseButton();
            ind++;
        }
    }
}