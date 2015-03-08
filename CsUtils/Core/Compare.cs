using System;

namespace CsUtils.Core {
    /// <summary>Equivalence test delegate for type <c>T<c>.</summary>
    public delegate bool EqF<T>(T a, T b);

    /// <summary>Comparison result.</summary>
    public enum CmpRes {
        LT = -1,
        EQ = 0,
        GT = 1,
    }

    /// <summary>Comparison delegate for type <c>T</c>.</summary>
    public delegate CmpRes CmpF<T>(T a, T b);

    /// <summary>Static class containing comparison utilities.</summary>
    public static class CmpUtils {
        /// <summary>Negates given equivalence test function.</summary>
        public static EqF<T> Negate<T>(EqF<T> eqF) {
            return (T a, T b) => !eqF(a, b);
        }

        /// <summary>Standard equivalence test function.</summary>
        /// <remarks>This function is defined only for implementations of <c>IComparable</c> interface.</remarks>
        public static bool StdEq<T>(T a, T b) where T: IComparable<T> {
            return a.CompareTo(b) == 0;
        }

        /// <summary>Gives the equivalence test function based on the given comparison function.</summary>
        public static EqF<T> EqByCmp<T>(CmpF<T> cmpF) {
            return (T a, T b) => cmpF(a, b) == CmpRes.EQ;
        }

        /// <summary>Inverts the comparison result.</summary>
        public static CmpRes Invert(CmpRes cmpRes) {
            switch (cmpRes) {
            case CmpRes.LT:
                return CmpRes.GT;
            case CmpRes.GT:
                return CmpRes.LT;
            default:
                return CmpRes.EQ;
            }
        }

        /// <summary>Inverts the comparsion function.</summary>
        public static CmpF<T> Invert<T>(CmpF<T> cmpF) {
            return (T a, T b) => Invert(cmpF(a, b));
        }

        /// <summary>Standard comparsion function.</summary>
        /// <remarks>This function is defined only for implementations of <c>IComparable</c> interface.</remarks>
        public static CmpRes StdCmp<T>(T a, T b) where T: IComparable<T> {
            return (CmpRes) a.CompareTo(b);
        }

        /// <summary>Evaluate the minimum of arguments.</summary>
        public static T Min<T>(CmpF<T> cmpF, params T[] args) {
            Assert.Validate(args.Length > 0, "Min must have at least one arg");
            T min = args[0];
            for (int i = 1; i < args.Length; i++) {
                if (cmpF(args[i], min) == CmpRes.LT)
                    min = args[i];
            }
            return min;
        }

        /// <summary>Evaluate the maximum of arguments.</summary>
        public static T Max<T>(CmpF<T> cmpF, params T[] args) {
            Assert.Validate(args.Length > 0, "Min must have at least one arg");
            T max = args[0];
            for (int i = 1; i < args.Length; i++) {
                if (cmpF(args[i], max) == CmpRes.GT)
                    max = args[i];
            }
            return max;
        }
    }
}
