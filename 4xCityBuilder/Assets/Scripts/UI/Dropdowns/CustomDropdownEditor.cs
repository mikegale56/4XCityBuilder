/*using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(CustomDropdown))]
public class SDropdownEditor : Editor
{
    public CustomDropdown currDropdown;

    void OnEnable()
    {
        currDropdown = target as CustomDropdown;
    }

    public override void OnInspectorGUI()
    {
        VerifyValid();
        if (GUILayout.Button("Add Child"))
        {
            AddChild();
        }
        currDropdown.isOpen = EditorGUILayout.Toggle("Is Open?", currDropdown.isOpen);

        GUILayout.Space(5);
        currDropdown.mainText.text = EditorGUILayout.TextField("Main Text", currDropdown.mainText.text);
        currDropdown.mainText.fontSize = EditorGUILayout.IntField("Font Size", currDropdown.mainText.fontSize);

        GUILayout.Space(5);
        currDropdown.mainText.font = (Font)EditorGUILayout.ObjectField("Font", currDropdown.mainText.font, typeof(Font), false);
        currDropdown.mainText.color = EditorGUILayout.ColorField("Font Color", currDropdown.mainText.color);

        GUILayout.Space(5);
        currDropdown.image.sprite = (Sprite)EditorGUILayout.ObjectField("Button Sprite", currDropdown.image.sprite, typeof(Sprite), false, GUILayout.Height(16));
        currDropdown.image.type = (Image.Type)EditorGUILayout.EnumPopup("Type", currDropdown.image.type);
        currDropdown.normal = EditorGUILayout.ColorField("Button Normal", currDropdown.normal);
        currDropdown.highlighted = EditorGUILayout.ColorField("Button Highlighted", currDropdown.highlighted);
        currDropdown.pressed = EditorGUILayout.ColorField("Button Pressed", currDropdown.pressed);
        currDropdown.image.color = currDropdown.normal;

        UpdateChildren();
        currDropdown.Update();
        EditorUtility.SetDirty(currDropdown);
        Repaint();
    }

    void VerifyValid()
    {
        if (currDropdown.image == null)
            currDropdown.gameObject.AddComponent<Image>();

        if (currDropdown.container == null)
        {
            if (currDropdown.transform.Find("Container") == null)
            {
                // I think this here creates the new one?
                currDropdown.container = DropdownUtilities.NewUIElement("Container", currDropdown.transform);
                currDropdown.container.gameObject.AddComponent<VerticalLayoutGroup>();
                DropdownUtilities.ScaleRect(currDropdown.container, 0, 0);
                currDropdown.container.anchorMin = new Vector2(0, 0);
                currDropdown.container.anchorMax = new Vector2(1, 0);
            }
            else
                currDropdown.container = currDropdown.transform.Find("Container").GetComponent<RectTransform>();
        }

        if (currDropdown.mainText == null)
        {
            if (currDropdown.transform.Find("Text") == null)
            {
                currDropdown.mainText = DropdownUtilities.NewText("Dropdown", currDropdown.transform);
            }
            else
                currDropdown.mainText = currDropdown.transform.Find("Text").GetComponent<Text>();

        }
    }
    void AddChild()
    {
        if (currDropdown.children == null)
            currDropdown.children = new List<CustomDropdownChild>();
        currDropdown.children.Add(new CustomDropdownChild(currDropdown));
    }

    void UpdateChildren()
    {
        if (currDropdown.children == null)
            return;
        GUILayout.Space(15);
        GUILayout.Label("Edit Children", EditorStyles.centeredGreyMiniLabel);
        currDropdown.childHeight = EditorGUILayout.FloatField("Child Height", currDropdown.childHeight);
        currDropdown.childFontSize = EditorGUILayout.IntField("Child Font Size", currDropdown.childFontSize);
        for (int i = 0; i < currDropdown.children.Count; i++)
            if (currDropdown.children[i].UpdateChild(currDropdown) == false)
                currDropdown.children.RemoveAt(i);
    }
}*/