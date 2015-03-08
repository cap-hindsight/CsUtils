using System;
using System.Collections.Generic;

namespace CsUtils.DataStructs {
    /// <summary>Describes a generic data structure.</summary>
    public interface IDataStruct<T> {
        /// <summary>Initialize the structure with new data.</summary>
        void Init(T[] data);

        /// <summary>Initialize the structure with default data of size <c>n</c>.</summary>
        void Init(int n);

        /// <summary>Clear all data (same as <c>Init</c> with an empty array).</summary>
        void Clear();

        /// <summary>Destroy the structure and renders it unusable until further initialization.</summary>
        void Destroy();

        /// <summary>The size of the data in the structure.</summary>
        int Size { get; }

        /// <summary>Iterates through the structure</summary>
        IEnumerable<T> Elements { get; }
    }

    /// <summary>Describes a random-access container.</summary>
    public interface IRandomAccess<T> {
        /// <summary>Index-based accessor.</summary>
        T this[int index] { get; set; }
    }

    /// <summary>Describes an immutable random-access container.</summary>
    public interface IImmutableRandomAccess<T> {
        T this[int index] { get; }
    }
}
