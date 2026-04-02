using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyGridMap
{
    [DisallowMultipleComponent]
    public class GridMap : MonoBehaviour
    {
        private static GridMap _instance;
        public static GridMap Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GridMap>(FindObjectsInactive.Include);
                }
                return _instance;
            }
        }

        [Header("Grid Settings")]
        [SerializeField] private Vector2 cellSize = Vector2.one;
        [SerializeField] private int width = 5;
        [SerializeField] private int height = 5;

        // ✅ Separate layers
        private readonly Dictionary<Vector2Int, IGridTile> tiles = new();
        private readonly Dictionary<Vector2Int, IEntity> entities = new();

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"GridMap already exists! Destroying duplicate '{gameObject.name}'");
                Destroy(gameObject); // 🔥 use Immediate in editor context
                return;
            }

            _instance = this;
        }

        // =========================
        // TILE
        // =========================
        public void AddTile(Vector2Int index, IGridTile tile)
        {
            if (tiles.ContainsKey(index))
            {
                Debug.LogWarning($"Tile already exists at {index}", tile as MonoBehaviour);
                return;
            }

            tile.GridPosition = index;
            tiles.Add(index, tile);

            if (tile is MonoBehaviour mb)
                mb.transform.SetParent(transform, false);

            SnapToGrid(tile);
        }

        // =========================
        // ENTITY
        // =========================
        public void AddEntity(Vector2Int index, IEntity entity)
        {
            if (entities.ContainsKey(index))
            {
                Debug.LogWarning($"Entity already exists at {index}");
                return;
            }

            entity.GridPosition = index;
            entities.Add(index, entity);

            SnapToGrid(entity);
        }

        public bool MoveEntity(IEntity entity, Vector2Int to)
        {
            Vector2Int from = entity.GridPosition;

            // tile check
            if (!tiles.TryGetValue(to, out var tile))
                return false;

            // entity collision
            if (entities.ContainsKey(to))
                return false;

            // remove old
            entities.Remove(from);

            // add new
            entities[to] = entity;
            entity.GridPosition = to;

            // snap
            SnapToGrid(entity);

            // trigger tile effect
            if (tile is ITileEnter enter)
                enter.OnEnter(entity);

            return true;
        }

        // =========================
        // GET
        // =========================
        public bool TryGetTile(Vector2Int index, out IGridTile tile)
        {
            return tiles.TryGetValue(index, out tile);
        }

        public bool TryGetEntity(Vector2Int index, out IEntity entity)
        {
            return entities.TryGetValue(index, out entity);
        }

        // =========================
        // SNAP
        // =========================
        public void SnapToGrid(object obj, bool useLocalPosition = false)
        {
            if (obj is not MonoBehaviour mb) return;

            Vector2Int gridPos;

            if (useLocalPosition)
            {
                // Convert current transform → grid
                gridPos = LocalToGrid(mb.transform.localPosition);

                // Write back to object
                switch (obj)
                {
                    case IGridTile t:
                        t.GridPosition = gridPos;
                        break;

                    case IEntity e:
                        e.GridPosition = gridPos;
                        break;
                }
            }
            else
            {
                // Use stored GridPosition
                gridPos = obj switch
                {
                    IGridTile t => t.GridPosition,
                    IEntity e => e.GridPosition,
                    _ => default
                };
            }

            // Snap transform to grid
            mb.transform.localPosition = GridToLocal(gridPos);
        }

        // =========================
        // CONVERSION
        // =========================
        public Vector3 GridToLocal(Vector2Int index)
        {
            return new Vector3(
                index.x * cellSize.x + cellSize.x * 0.5f,
                index.y * cellSize.y + cellSize.y * 0.5f,
                0f
            );
        }

        public Vector2Int LocalToGrid(Vector3 localPos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(localPos.x / cellSize.x),
                Mathf.FloorToInt(localPos.y / cellSize.y)
            );
        }

        // =========================
        // GIZMOS
        // =========================
        private void OnDrawGizmos()
        {
            if (cellSize.x <= 0 || cellSize.y <= 0) return;

            Vector3 origin = transform.position;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int index = new(x, y);

                    bool hasTile = tiles.ContainsKey(index);
                    bool hasEntity = entities.ContainsKey(index);


                    Vector3 cellPos = origin +
                    new Vector3(x * cellSize.x + cellSize.x * 0.5f,
                     y * cellSize.y + cellSize.y * 0.5f, 0f);

#if UNITY_EDITOR
                    GUIStyle style = new GUIStyle
                    {
                        fontSize = 12,
                        alignment = TextAnchor.MiddleCenter
                    };

                    style.normal.textColor = hasEntity ? Color.yellow :
                                             hasTile ? Color.green : Color.red;

                    Handles.Label(cellPos, $"({x},{y})", style);
#endif
                }
            }
        }
    }
}