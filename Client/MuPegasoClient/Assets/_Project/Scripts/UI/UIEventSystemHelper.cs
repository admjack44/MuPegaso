using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace MuPegaso.Client.UI
{
    /// <summary>
    /// Garantiza EventSystem compatible con Input System (evita spam de InvalidOperationException).
    /// </summary>
    public static class UIEventSystemHelper
    {
        public static EventSystem Ensure()
        {
            var es = Object.FindFirstObjectByType<EventSystem>();
            if (es == null)
            {
                var go = new GameObject("EventSystem");
                es = go.AddComponent<EventSystem>();
            }

            var legacy = es.GetComponent<StandaloneInputModule>();
            if (legacy != null)
                Object.Destroy(legacy);

            if (es.GetComponent<InputSystemUIInputModule>() == null)
                es.gameObject.AddComponent<InputSystemUIInputModule>();

            return es;
        }
    }
}
