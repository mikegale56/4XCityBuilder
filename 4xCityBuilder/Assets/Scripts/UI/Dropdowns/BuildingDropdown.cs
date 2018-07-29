using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropdownBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isOpen;
    public string tooltip = "";
    public string tooltipData = "";
    public float childHeight = 30;
    public int childFontSize = 12;
    public GameObject thisGo;
    public RectTransform container;
    public Text textGo;
    public Color normal = Color.white;
    public Color highlighted = Color.gray;
    public Color pressed = Color.green;
    public List<DropdownBase> children;
    public Button.ButtonClickedEvent events;
    public Image imageGo { get { return GetComponent<Image>(); } }
    public DropdownBase mainDropdown;

    public Button button { get { return thisGo.GetComponent<Button>(); } }
    private LayoutElement element { get { return thisGo.GetComponent<LayoutElement>(); } }
    private GUIStyle style;

    // Use this for initialization
    void Awake()
    {
        isOpen = false;
        container = DropdownUtilities.NewUIElement("Container", transform);
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        DropdownUtilities.ScaleRect(container, 0, 0);

        // Text defaults
        textGo = transform.Find("Text").GetComponent<Text>();
        textGo.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textGo.color = Color.black;

        // Get this game object
        thisGo = this.gameObject;

        // Base, but these get changed later in subbuttons
        container.anchorMin = new Vector2(0, 0);
        container.anchorMax = new Vector2(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 10);
        container.localScale = scale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
        tooltip = tooltipData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
        tooltip = "";
    }

    //public void OnClick()

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle();
            style.fontSize = 14;
            style.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        if (children != null)
        { 
            foreach (DropdownBase child in children)
            {
                if (child.tooltip != "")
                    GUI.Label(new Rect(Input.mousePosition.x + 25, Screen.height - Input.mousePosition.y, child.tooltip.Length * 10, 20), child.tooltip, style);
            }
        }
    }

    public void HideAll()
    {
        this.textGo.enabled = false;
        this.imageGo.enabled = false;
        this.enabled = false;
    }

    public void ShowAll()
    {
        this.textGo.enabled = true;
        this.imageGo.enabled = true;
        this.enabled = true;
    }

    public void AddChild()
    {
        if (children == null)
            children = new List<DropdownBase>();

        GameObject childObj = DropdownUtilities.NewButton("Child", "Button", this.container.transform, 64, 64).gameObject;
        DropdownChild dbChild = childObj.AddComponent<DropdownChild>();
        if (mainDropdown == null)
            mainDropdown = this;
        dbChild.mainDropdown = this.mainDropdown;
        children.Add(dbChild);
        //dbChild.thisGo = childObj;

        // Anchor to lower right
        dbChild.container.anchorMin = new Vector2(1, 0);
        dbChild.container.anchorMax = new Vector2(1, 0);

        dbChild.thisGo.AddComponent<LayoutElement>();

        dbChild.element.minHeight = this.childHeight;

        // Set image sprite and type
        dbChild.imageGo.sprite = this.imageGo.sprite;
        dbChild.imageGo.type = this.imageGo.type;

        // Set childText font, font color, and font size
        dbChild.textGo.font = this.textGo.font;
        dbChild.textGo.color = this.textGo.color;
        dbChild.textGo.fontSize = this.childFontSize;

        // Set button normal, highlighted, pressed colors
        dbChild.events = dbChild.button.onClick;

        ColorBlock b = button.colors;
        b.normalColor = this.normal;
        b.highlightedColor = this.highlighted;
        b.pressedColor = this.pressed;
        button.colors = b;

        // Set button's onClick and childEvents
        dbChild.button.onClick = dbChild.events;
        dbChild.button.onClick.AddListener(delegate() { CloseButton(); });
        dbChild.button.onClick.AddListener(delegate () { ChangeMainButton(dbChild.textGo.text, dbChild.imageGo.sprite); });
    }

    public void ChangeMainButton(string s, Sprite sp)
    {
        if (!string.IsNullOrEmpty(s))
            this.mainDropdown.textGo.text = s;
        if (sp != null)
        {
            this.mainDropdown.imageGo.sprite = sp;
            this.mainDropdown.imageGo.color = Color.white;
        }
    }

    public void CloseButton()
    {
        isOpen = false;
        Vector3 scale = container.localScale;
        scale.y = 0;
        container.localScale = scale;
    }
}

public class DropdownChild : DropdownBase
{
    void Awake()
    {
        isOpen = false;
        container = DropdownUtilities.NewUIElement("Container", transform);
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        DropdownUtilities.ScaleRect(container, 0, 0);

        // Text defaults
        textGo = transform.Find("Text").GetComponent<Text>();
        textGo.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textGo.color = Color.black;

        // Get this game object
        thisGo = this.gameObject;

        // Base, but these get changed later in subobjects
        container.anchorMin = new Vector2(1, 0);
        container.anchorMax = new Vector2(1, 0);

        container.pivot = new Vector2(0, 0);
        container.sizeDelta = new Vector2(160, 32);

    }

    void Update()
    {
        Vector3 scale = container.localScale;
        if (isOpen)
        {
            scale.x = Mathf.Lerp(scale.x, isOpen ? 1 : 0, Time.deltaTime * 10);
            scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 10);
        } else
        {
            scale.x = Mathf.Lerp(scale.x, isOpen ? 1 : 0, Time.deltaTime * 3);
            scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 3);
        }
        
        container.localScale = scale;
    }

    new public void CloseButton()
    {
        isOpen = false;
        Vector3 scale = container.localScale;
        scale.x = 0;
        scale.y = 0;
        container.localScale = scale;
    }
}