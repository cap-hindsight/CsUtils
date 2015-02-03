using System;

namespace CsUtils {
    /// <summary>Static class containing assertion utilities.</summary>
    /// <remarks>Define the DISABLE_ASSERTIONS compiler symbol in order to disable all assertions.</remarks>
    public static class Assert {
        /// <summary>Throw an instance of AssertionException.</summary>
        /// <param name="format">AssertionException message format.</param>
        /// <param name="args">AssertionException message format arguments.</param>
        public static void Fail(string format, params object[] args) {
            #if !DISABLE_ASSERTIONS
                throw new AssertionException(format, args);
            #endif
        }

        /// <summary>Throw an instance of AssertionException.</summary>
        public static void Fail() {
            #if !DISABLE_ASSERTIONS
                Fail("Assertion failed");
            #endif
        }

        /// <summary>Validate <c>expr</c> and throw an instance of AssertionException if it evaluates to false.</summary>
        /// <param name="expr">Expression to validate.</param>
        /// <param name="format">AssertionException message format.</param>
        /// <param name="args">AssertionException message format arguments.</param>
        public static void Validate(bool expr, string format, params object[] args) {
            #if !DISABLE_ASSERTIONS
                if (!expr) Fail(format, args);
            #endif
        }

        /// <summary>Validate <c>expr</c> and throw an instance of AssertionException if it evaluates to false.</summary>
        /// <param name="expr">Expression to validate.</param>
        public static void Validate(bool expr) {
            #if !DISABLE_ASSERTIONS
                if (!expr) Fail();
            #endif
        }
    }

    /// <summary>A type of Exception which occures when an assertion fails.</summary>
    public sealed class AssertionException: Exception {
        public AssertionException(string format, params object[] args):
            base(String.Format(format, args)) {}
    }
}
