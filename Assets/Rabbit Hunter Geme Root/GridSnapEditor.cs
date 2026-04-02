#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MyGridMap;

public static class GridSnapEditor
{
    private static bool isRegistered = false;

    [InitializeOnLoadMethod]
    private static void Init()
    {
        EditorApplication.update += EnsureRegistered;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void EnsureRegistered()
    {
        if (isRegistered) return;

        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;

        isRegistered = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            isRegistered = false; // 🔥 force re-register
        }
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (Application.isPlaying) return;
        if (Event.current.type != EventType.MouseUp)
            return;

        var grid = GridMap.Instance;
        if (grid == null) return;

        foreach (var t in Selection.transforms)
        {
            foreach (var comp in t.GetComponents<MonoBehaviour>())
            {
                if (comp is IEntity entity)
                    grid.SnapToGrid(entity, true);

                if (comp is IGridTile tile)
                    grid.SnapToGrid(tile, true);
            }
        }
    }
}
#endif