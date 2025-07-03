using DynamicData.Kernel;

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
    }
}
