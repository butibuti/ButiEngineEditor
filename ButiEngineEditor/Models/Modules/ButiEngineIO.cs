using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
namespace ButiEngineEditor.Models.Modules
{

    class ButiEngineIO
    {
        private static Channel _ch;
        private static ButiEngine.EngineCommunicate.EngineCommunicateClient _cl;
        private static Channel ButiEngineChannel {
            get
            {
                if (_ch==null)
                {
                    _ch = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure); 
                }
                return _ch; }
            set { }
        }
        private static ButiEngine.EngineCommunicate.EngineCommunicateClient EditorClient {
            get
            {
                if (_cl == null)
                {
                    _cl= new ButiEngine.EngineCommunicate.EngineCommunicateClient(ButiEngineChannel);
                }
                return _cl;
            }
            set { } }


        public static bool SetSceneActive(bool arg_isActive)
        {
            return EditorClient.SceneActive(new ButiEngine.Boolean { Value = arg_isActive }).Value;
        }
        public static void SceneSave()
        {
            EditorClient.SceneSave(new ButiEngine.Integer { Value = 0 });
        }
        public static void SceneReload()
        {
            EditorClient.SceneReload(new ButiEngine.Integer { Value = 0 });
        }
        public static int SceneChange(string arg_sceneChangeName)
        {
            return EditorClient.SceneChange(new ButiEngine.String { Value = arg_sceneChangeName }).Value;
        }
        public static int ApplicationStartUp()
        {
            return EditorClient.ApplicationStartUp(new ButiEngine.Integer { Value = 0 }).Value;
        }
        public static int ApplicationShutDown()
        {
            return EditorClient.ApplicationShutDown(new ButiEngine.Integer { Value = 0 }).Value;
        }
        public static int ApplicationReload()
        {
            return EditorClient.ApplicationReload(new ButiEngine.Integer { Value = 0 }).Value;
        }
        public static void GetFPS(ref float arg_ref_current,ref float arg_ref_average ,ref int arg_ref_drawMillSec,ref int arg_ref_updateMillSec)
        {
            var frameRate = EditorClient.GetFPS(new ButiEngine.Integer { Value = 0 });
            arg_ref_current = frameRate.Current;
            arg_ref_average = frameRate.Average;
            arg_ref_drawMillSec= frameRate.DrawMillSec;
            arg_ref_updateMillSec= frameRate.UpdateMillSec;
        }
        public static RenderTargetInformation GetRenderTargetInformation(string arg_renderTargetName)
        {
            RenderTargetInformation output = new RenderTargetInformation();
            var reply = EditorClient.GetRenderTargetInformation(new ButiEngine.String { Value = arg_renderTargetName });
            output.width = reply.Width;
            output.height = reply.Height;
            output.stride = reply.Stride;
            switch (reply.Format) {
                case 28:
                    output.format = System.Windows.Media.PixelFormats.Pbgra32;
                    output.pixelSize = 4;
                break;
            }

            return output;
        }
        public static bool SetRenderTargetViewedByEditor(string arg_renderTargetViewName, bool arg_isViewd)
        {
            var reply = EditorClient.SetRenderTargetView(new ButiEngine.RenderTargetViewed { Name = arg_renderTargetViewName, IsViewed = arg_isViewd });
            return reply.Value;
        }
        public async static Task<Byte[]> GetRenderTargetData(string arg_renderTargetTextureName,RenderTargetInformation rtvInfo)
        {
            var key = new ButiEngine.String { Value = arg_renderTargetTextureName };
            Byte[] output = new Byte[rtvInfo.width* rtvInfo.height*rtvInfo.pixelSize];
            int index = 0;
            using (AsyncServerStreamingCall<ButiEngine.FileData> call = EditorClient.GetRenderTargetImage(key))
            {
                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    ButiEngine.FileData response = call.ResponseStream.Current;

                    if (response.Data != null && response.Data.Length > 0)
                    {
                        response.Data.CopyTo(output, index);
                        index += response.Data.Length;
                    }

                    if (response.Eof)
                    {
                        break;
                    }
                }
                return output;
            }

        }
        public async static Task MessageStream()
        {
            var i = new ButiEngine.Integer { Value = 0};
            using (AsyncServerStreamingCall<ButiEngine.OutputMessage> call = EditorClient.StreamOutputMessage(i))
            {
                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    ButiEngine.OutputMessage response = call.ResponseStream.Current;

                    if (response.MessageType == ButiEngine.OutputMessage.Types.MessageType.Console&&response.Content.Length>0)
                    {
                        int t = 0;
                    }

                    if (response.MessageType==ButiEngine.OutputMessage.Types.MessageType.End)
                    {
                        break;
                    }
                }
                return ;
            }
        }
        public static void ShutDown()
        {
            ButiEngineChannel.ShutdownAsync().Wait();
        }
    }

    public class RenderTargetInformation
    {
        public System.Windows.Media. PixelFormat format;
        public int stride;
        public int pixelSize;
        public int width;
        public int height;
    }
}
