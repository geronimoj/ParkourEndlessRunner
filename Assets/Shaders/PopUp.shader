Shader "Custom/PopUp"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _WallTex ("Wall Texture", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Left("Left Clip Height", Float) = 0.0
        _Right("Right Clip Height", Float) = 0.0
        _Back("Back Clip Height", Float) = 0.0
        _Extrusion("Extrusion", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _WallTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_WallTex;
            float3 vertexNormal;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _DropEnd;
        float _Left;
        float _Right;
        float _Back;
        float _Extrusion;
        float4 _AABBP1;
        float4 _AABBP2;

        void vert(inout appdata_full v, out Input o)
        {   
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexNormal = v.normal;
            //Gets the world position of the vertex
            float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
            //Gets the distance to that point
            float dist = distance(worldPos, _WorldSpaceCameraPos);
            //Gets the distance past _DropEnd as a value between 0 and 1 to determine how much to reduce the y axis of the vertex by
            float sat = saturate(1 - _DropEnd / dist);
            //Calculate the new world position
            worldPos.y -= (sat * sat) * dist;

            o.worldPos = worldPos;
            //Swap the position back into object space from world space
            v.vertex = mul(unity_WorldToObject, float4(worldPos, 1.0));
            //Extrude the surface
            v.vertex.xyz += v.normal * _Extrusion;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {   //If the normal is pointing to the left, Cull it based on _Left.
            float3 left = float3(-1, 0, 0);
            float doCull = saturate(dot(IN.vertexNormal, left.xyz));
            clip(doCull * (IN.worldPos.y - _Left));
            //If the normal is pointing to the right, Cull it based on _Right.
            doCull = saturate(dot(IN.vertexNormal, -left.xyz));
            clip(doCull * (IN.worldPos.y - _Right));
            //If the normal is pointing towards the player, Cull it based on _Back.
            float3 back = float3(0, 0, -1);
            doCull = saturate(dot(IN.vertexNormal, back.xyz));
            clip(doCull * (IN.worldPos.y - _Back));
            //Now we clip it if its inside the AABB
            float3 p = float4(IN.worldPos, 1);
            float v = step((p.x - _AABBP1.x) * (p.x - _AABBP2.x), 0)
                * step((p.y - _AABBP1.y) * (p.y - _AABBP2.y), 0)
                * step((p.z - _AABBP1.z) * (p.z - _AABBP2.z), 0);

            clip(v * -1);
            //Get the dot onto the forward and right vector of the normal. This is used to determine if a surface is pointing upwards, aka, floor or wall
            //The value is made positive to reduce the number of checks
            float d = abs(dot(IN.vertexNormal, float3(1, 0, 1)));
            // Albedo comes from a texture tinted by color
            fixed4 c = d >= 0.9 ? tex2D(_WallTex, IN.uv_WallTex) : tex2D(_MainTex, IN.uv_MainTex);
            c *= _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
