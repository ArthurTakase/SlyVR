using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LaserManager : MonoBehaviour
{
    [SerializeField] private GameObject warningLightGO;
    private Light warningLight;
    private readonly List<GameObject> lasers = new();
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float warningSoundDuration = 5f;
    [SerializeField] private AudioSource audioSourceWarning;
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private AudioClip warningMusic;
    public static LaserManager Instance { get; private set; }
    [SerializeField] private List<GameObject> ennemies;
    bool warningTriggered = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (!warningTriggered) return;

        // if ennemies.Count == 0 or all ennemies are Null, deactivate warning light
        if (ennemies.Count == 0 || ennemies.TrueForAll(e => e == null))
        {
            warningLightGO.SetActive(false);
            warningTriggered = false;
            audioSourceMusic.clip = defaultMusic;
            audioSourceMusic.Play();
            StopWarningSound();
            return;
        }
    }

    private void Start()
    {
        if (warningLightGO.TryGetComponent(out Light lightComponent))
            warningLight = lightComponent;
        else
            Debug.LogError("warningLightObject does not have a Light component!");

        warningLight.intensity = 0;

        foreach (Transform child in transform)
            lasers.Add(child.gameObject);
    }

    public void LaserDeactivated()
    {
        foreach (var laser in lasers)
            laser.SetActive(false);
    }

    private void StopWarningSound()
    {
        audioSourceWarning.Stop();
    }

    public void LaserTriggered()
    {
        // loop warning sound
        audioSourceWarning.Play();
        Invoke(nameof(StopWarningSound), warningSoundDuration);

        // change music to warning music
        audioSourceMusic.clip = warningMusic;
        audioSourceMusic.Play();

        warningTriggered = true;

        // enable warning light
        StartCoroutine(WarningCoroutine());
    }

    private IEnumerator WarningCoroutine()
    {
        warningLightGO.SetActive(true);

        var enemies = Object.FindObjectsByType<Ennemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            if (enemy != null) enemy.ChasePlayerNow();
            yield return null;
        }

        // disable all lasers
        foreach (var laser in lasers)
        {
            laser.SetActive(false);
            yield return null;
        }

        while (true)
        {
            // Fade In
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                warningLight.intensity = Mathf.Lerp(0.1f, maxIntensity, t / fadeDuration);
                yield return null;
            }

            warningLight.intensity = maxIntensity;

            // Fade Out
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                warningLight.intensity = Mathf.Lerp(maxIntensity, 0.1f, t / fadeDuration);
                yield return null;
            }

            warningLight.intensity = 0.1f;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LaserManager))]
public class LaserManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LaserManager ennemy = (LaserManager)target;

        if (GUILayout.Button("Launch alarm"))
        {
            ennemy.LaserTriggered();
        }
    }
}
#endif