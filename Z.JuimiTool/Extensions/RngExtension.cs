using Z.JiumiTool.Common;

namespace Z.JiumiTool.Extensions
{
    public static class RngExtension
    {
        public static byte[] GetDecryptionArray(this Rng rag, int length = 131072)
        {
            byte[] decryptor_array = new byte[length];
            for (int i = 0; i < length; i++)
            {
                var value = rag.Rand8();
                if (i % 8 == 0)
                {
                    decryptor_array[i + 7] = value;
                }
                else
                {
                    decryptor_array[i - 1] = value;
                }
            }

            return decryptor_array;
        }
    }
}
