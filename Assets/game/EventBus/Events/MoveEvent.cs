using System.Numerics;

public struct MoveEvent : IEvent
{
    public Vector2 speed;
    public MoveEvent(Vector2 speed)
    {
        this.speed = speed;
    }
}