using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MuPegaso.Client.UI
{
    public class LoadingController : MonoBehaviour
    {
        [Header("Escena destino")]
        [SerializeField] private string targetScene = "CharacterSelect";

        [Header("Backgrounds")]
        [SerializeField] private Texture2D bgDK;
        [SerializeField] private Texture2D bgPJMU;

        [Header("Assets UI")]
        [SerializeField] private Sprite logoSprite;
        [SerializeField] private Sprite barFrameSprite;
        [SerializeField] private Sprite barFillSprite;

        static readonly string[] Tips =
        {
            "Los Caballeros Oscuros dominan el combate cuerpo a cuerpo con fuerza brutal",
            "Las Elves pueden convocar criaturas para luchar a su lado",
            "El Mago de las Almas domina las artes arcanas más poderosas",
            "Lorencia es la ciudad principal y punto de encuentro de héroes",
            "El Caos Castillo se abre cada hora para los más valientes",
            "Combina runas para crear equipamiento legendario",
            "Los Mu Ancient Items otorgan poderes únicos a sus portadores"
        };

        RawImage _bg;
        Image _barFill;
        Image _barGlow;
        TextMeshProUGUI _percentText;
        TextMeshProUGUI _tipText;
        CanvasGroup _tipGroup;
        AsyncOperation _op;
        float _progress;

        void Awake()
        {
            // EventSystem
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            // Cargar texturas en editor
#if UNITY_EDITOR
            if (bgDK == null)
                bgDK = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/UI/DK_loading.png");
            if (bgPJMU == null)
                bgPJMU = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/UI/PJMU_Loading.png");
            if (logoSprite == null)
                logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/Splash/logo_mupegaso.png");
            if (barFillSprite == null)
                barFillSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/loading_bar_sprite.png");
#endif
            BuildUI();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (barFrameSprite == null)
                barFrameSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/barra_BG.png");

            var frameImg = FindFirstObjectByType<Canvas>()
                ?.transform.Find("BarFrame")?.GetComponent<Image>();
            if (frameImg != null && barFrameSprite != null)
                frameImg.sprite = barFrameSprite;
#endif

            ApplyRandomBackground();

            _op = SceneManager.LoadSceneAsync(targetScene);
            if (_op != null) _op.allowSceneActivation = false;

            StartCoroutine(TipLoop());
        }

        void ApplyRandomBackground()
        {
            var tex = Random.Range(0, 2) == 0 ? bgDK : bgPJMU;
            if (tex == null) tex = bgDK != null ? bgDK : bgPJMU;
            if (_bg != null && tex != null)
            {
                _bg.texture = tex;
                _bg.uvRect = new Rect(0, 0, 1, 1);
                _bg.color = Color.white;
                Debug.Log($"[Loading] Fondo aplicado: {tex.name}");
            }
            else
            {
                Debug.LogWarning($"[Loading] Fondo NULL - bg:{_bg != null} tex:{tex != null} dk:{bgDK != null} pjmu:{bgPJMU != null}");
            }
        }

        void Update()
        {
            if (_op == null) return;

            float target = Mathf.Clamp01(_op.progress / 0.9f);
            _progress = Mathf.MoveTowards(_progress, target, Time.deltaTime * 0.6f);

            if (_op.progress >= 0.9f)
            {
                _progress = Mathf.MoveTowards(_progress, 1f, Time.deltaTime * 1.5f);
                if (_progress >= 0.99f)
                    _op.allowSceneActivation = true;
            }

            if (_barFill != null) _barFill.fillAmount = _progress;
            if (_percentText != null) _percentText.text = Mathf.RoundToInt(_progress * 100f) + "%";

            if (_barGlow != null)
            {
                _barGlow.fillAmount = _progress;
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 4f);
                _barGlow.color = new Color(1f, 0.8f, 0.2f, 0.12f + 0.1f * pulse);
            }
        }

        void BuildUI()
        {
            // Usar canvas existente o crear uno nuevo
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasGo = new GameObject("Canvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvasGo.AddComponent<GraphicRaycaster>();
                var scaler = canvasGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var canvasScaler = canvas.GetComponent<CanvasScaler>();
            if (canvasScaler == null)
                canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasScaler.matchWidthOrHeight = 0.5f;

            // Limpiar hijos existentes del canvas
            for (int i = canvas.transform.childCount - 1; i >= 0; i--)
                Destroy(canvas.transform.GetChild(i).gameObject);

            Transform ct = canvas.transform;

            // 1. Fondo fullscreen
            _bg = MakeRawImage("BG", ct, Color.black);
            Stretch(_bg.rectTransform);
            _bg.raycastTarget = false;

            // 2. Gradiente oscuro inferior
            var grad = MakeImage("Gradient", ct, Color.white);
            grad.sprite = BuildGradientSprite();
            grad.type = Image.Type.Simple;
            grad.raycastTarget = false;
            var grt = grad.rectTransform;
            grt.anchorMin = new Vector2(0f, 0f);
            grt.anchorMax = new Vector2(1f, 0.5f);
            grt.offsetMin = grt.offsetMax = Vector2.zero;
            grt.pivot = new Vector2(0.5f, 0f);

            // 3. Logo
            var logo = MakeImage("Logo", ct, Color.white);
            logo.sprite = logoSprite;
            logo.preserveAspect = true;
            logo.raycastTarget = false;
            Center(logo.rectTransform, 0f, -120f, 400f, 140f);

            // 4. Label CARGANDO
            var lbl = MakeTMP("LblCargando", ct, "CARGANDO...", 12f, 
                new Color(0.85f, 0.85f, 0.85f, 0.9f), FontStyles.Normal);
            lbl.characterSpacing = 4f;
            Center(lbl.rectTransform, 0f, -175f, 500f, 26f);

            // 5. Barra — fondo oscuro interior (debajo, Z-order primero)
            var barBG = MakeImage("BarBG", ct, new Color(0.03f, 0.03f, 0.05f, 0.95f));
            barBG.raycastTarget = false;
            Center(barBG.rectTransform, 0f, -207.9f, 320f, 16f);

            // 6. Barra fill
            _barFill = MakeImage("BarFill", ct, new Color(1f, 0.7f, 0f, 1f));
            _barFill.raycastTarget = false;
            _barFill.type = Image.Type.Filled;
            _barFill.fillMethod = Image.FillMethod.Horizontal;
            _barFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            _barFill.fillAmount = 0f;
            _barFill.preserveAspect = false;
            if (barFillSprite != null)
            {
                _barFill.sprite = barFillSprite;
                _barFill.color = Color.white;
            }
            Center(_barFill.rectTransform, 0f, -207.9f, 320f, 16f);

            // 7. Glow pulsante
            _barGlow = MakeImage("BarGlow", ct, new Color(1f, 0.8f, 0.2f, 0.15f));
            _barGlow.raycastTarget = false;
            _barGlow.type = Image.Type.Filled;
            _barGlow.fillMethod = Image.FillMethod.Horizontal;
            _barGlow.fillOrigin = (int)Image.OriginHorizontal.Left;
            _barGlow.fillAmount = 0f;
            Center(_barGlow.rectTransform, 0f, -207.9f, 320f, 16f);

            // 8. Marco del dragón (encima de todo, Z-order último)
            var barFrame = MakeImage("BarFrame", ct, Color.white);
            barFrame.sprite = barFrameSprite;
            barFrame.color = new Color(1f, 1f, 1f, 1f);
            barFrame.type = Image.Type.Simple;
            barFrame.preserveAspect = true;
            barFrame.raycastTarget = false;
            Center(barFrame.rectTransform, 0f, -210f, 820f, 70f);

            // 9. Porcentaje
            _percentText = MakeTMP("PctText", ct, "0%", 16f, 
                new Color(1f, 0.84f, 0f, 1f), FontStyles.Bold);
            Center(_percentText.rectTransform, 0f, -245f, 120f, 28f);

            // 10. Tip
            var tipGo = new GameObject("TipText");
            tipGo.transform.SetParent(ct, false);
            _tipText = tipGo.AddComponent<TextMeshProUGUI>();
            _tipText.fontSize = 14f;
            _tipText.fontStyle = FontStyles.Italic;
            _tipText.color = new Color(0.82f, 0.85f, 0.9f, 0.95f);
            _tipText.alignment = TextAlignmentOptions.Center;
            _tipText.enableWordWrapping = true;
            _tipText.text = Tips[Random.Range(0, Tips.Length)];
            _tipGroup = tipGo.AddComponent<CanvasGroup>();
            Center(tipGo.GetComponent<RectTransform>(), 0f, -272f, 900f, 44f);
        }

        // ── Helpers ──────────────────────────────────────

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }

        static void Center(RectTransform rt, float x, float y, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(w, h);
        }

        static RawImage MakeRawImage(string n, Transform p, Color c)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            var ri = go.AddComponent<RawImage>();
            ri.color = c;
            return ri;
        }

        static Image MakeImage(string n, Transform p, Color c)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            var img = go.AddComponent<Image>();
            img.color = c;
            return img;
        }

        static TextMeshProUGUI MakeTMP(string n, Transform p, string txt, 
            float size, Color c, FontStyles style)
        {
            var go = new GameObject(n);
            go.transform.SetParent(p, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = txt;
            tmp.fontSize = size;
            tmp.color = c;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = true;
            tmp.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            return tmp;
        }

        static Sprite BuildGradientSprite()
        {
            const int h = 64;
            var tex = new Texture2D(1, h, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < h; y++)
                tex.SetPixel(0, y, new Color(0f, 0f, 0f, (1f - (float)y / h) * 0.85f));
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, h), new Vector2(0.5f, 0f), 100f);
        }

        IEnumerator TipLoop()
        {
            if (_tipText == null || _tipGroup == null) yield break;
            int i = Random.Range(0, Tips.Length);
            _tipText.text = Tips[i];
            _tipGroup.alpha = 1f;

            while (true)
            {
                yield return new WaitForSeconds(3.5f);

                for (float t = 0; t < 0.3f; t += Time.deltaTime)
                {
                    _tipGroup.alpha = 1f - t / 0.3f;
                    yield return null;
                }
                i = (i + 1) % Tips.Length;
                _tipText.text = Tips[i];
                for (float t = 0; t < 0.3f; t += Time.deltaTime)
                {
                    _tipGroup.alpha = t / 0.3f;
                    yield return null;
                }
                _tipGroup.alpha = 1f;
            }
        }
    }
}
