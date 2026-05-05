using UnityEngine;

public class ProjectileMover : MonoBehaviour
{

    [Header("이동 설정")]
    [SerializeField]
    private Vector3 moveDirection = Vector3.forward;

    [SerializeField]
    private float moveSpeed = 12.0f;

    [SerializeField]
    private float lifeTime = 3.0f;

    private float elapsedTime = 0.0f;

    private bool isInitialized = false;

    public void Initialize(Vector3 direction, float speed, float destroyTime)
    {

        if (direction.sqrMagnitude <= 0.0001f)
        {

            moveDirection = transform.forward;

        }
        else
        {

            moveDirection = direction.normalized;

        }

        moveSpeed = speed;
        lifeTime = destroyTime;

        elapsedTime = 0.0f;
        isInitialized = true;

        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);

    }

    private void Start()
    {

        if (isInitialized == false)
        {

            moveDirection = transform.forward.normalized;

        }

    }

    private void Update()
    {

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifeTime)
        {

            Destroy(gameObject);

        }

    }

}