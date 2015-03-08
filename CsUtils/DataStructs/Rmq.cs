using System;
using System.Collections.Generic;
using CsUtils.Core;

namespace CsUtils.DataStructs {
    /// <summary>Describes a Range Mimimal Query structure (RMQ).
    /// RMQ allows fast accumulation of <c>TAcc</c> values on intervals. </summary>
    public interface IRmq<TElem, TAcc> : IDataStruct<TElem>, IRandomAccess<TElem> {
        /// <summary>Get accumulated value for element at <c>index</c>.</summary>
        TAcc Get(int index);

        /// <summary>Get accumulated value for <c>length</c> elements,
        /// starting with the one at <c>index</c>.</summary>
        TAcc Get(int index, int length);
    }

    /// <summary>Default implementation of the <c>IRmq</c> interface (also known as interval tree).</summary>
    public sealed class Rmq<TElem, TAcc> : IRmq<TElem, TAcc> {
        public Rmq(IAccumulator<TAcc, TElem> accumulator) {
            this.accumulator = accumulator;
        }

        private readonly IAccumulator<TAcc, TElem> accumulator;
        private TElem[] data;
        private TAcc[] tree;
        private int[] leftBoundary;
        private int[] rightBoundary;
        private int nodesCount;
        private int leafsCount;
        private int totalCount;
        private int dataLength;

        public void Init(TElem[] data) {
            dataLength = data.Length;
            this.data = new TElem[dataLength];
            Array.Copy(data, 0, this.data, 0, dataLength);

            leafsCount = 1;
            while (leafsCount < dataLength) {
                leafsCount *= 2;
            }

            nodesCount = leafsCount - 1;
            totalCount = nodesCount + leafsCount;

            tree = new TAcc[totalCount];
            leftBoundary = new int[totalCount];
            rightBoundary = new int[totalCount];

            for (int i = 0; i < dataLength; i++) {
                tree[nodesCount + i] = accumulator.Compute(data[i]);
            }

            for (int i = dataLength; i < leafsCount; i++) {
                tree[nodesCount + i] = accumulator.Neutral;
            }

            for (int i = nodesCount; i < totalCount; i++) {
                leftBoundary[i] = rightBoundary[i] = i - nodesCount;
            }

            for (int i = nodesCount - 1; i >= 0; i--) {
                int l = LeftChild(i), r = RightChild(i);
                tree[i] = accumulator.Combine(tree[l], tree[r]);
                leftBoundary[i] = leftBoundary[l];
                rightBoundary[i] = rightBoundary[r];
            }
        }

        public void Init(int n) {
            Init(new TElem[n]);
        }

        public void Clear() {
            Init(new TElem[] {});
        }

        public void Destroy() {
            this.data = null;
            this.tree = null;
            this.leftBoundary = null;
            this.rightBoundary = null;
            this.leafsCount = 0;
            this.nodesCount = 0;
            this.totalCount = 0;
            this.dataLength = 0;
        }

        public int Size {
            get { return dataLength; }
        }

        public IEnumerable<TElem> Elements {
            get {
                foreach (TElem elem in data) {
                    yield return elem;
                }
            }
        }

        private int Parent(int node) {
            return (node + 1) / 2 - 1;
        }

        private int LeftChild(int node) {
            return (node + 1) * 2 - 1;
        }

        private int RightChild(int node) {
            return (node + 1) * 2;
        }

        private void ValidateIndex(int index) {
            Assert.Validate(index >= 0 && index < dataLength,
                "Rmq index {0} is out of range [0..{1}]", index, dataLength - 1);
        }

        private void ValidateQuery(int index, int length) {
            Assert.Validate(length >= 0 && index >= 0 && index + length <= dataLength,
                "Rmq query [{0}..{1}] is out of range [{0}..{2}]", index, index + length - 1, dataLength - 1);
        }

        public TElem this[int index] {
            get {
                ValidateIndex(index);
                return data[index];
            } set {
                ValidateIndex(index);
                data[index] = value;
                tree[nodesCount + index] = accumulator.Compute(data[index]);

                for (int node = Parent(nodesCount + index); node >= 0; node = Parent(node)) {
                    tree[node] = accumulator.Combine(tree[LeftChild(node)], tree[RightChild(node)]);
                }
            }
        }

        public TAcc Get(int index) {
            ValidateIndex(index);
            return tree[nodesCount + index];
        }

        private TAcc Query(int node, int leftIdx, int rightIdx) {
            int lb = leftBoundary[node], rb = rightBoundary[node];
            if (leftIdx <= lb && rightIdx >= rb) {
                return tree[node];
            } else if (leftIdx > rb || rightIdx < lb) {
                return accumulator.Neutral;
            } else {
                return accumulator.Combine(
                    Query(LeftChild(node), leftIdx, rightIdx),
                    Query(RightChild(node), leftIdx, rightIdx)
                );
            }
        }

        public TAcc Get(int index, int length) {
            ValidateQuery(index, length);
            if (length == 0)
                return accumulator.Neutral;

            return Query(0, index, index + length - 1);
        }
    }
}
