using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ButiEngineEditor.ViewModels.Panes;
using Grpc.Core;
namespace ButiEngineEditor.Models.Modules
{

    class ButiEngineIO
    {
        private static readonly string ButiEngineAddress="127.0.0.1:50051";
        private static Channel _ch;
        private static ButiEngine.EngineCommunicate.EngineCommunicateClient _cl;
        private static bool _isMessageStream=false;
        private static Action<string, Color> _consoleAct;
        private static Channel ButiEngineChannel {
            get
            {
                if (_ch==null)
                {
                    _ch = new Channel(ButiEngineAddress, ChannelCredentials.Insecure); 
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

        public static bool IsEngineActive()
        {
            return App.ButiEngineProcess != null;
        }

        public static bool SetSceneActive(bool arg_isActive)
        {
            if (!IsEngineActive())
            {
                return false;
            }

            return EditorClient.SceneActive(new ButiEngine.Boolean { Value = arg_isActive }).Value;
        }
        public static void SceneSave()
        {
            if (!IsEngineActive())
            {
                return ;
            }
            EditorClient.SceneSave(new ButiEngine.Integer { Value = 0 });
        }
        public static void SceneReload()
        {
            if (!IsEngineActive())
            {
                return;
            }
            EditorClient.SceneReload(new ButiEngine.Integer { Value = 0 });
        }
        public static int SceneChange(string arg_sceneChangeName)
        {
            if (!IsEngineActive())
            {
                return 0;
            }
            return EditorClient.SceneChange(new ButiEngine.String { Value = arg_sceneChangeName }).Value;
        }
        public static int ApplicationStartUp()
        {
            if (!IsEngineActive())
            {
                return 0;
            }
            return EditorClient.ApplicationStartUp(new ButiEngine.Integer { Value = 0 }).Value;
        }
        public static int ApplicationShutDown(int arg_shutDownCode)
        {
            if (!IsEngineActive())
            {
                return 0;
            }
            return EditorClient.ApplicationShutDown(new ButiEngine.Integer { Value = arg_shutDownCode }).Value;
        }
        public static int ApplicationReload()
        {

            if (!IsEngineActive())
            {
                return 0;
            }
            return EditorClient.ApplicationReload(new ButiEngine.Integer { Value = 0 }).Value;
        }
        public static void GetFPS(ref float arg_ref_current,ref float arg_ref_average ,ref int arg_ref_drawMillSec,ref int arg_ref_updateMillSec)
        {
            if (!IsEngineActive())
            {
                return;
            }
            var frameRate = EditorClient.GetFPS(new ButiEngine.Integer { Value = 0 });
            arg_ref_current = frameRate.Current;
            arg_ref_average = frameRate.Average;
            arg_ref_drawMillSec= frameRate.DrawMillSec;
            arg_ref_updateMillSec= frameRate.UpdateMillSec;
        }
        public static RenderTargetInformation GetRenderTargetInformation(string arg_renderTargetName)
        {
            RenderTargetInformation output = new RenderTargetInformation();
            if (!IsEngineActive())
            {
                output.width = 0;
                output.width = 0;
                output.height = 0;
                output.stride = 0;
                output.format = System.Windows.Media.PixelFormats.Pbgra32;
                output.pixelSize = 0;
            }
            else
            {
                var reply = EditorClient.GetRenderTargetInformation(new ButiEngine.String { Value = arg_renderTargetName });
                output.width = reply.Width;
                output.height = reply.Height;
                output.stride = reply.Stride;
                switch (reply.Format)
                {
                    case 28:
                        output.format = System.Windows.Media.PixelFormats.Pbgra32;
                        output.pixelSize = 4;
                        break;
                }
            }

            return output;
        }
        public static bool SetRenderTargetViewedByEditor(string arg_renderTargetViewName, bool arg_isViewd)
        {
            if (!IsEngineActive())
            {
                return false;
            }
            var reply = EditorClient.SetRenderTargetView(new ButiEngine.RenderTargetViewed { Name = arg_renderTargetViewName, IsViewed = arg_isViewd });
            return reply.Value;
        }
        public async static Task<Byte[]> GetRenderTargetData(string arg_renderTargetTextureName,RenderTargetInformation rtvInfo)
        {

            Byte[] output = new Byte[rtvInfo.width* rtvInfo.height*rtvInfo.pixelSize];
            if (!IsEngineActive())
            {
                return output;
            }
            var key = new ButiEngine.String { Value = arg_renderTargetTextureName };
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

        public async static Task GetRenderTargetStream(string arg_renderTargetTextureName, RenderTargetInformation rtvInfo, Action<Byte[]> arg_readAct)
        {
            if (!IsEngineActive())
            {
                return;
            }
            var key = new ButiEngine.String { Value = arg_renderTargetTextureName };
            byte[] output = new byte[rtvInfo.width * rtvInfo.height * rtvInfo.pixelSize];
            int index = 0;
            using (AsyncServerStreamingCall<ButiEngine.FileData> call = EditorClient.GetRenderTargetImageStream(key))
            {
                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    var response = call.ResponseStream.Current;

                    if (response.Data != null && response.Data.Length > 0)
                    {
                        response.Data.CopyTo(output, index);
                        index += response.Data.Length;
                    }

                    if (response.Eof)
                    {
                        arg_readAct(output);
                        index = 0;
                    }
                    if (response.StreamEnd)
                    {
                        break;
                    }
                }
                return;
            }
        }
        public static void SetConsoleAction(Action<string,Color> arg_act)
        {
            _consoleAct = arg_act;
        }
        public async static Task MessageStream()
        {
            if (!IsEngineActive())
            {
                return;
            }
            _isMessageStream = true;
            var i = new ButiEngine.Integer { Value = 0 };
            using (AsyncServerStreamingCall<ButiEngine.OutputMessage> call = EditorClient.StreamOutputMessage(i))
            {
                while (await call.ResponseStream.MoveNext().ConfigureAwait(false))
                {
                    ButiEngine.OutputMessage response = call.ResponseStream.Current;

                    if (response.MessageType == ButiEngine.OutputMessage.Types.MessageType.Console && _consoleAct != null)
                    {
                        _consoleAct(response.Content, Color.FromArgb(255, (byte)(response.R * 255), (byte)(response.G * 255), (byte)(response.B * 255)));
                    }

                    if (response.MessageType == ButiEngine.OutputMessage.Types.MessageType.End | !_isMessageStream)
                    {
                        break;
                    }
                }
                return;
            }
        }
        public static void MessageStreamStop()
        {
            if (!IsEngineActive())
            {
                return ;
            }
            _isMessageStream = false;
            EditorClient.StreamOutputStop(new ButiEngine.Integer { Value = 0 });
        }
        public static bool SetWindowActive(bool arg_isActive)
        {
            if (!IsEngineActive())
            {
                return false;
            }
            return EditorClient.SetWindowActive(new ButiEngine.Boolean { Value = arg_isActive }).Value;
        }
        public static void ShutDown()
        {
            if (!IsEngineActive())
            {
                return ;
            }
            ApplicationShutDown(0);
            ButiEngineChannel.ShutdownAsync();
        }
        public int LoadTexture(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadTexture(req).Value;
        }
        public int LoadSound(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadSound(req).Value;
        }
        public int LoadScript(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadScript(req).Value;
        }
        public int LoadModel(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadModel(req).Value;
        }
        public int LoadMotion(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadMotion(req).Value;
        }
        public int LoadFont(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadFont(req).Value;
        }
        public int LoadVertexShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadVertexShader(req).Value;
        }
        public int LoadPixelShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadVertexShader(req).Value;
        }
        public int LoadGeometryShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.LoadVertexShader(req).Value;
        }
        public int UnLoadTexture(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadTexture(req).Value;
        }
        public int UnLoadSound(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadSound(req).Value;
        }
        public int UnLoadScript(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadScript(req).Value;
        }
        public int UnLoadModel(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadModel(req).Value;
        }
        public int UnLoadMotion(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadMotion(req).Value;
        }
        public int UnLoadFont(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadFont(req).Value;
        }
        public int UnLoadVertexShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadVertexShader(req).Value;
        }
        public int UnLoadPixelShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadVertexShader(req).Value;
        }
        public int UnLoadGeometryShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadVertexShader(req).Value;
        }
        public int UnLoadShader(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadShader(req).Value;
        }
        public int UnLoadMaterial(List<string> arg_path)
        {
            var req = new ButiEngine.StringArray();
            req.Values.Add(arg_path);
            return EditorClient.UnLoadMaterial(req).Value;
        }
        public int LoadShader(List<ResourceLoadViewModel.ShaderData> arg_datas)
        {
            var req = new ButiEngine.ShaderLoadInfoArray();
            arg_datas.ForEach(data => req.Values.Add(new ButiEngine.ShaderLoadInformation() { ShaderName = data.ShaderName, VertexShaderName = data.VertexShader, PixelShaderName = data.PixelShader, GeometryShaderName = data.GeometryShader }));
            return EditorClient.LoadShader(req).Value;
        }
        public int LoadMaterial(List<ResourceLoadModel.MaterialLoadInfo> arg_datas)
        {
            var req = new ButiEngine.MaterialLoadInfoArray();
            arg_datas.ForEach(data => req.Values.Add(new ButiEngine.MaterialLoadInformation() { MaterialName = data.materialName,
                Diffude = new ButiEngine.Vector4_message() { X = data.var.diffuse.X, Y = data.var.diffuse.Y, Z = data.var.diffuse.Z, W = data.var.diffuse.W },
                Ambient = new ButiEngine.Vector4_message() { X = data.var.ambient.X, Y = data.var.ambient.Y, Z = data.var.ambient.Z, W = data.var.ambient.W },
                Emissive = new ButiEngine.Vector4_message() { X = data.var.emissive.X, Y = data.var.emissive.Y, Z = data.var.emissive.Z, W = data.var.emissive.W },
                Specular= new ButiEngine.Vector4_message() { X = data.var.specular.X, Y = data.var.specular.Y, Z = data.var.specular.Z, W = data.var.specular.W },
            }));
            return EditorClient.LoadMaterial(req).Value;
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
