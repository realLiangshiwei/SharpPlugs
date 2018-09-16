using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SharpPlug.Geetest
{
    public class Geetest
    {

        /// <summary>
        /// SDK版本号
        /// </summary>
        public const string Version = "3.2.0";
        /// <summary>
        /// SDK开发语言
        /// </summary>
        public const string SdkLang = "csharp";
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
        /// <summary>
        /// 极验验证API服务状态Session Key
        /// </summary>
        public const string GtServerStatusSessionKey = "gt_server_status";
        /// <summary>
        /// 极验验证二次验证表单数据 Chllenge
        /// </summary>
        public const string FnGeetestChallenge = "geetest_challenge";
        /// <summary>
        /// 极验验证二次验证表单数据 Validate
        /// </summary>
        public const string FnGeetestValidate = "geetest_validate";
        /// <summary>
        /// 极验验证二次验证表单数据 Seccode
        /// </summary>
        public const string FnGeetestSeccode = "geetest_seccode";

        /// <summary>
        /// 验证成功
        /// </summary>
        public const int SuccessResult = 1;
        /// <summary>
        /// 验证失败
        /// </summary>
        public const int FailResult = 0;


        private string _userId;

        private string _clientType;

        private string _ipAddress;

        private string _response;

        public string Getresponse()
        {
            return _response;
        }

        private readonly ILogger _logger;

        private readonly GeetestOptions _options;

        private readonly HttpClient _client;

        private readonly Random _random;

        public Geetest(ILogger<Geetest> logger, IOptions<GeetestOptions> options, IHttpClientFactory factory)
        {
            _logger = logger;
            _options = options.Value;
            _client = factory.CreateClient(nameof(Geetest));
            _client.Timeout = TimeSpan.FromSeconds(20);
            _random = new Random();
        }

        /// <summary>
        /// 验证初始化预处理
        /// </summary>
        /// <returns>初始化结果</returns>
        public async Task<int> PreProcess(string userId = "", string clientType = "web", string ipAddress = "")
        {

            if (_options.Key == null)
            {
                _logger.LogError("publicKey is null!");
            }
            else
            {
                _userId = userId;
                _clientType = clientType;
                _ipAddress = ipAddress;
                var challenge = await RegisterChallenge();
                if (challenge.Length == 32)
                {
                    GetSuccessPreProcess(challenge);
                    return 1;
                }

                GetFailPreProcess();
                _logger.LogError("Server regist challenge failed!");
            }

            return 0;

        }

        private async Task<string> RegisterChallenge()
        {
            var url = string.IsNullOrWhiteSpace(_userId) ? $"{ApiUrl}{RegisterUrl}?gt={_options.AppId}&client_type={_clientType}&ip_address={_ipAddress}" : $"{ApiUrl}{RegisterUrl}?gt={_options.AppId}&user_id={_userId}&client_type={_clientType}&ip_address={_ipAddress}";
            return await ReadContentFromGet(url);
        }

        private async Task<string> ReadContentFromGet(string url)
        {
            try
            {
                return await _client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, $"Requesting to {url}");
                return "";
            }

        }

        private int GetRandomNum()
        {
            return _random.Next(1, 100);
        }

        private string md5Encode(string plainText)
        {
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(plainText)));
            return t2.Replace("-", "").ToLower();

        }

        /// <summary>
        /// 预处理失败后的返回格式串
        /// </summary>
        private void GetFailPreProcess()
        {
            var md5Str1 = md5Encode(GetRandomNum().ToString());
            var md5Str2 = md5Encode(GetRandomNum().ToString());
            string challenge = md5Str1 + md5Str2.Substring(0, 2);
            _response = "{" + $"\"success\":{0},\"gt\":\"{_options.AppId}\",\"challenge\":\"{challenge}\",\"new_captcha\":{"true"}" + "}";
        }

        /// <summary>
        /// 预处理成功后的标准串
        /// </summary>
        private void GetSuccessPreProcess(string challenge)
        {
            challenge = md5Encode(challenge + _options.Key);
            _response = "{" +
                        $"\"success\":{1},\"gt\":\"{_options.AppId}\",\"challenge\":\"{challenge}\",\"new_captcha\":{"true"}" + "}";
        }


        /// <summary>
        /// failback模式的验证方式
        /// </summary>
        /// <param name="challenge">failback模式下用于与validate一起解码答案， 判断验证是否正确</param>
        /// <param name="validate">failback模式下用于与challenge一起解码答案， 判断验证是否正确</param>
        /// <param name="seccode">failback模式下，其实是个没用的参数</param>
        /// <returns>验证结果</returns>
        public int FailbackValidateRequest(string challenge, string validate, string seccode)
        {
            if (!this.RequestIsLegal(challenge, validate, seccode)) return FailResult;
            int validateResult = FailbackCheckResult(challenge, validate);
            return validateResult;
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

        private int FailbackCheckResult(string challenge, string validate)
        {
            string encodeStr = md5Encode(challenge);
            if (encodeStr == validate)
            {
                return SuccessResult;
            }
            return FailResult;
        }


        /// <summary>
        /// 向gt-server进行二次验证
        /// </summary>
        /// <param name="challenge">本次验证会话的唯一标识</param>
        /// <param name="validate">拖动完成后server端返回的验证结果标识字符串</param>
        /// <param name="seccode">验证结果的校验码，如果gt-server返回的不与这个值相等则表明验证失败</param>
        /// <param name="userId"></param>
        /// <returns>二次验证结果</returns>
        public async Task<int> EnhencedValidateRequest(string challenge, string validate, string seccode, string userId = null)
        {
            if (!RequestIsLegal(challenge, validate, seccode)) return FailResult;
            if (validate.Length > 0 && CheckResultByPrivate(challenge, validate))
            {
                string query = "seccode=" + seccode;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    query += "&user_id=" + userId;
                }
                query += "&sdk=csharp_" + Version;

                string response = "";
                try
                {
                    response = await PostValidate(query);
                }
                catch (Exception e)
                {
                    _logger.LogError(0, e, "Failed to postValidate");
                }

                if (response.Equals(md5Encode(seccode)))
                {
                    return SuccessResult;
                }
            }
            return FailResult;
        }

        private bool CheckResultByPrivate(string origin, string validate)
        {
            return validate.Equals(md5Encode(_options.Key + "geetest" + origin));
        }

        private async Task<string> PostValidate(String data)
        {
            var url = $"{ApiUrl}{ValidateUrl}";
            var response = await _client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded"));
            return await response.Content.ReadAsStringAsync();
        }
    }
}
