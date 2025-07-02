using UnityEngine;
using TMPro;

public class Basketball : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            score++;
            scoreText.text = score.ToString();
        }
    }
}