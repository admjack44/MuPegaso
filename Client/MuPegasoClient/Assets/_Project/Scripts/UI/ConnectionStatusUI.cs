using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using MuPegaso.Client.Network;

namespace MuPegaso.Client.UI
{
    public class ConnectionStatusUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statusText;

        void Start()
        {
            if (NetworkClient.Instance == null) return;
            NetworkClient.Instance.OnConnected    += () => statusText.text = "Conectado al servidor";
            NetworkClient.Instance.OnDisconnected += r  => statusText.text = "Error: " + r;
            NetworkClient.Instance.OnPongReceived += ms => statusText.text = "Online - " + ms.ToString("F0") + "ms";
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
                NetworkClient.Instance?.SendPing();
        }
    }
}
