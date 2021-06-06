using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGLTF;

public class Export : MonoBehaviour
{
    public string outputPath = "d:/test.glb";

    public GameObject go;

    public bool export = false;

    public Material genericStandardMat;

    public Material roughnessToStandardMaterial;

    private Texture2D m_source = null;
    private RenderTexture m_dest = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(export)
        {
            var exporter = new GLTFSceneExporter(new[] { go.transform }, new ExportOptions());
            var appPath = Application.dataPath;
            var wwwPath = appPath.Substring(0, appPath.LastIndexOf("Assets")) + "www";
            exporter.SaveGLTFandBin(Path.Combine(wwwPath, "TestScene"), go.transform.name);

            Texture2D metallness = convertRoughnessToStandard(go);
            print("Metalness number of channels: " + metallness.graphicsFormat);
            //SaveTexturePNG(metallness);

            createStandardMaterialForGameObject(go, metallness);

            export = false;
        }
    }

    private void createStandardMaterialForGameObject(GameObject _go, Texture2D metalMap)
    {
        Material stdMat = new Material(Shader.Find("Standard"));
        Material oldMat = _go.GetComponent<Renderer>().material;

        stdMat.CopyPropertiesFromMaterial(oldMat);
        stdMat.SetFloat("_Glossiness", 1.0f);
        //stdMat.SetTexture("_MetalnessMap", metalMap);
        /*Texture2D blue = new Texture2D(2, 2);
        blue.SetPixel(0, 0, Color.blue);
        blue.Apply();*/
        stdMat.SetTexture("_MetallicGlossMap", metalMap);
        //stdMat.name = "Meuh";
        //Texture2D blue = new Texture2D(2, 2);

        SaveTexturePNG(metalMap);
        _go.GetComponent<Renderer>().sharedMaterial = stdMat;


    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    // Save Texture as PNG
    void SaveTexturePNG(Texture2D tex)
    {
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        //Object.Destroy(tex);

        // For testing purposes, also write to a file in the project folder
        File.WriteAllBytes(Application.dataPath + "/../metalness.png", bytes);
    }

    public Texture2D convertRoughnessToStandard(GameObject _go)
    {
        Material originalRoughnessMat = _go.GetComponent<Renderer>().sharedMaterial;
        print("Original material name: " + originalRoughnessMat.name);
        genericStandardMat.SetTexture("_MainTex",originalRoughnessMat.GetTexture("_MainTex"));
        genericStandardMat.SetTexture("_BumpMap", originalRoughnessMat.GetTexture("_BumpMap"));
        //genericStandardMat.SetTexture("_Occlusion", originalRoughnessMat.GetTexture("_Occlusion"));

        Texture2D metalMap = originalRoughnessMat.GetTexture("_MetallicGlossMap") as Texture2D;
        Texture2D roughnessMap = originalRoughnessMat.GetTexture("_SpecGlossMap") as Texture2D;

        roughnessToStandardMaterial.SetTexture("_MetalnessMap", metalMap);
        roughnessToStandardMaterial.SetTexture("_RoughnessMap", roughnessMap);

        int widthMap = metalMap.width;
        int heightMap = metalMap.height;

        if (m_dest)
        {
            if (RenderTexture.active == m_dest)
            {
                RenderTexture.active = null;
            }
            m_dest.Release();
            m_dest = null;
        }

        m_dest = new RenderTexture(widthMap, heightMap, 0);
        m_dest.Create();


        m_source = new Texture2D(2, 2);
        m_source.Apply();

        Graphics.Blit(m_source, m_dest, roughnessToStandardMaterial);

        return toTexture2D(m_dest); ;

    }
}
