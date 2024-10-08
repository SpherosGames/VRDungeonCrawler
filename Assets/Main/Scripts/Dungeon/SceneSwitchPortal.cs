using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchPortal : MonoBehaviour
{
    [SerializeField] private int playerLayerNum;
    [SerializeField] private string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayerNum)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
