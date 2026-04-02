using UnityEngine;
using MyGridMap;

public class Rabbit : MonoBehaviour, IEntity, IMovable
{

    private Vector2 mouseDown;

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


    private void Start()
    {
        GridMap.Instance.AddEntity(GridPosition, this);
    }

    private void OnMouseDown()
    {
        if (turnManager.IsPlayerTurn() == false) return;
        mouseDown = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (turnManager.IsPlayerTurn() == false) return;

        Vector2 swipe = (Vector2)Input.mousePosition - mouseDown;

        if (swipe.magnitude < 20f) return;

        if (MovementHelper.TryGetCardinalDirection(swipe, out Vector2Int moveDir))

        if(TryMove(moveDir))
        turnManager.SetIsPlayerTurn(false);
    }

    public bool TryMove(Vector2Int dir)
    {
        return MovementHelper.TryMove(this, dir);
    }
}