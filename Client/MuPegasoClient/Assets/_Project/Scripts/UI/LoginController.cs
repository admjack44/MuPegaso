using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace MuPegaso.Client.UI
{
    public class LoginController : MonoBehaviour
    {
        const string SelectServerScene = "SelectServer";

        Canvas _canvas;

        VideoPlayer _videoPlayer;
        RawImage _videoBackground;

        TextMeshProUGUI _statusText;

        GameObject _panelAccount;
        Button _btnClose;
        Button _tabLogin;
        Button _tabRegister;
        Button _tabRecover;
        GameObject _panelLoginForm;
        GameObject _panelRegisterForm;
        GameObject _panelRecoverForm;
        TMP_InputField _inputUser;
        TMP_InputField _inputPass;
        TMP_InputField _inputRegUser;
        TMP_InputField _inputRegEmail;
        TMP_InputField _inputRegPass;
        TMP_InputField _inputRegPass2;
        TMP_InputField _inputRecEmail;
        Button _btnLoginAccount;
        Button _btnRegisterAccount;
        Button _btnRecoverAccount;
        TextMeshProUGUI _formStatus;

        void Start()
        {
            Debug.Log("LoginController Start OK");

            if (!BindReferences())
                return;

            BindLoginButtons();
            SetupVideo();

            if (_panelAccount != null)
                _panelAccount.SetActive(false);

            if (_btnClose != null)
                _btnClose.onClick.AddListener(CloseAccountPanel);

            if (_tabLogin != null)
                _tabLogin.onClick.AddListener(() => ShowTab("login"));
            if (_tabRegister != null)
                _tabRegister.onClick.AddListener(() => ShowTab("register"));
            if (_tabRecover != null)
                _tabRecover.onClick.AddListener(() => ShowTab("recover"));

            if (_btnLoginAccount != null)
                _btnLoginAccount.onClick.AddListener(OnFormLogin);
            if (_btnRegisterAccount != null)
                _btnRegisterAccount.onClick.AddListener(OnRegister);
            if (_btnRecoverAccount != null)
                _btnRecoverAccount.onClick.AddListener(OnRecover);

            if (_statusText != null)
                _statusText.text = "";
            if (_formStatus != null)
                _formStatus.text = "";
        }

        bool BindReferences()
        {
            _canvas = FindObjectOfType<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError("[Login] No se encontró Canvas.");
                return false;
            }

            var canvasTransform = _canvas.transform;

            _panelAccount = canvasTransform.Find("Panel_Account")?.gameObject;

            _statusText = FindText("Panel_Login/Text_Status");
            if (_statusText == null)
                _statusText = FindText("Text_Status");
            _btnClose = FindButton("Panel_Account/Btn_Close");
            _tabLogin = FindButton("Panel_Account/Panel_Tabs/Tab_Login");
            _tabRegister = FindButton("Panel_Account/Panel_Tabs/Tab_Register");
            _tabRecover = FindButton("Panel_Account/Panel_Tabs/Tab_Recover");

            _panelLoginForm = FindObject("Panel_Account/Panel_Login_Form");
            _panelRegisterForm = FindObject("Panel_Account/Panel_Register_Form");
            _panelRecoverForm = FindObject("Panel_Account/Panel_Recover_Form");

            _inputUser = FindInput("Panel_Account/Panel_Login_Form/Input_User");
            _inputPass = FindInput("Panel_Account/Panel_Login_Form/Input_Pass");
            _btnLoginAccount = FindButton("Panel_Account/Panel_Login_Form/Btn_Login_Account");
            if (_btnLoginAccount == null)
                _btnLoginAccount = FindButton("Panel_Account/Panel_Login_Form/Btn_Login");

            _inputRegUser = FindInput("Panel_Account/Panel_Register_Form/Input_RegUser");
            _inputRegEmail = FindInput("Panel_Account/Panel_Register_Form/Input_RegEmail");
            _inputRegPass = FindInput("Panel_Account/Panel_Register_Form/Input_RegPass");
            _inputRegPass2 = FindInput("Panel_Account/Panel_Register_Form/Input_RegPass2");
            _btnRegisterAccount = FindButton("Panel_Account/Panel_Register_Form/Btn_Register_Account");
            if (_btnRegisterAccount == null)
                _btnRegisterAccount = FindButton("Panel_Account/Panel_Register_Form/Btn_Register");

            _inputRecEmail = FindInput("Panel_Account/Panel_Recover_Form/Input_RecEmail");
            _btnRecoverAccount = FindButton("Panel_Account/Panel_Recover_Form/Btn_Recover_Account");
            if (_btnRecoverAccount == null)
                _btnRecoverAccount = FindButton("Panel_Account/Panel_Recover_Form/Btn_Recover");

            _formStatus = FindText("Panel_Account/Text_FormStatus");

            var videoGo = GameObject.Find("VideoPlayer");
            if (videoGo != null)
                _videoPlayer = videoGo.GetComponent<VideoPlayer>();

            _videoBackground = canvasTransform.Find("BG_Video")?.GetComponent<RawImage>();

            LogMissing(_panelAccount, "Panel_Account");
            LogMissing(_btnClose, "Btn_Close");

            return _canvas != null && _panelAccount != null;
        }

        void BindLoginButtons()
        {
            var allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
            foreach (var btn in allButtons)
            {
                switch (btn.gameObject.name)
                {
                    case "Btn_Google":
                        btn.onClick.AddListener(() => SceneManager.LoadScene("SelectServer"));
                        Debug.Log("Btn_Google asignado");
                        break;
                    case "Btn_Facebook":
                        btn.onClick.AddListener(() => SceneManager.LoadScene("SelectServer"));
                        Debug.Log("Btn_Facebook asignado");
                        break;
                    case "Btn_Apple":
                        btn.onClick.AddListener(() => SceneManager.LoadScene("SelectServer"));
                        Debug.Log("Btn_Apple asignado");
                        break;
                    case "Btn_Guest":
                        btn.onClick.AddListener(() => SceneManager.LoadScene("SelectServer"));
                        Debug.Log("Btn_Guest asignado");
                        break;
                    case "Btn_Account":
                        btn.onClick.AddListener(OnAccountClicked);
                        Debug.Log("Btn_Account asignado");
                        break;
                }
            }
        }

        static void LogMissing(Object obj, string name)
        {
            if (obj == null)
                Debug.LogWarning($"[Login] No encontrado: {name}");
        }

        Button FindButton(string path)
        {
            return _canvas.transform.Find(path)?.GetComponent<Button>();
        }

        TMP_InputField FindInput(string path)
        {
            return _canvas.transform.Find(path)?.GetComponent<TMP_InputField>();
        }

        TextMeshProUGUI FindText(string path)
        {
            return _canvas.transform.Find(path)?.GetComponent<TextMeshProUGUI>();
        }

        GameObject FindObject(string path)
        {
            return _canvas.transform.Find(path)?.gameObject;
        }

        void SetupVideo()
        {
            if (_videoPlayer == null || _videoBackground == null)
                return;

            _videoPlayer.isLooping = true;
            _videoPlayer.playOnAwake = true;

            if (_videoPlayer.targetTexture == null)
            {
                var rt = new RenderTexture(1920, 1080, 24);
                _videoPlayer.targetTexture = rt;
                _videoBackground.texture = rt;
            }
            else
            {
                _videoBackground.texture = _videoPlayer.targetTexture;
            }

            _videoPlayer.Play();
        }

        void OnAccountClicked()
        {
            _panelAccount.SetActive(true);
            ShowTab("login");
        }

        void CloseAccountPanel()
        {
            _panelAccount.SetActive(false);
            if (_formStatus != null)
                _formStatus.text = "";
        }

        void ShowTab(string tab)
        {
            if (_panelLoginForm != null)
                _panelLoginForm.SetActive(tab == "login");
            if (_panelRegisterForm != null)
                _panelRegisterForm.SetActive(tab == "register");
            if (_panelRecoverForm != null)
                _panelRecoverForm.SetActive(tab == "recover");
            if (_formStatus != null)
                _formStatus.text = "";

            var active = new Color32(200, 164, 0, 255);
            var inactive = new Color32(51, 51, 51, 255);
            SetTabVisual(_tabLogin, tab == "login", active, inactive);
            SetTabVisual(_tabRegister, tab == "register", active, inactive);
            SetTabVisual(_tabRecover, tab == "recover", active, inactive);
        }

        static void SetTabVisual(Button button, bool isOn, Color32 active, Color32 inactive)
        {
            if (button == null)
                return;
            var image = button.targetGraphic as Image;
            if (image != null)
                image.color = isOn ? active : inactive;
        }

        void OnFormLogin()
        {
            if (_inputUser == null || _inputPass == null)
                return;

            var user = _inputUser.text.Trim();
            var pass = _inputPass.text;
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                if (_formStatus != null)
                    _formStatus.text = "Completa usuario y contraseña";
                return;
            }

            if (_btnLoginAccount != null)
                _btnLoginAccount.interactable = false;
            if (_formStatus != null)
                _formStatus.text = "Verificando...";

            Debug.Log($"[Login] Cuenta: {user}");
            SceneManager.LoadScene(SelectServerScene);
        }

        void OnRegister()
        {
            if (_inputRegUser == null || _inputRegEmail == null || _inputRegPass == null || _inputRegPass2 == null)
                return;

            var user = _inputRegUser.text.Trim();
            var email = _inputRegEmail.text.Trim();
            var pass = _inputRegPass.text;
            var pass2 = _inputRegPass2.text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                if (_formStatus != null)
                    _formStatus.text = "Completa todos los campos";
                return;
            }

            if (pass != pass2)
            {
                if (_formStatus != null)
                    _formStatus.text = "Las contraseñas no coinciden";
                return;
            }

            if (_formStatus != null)
                _formStatus.text = "Creando cuenta...";
            Debug.Log($"[Register] {user} / {email}");
        }

        void OnRecover()
        {
            if (_inputRecEmail == null)
                return;

            var email = _inputRecEmail.text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                if (_formStatus != null)
                    _formStatus.text = "Ingresa tu email";
                return;
            }

            if (_formStatus != null)
                _formStatus.text = "Enviando código...";
            Debug.Log($"[Recover] {email}");
        }
    }
}
