using UnityEngine;
using UnityEngine.SceneManagement;
using MuPegaso.Client.Network;

namespace MuPegaso.Client.UI
{
    public class BootController : MonoBehaviour
    {
        [SerializeField] private string nextScene = "Splash";
        [SerializeField] private float splashDelay = 0.1f;

        void Start()
        {
            // Iniciar conexión al servidor en background
            // NetworkClient ya tiene DontDestroyOnLoad y conecta solo
            // El jugador ve Splash mientras se conecta
            Invoke(nameof(GoToSplash), splashDelay);
        }

        void GoToSplash()
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
