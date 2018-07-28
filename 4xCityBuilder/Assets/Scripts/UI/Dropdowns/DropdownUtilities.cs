using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownUtilities : MonoBehaviour
{

    //Creates & initializes a button(with a text child) inside of the given parent.
    public static Button NewButton(string name, string text, Transform parent, float w, float h )
    {
        RectTransform btnRect = NewUIElement(name, parent);
        btnRect.gameObject.AddComponent<Image>();
        btnRect.gameObject.AddComponent<Button>();
        ScaleRect(btnRect, w, h);
        NewText(text, btnRect); 

        return btnRect.GetComponent<Button>();
    }

    //Creates & initializes an empty recttransform inside the given parent.
    public static RectTransform NewUIElement(string name, Transform parent)
    {
        RectTransform temp = new GameObject().AddComponent<RectTransform>();
        temp.name = name;
        temp.gameObject.layer = 9;
        temp.SetParent(parent);
        temp.localScale = new Vector3(1, 1, 1);
        temp.localPosition = new Vector3(0, 0, 0);
        return temp;
    }

    //Creates & initializes a new text element inside the given parent.
    public static Text NewText(string text, Transform parent)
    {
        RectTransform textRect = NewUIElement("Text", parent);
        Text t = textRect.gameObject.AddComponent<Text>();
        t.text = text;
        t.color = Color.black;
        t.alignment = TextAnchor.MiddleCenter;
        t.gameObject.layer = 11;
        ScaleRect(textRect, 0, 0);
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        return t;
    }

    //Sets width and height with current anchors
    public static void ScaleRect(RectTransform r, float w, float h)
    {
        //Setting size along horizontal axis (width)
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);

        //Setting size along vertical axis (height)
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
}