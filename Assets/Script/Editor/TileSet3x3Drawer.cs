using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TileSet3x3))]
public class TileSet3x3Drawer : PropertyDrawer
{
   private const float PreviewSize = 70f;
    private const float CellSize = 26f;
    private const float Padding = 6f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        float contentHeight = Mathf.Max(PreviewSize, CellSize * 3);
        return line + spacing + contentHeight + Padding * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float line = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty spriteProp = property.FindPropertyRelative("sprite");

        SerializedProperty upLeftProp = property.FindPropertyRelative("upLeft");
        SerializedProperty upMidProp = property.FindPropertyRelative("upMid");
        SerializedProperty upRightProp = property.FindPropertyRelative("upRight");

        SerializedProperty midLeftProp = property.FindPropertyRelative("midLeft");
        SerializedProperty midMidProp = property.FindPropertyRelative("midMid");
        SerializedProperty midRightProp = property.FindPropertyRelative("midRight");

        SerializedProperty downLeftProp = property.FindPropertyRelative("downLeft");
        SerializedProperty downMidProp = property.FindPropertyRelative("downMid");
        SerializedProperty downRightProp = property.FindPropertyRelative("downRight");

        SerializedProperty[] props = new SerializedProperty[]
        {
            upLeftProp, upMidProp, upRightProp,
            midLeftProp, midMidProp, midRightProp,
            downLeftProp, downMidProp, downRightProp
        };

        GUI.Box(position, GUIContent.none);

        Rect contentRect = new Rect(
            position.x + Padding,
            position.y + Padding,
            position.width - Padding * 2,
            position.height - Padding * 2
        );

        Rect spriteFieldRect = new Rect(
            contentRect.x,
            contentRect.y,
            contentRect.width,
            line
        );

        EditorGUI.PropertyField(spriteFieldRect, spriteProp, label);

        float y = spriteFieldRect.yMax + spacing + Padding;

        Rect previewRect = new Rect(
            contentRect.x,
            y,
            PreviewSize,
            PreviewSize
        );

        DrawSpritePreview(previewRect, spriteProp.objectReferenceValue as Sprite);

        Rect gridRect = new Rect(
            previewRect.xMax + 14f,
            y,
            CellSize * 3,
            CellSize * 3
        );

        DrawIntGrid(gridRect, props);

        EditorGUI.EndProperty();
    }

    private void DrawSpritePreview(Rect rect, Sprite sprite)
    {
        GUI.Box(rect, GUIContent.none);

        if (sprite == null || sprite.texture == null)
            return;

        Texture2D tex = sprite.texture;
        Rect texRect = sprite.textureRect;

        Rect uv = new Rect(
            texRect.x / tex.width,
            texRect.y / tex.height,
            texRect.width / tex.width,
            texRect.height / tex.height
        );

        GUI.DrawTextureWithTexCoords(rect, tex, uv, true);
    }

    private void DrawIntGrid(Rect rect, SerializedProperty[] props)
    {
        Event e = Event.current;

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int index = row * 3 + col;
                SerializedProperty prop = props[index];

                Rect cellRect = new Rect(
                    rect.x + col * CellSize,
                    rect.y + row * CellSize,
                    CellSize,
                    CellSize
                );

                int value = Mathf.Clamp(prop.intValue, -1, 1);
                prop.intValue = value;

                Color backgroundColor = GetColorForValue(value);
                EditorGUI.DrawRect(cellRect, backgroundColor);

                if (cellRect.Contains(e.mousePosition))
                {
                    EditorGUI.DrawRect(cellRect, new Color(1f, 1f, 1f, 0.15f));
                }

                DrawCellBorder(cellRect);

                if (e.type == EventType.MouseDown && e.button == 0 && cellRect.Contains(e.mousePosition))
                {
                    value++;

                    if (value > 1)
                        value = -1;

                    prop.intValue = value;
                    e.Use();
                }

                GUIStyle centeredStyle = new GUIStyle(EditorStyles.boldLabel);
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.normal.textColor = Color.white;

                EditorGUI.LabelField(cellRect," ", centeredStyle);
            }
        }
    }

    private Color GetColorForValue(int value)
    {
        switch (value)
        {
            case -1:
                return new Color(0.75f, 0.25f, 0.25f);
            case 1:
                return new Color(0.25f, 0.65f, 0.3f);
            default:
                return new Color(0.35f, 0.35f, 0.35f);
        }
    }

    private void DrawCellBorder(Rect rect)
    {
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), Color.black);
        EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), Color.black);
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1f, rect.height), Color.black);
        EditorGUI.DrawRect(new Rect(rect.xMax - 1f, rect.y, 1f, rect.height), Color.black);
    }
}