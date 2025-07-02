using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Player : MonoBehaviour
{
    public float hitBeforeDiyng = 1f;
    [SerializeField] private float currentHitCount = 0f;
    [SerializeField] private float notHitDuration = 2f;
    private float lastHitTime = 0f;
    public List<Audio> hitSounds;
    private int lastHitSoundIndex = -1;
    public XRDirectInteractor leftController;
    public XRDirectInteractor rightController;
    public float regenHealthTime = 5f;
    private float lastRegenTime = 0f;

    public static Player Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (currentHitCount > 0 && Time.time - lastRegenTime > regenHealthTime)
        {
            currentHitCount--;
            lastRegenTime = Time.time;

            if (currentHitCount == 0)
                PlayerMovements.Instance.DisableHealthVignette();
        }
    }

    public void TakeDamage()
    {
        if (leftController != null) leftController.SendHapticImpulse(0.8f, 1f);
        if (rightController != null) rightController.SendHapticImpulse(0.8f, 1f);

        if (hitSounds.Count > 0)
        {
            lastHitSoundIndex = (lastHitSoundIndex + 1) % hitSounds.Count;
            Audio hitSound = hitSounds[lastHitSoundIndex];
            AudioSource.PlayClipAtPoint(hitSound.clip, transform.position, hitSound.volume);
        }

        currentHitCount++;
        lastHitTime = Time.time;
        lastRegenTime = Time.time;

        if (currentHitCount >= hitBeforeDiyng)
        {
            Die();
        }

        if (hitBeforeDiyng == currentHitCount + 1)
        {
            Debug.Log("Player hit");
            PlayerMovements.Instance.EnableHealthVignette();
        }
    }

    public bool CanAttackPlayer()
    {
        return Time.time - lastHitTime > notHitDuration;
    }

    private void Die()
    {
        Debug.Log("Player died");
        PlayerMovements.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }
}