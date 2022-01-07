using ButiEngineEditor.ViewModels.Panes;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ButiEngineEditor.Models
{
    public enum RenderTargetFormat
    {
        R8G8B8A8_UNORM = 28, R32G32B32A32_FLOAT = 2, R32G32B32_FLOAT = 6, R32G32_FLOAT = 16, R32_FLOAT = 41
    }
    class MaterialCreateModel: NotificationObject
    {
        public Color Diffuse { get; set; } = Color.FromArgb(255,255,255,255);
        public Color Ambient { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public Color Emissive { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public Color Specular { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public string MaterialName { get; set; } = "";
        public List<ResourceLoadViewModel.TextureData> List_currentSelectTextures { get; set; } =new List<ResourceLoadViewModel.TextureData>();

    }
    class RenderTargetCreateModel : NotificationObject
    {
        public string RenderTargetName { get; set; } = "";
        public int Width { get; set; } = 1024;
        public int Height { get; set; } = 1024;
        public RenderTargetFormat Format { get; set; } = RenderTargetFormat.R8G8B8A8_UNORM;

    }
    class ShaderCreateModel : NotificationObject
    {
        public string ShaderName { get; set; } = "";
        public ResourceLoadViewModel.VerexShaderData VertexShader { get; set; } = new ResourceLoadViewModel.VerexShaderData() { Title = "None", FilePath = "" };
        public ResourceLoadViewModel.PixelShaderData PixelShader { get; set; } = new ResourceLoadViewModel.PixelShaderData() { Title = "None", FilePath = "" };
        public ResourceLoadViewModel.GeometryShaderData GeometryShader { get; set; } = new ResourceLoadViewModel.GeometryShaderData() { Title = "None", FilePath = "" };
    }


}
