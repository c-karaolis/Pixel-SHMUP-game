// custom property values
float _Lit;

// camera 1
sampler2D _Cam1_Texture;
vector _Cam1_Rect;
float _Cam1_Rotation;

// camera 2
sampler2D _Cam2_Texture;
vector _Cam2_Rect;
float _Cam2_Rotation;

bool InCamera (float2 pos, float2 rectSize) {
    float2 rectPos = -rectSize / 2;
    return (pos.x < rectPos.x || pos.x > rectPos.x + rectSize.x || pos.y < rectPos.y || pos.y > rectPos.y + rectSize.y) == false;
}

float2 TransformToCamera(float2 pos, float rotation) {
    float c = cos(-rotation);
    float s = sin(-rotation);

    float x = pos.x;
    float y = pos.y;

    pos.x = x * c - y * s;
    pos.y = x * s + y * c;

    return(pos);
}

fixed4 LightmapColor(float id, float2 texcoord) {
    switch(id) {
        case 0:
            return(tex2D (_Cam1_Texture, texcoord));

        case 1:
            return(tex2D (_Cam2_Texture, texcoord));
    }

    return(fixed4(0, 0, 0, 0));
}

fixed4 LightColor(float2 worldPos) {
    fixed4 color = fixed4(1,1,1,1);

    if (_Cam1_Rect.z > 0) {
        float2 camera_1_Size = float2(_Cam1_Rect.z, _Cam1_Rect.w);
        float2 posInCamera1 = TransformToCamera(worldPos - float2(_Cam1_Rect.x, _Cam1_Rect.y),  _Cam1_Rotation);

        if (InCamera(posInCamera1, camera_1_Size)) {
            color = LightmapColor(0, (posInCamera1 + camera_1_Size / 2) / camera_1_Size);
        }
    }

    if (_Cam2_Rect.z > 0) {
        float2 camera_2_Size = float2(_Cam2_Rect.z, _Cam2_Rect.w);
        float2 posInCamera2 = TransformToCamera(worldPos - float2(_Cam2_Rect.x, _Cam2_Rect.y),  _Cam2_Rotation);
        
        if (InCamera(posInCamera2, camera_2_Size)) {
            color = LightmapColor(1, (posInCamera2 + camera_2_Size / 2) / camera_2_Size);
        }
    }

    return(color);
}

fixed4 Lighting_Lit(fixed4 spritePixel, float2 worldPos) {
    fixed4 lightPixel = LightColor(worldPos);

    lightPixel = lerp(lightPixel, fixed4(1, 1, 1, 1), 1 - _Lit);

    lightPixel.a = 1;

    return spritePixel * lightPixel;
}

fixed4 Lighting_FogOfWar(fixed4 spritePixel, float2 worldPos) {
    fixed4 lightPixel = LightColor(worldPos);

    float multiplier = (lightPixel.r + lightPixel.g + lightPixel.b) / 3;

    spritePixel.a *= multiplier;

    if (spritePixel.a > 1) {
        spritePixel.a = 1;
    }

    return spritePixel;
}
        