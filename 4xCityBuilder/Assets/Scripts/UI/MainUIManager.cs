using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour {

    public Button layerSwapButton;
    private bool viewSurfaceLayer = true;
    private List<Sprite> layerSwapSprites;
    public MapManager mapManager;
    public ResourceManager resourceManager;
    public BuildingManager buildingManager;
    public TileDetailManager tileDetailManager;
    public Camera mainCamera;
    public string currentUi = "MainMap";
    public RectTransform mapCollider;
    public RectTransform eventPanel;
    public RectTransform tileDetailBackPanel;
    float mainMapMaxX, mainMapMinY, mainMapMaxY;
    float tvMapMaxX, tvMapMinY, tvMapMaxY;


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

        // Get the dimensions of the tile view in the main map and tile view screens
        Vector3[] v = new Vector3[4];
        eventPanel.GetWorldCorners(v);
        mainMapMaxX = v[0].x;
        mainMapMinY = v[0].y;
        mainMapMaxY = v[1].y;
        tileDetailManager.tileDetailCanvas.enabled = true;
        Vector3[] v2 = new Vector3[4];
        tileDetailBackPanel.GetWorldCorners(v2);
        tvMapMaxX = v2[0].x;
        tvMapMinY = v2[0].y;
        tvMapMaxY = v2[1].y;
        tileDetailManager.tileDetailCanvas.enabled = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
        // Check for clicking on a tile in the main map
        if (Input.GetMouseButtonDown(0))
        {
            if (currentUi.Equals("MainMap"))
                ResizeMapCollider(0, mainMapMaxX, mainMapMinY, mainMapMaxY);
            else if (currentUi.Equals("TileDetail"))
                ResizeMapCollider(0, tvMapMaxX, tvMapMinY, tvMapMaxY);
            else
                return;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider == null)
            { 
                // Did not hit a collider, nothing
            }
            else if (hit.collider.name == mapCollider.name)
            {
                // Get the tile position
                //Debug.Log(string.Format("Coordinates of mouse is [X: {0} Y: {0}]", pos.x, pos.y));
                PressTile(pos);
            }
            else
                Debug.Log("Did not hit mapCollider, hit " + hit.collider.ToString());
        }
    }

    void ResizeMapCollider(float minX, float maxX, float minY, float maxY)
    {
        // Transform into world coordinates
        Vector3 ll = mainCamera.ScreenToWorldPoint(new Vector3(minX, minY, 0));
        Vector3 ur = mainCamera.ScreenToWorldPoint(new Vector3(maxX, maxY, 0));
        Vector2 delta = ur - ll;
        //Set the new position
        mapCollider.anchoredPosition = ll;
        mapCollider.sizeDelta = delta;
        BoxCollider2D boxCollider2D = mapCollider.gameObject.GetComponent<BoxCollider2D>();
        boxCollider2D.size = mapCollider.sizeDelta;
        boxCollider2D.offset = mapCollider.sizeDelta / 2.0F;
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
        // If already on the main map, return
        if (currentUi.Equals("MainMap"))
            return;

        // Enable map movement & Show the map
        mainCamera.GetComponent<MapPanZoom>().enabled = true;
        ShowMaps();

        // Disable all UIs
        DisableAllUis();
        currentUi = "MainMap";
    }

    public void PressTile(Vector3 tileLocation)
    {
        tileLocation.z = 0;
        Debug.Log("Tile Detail on " + tileLocation.x.ToString() + "," + tileLocation.y.ToString() + "," + tileLocation.z.ToString());

        MapPanZoom mpz = mainCamera.GetComponent<MapPanZoom>();
        // Disable map movement, reset inertia & Show the map
        mpz.enabled       = false;
        mpz.inertiaVector = Vector3.zero;
        ShowMaps();
        // Change the focus of the camera
        Vector3 oldCameraLocation = mpz.transform.localPosition;
        //0, tvMapMaxX, tvMapMinY, tvMapMaxY

        Vector3 mapPanelCenter = new Vector3(tvMapMaxX / 2, tvMapMinY + (tvMapMaxY - tvMapMinY) / 2, 0.0F);
        mapPanelCenter = mainCamera.ScreenToWorldPoint(mapPanelCenter);
        Vector3 newCameraFocus;
        newCameraFocus = oldCameraLocation + (tileLocation - mapPanelCenter);
        newCameraFocus.z = -10.0F; //Make sure this doesn't move

        Debug.Log("Old Camera");
        Debug.Log(oldCameraLocation);
        Debug.Log("mapPanelCenter position");
        Debug.Log(mapPanelCenter);
        Debug.Log("New Camera");
        Debug.Log(newCameraFocus);
        mpz.ZoomTo(newCameraFocus);
        
        // Disable all UIs
        DisableAllUis();

        // Set the resource ui enabled
        tileDetailManager.tileDetailUI.enabled = true;
        tileDetailManager.tileDetailCanvas.enabled = true;
        currentUi = "TileDetail";

        // Convert to int and have the tile ui work on it
        tileDetailManager.tileDetailUI.FocusOnTile(Mathf.FloorToInt(tileLocation.x), Mathf.FloorToInt(tileLocation.y));

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
            mapManager.surfaceTileMap.GetComponent<Renderer>().enabled = true;
            mapManager.undergroundTileMap.GetComponent<Renderer>().enabled = false;
        } else
        {
            mapManager.groundTileMap.GetComponent<Renderer>().enabled = false;
            mapManager.surfaceTileMap.GetComponent<Renderer>().enabled = false;
            mapManager.undergroundTileMap.GetComponent<Renderer>().enabled = true;
        }
    }

    void HideMaps()
    {
        mapManager.groundTileMap.GetComponent<Renderer>().enabled = false;
        mapManager.surfaceTileMap.GetComponent<Renderer>().enabled = false;
        mapManager.undergroundTileMap.GetComponent<Renderer>().enabled = false;
    }

    void DisableAllUis()
    {
        resourceManager.resourceUI.enabled = false;
        resourceManager.resourceUiCanvas.enabled = false;
        buildingManager.buildingUI.enabled = false;
        buildingManager.buildingUiCanvas.enabled = false;
        tileDetailManager.tileDetailUI.enabled = false;
        tileDetailManager.tileDetailCanvas.enabled = false;
    }
}
