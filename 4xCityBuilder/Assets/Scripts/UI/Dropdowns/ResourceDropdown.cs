using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResourceDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public RectTransform container;
    public bool isOpen;
    public Text mainText;
    public Image image { get { return GetComponent<Image>(); } }
    public Image mainImage;
    private GUIStyle style;

    public List<ResourceDropdownChild> children;
    public float childHeight = 64;
    public int childFontSize = 11;
    public Color normal = Color.white;
    public Color highlighted = Color.gray;
    public Color pressed = Color.green;

    // Use this for initialization
    void Awake ()
    {
        container = DropdownUtilities.NewUIElement("Container", transform);
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        DropdownUtilities.ScaleRect(container, 0, 0);
        container.anchorMin = new Vector2(0, 0);
        container.anchorMax = new Vector2(1, 0);
        isOpen = false;
    }
	
	// Update is called once per frame
	void Update ()
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

        foreach (ResourceDropdownChild child in children)
        {
            if (child.tooltip != "")
                GUI.Label(new Rect(Input.mousePosition.x + 25, Screen.height - Input.mousePosition.y, child.tooltip.Length * 10, 20), child.tooltip, style);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
    }
    
    public void AddChild(string resourceName)
    {
        if (children == null)
            children = new List<ResourceDropdownChild>();
        //children.Add(new ResourceDropdownChild(this, resourceName));
        GameObject childObj = DropdownUtilities.NewButton("Child", "Button", this.container.transform, 64, 64).gameObject;
        ResourceDropdownChild rdc = childObj.AddComponent<ResourceDropdownChild>();
        rdc.childObj = childObj;
        rdc.Init(this, resourceName);
        children.Add(rdc);
    }
}

public class ResourceDropdownChild : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject childObj;
    public Text childText;
    public Button.ButtonClickedEvent childEvents;
    public string tooltip = "";
    public string resourceName;

    private LayoutElement element { get { return childObj.GetComponent<LayoutElement>(); } }
    private Button button { get { return childObj.GetComponent<Button>(); } }
    public Image image { get { return childObj.GetComponent<Image>(); } }
    public void Init(ResourceDropdown parent, string resourceName)
    {
        this.resourceName = resourceName;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip = resourceName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip = "";
    }

}