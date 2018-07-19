using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager : MonoBehaviour {

    public Canvas resourceUiCanvas;
    public ResourceUI resourceUI;
    public List<ResourceDef> resourceDefinitions;
    public ResourceStock domainResources;
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
            //Debug.Log("reading in resource " + resourceDefinitions[resourceDefinitions.Count - 1].name);
        }

        // Initialize the domain's stock
        domainResources = new ResourceStock("Aster", "Aster", resourceDefinitions);

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
