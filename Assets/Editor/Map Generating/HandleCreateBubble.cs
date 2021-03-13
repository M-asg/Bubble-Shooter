using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HandleCreateBubble : MonoBehaviour
{
    static HandleCreateBubble()
    {
        SceneView.duringSceneGui -= OnSceneGui;
        SceneView.duringSceneGui += OnSceneGui;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGui;
    }

    public static void OnSceneGui(SceneView sceneView)
    {
        if (Event.current == null)
        {
            return;
        }

        if (ToolBarAndPilet.SelectionTool == 0)
        {
            return;
        }

        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // if mouseButton 1 clicked
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (ToolBarAndPilet.SelectionTool == 1)
            {
                RemoveBubble(ray);
            }
            else if (ToolBarAndPilet.SelectionTool == 2)
            {
                AddBubble(ray);
            }
        }
    }

    private static string _bubbleLayerName = "Bubble";
    private static string _mapGeneratorLayerName = "MapGenerator";

    private static void RemoveBubble(Ray ray)
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, 1 << LayerMask.NameToLayer(_bubbleLayerName));
        if (hit)
        {
            DestroyImmediate(hit.collider.gameObject);
        }
    }

    private static void AddBubble(Ray ray)
    {
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity , 1 << LayerMask.NameToLayer(_mapGeneratorLayerName));
        RaycastHit2D bubbleHit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity , 1 << LayerMask.NameToLayer(_bubbleLayerName));

        

        if (hit && !bubbleHit)
        {
                GameObject bubble = Instantiate(MapGeneratorEditor.mapGenerator.Bubbles[ToolBarAndPilet.BubbleSelected].BubblePrefab,
                   new Vector3(hit.transform.position.x, hit.transform.position.y, 0), Quaternion.identity);
                bubble.transform.parent = MapGeneratorEditor.kinematicBuubleManager.transform;
            
        }
    }
}