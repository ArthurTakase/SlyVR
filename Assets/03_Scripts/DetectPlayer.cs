using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent onTriggerEnter;
    public UnityEngine.Events.UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerExit.Invoke();
        }
    }
}