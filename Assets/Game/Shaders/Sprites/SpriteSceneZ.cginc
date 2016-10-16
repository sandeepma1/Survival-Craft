#ifndef SPRITE_UNLIT_INCLUDED
#define SPRITE_UNLIT_INCLUDED

#include "ShaderShared.cginc"

////////////////////////////////////////
// Vertex structs
//
				
struct VertexInput
{
	float4 vertex : POSITION;
	noperspective float4 texcoord : TEXCOORD0;
	fixed4 color : COLOR;
};

struct VertexOutput
{
	float4 pos : SV_POSITION;
	noperspective float2 texcoord : TEXCOORD0;
	fixed4 color : COLOR;
#if defined(_FOG)
	UNITY_FOG_COORDS(1)
#endif // _FOG	
};

////////////////////////////////////////
// Vertex program
//

VertexOutput vert(VertexInput input)
{
	VertexOutput output;
	
	output.pos = calculateLocalPos(input.vertex);	
	output.texcoord = calculateTextureCoord(input.texcoord);
	output.color = calculateVertexColor(input.color);

#if defined(_FOG)
	UNITY_TRANSFER_FOG(output,output.pos);
#endif // _FOG	
	
	return output;
}

////////////////////////////////////////
// Fragment program
//

fixed4 frag(VertexOutput input) : SV_Target
{
	//Need to read from depth buffer and get a zero to 1 reading
	//this can be used to lerp the texture coords??
	//Need two verts to do this.
	//One if z depth is different then lerp the uvs non linearly between them
	//think cg setup means have to have a z value per vert and this gets lerped to give z value for pixel
	//this means no way of knowing wether the pixel is 
	
	//UNLESS!! use the verts screenspace normal as well. If the screen space normal is facing 90 from the camera then lerp the texcoord non linearly

	fixed4 texureColor = calculateTexturePixel(input.texcoord.xy);
	ALPHA_CLIP(texureColor, input.color)

	fixed4 pixel = calculatePixel(texureColor, input.color);
	
	COLORISE(pixel)
	
#if defined(_FOG)
	fixed4 fogColor = lerp(fixed4(0,0,0,0), unity_FogColor, pixel.a);
	UNITY_APPLY_FOG_COLOR(input.fogCoord, pixel, fogColor);
#endif // _FOG	
	
	return pixel;
}

#endif // SPRITE_UNLIT_INCLUDED