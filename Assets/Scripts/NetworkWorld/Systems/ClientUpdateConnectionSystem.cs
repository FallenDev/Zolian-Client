using Unity.Burst;
using Unity.Entities;
using Unity.Networking.Transport;

using UnityEngine;

namespace Assets.Scripts.NetworkWorld.Systems
{
    [BurstCompile]
    public partial struct ClientUpdateConnectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var hasClientNet = SystemAPI.TryGetSingletonRW<Transport.WorldClientNetwork>(out var clientNet);

            if (!hasClientNet)
                return;

            clientNet.ValueRW.Driver.ScheduleUpdate().Complete();

            if (!clientNet.ValueRW.Connection.IsCreated)
                return;

            Unity.Collections.DataStreamReader stream;
            NetworkEvent.Type evt;
            while ((evt = clientNet.ValueRW.Driver.PopEventForConnection(clientNet.ValueRW.Connection, out stream)) != NetworkEvent.Type.Empty)
            {
                if (evt == NetworkEvent.Type.Connect)
                {
                    Debug.Log("[WorldClient] Connected to world server.");
                }
                else if (evt == NetworkEvent.Type.Data)
                {
                    // TODO: Parse and queue for ECS
                }
                else if (evt == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("[WorldClient] Disconnected from world server.");
                    clientNet.ValueRW.Connection = default;
                }
            }
        }
    }
}