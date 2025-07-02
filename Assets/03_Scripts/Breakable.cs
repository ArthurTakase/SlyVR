using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Breakable : MonoBehaviour
{
    public List<GameObject> breakParticles;
    public Audio breakSound;
    public XRDirectInteractor leftController;
    public XRDirectInteractor rightController;
    public int hitToBreak = 1;
    private int hitCount = 0;
    public bool knockbackOnHit = true;
    public float knockbackDistance = 0.2f;
    public float knockbackDuration = 0.1f;
    private Coroutine knockbackCoroutine;
    public bool shakeOnHit = false;
    public GameObject shakedObject;
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.1f;
    public float shakeSpeed = 1f;
    public Coroutine shakeCoroutine;
    public UnityEngine.Events.UnityEvent onHitEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Weapon")) return;

        hitCount++;
        Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position);
        HitEffect(collisionPoint);
        if (hitCount >= hitToBreak) Break(collisionPoint);
    }

    private void HitEffect(Vector3 hitPosition)
    {
        if (leftController != null) leftController.SendHapticImpulse(0.8f, 0.5f);
        if (rightController != null) rightController.SendHapticImpulse(0.8f, 0.5f);

        if (shakeOnHit) Shake();
        if (knockbackOnHit) Knockback();

        foreach (GameObject particles in breakParticles)
        {
            var particlesInstant = Instantiate(particles, hitPosition, Quaternion.identity);
            particlesInstant.SetActive(true);
            Destroy(particlesInstant, 2);
        }

        if (breakSound.clip != null)
        {
            AudioSource.PlayClipAtPoint(breakSound.clip, hitPosition, breakSound.volume);
        }

        onHitEvents?.Invoke();
    }

    public void Break(Vector3 hitPosition)
    {
        HitEffect(hitPosition);

        // disable collider and renderer
        if (TryGetComponent<Collider>(out var collider)) collider.enabled = false;
        if (TryGetComponent<Renderer>(out var renderer)) renderer.enabled = false;

        if (gameObject.TryGetComponent<Rigidbody>(out var existingRigidbody))
        {
            // If a Rigidbody already exists, just enable it
            existingRigidbody.isKinematic = false;
            existingRigidbody.useGravity = true;
        }
        else
        {
            var rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }

        // destroy after a delay
        Destroy(gameObject);
    }

    public void Knockback()
    {
        if (!knockbackOnHit) return;
        if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
        knockbackCoroutine = StartCoroutine(KnockbackCoroutine());
    }

    private IEnumerator KnockbackCoroutine()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition - transform.forward * knockbackDistance;

        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    public void Shake()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        if (shakedObject == null) yield break;

        Vector3 originalLocalPos = shakedObject.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) * 2f - 1f;
            float y = Mathf.PerlinNoise(0f, Time.time * shakeSpeed) * 2f - 1f;
            Vector3 offset = new Vector3(x, y, 0f) * shakeMagnitude;

            shakedObject.transform.localPosition = originalLocalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakedObject.transform.localPosition = originalLocalPos;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Breakable))]
public class BreakableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Breakable ennemy = (Breakable)target;

        if (GUILayout.Button("Die"))
        {
            ennemy.Break(ennemy.transform.position);
        }
    }
}
#endif