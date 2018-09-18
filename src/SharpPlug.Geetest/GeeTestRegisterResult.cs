using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPlug.Geetest
{
    public class GeeTestRegisterResult
    {
        public bool Success { get; set; }

        public string Gt { get; set; }

        public string Challenge { get; set; }

        public bool NewCaptcha { get; set; }
    }
}
