using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public static class UIElementFunctions
{

    public static CustomUIElement ImageOnly(Transform parent, Sprite img, Vector3 localPosition, Vector2 size)
    {
        // Rect
        RectTransform rect = AddRect(parent, localPosition, size);
        CustomUIElement uie = rect.gameObject.AddComponent<CustomUIElement>();
        uie.rectGo = rect;
        // Component Image
        uie.imageGo = AddImage(uie.rectGo.transform, img);
        return uie;
    }

    public static CustomUIElement ImageText(Transform parent, Sprite img, string txt, Vector3 localPosition, Vector2 size)
    {
        // Rect
        RectTransform rect = AddRect(parent, localPosition, size);
        CustomUIElement uie = rect.gameObject.AddComponent<CustomUIElement>();
        uie.rectGo = rect;
        // Component Image
        uie.imageGo = AddImage(uie.rectGo.transform, img);
        // Child Rect
        RectTransform chRect = AddRect(uie.rectGo.transform);
        // Child Rect's Text
        uie.textGo = AddText(chRect.transform, txt);
        return uie;
    }

    public static CustomUIElement ButtonImage(Transform parent, Sprite img, Vector3 localPosition, Vector2 size)
    {
        // Rect
        RectTransform rect = AddRect(parent, localPosition, size);
        CustomUIElement uie = rect.gameObject.AddComponent<CustomUIElement>();
        uie.rectGo = rect;
        // Component Image
        uie.imageGo = AddImage(uie.rectGo.transform, img);
        // Button
        uie.buttonGo = AddButton(uie.rectGo.transform);
        return uie;
    }

    public static CustomUIElement ButtonImageText(Transform parent, Sprite img, string txt, Vector3 localPosition, Vector2 size)
    { 
        // Rect
        RectTransform rect = AddRect(parent, localPosition, size);
        CustomUIElement uie = rect.gameObject.AddComponent<CustomUIElement>();
        uie.rectGo = rect;
        // Component Image
        uie.imageGo = AddImage(uie.rectGo.transform, img);
        // Button
        uie.buttonGo = AddButton(uie.rectGo.transform);
        // Child Rect
        RectTransform chRect = AddRect(uie.rectGo.transform);
        // Child Rect's Text
        uie.textGo = AddText(chRect.transform, txt);
        return uie;
    }

    public static CustomUIElement ButtonTextColor(Transform parent, string txt, Color c, Vector3 localPosition, Vector2 size)
    {
        // Rect
        RectTransform rect = AddRect(parent, localPosition, size);
        CustomUIElement uie = rect.gameObject.AddComponent<CustomUIElement>();
        uie.rectGo = rect;
        // Component Image
        uie.imageGo = AddImage(uie.rectGo.transform);
        uie.imageGo.color = c;
        // Button
        uie.buttonGo = AddButton(uie.rectGo.transform);
        // Child Rect
        RectTransform chRect = AddRect(uie.rectGo.transform);
        // Child Rect's Text
        uie.textGo = AddText(chRect.transform, txt);
        return uie;
    }

    private static RectTransform AddRect(Transform parent, Vector3 localPosition, Vector2 size)
    {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();
        rect.SetParent(parent);
        rect.name = "cui_rect";
        rect.gameObject.layer = 9;
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.localPosition = localPosition;
        rect.sizeDelta = size;
        return rect;
    }

    private static RectTransform AddRect(Transform parent)
    {
        RectTransform rect = new GameObject().AddComponent<RectTransform>();
        rect.SetParent(parent);
        rect.name = "cui_rect";
        rect.gameObject.layer = 9;
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(0, 0);
        rect.offsetMax = new Vector2(0, 0);
        return rect;
    }

    private static Image AddImage(Transform parent, Sprite img)
    {
        Image image = parent.gameObject.AddComponent<Image>();
        image.sprite = img;
        return image;
    }

    private static Image AddImage(Transform parent)
    {
        return parent.gameObject.AddComponent<Image>();
    }

    private static Text AddText(Transform parent, string txt)
    {
        Text text = parent.gameObject.AddComponent<Text>();
        text.text = txt;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.gameObject.layer = 11;
        return text;
    }

    private static Button AddButton(Transform parent)
    {
        return parent.gameObject.AddComponent<Button>();
    }

}