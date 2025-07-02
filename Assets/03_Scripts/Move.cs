using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move : MonoBehaviour
{
    [SerializeField] private float swingThreshold = 0.01f;
    [SerializeField] private float acceleration = 2.0f;
    [SerializeField] private float deceleration = 3.0f;
    [SerializeField] private float accelerationAerial = 2.0f;
    [SerializeField] private float decelerationAerial = 3.0f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float maxSpeedAerial = 5.0f;
    private PlayerMovements p;
    private Vector3 lastLeftControllerPosition;
    private Vector3 lastRightControllerPosition;
    private float lastMovementTimestamp = 0;
    [HideInInspector] public float lastPlayerSpeed = 0;
    private float currentSpeed = 0.0f;
    private bool isMoving = false;
    private bool isAudioPlaying = false;
    public AudioSource audioSource_footstep;
    public List<Audio> footstepSounds;
    private Vector3 lastFramePosition;
    private Coroutine footstepAudioCoroutine;

    public bool HasMovedInLastFrames()
    {
        const float timestampThreshold = 0.5f;
        return Time.time - lastMovementTimestamp < timestampThreshold;
    }

    public void Start()
    {
        p = PlayerMovements.Instance;
        lastLeftControllerPosition = p.leftController.position;
        lastRightControllerPosition = p.rightController.position;
        footstepAudioCoroutine = StartCoroutine(FootstepAudio());
    }

    private void OnDestroy()
    {
        if (footstepAudioCoroutine != null) StopCoroutine(footstepAudioCoroutine);
    }

    public void Update()
    {
        DetectSwing();
        MovePlayer();

        lastLeftControllerPosition = p.leftController.position;
        lastRightControllerPosition = p.rightController.position;

        isAudioPlaying = audioSource_footstep.isPlaying;
    }

    private IEnumerator FootstepAudio()
    {
        while (true)
        {
            if (!p.isGrounded || (lastFramePosition == transform.position))
            {
                if (isAudioPlaying) audioSource_footstep.Stop();
                yield return null;
            }
            else if (!isAudioPlaying)
            {
                Audio stepSound = footstepSounds[Random.Range(0, footstepSounds.Count)];
                audioSource_footstep.clip = stepSound.clip;
                audioSource_footstep.volume = stepSound.volume;
                audioSource_footstep.Play();
            }
            lastFramePosition = transform.position;
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void DetectSwing()
    {
        Vector3 leftMovement = lastLeftControllerPosition - p.leftController.position;
        Vector3 rightMovement = lastRightControllerPosition - p.rightController.position;

        if (leftMovement.magnitude < swingThreshold || rightMovement.magnitude < swingThreshold) return;
        if (Mathf.Sign(leftMovement.y) == Mathf.Sign(rightMovement.y)) return;

        float verticalMovement = Mathf.Abs(leftMovement.y - rightMovement.y);
        if (verticalMovement < swingThreshold) return;

        currentSpeed += (p.isGrounded ? acceleration : accelerationAerial) * verticalMovement;
        lastPlayerSpeed = verticalMovement;
        lastMovementTimestamp = Time.time;
        isMoving = true;
    }

    private void MovePlayer()
    {
        if (currentSpeed <= 0) return;

        Vector3 forward = new Vector3(p.playerCamera.transform.forward.x, 0, p.playerCamera.transform.forward.z).normalized;
        Vector3 movement = currentSpeed * Time.deltaTime * forward;

        CollisionFlags collisionFlags = p.characterController.Move(movement);

        if (!isMoving)
        {
            currentSpeed -= (p.isGrounded ? deceleration : decelerationAerial) * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0);
        }

        currentSpeed = Mathf.Min(currentSpeed, p.isGrounded ? maxSpeed : maxSpeedAerial);

        if (currentSpeed > 0) p.EnableVignette();
        else p.DisableVignette();

        isMoving = false;

        // set a random pith for the footstep sound based on the current speed, between 0.5 and 1.5
        float basedOnSpeed = Mathf.Clamp(currentSpeed / maxSpeed, 0.5f, 1.5f);
        float addSomeVariation = Random.Range(-0.1f, 0.1f);
        audioSource_footstep.pitch = basedOnSpeed + addSomeVariation;

        if ((collisionFlags & CollisionFlags.Sides) != 0)
        {
            currentSpeed = 0;
            p.DisableVignette();
        }
    }
}