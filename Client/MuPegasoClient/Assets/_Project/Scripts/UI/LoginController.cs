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
        [SerializeField] private GameObject      panelLoginForm;
        [SerializeField] private GameObject      panelRegisterForm;
        [SerializeField] private GameObject      panelRecoverForm;
        [SerializeField] private Button          tabLogin;
        [SerializeField] private Button          tabRegister;
        [SerializeField] private Button          tabRecover;
        [SerializeField] private Button          btnCloseAccount;
        [SerializeField] private TMP_InputField  inputUser;
        [SerializeField] private TMP_InputField  inputPass;
        [SerializeField] private TMP_InputField  inputRegUser;
        [SerializeField] private TMP_InputField  inputRegEmail;
        [SerializeField] private TMP_InputField  inputRegPass;
        [SerializeField] private TMP_InputField  inputRegPass2;
        [SerializeField] private TMP_InputField  inputRecEmail;
        [SerializeField] private Button          btnLoginAccount;
        [SerializeField] private Button          btnRegisterAccount;
        [SerializeField] private Button          btnRecoverAccount;
        [SerializeField] private TextMeshProUGUI formStatus;

        [SerializeField] private TextMeshProUGUI statusText;

        void Start()
        {
            SetupVideo();
            panelAccount.SetActive(false);

            btnGoogle.onClick.AddListener(OnGoogleLogin);
            btnFacebook.onClick.AddListener(OnFacebookLogin);
            btnApple.onClick.AddListener(OnAppleLogin);
            btnGuest.onClick.AddListener(OnGuestLogin);
            btnAccount.onClick.AddListener(() =>
            {
                panelAccount.SetActive(true);
                ShowTab("login");
            });

            btnCloseAccount.onClick.AddListener(() => panelAccount.SetActive(false));
            tabLogin.onClick.AddListener(() => ShowTab("login"));
            tabRegister.onClick.AddListener(() => ShowTab("register"));
            tabRecover.onClick.AddListener(() => ShowTab("recover"));

            btnLoginAccount.onClick.AddListener(OnAccountLogin);
            btnRegisterAccount.onClick.AddListener(OnRegister);
            btnRecoverAccount.onClick.AddListener(OnRecover);

            statusText.text = "";
            formStatus.text = "";
        }

        void ShowTab(string tab)
        {
            panelLoginForm.SetActive(tab == "login");
            panelRegisterForm.SetActive(tab == "register");
            panelRecoverForm.SetActive(tab == "recover");
            formStatus.text = "";

            var active = new Color32(200, 164, 0, 255);
            var inactive = new Color32(51, 51, 51, 255);
            SetTabVisual(tabLogin, tab == "login", active, inactive);
            SetTabVisual(tabRegister, tab == "register", active, inactive);
            SetTabVisual(tabRecover, tab == "recover", active, inactive);
        }

        static void SetTabVisual(Button b, bool on, Color32 active, Color32 inactive)
        {
            if (b == null) return;
            var img = b.targetGraphic as Image;
            if (img != null) img.color = on ? active : inactive;
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
        }

        void OnFacebookLogin()
        {
            statusText.text = "Conectando con Facebook...";
            Debug.Log("[Login] Facebook");
        }

        void OnAppleLogin()
        {
            statusText.text = "Conectando con Apple...";
            Debug.Log("[Login] Apple");
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
                formStatus.text = "Completa usuario y contraseña";
                return;
            }
            btnLoginAccount.interactable = false;
            formStatus.text = "Verificando...";
            SendLoginRequest(user, pass);
        }

        void OnRegister()
        {
            string user  = inputRegUser.text.Trim();
            string email = inputRegEmail.text.Trim();
            string pass  = inputRegPass.text;
            string pass2 = inputRegPass2.text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(pass))
            {
                formStatus.text = "Completa todos los campos";
                return;
            }

            if (pass != pass2)
            {
                formStatus.text = "Las contraseñas no coinciden";
                return;
            }

            formStatus.text = "Creando cuenta...";
            Debug.Log($"[Register] {user} / {email}");
        }

        void OnRecover()
        {
            string email = inputRecEmail.text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                formStatus.text = "Ingresa tu email";
                return;
            }

            formStatus.text = "Enviando código...";
            Debug.Log($"[Recover] {email}");
        }

        void SendLoginRequest(string user, string pass)
        {
            Debug.Log($"[Login] Enviando request: {user}");
        }
    }
}
