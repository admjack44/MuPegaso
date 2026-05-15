using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

namespace MuPegaso.Client.UI
{
    public class LoginController : MonoBehaviour
    {
        [Header("Video Background")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RawImage    videoBackground;

        [Header("Login Buttons")]
        [SerializeField] private Button btnGoogle;
        [SerializeField] private Button btnFacebook;
        [SerializeField] private Button btnApple;
        [SerializeField] private Button btnGuest;
        [SerializeField] private Button btnAccount;

        [Header("Panel Cuenta")]
        [SerializeField] private GameObject      panelAccount;
        [SerializeField] private TMP_InputField  inputUser;
        [SerializeField] private TMP_InputField  inputPass;
        [SerializeField] private Button          btnLogin;
        [SerializeField] private Button          btnRegister;
        [SerializeField] private Button          btnClosePanel;
        [SerializeField] private TextMeshProUGUI statusText;

        void Start()
        {
            SetupVideo();
            panelAccount.SetActive(false);

            btnGoogle.onClick.AddListener(OnGoogleLogin);
            btnFacebook.onClick.AddListener(OnFacebookLogin);
            btnApple.onClick.AddListener(OnAppleLogin);
            btnGuest.onClick.AddListener(OnGuestLogin);
            btnAccount.onClick.AddListener(() => panelAccount.SetActive(true));
            btnClosePanel.onClick.AddListener(() => panelAccount.SetActive(false));
            btnLogin.onClick.AddListener(OnAccountLogin);
            btnRegister.onClick.AddListener(OnRegister);
        }

        void SetupVideo()
        {
            if (videoPlayer == null || videoBackground == null) return;
            videoPlayer.isLooping   = true;
            videoPlayer.playOnAwake = true;
            if (videoPlayer.targetTexture == null)
            {
                var rt = new RenderTexture(1920, 1080, 24);
                videoPlayer.targetTexture = rt;
                videoBackground.texture   = rt;
            }
            else
                videoBackground.texture = videoPlayer.targetTexture;
            videoPlayer.Play();
        }

        void OnGoogleLogin()
        {
            statusText.text = "Conectando con Google...";
            Debug.Log("[Login] Google");
            // TODO: Google Play Games SDK
        }

        void OnFacebookLogin()
        {
            statusText.text = "Conectando con Facebook...";
            Debug.Log("[Login] Facebook");
            // TODO: Facebook SDK
        }

        void OnAppleLogin()
        {
            statusText.text = "Conectando con Apple...";
            Debug.Log("[Login] Apple");
            // TODO: Sign in with Apple
        }

        void OnGuestLogin()
        {
            statusText.text = "Entrando como invitado...";
            Debug.Log("[Login] Guest");
            string guestId = "guest_" + SystemInfo.deviceUniqueIdentifier.Substring(0, 8);
            SendLoginRequest(guestId, "guest");
        }

        void OnAccountLogin()
        {
            string user = inputUser.text.Trim();
            string pass = inputPass.text;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                statusText.text = "Completa usuario y contraseña";
                return;
            }
            btnLogin.interactable = false;
            statusText.text = "Verificando...";
            SendLoginRequest(user, pass);
        }

        void OnRegister()
        {
            Debug.Log("[Login] Abrir registro");
            // TODO: SceneManager.LoadScene("Register")
        }

        void SendLoginRequest(string user, string pass)
        {
            Debug.Log($"[Login] Enviando request: {user}");
            // TODO: NetworkClient.Instance.SendLogin(user, pass)
        }
    }
}
