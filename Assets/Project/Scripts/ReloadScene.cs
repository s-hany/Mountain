using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reload()
    {
        // Reload the current scene when the object is clicked
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
