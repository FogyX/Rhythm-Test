using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Scripts
{
    public class RestartButton : MonoBehaviour
    {
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}