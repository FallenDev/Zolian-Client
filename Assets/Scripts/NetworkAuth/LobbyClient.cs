using System;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.Converters.SendToServer;
using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.NetworkAuth
{
    /// <summary>
    /// LobbyClient is a singleton class that handles the connection to the lobby server.
    /// Note: Cannot be Abstract because it inherits a MonoBehaviour
    /// </summary>
    public class LobbyClient : NetworkBase
    {
        private static LobbyClient _instance;

        public static LobbyClient LobbyInstance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindFirstObjectByType<LobbyClient>();
                if (_instance != null) return _instance;

                var singletonObject = new GameObject("LobbyClient");
                _instance = singletonObject.AddComponent<LobbyClient>();

                return _instance;
            }
        }

        private void Awake()
        {
            IndexServerConverters();
            IndexClientConverters();
            ConnectToServer(LobbyPort);
        }

        public void SendVersionNumber()
        {
            try
            {
                var args = new VersionArgs();
                SendPacket(VersionArgs.OpCode, args);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send connection confirmation: {e.Message}");
                MainThreadDispatcher.RunOnMainThread(() =>
                {
                    PopupManager.Instance.ShowMessage(e.Message, PopupMessageType.System);
                });
            }
        }
        
        protected override void HandlePacket(byte opCode, object args)
        {
            switch (opCode)
            {
                case (byte)ServerOpCode.AcceptConnection: // Accepted Connection message
                    {
                        var acceptArgs = (AcceptConnectionArgs)args;
                        Debug.Log($"Accept connection message: {acceptArgs.Message}");
                        break;
                    }
                case (byte)ServerOpCode.LoginMessage: // Login message
                    {
                        var loginArgs = (LoginMessageArgs)args;
                        MainThreadDispatcher.RunOnMainThread(() =>
                        {
                            PopupManager.Instance.ShowMessage(loginArgs.Message, loginArgs.LoginMessageType);
                        });
                        break;
                    }
                case (byte)ServerOpCode.ConnectionInfo: // Redirection
                    {
                        var connectionInfoArgs = (ConnectionInfoArgs)args;
                        if (connectionInfoArgs.PortNumber == LoginPort)
                        {
                            LobbySceneTransfer = true;
                        }
                        break;
                    }
                default:
                    MainThreadDispatcher.RunOnMainThread(() =>
                    {
                        PopupManager.Instance.ShowMessage($"Unhandled OpCode: {opCode}", PopupMessageType.System);
                    });
                    break;
            }
        }

        private void IndexServerConverters()
        {
            ServerConverters.Add((byte)ServerOpCode.AcceptConnection, new AcceptConnectionConverter());
            ServerConverters.Add((byte)ServerOpCode.LoginMessage, new LoginMessageConverter());
            ServerConverters.Add((byte)ServerOpCode.ConnectionInfo, new ConnectionInfoConverter());
        }

        private void IndexClientConverters()
        {
            ClientConverters.Add((byte)ClientOpCode.Version, new VersionConverter());
        }
    }
}