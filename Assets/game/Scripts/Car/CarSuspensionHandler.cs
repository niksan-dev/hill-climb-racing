using System.Linq;
using UnityEngine;

namespace Game.Car
{
    public class CarSuspensionHandler
    {
        private readonly CarConfigStats config;

        public Rigidbody2D CarBody { get; private set; }
        public Rigidbody2D FrontWheel { get; private set; }
        public Rigidbody2D BackWheel { get; private set; }
        public WheelJoint2D FrontJoint { get; private set; }
        public WheelJoint2D BackJoint { get; private set; }

        public CarSuspensionHandler(CarConfigStats config)
        {
            this.config = config;
        }

        public void CacheWheelJoints(GameObject car)
        {
            CarBody = car.GetComponent<Rigidbody2D>();
            var joints = car.GetComponentsInChildren<WheelJoint2D>().ToList();
            if (joints.Count >= 2)
            {
                FrontJoint = joints[0];
                BackJoint = joints[1];
                FrontWheel = FrontJoint.connectedBody;
                BackWheel = BackJoint.connectedBody;
            }
        }

        public void ConfigureSuspension()
        {
            SetSuspension(FrontJoint, config.suspensionFrequency.Value, config.suspensionDamping.Value);
            SetSuspension(BackJoint, config.suspensionFrequency.Value, config.suspensionDamping.Value);
        }

        private void SetSuspension(WheelJoint2D wheel, float frequency, float damping)
        {
            JointSuspension2D sus = wheel.suspension;
            sus.frequency = frequency;
            sus.dampingRatio = damping;
            wheel.suspension = sus;
        }
    }
}

