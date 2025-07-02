using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoors : MonoBehaviour
{
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    [SerializeField] private List<GameObject> toDisable = new();
    [SerializeField] private List<GameObject> toEnable = new();
    [SerializeField] private float closeSpeed = 1f;
    [SerializeField] private AudioSource doorCloseSound;

    public void CloseDoorsNow()
    {
        doorCloseSound.Play();
        StartCoroutine(CloseDoorsCoroutine());
    }

    private IEnumerator CloseDoorsCoroutine()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * closeSpeed;

            // rotate the doors
            door1.transform.rotation = Quaternion.Slerp(door1.transform.rotation, Quaternion.Euler(0, 0, 0), t);
            door2.transform.rotation = Quaternion.Slerp(door2.transform.rotation, Quaternion.Euler(0, 180, 0), t);
            yield return null;
        }

        foreach (var obj in toDisable)
            obj.SetActive(false);

        foreach (var obj in toEnable)
            obj.SetActive(true);

        while (doorCloseSound.isPlaying) yield return null;
        gameObject.SetActive(false);
    }
}