using UnityEngine;

namespace Game.Car
{
    public class CarPhysicsHandler
    {
        private readonly CarConfig config;
        private CarSuspensionHandler suspension;
        private LayerMask groundLayer;

        public CarPhysicsHandler(CarConfig config)
        {
            this.config = config;
        }

        public void Initialize(CarSuspensionHandler suspensionHandler)
        {
            suspension = suspensionHandler;
            groundLayer = LayerMask.GetMask("Ground");
        }

        public void HandleDrive(CarInputHandler input)
        {
            if (input.IsBraking)
                ApplyBrake();
            else
                ApplyAcceleration(input);

            ClampWheelAngularVelocity();
        }

        private void ApplyAcceleration(CarInputHandler input)
        {
            float torque = -input.Horizontal * config.accelerationTorque * Time.fixedDeltaTime;
            suspension.FrontWheel.AddTorque(torque, ForceMode2D.Force);
            suspension.BackWheel.AddTorque(torque, ForceMode2D.Force);

            if (IsGrounded(suspension.BackWheel) && !IsGrounded(suspension.FrontWheel) && Mathf.Abs(input.Horizontal) > 0.01f)
            {
                float flipTorque = input.Horizontal * config.accelerationTorque * Time.fixedDeltaTime;
                suspension.CarBody.AddTorque(flipTorque, ForceMode2D.Force);
            }
        }

        private void ApplyBrake()
        {
            bool inAir = !IsGrounded(suspension.FrontWheel) && !IsGrounded(suspension.BackWheel);

            if (inAir || !IsGrounded(suspension.FrontWheel))
            {
                float flipTorque = config.brakeTorque * Time.fixedDeltaTime;
                suspension.CarBody.AddTorque(-flipTorque, ForceMode2D.Force);
            }
            else
            {
                float torque = config.brakeTorque * Time.fixedDeltaTime;
                suspension.BackWheel.AddTorque(torque, ForceMode2D.Force);
            }
        }

        public void HandleTilt(CarInputHandler input)
        {
            bool inAir = !IsGrounded(suspension.FrontWheel) && !IsGrounded(suspension.BackWheel);
            if (inAir && !input.IsBraking)
            {
                float tilt = input.Horizontal * config.airTiltTorque * Time.fixedDeltaTime;
                suspension.CarBody.AddTorque(tilt, ForceMode2D.Force);
            }
        }

        public void ApplyDrag()
        {
            suspension.CarBody.linearDamping =
                (IsGrounded(suspension.FrontWheel) || IsGrounded(suspension.BackWheel))
                ? config.groundLinearDamping
                : config.airLinearDamping;

            suspension.CarBody.angularDamping = config.angularDamping;
        }

        private void ClampWheelAngularVelocity()
        {
            suspension.FrontWheel.angularVelocity =
                Mathf.Clamp(suspension.FrontWheel.angularVelocity, -config.maxAngularVelocity, config.maxAngularVelocity);
            suspension.BackWheel.angularVelocity =
                Mathf.Clamp(suspension.BackWheel.angularVelocity, -config.maxAngularVelocity, config.maxAngularVelocity);
        }

        private bool IsGrounded(Rigidbody2D wheelRB)
        {
            return wheelRB.IsTouchingLayers(groundLayer);
        }
    }
}
