using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ButiEngineEditor.ViewModels.Panes;
using Grpc.Core;
using static ButiEngineEditor.Models.ResourceLoadModel;

namespace ButiEngineEditor.Models.Modules
{

    class ButiEngineIO
    {
        private static ButiEngine.Vector3_message ToMessage(Vector3 arg_vec3)
        {
            return new ButiEngine.Vector3_message(){X = arg_vec3.X,Y = arg_vec3.Y,Z = arg_vec3.Z};
        }
        private static ButiEngine.Transform_message ToMessage(Transform arg_transform)
        {
            return new ButiEngine.Transform_message() { Position = ToMessage(arg_transform.position),Rotation=ToMessage(arg_transform.rotation),Scaling=ToMessage(arg_transform.scaling) };
        }
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
            return  (!App.ButiEngineProcess.HasExited)&&App.ButiEngineProcess != null;
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
            return;
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
        public static string GetDefaultRenderTargetName()
        {
            if (!IsEngineActive())
            {
                return "";
            }
            return EditorClient.GetDefaultRenderTargetImageName(new ButiEngine.Integer()).Value;
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
        public static void SetWindowHandle(IntPtr arg_handle)
        {
            EditorClient.SetEditorWindowHandle(new ButiEngine.LongInteger { Value = arg_handle.ToInt64() });
        }
        public static string CreateGameObjectFromCereal(string arg_fileName, Transform arg_transform)
        {
            if (!IsEngineActive())
            {
                return "";
            }
            return EditorClient.CreateGameObject(new ButiEngine.GameObjectCreate_message { GameObjectName = arg_fileName, Transform = ToMessage(arg_transform) }).Value;
        }
        public static string CreateGameObject(string arg_fileName, Transform arg_transform)
        {
            if (!IsEngineActive())
            {
                return "";
            }
            return EditorClient.CreateGameObject(new ButiEngine.GameObjectCreate_message { GameObjectName = arg_fileName, Transform = ToMessage(arg_transform) }).Value;
        }
        public static void SelectGameObject(string arg_fileName)
        {
            if (!IsEngineActive())
            {
                return;
            }
            EditorClient.SelectGameObject(new ButiEngine.String { Value = arg_fileName });
        }
        public static void SetTransformBase(string arg_fileName)
        {
            if (!IsEngineActive())
            {
                return;
            }
            EditorClient.SetBaseTransform(new ButiEngine.String { Value = arg_fileName });
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
        public static ResourceLoadData GetLoadedResourceData()
        {
            var data = EditorClient.GetLoadedResources(new ButiEngine.Integer { Value = 0 });
            ResourceLoadData output = new ResourceLoadData();
            data.Textures.ToList().ForEach(path => {
                if (path.Length > 0 && (path[0] == ':'|| path[0] == ';'))
                {
                    output.List_renderTargets.Add(path);
                }
                else
                {
                    output.List_textures.Add(path);
                }                
            });
            data.Meshes.ToList().ForEach(path => output.List_meshes.Add(path));
            data.Sounds.ToList().ForEach(path => output.List_sounds.Add(path));
            data.Scripts.ToList().ForEach(path => output.List_scripts.Add(path));
            data.Fonts.ToList().ForEach(path => output.List_fonts.Add(path));
            data.Models.ToList().ForEach(path => output.List_models.Add(path));
            data.Motions.ToList().ForEach(path => output.List_motions.Add(path));
            data.VertexShaders.ToList().ForEach(path => output.List_vertexShaders.Add(path));
            data.PixelShaders.ToList().ForEach(path => output.List_pixelShaders.Add(path));
            data.GeometryShaders.ToList().ForEach(path => output.List_geometryShaders.Add(path));

            data.MaterialLoadInfo.ToList().ForEach(materialLoadInfo =>
            {
                MaterialLoadInfo material=new MaterialLoadInfo();
                material.materialName = materialLoadInfo.MaterialName;
                material.filePath= materialLoadInfo.FilePath;
                material.var.diffuse.X = materialLoadInfo.Diffuse.X; material.var.diffuse.Y = materialLoadInfo.Diffuse.Y; material.var.diffuse.Z = materialLoadInfo.Diffuse.Z; material.var.diffuse.W = materialLoadInfo.Diffuse.W;
                material.var.ambient.X = materialLoadInfo.Ambient.X; material.var.ambient.Y = materialLoadInfo.Ambient.Y; material.var.ambient.Z = materialLoadInfo.Ambient.Z; material.var.ambient.W = materialLoadInfo.Ambient.W;
                material.var.emissive.X = materialLoadInfo.Emissive.X; material.var.emissive.Y = materialLoadInfo.Emissive.Y; material.var.emissive.Z = materialLoadInfo.Emissive.Z; material.var.emissive.W = materialLoadInfo.Emissive.W;
                material.var.specular.X = materialLoadInfo.Specular.X; material.var.specular.Y = materialLoadInfo.Specular.Y; material.var.specular.Z = materialLoadInfo.Specular.Z; material.var.specular.W = materialLoadInfo.Specular.W;
                var textures= materialLoadInfo.Textures.ToList();
                material.material_list_textures = new List<string>();
                textures.ForEach(textureTitle => material.material_list_textures.Add(textureTitle));
                output.List_materials.Add(material);
            });

            data.ShaderLoadInfo.ToList().ForEach(shaderLoadInfo => {
                ShaderLoadInfo shader = new ShaderLoadInfo();
                shader.ShaderName = shaderLoadInfo.ShaderName;
                shader.VertexShaderName= shaderLoadInfo.VertexShaderName;
                shader.PixelShaderName= shaderLoadInfo.PixelShaderName;
                shader.GeometryShaderName= shaderLoadInfo.GeometryShaderName;
                output.List_shaders.Add(shader);
            });

            return output;
        }
        public static void ReourceLoadDataToBinary(ResourceLoadData arg_data,string arg_filePath)
        {
            var req = new ButiEngine.ResourceLoadInformation();
            req.FilePath = arg_filePath;
            arg_data.List_textures.ForEach(path => req.Textures.Add(path));
            arg_data.List_geometryShaders.ForEach(path => req.Textures.Add(path));
            arg_data.List_sounds.ForEach(path => req.Sounds.Add(path));
            arg_data.List_scripts.ForEach(path => req.Scripts.Add(path));
            arg_data.List_models.ForEach(path => req.Models.Add(path));
            arg_data.List_motions.ForEach(path => req.Motions.Add(path));
            arg_data.List_fonts.ForEach(path => req.Fonts.Add(path));
            arg_data.List_vertexShaders.ForEach(path => req.VertexShaders.Add(path));
            arg_data.List_pixelShaders.ForEach(path => req.PixelShaders.Add(path));
            arg_data.List_geometryShaders.ForEach(path => req.GeometryShaders.Add(path));

            arg_data.List_materials.ForEach(material =>
            {
                var materialInfo = new ButiEngine.MaterialLoadInformation();
                materialInfo.FilePath = material.filePath;
                materialInfo.MaterialName = material.materialName;
                materialInfo.Diffuse = new ButiEngine.Vector4_message() { X = material.var.diffuse.X, Y = material.var.diffuse.Y, Z = material.var.diffuse.Z, W = material.var.diffuse.W };
                materialInfo.Ambient = new ButiEngine.Vector4_message() { X = material.var.ambient.X, Y = material.var.ambient.Y, Z = material.var.ambient.Z, W = material.var.ambient.W };
                materialInfo.Emissive = new ButiEngine.Vector4_message() { X = material.var.emissive.X, Y = material.var.emissive.Y, Z = material.var.emissive.Z, W = material.var.emissive.W };
                materialInfo.Specular = new ButiEngine.Vector4_message() { X = material.var.specular.X, Y = material.var.specular.Y, Z = material.var.specular.Z, W = material.var.specular.W };

                material.material_list_textures.ForEach(materialTexture => materialInfo.Textures.Add(materialTexture));                
                req.MaterialLoadInfo.Add(materialInfo);
            });
            arg_data.List_shaders.ForEach(shader=>
            {
                var shaderInfo= new ButiEngine.ShaderLoadInformation();
                shaderInfo.ShaderName = shader.ShaderName;
                shaderInfo.VertexShaderName = shader.VertexShaderName;
                shaderInfo.PixelShaderName = shader.PixelShaderName;
                shaderInfo.GeometryShaderName = shader.GeometryShaderName;
                req.ShaderLoadInfo.Add(shaderInfo);
            });

            EditorClient.LoadedResourcesToFile(req);
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
        public int LoadShader(List<ResourceLoadViewModel.ShaderData> arg_datas)
        {
            var req = new ButiEngine.ShaderLoadInfoArray();
            arg_datas.ForEach(data => req.Values.Add(new ButiEngine.ShaderLoadInformation() { ShaderName = data.ShaderName, VertexShaderName = data.VertexShader, PixelShaderName = data.PixelShader, GeometryShaderName = data.GeometryShader }));
            return EditorClient.LoadShader(req).Value;
        }
        public int LoadMaterial(List<ResourceLoadModel.MaterialLoadInfo> arg_datas)
        {
            var req = new ButiEngine.MaterialLoadInfoArray();
            arg_datas.ForEach(data => req.Values.Add(new ButiEngine.MaterialLoadInformation()
            {
                MaterialName = data.materialName,
                Diffuse = new ButiEngine.Vector4_message() { X = data.var.diffuse.X, Y = data.var.diffuse.Y, Z = data.var.diffuse.Z, W = data.var.diffuse.W },
                Ambient = new ButiEngine.Vector4_message() { X = data.var.ambient.X, Y = data.var.ambient.Y, Z = data.var.ambient.Z, W = data.var.ambient.W },
                Emissive = new ButiEngine.Vector4_message() { X = data.var.emissive.X, Y = data.var.emissive.Y, Z = data.var.emissive.Z, W = data.var.emissive.W },
                Specular = new ButiEngine.Vector4_message() { X = data.var.specular.X, Y = data.var.specular.Y, Z = data.var.specular.Z, W = data.var.specular.W },
            }));
            return EditorClient.LoadMaterial(req).Value;
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
