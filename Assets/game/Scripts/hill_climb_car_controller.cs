using System.Linq;
using UnityEngine;

public class WheelCarController2D : MonoBehaviour
{
    [Header("References")]
    private WheelJoint2D frontWheel;
    private WheelJoint2D backWheel;

    [Header("Settings")]
    public float motorSpeed = -1000f; // negative = forward (depends on orientation)
    public float brakeSpeed = 500f;   // reverse / braking motor speed
    public float motorTorque = 2000f; // how powerful the motor is

    [Header("Keyboard Controls")]
    public KeyCode gasKey = KeyCode.W;
    public KeyCode brakeKey = KeyCode.S;

    // Mobile UI flags
    private bool isGasPressed = false;
    private bool isBrakePressed = false;

    private JointMotor2D frontMotor;
    private JointMotor2D backMotor;

    void Start()
    {
        // Initialize motors from wheels

        // carBody.centerOfMass = new Vector2(0f, 0.5f); // raises CG to encourage flipping
        var WheelJoints = GetComponentsInChildren<WheelJoint2D>().ToList();
        frontWheel = WheelJoints[0];
        backWheel = WheelJoints[1];
        frontMotor = frontWheel.motor;
        backMotor = backWheel.motor;
    }

    void FixedUpdate()
    {
        HandleKeyboardInput();
        ApplyWheelMotor();
    }

    void HandleKeyboardInput()
    {
        // Gas pedal
        if (Input.GetKey(gasKey))
            isGasPressed = true;
        else
            isGasPressed = false;

        // Brake pedal
        if (Input.GetKey(brakeKey))
            isBrakePressed = true;
        else
            isBrakePressed = false;
    }

    void ApplyWheelMotor()
    {

        frontWheel.useMotor = isGasPressed || isBrakePressed;
        backWheel.useMotor = isGasPressed || isBrakePressed;
        if (isGasPressed)
        {
            // Forward movement
            frontMotor.motorSpeed = motorSpeed;
            backMotor.motorSpeed = motorSpeed;

            frontMotor.maxMotorTorque = motorTorque;
            backMotor.maxMotorTorque = motorTorque;

            frontWheel.motor = frontMotor;
            backWheel.motor = backMotor;
        }
        else if (isBrakePressed)
        {
            // Reverse or brake movement
            frontMotor.motorSpeed = brakeSpeed;
            backMotor.motorSpeed = brakeSpeed;

            frontMotor.maxMotorTorque = motorTorque;
            backMotor.maxMotorTorque = motorTorque;

            frontWheel.motor = frontMotor;
            backWheel.motor = backMotor;


        }
        else
        {
            // Release motors (coast)
            frontWheel.useMotor = false;
            backWheel.useMotor = false;
        }
    }

    // --- Mobile UI Hooks ---
    public void GasDown() => isGasPressed = true;
    public void GasUp() => isGasPressed = false;

    public void BrakeDown() => isBrakePressed = true;
    public void BrakeUp() => isBrakePressed = false;
}
