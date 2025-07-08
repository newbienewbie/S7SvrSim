using DynamicData.Kernel;
using System;

namespace S7SvrSim.Shared
{
    public static class ParseExtensions
    {
        public static Optional<bool> ParseBool(this string value)
        {
            if (bool.TryParse(value, out bool result))
                return result;

            return Optional.None<bool>();
        }

        public static Optional<T> ParseEnum<T>(this string value)
            where T : Enum
        {
            if (Enum.TryParse(typeof(T), value, true, out var result))
                return (Optional<T>)(T)result;

            return Optional.None<T>();
        }
    }
}
