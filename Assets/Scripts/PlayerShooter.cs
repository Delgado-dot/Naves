using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private InputActionAsset inputActions;

    [Header("Fire Rate")]
    [SerializeField] private float fireCooldown = 0.15f;

    [Header("Recoil")]
    [SerializeField] private float recoilDistance = 0.15f;
    [SerializeField] private float recoilReturnSpeed = 8f;

    private InputAction attackAction;
    private bool useLeft = true;
    private float lastFireTime;

    private Vector3 leftOrigin;
    private Vector3 rightOrigin;
    private float leftRecoil;
    private float rightRecoil;

    private void Awake()
    {
        if (firePointLeft == null)
            firePointLeft = transform.Find("FirePointLeft");

        if (firePointRight == null)
            firePointRight = transform.Find("FirePointRight");

        attackAction = InputHelper.GetPlayerAction(inputActions, "Attack");
    }

    private void Start()
    {
        if (firePointLeft != null)
            leftOrigin = firePointLeft.localPosition;

        if (firePointRight != null)
            rightOrigin = firePointRight.localPosition;
    }

    private void OnEnable()
    {
        if (attackAction != null)
        {
            attackAction.performed += OnAttackPerformed;
            attackAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.performed -= OnAttackPerformed;
            attackAction.Disable();
        }
    }

    private void Update()
    {
        ApplyRecoilReturn();
    }

    private void OnAttackPerformed(InputAction.CallbackContext _)
    {
        if (Time.time < lastFireTime + fireCooldown)
            return;

        Fire();

        lastFireTime = Time.time;
    }

    private void Fire()
    {
        if (projectilePrefab == null)
            return;

        Transform firePoint = useLeft ? firePointLeft : firePointRight;

        if (firePoint == null)
            return;

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        if (useLeft)
            leftRecoil = -recoilDistance;
        else
            rightRecoil = -recoilDistance;

        useLeft = !useLeft;
    }

    private void ApplyRecoilReturn()
    {
        if (firePointLeft != null)
        {
            leftRecoil = Mathf.Lerp(leftRecoil, 0f, recoilReturnSpeed * Time.deltaTime);
            Vector3 pos = leftOrigin;
            pos.z += leftRecoil;
            firePointLeft.localPosition = pos;
        }

        if (firePointRight != null)
        {
            rightRecoil = Mathf.Lerp(rightRecoil, 0f, recoilReturnSpeed * Time.deltaTime);
            Vector3 pos = rightOrigin;
            pos.z += rightRecoil;
            firePointRight.localPosition = pos;
        }
    }
}
