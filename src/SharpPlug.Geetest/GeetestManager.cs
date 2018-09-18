using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharpPlug.Geetest
{
    public class GeetestManager
    {

        /// <summary>
        /// SDK版本号
        /// </summary>
        public const string Version = "3.2.0";

        /// <summary>
        /// 极验验证API URL
        /// </summary>
        protected const string ApiUrl = "https://api.geetest.com";
        /// <summary>
        /// register url
        /// </summary>
        protected const string RegisterUrl = "/register.php";
        /// <summary>
        /// validate url
        /// </summary>
        protected const string ValidateUrl = "/validate.php";

        private readonly ILogger _logger;

        private readonly GeetestOptions _options;

        private readonly HttpClient _client;

        private readonly Random _random;

        public GeetestManager(ILogger<GeetestManager> logger, IOptions<GeetestOptions> options, IHttpClientFactory factory)
        {
            _logger = logger;
            _options = options.Value;
            _client = factory.CreateClient(nameof(GeetestManager));
            _client.Timeout = TimeSpan.FromSeconds(20);
            _random = new Random();
        }

        /// <summary>
        /// 验证初始化预处理
        /// </summary>
        /// <returns>初始化结果</returns>
        public async Task<GeeTestRegisterResult> Register(string userId = null, string clientType = "unknown", string ipAddress = "unknown")
        {
            if (string.IsNullOrWhiteSpace(_options.Key))
            {
                throw new ArgumentNullException(nameof(_options.Key) + "is not can be null");
            }

            if (string.IsNullOrWhiteSpace(_options.Id))
            {
                throw new ArgumentNullException(nameof(_options.Id) + "is not can be null");
            }

            var result = new GeeTestRegisterResult
            {
                Gt = _options.Id,
                NewCaptcha = true,
                Challenge = GetFailChallenge()
            };

            var url = string.IsNullOrWhiteSpace(userId) ? $"{ApiUrl}{RegisterUrl}?gt={_options.Id}&client_type={clientType}&ip_address={ipAddress}"
                : $"{ApiUrl}{RegisterUrl}?gt={_options.Id}&user_id={userId}&client_type={clientType}&ip_address={ipAddress}";

            var challenge = await _client.GetStringAsync(url);

            if (challenge.Length == 32)
            {
                result.Success = true;
                result.Challenge = md5Encode(challenge + _options.Key);
                return result;
            }

            _logger.LogError("Server regist challenge failed!");

            return result;

        }



        private int GetRandomNum()
        {
            return _random.Next(1, 100);
        }

        private string md5Encode(string plainText)
        {
            using (var md5 = MD5.Create())
            {
                var md5Hash = md5.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                return BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
            }

        }

        /// <summary>
        /// 预处理失败后的返回格式串
        /// </summary>
        private string GetFailChallenge()
        {
            var md5Str1 = md5Encode(GetRandomNum().ToString());
            var md5Str2 = md5Encode(GetRandomNum().ToString());
            return md5Str1 + md5Str2.Substring(0, 2);
        }


        /// <summary>
        /// 一个简单的验证,这时应该调用服务器的其他认证方式,例如手机验证码
        /// </summary>
        /// <param name="challenge">failback模式下用于与validate一起解码答案， 判断验证是否正确</param>
        /// <param name="validate">failback模式下用于与challenge一起解码答案， 判断验证是否正确</param>
        /// <param name="seccode">failback模式下，其实是个没用的参数</param>
        /// <returns>验证结果</returns>
        public bool FailbackValidate(string challenge, string validate, string seccode)
        {
            if (!RequestIsLegal(challenge, validate, seccode))
            {
                return false;
            }
            return md5Encode(challenge) == validate;
        }

        private bool RequestIsLegal(string challenge, string validate, string seccode)
        {
            if (string.IsNullOrWhiteSpace(challenge) || string.IsNullOrWhiteSpace(validate) ||
                string.IsNullOrWhiteSpace(seccode))
            {
                return false;
            }
            return true;
        }



        /// <summary>
        /// 二次验证
        /// </summary>
        /// <returns>二次验证结果</returns>
        public async Task<bool> Validate(GeetestValidateInput input)
        {
            if (input.Offline)
            {
                return md5Encode(input.Challenge) == input.Validate;
            }
      
            if (input.Validate.Length > 0 && input.Validate == md5Encode(_options.Key + "geetest" + input.Challenge))
            {
                string query = "seccode=" + input.Seccode;

                if (!string.IsNullOrWhiteSpace(input.UserId))
                {
                    query += "&user_id=" + input.UserId;
                }
                query += "&sdk=csharp_" + Version;

                string response = "";
                try
                {
                    var url = $"{ApiUrl}{ValidateUrl}";
                    var  responseMessage = await _client.PostAsync(url, new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded"));
                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        return false;
                    }
                    response =  await responseMessage.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to postValidate");
                }

                if (response == md5Encode(input.Seccode))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
