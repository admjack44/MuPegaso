using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MuPegaso.Client.UI
{
    public class CharacterSelectController : MonoBehaviour
    {
        [Header("Escenas")]
        [SerializeField] public string gameScene = "Game";
        [SerializeField] public string backScene = "SelectServer";

        [Header("Cámara preview")]
        public Camera previewCamera;
        public RawImage previewImage;

        [Header("Modelos")]
        [SerializeField] public GameObject[] classModels;

        [Header("UI")]
        [SerializeField] public Button[] classButtons;
        [SerializeField] public TextMeshProUGUI className;
        [SerializeField] public TextMeshProUGUI classDescription;
        [SerializeField] public TextMeshProUGUI statStrength;
        [SerializeField] public TextMeshProUGUI statAgility;
        [SerializeField] public TextMeshProUGUI statVitality;
        [SerializeField] public TextMeshProUGUI statEnergy;
        [SerializeField] public TMP_InputField characterNameInput;
        [SerializeField] public Button btnCreate;
        [SerializeField] public Button btnBack;

        static readonly string[] ClassNames = {
            "Dark Knight", "Dark Wizard", "Fairy Elf",
            "Magic Gladiator", "Dark Lord", "Summoner",
            "Rage Fighter", "Grow Lancer", "Illusion Knight"
        };

        static readonly string[] ClassDescriptions = {
            "Maestro del combate cuerpo a cuerpo. Alta vitalidad y fuerza devastadora.",
            "Domina las artes arcanas más poderosas. Baja defensa pero magia letal.",
            "Experta en arco y habilidades de apoyo. Velocidad y precisión incomparables.",
            "Combina espada y magia en perfecta armonía. Versátil y poderoso.",
            "Líder nato con habilidades de comando. Potencia a sus aliados en batalla.",
            "Invocadora de criaturas oscuras. Control del campo de batalla.",
            "Luchador sin armas. Fuerza bruta y velocidad de reacción extrema.",
            "Portador de la lanza sagrada. Ataques en área devastadores.",
            "Caballero de las ilusiones. Engaña y destruye a sus enemigos."
        };

        static readonly int[,] ClassStats = {
            { 28, 20, 25, 10 },
            { 15, 20, 15, 30 },
            { 15, 25, 20, 15 },
            { 26, 26, 26, 26 },
            { 26, 20, 20, 15 },
            { 15, 20, 15, 30 },
            { 32, 27, 25,  5 },
            { 25, 20, 23, 15 },
            { 20, 25, 20, 20 },
        };

        static readonly Color BtnSelected = new(0.55f, 0.27f, 0f, 1f);
        static readonly Color BtnNormal = new(0.1f, 0.1f, 0.14f, 0.92f);

        int _selectedClass;
        GameObject _currentModel;

        void Start()
        {
            if (btnCreate != null)
                btnCreate.onClick.AddListener(OnCreateClicked);
            if (btnBack != null)
                btnBack.onClick.AddListener(() => SceneManager.LoadScene(backScene));

            SelectClass(0);
        }

        public void SelectClass(int index)
        {
            if (index < 0 || index >= ClassNames.Length) return;
            _selectedClass = index;

            if (_currentModel != null)
            {
                Destroy(_currentModel);
                _currentModel = null;
            }

            if (classModels != null && index < classModels.Length && classModels[index] != null)
            {
                _currentModel = Instantiate(classModels[index]);
                _currentModel.transform.position = new Vector3(0f, 0f, 0f);
                _currentModel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                _currentModel.transform.localScale = Vector3.one;

                var smrs = _currentModel.GetComponentsInChildren<SkinnedMeshRenderer>();
                if (smrs.Length > 0)
                {
                    var bounds = smrs[0].bounds;
                    foreach (var r in smrs) bounds.Encapsulate(r.bounds);
                    float cy = bounds.center.y;
                    float h = bounds.size.y;

                    float bustY = cy + h * 0.1f;
                    if (Camera.main != null)
                    {
                        Camera.main.transform.position = new Vector3(0f, bustY, -2f);
                        Camera.main.transform.rotation = Quaternion.identity;
                        Camera.main.fieldOfView = 35f;
                        Camera.main.clearFlags = CameraClearFlags.SolidColor;
                        Camera.main.backgroundColor = new Color(0.04f, 0.04f, 0.08f);
                    }

                    Debug.Log($"Busto Y={bustY:F2} h={h:F2}");
                }
            }

            if (previewCamera != null)
                previewCamera.gameObject.SetActive(false);

            if (className != null)
                className.text = ClassNames[index].ToUpperInvariant();
            if (classDescription != null)
                classDescription.text = ClassDescriptions[index];
            if (statStrength != null) statStrength.text = ClassStats[index, 0].ToString();
            if (statAgility != null) statAgility.text = ClassStats[index, 1].ToString();
            if (statVitality != null) statVitality.text = ClassStats[index, 2].ToString();
            if (statEnergy != null) statEnergy.text = ClassStats[index, 3].ToString();

            if (classButtons != null)
            {
                for (int i = 0; i < classButtons.Length; i++)
                {
                    if (classButtons[i] == null) continue;
                    classButtons[i].GetComponent<Image>().color = i == index ? BtnSelected : BtnNormal;
                }
            }
        }

        void OnCreateClicked()
        {
            if (characterNameInput == null) return;
            string charName = characterNameInput.text.Trim();
            if (string.IsNullOrEmpty(charName))
            {
                Debug.LogWarning("[CharacterSelect] Nombre vacío");
                return;
            }
            Debug.Log($"[CharacterSelect] Crear: {charName} — {ClassNames[_selectedClass]}");
            SceneManager.LoadScene(gameScene);
        }
    }
}
