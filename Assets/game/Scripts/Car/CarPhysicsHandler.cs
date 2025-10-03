using UnityEngine;

namespace Game.Car
{
    public class CarPhysicsHandler
    {
        private readonly CarConfigStats config;
        private CarSuspensionHandler suspension;
        private LayerMask groundLayer;

        public CarPhysicsHandler(CarConfigStats config)
        {
            this.config = config;
        }

        public void Initialize(CarSuspensionHandler suspensionHandler)
        {
            suspension = suspensionHandler;
            groundLayer = LayerMask.GetMask("ground");
        }

        public void HandleDrive(InputController input)
        {
            if (input.brakePressed)
                ApplyBrake();
            else
                ApplyAcceleration(input);

            ClampWheelAngularVelocity();
        }

        private void ApplyAcceleration(InputController input)
        {
            float torque = -input._moveInput * config.accelerationTorque.Value * Time.fixedDeltaTime;

            Debug.Log("torque : " + torque);
            suspension.FrontWheel.AddTorque(torque, ForceMode2D.Force);
            suspension.BackWheel.AddTorque(torque, ForceMode2D.Force);

            if (IsGrounded(suspension.BackWheel) && !IsGrounded(suspension.FrontWheel) && Mathf.Abs(input._moveInput) > 0.01f)
            {
                float flipTorque = input._moveInput * config.accelerationTorque.Value * Time.fixedDeltaTime;

                Debug.Log("flipTorque: " + flipTorque);
                suspension.CarBody.AddTorque(flipTorque * 0.8f, ForceMode2D.Force);
            }
        }

        private void ApplyBrake()
        {
            bool inAir = !IsGrounded(suspension.FrontWheel) && !IsGrounded(suspension.BackWheel);

            if (inAir || !IsGrounded(suspension.FrontWheel))
            {
                float flipTorque = config.brakeTorque.Value * Time.fixedDeltaTime;
                suspension.CarBody.AddTorque(-flipTorque, ForceMode2D.Force);
            }
            else
            {
                float torque = config.brakeTorque.Value * Time.fixedDeltaTime;
                suspension.BackWheel.AddTorque(torque, ForceMode2D.Force);
            }
        }

        public void HandleTilt(InputController input)
        {
            bool inAir = !IsGrounded(suspension.FrontWheel) && !IsGrounded(suspension.BackWheel);
            if (inAir && !input.brakePressed)
            {
                float tilt = input._moveInput * config.airTiltTorque.Value * Time.fixedDeltaTime;
                suspension.CarBody.AddTorque(tilt, ForceMode2D.Force);
            }
        }

        public void ApplyDrag()
        {
            suspension.CarBody.linearDamping =
                (IsGrounded(suspension.FrontWheel) || IsGrounded(suspension.BackWheel))
                ? config.groundLinearDamping.Value
                : config.airLinearDamping.Value;

            suspension.CarBody.angularDamping = config.angularDamping.Value;
        }

        private void ClampWheelAngularVelocity()
        {
            suspension.FrontWheel.angularVelocity =
                Mathf.Clamp(suspension.FrontWheel.angularVelocity, -config.maxAngularVelocity.Value, config.maxAngularVelocity.Value);
            suspension.BackWheel.angularVelocity =
                Mathf.Clamp(suspension.BackWheel.angularVelocity, -config.maxAngularVelocity.Value, config.maxAngularVelocity.Value);
        }

        private bool IsGrounded(Rigidbody2D wheelRB)
        {
            return wheelRB.IsTouchingLayers(groundLayer);
        }
    }
}
