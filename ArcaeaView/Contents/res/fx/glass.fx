#include "monogame.fxh"

// https://sogoodgames.wordpress.com/2017/09/11/glass-shader-with-opaque-edge-effect-in-unity/

float gOpacity;
float4 gTintColor;

float4x4 gWorldViewProjection;
float4x4 gWorld;
float4x4 gWorldInv;
float3 gCameraPosition;

float4 gEdgeColor;
float gEdgeThickness;

struct VS_IN {
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float4 Color : COLOR;
};

struct PS_IN {
    float4 PositionT : SV_POSITION;
    float4 Color : COLOR;
    float3 Normal : NORMAL;
    float3 ViewDir : COLOR1;
};

PS_IN vs(in VS_IN input) {
    PS_IN output = (PS_IN)0;

    output.PositionT = mul(float4(input.Position, 1.0f), gWorldViewProjection);
    output.Normal = normalize(mul(float4(input.Normal, 0.0f), gWorldInv).xyz);
    output.ViewDir = normalize(gCameraPosition - mul(gWorld, float4(input.Position, 1.0f)).xyz);
    output.Color = input.Color;

    return output;
}

float4 ps(in PS_IN input) : SV_TARGET {
    float4 output = input.Color;

    float edgeFactor = abs(dot(input.ViewDir, input.Normal));

    float oneMinusEdge = 1.0 - edgeFactor;
	float3 rgb = (input.Color.rgb * edgeFactor) + (gEdgeColor.rgb * oneMinusEdge);

	rgb = min(float3(1, 1, 1), rgb);
	rgb = rgb * output.rgb;

    float opacity = min(1.0, input.Color.a / edgeFactor);

    opacity = pow(abs(opacity), gEdgeThickness);
	opacity = opacity * output.a;

    output.a = opacity;
    
    float4 tint = gTintColor;
    tint.rgb *= tint.a;

    output.rgb *= output.a;
    output *= tint;

    output *= gOpacity;

    return output;
}

technique Glass {
    pass {
        VertexShader = compile VS_SHADERMODEL vs();
        PixelShader = compile PS_SHADERMODEL ps();
    }
}
