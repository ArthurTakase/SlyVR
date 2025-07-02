using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class Rotator : MonoBehaviour
{
    [SerializeField] private XRBaseInteractable myInteractable;

    private void Start()
    {
        myInteractable.selectEntered.AddListener((args) => StartGrab(args.interactorObject as XRBaseInteractor));
        myInteractable.selectExited.AddListener((args) => EndGrab());
    }

    [SerializeField] private Transform linkedDial;
    [SerializeField] private TextMeshProUGUI rotationText;
    [SerializeField] private float hapticAmplitude = 0.5f;
    [SerializeField] private float hapticDuration = 0.1f;

    public XRDirectInteractor leftController;
    public XRDirectInteractor rightController;

    private float previousAngle;
    [HideInInspector] public int previousRotationNumber = -1;
    private bool isGrabbing = false;
    private XRBaseInteractor interactor;

    public void StartGrab(XRBaseInteractor interactor)
    {
        this.interactor = interactor;
        isGrabbing = true;
        previousAngle = GetInteractorZRotation(interactor);
        previousRotationNumber = GetRotationNumber();
    }

    public void EndGrab()
    {
        isGrabbing = false;
        interactor = null;
    }

    private void Update()
    {
        if (isGrabbing && interactor != null)
        {
            RotateWithInteractor();
        }
    }

    private void RotateWithInteractor()
    {
        float currentAngle = GetInteractorZRotation(interactor);
        float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);

        // Applique la rotation localement autour de Z
        linkedDial.Rotate(0f, 0f, deltaAngle, Space.Self);

        previousAngle = currentAngle;

        int currentNumber = GetRotationNumber();
        if (currentNumber != previousRotationNumber)
        {
            rotationText.text = currentNumber.ToString();
            previousRotationNumber = currentNumber;
            SendHapticFeedback();
        }
    }

    private int GetRotationNumber()
    {
        float zRotation = (linkedDial.localEulerAngles.z + 360) % 360;
        return Mathf.FloorToInt(zRotation / 36f);
    }

    private float GetInteractorZRotation(XRBaseInteractor interactor)
    {
        return -interactor.attachTransform.rotation.eulerAngles.z;
    }

    private void SendHapticFeedback()
    {
        if (leftController != null) leftController.SendHapticImpulse(hapticAmplitude, hapticDuration);
        if (rightController != null) rightController.SendHapticImpulse(hapticAmplitude, hapticDuration);
    }

    public void Disable()
    {
        EndGrab();
        linkedDial.gameObject.SetActive(false);
        rotationText.gameObject.SetActive(false);
    }
}