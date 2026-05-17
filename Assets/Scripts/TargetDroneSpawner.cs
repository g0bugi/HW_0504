using UnityEngine;

public class TargetDroneSpawner : MonoBehaviour
{

    [Header("풀")]
    [SerializeField]
    private ObjectPool targetPool;

    [Header("생성 위치")]
    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private float spawnRadius = 10.0f;

    [SerializeField]
    private float spawnHeight = 2.0f;

    [Header("돌진 목표")]
    [SerializeField]
    private Transform chargeTarget;

    [Header("타겟 설정")]
    [SerializeField]
    private float targetMoveSpeed = 3.0f;

    [SerializeField]
    private float reachDistance = 0.5f;

    [Header("생성 설정")]
    [SerializeField]
    private float firstSpawnDelay = 1.0f;

    [SerializeField]
    private float spawnInterval = 2.0f;

    [SerializeField]
    private bool spawnOnlyOneTarget = true;

    [Header("폭발 효과")]
    [SerializeField]
    private GameObject explosionPrefab;

    [SerializeField]
    private float explosionLifeTime = 2.0f;

    private float nextSpawnTime = 0.0f;

    private TargetDrone currentTarget;

    public TargetDrone CurrentTarget
    {
        get
        {

            if (currentTarget == null)
            {

                return null;

            }

            if (currentTarget.IsActive == false)
            {

                return null;

            }

            return currentTarget;

        }
    }

    private void Start()
    {

        nextSpawnTime = Time.time + firstSpawnDelay;

    }

    private void Update()
    {

        if (targetPool == null)
        {

            return;

        }

        if (chargeTarget == null)
        {

            return;

        }

        if (spawnOnlyOneTarget == true && CurrentTarget != null)
        {

            return;

        }

        if (Time.time < nextSpawnTime)
        {

            return;

        }

        SpawnTarget();

    }

    private void SpawnTarget()
    {

        Vector3 spawnPosition = GetSpawnPosition();

        Vector3 directionToTarget = chargeTarget.position - spawnPosition;

        if (directionToTarget.sqrMagnitude <= 0.0001f)
        {

            directionToTarget = Vector3.forward;

        }

        Quaternion spawnRotation = Quaternion.LookRotation(directionToTarget.normalized, Vector3.up);

        GameObject targetObject = targetPool.GetObject(spawnPosition, spawnRotation);

        TargetDrone targetDrone = targetObject.GetComponent<TargetDrone>();

        if (targetDrone == null)
        {

            targetDrone = targetObject.AddComponent<TargetDrone>();

        }

        targetDrone.ReturnedToPool -= HandleTargetReturnedToPool;
        targetDrone.ReturnedToPool += HandleTargetReturnedToPool;

        targetDrone.Initialize(
            targetPool,
            chargeTarget,
            targetMoveSpeed,
            reachDistance,
            explosionPrefab,
            explosionLifeTime
        );

        currentTarget = targetDrone;

    }

    private Vector3 GetSpawnPosition()
    {

        if (spawnPoints != null && spawnPoints.Length > 0)
        {

            int randomIndex = Random.Range(0, spawnPoints.Length);

            if (spawnPoints[randomIndex] != null)
            {

                return spawnPoints[randomIndex].position;

            }

        }

        Vector2 randomCircle = Random.insideUnitCircle;

        if (randomCircle.sqrMagnitude <= 0.0001f)
        {

            randomCircle = Vector2.right;

        }

        randomCircle.Normalize();

        Vector3 offset = new Vector3(
            randomCircle.x * spawnRadius,
            spawnHeight,
            randomCircle.y * spawnRadius
        );

        return chargeTarget.position + offset;

    }

    private void HandleTargetReturnedToPool(TargetDrone returnedTarget)
    {

        if (currentTarget == returnedTarget)
        {

            currentTarget = null;

        }

        returnedTarget.ReturnedToPool -= HandleTargetReturnedToPool;

        nextSpawnTime = Time.time + spawnInterval;

    }

    private void OnDrawGizmosSelected()
    {

        if (chargeTarget == null)
        {

            return;

        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(chargeTarget.position, spawnRadius);

    }

}