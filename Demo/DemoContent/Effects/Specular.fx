#define NUMBER 2

float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.
float3 AmbientColor = float3(.15, .15, .15);
float AmbientIntensity = 1;

float3 DiffuseColor = float3(.85, .85, .85);
float DiffuseIntensity = 1.0;

float3 SpecularColor = float3(1, 1, 1);
float SpecularIntensity = 1;
float Shininess = 200;

float3 LightPosition[NUMBER];
float3 LightDirection[NUMBER];
float ConeAngle[NUMBER];
float3 LightColor[NUMBER];
float LightFalloff[NUMBER];
int amount;

float3 CameraPosition;
bool isBlinn = false;
texture ModelTexture;
sampler	textureSampler = sampler_state {
	texture = <ModelTexture>;
	MinFilter = Anisotropic; // Minification Filter
	MagFilter = Anisotropic; // Magnification Filter
	MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 WorldPosition : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = normalize(mul(input.Normal, World));
	return output;
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float3 diffuseColor = DiffuseColor;
	diffuseColor *= tex2D(textureSampler, input.TextureCoordinate).rgb;
	float3 specDot1, specDot2;

	float3 totalLight = AmbientColor * AmbientIntensity;

	for (int i = 0; i < min(amount, NUMBER); i++)
	{
		float3 lightDir = normalize(LightPosition[i] - input.WorldPosition);
		float diffuse = saturate(dot(normalize(input.Normal), lightDir));

		float d = dot(-lightDir, normalize(LightDirection[i]));
		float a = cos(ConeAngle[i]);
		float att = 0;
		float3 view = normalize(mul(normalize(CameraPosition), World));
		
		if (a<d)
			att = 1 - pow(clamp(a / d, 0, 1), LightFalloff[i]);
		totalLight += diffuse * att * LightColor[i];

		if (!isBlinn)
		{
			specDot1 = normalize(2 * dot(lightDir, input.Normal)*input.Normal - lightDir);
			specDot2 = view;
		}
		else
		{
			specDot1 = input.Normal;
			specDot2 = (lightDir + view) / length(lightDir + view);
		}
		totalLight += pow(saturate(dot(specDot1, specDot2)), Shininess) * SpecularColor;
	}
	return float4(DiffuseIntensity * diffuseColor * totalLight, 1);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
