using UnityEngine;

namespace Game.Car
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HillClimbCarController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private CarConfig carConfig;

        private CarInputHandler inputHandler;
        private CarSuspensionHandler suspensionHandler;
        private CarPhysicsHandler physicsHandler;

        private void Awake()
        {
            inputHandler = new CarInputHandler();
            suspensionHandler = new CarSuspensionHandler(carConfig);
            physicsHandler = new CarPhysicsHandler(carConfig);

            suspensionHandler.CacheWheelJoints(gameObject);
            suspensionHandler.ConfigureSuspension();
            physicsHandler.Initialize(suspensionHandler);
        }

        private void Update()
        {
            inputHandler.ReadInput();
        }

        private void FixedUpdate()
        {
            physicsHandler.HandleDrive(inputHandler);
            physicsHandler.HandleTilt(inputHandler);
            physicsHandler.ApplyDrag();
        }
    }
}