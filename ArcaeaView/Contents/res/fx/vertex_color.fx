#include "monogame.fxh"

float gOpacity;
float4 gTintColor;
float4x4 gWorldViewProjection;

struct VS_IN {
    float3 Position : Position;
    float4 Color : COLOR;
};

struct PS_IN {
    float4 PositionT : SV_POSITION;
    float4 Color : COLOR;
};

PS_IN vs(in VS_IN input) {
    PS_IN output = (PS_IN)0;

    output.PositionT = mul(float4(input.Position, 1.0f), gWorldViewProjection);
    output.Color = input.Color;

    return output;
}

float4 ps(in PS_IN input) : SV_TARGET {
    float4 output = input.Color;
    
    float4 tint = gTintColor;
    tint.rgb *= tint.a;

    output.rgb *= output.a;
    output *= tint;

    output *= gOpacity;

    return output;
}

technique VertexColor {
    pass {
        VertexShader = compile VS_SHADERMODEL vs();
        PixelShader = compile PS_SHADERMODEL ps();
    }
}
