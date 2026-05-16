using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using MuPegaso.Client.Data;

namespace MuPegaso.Client.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class SelectServerController : MonoBehaviour
    {
        static readonly int[] RecentServerIds = { 21, 7 };

        [SerializeField] private UIDocument uiDocument;
        [Tooltip("Opcional: si el UIDocument no tiene Source Asset, se asigna aquí (ej. SelectServer.uxml).")]
        [SerializeField] private VisualTreeAsset uiTreeAsset;
        [SerializeField] private List<ServerData> servers = new List<ServerData>();
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private string closeSceneName = "Login";

        private int selectedServerId = -1;
        private int currentGroup = 1;
        private bool _firstGridRender = true;

        private readonly Dictionary<int, VisualElement> _cards = new Dictionary<int, VisualElement>();

        private VisualElement _panel;
        private VisualElement _serverGrid;
        private VisualElement _particlesHost;
        private Label _footerText;
        private Button _connectBtn;
        private Button _btnRecent;
        private Button _btnG1;
        private Button _btnG2;
        private Button _btnG3;
        private Button _closeBtn;

        private IVisualElementScheduledItem _particleTicker;

        private struct Particle
        {
            public VisualElement Element;
            public Vector2 Pos;
            public float Vy;
        }

        private readonly List<Particle> _particles = new List<Particle>();

        private void Awake()
        {
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();

            if (uiDocument.visualTreeAsset == null && uiTreeAsset != null)
                uiDocument.visualTreeAsset = uiTreeAsset;

            if (servers == null || servers.Count == 0)
                servers = ServerDataCatalog.BuildDefault();

            EnsurePanelSettings();
        }

        private void EnsurePanelSettings()
        {
            if (panelSettingsAsset != null)
            {
                uiDocument.panelSettings = panelSettingsAsset;
                return;
            }

            if (uiDocument.panelSettings != null)
                return;

            var ps = ScriptableObject.CreateInstance<PanelSettings>();
            ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            ps.referenceResolution = new Vector2Int(1920, 1080);
            ps.screenMatchMode = ScreenMatchMode.MatchWidthOrHeight;
            ps.match = 0.5f;
            uiDocument.panelSettings = ps;
        }

        private void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            _panel = root.Q<VisualElement>("panel");
            _particlesHost = root.Q<VisualElement>("particles-host");
            _serverGrid = root.Q<VisualElement>("server-grid");
            _footerText = root.Q<Label>("footer-text");
            _connectBtn = root.Q<Button>("connect-btn");
            _btnRecent = root.Q<Button>("btn-recent");
            _btnG1 = root.Q<Button>("btn-group-1");
            _btnG2 = root.Q<Button>("btn-group-2");
            _btnG3 = root.Q<Button>("btn-group-3");
            _closeBtn = root.Q<Button>("close-btn");

            _btnRecent.clicked += OnRecentClicked;
            _btnG1.clicked += OnGroup1Clicked;
            _btnG2.clicked += OnGroup2Clicked;
            _btnG3.clicked += OnGroup3Clicked;
            _connectBtn.clicked += OnConnectClicked;
            _closeBtn.clicked += OnCloseClicked;

            if (_footerText != null)
                _footerText.enableRichText = true;

            _connectBtn.SetEnabled(false);
            RefreshFooter();

            PlayPanelIntro();
            ScheduleParticles();

            currentGroup = 1;
            UpdateSidebarHighlight();
            SwitchGroup(1);
        }

        private void OnDisable()
        {
            _particleTicker?.Pause();
            _particleTicker = null;
            _particles.Clear();

            if (_btnRecent != null) _btnRecent.clicked -= OnRecentClicked;
            if (_btnG1 != null) _btnG1.clicked -= OnGroup1Clicked;
            if (_btnG2 != null) _btnG2.clicked -= OnGroup2Clicked;
            if (_btnG3 != null) _btnG3.clicked -= OnGroup3Clicked;
            if (_connectBtn != null) _connectBtn.clicked -= OnConnectClicked;
            if (_closeBtn != null) _closeBtn.clicked -= OnCloseClicked;

            _cards.Clear();
            _firstGridRender = true;
        }

        private void PlayPanelIntro()
        {
            if (_panel == null) return;

            _panel.style.opacity = 0f;
            _panel.style.translate = new Translate(
                new Length(0, LengthUnit.Pixel),
                new Length(24, LengthUnit.Pixel),
                0);

            _panel.schedule.Execute(() =>
            {
                _panel.style.opacity = 1f;
                _panel.style.translate = new Translate(0, 0, 0);
            }).StartingIn(32);
        }

        private void ScheduleParticles()
        {
            if (_particlesHost == null) return;

            _particlesHost.schedule.Execute(InitParticlesIfNeeded).StartingIn(120);

            _particleTicker?.Pause();
            _particleTicker = _particlesHost.schedule.Execute(TickParticles).Every(16);
        }

        private void InitParticlesIfNeeded()
        {
            if (_particlesHost == null) return;

            _particlesHost.Clear();
            _particles.Clear();

            var w = _particlesHost.resolvedStyle.width;
            var h = _particlesHost.resolvedStyle.height;
            if (w < 16f || float.IsNaN(w)) w = Screen.width;
            if (h < 16f || float.IsNaN(h)) h = Screen.height;

            for (var i = 0; i < 18; i++)
            {
                var el = new VisualElement();
                el.AddToClassList("particle-dot");
                var size = Random.Range(1f, 3f);
                el.style.width = size;
                el.style.height = size;

                var p = new Particle
                {
                    Element = el,
                    Pos = new Vector2(Random.Range(0f, w), Random.Range(0f, h)),
                    Vy = Random.Range(18f, 55f)
                };

                el.style.left = p.Pos.x;
                el.style.top = p.Pos.y;

                _particlesHost.Add(el);
                _particles.Add(p);
            }
        }

        private void TickParticles()
        {
            if (_particlesHost == null || _particles.Count == 0) return;

            var h = _particlesHost.resolvedStyle.height;
            if (h < 16f || float.IsNaN(h)) h = Screen.height;

            var dt = Time.deltaTime;

            for (var i = 0; i < _particles.Count; i++)
            {
                var p = _particles[i];
                p.Pos.y -= p.Vy * dt;
                if (p.Pos.y < -10f)
                    p.Pos.y = h + Random.Range(0f, 48f);

                p.Element.style.left = p.Pos.x;
                p.Element.style.top = p.Pos.y;
                _particles[i] = p;
            }
        }

        private void OnRecentClicked() => SwitchGroup(0);
        private void OnGroup1Clicked() => SwitchGroup(1);
        private void OnGroup2Clicked() => SwitchGroup(2);
        private void OnGroup3Clicked() => SwitchGroup(3);

        private void SwitchGroup(int groupId)
        {
            currentGroup = groupId;
            ClearSelection();
            UpdateSidebarHighlight();

            if (_firstGridRender)
            {
                _firstGridRender = false;
                RenderGroup(groupId);
                return;
            }

            if (_serverGrid == null) return;

            _serverGrid.style.opacity = 0f;
            _serverGrid.style.translate = new Translate(
                new Length(0, LengthUnit.Pixel),
                new Length(-8, LengthUnit.Pixel),
                0);

            _serverGrid.schedule.Execute(() =>
            {
                RenderGroup(groupId);
                _serverGrid.style.translate = new Translate(0, 0, 0);
                _serverGrid.style.opacity = 1f;
            }).StartingIn(150);
        }

        private void RenderGroup(int groupId)
        {
            if (_serverGrid == null) return;

            _serverGrid.Clear();
            _cards.Clear();

            if (groupId == 0)
            {
                foreach (var rid in RecentServerIds)
                {
                    var s = servers.Find(x => x.id == rid);
                    if (s != null)
                        _serverGrid.Add(CreateServerCard(s));
                }

                return;
            }

            foreach (var s in servers)
            {
                if (s.groupId == groupId)
                    _serverGrid.Add(CreateServerCard(s));
            }
        }

        private VisualElement CreateServerCard(ServerData server)
        {
            var card = new VisualElement();
            card.AddToClassList("server-card");

            var leftBar = new VisualElement();
            leftBar.AddToClassList("card-left-bar");
            card.Add(leftBar);

            if (server.isNew)
            {
                var badgeWrap = new VisualElement();
                badgeWrap.AddToClassList("badge-new-wrap");
                var badgeBg = new VisualElement();
                badgeBg.AddToClassList("badge-new-bg");
                var badgeTxt = new Label("NUEVO");
                badgeTxt.AddToClassList("badge-new-text");
                badgeBg.Add(badgeTxt);

                var shimmer = new VisualElement();
                shimmer.AddToClassList("badge-shimmer-layer");
                if (Random.value > 0.35f)
                    shimmer.AddToClassList("shimmer-delay");

                badgeWrap.Add(badgeBg);
                badgeWrap.Add(shimmer);
                card.Add(badgeWrap);
            }

            var inner = new VisualElement();
            inner.AddToClassList("server-card-inner");

            var dot = new VisualElement();
            dot.AddToClassList("card-dot");
            ApplyStatusToDot(dot, server.status);

            var nameLabel = new Label(server.name);
            nameLabel.AddToClassList("server-card-name");

            var statusLbl = new Label(StatusToSpanish(server.status));
            statusLbl.AddToClassList("server-status-label");
            statusLbl.AddToClassList(StatusToClass(server.status));

            inner.Add(dot);
            inner.Add(nameLabel);
            inner.Add(statusLbl);
            card.Add(inner);

            _cards[server.id] = card;

            if (server.status == ServerStatus.Maintenance)
            {
                card.AddToClassList("server-card--maintenance");
                card.pickingMode = PickingMode.Ignore;
            }
            else
            {
                var id = server.id;
                card.RegisterCallback<ClickEvent>(_ => ToggleSelectServer(id));
            }

            return card;
        }

        private static void ApplyStatusToDot(VisualElement dot, ServerStatus status)
        {
            dot.RemoveFromClassList("dot-green");
            dot.RemoveFromClassList("dot-red");
            dot.RemoveFromClassList("dot-yellow");
            dot.RemoveFromClassList("dot-gray");
            dot.RemoveFromClassList("dot-pulse");

            switch (status)
            {
                case ServerStatus.Available:
                    dot.AddToClassList("dot-green");
                    dot.AddToClassList("dot-pulse");
                    break;
                case ServerStatus.Full:
                    dot.AddToClassList("dot-red");
                    dot.AddToClassList("dot-pulse");
                    break;
                case ServerStatus.Saturated:
                    dot.AddToClassList("dot-yellow");
                    dot.AddToClassList("dot-pulse");
                    break;
                default:
                    dot.AddToClassList("dot-gray");
                    break;
            }
        }

        private static string StatusToSpanish(ServerStatus s)
        {
            switch (s)
            {
                case ServerStatus.Available: return "Disponible";
                case ServerStatus.Full: return "Lleno";
                case ServerStatus.Saturated: return "Saturado";
                default: return "Mantenimiento";
            }
        }

        private static string StatusToClass(ServerStatus s)
        {
            switch (s)
            {
                case ServerStatus.Available: return "status-available";
                case ServerStatus.Full: return "status-full";
                case ServerStatus.Saturated: return "status-saturated";
                default: return "status-maintenance";
            }
        }

        private void ToggleSelectServer(int id)
        {
            if (selectedServerId == id)
                selectedServerId = -1;
            else
                selectedServerId = id;

            RefreshCardSelectionStyles();
            RefreshFooter();
        }

        private void ClearSelection()
        {
            selectedServerId = -1;
            RefreshCardSelectionStyles();
            RefreshFooter();
        }

        private void RefreshCardSelectionStyles()
        {
            foreach (var kv in _cards)
            {
                if (kv.Key == selectedServerId)
                    kv.Value.AddToClassList("selected");
                else
                    kv.Value.RemoveFromClassList("selected");
            }
        }

        private void RefreshFooter()
        {
            if (_footerText == null || _connectBtn == null) return;

            if (selectedServerId < 0)
            {
                _footerText.text = "Servidor seleccionado: <color=#94A3B8>—</color>";
                _connectBtn.SetEnabled(false);
                return;
            }

            var data = servers.Find(s => s.id == selectedServerId);
            var nm = data != null ? data.name : "—";
            _footerText.text = $"Servidor seleccionado: <color=#60A5FA>{nm}</color>";
            _connectBtn.SetEnabled(true);
        }

        private void UpdateSidebarHighlight()
        {
            SetSidebarActive(_btnRecent, currentGroup == 0);
            SetSidebarActive(_btnG1, currentGroup == 1);
            SetSidebarActive(_btnG2, currentGroup == 2);
            SetSidebarActive(_btnG3, currentGroup == 3);
        }

        private static void SetSidebarActive(Button btn, bool active)
        {
            if (btn == null) return;
            if (active)
                btn.AddToClassList("sidebar-btn-active");
            else
                btn.RemoveFromClassList("sidebar-btn-active");
        }

        private void OnConnectClicked()
        {
            var data = servers.Find(s => s.id == selectedServerId);
            if (data == null) return;

            if (!string.IsNullOrEmpty(gameSceneName))
                SceneManager.LoadScene(gameSceneName);
        }

        private void OnCloseClicked()
        {
            if (!string.IsNullOrEmpty(closeSceneName))
                SceneManager.LoadScene(closeSceneName);
            else
            {
                var idx = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(Mathf.Max(0, idx - 1));
            }
        }
    }
}
