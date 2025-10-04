using UnityEngine;

namespace Game.Car
{
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]
    public class HillClimbCarController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private CarConfigStats carConfig;

        [SerializeField] private RectTransform rpmNeedleTransform;
        [SerializeField] private RectTransform boostNeedleTransform;
        [SerializeField] private InputController inputHandler;
        private CarSuspensionHandler suspensionHandler;
        private CarPhysicsHandler physicsHandler;

        private void Awake()
        {
            //inputHandler = new CarInputHandler();
            suspensionHandler = new CarSuspensionHandler(carConfig);
            physicsHandler = new CarPhysicsHandler(carConfig);

            suspensionHandler.CacheWheelJoints(gameObject);
            suspensionHandler.ConfigureSuspension();
            physicsHandler.Initialize(suspensionHandler, rpmNeedleTransform, boostNeedleTransform);
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

        void OnEnable()
        {
            EventBus.Subscribe<HeadCollidedEvent>(e =>
            {
                physicsHandler.ResetValues();
                inputHandler.ResetValues();
            });
        }

        void OnDisable()
        {
            EventBus.Unsubscribe<HeadCollidedEvent>(e =>
            {
                physicsHandler.ResetValues();
                inputHandler.ResetValues();
            });
        }
    }
}