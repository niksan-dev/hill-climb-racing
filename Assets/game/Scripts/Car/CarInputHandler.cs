using UnityEngine;

namespace Game.Car
{
    /// <summary>
    /// Handles player input for the car.
    /// Replaceable with AI or network input.
    /// </summary>
    public class CarInputHandler
    {
        public float Horizontal { get; private set; }
        public bool IsBraking { get; private set; }

        public void ReadInput()
        {
            Horizontal = Input.GetAxis("Horizontal");
            IsBraking = Input.GetKey(KeyCode.Space);
        }
    }
}
