using UnityEngine;
using DG.Tweening;

public class TutorialStep : MonoBehaviour
{
    private Vector3 initialScale;

    public void Awake()
    {
        initialScale = transform.localScale;
    }

    public void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        transform
            .DOScale(initialScale, 0.3f)
            .SetEase(Ease.OutBack);
    }

    public void OnDisable()
    {
        transform
            .DOScale(new Vector3(0, 0, 0), 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}