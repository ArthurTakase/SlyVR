using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public new Camera camera;

    public void Update()
    {
        if (camera == null) return;

        Vector3 cameraPosition = camera.transform.position;
        this.gameObject.transform.LookAt(cameraPosition);
        this.gameObject.transform.Rotate(0, 180, 0);
    }
}