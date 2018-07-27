/*using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CustomDropdown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform container;
    public bool isOpen;
    public Text mainText;
    public Image image { get { return GetComponent<Image>(); } }

    public List<CustomDropdownChild> children;
    public float childHeight = 30;
    public int childFontSize = 11;
    public Color normal = Color.white;
    public Color highlighted = Color.red;
    public Color pressed = Color.gray;

    void Start()
    {
        container = transform.Find("Container").GetComponent<RectTransform>();
        isOpen = false;
    }
    public void Update()
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
}

[System.Serializable]
public class CustomDropdownChild
{
    public GameObject childObj;
    public Text childText;
    public Button.ButtonClickedEvent childEvents;

    private LayoutElement element { get { return childObj.GetComponent<LayoutElement>(); } }
    private Button button { get { return childObj.GetComponent<Button>(); } }
    private Image image { get { return childObj.GetComponent<Image>(); } }
    public CustomDropdownChild(CustomDropdown parent)
    {
        childObj = DropdownUtilities.NewButton("Child", "Button", parent.container).gameObject;
        childObj.AddComponent<LayoutElement>();

        childText = childObj.GetComponentInChildren<Text>();

        childEvents = button.onClick;
    }

    public bool UpdateChild(CustomDropdown parent)
    {
        if (childObj == null)
            return false;

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

        return true;
    }
}*/