using MyGridMap;
using UnityEngine;

public static class MovementHelper
{
    public static bool TryMove(IEntity self, Vector2Int dir)
    {
        var grid = GridMap.Instance;
        Vector2Int target = self.GridPosition + dir;

        if (!grid.TryGetTile(target, out _))
            return false;

        // entity collision
        if (grid.TryGetEntity(target, out IEntity entity))
        {
            if (entity is IMovable movable)
            {
                if (!movable.TryMove(dir))
                    return false;
            }
            else
            {
                return false;
            }
        }

        // move
        grid.MoveEntity(self, target);
        return true;
    }

    public static bool TryGetCardinalDirection(Vector2 swipe, out Vector2Int dir)
    {
        dir = Vector2Int.zero;

        float absX = Mathf.Abs(swipe.x);
        float absY = Mathf.Abs(swipe.y);

        if (Mathf.Abs(absX - absY) < 20f)
            return false;

        dir = absX > absY
            ? (swipe.x > 0 ? Vector2Int.right : Vector2Int.left)
            : (swipe.y > 0 ? Vector2Int.up : Vector2Int.down);

        return true;
    }
}