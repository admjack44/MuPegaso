#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

namespace MuPegaso.Client.Editor
{
    public static class LoginAccountPanelRebuild
    {
        [MenuItem("Tools/MuPegaso/Rebuild Login Account Panel")]
        public static void Run()
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null) { Debug.LogError("No Canvas"); return; }
            var paTf = canvas.transform.Find("Panel_Account");
            if (paTf == null) { Debug.LogError("No Panel_Account"); return; }

            for (int i = paTf.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(paTf.GetChild(i).gameObject);

            var oldV = paTf.GetComponent<VerticalLayoutGroup>();
            if (oldV != null)
                Object.DestroyImmediate(oldV, true);

            var uiSpr = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            if (uiSpr == null) uiSpr = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");

            var paRt = paTf.GetComponent<RectTransform>();
            paRt.anchorMin = paRt.anchorMax = new Vector2(0.5f, 0.5f);
            paRt.pivot = new Vector2(0.5f, 0.5f);
            paRt.sizeDelta = new Vector2(520f, 580f);
            paRt.anchoredPosition = Vector2.zero;

            var paImg = paTf.GetComponent<Image>();
            if (paImg != null)
            {
                paImg.color = new Color(0f, 0f, 0f, 220f / 255f);
                if (uiSpr != null) { paImg.sprite = uiSpr; paImg.type = Image.Type.Sliced; }
            }

            var rootV = paTf.gameObject.AddComponent<VerticalLayoutGroup>();
            rootV.padding = new RectOffset(20, 20, 48, 16);
            rootV.spacing = 12;
            rootV.childAlignment = TextAnchor.UpperCenter;
            rootV.childControlHeight = true;
            rootV.childForceExpandHeight = false;
            rootV.childControlWidth = true;
            rootV.childForceExpandWidth = true;

            void Stretch(RectTransform rt)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = rt.offsetMax = Vector2.zero;
            }

            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(paTf, false);
            var titleRt = titleGo.AddComponent<RectTransform>();
            var titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "INICIAR SESI\u00D3N";
            titleTmp.fontSize = 28;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.color = new Color32(200, 164, 0, 255);
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleGo.AddComponent<LayoutElement>().preferredHeight = 40f;

            var closeGo = new GameObject("Btn_Close");
            closeGo.transform.SetParent(paTf, false);
            var cRt = closeGo.AddComponent<RectTransform>();
            cRt.anchorMin = cRt.anchorMax = new Vector2(1f, 1f);
            cRt.pivot = new Vector2(1f, 1f);
            cRt.sizeDelta = new Vector2(40f, 40f);
            cRt.anchoredPosition = new Vector2(-12f, -12f);
            var cImg = closeGo.AddComponent<Image>();
            cImg.color = new Color(1f, 1f, 1f, 0f);
            if (uiSpr != null) { cImg.sprite = uiSpr; cImg.type = Image.Type.Sliced; }
            var cBtn = closeGo.AddComponent<Button>();
            cBtn.targetGraphic = cImg;
            var cx = new GameObject("Text");
            cx.transform.SetParent(closeGo.transform, false);
            Stretch(cx.AddComponent<RectTransform>());
            var cxTmp = cx.AddComponent<TextMeshProUGUI>();
            cxTmp.text = "\u2715";
            cxTmp.fontSize = 22;
            cxTmp.alignment = TextAlignmentOptions.Center;
            cxTmp.color = Color.white;
            var closeLe = closeGo.AddComponent<LayoutElement>();
            closeLe.ignoreLayout = true;

            var tabsGo = new GameObject("Panel_Tabs");
            tabsGo.transform.SetParent(paTf, false);
            var tabsRt = tabsGo.AddComponent<RectTransform>();
            var tabsH = tabsGo.AddComponent<HorizontalLayoutGroup>();
            tabsH.spacing = 8;
            tabsH.childAlignment = TextAnchor.MiddleCenter;
            tabsH.childControlWidth = true;
            tabsH.childForceExpandWidth = true;
            tabsH.childControlHeight = true;
            tabsH.childForceExpandHeight = true;
            tabsGo.AddComponent<LayoutElement>().preferredHeight = 44f;

            Button MakeTab(string nm, string label, Color32 bg)
            {
                var go = new GameObject(nm);
                go.transform.SetParent(tabsGo.transform, false);
                go.AddComponent<RectTransform>();
                var img = go.AddComponent<Image>();
                if (uiSpr != null) { img.sprite = uiSpr; img.type = Image.Type.Sliced; }
                img.color = bg;
                var btn = go.AddComponent<Button>();
                btn.targetGraphic = img;
                var tg = new GameObject("Text");
                tg.transform.SetParent(go.transform, false);
                var trt = tg.AddComponent<RectTransform>();
                Stretch(trt);
                var tt = tg.AddComponent<TextMeshProUGUI>();
                tt.text = label;
                tt.fontSize = 18;
                tt.alignment = TextAlignmentOptions.Center;
                tt.color = Color.white;
                return btn;
            }

            var gold = new Color32(200, 164, 0, 255);
            var grey = new Color32(51, 51, 51, 255);
            var tabLogin = MakeTab("Tab_Login", "Ingresar", gold);
            var tabRegister = MakeTab("Tab_Register", "Registrarse", grey);
            var tabRecover = MakeTab("Tab_Recover", "Recuperar", grey);

            GameObject MakeFormPanel(string nm, bool active)
            {
                var go = new GameObject(nm);
                go.transform.SetParent(paTf, false);
                go.SetActive(active);
                var v = go.AddComponent<VerticalLayoutGroup>();
                v.spacing = 10;
                v.padding = new RectOffset(0, 0, 0, 0);
                v.childAlignment = TextAnchor.UpperCenter;
                v.childControlHeight = true;
                v.childForceExpandHeight = false;
                v.childControlWidth = true;
                v.childForceExpandWidth = true;
                go.AddComponent<LayoutElement>().flexibleHeight = 1f;
                return go;
            }

            var formLogin = MakeFormPanel("Panel_Login_Form", true);
            var formReg = MakeFormPanel("Panel_Register_Form", false);
            var formRec = MakeFormPanel("Panel_Recover_Form", false);

            TMP_InputField MakeInput(Transform parent, string nm, string ph, bool pass, float h)
            {
                var root = new GameObject(nm);
                root.transform.SetParent(parent, false);
                root.AddComponent<RectTransform>();
                var bg = root.AddComponent<Image>();
                if (uiSpr != null) { bg.sprite = uiSpr; bg.type = Image.Type.Sliced; }
                bg.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);
                var inp = root.AddComponent<TMP_InputField>();
                var vp = new GameObject("Text Area");
                vp.transform.SetParent(root.transform, false);
                var vpRt = vp.AddComponent<RectTransform>();
                Stretch(vpRt);
                vpRt.offsetMin = new Vector2(12f, 8f);
                vpRt.offsetMax = new Vector2(-12f, -8f);
                vp.AddComponent<RectMask2D>();
                var phGo = new GameObject("Placeholder");
                phGo.transform.SetParent(vp.transform, false);
                Stretch(phGo.AddComponent<RectTransform>());
                var phTmp = phGo.AddComponent<TextMeshProUGUI>();
                phTmp.text = ph;
                phTmp.fontSize = 18;
                phTmp.color = new Color(1f, 1f, 1f, 0.45f);
                phTmp.alignment = TextAlignmentOptions.Left;
                var txGo = new GameObject("Text");
                txGo.transform.SetParent(vp.transform, false);
                Stretch(txGo.AddComponent<RectTransform>());
                var txTmp = txGo.AddComponent<TextMeshProUGUI>();
                txTmp.fontSize = 20;
                txTmp.color = Color.white;
                txTmp.alignment = TextAlignmentOptions.Left;
                inp.textViewport = vpRt;
                inp.textComponent = txTmp;
                inp.placeholder = phTmp;
                inp.pointSize = 20;
                if (pass) inp.contentType = TMP_InputField.ContentType.Password;
                root.AddComponent<LayoutElement>().preferredHeight = h;
                return inp;
            }

            Button MakeActionBtn(Transform parent, string nm, string txt, float h)
            {
                var go = new GameObject(nm);
                go.transform.SetParent(parent, false);
                go.AddComponent<RectTransform>();
                var img = go.AddComponent<Image>();
                if (uiSpr != null) { img.sprite = uiSpr; img.type = Image.Type.Sliced; }
                img.color = gold;
                var btn = go.AddComponent<Button>();
                btn.targetGraphic = img;
                go.AddComponent<LayoutElement>().preferredHeight = h;
                var tg = new GameObject("Text");
                tg.transform.SetParent(go.transform, false);
                Stretch(tg.AddComponent<RectTransform>());
                var tt = tg.AddComponent<TextMeshProUGUI>();
                tt.text = txt;
                tt.fontSize = 20;
                tt.fontStyle = FontStyles.Bold;
                tt.alignment = TextAlignmentOptions.Center;
                tt.color = Color.white;
                return btn;
            }

            var inUser = MakeInput(formLogin.transform, "Input_User", "Usuario", false, 55f);
            var inPass = MakeInput(formLogin.transform, "Input_Pass", "Contrase\u00F1a", true, 55f);
            var btnL = MakeActionBtn(formLogin.transform, "Btn_Login", "INGRESAR", 60f);

            MakeInput(formReg.transform, "Input_RegUser", "Usuario", false, 55f);
            MakeInput(formReg.transform, "Input_RegEmail", "Email", false, 55f);
            MakeInput(formReg.transform, "Input_RegPass", "Contrase\u00F1a", true, 55f);
            MakeInput(formReg.transform, "Input_RegPass2", "Confirmar clave", true, 55f);
            MakeActionBtn(formReg.transform, "Btn_Register", "CREAR CUENTA", 60f);

            MakeInput(formRec.transform, "Input_RecEmail", "Email de la cuenta", false, 55f);
            MakeActionBtn(formRec.transform, "Btn_Recover", "ENVIAR C\u00D3DIGO", 60f);

            var fsGo = new GameObject("Text_FormStatus");
            fsGo.transform.SetParent(paTf, false);
            var fsTmp = fsGo.AddComponent<TextMeshProUGUI>();
            fsTmp.text = "";
            fsTmp.fontSize = 16;
            fsTmp.color = new Color32(255, 68, 68, 255);
            fsTmp.alignment = TextAlignmentOptions.Center;
            fsGo.AddComponent<LayoutElement>().preferredHeight = 36f;

            var lcGo = GameObject.Find("LoginSceneRoot");
            var lc = lcGo != null ? lcGo.GetComponent<MuPegaso.Client.UI.LoginController>() : Object.FindObjectOfType<MuPegaso.Client.UI.LoginController>();
            if (lc == null) { Debug.LogError("No LoginController"); return; }
            var so = new SerializedObject(lc);
            so.FindProperty("panelAccount").objectReferenceValue = paTf.gameObject;
            so.FindProperty("panelLoginForm").objectReferenceValue = formLogin;
            so.FindProperty("panelRegisterForm").objectReferenceValue = formReg;
            so.FindProperty("panelRecoverForm").objectReferenceValue = formRec;
            so.FindProperty("tabLogin").objectReferenceValue = tabLogin;
            so.FindProperty("tabRegister").objectReferenceValue = tabRegister;
            so.FindProperty("tabRecover").objectReferenceValue = tabRecover;
            so.FindProperty("btnCloseAccount").objectReferenceValue = cBtn;
            so.FindProperty("inputUser").objectReferenceValue = inUser;
            so.FindProperty("inputPass").objectReferenceValue = inPass;
            so.FindProperty("inputRegUser").objectReferenceValue = formReg.transform.Find("Input_RegUser").GetComponent<TMP_InputField>();
            so.FindProperty("inputRegEmail").objectReferenceValue = formReg.transform.Find("Input_RegEmail").GetComponent<TMP_InputField>();
            so.FindProperty("inputRegPass").objectReferenceValue = formReg.transform.Find("Input_RegPass").GetComponent<TMP_InputField>();
            so.FindProperty("inputRegPass2").objectReferenceValue = formReg.transform.Find("Input_RegPass2").GetComponent<TMP_InputField>();
            so.FindProperty("inputRecEmail").objectReferenceValue = formRec.transform.Find("Input_RecEmail").GetComponent<TMP_InputField>();
            so.FindProperty("btnLoginAccount").objectReferenceValue = btnL;
            so.FindProperty("btnRegisterAccount").objectReferenceValue = formReg.transform.Find("Btn_Register").GetComponent<Button>();
            so.FindProperty("btnRecoverAccount").objectReferenceValue = formRec.transform.Find("Btn_Recover").GetComponent<Button>();
            so.FindProperty("formStatus").objectReferenceValue = fsTmp;
            so.ApplyModifiedProperties();

            paTf.gameObject.SetActive(false);
            EditorSceneManager.MarkSceneDirty(paTf.gameObject.scene);
            Debug.Log("Login Account panel rebuilt.");
        }
    }
}
#endif
