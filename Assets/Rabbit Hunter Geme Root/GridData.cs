using UnityEngine;
namespace MyGridMap
{
    public interface IGridTile
    {
        Vector2Int GridPosition { get; set; }
    }

    public interface IEntity
    {
        Vector2Int GridPosition { get; set; }
    }

    public interface ITileEnter
    {
        void OnEnter(IEntity entity);
    }

    public interface ITileExit
    {
        void OnExit(IEntity entity);
    }

    public interface ITileStay
    {
        void OnStay(IEntity entity);
    }
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public interface IMovable
    {
        bool TryMove(Vector2Int dir);
    }
}