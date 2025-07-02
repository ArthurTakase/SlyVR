using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}