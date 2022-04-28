using System.Security.Cryptography;
using System.Text;

namespace menu_service
{
    public static class HashManager
    {
        public static string GetHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input), "An input has to be provided");
            }

            using (SHA256 hasher = SHA256.Create())
            {
                byte[] hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new();
                foreach (byte hashByte in hashBytes) 
                {
                    sBuilder.Append(hashByte.ToString("x2"));
                }
               
                return sBuilder.ToString();
            }
        }

        public static bool CompareStringToHash(string input, string hash)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(hash))
            {
                return false;
            }

            return (GetHash(input) == hash);
        }
    }
}
