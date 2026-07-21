using UnityEngine;

/// <summary>Third-person camera that follows the target with smooth position and rotation.</summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private float followDistance = 6f;
    [SerializeField] private float followHeight = 3f;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 8f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position
            - target.forward * followDistance
            + Vector3.up * followHeight;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}