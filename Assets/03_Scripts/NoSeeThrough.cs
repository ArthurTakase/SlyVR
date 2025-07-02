using UnityEngine;
using System;

public class NoSeeThrough : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float sphereCheckSize = 0.15f;
    private Material cameraFadeMat;
    private bool isCameraFadeOut = false;

    private void Awake()
    {
        cameraFadeMat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (Physics.CheckSphere(transform.position, sphereCheckSize, collisionLayer, QueryTriggerInteraction.Ignore))
        {
            CameraFade(1f);
            isCameraFadeOut = true;
        }
        else if (isCameraFadeOut)
            CameraFade(0f);
    }

    public void CameraFade(float targetAlpha)
    {
        var alphaValue = cameraFadeMat.GetFloat("_AlphaValue");
        var fadeValue = Mathf.MoveTowards(alphaValue, targetAlpha, fadeSpeed * Time.deltaTime);
        cameraFadeMat.SetFloat("_AlphaValue", fadeValue);

        if (fadeValue <= 0.01) isCameraFadeOut = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereCheckSize);
    }
}