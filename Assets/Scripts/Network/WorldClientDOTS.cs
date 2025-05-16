using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using System;
using Assets.Scripts.Managers;
using Gaia;

namespace Assets.Scripts.Network
{
    /// <summary>
    /// Handles DOTS-based world server connection using Unity Transport.
    /// Uses raw struct messaging — no TLS or legacy converters.
    /// </summary>
    public class WorldClientDOTS : MonoBehaviour
    {
        private NetworkDriver _driver;
        private NetworkConnection _connection;
        private bool _connected;

        public ushort Port = 7777;
        public string IPAddress = "127.0.0.1";

        private void Start()
        {
            _driver = NetworkDriver.Create();
            var endpoint = IPAddress is "127.0.0.1" or "localhost" ? NetworkEndpoint.LoopbackIpv4.WithPort(Port) : NetworkEndpoint.Parse(IPAddress, Port);
            Debug.Log($"[WorldClientDOTS] Parsed endpoint → Host: {endpoint.Address}, Port: {endpoint.Port}, Family: {endpoint.Family}");

            _connection = _driver.Connect(endpoint);

            Debug.Log("[WorldClientDOTS] Attempting to connect to WorldServer...");
        }

        private void Update()
        {
            _driver.ScheduleUpdate().Complete();

            if (!_connection.IsCreated)
                return;

            if (!_connected && _connection.GetState(_driver) == NetworkConnection.State.Connected)
            {
                Debug.Log("[WorldClientDOTS] Connected to WorldServer.");
                _connected = true;

                SendConfirmConnection();
            }

            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = _connection.PopEvent(_driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    Debug.Log($"[WorldClientDOTS] Received data of length {stream.Length}");
                    // TODO: In the future, interpret data (e.g. server spawn instruction)
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.LogWarning("[WorldClientDOTS] Disconnected from WorldServer.");
                    _connection = default;
                    _connected = false;
                }
            }
        }

        /// <summary>
        /// Sends a ConfirmConnection message to the server.
        /// </summary>
        private void SendConfirmConnection()
        {
            var guid = CharacterGameManager.Instance.Serial.ToString();
            var steamId = CharacterGameManager.Instance.SteamId;

            if (_driver.BeginSend(_connection, out var writer) == 0)
            {
                writer.WriteFixedString128(guid);
                writer.WriteLong(steamId);

                _driver.EndSend(writer);
                Debug.Log($"[WorldClientDOTS] Sent ConfirmConnectionMessage → Serial: {guid}, SteamId: {steamId}");
            }
            else
            {
                Debug.LogError("[WorldClientDOTS] Failed to begin send to server.");
            }
        }

        private void OnDestroy()
        {
            _connection.Disconnect(_driver);
            _driver.Dispose();
        }
    }
}
