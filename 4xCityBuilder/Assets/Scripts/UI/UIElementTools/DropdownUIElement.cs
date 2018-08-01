using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownUIElement : CustomUIElement, IPointerEnterHandler, IPointerExitHandler
{
	
	//TO DO: Add the option to make it a click-by dropdown or a hover-over dropdown

    // Game Objects
	public RectTransform container; // Container for the vertical layout - rename to vlgRectGo
	public DropdownUIElement rootDropdown;
    public Button.ButtonClickedEvent events;

    // Information
    public bool isOpen = false;
	public bool allowed = true;
	public string defName = "";
	
	// Children
	public float childHeight = 30;
    public int childFontSize = 12;
	public List<DropdownUIElement> children;
	private LayoutElement element { get { return thisGo.GetComponent<LayoutElement>(); } } // For scaling the children
    

    void Awake()
    {
        isOpen = false;
        thisGo = this.gameObject;
		
		container = UIElementFunctions.AddVLGRect(transform);
        container.name = "vlgRect";
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        UIElementFunctions.ScaleRect(container, 0, 0);
        container.anchorMin = new Vector2(0, 0);
        container.anchorMax = new Vector2(1, 0);
		
    }
	
	void Update()
    {
		Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 10);
        container.localScale = scale;
	}

    new public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip = tooltipData;
		isOpen = true;
    }

    new public void OnPointerExit(PointerEventData eventData)
    {
        tooltip = "";
		isOpen = false;
    }

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
            foreach (DropdownUIElement child in children)
            {
                if (child.tooltip != "")
                    GUI.Label(new Rect(Input.mousePosition.x + 25, Screen.height - Input.mousePosition.y, child.tooltip.Length * 10, 20), child.tooltip, style);
            }
        }
    }
	
	public void AddChild()
    {
        if (children == null)
            children = new List<DropdownUIElement>();

        DropdownUIChild duiChild = UIElementFunctions.DropdownChild(this.container.transform, null, "", new Vector3(0, 0, 0), new Vector2(64, 64));
        if (rootDropdown == null)
            rootDropdown = this;

        duiChild.rootDropdown = this.rootDropdown;
        children.Add(duiChild);

        // Anchor to lower right
        duiChild.container.anchorMin = new Vector2(1, 0);
        duiChild.container.anchorMax = new Vector2(1, 0);

        duiChild.thisGo.AddComponent<LayoutElement>();

        duiChild.element.minHeight = this.childHeight;

        // Set image sprite and type
        duiChild.imageGo.sprite = this.imageGo.sprite;
        duiChild.imageGo.type = this.imageGo.type;

        // Set childText font, font color, and font size
        duiChild.textGo.font = this.textGo.font;
        duiChild.textGo.color = this.textGo.color;
        duiChild.textGo.fontSize = this.childFontSize;

        // Set button normal, highlighted, pressed colors
        duiChild.events = duiChild.buttonGo.onClick;

        ColorBlock b = buttonGo.colors;
        b.normalColor = this.normal;
        b.highlightedColor = this.highlighted;
        b.pressedColor = this.pressed;
        buttonGo.colors = b;

        // Set button's onClick and childEvents
        duiChild.buttonGo.onClick = duiChild.events;
        duiChild.buttonGo.onClick.AddListener(delegate() { CloseButton(); });
        duiChild.buttonGo.onClick.AddListener(delegate() { ChangeMainButton(duiChild.textGo.text, duiChild.imageGo.sprite, duiChild.defName); });
    }
	
	public void ChangeMainButton(string s, Sprite sp, string newName)
    {
        if (!string.IsNullOrEmpty(s))
            this.rootDropdown.textGo.text = s;
        if (sp != null)
        {
            this.rootDropdown.imageGo.sprite = sp;
            this.rootDropdown.imageGo.color = Color.white;
        }
        this.defName = newName;
		this.allowed = true;
    }

    public void CloseButton()
    {
        isOpen = false;
        Vector3 scale = container.localScale;
        scale.y = 0;
        container.localScale = scale;
    }
}

public class DropdownUIChild : DropdownUIElement
{
    void Awake()
    {

        isOpen = false;
        thisGo = this.gameObject;
		
		container = UIElementFunctions.AddTextRect(transform);
        container.name = "vlgRect";
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        UIElementFunctions.ScaleRect(container, 0, 0);
		
		// Do I need to do this?
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