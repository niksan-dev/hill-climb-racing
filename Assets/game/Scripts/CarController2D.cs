using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HillClimbCarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D carBody;
    [SerializeField] private Rigidbody2D frontWheelRB;
    [SerializeField] private Rigidbody2D backWheelRB;
    [SerializeField] private WheelJoint2D frontWheelJoint;
    [SerializeField] private WheelJoint2D backWheelJoint;

    [Header("Engine Settings")]
    [SerializeField] private float accelerationTorque = 250f;
    [SerializeField] private float brakeTorque = 400f;
    [SerializeField] private float maxAngularVelocity = 3200f;

    [Header("Tilt Settings")]
    [SerializeField] private float airTiltTorque = 220f;

    [Header("Suspension Settings")]
    [SerializeField] private float suspensionFrequency = 2.2f; // softer
    [SerializeField] private float suspensionDamping = 0.45f;  // bouncier

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private float inputHorizontal;
    private bool isBraking;

    void Awake()
    {
        CacheWheelJoints();
        SetupSuspension(frontWheelJoint);
        SetupSuspension(backWheelJoint);
    }

    private void CacheWheelJoints()
    {
        var joints = GetComponentsInChildren<WheelJoint2D>().ToList();
        frontWheelJoint = joints[0];
        backWheelJoint = joints[1];


        Debug.Log("Front Wheel Joint: " + frontWheelJoint.connectedBody.name);
        Debug.Log("Back Wheel Joint: " + backWheelJoint.connectedBody.name);
    }

    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        isBraking = Input.GetKey(KeyCode.Space) ? true : false;
    }

    void FixedUpdate()
    {
        HandleDrive();
        HandleTilt();
        ApplyDrag();
    }

    private void HandleDrive()
    {
        if (isBraking)
        {
            ApplyBrake();
            return;
        }

        float torque = -inputHorizontal * accelerationTorque * Time.fixedDeltaTime;
        //Debug.Log($"torque : {torque}, inputHorizontal : {inputHorizontal} Time.fixedDeltaTime : {Time.fixedDeltaTime}");
        // Always apply torque to both wheels for a soft, playful feel
        frontWheelRB.AddTorque(torque, ForceMode2D.Force);
        backWheelRB.AddTorque(torque, ForceMode2D.Force);

        // If only back wheel is grounded and front is in air, add extra torque to car body for fast tilt/flip
        if (IsGrounded(backWheelRB) && !IsGrounded(frontWheelRB) && Mathf.Abs(inputHorizontal) > 0.01f)
        {
            float flipTorque = inputHorizontal * accelerationTorque * Time.fixedDeltaTime; // 1.5f is a tweak factor
            carBody.AddTorque(flipTorque, ForceMode2D.Force);
        }

        ClampWheelAngularVelocity();
    }


    private void ApplyBrake()
    {
        if (!isBraking) return;
        bool inAir = !IsGrounded(frontWheelRB) && !IsGrounded(backWheelRB);
        // Debug.Log("========BRAKE========");
        if (inAir || !IsGrounded(frontWheelRB))
        {
            // ApplyTorqueToWheels(brakeTorque);
            float flipTorque = brakeTorque * Time.fixedDeltaTime; // 1.5f is a tweak factor
            carBody.AddTorque(-flipTorque, ForceMode2D.Force);
            Debug.Log("=====AIR BRAKE=====");
        }
        else
        {
            float torque = brakeTorque * Time.fixedDeltaTime;
            Debug.Log($"torque : {torque}, inputHorizontal : {inputHorizontal} Time.fixedDeltaTime : {Time.fixedDeltaTime}");
            // Always apply torque to both wheels for a soft, playful feel
            // frontWheelRB.AddTorque(torque, ForceMode2D.Force);
            backWheelRB.AddTorque(torque, ForceMode2D.Force);
            Debug.Log("=====GROUND BRAKE=====");

        }
        ClampWheelAngularVelocity();

    }

    private void HandleTilt()
    {

        // return;
        bool inAir = !IsGrounded(frontWheelRB) && !IsGrounded(backWheelRB);

        if (inAir)
        {
            // Apply reverse tilt when braking in air
            if (!isBraking)
            {

                Debug.Log("=====AIR TILT=====");
                float brakeTilt = inputHorizontal * airTiltTorque * Time.fixedDeltaTime;
                carBody.AddTorque(brakeTilt, ForceMode2D.Force);
            }
        }
    }

    private void ClampWheelAngularVelocity()
    {
        frontWheelRB.angularVelocity = Mathf.Clamp(frontWheelRB.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        backWheelRB.angularVelocity = Mathf.Clamp(backWheelRB.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
    }

    private bool IsGrounded(Rigidbody2D wheelRB)
    {
        return wheelRB.IsTouchingLayers(groundLayer);
    }

    private void SetupSuspension(WheelJoint2D joint)
    {
        SetSuspension(joint, suspensionFrequency, suspensionDamping);
    }


    public void SetSuspension(WheelJoint2D wheel, float frequency, float damping)
    {
        JointSuspension2D sus = wheel.suspension;
        sus.frequency = frequency;
        sus.dampingRatio = damping;
        wheel.suspension = sus; // must reassign!
    }

    private void ApplyDrag()
    {
        // Optional: add a little drag for stability, tweak as needed
        carBody.linearDamping = IsGrounded(frontWheelRB) || IsGrounded(backWheelRB) ? 1.2f : 0.2f;
        carBody.angularDamping = 1.5f;
    }
}