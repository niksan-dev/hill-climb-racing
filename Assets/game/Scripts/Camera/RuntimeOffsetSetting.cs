using Unity.Cinemachine;
using UnityEngine;

public class RuntimeOffsetSertting : MonoBehaviour
{
    [SerializeField]
    CinemachineCamera cinemachineCamera;

    [SerializeField] private Vector2 defaultOffeset = new Vector2(6.5f, 1);
    private Vector3 offset = new Vector3(3f, 1, -10);


    private Vector2 referenceScreen = new Vector2(1920, 1080);

    void Awake()
    {
        float aspect = (float)Screen.width / Screen.height;

        float xOffset = defaultOffeset.x * (aspect / 1.77f);

        offset.x = xOffset;

        cinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = offset;

    }
}
