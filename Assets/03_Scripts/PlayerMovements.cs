using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using System.Collections;

public class PlayerMovements : MonoBehaviour
{
    private float playerHeight = 1.7f;
    public CharacterController characterController;
    public Transform leftController;
    public Transform rightController;
    public Camera playerCamera;
    [HideInInspector] public Vector3 lastLeftControllerPosition;
    [HideInInspector] public Vector3 lastRightControllerPosition;
    public LayerMask groundLayer;
    public TunnelingVignetteController vignette;
    public LocomotionVignetteProvider vignetteProvider;
    public TunnelingVignetteController vignetteHealth;
    public LocomotionVignetteProvider vignetteProviderHealth;
    public TunnelingVignetteController vignetteLoading;
    public LocomotionVignetteProvider vignetteProviderLoading;
    public float skinWidth = 0.05f;

    public static PlayerMovements Instance;
    public bool isGrounded = false;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        playerHeight = characterController.height;
        vignetteLoading.EndTunnelingVignette(vignetteProviderLoading);
    }

    public void EnableHealthVignette()
    {
        vignetteHealth.BeginTunnelingVignette(vignetteProviderHealth);
    }

    public void DisableHealthVignette()
    {
        vignetteHealth.EndTunnelingVignette(vignetteProviderHealth);
    }

    public void EnableVignette()
    {
        vignette.BeginTunnelingVignette(vignetteProvider);
    }

    public void DisableVignette()
    {
        vignette.EndTunnelingVignette(vignetteProvider);
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        TrackControllerMovement();
        UpdateCharacterController();
    }

    private void TrackControllerMovement()
    {
        lastLeftControllerPosition = leftController.position;
        lastRightControllerPosition = rightController.position;
    }

    private bool IsGrounded()
    {
        Vector3 playerFeetPosition = transform.position + Vector3.up * 0.1f;
        float rayLength = 0.2f;
        Debug.DrawRay(playerFeetPosition, Vector3.down * rayLength, Color.red, 0.1f);
        return Physics.Raycast(playerFeetPosition, Vector3.down, out RaycastHit hit, rayLength, groundLayer);
    }

    void UpdateCharacterController()
    {
        // Mise à jour de la hauteur
        // float headHeight = Mathf.Clamp(playerCamera.transform.localPosition.y, 0.5f, 2.5f);
        // characterController.height = headHeight;

        // Centrage de la capsule (pivot bas)
        Vector3 center = Vector3.zero;
        center.y = characterController.height / 2 + characterController.skinWidth;

        // Décalage horizontal basé sur la tête
        center.x = playerCamera.transform.localPosition.x;
        center.z = playerCamera.transform.localPosition.z;

        characterController.center = center;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        vignetteLoading.BeginTunnelingVignette(vignetteProviderLoading);
        yield return new WaitForSeconds(0.5f);
        var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone) yield return null;
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        vignetteLoading.BeginTunnelingVignette(vignetteProviderLoading);
        yield return new WaitForSeconds(0.5f);
        var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncOperation.isDone) yield return null;
    }

    public void TpTo(Transform tpPosition)
    {
        StartCoroutine(TpToCoroutine(tpPosition));
    }

    private IEnumerator TpToCoroutine(Transform tpPosition)
    {
        Debug.Log("Teleporting to: " + tpPosition.position);
        vignetteLoading.BeginTunnelingVignette(vignetteProviderLoading);
        yield return new WaitForSeconds(0.5f);
        transform.SetPositionAndRotation(tpPosition.position, tpPosition.rotation);
        yield return new WaitForSeconds(0.5f);
        vignetteLoading.EndTunnelingVignette(vignetteProviderLoading);
    }
}