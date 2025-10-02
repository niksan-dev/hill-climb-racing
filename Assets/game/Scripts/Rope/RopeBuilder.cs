using UnityEngine;
using System.Collections.Generic;

public class RopeBuilder : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] Transform ropeParent;
    public GameObject ropeSegmentPrefab;
    public int segmentCount = 10;
    public Transform attachPoint; // back of car
    public GameObject endObjectPrefab; // flag, fuel, etc.
                                       // public LineRenderer lineRenderer;

    private List<Transform> ropeSegments = new List<Transform>();

    void Start()
    {
        // if (lineRenderer == null)
        //     lineRenderer = GetComponent<LineRenderer>();

        Rigidbody2D prevBody = attachPoint.GetComponent<Rigidbody2D>();

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject newSeg = Instantiate(
                ropeSegmentPrefab,
                attachPoint.position + new Vector3(0, i * 0.22f, 0),

                Quaternion.identity,
                ropeParent
            );

            Rigidbody2D rb = newSeg.GetComponent<Rigidbody2D>();
            HingeJoint2D joint = newSeg.GetComponent<HingeJoint2D>();

            joint.connectedBody = prevBody;
            prevBody = rb;

            ropeSegments.Add(newSeg.transform);
        }

        // Optional: attach end object
        if (endObjectPrefab != null)
        {
            GameObject endObj = Instantiate(endObjectPrefab, prevBody.position - Vector2.up * 0.2f, Quaternion.identity);
            Rigidbody2D endRb = endObj.GetComponent<Rigidbody2D>();
            HingeJoint2D endJoint = endObj.AddComponent<HingeJoint2D>();
            endJoint.connectedBody = prevBody;

            ropeSegments.Add(endObj.transform);
        }
    }

    void LateUpdate()
    {
        // if (lineRenderer == null || ropeSegments.Count == 0) return;

        // lineRenderer.positionCount = ropeSegments.Count;

        // for (int i = 0; i < ropeSegments.Count; i++)
        // {
        //     lineRenderer.SetPosition(i, ropeSegments[i].position);
        // }
    }
}
