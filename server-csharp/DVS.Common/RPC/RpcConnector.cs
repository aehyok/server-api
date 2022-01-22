using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Common.RPC
{
    public static class RpcConnector
    {
        public static RpcOption EndPoint { get; set; }
        public static IApplicationBuilder UseRpc(this IApplicationBuilder app, Action<RpcOption> configure)
        {
            RpcOption rpcOption = new RpcOption();
            configure(rpcOption);
            EndPoint =rpcOption;
            return app;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
    //    public static string SendData(string methodName, string data)
    //    {
    //        using var channel = GrpcChannel.ForAddress("http://139.9.184.171:33071");
    //        var client = new SunCenter.SunCenterClient(channel);
     

    //        using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
    //        {
    //           //f clientSocket.Connect(EndPoint);

    //            //send message
    //            string sendStr = "{\"method\":\"" + methodName + "\",\"params\":[\"" + Convert.ToBase64String(Encoding.UTF8.GetBytes(data)) + "\"],\"id\":" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "}";
    //            byte[] sendBytes = Encoding.UTF8.GetBytes(sendStr);
    //            clientSocket.Send(sendBytes);

    //            //receive message
    //            string recStr = "";
    //            byte[] recBytes = new byte[40960];
    //            int total = clientSocket.Receive(recBytes, 0, recBytes.Length, SocketFlags.None);
    //            recStr += Encoding.UTF8.GetString(recBytes, 0, total);
    //            Log.Information("收到数据长度:" + total);
    //            clientSocket.Close();
    //            return recStr;
    //        }
    //    }
    }
    public class RpcOption
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }
}