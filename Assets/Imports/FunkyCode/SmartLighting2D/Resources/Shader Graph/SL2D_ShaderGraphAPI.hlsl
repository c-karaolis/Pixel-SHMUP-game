
// Internal API

float2 TransformToCamera(float2 pos, float rotation) {
    float c = cos(-rotation);
    float s = sin(-rotation);

    float x = pos.x;
    float y = pos.y;

    pos.x = x * c - y * s;
    pos.y = x * s + y * c;

    return(pos);
}

bool InCamera (float2 pos, float2 rectPos, float2 rectSize) {
    rectPos -= rectSize / 2;
    return (pos.x < rectPos.x || pos.x > rectPos.x + rectSize.x || pos.y < rectPos.y || pos.y > rectPos.y + rectSize.y) == false;
}

// Shader Graph API

void LightmapWhiteOutside_float(float4 rect, float rotation, float2 world, float4 incolor, out float4 outcolor) {

    if (rect.z > 0) {
        float2 cameraSize = float2(rect.z, rect.w);
        float2 posInCamera = TransformToCamera(world - float2(rect.x, rect.y), rotation);

        if (InCamera(posInCamera, float2(0, 0), cameraSize)) {
            outcolor = incolor;
        } else {
            outcolor = float4(1, 1, 1, 1);
        }

    } else {
        outcolor = float4(1, 1, 1, 1);
    }
}

void LightmapLit_float(float4 incolor, float lit, out float4 outcolor) {
    
    outcolor = lerp(incolor, float4(1, 1, 1, 1), 1 - lit);

}

void LightmapUV_float(float4 rect, float rotation, float2 world, out float2 uv) {

    if (rect.z > 0) {
        float2 cameraSize = float2(rect.z, rect.w);
        float2 posInCamera = TransformToCamera(world - float2(rect.x, rect.y), rotation);

        if (InCamera(posInCamera, float2(0, 0), cameraSize)) {
            uv = (posInCamera + cameraSize / 2) / cameraSize;
        } else {
            uv = float2(0, 0);
        }

    } else {
        uv = float2(0, 0);
    }
}

void BlendFogOfWar_float(float4 incolor, float4 lightmap, out float4 outcolor) {

    outcolor = incolor;

    outcolor.a *= (lightmap.r + lightmap.g + lightmap.b) / 3;

    if (outcolor.a > 1) {
        outcolor.a = 1;
    }
}

/*
void CustomLight_float(float4 rect, float rotation, float2 world, out float4 color) {

    if (rect.z > 0) {
        float2 cameraSize = float2(rect.z, rect.w);
        float2 posInCamera = TransformToCamera(world - float2(rect.x, rect.y), rotation);

        if (InCamera(posInCamera, float2(0, 0), cameraSize)) {
            color = float4(1, 1, 1, 1);
        } else {
            color = float4(0, 0, 0, 0);
        }

    } else {
        color = float4(0, 0, 0, 0);
    }
}

*/