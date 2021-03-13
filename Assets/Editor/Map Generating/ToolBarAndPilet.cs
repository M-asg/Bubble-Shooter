using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ToolBarAndPilet : Editor
{
    private static int selectiontool;
    public static int BubbleSelected;

    public static int SelectionTool
    {
        set
        {
            if (SelectionTool == value)
            {
                return;
            }

            selectiontool = value;
            SelectionTool = value;

            switch (value)
            {
                case 0: Tools.hidden = true; break;
                default: Tools.hidden = false; break;
            }
        }
        get
        {
            return selectiontool;
        }
    }

    static ToolBarAndPilet()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();
        {
            DrawSelectionTools(sceneView);

            DrawPilet(sceneView);
        }
        Handles.EndGUI();
    }

    private static void DrawSelectionTools(SceneView sceneView)
    {
        GUILayout.BeginArea(new Rect(0, 0, sceneView.position.width, 100), GUIContent.none, EditorStyles.toolbar);
        {
            string[] buttons = new string[] { "None", "Erase", "Paint" };
            SelectionTool = GUILayout.SelectionGrid(SelectionTool, buttons, 3, EditorStyles.toolbarButton);
        }
        GUILayout.EndArea();
    }

    private static void DrawPilet(SceneView sceneView)
    {
        if (SelectionTool == 0 || SelectionTool == 1)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(0, 20, 100, sceneView.position.height), GUIContent.none, EditorStyles.textArea);
        {
            for (int i = 0; i < MapGeneratorEditor.mapGenerator.Bubbles.Length; i++)
            {
                DrawPiletButtons(i);
            }
        }
        GUILayout.EndArea();
    }

    private static void DrawPiletButtons(int index)
    {
        Texture2D texture2D = AssetPreview.GetAssetPreview(MapGeneratorEditor.mapGenerator.Bubbles[index].BubblePrefab);
        GUIContent content = new GUIContent(texture2D);

        GUI.Label(new Rect(10, 100 * index + 5, 100, 25), MapGeneratorEditor.mapGenerator.Bubbles[index].Name);
        if (GUI.Button(new Rect(10, 100 * index + 25, 80, 80), content))
        {
            BubbleSelected = index;
        }
    }
}
