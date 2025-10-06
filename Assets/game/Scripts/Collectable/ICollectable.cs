using UnityEngine;
public interface ICollectable
{
    int Value { get; }
    void Collect(GameObject collector);
}