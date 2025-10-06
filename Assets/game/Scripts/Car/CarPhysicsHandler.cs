using UnityEngine;

namespace Game.Car
{
    public class CarPhysicsHandler
    {
        private readonly CarConfigStats config;
        private CarSuspensionHandler suspension;
        private RPMMeter rPMMeter = new RPMMeter();
        private BoostMeter boostMeter = new BoostMeter();
        private LayerMask groundLayer;

        public CarPhysicsHandler(CarConfigStats config)
        {
            this.config = config;
        }

        public void Initialize(CarSuspensionHandler suspensionHandler, RectTransform rpmNeedleTransform, RectTransform boostNeedleTransform)
        {
            suspension = suspensionHandler;
            groundLayer = LayerMask.GetMask("ground");
            rPMMeter.Initialize(config.maxAngularVelocity.Value, rpmNeedleTransform);
            boostMeter.Initialize(config.maxAngularVelocity.Value, boostNeedleTransform);
        }

        public void HandleDrive(InputController input)
        {
            if (input.brakePressed)
                ApplyBrake();
            else
                ApplyAcceleration(input);

            ClampWheelAngularVelocity();
        }

        internal void ResetValues()
        {
            suspension.CarBody.linearVelocity = Vector2.zero;
            suspension.CarBody.angularVelocity = 0f;
            suspension.FrontWheel.angularVelocity = 0f;
            suspension.BackWheel.angularVelocity = 0f;
        }

        private void ApplyAcceleration(InputController input)
        {
            float torque = -input._moveInput * config.accelerationTorque.Value * Time.fixedDeltaTime;

            //Debug.Log("torque : " + torque);
            suspension.FrontWheel.AddTorque(torque, ForceMode2D.Force);
            suspension.BackWheel.AddTorque(torque, ForceMode2D.Force);

            if (IsGrounded(suspension.BackWheel) && !IsGrounded(suspension.FrontWheel) && Mathf.Abs(input._moveInput) > 0.01f)
            {
                float flipTorque = input._moveInput * config.accelerationTorque.Value * Time.fixedDeltaTime;

                // Debug.Log("flipTorque: " + flipTorque);
                suspension.CarBody.AddTorque(flipTorque * 0.8f, ForceMode2D.Force);
            }

            rPMMeter.UpdateNeedle(Mathf.Abs(suspension.BackWheel.angularVelocity * input._moveInput) * 10f);
            boostMeter.UpdateNeedle(Mathf.Abs(suspension.BackWheel.angularVelocity * input._moveInput) * 3);
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

            // rPMMeter.UpdateNeedle(Mathf.Abs(suspension.BackWheel.angularVelocity * input._moveInput) * 10f);
        }

        public void HandleTilt(InputController input)
        {
            bool inAir = !IsGrounded(suspension.FrontWheel) && !IsGrounded(suspension.BackWheel);
            if (inAir && !input.brakePressed)
            {
                float tilt = input._moveInput * config.airTiltTorque.Value * Time.fixedDeltaTime;
                suspension.CarBody.AddTorque(tilt, ForceMode2D.Force);
            }
            // rPMMeter.UpdateNeedle(Mathf.Abs(suspension.BackWheel.angularVelocity) * 10f);
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
