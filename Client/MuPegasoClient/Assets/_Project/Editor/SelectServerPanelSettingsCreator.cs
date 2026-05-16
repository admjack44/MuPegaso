#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MuPegaso.Client.Editor
{
    /// <summary>Crea un Panel Settings para UIDocument (evita advertencia y muestra UI en Editor).</summary>
    public static class SelectServerPanelSettingsCreator
    {
        const string AssetPath = "Assets/UI/SelectServerPanelSettings.asset";

        [MenuItem("Tools/MuPegaso/Create SelectServer Panel Settings")]
        public static void Create()
        {
            var existing = AssetDatabase.LoadAssetAtPath<PanelSettings>(AssetPath);
            if (existing != null)
            {
                Selection.activeObject = existing;
                Debug.Log("[MuPegaso] Panel Settings ya existe: " + AssetPath);
                return;
            }

            var ps = ScriptableObject.CreateInstance<PanelSettings>();
            ps.name = "SelectServerPanelSettings";
            ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            ps.referenceResolution = new Vector2Int(1920, 1080);
            ps.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            ps.match = 0.5f;

            AssetDatabase.CreateAsset(ps, AssetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = ps;
            Debug.Log("[MuPegaso] Creado " + AssetPath + " — asígnalo al UIDocument (Panel Settings).");
        }
    }
}
#endif
