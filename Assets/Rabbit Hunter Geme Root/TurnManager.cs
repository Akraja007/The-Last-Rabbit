using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGridMap;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private Rabbit player;
    [SerializeField] private Hunter hunter;
    [SerializeField] private bool isPlayerTurn = true;
    [Header("--------")]
    [SerializeField] private GridMap gridMap;
    [SerializeField] private Vector2Int start, goal;
    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void SetIsPlayerTurn(bool turn)
    {
        isPlayerTurn = turn;

        if (isPlayerTurn == false)
            hunter.TurnToMove();
    }
    [SerializeField] private float loopInterval = 1;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(loopInterval);

        Queue<IGridTile> buffer = new();
        HashSet<IGridTile> visited = new();
        Dictionary<IGridTile, IGridTile> parent = new(); // 👈 track path

        if (!gridMap.TryGetTile(start, out IGridTile startTile))
            yield break;

        if (!gridMap.TryGetTile(goal, out IGridTile goalTile))
            yield break;

        buffer.Enqueue(startTile);
        visited.Add(startTile);

        while (buffer.Count > 0)
        {
            IGridTile tile = buffer.Dequeue();
            Debug.Log(tile.GridPosition);

            // 🎯 Goal check (clean)
            if (tile == goalTile)
            {
                Debug.Log("GOAL FOUND");

                // 🔥 Reconstruct path
                List<IGridTile> path = new();
                IGridTile current = tile;

                while (current != startTile)
                {
                    path.Add(current);
                    current = parent[current];
                }
                path.Add(startTile);
                path.Reverse();

                // 🎮 visualize path
                foreach (var p in path)
                {
                    Debug.Log("PATH: " + p.GridPosition);
                    if (p is MonoBehaviour mb)
                    {
                        SpriteRenderer sr = mb.GetComponent<SpriteRenderer>();
                        if (sr != null)
                            sr.color = Color.cyan;
                    }
                    yield return new WaitForSeconds(loopInterval);
                    // p.HighlightPath(); // your visual
                }

                yield break;
            }

            Vector2Int[] neighborsId =
            {
            tile.GridPosition + Vector2Int.down,
            tile.GridPosition + Vector2Int.up,
            tile.GridPosition + Vector2Int.left,
            tile.GridPosition + Vector2Int.right
        };

            foreach (var nId in neighborsId)
            {
                if (gridMap.TryGetTile(nId, out IGridTile neighbor))
                {
                    if (visited.Contains(neighbor)) continue;

                    buffer.Enqueue(neighbor);
                    visited.Add(neighbor);

                    parent[neighbor] = tile; // 👈 track parent
                }
            }

            // yield return new WaitForSeconds(loopInterval);
        }

        Debug.Log("No path found");
    }
}
