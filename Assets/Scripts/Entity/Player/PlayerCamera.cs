using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    
    public Transform target;
    public bool isFixed;
    
    [SerializeField] private float followSpeed = 10;
    [SerializeField] private Vector3 cameraOffset;

    private Vector3 refVelocity = Vector3.zero;

    private void Start()
    {
        playerCamera = Camera.main;
        target = transform;
    }

    public void CalculateCameraPosition()
    {
        Vector3 targetPos = target.position + cameraOffset;
        Vector3 clampedPos = new Vector3(Mathf.Clamp(targetPos.x, float.NegativeInfinity, float.MaxValue), targetPos.y, targetPos.z);
        Vector3 smoothPos = Vector3.SmoothDamp(playerCamera.transform.position, clampedPos, ref refVelocity, followSpeed * Time.fixedDeltaTime);

        playerCamera.transform.position = isFixed ? targetPos : smoothPos;
    }
}