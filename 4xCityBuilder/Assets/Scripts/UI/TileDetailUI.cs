using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public delegate void BuildingButtonCallback(string s);
public delegate void JobButtonCallback(System.Guid jobGuid);

public class TileDetailUI : MonoBehaviour
{

    public RectTransform tileDetailUiPanel;
    public RectTransform surfacePanel;
    public RectTransform spText;
    public RectTransform groundPanel;
    public RectTransform gpText;
    public RectTransform undergroundPanel;
    public RectTransform ugpText;
    public RectTransform specialPanel;
    public RectTransform sppText;
    public RectTransform jobsPanel;
    public RectTransform militaryPanel;
    public MapManager mapManager;
    public BuildingManager buildingManager;
    public JobManager jobManager;
    public BuildingObj thisBuilding;

    public BuildingActionDropdown buildingActionDropdown;
    public NewConstructionDropdown newConstructionDropdown;
    public ResourceDropdown resourceChoiceDropdown;
    public List<ResourceDropdown> activeJobDisplays;

    public BuildingButtonCallback bbcb;

    private CustomUIElement jobDescriptionText;

    public int iLoc, jLoc;

    // Use this for initialization
    void Awake()
    {
        newConstructionDropdown.bbcb = new BuildingButtonCallback(BuildActionSelected);
        buildingActionDropdown.buildCallback = new BuildingButtonCallback(BuildActionSelected);
        buildingActionDropdown.demolishCallback = new BuildingButtonCallback(DemolishActionSelected);
        buildingActionDropdown.newJobCallback = new JobButtonCallback(NewJobActionSelected);
        resourceChoiceDropdown = null;
        activeJobDisplays = new List<ResourceDropdown>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FocusOnTile(int i, int j)
    {
        // Set Location
        iLoc = i;
        jLoc = j;

        // Delete buttons
        ClearUIObjects();

        // Surface Panel
        if (SetSurfacePanel(i, j))
            thisBuilding = BuildingQueries.ByLocation(ManagerBase.domain.buildings, new Vector2Int(iLoc, jLoc)); // Get the building here

        // Ground Panel
        SetGroundPanel(i, j);

        // Underground Panel 
        SetUndergroundPanel(i, j);

        // Draw Jobs up from the bottom
        DrawActiveJobs(i, j);

    }

    private void DrawActiveJobs(int i, int j)
    {
        // Get active jobs
        List<JobObj> activeJobs = jobManager.ByLocation(i, j).ToList();

        Vector3 location = new Vector3(-200, -670);
        foreach (JobObj job in activeJobs)
        {
            if (job.toDelete == true)
            {
                Debug.Log("Skipping job to be deleted");
                continue;
            }
            ResourceDropdown resourceDropdown;
            if (job.GetInputResources().rqqList.Count == 0)
                resourceDropdown = ResourceDropdownCreator.CreateNoResourceStaticView(tileDetailUiPanel.transform, location, job.jobDef.name, ManagerBase.domain, job.GetSprite());
            else
                resourceDropdown = ResourceDropdownCreator.CreateResourceStaticView(tileDetailUiPanel.transform, location, job.GetInputResources(), job.jobDef.name, ManagerBase.domain, job.GetSprite());
            activeJobDisplays.Add(resourceDropdown);
            location.y += 100;
        }
    }

    public void BuildActionSelected(string s)
    {
        // Delete buttons
        ClearUIObjects();

        BuildingDef buildingSelected = ManagerBase.buildingDefinitions[ManagerBase.buildingIndexOf[s]];

        if (buildingSelected == null)
            Debug.LogError("Should not have an unbuildable building here: " + s);

        string txt = buildingSelected.name + "\n" + buildingSelected.description + "\n";
        if (jobDescriptionText == null)
            jobDescriptionText = UIElementFunctions.TextOnly(jobsPanel.transform, txt, new Vector3(180, -50), new Vector2(300, 100));
        else
            jobDescriptionText.textGo.text = txt;
        // Add prerequisites, colored, here

        // Figure out what the building needs
        resourceChoiceDropdown = ResourceDropdownCreator.CreateResourceChoiceDropdown(tileDetailUiPanel.transform, new Vector3(-200, -185), buildingSelected.resourcesToBuild, buildingSelected.name, ManagerBase.domain, buildingSelected.sprite);

        resourceChoiceDropdown.jobStartButton.buttonGo.onClick.AddListener(delegate () { StartConstructionJobButton(); });
    }
    
    public void DemolishActionSelected(string s)
    {
        //Debug.Log("DemolishActionSelected");
        // Delete buttons
        //Debug.Log("Clear UI");
        ClearUIObjects();

        string txt = "Demolition\nTears down the current structure\n";
        if (jobDescriptionText == null)
            jobDescriptionText = UIElementFunctions.TextOnly(jobsPanel.transform, txt, new Vector3(180, -50), new Vector2(300, 100));
        else
            jobDescriptionText.textGo.text = txt;
        // Add prerequisites, colored, here

        // Buttons to make it go
        //Debug.Log("ResourceDropdownCreator");
        resourceChoiceDropdown = ResourceDropdownCreator.CreateNoResourceDropdown(tileDetailUiPanel.transform, new Vector3(-200, -185), "Demolition", ManagerBase.domain, null);
        resourceChoiceDropdown.jobStartButton.buttonGo.onClick.AddListener(delegate () { StartDemolitionJobButton(); });
        //Debug.Log("Done DemolishActionSelected");
    }

    public void NewJobActionSelected(System.Guid jobGuid)
    {
        JobDef jobSelected = JobQueries.ByGuid(ManagerBase.jobDefinitions, jobGuid);

        if (jobSelected == null)
            Debug.LogError("Should not have an empty job here");

        string txt = jobSelected.name + "\n" + jobSelected.description + "\n";
        if (jobDescriptionText == null)
            jobDescriptionText = UIElementFunctions.TextOnly(jobsPanel.transform, txt, new Vector3(180, -50), new Vector2(300, 100));
        else
            jobDescriptionText.textGo.text = txt;
        // Add prerequisites, colored, here

        // Figure out what the building needs
        Sprite output1Sprite = ManagerBase.resourceDefinitions[ManagerBase.resourceIndexOf[jobSelected.outputName[0]]].image;
        if (jobSelected.inputResources.rqqList.Count > 0) // Has inputs
            resourceChoiceDropdown = ResourceDropdownCreator.CreateResourceChoiceDropdown(tileDetailUiPanel.transform, new Vector3(-200, -185), jobSelected.inputResources, jobSelected.name, ManagerBase.domain, output1Sprite);
        else
            resourceChoiceDropdown = ResourceDropdownCreator.CreateNoResourceDropdown(tileDetailUiPanel.transform, new Vector3(-200, -185), jobSelected.name, ManagerBase.domain, output1Sprite);


        resourceChoiceDropdown.jobStartButton.buttonGo.onClick.AddListener(delegate () { StartJobButton(jobSelected); });

    }

    void StartJobButton(JobDef jobSelected)
    {
        thisBuilding = BuildingQueries.ByLocation(ManagerBase.domain.buildings, new Vector2Int(iLoc, jLoc)); // Update the building here
        ResourceJobObj newJob = jobManager.AddJob(jobSelected, thisBuilding);
        ResourceQuantityQualityList jobResources = resourceChoiceDropdown.GetCurrentChoices();
        newJob.SetResources(jobResources);
        newJob.StartJob();
        newJob.AddWorker();
        newJob.AddWorker();
        FocusOnTile(iLoc, jLoc);
    }

    void StartConstructionJobButton()
    {
        // Get resource list
        ResourceQuantityQualityList jobResources = resourceChoiceDropdown.GetCurrentChoices();

        JobDef newConstructionJobDef = CreateConstructionJobDef(jobResources, resourceChoiceDropdown.taskName);

        BuildingJobObj newJob = jobManager.AddConstructionJob(newConstructionJobDef, iLoc, jLoc);
        newJob.SetResources(jobResources);
        newJob.StartJob();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();

        // Change surface type to construction
        ManagerBase.domain.mapData.SetSurfaceValue(iLoc, jLoc, ManagerBase.surfaceValueDictionary["Gear"]);

        // Reload the map and re-focus on the tile
        WorldEventHandlerManager.Broadcast(worldEventChannels.map, mapChannelEvents.change, new WorldEventArg(iLoc, jLoc));

    }

    private JobDef CreateConstructionJobDef(ResourceQuantityQualityList jobResources, string taskName)
    {
        BuildingDef bd = BuildingQueries.ByName(ManagerBase.buildingDefinitions, taskName).ElementAt(0);
        JobDef constructionJob = new JobDef();
        constructionJob.name = "Construction of " + taskName;
        constructionJob.description = "Construction of " + taskName;
        constructionJob.industry = "Construction";
        constructionJob.skill = "Building";
        constructionJob.tier = bd.tier;
        constructionJob.defaultPMUs = bd.defaultPMUs;
        constructionJob.inputResources = jobResources;
        constructionJob.outputName = new List<string>();
        constructionJob.outputName.Add(taskName);
        //List<int> defaultOutputQuantity;

        return constructionJob;
    }

    void StartDemolitionJobButton()
    {
        //Debug.Log("StartDemolitionJobButton");
        short surfaceValue = ManagerBase.domain.mapData.GetSurfaceValue(iLoc, jLoc);
        string surfaceType = ManagerBase.surfaceValueDictionary.FirstOrDefault(x => x.Value == surfaceValue).Key;

        JobDef newDemolitionJobDef = CreateDemolitionJobDef(surfaceType);

        ResourceQuantityQualityList jobResources = new ResourceQuantityQualityList();

        //Debug.Log("AddDemolitionJob");
        DemoJobObj newJob = jobManager.AddDemolitionJob(newDemolitionJobDef, iLoc, jLoc);
        //Debug.Log("SetResources");
        newJob.SetResources(jobResources);
        newJob.StartJob();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();
        newJob.AddWorker();

        // Change surface type to construction
        //Debug.Log("SetSurfaceValue");
        ManagerBase.domain.mapData.SetSurfaceValue(iLoc, jLoc, ManagerBase.surfaceValueDictionary["Cancel"]);

        // Reload the map and re-focus on the tile
        //Debug.Log("Broadcast");
        WorldEventHandlerManager.Broadcast(worldEventChannels.map, mapChannelEvents.change, new WorldEventArg(iLoc, jLoc));

        //Debug.Log("Done StartDemolitionJobButton");
    }

    private JobDef CreateDemolitionJobDef(string currentBuilding)
    {
        JobDef demolitionJob = new JobDef();
        demolitionJob.name = "Demolition of " + currentBuilding;
        demolitionJob.description = "Demolition of " + currentBuilding;
        demolitionJob.industry = "Construction";
        demolitionJob.skill = "Demolition";
        demolitionJob.tier = 1;
        demolitionJob.defaultPMUs = 360;
        return demolitionJob;
    }

    void ClearUIObjects()
    {
        // Delete buttons
        if (jobDescriptionText != null)
            jobDescriptionText.textGo.text = "";
        // Reset the building button
        if (newConstructionDropdown.buildingDropdown != null)
            newConstructionDropdown.buildingDropdown.textGo.text = "Select Building";
        if (buildingActionDropdown.actionDropdown != null)
            buildingActionDropdown.actionDropdown.textGo.text = "Surface Actions";
        if (activeJobDisplays != null)
        {
            foreach (ResourceDropdown rd in activeJobDisplays)
            {
                rd.ClearResourceList();
                Destroy(rd.gameObject);
            }
            activeJobDisplays.Clear();
        }
        if (resourceChoiceDropdown != null)
        {
            resourceChoiceDropdown.ClearResourceList();
            Destroy(resourceChoiceDropdown.gameObject);
        }
    }

    private bool SetSurfacePanel(int i, int j)
    {
        if (newConstructionDropdown.buildingDropdown != null)
            newConstructionDropdown.DestroyDropdown();
        if (buildingActionDropdown.actionDropdown != null)
            buildingActionDropdown.DestroyDropdown();
        if (ManagerBase.domain.mapData.GetSurfaceValue(i, j) >= 0)
        {
            short surfaceValue = ManagerBase.domain.mapData.GetSurfaceValue(i, j);
            Tile surfaceTile = ManagerBase.surfaceTiles[surfaceValue];
            surfacePanel.GetComponent<Image>().sprite = surfaceTile.sprite;
            surfacePanel.GetComponent<Image>().type = Image.Type.Filled;
            surfacePanel.GetComponent<Image>().color = surfaceTile.color;
            //Debug.Log(spText.GetComponent<Text>().text);
            // Create the new string
            string surfaceType = ManagerBase.surfaceValueDictionary.FirstOrDefault(x => x.Value == surfaceValue).Key;
            string newText = surfaceType;
            newText += "\n";
            newText += "No Description Yet";
            spText.GetComponent<Text>().text = newText;
            buildingActionDropdown.CreateDropdown(new Vector3(-468, -70), 300, 45, surfaceType);

            return true;
        }
        else
        {
            surfacePanel.GetComponent<Image>().sprite = null;
            surfacePanel.GetComponent<Image>().color = Color.gray;
            spText.GetComponent<Text>().text = "Build a Structure:\n";
            newConstructionDropdown.CreateDropdown(new Vector3(-468, -70), 300, 45);
            return false;
        }
    }

    public void SetGroundPanel(int i, int j)
    {
        byte groundValue = ManagerBase.domain.mapData.GetGroundValue(i, j);
        Tile groundTile = ManagerBase.groundTiles[groundValue];
        groundPanel.GetComponent<Image>().sprite = groundTile.sprite;
        groundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        groundPanel.GetComponent<Image>().color = groundTile.color;
        string newText = ManagerBase.groundValueDictionary.FirstOrDefault(x => x.Value == groundValue).Key;
        newText += "\n";
        newText += "No Description Yet\n";
        newText += "Worked by:";
        gpText.GetComponent<Text>().text = newText;
    }

    public void SetUndergroundPanel(int i, int j)
    {

        byte ugv = ManagerBase.domain.mapData.GetUndergroundValue(i, j);
        byte sv = ManagerBase.domain.mapData.GetStoneValue(i, j);
        Tile undergroundTile = ManagerBase.undergroundTiles[ugv][sv];
        undergroundPanel.GetComponent<Image>().sprite = undergroundTile.sprite;
        undergroundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        undergroundPanel.GetComponent<Image>().color = undergroundTile.color;
        string newText;
        if (ugv == 0)
            newText = ManagerBase.stoneValueDictionary.FirstOrDefault(x => x.Value == sv).Key;
        else
            newText = ManagerBase.undergroundValueDictionary.FirstOrDefault(x => x.Value == ugv).Key;
        newText += "\n";
        newText += "No Description Yet\n";
        newText += "Worked by:";
        ugpText.GetComponent<Text>().text = newText;
    }
}