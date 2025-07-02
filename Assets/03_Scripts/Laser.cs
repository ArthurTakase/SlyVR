using UnityEngine;

public class Laser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LaserManager.Instance.LaserTriggered();
        }
    }
}