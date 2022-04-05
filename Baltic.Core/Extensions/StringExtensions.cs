namespace Baltic.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Always returns the same 64-bits hash code for an identical string
        /// </summary>
        /// <returns>Hash code</returns>
        public static long GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                var hash1 = 5381L;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        /// <summary>
        /// Always returns the same 32-bits hash code for an identical string
        /// </summary>
        /// <returns>Hash code</returns>
        public static int GetDeterministicHashCode32(this string str)
        {
            unchecked
            {
                var hash1 = 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
