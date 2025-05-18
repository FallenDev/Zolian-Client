using Unity.Networking.Transport;
using Unity.Entities;

namespace Assets.Scripts.NetworkWorld.Transport
{
    public struct WorldClientNetwork : IComponentData
    {
        public NetworkDriver Driver;
        public NetworkConnection Connection;
    }
}