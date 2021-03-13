using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    private int lastYRows = 0;

    private readonly int width = 11;
    private readonly int maxHeight = 9;

    public static KinematicBubbleManager kinematicBuubleManager;
    private Vector2 offsetPosition = new Vector2(0, -0.65f);

    public static MapGenerator mapGenerator;

    private void OnEnable()
    {
        mapGenerator = (MapGenerator)target;
        kinematicBuubleManager = GameObject.FindObjectOfType<KinematicBubbleManager>();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (lastYRows != mapGenerator.yRows && !kinematicBuubleManager.gameStart)
        {
            lastYRows = mapGenerator.yRows;
            GameObject[] placeHolders;
            placeHolders = GameObject.FindGameObjectsWithTag("PlaceHolder");
            foreach (var placeholder in placeHolders)
            {
                DestroyImmediate(placeholder);
            }

            UpdateManagerHeight();

            for (int i = 0; i < lastYRows; i++)
            {
                DrawPlaceHolder(i);
            }
        }

        EditorGUILayout.Space(40);

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Name of Level", EditorStyles.boldLabel);
            string levelName = GUILayout.TextField("Level 1" , EditorStyles.textField);
        }
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Save & Create BubbleContainer"))
        {
            BubblesArrayContainer bubblesArray = ScriptableObject.CreateInstance<BubblesArrayContainer>();
            

            AssetDatabase.CreateAsset(bubblesArray, "Assets/BubbleArrays/NewScripableObject.asset");
            AssetDatabase.SaveAssets();

        }


    }

    private void UpdateManagerHeight()
    {
        int deltaHeight = (int)kinematicBuubleManager.transform.position.y - mapGenerator.yRows;
        if (deltaHeight < 0)
        {
            kinematicBuubleManager.transform.position =
                new Vector2(kinematicBuubleManager.transform.position.x, kinematicBuubleManager.transform.position.y - deltaHeight);
        }
        else
        {
            kinematicBuubleManager.transform.position = 
                new Vector2(kinematicBuubleManager.transform.position.x , maxHeight);
        }
        SceneView.RepaintAll();
    }

    private void DrawPlaceHolder(int i)
    {
        if (i % 2 == 0)
        {
            addPlaceHolderShort(-i + offsetPosition.y);
        }
        else
        {
            addPlaceHolderLong(-i + offsetPosition.y);
        }
    }

    private void addPlaceHolderShort(float y)
    {
        float initialXPos = -(width / 2) + 0.5f;
        for (int i = 0; i < width - 1; i++)
        {
            GameObject placeHolder = Instantiate(mapGenerator.Placeholder, new Vector2(initialXPos, y), Quaternion.identity);
            placeHolder.transform.parent = kinematicBuubleManager.transform;
            placeHolder.transform.localPosition = new Vector2(initialXPos, y);
            Undo.RegisterCreatedObjectUndo(placeHolder, "obj created");
            initialXPos++;
        }
    }

    private void addPlaceHolderLong(float y)
    {
        float initialXPos = (width - 1) / -2;
        for (int i = 0; i < width; i++)
        {
            GameObject placeHolder = Instantiate(mapGenerator.Placeholder, new Vector2(initialXPos, y), Quaternion.identity);
            placeHolder.transform.parent = kinematicBuubleManager.transform;
            placeHolder.transform.localPosition = new Vector2(initialXPos, y);
            Undo.RegisterCreatedObjectUndo(placeHolder, "obj created");
            initialXPos++;
        }
    }
}