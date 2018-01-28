#if ENABLE_UNET

using System.Net;
using System.Net.Sockets;

namespace UnityEngine.Networking
{
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class NetworkManagerHUD : MonoBehaviour
    {
        public NetworkManager manager;
        [SerializeField] public bool showGUI = true;
        [SerializeField] public int offsetX;
        [SerializeField] public int offsetY;

        public string IpAddress;
        public string Port;

        public int ButtonWidth = 100;
        public int ButtonHeight = 30;
        public int TextBoxWidth = 200;
        public int TextBoxHeight = 25;
        public int LabelWidth = 275;
        public int LabelHeight = 25;
        public int yLobbyOffset = 25;

        // Runtime variable
        bool showServer = false;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        private void Start()
        {
            TextMesh text = InGameManager.GetSingleton.textIp.GetComponent<TextMesh>(); 
            if(text)
            {
                text.text = FindIp();
            }
        }

        void Update()
        {
            if (!showGUI)
                return;

            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    manager.StartServer();
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    manager.StartHost();
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    manager.StartClient();
                }
            }
            if (NetworkServer.active && NetworkClient.active)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    manager.StopHost();
                }
            }
        }

        void OnGUI()
        {
            if (!showGUI)
                return;

            int xpos = 10 + offsetX;
            int ypos = 40 + offsetY;
            int spacing = 24;

            if (!NetworkClient.active && !NetworkServer.active && manager.matchMaker == null)
            {
                HostButton();

                IpAddress = GUI.TextField(new Rect((Screen.width - TextBoxWidth) / 2, Screen.height / 2 - 1.5f * TextBoxHeight + yLobbyOffset, TextBoxWidth, TextBoxHeight), IpAddress);
                //Port = GUI.TextField(new Rect((Screen.width - TextBoxWidth) / 2, Screen.height / 2 - 2.5f * TextBoxHeight + yLobbyOffset, TextBoxWidth, TextBoxHeight), Port, 5);
                Port = "55555";
                JoinButton();
                // if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Host"))
                //{
                //	manager.StartHost();
                //}
                ypos += spacing;

                //if (GUI.Button(new Rect(xpos, ypos, 105, 20), "Join"))
                //{
                //	manager.StartClient();
                //}

                //IpAddress = GUI.TextField(new Rect((Screen.width - TextBoxWidth) / 2, Screen.height / 2 - 1.5f * TextBoxHeight + yLobbyOffset, TextBoxWidth, TextBoxHeight), IpAddress);
                manager.networkAddress = IpAddress;
                //manager.networkAddress = GUI.TextField(new Rect(xpos + 100, ypos, 95, 20), IpAddress);
                ypos += spacing;

                //if (GUI.Button(new Rect(xpos, ypos, 200, 20), "LAN Server Only(S)"))
                //{
                //	manager.StartServer();
                //}
                ypos += spacing;
            }
            else
            {
                if (InGameManager.GetSingleton.State == GameState.Pause)
                {
                    if (NetworkServer.active)
                    {
                        GUI.Label(new Rect(xpos, ypos, 300, 20), "Server: port=" + manager.networkPort);
                        ypos += spacing;
                    }
                    if (NetworkClient.active)
                    {
                        GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                        ypos += spacing;
                    } 
                }
            }

            if ((NetworkServer.active || NetworkClient.active) && InGameManager.GetSingleton.State == GameState.Pause)
            {
                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Disconnect"))
                {
                    manager.StopHost();
                }
                ypos += spacing;
            }           
        }

        void JoinButton()
        {
            if (GUI.Button(new Rect(Screen.width / 2, (Screen.height - ButtonHeight) / 2 + yLobbyOffset, ButtonWidth, ButtonHeight), "Join"))
            {
                NetworkManager.singleton.networkAddress = IpAddress;
                NetworkManager.singleton.networkPort = int.Parse(Port);
                NetworkManager.singleton.StartClient();

            }
        }

        void HostButton()
        {
            if (GUI.Button(new Rect(Screen.width / 2 - ButtonWidth, (Screen.height - ButtonHeight) / 2 + yLobbyOffset, ButtonWidth, ButtonHeight), "Host"))
            {
                NetworkManager.singleton.networkPort = int.Parse(Port);
                NetworkManager.singleton.StartHost();
            }
        }

        public string FindIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                print("IP : " + ip.ToString());
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
        }
    }
};
#endif //ENABLE_UNET
