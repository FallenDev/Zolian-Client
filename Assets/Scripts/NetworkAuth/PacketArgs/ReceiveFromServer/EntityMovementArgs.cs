using System;
using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.PacketHandling;
using UnityEngine;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record EntityMovementArgs : IPacketSerializable
    {
        public EntityType EntityType { get; set; }
        public Guid Serial { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 InputDirection { get; set; }
        public float CameraYaw { get; set; }
        public float Speed { get; set; }
        public float VerticalVelocity { get; set; }
    }
}