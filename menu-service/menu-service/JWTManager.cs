#pragma warning disable S101

using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;

namespace menu_service
{
    public class JWTManager
    {
        private readonly string secret;
        private readonly double? validityTime;

        public JWTManager(string secret, double? validityTime = null)
        {
            this.secret = secret ?? throw new ArgumentNullException(nameof(secret));
            this.validityTime = validityTime;
        }

        /// <summary>
        /// Create a Json Web Token with the given payload
        /// </summary>
        /// <param name="json">string-object dictionary of the values a token should be generated for</param>
        /// <returns>Json Web Token</returns>
        /// <exception cref="ArgumentException">An argument was infalid</exception>
        public string Create(Dictionary<string, object> json)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            
            if (validityTime != null)
            {
                IDateTimeProvider provider = new UtcDateTimeProvider();
                json.Add("exp", UnixEpoch.GetSecondsSince(provider.GetNow()) + validityTime);
            }
                
            string token = encoder.Encode(json, secret);
            return token;
        }

        /// <summary>
        /// Decode and possibly verify a given JWT
        /// </summary>
        /// <param name="token">A JWT that will be decoded</param>
        /// <param name="validate">Wether the JWT should be validated</param>
        /// <returns>A string-object dictionary representing the JSON</returns>
        /// <exception cref="ArgumentNullException">A given argument was not valid</exception>
        /// <exception cref="TokenExpiredException">The given JWT has expired</exception>
        /// <exception cref="SignatureVerificationException">The given JWT has not been signed with the right signature</exception>
        public Dictionary<string, object> Decode(string token, bool validate = true)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                return decoder.DecodeToObject<Dictionary<string, object>>(token, secret, verify: validate);
            }
            catch (TokenExpiredException)
            {
                throw new TokenExpiredException("Token has expired");
            }
            catch (SignatureVerificationException)
            {
                throw new SignatureVerificationException("Token has invalid signature");
            }
            catch (Exception ex)
            {
                throw new FormatException(ex.Message);
            }
        }
    }
}