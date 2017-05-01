using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharpChannels.Core.Contracts
{
    internal static class Enforce
    {
        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object value, string argumentName)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName, $"Argument '{argumentName}' should not be null");
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NonEmptyString(string value, string argumentName)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName, $"Argument '{argumentName}' should not be null");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"Argument '{argumentName}' should not be empty", argumentName);
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive(int value, string argumentName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException($"Argument '{argumentName}' should be positive", argumentName);
        }

        [DebuggerNonUserCode]
        public static void IsTrue(Func<bool> predicate, string argumentName)
        {
            if(!predicate())
                throw new ArgumentException($"Condition failed for argument '{argumentName}'", argumentName);
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool value, string argumentName)
        {
            if (!value)
                throw new ArgumentException($"Condition failed for argument '{argumentName}'", argumentName);
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool value, string customMessage, string argumentName)
        {
            if (!value)
                throw new ArgumentException(customMessage, argumentName);
        }

        [DebuggerNonUserCode]
        public static void Is<T>(object value, string argumentName)
        {
            if (!(value is T))
                throw new ArgumentException($"Argument {argumentName} should be instance of {typeof(T).FullName}", argumentName);
        }

        [DebuggerNonUserCode]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotDisposed(object obj, bool isDisposedFlag)
        {
            if(isDisposedFlag)
                throw new ObjectDisposedException(obj.GetType().FullName);
        }

        [DebuggerNonUserCode]
        public static class State
        {
            [DebuggerNonUserCode]
            public static void FitsTo(Func<bool> predicate, string description)
            {
                if (!predicate()) throw new InvalidOperationException(description);
            }

            [DebuggerNonUserCode]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FitsTo(bool value, string description)
            {
                if (!value) throw new InvalidOperationException(description);
            }
        }
    }
}
