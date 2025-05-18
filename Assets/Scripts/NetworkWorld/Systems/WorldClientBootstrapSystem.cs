using Unity.Entities;
using Unity.Networking.Transport;

using UnityEngine;

namespace Assets.Scripts.NetworkWorld.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct WorldClientBootstrapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var driver = NetworkDriver.Create();
            var endpoint = NetworkEndpoint.Parse("127.0.0.1", 7777);
            var connection = driver.Connect(endpoint);

            state.EntityManager.CreateSingleton(new Transport.WorldClientNetwork
            {
                Driver = driver,
                Connection = connection
            });

            Debug.Log("[WorldClient] Bootstrap: Attempting to connect to World Server.");
        }

        public void OnDestroy(ref SystemState state)
        {
            var clientNet = SystemAPI.GetSingleton<Transport.WorldClientNetwork>();
            if (clientNet.Driver.IsCreated) clientNet.Driver.Dispose();
        }
    }
}