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

    public List<ResourceDropdownChild> children;
    public float childHeight = 30;
    public int childFontSize = 11;
    public Color normal = Color.white;
    public Color highlighted = Color.red;
    public Color pressed = Color.gray;

    // Use this for initialization
    void Awake ()
    {
        container = DropdownUtilities.NewUIElement("Container", transform);
        container.gameObject.AddComponent<VerticalLayoutGroup>();
        Debug.Log("Container: " + container);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
    }

    public void AddChild()
    {
        if (children == null)
            children = new List<ResourceDropdownChild>();
        children.Add(new ResourceDropdownChild(this));
    }


}

public class ResourceDropdownChild
{
    public GameObject childObj;
    public Text childText;
    public Button.ButtonClickedEvent childEvents;

    private LayoutElement element { get { return childObj.GetComponent<LayoutElement>(); } }
    private Button button { get { return childObj.GetComponent<Button>(); } }
    private Image image { get { return childObj.GetComponent<Image>(); } }
    public ResourceDropdownChild(ResourceDropdown parent)
    {
        Debug.Log(parent);
        Debug.Log(parent.container);
        childObj = DropdownUtilities.NewButton("Child", "Button", parent.container.transform).gameObject;
        
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
}