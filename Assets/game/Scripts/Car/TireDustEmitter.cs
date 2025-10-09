using UnityEngine;

[RequireComponent(typeof(WheelJoint2D))]
public class TireDustEmitter : MonoBehaviour
{

    [Header("References")]
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float wheelRadius = 0.26f;
    [SerializeField] private float slipFactor = 1.2f;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private Rigidbody2D carBody;

    [Header("Settings")]
    [SerializeField] private float slipThreshold = 1.5f;
    [SerializeField] private float baseEmission = 10f;
    [SerializeField] private float maxEmission = 50f;

    private ParticleSystem.EmissionModule emission;
    private Rigidbody2D wheelRb;

    private readonly Vector3 tireSizeOffset = new Vector3(0.05f, -0.266f, 0);

    private bool isGrounded;

    private void Awake()
    {
        if (!dustParticles) dustParticles = GetComponentInChildren<ParticleSystem>();
        wheelRb = GetComponent<Rigidbody2D>();
        emission = dustParticles.emission;
    }

    void Update()
    {
        isGrounded = wheelRb.IsTouchingLayers(groundLayer);

        HandleTireDust();
    }

    #region Tire Dust

    private void HandleTireDust()
    {
        float carSpeed = carBody.linearVelocity.x;
        bool tireSlip = IsWheelSlipping(wheelRb, isGrounded, carSpeed);
        Debug.Log($"tireSlip : {tireSlip}");
        SetParticleState(dustParticles, tireSlip);

        if (tireSlip)
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
        dustParticles.transform.position = wheelRb.transform.position + tireSizeOffset;
    }
    #endregion
}
