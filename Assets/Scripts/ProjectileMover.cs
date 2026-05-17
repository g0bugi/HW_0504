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

    [Header("피격 판정")]
    [SerializeField]
    private float hitRadius = 0.15f;

    [SerializeField]
    private LayerMask hitLayerMask = ~0;

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

        if (TryHitOverlappedTarget() == true)
        {

            return;

        }

        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        if (TryHitMovingTarget(movement) == true)
        {

            return;

        }

        transform.position += movement;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifeTime)
        {

            Destroy(gameObject);

        }

    }

    private bool TryHitOverlappedTarget()
    {

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            hitRadius,
            hitLayerMask,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < colliders.Length; i++)
        {

            TargetDrone targetDrone = colliders[i].GetComponentInParent<TargetDrone>();

            if (targetDrone == null)
            {

                continue;

            }

            if (targetDrone.IsActive == false)
            {

                continue;

            }

            targetDrone.TakeHit();

            Destroy(gameObject);

            return true;

        }

        return false;

    }

    private bool TryHitMovingTarget(Vector3 movement)
    {

        float distance = movement.magnitude;

        if (distance <= 0.0001f)
        {

            return false;

        }

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            hitRadius,
            moveDirection,
            distance,
            hitLayerMask,
            QueryTriggerInteraction.Collide
        );

        TargetDrone hitTarget = null;
        RaycastHit selectedHit = new RaycastHit();
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {

            TargetDrone targetDrone = hits[i].collider.GetComponentInParent<TargetDrone>();

            if (targetDrone == null)
            {

                continue;

            }

            if (targetDrone.IsActive == false)
            {

                continue;

            }

            if (hits[i].distance < closestDistance)
            {

                closestDistance = hits[i].distance;
                selectedHit = hits[i];
                hitTarget = targetDrone;

            }

        }

        if (hitTarget == null)
        {

            return false;

        }

        transform.position = selectedHit.point;

        hitTarget.TakeHit();

        Destroy(gameObject);

        return true;

    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);

    }

}