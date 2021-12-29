using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Grpc.Core;
namespace ButiEngineEditor.Models.Modules
{

    class ButiEngineIO
    {
        private static Channel _ch;
        private static ButiEngine.EngineCommunicate.EngineCommunicateClient _cl;
        private static bool _isMessageStream=false;
        private static Action<string, Color> _consoleAct;
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
        public static void SetConsoleAction(Action<string,Color> arg_act)
        {
            _consoleAct = arg_act;
        }
        public async static Task MessageStream()
        {
            _isMessageStream = true;
            var i = new ButiEngine.Integer { Value = 0};
            using (AsyncServerStreamingCall<ButiEngine.OutputMessage> call = EditorClient.StreamOutputMessage(i))
            {
                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    ButiEngine.OutputMessage response = call.ResponseStream.Current;

                    if (response.MessageType == ButiEngine.OutputMessage.Types.MessageType.Console&&_consoleAct!=null)
                    {
                        _consoleAct(response.Content, Color.FromArgb(255, (byte)(response.R * 255), (byte)(response.G * 255), (byte)(response.B * 255)));
                    }

                    if (response.MessageType==ButiEngine.OutputMessage.Types.MessageType.End|!_isMessageStream)
                    {
                        break;
                    }
                }
                return ;
            }
        }
        public static void MessageStreamStop()
        {
            _isMessageStream = false;
            EditorClient.StreamOutputStop(new ButiEngine.Integer { Value = 0 });
        }
        public static bool SetWindowActive(bool arg_isActive)
        {
            return EditorClient.SetWindowActive(new ButiEngine.Boolean { Value = arg_isActive }).Value;
        }
        public static void ShutDown()
        {
            ButiEngineChannel.ShutdownAsync();
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
