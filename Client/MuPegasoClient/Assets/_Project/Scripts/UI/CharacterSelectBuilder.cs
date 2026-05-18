using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MuPegaso.Client.UI
{
    public class CharacterSelectBuilder : MonoBehaviour
    {
        static readonly string[] ClassNames = {
            "Dark Knight", "Dark Wizard", "Fairy Elf",
            "Magic Gladiator", "Dark Lord", "Summoner",
            "Rage Fighter", "Grow Lancer", "Illusion Knight"
        };

        static readonly string[] FbxPaths = {
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace02/SKM_NewFace02.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace01/SKM_NewFace01.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace03/SKM_NewFace03.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace04/SKM_NewFace04.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace05/SKM_NewFace05.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace06/SKM_NewFace06.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace07/SKM_NewFace07.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace08/SKM_NewFace08.fbx",
            "Assets/Art/Characters/ClassModels/_Fbx_NewFace14/SKM_NewFace14.fbx",
        };

        static readonly Color Gold = new(1f, 0.85f, 0.35f, 1f);
        static readonly Color BtnSelected = new(0.55f, 0.27f, 0f, 1f);
        static readonly Color BtnNormal = new(0.1f, 0.1f, 0.14f, 0.92f);
        static readonly Color PanelBg = new(0f, 0f, 0f, 0.75f);

        void Awake() => Build();

        void Build()
        {
            // ── PreviewCamera (no tocar Main Camera) ──
            var previewCamGo = new GameObject("PreviewCamera");
            var previewCam = previewCamGo.AddComponent<Camera>();
            previewCam.clearFlags = CameraClearFlags.SolidColor;
            previewCam.backgroundColor = Color.black;
            previewCam.fieldOfView = 35f;
            previewCamGo.transform.position = new Vector3(0f, 1.2f, -1.5f);
            previewCamGo.transform.LookAt(new Vector3(0f, 1f, 2f));

            RenderTexture rt;
#if UNITY_EDITOR
            rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(
                "Assets/Art/UI/CharacterPreviewRT.renderTexture");
            if (rt == null)
            {
                rt = new RenderTexture(512, 768, 24);
                rt.name = "CharacterPreviewRT";
                AssetDatabase.CreateAsset(rt,
                    "Assets/Art/UI/CharacterPreviewRT.renderTexture");
                AssetDatabase.SaveAssets();
            }
#else
            rt = new RenderTexture(512, 768, 24);
            rt.Create();
#endif
            rt.antiAliasing = 1;

            previewCam.targetTexture = rt;
            previewCam.depth = -1;
            previewCam.cullingMask = -1;

            // ── Canvas ──
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            canvasGo.AddComponent<GraphicRaycaster>();
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            var ct = canvas.transform;

            var previewImg = MkRaw("PreviewImage", ct, Color.white);
            previewImg.texture = rt;
            previewImg.raycastTarget = false;
            SetAnchors(previewImg.rectTransform, 0f, 0f, 0.55f, 1f);

            // Derecha 55–100%: panel oscuro
            var rightPanel = MkImg("RightPanel", ct, PanelBg);
            rightPanel.raycastTarget = false;
            SetAnchors(rightPanel.rectTransform, 0.55f, 0.12f, 1f, 1f);

            var sep = MkImg("Separator", ct, new Color(0.75f, 0.5f, 0.12f, 0.85f));
            sep.raycastTarget = false;
            SetAnchors(sep.rectTransform, 0.548f, 0.12f, 0.552f, 1f);

            var classNameTmp = MkTMP("ClassName", ct, "DARK KNIGHT", 26f, Gold, FontStyles.Bold);
            classNameTmp.alignment = TextAlignmentOptions.Left;
            SetAnchors(classNameTmp.rectTransform, 0.57f, 0.88f, 0.98f, 0.96f);

            string[] statLabels = { "Fuerza", "Agilidad", "Vitalidad", "Energía" };
            var statValues = new TextMeshProUGUI[4];
            for (int i = 0; i < 4; i++)
            {
                var lbl = MkTMP($"StatLbl{i}", ct, statLabels[i], 16f,
                    new Color(0.75f, 0.75f, 0.75f, 1f), FontStyles.Normal);
                lbl.alignment = TextAlignmentOptions.Left;
                SetAnchors(lbl.rectTransform, 0.57f, 0.78f - i * 0.05f, 0.72f, 0.83f - i * 0.05f);

                statValues[i] = MkTMP($"StatVal{i}", ct, "0", 16f, Gold, FontStyles.Bold);
                statValues[i].alignment = TextAlignmentOptions.Right;
                SetAnchors(statValues[i].rectTransform, 0.72f, 0.78f - i * 0.05f, 0.82f, 0.83f - i * 0.05f);
            }

            var statDiv = MkImg("StatDiv", ct, new Color(0.6f, 0.4f, 0.1f, 0.65f));
            statDiv.raycastTarget = false;
            SetAnchors(statDiv.rectTransform, 0.57f, 0.56f, 0.98f, 0.562f);

            var classBtns = new Button[9];
            for (int i = 0; i < 9; i++)
            {
                classBtns[i] = MkBtn($"BtnClass{i}", ct, ClassNames[i], 13f);
                float yMax = 0.54f - i * 0.048f;
                float yMin = yMax - 0.044f;
                SetAnchors(classBtns[i].GetComponent<RectTransform>(), 0.57f, yMin, 0.98f, yMax);
                classBtns[i].GetComponent<Image>().color = i == 0 ? BtnSelected : BtnNormal;
            }

            // Barra inferior 0–12%
            var bottomBar = MkImg("BottomBar", ct, new Color(0f, 0f, 0f, 0.9f));
            bottomBar.raycastTarget = false;
            SetAnchors(bottomBar.rectTransform, 0f, 0f, 1f, 0.12f);

            var descTmp = MkTMP("Desc", bottomBar.transform, "", 12f,
                new Color(0.78f, 0.78f, 0.78f, 1f), FontStyles.Italic);
            descTmp.enableWordWrapping = true;
            descTmp.alignment = TextAlignmentOptions.Left;
            SetAnchors(descTmp.rectTransform, 0.02f, 0.15f, 0.42f, 0.88f);

            var nameLbl = MkTMP("NombreLbl", bottomBar.transform, "NOMBRE:", 13f,
                new Color(0.85f, 0.7f, 0.25f, 1f), FontStyles.Bold);
            nameLbl.alignment = TextAlignmentOptions.Left;
            SetAnchors(nameLbl.rectTransform, 0.44f, 0.55f, 0.52f, 0.88f);

            var inputGo = new GameObject("InputName");
            inputGo.transform.SetParent(bottomBar.transform, false);
            inputGo.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            SetAnchors(inputGo.GetComponent<RectTransform>(), 0.52f, 0.2f, 0.72f, 0.82f);

            var textAreaGo = new GameObject("TextArea");
            textAreaGo.transform.SetParent(inputGo.transform, false);
            var taRT = textAreaGo.AddComponent<RectTransform>();
            Stretch(taRT);
            taRT.offsetMin = new Vector2(8f, 4f);
            taRT.offsetMax = new Vector2(-8f, -4f);
            var inputTMP = textAreaGo.AddComponent<TextMeshProUGUI>();
            inputTMP.fontSize = 15f;
            inputTMP.color = Color.white;
            var inputField = inputGo.AddComponent<TMP_InputField>();
            inputField.textComponent = inputTMP;
            inputField.textViewport = taRT;

            var btnCreate = MkBtn("BtnCreate", bottomBar.transform, "CREAR", 14f);
            btnCreate.GetComponent<Image>().color = BtnSelected;
            SetAnchors(btnCreate.GetComponent<RectTransform>(), 0.74f, 0.18f, 0.88f, 0.84f);

            var btnBack = MkBtn("BtnBack", bottomBar.transform, "VOLVER", 13f);
            btnBack.GetComponent<Image>().color = BtnNormal;
            SetAnchors(btnBack.GetComponent<RectTransform>(), 0.89f, 0.18f, 0.99f, 0.84f);

            UIEventSystemHelper.Ensure();

            var controller = GetComponent<CharacterSelectController>();
            if (controller == null)
                controller = gameObject.AddComponent<CharacterSelectController>();

            var models = new GameObject[9];
#if UNITY_EDITOR
            for (int i = 0; i < 9; i++)
                models[i] = AssetDatabase.LoadAssetAtPath<GameObject>(FbxPaths[i]);
#endif

            controller.previewCamera = previewCam;
            controller.previewImage = previewImg;
            controller.classModels = models;
            controller.className = classNameTmp;
            controller.classDescription = descTmp;
            controller.statStrength = statValues[0];
            controller.statAgility = statValues[1];
            controller.statVitality = statValues[2];
            controller.statEnergy = statValues[3];
            controller.characterNameInput = inputField;
            controller.classButtons = classBtns;
            controller.btnCreate = btnCreate;
            controller.btnBack = btnBack;

            for (int i = 0; i < 9; i++)
            {
                int idx = i;
                classBtns[i].onClick.AddListener(() => controller.SelectClass(idx));
            }
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        static void SetAnchors(RectTransform rt, float xMin, float yMin, float xMax, float yMax)
        {
            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        static Image MkImg(string n, Transform p, Color c)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            var img = go.AddComponent<Image>();
            img.color = c;
            return img;
        }

        static RawImage MkRaw(string n, Transform p, Color c)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            var ri = go.AddComponent<RawImage>();
            ri.color = c;
            return ri;
        }

        static TextMeshProUGUI MkTMP(string n, Transform p, string txt,
            float size, Color c, FontStyles style)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = txt;
            tmp.fontSize = size;
            tmp.color = c;
            tmp.fontStyle = style;
            return tmp;
        }

        static Button MkBtn(string n, Transform p, string txt, float fontSize)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            go.AddComponent<Image>().color = BtnNormal;
            var btn = go.AddComponent<Button>();
            var tGo = new GameObject("Text");
            tGo.transform.SetParent(go.transform, false);
            var tmp = tGo.AddComponent<TextMeshProUGUI>();
            tmp.text = txt;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            Stretch(tmp.rectTransform);
            return btn;
        }
    }
}
