using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public delegate void BuildingButtonCallback(string s);

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

    public BuildingDropdownCreator buildingDC;
    public ResourceDropdown resourceChoiceDropdown;

    public BuildingButtonCallback bbcb;

    private Text buildingDescriptionText;
    private Button jobGoButton;
    private Image arrowImage;
    private Image resultImage;

    private List<CustomUIElement> uiElement;
    private CustomUIElement jobStartButton;

    private Sprite rightArrow;

    private int iLoc, jLoc;

    private void Start()
    {
        
    }

    public void FocusOnTile(int i, int j)
    {

        iLoc = i;
        jLoc = j;

        // Delete buttons
        ClearUIObjects();

        string newText;
        // Surface Panel
        if (ManagerBase.domain.mapData.GetSurfaceValue(i, j) >= 0)
        {
            if (buildingDC.buildingDropdown != null)
                buildingDC.buildingDropdown.Hide();
            short surfaceValue = ManagerBase.domain.mapData.GetSurfaceValue(i, j);
            Tile surfaceTile = ManagerBase.surfaceTiles[surfaceValue];
            surfacePanel.GetComponent<Image>().sprite = surfaceTile.sprite;
            surfacePanel.GetComponent<Image>().type = Image.Type.Filled;
            surfacePanel.GetComponent<Image>().color = surfaceTile.color;
            //Debug.Log(spText.GetComponent<Text>().text);
            // Create the new string
            string surfaceType = ManagerBase.surfaceValueDictionary.FirstOrDefault(x => x.Value == surfaceValue).Key;
            newText = surfaceType;
            newText += "\n";
            newText += "No Description Yet";
            spText.GetComponent<Text>().text = newText;

            if (buildingDC.demolishDropdown == null)
                buildingDC.CreateDemolishOrUpgradeDropdown(new Vector3(-468, -140), 300, 45, surfaceType);
            else
                buildingDC.demolishDropdown.Show();

        } else
        {
            if (buildingDC.demolishDropdown != null)
                buildingDC.demolishDropdown.Hide();
            surfacePanel.GetComponent<Image>().sprite = null;
            surfacePanel.GetComponent<Image>().color = Color.gray;
            spText.GetComponent<Text>().text = "Build a Structure:\n";

            if (buildingDC.buildingDropdown == null)
                buildingDC.CreateDropdown(new Vector3(-468, -70), 300, 45);
            else
                buildingDC.buildingDropdown.Show();
        }

        // Ground Panel
        byte groundValue = ManagerBase.domain.mapData.GetGroundValue(i, j);
        Tile groundTile = ManagerBase.groundTiles[groundValue];
        groundPanel.GetComponent<Image>().sprite = groundTile.sprite;
        groundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        groundPanel.GetComponent<Image>().color = groundTile.color;
        newText = ManagerBase.groundValueDictionary.FirstOrDefault(x => x.Value == groundValue).Key;
        newText += "\n";
        newText += "No Description Yet\n";
        newText += "Worked by:";
        gpText.GetComponent<Text>().text = newText;

        // Underground Panel 
        byte ugv = ManagerBase.domain.mapData.GetUndergroundValue(i, j);
        byte sv  = ManagerBase.domain.mapData.GetStoneValue(i, j);
        Tile undergroundTile = ManagerBase.undergroundTiles[ugv][sv];
        undergroundPanel.GetComponent<Image>().sprite = undergroundTile.sprite;
        undergroundPanel.GetComponent<Image>().type = Image.Type.Tiled;
        undergroundPanel.GetComponent<Image>().color = undergroundTile.color;
        if (ugv == 0)
            newText = ManagerBase.stoneValueDictionary.FirstOrDefault(x => x.Value == sv).Key;
        else
            newText = ManagerBase.undergroundValueDictionary.FirstOrDefault(x => x.Value == ugv).Key;
        newText += "\n";
        newText += "No Description Yet\n";
        newText += "Worked by:";
        ugpText.GetComponent<Text>().text = newText;

    }

    public void ActionSelected(string s)
    {
        // Delete buttons
        ClearUIObjects();

        BuildingDef buildingSelected = ManagerBase.buildingDefinitions[ManagerBase.buildingIndexOf[s]];

        if (buildingSelected == null)
            Debug.LogError("Should not have an unbuildable building here: " + s);

        string txt = buildingSelected.name + "\n" + buildingSelected.description + "\n";
        if (buildingDescriptionText == null)
            buildingDescriptionText = NewTextBox(jobsPanel.transform, txt, new Vector3(180, -350), new Vector2(300, 100));
        else
            buildingDescriptionText.text = txt;
        // Add prerequisites, colored, here

        // Figure out what the building needs
        resourceChoiceDropdown = ResourceDropdownCreator.CreateResourceChoiceDropdown(tileDetailUiPanel.transform, new Vector3(-200, -485), buildingSelected.resourcesToBuild, buildingSelected.name, ManagerBase.domain);

        int ind = 0;
        foreach (DropdownUIElement db in resourceChoiceDropdown.elements)
        {
            string quantityString = buildingSelected.resourcesToBuild.rqqList[ind].quantity.ToString();
            Text temp = NewTextBox(db.thisGo.transform, quantityString, new Vector3(0, ResourceDropdownCreator.imageSize-20), new Vector2(ResourceDropdownCreator.imageSize, 20F));
            temp.alignment = TextAnchor.MiddleCenter;
            ind++;
        }

        // Add stuff to the right of the resources
        Vector3 localPosition = resourceChoiceDropdown.elements[resourceChoiceDropdown.elements.Count-1].transform.localPosition;
        if (uiElement == null)
            uiElement = new List<CustomUIElement>();
        // Arrow 
        localPosition.x += ResourceDropdownCreator.imageSize * 4 / 2;
        uiElement.Add(UIElementFunctions.ImageOnly(tileDetailUiPanel.transform, rightArrow, localPosition, new Vector2(ResourceDropdownCreator.imageSize, ResourceDropdownCreator.imageSize)));
        uiElement[uiElement.Count - 1].gameObject.name = "Job Arrow";
        // Result
        localPosition.x += ResourceDropdownCreator.imageSize * 3 / 2;
        uiElement.Add(UIElementFunctions.ImageOnly(tileDetailUiPanel.transform, buildingSelected.image, localPosition, new Vector2(ResourceDropdownCreator.imageSize, ResourceDropdownCreator.imageSize)));
        uiElement[uiElement.Count - 1].gameObject.name = "Job Result";
        // "Go" button
        localPosition.x += ResourceDropdownCreator.imageSize * 3 / 2;
        jobStartButton = UIElementFunctions.ButtonTextColor(tileDetailUiPanel.transform, "Go", Color.green, localPosition, new Vector2(ResourceDropdownCreator.imageSize, ResourceDropdownCreator.imageSize));
        jobStartButton.textGo.fontSize = 22;
        jobStartButton.buttonGo.onClick.AddListener(delegate () { StartJobButton(); });
        jobStartButton.buttonGo.interactable = false;
        jobStartButton.gameObject.name = "Job Start Button";
    }

    void StartJobButton()
    {
        // Get resource list
        ResourceQuantityQualityList jobResources = resourceChoiceDropdown.GetCurrentChoices();

        // Remove resources
        jobResources.RemoveResources(ManagerBase.domain.stock);

        // Delete buttons
        ClearUIObjects();

        // Change surface type
        ManagerBase.domain.mapData.SetSurfaceValue(iLoc, jLoc, ManagerBase.surfaceValueDictionary[resourceChoiceDropdown.taskName]);

        // Reload Tile UI
        FocusOnTile(iLoc, jLoc);
    }

    void ClearUIObjects()
    {
        // Delete buttons
        if (buildingDescriptionText != null)
            buildingDescriptionText.text = "";
        if (resourceChoiceDropdown != null)
        {
            resourceChoiceDropdown.ClearResourceList();
            Destroy(resourceChoiceDropdown.gameObject);
        }
        if (uiElement != null)
            foreach (CustomUIElement uie in uiElement)
                Destroy(uie.thisGo);
        if (jobStartButton != null)
            Destroy(jobStartButton.thisGo);
    }

    // Use this for initialization
    void Awake ()
    {
        buildingDC.bbcb = new BuildingButtonCallback(ActionSelected);
        resourceChoiceDropdown = null;

        Texture2D rightArrowTex = Resources.Load("Textures/RightArrow") as Texture2D;
        rightArrow = Sprite.Create(rightArrowTex,
            new Rect(0, 0, rightArrowTex.width, rightArrowTex.height),
            new Vector2(0.5f, 0.5f), rightArrowTex.width);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Check if the go button should be active or not
        if (jobStartButton != null)
            jobStartButton.buttonGo.interactable = resourceChoiceDropdown.CheckResources();

    }

    public static Text NewTextBox(Transform parent, string text, Vector3 localPosition, Vector2 size)
        {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();
        rect.SetParent(parent);
        rect.name = "TileDetailTextBox";
        rect.gameObject.layer = 9;
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.localPosition = localPosition;
        rect.sizeDelta = size;
        Text t = rect.gameObject.AddComponent<Text>();
        t.text = text;
        t.fontSize = 16;
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleLeft;
        t.gameObject.layer = 11;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        return t;
    }
}