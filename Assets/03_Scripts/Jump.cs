using UnityEngine;
using System.Collections.Generic;

public class Jump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 7.0f; // Augmenter la force verticale
    [SerializeField] private float propulsionForce = 4.0f; // Réduire la force horizontale
    [SerializeField] private float maxPropulsionForce = 10.0f; // Limite supérieure de la force horizontale
    [SerializeField] private float minPropulsionForce = 0.5f; // Limite inférieure de la force horizontale
    [SerializeField] private float jumpThreshold = 1.5f;
    [SerializeField] private float climbEjectVerticalMax = 0.2f;
    [SerializeField] private float climbEjectHorizontalMax = 0.2f;
    [SerializeField] private float gravityForce = 5f;
    private PlayerMovements p;
    private Vector3 lastLeftControllerPosition;
    private Vector3 lastRightControllerPosition;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private Vector3 horizontalVelocity = Vector3.zero;
    private Move moveScript;
    private bool canJump = true;
    private float jumpTimeStamp = 0;
    public List<Audio> jumpSounds;
    public List<Audio> landSounds;
    private bool wasGrounded = true;
    private int lastJumpSoundIndex = -1;
    private int lastLandSoundIndex = -1;
    private float lastLandSoundTimestamp = 0;
    private readonly float delayBetweenLandSounds = 0.5f;

    private void Start()
    {
        moveScript = GetComponent<Move>();
        p = PlayerMovements.Instance;
        lastLeftControllerPosition = p.leftController.position;
        lastRightControllerPosition = p.rightController.position;
    }

    private void Update()
    {
        if (!canJump)
        {
            UpdateControllerPositions();
            return;
        }

        JumpPlayer();
        ApplyGravity();
        ApplyPropulsion();
        p.characterController.Move((horizontalVelocity + new Vector3(0, verticalVelocity, 0)) * Time.deltaTime);

        if (p.isGrounded && jumpTimeStamp + 0.2f < Time.time)
        {
            verticalVelocity = -0.1f;
            horizontalVelocity = Vector3.zero;
        }

        if (wasGrounded && !p.isGrounded && jumpSounds.Count > 0 && verticalVelocity > 0)
        {
            lastJumpSoundIndex = (lastJumpSoundIndex + 1) % jumpSounds.Count;
            Audio randomJumpSound = jumpSounds[lastJumpSoundIndex];
            AudioSource.PlayClipAtPoint(randomJumpSound.clip, transform.position, randomJumpSound.volume);
        }

        if (!wasGrounded && p.isGrounded && landSounds.Count > 0 && verticalVelocity <= 0)
        {
            if (Time.time - lastLandSoundTimestamp < delayBetweenLandSounds)
                return;
            lastLandSoundIndex = (lastLandSoundIndex + 1) % landSounds.Count;
            Audio randomLandSound = landSounds[lastLandSoundIndex];
            AudioSource.PlayClipAtPoint(randomLandSound.clip, transform.position, randomLandSound.volume);
            lastLandSoundTimestamp = Time.time;
        }

        wasGrounded = p.isGrounded;
    }

    private void JumpPlayer()
    {
        Vector3 leftPosition = p.leftController.position - transform.position;
        Vector3 rightPosition = p.rightController.position - transform.position;

        Vector3 leftVelocity = (lastLeftControllerPosition - leftPosition) / Time.deltaTime;
        Vector3 rightVelocity = (lastRightControllerPosition - rightPosition) / Time.deltaTime;

        // Vector3 leftVelocity = new(10, 10, 10);
        // Vector3 rightVelocity = new(10, 10, 10);

        if (p.isGrounded
            && leftVelocity.y > jumpThreshold
            && rightVelocity.y > jumpThreshold)
        {
            if (leftVelocity.magnitude < 0.1f && rightVelocity.magnitude < 0.1f)
            {
                leftVelocity = new Vector3(0, jumpThreshold * 2, 0);
                rightVelocity = new Vector3(0, jumpThreshold * 2, 0);
            }

            Vector3 combinedVelocity = (leftVelocity + rightVelocity) / 2f;
            Vector3 jumpDirection = combinedVelocity.normalized;

            verticalVelocity = jumpForce * Mathf.Abs(jumpDirection.y);
            if (moveScript.HasMovedInLastFrames())
            {
                float propulsionFactor = propulsionForce * (moveScript.lastPlayerSpeed * 50);
                propulsionFactor = Mathf.Clamp(propulsionFactor, minPropulsionForce, maxPropulsionForce);
                horizontalVelocity = propulsionFactor * new Vector3(p.playerCamera.transform.forward.x, 0, p.playerCamera.transform.forward.z);
            }
            else horizontalVelocity = Vector3.zero;

            jumpTimeStamp = Time.time;
        }

        lastLeftControllerPosition = leftPosition;
        lastRightControllerPosition = rightPosition;
    }

    private void ApplyGravity()
    {
        if (!p.characterController.isGrounded)
            verticalVelocity -= gravityForce * Time.deltaTime;
        else if (verticalVelocity < 0)
            verticalVelocity = -0.1f;
    }

    private void ApplyPropulsion()
    {
        horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime);
    }

    public void SetCanJump(bool value)
    {
        canJump = value;
    }

    private void UpdateControllerPositions()
    {
        lastLeftControllerPosition = p.leftController.position;
        lastRightControllerPosition = p.rightController.position;
    }

    public void ReleaseGrip()
    {
        if (!p.isGrounded)
        {
            Vector3 leftVelocity = (p.leftController.position - lastLeftControllerPosition) / Time.deltaTime;
            Vector3 rightVelocity = (p.rightController.position - lastRightControllerPosition) / Time.deltaTime;
            Vector3 launchVelocity = (leftVelocity + rightVelocity) * 0.5f;

            verticalVelocity = Mathf.Clamp(Mathf.Max(launchVelocity.y * climbEjectVerticalMax, 0), -climbEjectVerticalMax, climbEjectVerticalMax);
            horizontalVelocity = new Vector3(launchVelocity.x * climbEjectHorizontalMax, 0, launchVelocity.z * climbEjectHorizontalMax);
        }
    }
}