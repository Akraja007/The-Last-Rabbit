using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGridMap;


public class GridTile : MonoBehaviour, IGridTile
{
    [SerializeField] private Vector2Int gridPosition;
    public Vector2Int GridPosition
    {
        get => gridPosition; set
        {
            if (gridPosition == value) return;

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Grid Change");
#endif

            gridPosition = value;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
    private GridMap gridMap;

    private void Start()
    {
        gridMap = GridMap.Instance;
        gridMap.AddTile(GridPosition, this);
    }
}
