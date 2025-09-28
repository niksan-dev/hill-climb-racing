using System.Linq;
using UnityEngine;


public class CarController : MonoBehaviour
{

    [Header("Tilt Settings")]
    public float tiltTorque = 150f;              // torque for mid-air rotation

    [Header("Ground Check")]
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isBraking;

    [Header("Car References")]
    [SerializeField] private Rigidbody2D carBody;
    [SerializeField] private Rigidbody2D frontWheelRB;
    [SerializeField] private Rigidbody2D backWheelRB;

    [Header("Motor Settings")]         // how strongly wheels slow down when braking
    [SerializeField] private float maxCarAngularVelocity = 200f;   // clamp car body rotation
    [SerializeField] float _speed = 150f;
    [SerializeField] float rotationSpeed = 300;
    private float _moveInput;



    [Header("Engine Settings")]
    public float baseMotorTorque = 400f;      // baseline torque
    public float maxMotorSpeed = 1500f;       // deg/sec target speed

    [Header("Suspension Settings")]
    [Range(0f, 30f)] public float suspensionFrequency = 4.5f;
    [Range(0f, 1f)] public float suspensionDamping = 0.7f;

    [Header("Tire Grip")]
    public PhysicsMaterial2D tireMaterial;    // assign in inspector
    [Range(0f, 5f)] public float gripMultiplier = 1f;

    [Header("Air Control")]
    public float airTiltTorque = 150f;

    [Header("Runtime Multipliers (Upgrades)")]
    public float engineUpgradeMultiplier = 1f;
    public float suspensionUpgradeMultiplier = 1f;
    public float gripUpgradeMultiplier = 1f;

    [SerializeField] private float brakeTorque = 300f;
    [SerializeField] private WheelJoint2D frontWheelJoint;
    [SerializeField] private WheelJoint2D backWheelJoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        carBody.centerOfMass = new Vector2(0f, 0.5f); // raises CG to encourage flipping
        var WheelJoints = GetComponentsInChildren<WheelJoint2D>().ToList();
        frontWheelJoint = WheelJoints[0];
        backWheelJoint = WheelJoints[1];

        Debug.Log("frontWheelJoint: " + frontWheelJoint + "   backWheelJoint: " + backWheelJoint);


        Debug.Log("frontWheelJoint rigidbody: " + frontWheelJoint.connectedBody + "   backWheelJoint rigidbody: " + backWheelJoint.connectedBody);
    }

    // Update is called once per frame
    void Update()
    {

        _moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = frontWheelRB.IsTouchingLayers(groundLayer) || backWheelRB.IsTouchingLayers(groundLayer);
        ApplyBrake();
        // Debug.Log("isGrounded: " + isGrounded + " isBraking: " + isBraking);
    }



    void FixedUpdate()
    {
        CheckForGround();
        // HandleMovement();
        HandleMotor();
        HandleTilt();
        ApplyBraking();
        // LimitAngularVelocity();
    }


    void HandleMovement()
    {
        if (!isGrounded && isBraking) return;
        frontWheelRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime);
        backWheelRB.AddTorque(-_moveInput * _speed * Time.fixedDeltaTime);
        carBody.AddTorque(_moveInput * rotationSpeed * Time.fixedDeltaTime);
    }


    private void HandleMotor()
    {
        if (isBraking) return;
        float targetSpeed = -_moveInput * maxMotorSpeed; // negative to match rotation direction
        float motorTorque = baseMotorTorque * engineUpgradeMultiplier;

        if (frontWheelJoint != null)
        {
            JointMotor2D motor = frontWheelJoint.motor;
            motor.motorSpeed = targetSpeed;
            motor.maxMotorTorque = motorTorque;
            frontWheelJoint.motor = motor;
            frontWheelJoint.useMotor = Mathf.Abs(_moveInput) > 0.01f;
        }

        if (backWheelJoint != null)
        {
            JointMotor2D motor = backWheelJoint.motor;
            motor.motorSpeed = targetSpeed;
            motor.maxMotorTorque = motorTorque;
            backWheelJoint.motor = motor;
            backWheelJoint.useMotor = Mathf.Abs(_moveInput) > 0.01f;
        }

        carBody.AddTorque(_moveInput * rotationSpeed * Time.fixedDeltaTime);
    }


    void ApplyBrake()
    {
        // True if player is pressing space, regardless of ground state
        isBraking = Input.GetKey(KeyCode.Space);
    }

    void CheckForGround()
    {
        isGrounded = frontWheelRB.IsTouchingLayers(groundLayer) || backWheelRB.IsTouchingLayers(groundLayer);
    }

    void ApplyBraking()
    {
        if (isBraking)
        {
            // Apply opposing torque to both wheels
            //frontWheelRB.AddTorque(Mathf.Sign(frontWheelRB.angularVelocity) * -brakeTorque * Time.fixedDeltaTime, ForceMode2D.Force);
            // backWheelRB.AddTorque(Mathf.Sign(backWheelRB.angularVelocity) * -brakeTorque * Time.fixedDeltaTime, ForceMode2D.Force);
            carBody.AddTorque(-brakeTorque * Time.fixedDeltaTime);

        }
    }

    void HandleTilt()
    {
        if (!isGrounded)
        {

            carBody.AddTorque(_moveInput * tiltTorque * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }

    void LimitAngularVelocity()
    {
        frontWheelRB.angularVelocity = Mathf.Clamp(frontWheelRB.angularVelocity, -maxCarAngularVelocity, maxCarAngularVelocity);
        backWheelRB.angularVelocity = Mathf.Clamp(backWheelRB.angularVelocity, -maxCarAngularVelocity, maxCarAngularVelocity);
        // carBody.angularVelocity = Mathf.Clamp(carBody.angularVelocity, -maxCarAngularVelocity, maxCarAngularVelocity);
    }
}
