using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Presistance.Helper
{
    public static class IpAddressHelper
    {
        public static string GetIpAddresss(HttpContext context)
            {
            // Getting host name 
            string host = Dns.GetHostName();
            // Getting ip address using host name 
            IPHostEntry ip = Dns.GetHostEntry(host);
            string IP = ip.AddressList[3].ToString();
            return IP;
            /*
            // Check if the request is coming from localhost
            if (IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
            {
                // Use the loopback address as the client's IP address
                return context.Connection.LocalIpAddress.ToString();
            }
            else
            {
                // Use the actual client's IP address
                return  context.Connection.RemoteIpAddress.ToString();
            }
            */
        }
    }
}
