using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseGameObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGameObject.SetActive(!pauseGameObject.activeInHierarchy);
        }
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
