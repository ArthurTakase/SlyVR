using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float height = 0.5f;
    public float frequency = 1f;

    [SerializeField] private Transform parentPosition;
    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        float newY = startY + Mathf.Sin(Time.time * frequency) * height;
        transform.position = new Vector3(parentPosition.position.x, newY, parentPosition.position.z);
    }
}