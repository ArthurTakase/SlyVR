using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject breakZone;
    public Transform xrCamera;
    [SerializeField] private bool attachToWaist = true;
    [SerializeField] private Vector3 waistOffset = new(0.2f, -0.2f, 0);
    [SerializeField] private Vector3 rotationOffset = new(0, -0.5f, 0);

    public void Awake()
    {
        ToggleBreakZone(false);
    }

    void Update()
    {
        if (xrCamera == null || !attachToWaist) return;

        // Position
        Vector3 hmdPosition = xrCamera.position;
        Vector3 weaponPosition = hmdPosition + xrCamera.right * waistOffset.x
                                              + Vector3.up * waistOffset.y
                                              + xrCamera.forward * waistOffset.z;
        transform.position = weaponPosition;

        // Rotation
        Vector3 forwardDirection = xrCamera.forward;
        forwardDirection.y = 0;
        forwardDirection.Normalize();
        Quaternion baseRotation = Quaternion.LookRotation(forwardDirection);
        Quaternion offsetRotation = Quaternion.Euler(rotationOffset);
        transform.rotation = baseRotation * offsetRotation;
    }

    public void ToggleBreakZone(bool isActive)
    {
        if (breakZone == null) return;
        breakZone.SetActive(isActive);
    }

    public void SetAttachToWaist(bool value)
    {
        attachToWaist = value;
    }
}