using DVS.Common.Infrastructures;
using DVS.Common.ModelDtos;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DVS.Common.RPC
{
    public class BasicRPC
    {
        public static ResultModel<object> SendSMS(List<string> mobiles, string message)
        {
            SunCenter.SunCenterClient client = createClient();
            Reply reply = client.SendSms(new Request() { Parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { phones = mobiles, msg = message }))) });
            string resultString = reply.Data;
            var data = JsonSerializer.Deserialize<ResultModel<object>>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return data;
        }

        private static void setSSLSupport()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
        }

        //public static string GetToken(string account)
        //{
        //    string resultString = RpcConnector.SendData("RcpTest.GetToken", account);
        //    RpcResultBase<string> res = JsonSerializer.Deserialize<RpcResultBase<string>>(resultString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        //    return res.Result;
        //}

        public static ResultModel<SSOVerifyWithMaxPermissionResult> GetMaxPermissions(SSOVerifyResult verifyResult,string moduleCode)
        {
            SunCenter.SunCenterClient client = createClient();
            Reply reply = client.Any(new Request()
            {
                Parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                   Method= "GetMaxPermissions",
                   Module= moduleCode,
                   Body=Newtonsoft.Json.JsonConvert.SerializeObject(verifyResult)
                })))
            });
            string value = Encoding.UTF8.GetString(Convert.FromBase64String(reply.Data));
            Console.WriteLine("最大权限值:" + value);
            var result= new ResultModel<SSOVerifyWithMaxPermissionResult>();
            RpcReply < SSOVerifyWithMaxPermissionResult > serverReply = JsonSerializer.Deserialize<RpcReply<SSOVerifyWithMaxPermissionResult>>(value);
            if (serverReply != null)
            {
                result.Data = serverReply.Body;
            }
            return result;
        }


        public static ResultModel<object> AllotScore(IntegralReq req)
        {
            Console.WriteLine("添加积分:"+ JsonSerializer.Serialize(req));
            SunCenter.SunCenterClient client = createClient();
            string methodName = Enum.GetName(typeof(IntegralAction), req.IntegralAction);
            Reply reply = client.Integral(new Request()
            {
                Parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    HouseholdId = req.HouseholdId,
                    Description = req.Description,
                    UserId = req.UserId,
                    Method = methodName.Substring(0, 1).ToLower() + methodName.Substring(1),
                    Score = req.Score
                })))
            });
            string value= Encoding.UTF8.GetString(Convert.FromBase64String(reply.Data));
            return new ResultModel<object>();
        }

        public static ResultModel<SSOVerifyResult> SSOVerify(SSOVerifyRequest ssoVerifyReq)
        {
            try
            {
                SunCenter.SunCenterClient client = createClient();
                Reply reply = client.TokenVerify(new Request() { Parameters = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ssoVerifyReq))) });
                string resultString = reply.Data;
                string resultData = Encoding.UTF8.GetString(Convert.FromBase64String(resultString));
                if (resultData.IsNullOrWhiteSpace())
                {
                    return null;
                }
                ResultModel<SSOVerifyResult> data = null;
                if (!string.IsNullOrWhiteSpace(resultData))
                {
                    data = JsonSerializer.Deserialize<ResultModel<SSOVerifyResult>>(resultData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                }

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static SunCenter.SunCenterClient createClient()
        {
            setSSLSupport();
            GrpcChannel channel = GrpcChannel.ForAddress($"http://{RpcConnector.EndPoint.Host}:{RpcConnector.EndPoint.Port}");
            return new SunCenter.SunCenterClient(channel);
        }
    }
}
