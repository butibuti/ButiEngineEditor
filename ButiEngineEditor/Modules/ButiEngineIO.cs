using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
namespace ButiEngineEditor.Modules
{
    class ButiEngineIO
    {
        public static void SceneStart()
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new ButiEngine.Greeter.GreeterClient(channel);
            String user = "youyouyou";

            var reply = client.SayHello(new ButiEngine.HelloRequest { Name = user });
            var message = reply.Message;

            channel.ShutdownAsync().Wait();
        }
    }
}
