using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingActionDropdown : MonoBehaviour
{

    public RectTransform panel;
    public DropdownUIElement actionDropdown;
    public BuildingManager buildingManager;
    public BuildingButtonCallback buildCallback, demolishCallback;
    public JobButtonCallback newJobCallback;

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

    public void CreateDropdown(Vector3 localPosition, float w, float h, string surfaceType)
    {

        // Create the button
		actionDropdown = UIElementFunctions.Dropdown(panel, null, "Surface Actions", localPosition, new Vector2(w, h));
        actionDropdown.childHeight = 30;
        actionDropdown.childFontSize = 16;
		
        // Demolish
		actionDropdown.AddChild();
		actionDropdown.children[0].textGo.text = "Demolish";
        actionDropdown.children[0].buttonGo.onClick.AddListener(() => demolishCallback("Demolish"));
        actionDropdown.children[0].CloseButton();

        // Upgrades
        int menuInd = actionDropdown.children.Count;
        actionDropdown.AddChild();
        actionDropdown.children[menuInd].textGo.text = "Upgrade";
        //actionDropdown.children[upgradeInd].buttonGo.interactable = false;
        actionDropdown.children[menuInd].CloseButton();

        Debug.Log(surfaceType);
        IEnumerable<BuildingDef> upgrades = BuildingQueries.ByParent(ManagerBase.buildingDefinitions, surfaceType);
        int ind = 0;
        foreach (BuildingDef def in upgrades)
        {
            actionDropdown.children[menuInd].AddChild();
            actionDropdown.children[menuInd].children[ind].textGo.text = def.name + " (Tier " + def.tier + ")";
            Debug.Log(actionDropdown.children[menuInd].children[ind].textGo.text);
            actionDropdown.children[menuInd].children[ind].buttonGo.onClick.AddListener(() => buildCallback(def.name));
            actionDropdown.children[menuInd].children[ind].CloseButton();
            ind++;
        }

        // Jobs
        if (ManagerBase.buildingIndexOf.ContainsKey(surfaceType))
        {
            menuInd++;

            BuildingDef bldgDef = ManagerBase.buildingDefinitions[ManagerBase.buildingIndexOf[surfaceType]];

            IEnumerable<JobDef> jobs = JobQueries.ByNameAndMaxTier(ManagerBase.jobDefinitions, bldgDef.name, bldgDef.tier);

            if (jobs.Count() > 0)
            {

                actionDropdown.AddChild();
                actionDropdown.children[menuInd].textGo.text = "Start Job";
                actionDropdown.children[menuInd].buttonGo.interactable = false;
                actionDropdown.children[menuInd].CloseButton();

                ind = 0;
                foreach (JobDef job in jobs)
                {
                    string jobString = job.name + " (" + job.outputName[0] + ")";
                    actionDropdown.children[menuInd].AddChild();
                    actionDropdown.children[menuInd].children[ind].textGo.text = jobString;
                    actionDropdown.children[menuInd].children[ind].buttonGo.onClick.AddListener(() => newJobCallback(job.guid));
                    actionDropdown.children[menuInd].children[ind].CloseButton();
                    ind++;
                }
            }
        }
    }

    public void DestroyDropdown()
    {
        actionDropdown.Hide();
        Destroy(actionDropdown.thisGo);
    }
}