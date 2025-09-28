using System.Linq;
using UnityEngine;

public class CarControllerWithParticles : MonoBehaviour
{
    [Header("Main References")]
    [SerializeField] private Rigidbody2D carBody;
    [SerializeField] private Rigidbody2D frontWheelRB;
    [SerializeField] private Rigidbody2D backWheelRB;
    private WheelJoint2D frontWheelJoint;
    private WheelJoint2D backWheelJoint;

    [Header("Engine / Brakes")]
    [SerializeField] private float engineTorque = 200f;
    [SerializeField] private float brakeTorque = 300f;
    [SerializeField] private float tiltTorque = 150f;

    [Header("Suspension")]
    [SerializeField] private float suspensionFrequency = 3f;
    [SerializeField] private float suspensionDamping = 0.5f;

    [Header("Particles")]
    [SerializeField] private ParticleSystem frontTireDust;
    [SerializeField] private ParticleSystem backTireDust;
    [SerializeField] private float slipThreshold = 50f; // when to trigger dust

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private float moveInput;
    private bool isBraking;
    private bool isGroundedFront;
    private bool isGroundedBack;

    void Awake()
    {
        // Adjust center of mass slightly higher to encourage flipping
        carBody.centerOfMass = new Vector2(0f, 0.5f);
        var WheelJoints = GetComponentsInChildren<WheelJoint2D>().ToList();
        frontWheelJoint = WheelJoints[0];
        backWheelJoint = WheelJoints[1];
        ApplySuspension(frontWheelJoint);
        ApplySuspension(backWheelJoint);
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isBraking = Input.GetKey(KeyCode.Space);

        // ground check
        isGroundedFront = frontWheelRB.IsTouchingLayers(groundLayer);
        isGroundedBack = backWheelRB.IsTouchingLayers(groundLayer);
    }

    void FixedUpdate()
    {
        HandleDrive();
        HandleTilt();
        HandleBrakes();
        HandleTireDust();
    }

    private void HandleDrive()
    {
        if (!isBraking)
        {
            // Drive wheels with torque
            frontWheelRB.AddTorque(-moveInput * engineTorque * Time.fixedDeltaTime, ForceMode2D.Force);
            backWheelRB.AddTorque(-moveInput * engineTorque * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private void HandleTilt()
    {
        // mid-air rotation
        if (!isGroundedFront && !isGroundedBack)
        {
            carBody.AddTorque(moveInput * tiltTorque * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private void HandleBrakes()
    {
        if (isBraking)
        {
            // apply opposite torque to slow wheels
            frontWheelRB.AddTorque(Mathf.Sign(frontWheelRB.angularVelocity) * -brakeTorque * Time.fixedDeltaTime, ForceMode2D.Force);
            backWheelRB.AddTorque(Mathf.Sign(backWheelRB.angularVelocity) * -brakeTorque * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    private void HandleTireDust()
    {
        // Check slip conditions
        bool frontSlip = isGroundedFront && Mathf.Abs(frontWheelRB.angularVelocity) > slipThreshold;
        bool backSlip = isGroundedBack && Mathf.Abs(backWheelRB.angularVelocity) > slipThreshold;

        if (frontSlip)
        {
            if (!frontTireDust.isPlaying) frontTireDust.Play();
        }
        else
        {
            if (frontTireDust.isPlaying) frontTireDust.Stop();
        }

        if (backSlip)
        {
            if (!backTireDust.isPlaying) backTireDust.Play();
        }
        else
        {
            if (backTireDust.isPlaying) backTireDust.Stop();
        }
    }

    private void ApplySuspension(WheelJoint2D joint)
    {
        JointSuspension2D susp = joint.suspension;
        susp.frequency = suspensionFrequency;
        susp.dampingRatio = suspensionDamping;
        joint.suspension = susp;
    }

    // Optional public upgrade methods:
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
}
