using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Livet;
using Newtonsoft.Json;
using System.IO;
using System.Numerics;
using ButiEngineEditor.Models.Modules;

namespace ButiEngineEditor.Models
{
    public class ResourceLoadModel : NotificationObject
    {
        [JsonObject("ShaderLoadInfo")]
        public class ShaderLoadInfo {
            [JsonProperty("ShaderName")]
            public string ShaderName { get; set; }
            [JsonProperty("VertexShaderName")]
            public string VertexShaderName { get; set; }
            [JsonProperty("PixelShaderName")]
            public string PixelShaderName { get; set; }
            [JsonProperty("GeometryShaderName")]
            public string GeometryShaderName { get; set; }
        }
        [JsonObject("MaterialValue")]
        public struct MaterialValue
        {
            [JsonProperty("emissive")]
            public Vector4 emissive;
            [JsonProperty("diffuse")]
            public Vector4 diffuse;
            [JsonProperty("ambient")]
            public Vector4 ambient;
            [JsonProperty("specular")]
            public Vector4 specular;
            [JsonProperty("roughness")]
            public float roughness;
        }
        [JsonObject("MaterialLoadData")]
        public class MaterialLoadInfo{
            [JsonProperty("material_materialName")]
            public string materialName;
            [JsonProperty("material_filePath")]
            public string filePath = "none";
            [JsonProperty("material_var")]
            public MaterialValue var;
            [JsonProperty("material_list_textures")]
            public List<string> material_list_textures;
        }
        [JsonObject("ResourceLoadData")]
        public class ResourceLoadData {
            public ResourceLoadData()
            {
                _list_meshes= new List<string>();
                _list_textures= new List<string>();
                _list_renderTargets = new List<string>();
                _list_sounds= new List<string>();
                _list_motions= new List<string>();
                _list_models= new List<string>();
                _list_scripts = new List<string>();
                _list_fonts= new List<string>();
                _list_vertexShaders= new List<string>();
                _list_pixelShaders= new List<string>();
                _list_geometryShaders= new List<string>();
                _list_materials = new List<MaterialLoadInfo>();
                _list_shaders = new List<ShaderLoadInfo>();
            }
            //パスを指定して読み込むリソース
            [JsonIgnore]
            public List<string> List_meshes{ get { return _list_meshes; } }
            public List<string> List_textures{ get { return _list_textures; } }
            public List<string> List_renderTargets{ get { return _list_renderTargets; } }
            public List<string> List_sounds{ get { return _list_sounds; } }
            public List<string> List_models{ get { return _list_models; } }
            public List<string> List_motions{ get { return _list_motions; } }
            public List<string> List_scripts{ get { return _list_scripts; } }
            public List<string> List_fonts{ get { return _list_fonts; } }
            public List<string> List_vertexShaders{ get { return _list_vertexShaders; } }
            public List<string> List_pixelShaders{ get { return _list_pixelShaders; } }
            public List<string> List_geometryShaders{ get { return _list_geometryShaders; } }
            //パス以外の内容があるモノ
            public List<ShaderLoadInfo> List_shaders{ get { return _list_shaders; } }
            public List<MaterialLoadInfo> List_materials{ get { return _list_materials; } }
            [JsonIgnore]
            public List<string> _list_meshes;
            [JsonIgnore]
            public List<string> _list_textures;
            [JsonIgnore]
            public List<string> _list_renderTargets;
            [JsonIgnore]
            public List<string> _list_sounds;
            [JsonIgnore]
            public List<string> _list_models;
            [JsonIgnore]
            public List<string> _list_motions;
            [JsonIgnore]
            public List<string> _list_scripts;
            [JsonIgnore]
            public List<string> _list_fonts;
            [JsonIgnore]
            public List<string> _list_vertexShaders;
            [JsonIgnore]
            public List<string> _list_pixelShaders;
            [JsonIgnore]
            public List<string> _list_geometryShaders;
            [JsonIgnore]
            public List<ShaderLoadInfo> _list_shaders;
            [JsonIgnore]
            public List<MaterialLoadInfo> _list_materials;
        }
        private ResourceLoadData _data;
        private string _filePath;

        public ResourceLoadData Data { get { return _data; } }
        public bool MaterialAddition { get { RaisePropertyChanged(); return true;}set { RaisePropertyChanged(); } }
        public bool RenderTargetAddition { get { RaisePropertyChanged(); return true;}set { RaisePropertyChanged(); } }
        public bool ShaderAddition { get { RaisePropertyChanged(); return true;}set { RaisePropertyChanged(); } }
        public string GetFilePath { get { return _filePath; } }

        public ResourceLoadModel(string arg_filePath)
        {
            _filePath = arg_filePath;
        }

        public void FileInput()
        {
            if (!File.Exists(_filePath))
            {
                _data = new ResourceLoadData();
                return;
            }

            SyncRuntime();

            //using (var reader = new StreamReader(_filePath, Encoding.UTF8))
            //{
            //    _data= JsonConvert.DeserializeObject<ResourceLoadData>(reader.ReadToEnd());
            //}
        }
        public void FileOutput()
        {
            string jsonStr = JsonConvert.SerializeObject(_data);

            using (var writer = new StreamWriter(_filePath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }
        public void ToBinary()
        {
            ButiEngineIO.ReourceLoadDataToBinary(_data, "resourceLoadData.resourceLoad");
        }
        public void SyncRuntime()
        {
            _data= ButiEngineIO.GetLoadedResourceData();
        }
    }
}
