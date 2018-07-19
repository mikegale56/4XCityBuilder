using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour {

    public Button layerSwapButton;
    private bool viewSurfaceLayer = true;
    private List<Sprite> layerSwapSprites;
    public MapManager mapManager;
    public ResourceManager resourceManager;
    public BuildingManager buildingManager;
    public Camera mainCamera;
    public string currentUi = "MainMap";

    // Use this for initialization
    void Start ()
    {


        // Swap Surface and Underground button
        Button btn1 = layerSwapButton.GetComponent<Button>();
        //Calls the TaskOnClick/TaskWithParameters method when you click the Button
        btn1.onClick.AddListener(UpdateLayerSwapButton);
        layerSwapSprites = new List<Sprite>();
        List<string> swapLayerButtonString = new List<string>();
        swapLayerButtonString.Add("Sprites/Buttons/LayerSwap_Surface");
        swapLayerButtonString.Add("Sprites/Buttons/LayerSwap_GoToUnderground");
        swapLayerButtonString.Add("Sprites/Buttons/LayerSwap_Underground");
        swapLayerButtonString.Add("Sprites/Buttons/LayerSwap_GoToSurface");

        foreach (var buttonString in swapLayerButtonString)
        {
            Texture2D tex = Resources.Load(buttonString) as Texture2D;
            Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            layerSwapSprites.Add(newSprite);
        }
    }
	
	// Update is called once per frame
	void Update () {
		// Nothing yet
	}

    void UpdateLayerSwapButton()
    {
        SpriteState ss = new SpriteState();

        if (viewSurfaceLayer) // Viewing the surface, swap to underground
        {
            Debug.Log("Swapping to underground");

            layerSwapButton.GetComponent<Image>().sprite = layerSwapSprites[2];
            ss.highlightedSprite = layerSwapSprites[3];
            layerSwapButton.spriteState = ss;
            viewSurfaceLayer = false;
            ShowMaps();
        } else // Viewing underground, swap to surface
        {
            Debug.Log("Swapping to surface");

            layerSwapButton.GetComponent<Image>().sprite = layerSwapSprites[0];
            ss.highlightedSprite = layerSwapSprites[1];
            layerSwapButton.spriteState = ss;
            viewSurfaceLayer = true;
            ShowMaps();
        }

    }

    public void PressMainMapButton()
    {
        // If already on the resource UI, return
        if (currentUi.Equals("MainMap"))
            return;

        // Enable map movement & Show the map
        mainCamera.GetComponent<MapPanZoom>().enabled = true;
        ShowMaps();

        // Disable all UIs
        DisableAllUis();
        currentUi = "MainMap";
    }

    public void PressResourceUiButton()
    {
        // If already on the resource UI, return
        if (currentUi.Equals("Resource"))
            return;

        // Disable map movement & Hide the map
        mainCamera.GetComponent<MapPanZoom>().enabled = false;
        HideMaps();
        DisableAllUis();

        // Set the resource ui enabled
        resourceManager.resourceUI.enabled = true;
        resourceManager.resourceUiCanvas.enabled = true;
        currentUi = "Resource";
    }

    public void PressBuildingUiButton()
    {
        // If already on the resource UI, return
        if (currentUi.Equals("Building"))
            return;

        // Disable map movement & Hide the map
        mainCamera.GetComponent<MapPanZoom>().enabled = false;
        HideMaps();
        DisableAllUis();

        // Set the resource ui enabled
        buildingManager.buildingUI.enabled = true;
        buildingManager.buildingUiCanvas.enabled = true;
        currentUi = "Building";
    }

    void ShowMaps()
    {
        if (viewSurfaceLayer)
        {
            mapManager.groundTileMap.GetComponent<Renderer>().enabled = true;
            mapManager.undergroundTileMap.GetComponent<Renderer>().enabled = false;
        } else
        {
            mapManager.groundTileMap.GetComponent<Renderer>().enabled = false;
            mapManager.undergroundTileMap.GetComponent<Renderer>().enabled = true;
        }
    }

    void HideMaps()
    {
        mapManager.groundTileMap.GetComponent<Renderer>().enabled = false;
        mapManager.undergroundTileMap.GetComponent<Renderer>().enabled = false;
    }

    void DisableAllUis()
    {
        resourceManager.resourceUI.enabled = false;
        resourceManager.resourceUiCanvas.enabled = false;
        buildingManager.buildingUI.enabled = false;
        buildingManager.buildingUiCanvas.enabled = false;
    }
}
