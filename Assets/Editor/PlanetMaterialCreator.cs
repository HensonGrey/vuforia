#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility — run this once from the menu to auto-create
/// all planet materials in Assets/Materials/.
/// Menu: SolarAR → Create Planet Materials
/// </summary>
public class PlanetMaterialCreator : Editor
{
    [MenuItem("SolarAR/Create Planet Materials")]
    static void CreateMaterials()
    {
        CreatePlanetMaterial("EarthMat",
            new Color(0.18f, 0.42f, 0.78f),   // ocean blue
            new Color(0.25f, 0.55f, 0.25f));   // land green tint (not used in standard shader but kept for reference)

        CreatePlanetMaterial("MarsMat",
            new Color(0.75f, 0.28f, 0.10f),    // rust red
            new Color(0.75f, 0.28f, 0.10f));

        CreatePlanetMaterial("SaturnMat",
            new Color(0.87f, 0.78f, 0.55f),    // pale gold
            new Color(0.87f, 0.78f, 0.55f));

        CreateRingMaterial("SaturnRingMat",
            new Color(0.80f, 0.72f, 0.50f, 0.65f)); // semi-transparent gold

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[SolarAR] Planet materials created in Assets/Materials/");
    }

    static void CreatePlanetMaterial(string name, Color albedo, Color emission)
    {
        string path = $"Assets/Materials/{name}.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null)
        {
            Debug.Log($"[SolarAR] {name} already exists, skipping.");
            return;
        }

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = albedo;
        mat.SetFloat("_Metallic", 0.0f);
        mat.SetFloat("_Glossiness", 0.3f);
        AssetDatabase.CreateAsset(mat, path);
    }

    static void CreateRingMaterial(string name, Color color)
    {
        string path = $"Assets/Materials/{name}.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) return;

        Material mat = new Material(Shader.Find("Standard"));
        // Enable transparency
        mat.SetFloat("_Mode", 3);  // Transparent
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
    }
}
#endif
