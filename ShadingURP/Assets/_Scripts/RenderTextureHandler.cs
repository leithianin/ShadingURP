using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureHandler : MonoBehaviour
{
    public RenderTexture rTex;

    private void Update()
    {
        
    }

    void UpdateRenderTexture()
    {
        //var tempTex = new RenderTexture(rTex.height, rTex.width, 24, RenderTextureFormat.ARGB32);

        //Graphics.Blit(new Texture2D(512, 512, TextureFormat.RGB24, false), rTex);
    }
}
