using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager : ManagerBase
{
    public Canvas resourceUiCanvas;
    public List<Sprite> resourceSprites;
    public ResourceUI resourceUI;
    public static Dictionary<QualityEnum, float> qualityMultiplier = new Dictionary<QualityEnum, float>
    {
        { QualityEnum.awful, 0.5F },
        { QualityEnum.poor, 0.75F },
        { QualityEnum.normal, 1.0F },
        { QualityEnum.good, 1.5F },
        { QualityEnum.masterwork, 2.0F }
    };

    // Use this for initialization
    void Awake()
    {

        resourceDefinitions = new List<ResourceDef>();
        ManagerBase.resourceIndexOf = new Dictionary<string, int>();
    string m_Path = Application.dataPath;
        //print(m_Path + "/Definitions/Resources.csv");
        List<string> lines = new List<string>();
        using (var reader = new StreamReader(m_Path + "/Definitions/Resources.csv"))
        {
            // Read the header
            reader.ReadLine();
            while (!reader.EndOfStream)
                lines.Add(reader.ReadLine());
        }

        // Create each resource definition
        foreach (var csvLine in lines)
        {
            resourceDefinitions.Add(new ResourceDef(csvLine));
            ManagerBase.resourceIndexOf.Add(resourceDefinitions[resourceDefinitions.Count - 1].name, resourceDefinitions.Count - 1);
        }

        // Initialize the domain's stock
        if (ManagerBase.domain == null)
            ManagerBase.domain = new Domain();
        ManagerBase.domain.stock = new ResourceStock(resourceDefinitions);

        resourceUI.resourceNameSpriteDict = new Dictionary<string, Sprite>();
        foreach (ResourceDef rd in resourceDefinitions)
            resourceUI.resourceNameSpriteDict.Add(rd.name, rd.image);
        resourceUI.domain = ManagerBase.domain;


        resourceUI.enabled = false;
        resourceUiCanvas.enabled = false;

        // Add a few resources
        ResourceQuantityQualityList startingResources = new ResourceQuantityQualityList();
        startingResources.rqqList.Add(new ResourceNameQuantityQuality("Cow", QualityEnum.normal, 10));
        startingResources.rqqList.Add(new ResourceNameQuantityQuality("Pine", QualityEnum.normal, 100));
        startingResources.rqqList.Add(new ResourceNameQuantityQuality("Pine", QualityEnum.good, 50));
        startingResources.rqqList.Add(new ResourceNameQuantityQuality("Limestone", QualityEnum.normal, 75));
        startingResources.rqqList.Add(new ResourceNameQuantityQuality("Marble", QualityEnum.normal, 25));
        startingResources.rqqList.Add(new ResourceNameQuantityQuality("Iron", QualityEnum.normal, 75));

        startingResources.AddResources(ManagerBase.domain.stock);

    }

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update () {
		
	}
}

public enum QualityEnum { awful, poor, normal, good, masterwork, any };
