using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public RectTransform container;
    public bool isOpen;
    public Text mainText;
    public Image image { get { return GetComponent<Image>(); } }
    public Image mainImage;
    private GUIStyle style;

    public List<BuildingDropdownChild> children;
    public float childHeight = 30;
    public int childFontSize = 11;
    public Color normal = Color.white;
    public Color highlighted = Color.gray;
    public Color pressed = Color.green;

    // Use this for initialization
    void Awake()
    {
        container = DropdownUtilities.NewUIElement("Container", transform);
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        DropdownUtilities.ScaleRect(container, 0, 0);
        container.anchorMin = new Vector2(0, 0);
        container.anchorMax = new Vector2(1, 0);
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 10);
        container.localScale = scale;
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle();
            style.fontSize = 14;
            style.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        /*foreach (BuildingDropdownChild child in children)
        {
            if (child.tooltip != "")
                GUI.Label(new Rect(Input.mousePosition.x + 25, Screen.height - Input.mousePosition.y, child.tooltip.Length * 10, 20), child.tooltip, style);
        }*/
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
    }

    public void AddChild(string text)
    {
        if (children == null)
            children = new List<BuildingDropdownChild>();
        GameObject childObj = DropdownUtilities.NewButton("Child", "Button", this.container.transform, 64, 64).gameObject;
        BuildingDropdownChild rdc = childObj.AddComponent<BuildingDropdownChild>();
        rdc.childObj = childObj;
        rdc.Init(this, text);
        children.Add(rdc);
    }
}

public class BuildingDropdownChild : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform subContainer;
    public GameObject childObj;
    public Text childText;
    public Button.ButtonClickedEvent childEvents;
    public bool isOpen = false;

    private LayoutElement element { get { return childObj.GetComponent<LayoutElement>(); } }
    private Button button { get { return childObj.GetComponent<Button>(); } }
    public Image image { get { return childObj.GetComponent<Image>(); } }
    public void Init(BuildingDropdown parent, string text)
    {

        !!! HERE
        // Just added subContainer, need to create / assign it and then get the horizontal slide stuff working

        childObj.AddComponent<LayoutElement>();

        childText = childObj.GetComponentInChildren<Text>();

        childEvents = button.onClick;

        element.minHeight = parent.childHeight;

        // Set image sprite and type
        image.sprite = parent.image.sprite;
        image.type = parent.image.type;

        // Set childText font, font color, and font size
        childText.font = parent.mainText.font;
        childText.color = parent.mainText.color;
        childText.fontSize = parent.childFontSize;

        // Set button normal, highlighted, pressed colors
        ColorBlock b = button.colors;
        b.normalColor = parent.normal;
        b.highlightedColor = parent.highlighted;
        b.pressedColor = parent.pressed;
        button.colors = b;

        // Set button's onClick and childEvents
        button.onClick = childEvents;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = subContainer.localScale;
        scale.x = Mathf.Lerp(scale.x, isOpen ? 1 : 0, Time.deltaTime * 10);
        subContainer.localScale = scale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
        //tooltip = buildingName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
        //tooltip = "";
    }

}