using System;
using UnityEngine;

public class TargetDrone : MonoBehaviour
{

    [Header("조준 기준점")]
    [SerializeField]
    private Transform aimPoint;

    private ObjectPool ownerPool;

    private Transform chargeTarget;

    private float moveSpeed = 3.0f;

    private float reachDistance = 0.5f;

    private GameObject explosionPrefab;

    private float explosionLifeTime = 2.0f;

    private bool isAlive = false;

    public event Action<TargetDrone> ReturnedToPool;

    public bool IsActive
    {
        get
        {

            return gameObject.activeInHierarchy == true && isAlive == true;

        }
    }

    public Transform AimPoint
    {
        get
        {

            if (aimPoint != null)
            {

                return aimPoint;

            }

            return transform;

        }
    }

    public void Initialize(
        ObjectPool pool,
        Transform target,
        float speed,
        float targetReachDistance,
        GameObject hitExplosionPrefab,
        float hitExplosionLifeTime
    )
    {

        ownerPool = pool;
        chargeTarget = target;
        moveSpeed = speed;
        reachDistance = targetReachDistance;
        explosionPrefab = hitExplosionPrefab;
        explosionLifeTime = hitExplosionLifeTime;

        isAlive = true;

        LookAtChargeTarget();

    }

    private void Update()
    {

        if (isAlive == false)
        {

            return;

        }

        if (chargeTarget == null)
        {

            return;

        }

        MoveToChargeTarget();

    }

    private void MoveToChargeTarget()
    {

        Vector3 direction = chargeTarget.position - transform.position;

        if (direction.sqrMagnitude <= reachDistance * reachDistance)
        {

            ReturnToPool();
            return;

        }

        Vector3 moveDirection = direction.normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveDirection.sqrMagnitude > 0.0001f)
        {

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                360.0f * Time.deltaTime
            );

        }

    }

    private void LookAtChargeTarget()
    {

        if (chargeTarget == null)
        {

            return;

        }

        Vector3 direction = chargeTarget.position - transform.position;

        if (direction.sqrMagnitude <= 0.0001f)
        {

            return;

        }

        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

    }

    public void TakeHit()
    {

        if (isAlive == false)
        {

            return;

        }

        isAlive = false;

        CreateExplosion();

        ReturnToPool();

    }

    private void CreateExplosion()
    {

        if (explosionPrefab == null)
        {

            return;

        }

        GameObject explosionObject = Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );

        ParticleSystem particleSystem = explosionObject.GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
        {

            particleSystem.Play();

        }

        Destroy(explosionObject, explosionLifeTime);

    }

    private void ReturnToPool()
    {

        isAlive = false;

        ReturnedToPool?.Invoke(this);

        if (ownerPool != null)
        {

            ownerPool.ReturnObject(gameObject);

        }
        else
        {

            gameObject.SetActive(false);

        }

    }

}