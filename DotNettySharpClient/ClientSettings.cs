using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotNettySharpClient
{
    public class ClientSettings
    {
        public static bool IsSsl
        {
            get
            {
                string ssl = ExampleHelper.Configuration["ssl"];
                return !string.IsNullOrEmpty(ssl) && bool.Parse(ssl);
            }
        }

        public static IPAddress Host => IPAddress.Parse(ExampleHelper.Configuration["host"]);

        public static int Port => int.Parse(ExampleHelper.Configuration["port"]);

        public static int Size => int.Parse(ExampleHelper.Configuration["size"]);
    }
}
