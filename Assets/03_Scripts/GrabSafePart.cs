using UnityEngine;

public class GrabSafePart : MonoBehaviour
{
    [SerializeField] private GameObject CercleCodeTemp;
    [SerializeField] private GameObject CercleCode;
    [SerializeField] private GameObject CercleCodeMissing;

    [SerializeField] private Audio grabSound;

    [SerializeField] private GameObject[] fencesToDisable;

    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor leftController;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightController;

    public void OnStartGrab()
    {
        AudioSource.PlayClipAtPoint(grabSound.clip, transform.position, grabSound.volume);

        foreach (GameObject fence in fencesToDisable)
            fence.SetActive(false);

        CercleCodeTemp.SetActive(false);
        CercleCode.SetActive(true);

        if (leftController != null) leftController.SendHapticImpulse(0.6f, 0.5f);
        if (rightController != null) rightController.SendHapticImpulse(0.6f, 0.5f);

        Invoke(nameof(Disable), 1f);
    }

    private void Disable()
    {
        CercleCodeMissing.SetActive(false);
    }
}