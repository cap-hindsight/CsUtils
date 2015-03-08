using System;

namespace CsUtils.Core {
    /// <summary>Describes an object which can accumulate values of type <c>TAcc</c>.</summary>
    public interface IAccumulator<TAcc, TVal> {
        /// <summary>Compute <c>TAcc</c> from <c>TVal</c>.</summary>
        TAcc Compute(TVal val);

        /// <summary>Accumulate <c>val</c> in <c>acc</c>.</summary>
        TAcc Accumulate(TAcc acc, TVal val);

        /// <summary>Combine two accumulators.</summary>
        TAcc Combine(TAcc acc1, TAcc acc2);

        /// <summary>Neutral element (gives <c>Compute(x)</c> when accumulated with x)</summary>
        TAcc Neutral { get; }
    }

    /// <summary>Simple accumulator of values, uses an arbitrary function.</summary>
    public sealed class SimpleAccumulator<T> : IAccumulator<T?, T>
        where T: struct
    {
        public SimpleAccumulator(Func<T, T, T> f) {
            this.f = f;
        }

        private readonly Func<T, T, T> f;

        public T? Compute(T val) {
            return val;
        }

        public T? Accumulate(T? acc, T val) {
            if (acc == null) return val;
            return f(acc.Value, val);
        }

        public T? Combine(T? acc1, T? acc2) {
            if (acc1 == null) return acc2;
            if (acc2 == null) return acc1;
            return f(acc1.Value, acc2.Value);
        }

        public T? Neutral {
            get { return null; }
        }
    }

    /// <summary>Frequently used accumulators.</summary>
    public static class Accumulators {
        /// <summary>Simple accumulator, by the accumulation function <c>f</c>.</summary>
        public static IAccumulator<T?, T> SimpleAccumulator<T>(Func<T, T, T> f)
            where T: struct
        {
            return new SimpleAccumulator<T>(f);
        }

        /// <summary>Accumulates minimal value.</summary>
        public static IAccumulator<T?, T> MinAccumulator<T>(CmpF<T> cmpF)
            where T: struct
        {
            return new SimpleAccumulator<T>((T a, T b) => {
                if (cmpF(a, b) == CmpRes.GT) return b;
                else return a;
            });
        }

        /// <summary>Accumulates maximal value.</summary>
        public static IAccumulator<T?, T> MaxAccumulator<T>(CmpF<T> cmpF)
            where T: struct
        {
            return new SimpleAccumulator<T>((T a, T b) => {
                if (cmpF(a, b) == CmpRes.LT) return b;
                else return a;
            });
        }
    }
}
