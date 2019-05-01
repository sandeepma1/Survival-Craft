using System.Collections;
using System.IO;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer textureRender;

    public void DrawTexture(Texture2D texture)
    {
        textureRender = GetComponent<Renderer>();
        textureRender.material.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, texture.height, 1);
    }

    private void SaveTextureToPNG(Texture2D tex)
    {
        Texture2D MyTex;
        MyTex = new Texture2D(tex.width, tex.height);
        MyTex.SetPixels32(tex.GetPixels32());
        MyTex.Apply();
        byte[] bytes = MyTex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + ("/aa.png"), bytes);
        DestroyImmediate(MyTex);
    }

    private void SaveTextureToFile(Texture2D tex)
    {
        ES2.SaveImage(tex, "mytex.png");
    }
}
