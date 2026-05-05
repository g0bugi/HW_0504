using UnityEngine;

public class TurretController : MonoBehaviour
{

    [Header("타겟")]
    [SerializeField]
    private Transform target;

    [Header("포탑 회전축")]
    [SerializeField]
    private Transform yawPivot;

    [SerializeField]
    private Transform pitchPivot;

    [SerializeField]
    private Transform muzzlePoint;

    [Header("회전 속도")]
    [SerializeField]
    private float yawRotationSpeed = 120.0f;

    [SerializeField]
    private float pitchRotationSpeed = 90.0f;

    [Header("Pitch 각도 제한")]
    [SerializeField]
    private float minPitch = -45.0f;

    [SerializeField]
    private float maxPitch = 20.0f;

    [Header("조준 판정")]
    [SerializeField]
    private float fireAngleThreshold = 5.0f;

    [Header("발사 설정")]
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private float fireInterval = 0.5f;

    [SerializeField]
    private float projectileSpeed = 12.0f;

    [SerializeField]
    private float projectileLifeTime = 3.0f;

    [Header("디버그")]
    [SerializeField]
    private bool drawDebugRay = true;

    private float nextFireTime = 0.0f;

    public bool IsAimed { get; private set; }

    public float CurrentAimAngle { get; private set; }

    private void Update()
    {

        if (IsReferenceMissing())
        {

            return;

        }

        RotateYaw();
        RotatePitch();

        IsAimed = CheckAim();

        if (IsAimed == true)
        {

            TryFire();

        }

    }

    private bool IsReferenceMissing()
    {

        if (target == null)
        {

            return true;

        }

        if (yawPivot == null)
        {

            return true;

        }

        if (pitchPivot == null)
        {

            return true;

        }

        if (muzzlePoint == null)
        {

            return true;

        }

        if (projectilePrefab == null)
        {

            return true;

        }

        return false;

    }

    private void RotateYaw()
    {

        Vector3 directionToTarget = target.position - yawPivot.position;

        directionToTarget.y = 0.0f;

        if (directionToTarget.sqrMagnitude <= 0.0001f)
        {

            return;

        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized, Vector3.up);

        yawPivot.rotation = Quaternion.RotateTowards(
            yawPivot.rotation,
            targetRotation,
            yawRotationSpeed * Time.deltaTime
        );

    }

    private void RotatePitch()
    {

        Vector3 worldDirectionToTarget = target.position - pitchPivot.position;

        Vector3 localDirectionToTarget = yawPivot.InverseTransformDirection(worldDirectionToTarget);

        if (localDirectionToTarget.sqrMagnitude <= 0.0001f)
        {

            return;

        }

        float targetPitchAngle = Mathf.Atan2(localDirectionToTarget.y, localDirectionToTarget.z) * Mathf.Rad2Deg;

        float pitchX = -targetPitchAngle;

        float clampedPitchX = Mathf.Clamp(pitchX, minPitch, maxPitch);

        Quaternion targetLocalRotation = Quaternion.Euler(clampedPitchX, 0.0f, 0.0f);

        pitchPivot.localRotation = Quaternion.RotateTowards(
            pitchPivot.localRotation,
            targetLocalRotation,
            pitchRotationSpeed * Time.deltaTime
        );

    }

    private bool CheckAim()
    {

        Vector3 directionToTarget = target.position - muzzlePoint.position;

        if (directionToTarget.sqrMagnitude <= 0.0001f)
        {

            CurrentAimAngle = 180.0f;
            return false;

        }

        CurrentAimAngle = Vector3.Angle(muzzlePoint.forward, directionToTarget.normalized);

        return CurrentAimAngle <= fireAngleThreshold;

    }

    private void TryFire()
    {

        if (Time.time < nextFireTime)
        {

            return;

        }

        Fire();

        nextFireTime = Time.time + fireInterval;

    }

    private void Fire()
    {

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            muzzlePoint.position,
            Quaternion.LookRotation(muzzlePoint.forward, Vector3.up)
        );

        ProjectileMover projectileMover = projectileObject.GetComponent<ProjectileMover>();

        if (projectileMover == null)
        {

            projectileMover = projectileObject.AddComponent<ProjectileMover>();

        }

        projectileMover.Initialize(
            muzzlePoint.forward,
            projectileSpeed,
            projectileLifeTime
        );

    }

    private void OnDrawGizmos()
    {

        if (drawDebugRay == false)
        {

            return;

        }

        if (muzzlePoint == null)
        {

            return;

        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(muzzlePoint.position, muzzlePoint.forward * 3.0f);

        if (target != null)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawLine(muzzlePoint.position, target.position);

        }

    }

}