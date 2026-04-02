using System.Collections;
using System.Collections.Generic;
using MyGridMap;
using UnityEngine;

public class Box : MonoBehaviour, IEntity, IMovable
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



    private void Start()
    {
        GridMap.Instance.AddEntity(GridPosition, this);
    }

    public bool TryMove(Vector2Int dir)
    {
        return MovementHelper.TryMove(this, dir);
    }
}
