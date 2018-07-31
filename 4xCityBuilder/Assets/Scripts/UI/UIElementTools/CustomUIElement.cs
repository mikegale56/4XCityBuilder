using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // Game Objects
    public RectTransform rectGo;
    public Text textGo;
    public Image imageGo;
    public Button buttonGo;
    public GameObject thisGo;

    // Tooltip
    private string tooltip = "";
    public string tooltipData = "";
    public GUIStyle style;

    // Highlight colors
    public Color normal = Color.white;
    public Color highlighted = Color.gray;
    public Color pressed = Color.green;

    void Start()
    {
        thisGo = this.gameObject;
    }
	
	void Update()
    {
		
	}

    public void Init()
    {

        // Set the normal, highlighted, and pressed colors
        ColorBlock b = buttonGo.colors;
        b.normalColor = this.normal;
        b.highlightedColor = this.highlighted;
        b.pressedColor = this.pressed;
        buttonGo.colors = b;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip = tooltipData;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip = "";
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle();
            style.fontSize = 14;
            style.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        if (tooltip != "")
            GUI.Label(new Rect(Input.mousePosition.x + 25, Screen.height - Input.mousePosition.y, tooltip.Length * 10, 20), tooltip, style);
    }

    public void Hide()
    {
        if (textGo != null)
            textGo.enabled = false;
        if (imageGo != null)
            imageGo.enabled = false;
    }

    public void Show()
    {
        if (textGo != null)
            textGo.enabled = true;
        if (imageGo != null)
            imageGo.enabled = true;
    }

    void OnDestroy()
    {
        
    }

}
