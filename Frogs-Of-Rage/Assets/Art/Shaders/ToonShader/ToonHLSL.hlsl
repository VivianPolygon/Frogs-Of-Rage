void ToonColor_float(in float3 worldNormal, in float4 color, 
in float detail, in float brightness, in float segmentColorDiffrence,
out float4 materialShading)
{
    #ifdef SHADERGRAPH_PREVIEW
    materialShading = float4(0.5, 0.5, 0.5, 0);
    #else

    Light light = GetMainLight();


    materialShading = color;
    float shading = clamp(dot(light.direction, worldNormal), 0, 1) ;

    shading *= detail;
    shading = round(shading);
    shading *= segmentColorDiffrence;

    materialShading *= shading;
    materialShading += brightness;


    #endif
}

void ToonSmoothness_float(in float3 cameraDirection, float3 worldNormal,
in float detail, in float normalMapDot,
out float3 smoothness)
 {
    #ifdef SHADERGRAPH_PREVIEW
    smoothness = (1, 1, 1);
    #else

    Light light = GetMainLight();


    smoothness = reflect(cameraDirection, worldNormal) - dot(reflect(cameraDirection, worldNormal), light.direction) - normalMapDot;
    float tooning = (smoothness.x + smoothness.y + smoothness.z) / 3;
    tooning *= detail;
    tooning = round(tooning);
    smoothness = (tooning, tooning, tooning);

    #endif
 }

 void GetMainLightColor_float(out float3 _lightColor)
 {
    #ifdef SHADERGRAPH_PREVIEW
    _lightColor = (1, 1, 1);
    #else
    Light light = GetMainLight();
    _lightColor = light.color;
    #endif
 }

void GetMainLightDirection_float(out float3 _lightDirection)
{
    #ifdef SHADERGRAPH_PREVIEW
    _lightDirection = (1, 1, 1);
    #else
    _lightDirection = GetMainLight().direction;
    #endif
}

  void ToonRound_float(in float4 colorIn, in float detail, out float4 colorOut)
 {
    #ifdef SHADERGRAPH_PREVIEW
    colorOut = (1,1,1,1);
    #else
    colorIn *= detail;
    colorOut = round(colorIn);
    #endif
 }

void NormalMap_float(in float3 NormalMap, out float3 MicroNormal)
{
    #ifdef SHADERGRAPH_PREVIEW
    MicroNormal = (0,0,1);
    #else
    MicroNormal =  normalize(NormalMap);
    #endif
}
