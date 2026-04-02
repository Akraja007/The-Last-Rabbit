using System.Collections;
using System.Collections.Generic;
using MyGridMap;
using UnityEngine;

public class Hunter : MonoBehaviour, IEntity, IMovable
{
    [SerializeField] private TurnManager turnManager;
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

    public void TurnToMove()
    {
        if(turnManager.IsPlayerTurn()) return;
        
    }
    public bool TryMove(Vector2Int dir)
    {
        return MovementHelper.TryMove(this, dir);
    }
}
