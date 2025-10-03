
using UnityEngine;

namespace Game.Car
{
    /// <summary>
    /// ScriptableObject configuration for car stats.
    /// Multiple CarConfig assets can define different cars.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Car/CarConfig", fileName = "CarConfig")]
    public class CarConfig : ScriptableObject
    {
        [Header("Engine Settings")]
        public float accelerationTorque = 250f;
        public float brakeTorque = 400f;
        public float maxAngularVelocity = 3200f;

        [Header("Tilt Settings")]
        public float airTiltTorque = 220f;

        [Header("Suspension Settings")]
        public float suspensionFrequency = 2.2f;
        public float suspensionDamping = 0.45f;

        [Header("Drag Settings")]
        public float groundLinearDamping = 1.2f;
        public float airLinearDamping = 0.2f;
        public float angularDamping = 1.5f;
    }
}