using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

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
    void Start ()
    {

        resourceDefinitions = new List<ResourceDef>();

        // Read in the definitions list from csv later.  For now, use some strings
        List<string> lines = new List<string>();
        lines.Add("Earthworking,Mining,Ore,1,,,Copper Ore");
        lines.Add("Metalworking,Metallurgy,Metal,1,,,Copper");
        lines.Add("Agricultural,Woodcutting,Wood,1,Fuel,1,Pine");
        lines.Add("Earthworking,Mining,Fuel,3,,,Coal");
        lines.Add("Metalworking,Weaponsmith,Long Sword,1,1H Weapon,1,Copper Long Sword");

        // Create each resource definition
        foreach (var csvLine in lines)
        {
            resourceDefinitions.Add(new ResourceDef(csvLine));
        }

        // Initialize the domain's stock
        domainResources = new ResourceStock("Aster", "Aster", resourceDefinitions);

    }

    // Update is called once per frame
    void Update () {
		
	}
}

public enum QualityEnum { awful, poor, normal, good, masterwork, any };
