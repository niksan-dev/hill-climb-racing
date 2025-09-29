using System.Linq;
using UnityEngine;

public class CarControllerWithParticles : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Input")]
    [SerializeField] private InputController inputController;
    [Header("Main References")]
    [SerializeField] private Rigidbody2D carBody;
    [SerializeField] private Rigidbody2D frontWheelRB;
    [SerializeField] private Rigidbody2D backWheelRB;
    [SerializeField] private ParticleSystem frontTireDust;
    [SerializeField] private ParticleSystem backTireDust;

    [Header("Engine / Brakes")]
    [SerializeField] private float engineTorque = 200f;
    [SerializeField] private float brakeTorque = 300f;
    [SerializeField] private float tiltTorque = 150f;

    [Header("Suspension")]
    [SerializeField] private float suspensionFrequency = 3f;
    [SerializeField] private float suspensionDamping = 0.5f;

    [Header("Particles")]
    [SerializeField] private float slipThreshold = 20f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wheelRadius = 0.26f;
    [SerializeField] private float slipFactor = 1.2f;

    #endregion

    #region Private Fields

    private WheelJoint2D frontWheelJoint;
    private WheelJoint2D backWheelJoint;

    private float moveInput;
    private bool isBraking;
    private bool isGroundedFront;
    private bool isGroundedBack;

    private readonly Vector3 tireSizeOffset = new Vector3(0.05f, -0.266f, 0);
    [Range(700, 1000)]
    private const float MaxBrakingAngularVelocity = 700f;
    [Range(2500, 4000)]
    private const float MaxForwardAngularVelocity = 3200f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        CacheWheelJoints();
        ApplySuspension(frontWheelJoint);
        ApplySuspension(backWheelJoint);
    }

    private void Update()
    {
        ReadInput();
        UpdateGroundedStatus();
    }

    private void FixedUpdate()
    {
        HandleDrive();
        HandleTilt();
        HandleBrakes();
        HandleTireDust();
    }

    #endregion

    #region Input & State

    private void ReadInput()
    {
#if UNITY_EDITOR
        //moveInput = Input.GetAxisRaw("Horizontal");
        //isBraking = Input.GetKey(KeyCode.Space);
        moveInput = inputController._moveInput;
        isBraking = inputController.brakePressed;
#else

        moveInput = inputController._moveInput;
        isBraking = inputController.brakePressed;
#endif
    }

    private void UpdateGroundedStatus()
    {
        isGroundedFront = frontWheelRB.IsTouchingLayers(groundLayer);
        isGroundedBack = backWheelRB.IsTouchingLayers(groundLayer);
    }

    #endregion

    #region Drive & Tilt

    private void HandleDrive()
    {
        if (isBraking || (!isGroundedFront && !isGroundedBack)) return;
        Debug.Log("========DRIVE========");
        ApplyTorqueToWheels(-moveInput * engineTorque);
        ApplyTorqueToCar(-moveInput * engineTorque);
        ClampWheelAngularVelocity(MaxForwardAngularVelocity);
    }

    private void HandleTilt()
    {
        if (isGroundedFront || isGroundedBack) return;
        // Negative sign controls which direction gas tilts
        Debug.Log("========TILT========");
        ApplyTorqueToCar(moveInput * tiltTorque);
    }

    private void ApplyTorqueToCar(float torque)
    {
        float torqueDelta = torque * Time.fixedDeltaTime;
        carBody.AddTorque(torqueDelta, ForceMode2D.Force);
    }



    private void ApplyTorqueToWheels(float torque)
    {
        float torqueDelta = torque * Time.fixedDeltaTime;
        frontWheelRB.AddTorque(torqueDelta, ForceMode2D.Force);
        backWheelRB.AddTorque(torqueDelta, ForceMode2D.Force);
    }

    #endregion

    #region Brakes

    private void HandleBrakes()
    {
        if (!isBraking) return;

        Debug.Log("========BRAKE========");
        if (frontWheelRB.angularVelocity < 0)
        {
            ApplyTorqueToWheels(brakeTorque);
            ApplyTorqueToCar(brakeTorque);
        }
        else
        {
            ApplyTorqueToWheels(brakeTorque * 0.3f);
        }

        ClampWheelAngularVelocity(MaxBrakingAngularVelocity);
    }

    #endregion

    #region Wheel Angular Velocity

    private void ClampWheelAngularVelocity(float maxAngularVelocity)
    {
        frontWheelRB.angularVelocity = Mathf.Clamp(frontWheelRB.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        backWheelRB.angularVelocity = Mathf.Clamp(backWheelRB.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    #endregion

    #region Tire Dust

    private void HandleTireDust()
    {
        float carSpeed = carBody.linearVelocity.x;
        bool frontSlip = IsWheelSlipping(frontWheelRB, isGroundedFront, carSpeed);
        bool backSlip = IsWheelSlipping(backWheelRB, isGroundedBack, carSpeed);

        SetParticleState(frontTireDust, frontSlip);
        SetParticleState(backTireDust, backSlip);

        if (frontSlip || backSlip)
            SetDustPosition();
    }

    private bool IsWheelSlipping(Rigidbody2D wheelRB, bool isGrounded, float carSpeed)
    {
        float radPerSec = wheelRB.angularVelocity * Mathf.Deg2Rad;
        float wheelSpeed = radPerSec * wheelRadius;
        return isGrounded && Mathf.Abs(wheelSpeed) > Mathf.Abs(carSpeed) * slipFactor;
    }

    private void SetParticleState(ParticleSystem ps, bool shouldPlay)
    {
        if (shouldPlay)
        {
            if (!ps.isPlaying) ps.Play();
        }
        else
        {
            if (ps.isPlaying) ps.Stop();
        }
    }

    private void SetDustPosition()
    {
        frontTireDust.transform.position = frontWheelRB.transform.position + tireSizeOffset;
        backTireDust.transform.position = backWheelRB.transform.position + tireSizeOffset;
    }

    #endregion

    #region Suspension

    private void CacheWheelJoints()
    {
        var joints = GetComponentsInChildren<WheelJoint2D>().ToList();
        frontWheelJoint = joints[0];
        backWheelJoint = joints[1];
    }

    private void ApplySuspension(WheelJoint2D joint)
    {
        JointSuspension2D susp = joint.suspension;
        susp.frequency = suspensionFrequency;
        susp.dampingRatio = suspensionDamping;
        joint.suspension = susp;
    }

    #endregion

    #region Upgrades

    public void UpgradeEngine(float extraTorque)
    {
        engineTorque += extraTorque;
    }

    public void UpgradeSuspension(float addFrequency, float addDamping)
    {
        suspensionFrequency += addFrequency;
        suspensionDamping = Mathf.Clamp01(suspensionDamping + addDamping);
        ApplySuspension(frontWheelJoint);
        ApplySuspension(backWheelJoint);
    }

    #endregion
}