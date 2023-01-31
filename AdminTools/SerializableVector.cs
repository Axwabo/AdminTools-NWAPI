using System;
using UnityEngine;

namespace AdminTools
{
    [Serializable]
    public struct SerializableVector
    {
        public SerializableVector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public static implicit operator SerializableVector(Vector3 vector) => new(vector.x, vector.y, vector.z);

        public static implicit operator Vector3(SerializableVector vector) => new(vector.X, vector.Y, vector.Z);
    }
}
