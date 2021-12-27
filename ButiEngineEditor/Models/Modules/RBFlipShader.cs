using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;

namespace ButiEngineEditor.Models.Modules
{
    class RBFlipShader : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(RBFlipShader), 0);
        public RBFlipShader()
        {
            string hlsl = @"
sampler2D input : register(s0);
float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 output=tex2D(input, uv);
    float b=output.x;
    output.r=output.z;
    output.b=b;
    return output;
}";

            var pixelShader = new PixelShader();
            var compileResult = SharpDX.D3DCompiler.ShaderBytecode.Compile(hlsl, "main", "ps_3_0");
            using (var ms = new System.IO.MemoryStream(compileResult.Bytecode))
            {
                pixelShader.SetStreamSource(ms);
            }


            this.PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
        }
    }
}
