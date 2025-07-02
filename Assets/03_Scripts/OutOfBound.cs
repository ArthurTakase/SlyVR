using UnityEngine;
using UnityEngine.SceneManagement;

public class OutOfBound : MonoBehaviour
{
    [SerializeField] private bool reloadSceneOnEnter = true;
    [SerializeField] private bool tpToPositionOnEnter = false;
    [SerializeField] private Transform tpPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (tpToPositionOnEnter && tpPosition != null)
            {
                PlayerMovements.Instance.TpTo(tpPosition);
            }
            else if (reloadSceneOnEnter)
            {
                PlayerMovements.Instance.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}