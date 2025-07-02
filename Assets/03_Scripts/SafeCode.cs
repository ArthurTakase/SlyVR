using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class SafeCode : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private float openSpeed = 1f;
    [SerializeField] private GameObject folder;
    [SerializeField] private AudioClip magicSound;
    public Rotator[] dials;
    int[] code;
    int[] dialsCode;
    bool isOpen = false;
    public Material[] numberMaterials;
    public DecalProjector[] projectorsSalle1;
    public DecalProjector[] projectorsSalle2;
    public DecalProjector[] projectorsSalle3;

    public void Start()
    {
        folder.SetActive(false);

        dialsCode = new int[3];
        code = new int[3];
        for (int i = 0; i < code.Length; i++)
            code[i] = Random.Range(0, 10);

        Debug.Log("Safe code: " + string.Join("", code));


        foreach (DecalProjector projector in projectorsSalle1) projector.material = numberMaterials[code[0]];
        foreach (DecalProjector projector in projectorsSalle2) projector.material = numberMaterials[code[1]];
        foreach (DecalProjector projector in projectorsSalle3) projector.material = numberMaterials[code[2]];

        foreach (DecalProjector projector in projectorsSalle1) projector.gameObject.SetActive(false);
        projectorsSalle1[Random.Range(0, projectorsSalle1.Length)].gameObject.SetActive(true);
        foreach (DecalProjector projector in projectorsSalle2) projector.gameObject.SetActive(false);
        projectorsSalle2[Random.Range(0, projectorsSalle2.Length)].gameObject.SetActive(true);
        foreach (DecalProjector projector in projectorsSalle3) projector.gameObject.SetActive(false);
        projectorsSalle3[Random.Range(0, projectorsSalle3.Length)].gameObject.SetActive(true);
    }

    public void Update()
    {
        if (isOpen) return;

        for (int i = 0; i < dials.Length; i++)
            dialsCode[i] = dials[i].previousRotationNumber;

        if (dialsCode.Length == code.Length &&
            dialsCode[0] == code[0] &&
            dialsCode[1] == code[1] &&
            dialsCode[2] == code[2])
            OpenSafe();
    }

    public void OpenSafe()
    {
        isOpen = true;

        foreach (Rotator dial in dials)
            dial.Disable();

        folder.SetActive(true);

        if (magicSound != null) AudioSource.PlayClipAtPoint(magicSound, transform.position);

        StartCoroutine(OpenSafeCoroutine());
    }

    private IEnumerator OpenSafeCoroutine()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * openSpeed;

            // rotate the doors
            door.transform.rotation = Quaternion.Slerp(door.transform.rotation, Quaternion.Euler(0, -90, 0), t);
            yield return null;
        }
    }
}