using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CsUtils.Core {
    /// <summary>Static class containing serialization extension methods.</summary>
    public static class Serialization {
        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        private static T SerializationCast<T>(object obj) {
            if (obj is T) {
                return (T) obj;
            } else {
                throw new SerializationException(
                        String.Format(
                            "Deserialize: expected {0}, got {1}",
                            typeof(T).Name, obj.GetType().Name
                        )
                    );
            }
        }

        /// <summary>Serialize <c>obj</c> to <c>stream</c>.</summary>
        public static void Serialize(this Stream stream, object obj) {
            formatter.Serialize(stream, obj);
        }

        /// <summary>Deserialize an object from <c>stream</c>.</summary>
        public static object Deserialize(this Stream stream) {
            return formatter.Deserialize(stream);
        }

        /// <summary>Deserialize an object of type <c>T</c> from <c>stream</c>.</summary>
        public static T Deserialize<T>(this Stream stream) {
            return SerializationCast<T>(Deserialize(stream));
        }

        /// <summary>Serialize <c>obj</c> into an array of bytes.</summary>
        /// <returns>Returns an array of bytes representing serialized <c>obj</c>.</returns>
        public static byte[] Compress(this object obj) {
            var stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        /// <summary>Deserialize an object from an array of bytes.</summary>
        public static object Extract(this byte[] data) {
            var stream = new MemoryStream(data);
            return formatter.Deserialize(stream);
        }

        /// <summary>Deserialize an object of type <c>T</c> from an array of bytes.</summary>
        public static T Extract<T>(this byte[] data) {
            return SerializationCast<T>(Extract(data));
        }

        /// <summary>Create a deep clone of <c>obj</c>.</summary>
        /// <returns>A complete copy of <c>obj</c> with no references to the origin.</returns>
        public static T DeepClone<T>(this T obj) {
            return Extract<T>(Compress(obj));
        }
    }
}
