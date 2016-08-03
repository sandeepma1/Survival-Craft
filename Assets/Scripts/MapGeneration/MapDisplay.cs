using UnityEngine;
using System.Collections;
using System.IO;

public class MapDisplay : MonoBehaviour
{
	public Renderer textureRender;

	public void DrawTexture (Texture2D texture)
	{
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, texture.height, texture.height);
		//SaveTextureToPNG (texture);
		//SaveTextureToFile (texture);
	}

	void SaveTextureToPNG (Texture2D tex)
	{		
		Texture2D MyTex;
		MyTex = new Texture2D (tex.width, tex.height);
		MyTex.SetPixels32 (tex.GetPixels32 ());
		MyTex.Apply ();
		byte[] bytes = MyTex.EncodeToPNG ();
		File.WriteAllBytes (Application.dataPath + ("/aa.png"), bytes);
		UnityEngine.Object.DestroyImmediate (MyTex);
	}

	void SaveTextureToFile (Texture2D tex)
	{
		/*for (int i = 0; i < tex.width; i++) {
			for (int j = 0; j < tex.height; j++) {
				print (tex.GetPixel (i, j));
			}
		}*/
		ES2.SaveImage (tex, "mytex.png");
	}
}
