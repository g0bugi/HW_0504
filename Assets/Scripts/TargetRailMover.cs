using UnityEngine;

public class TargetRailMover : MonoBehaviour
{

    [Header("드론 원형 이동 설정")]
    [SerializeField]
    private float rotationSpeed = 30.0f;

    [SerializeField]
    private bool rotateClockwise = true;

    private void Update()
    {

        float direction = rotateClockwise ? 1.0f : -1.0f;

        transform.Rotate(Vector3.up, rotationSpeed * direction * Time.deltaTime, Space.Self);

    }

}