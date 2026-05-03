#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Vuforia;

/// <summary>
/// Builds the entire Solar System AR scene automatically.
/// Menu: SolarAR → Build Scene
///
/// Prerequisites:
///   1. Vuforia Engine package imported
///   2. Run "SolarAR → Create Planet Materials" first
///   3. Your Vuforia Image Target Database imported (named "SolarSystemDB")
///      with targets named "Earth", "Mars", "Saturn"
/// </summary>
public class SceneBuilder : Editor
{
    [MenuItem("SolarAR/Build Scene")]
    static void BuildScene()
    {
        // --- 1. AR Camera ---
        if (GameObject.Find("AR Camera") == null)
        {
            GameObject arCam = (GameObject)PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Vuforia/Prefabs/AR Camera.prefab"));
            if (arCam == null)
            {
                Debug.LogError("[SolarAR] AR Camera prefab not found. " +
                    "Make sure Vuforia is imported. " +
                    "Try: GameObject > Vuforia Engine > AR Camera manually.");
                return;
            }
            arCam.name = "AR Camera";
        }

        // --- 2. Directional Light ---
        if (GameObject.Find("Directional Light") == null)
        {
            GameObject light = new GameObject("Directional Light");
            Light l = light.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = 1.2f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        // --- 3. Three Image Targets ---
        CreateImageTarget("Earth",  "SolarSystemDB", new Vector3(-0.15f, 0, 0),
            new Color(0.18f, 0.42f, 0.78f), 23.5f, false);
        CreateImageTarget("Mars",   "SolarSystemDB", new Vector3(0f,    0, 0),
            new Color(0.75f, 0.28f, 0.10f), 25.0f, false);
        CreateImageTarget("Saturn", "SolarSystemDB", new Vector3( 0.15f, 0, 0),
            new Color(0.87f, 0.78f, 0.55f), 26.7f, true);

        Debug.Log("[SolarAR] Scene built! " +
            "Remember to enter your Vuforia License Key in the AR Camera inspector, " +
            "and set the correct Image Target database + target names in each ImageTarget.");
    }

    static void CreateImageTarget(string planetName, string dbName,
        Vector3 pos, Color color, float tilt, bool hasSaturnRing)
    {
        // Load ImageTarget prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Vuforia/Prefabs/Image Target.prefab");

        GameObject imageTarget;
        if (prefab != null)
            imageTarget = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        else
        {
            // Fallback: plain GameObject (you'll configure it manually)
            imageTarget = new GameObject();
        }

        imageTarget.name = "ImageTarget_" + planetName;
        imageTarget.transform.position = pos;

        // Configure ImageTargetBehaviour
        var itb = imageTarget.GetComponent<ImageTargetBehaviour>();
        if (itb != null)
        {
            // These are set via SerializedObject because the fields may be read-only
            SerializedObject so = new SerializedObject(itb);
            so.FindProperty("m_DatabaseName").stringValue  = dbName;
            so.FindProperty("m_ImageTargetName").stringValue = planetName;
            so.ApplyModifiedProperties();
        }

        // Add tracking handler script
        var handler = imageTarget.AddComponent<PlanetTrackingHandler>();
        handler.planetName = planetName;

        // --- Planet Sphere ---
        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.name = planetName + "Sphere";
        planet.transform.SetParent(imageTarget.transform);
        planet.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        planet.transform.localScale    = new Vector3(0.08f, 0.08f, 0.08f);

        // Material
        string matPath = $"Assets/Materials/{planetName}Mat.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat != null)
            planet.GetComponent<Renderer>().sharedMaterial = mat;
        else
        {
            // Fallback: create inline material
            Material fallback = new Material(Shader.Find("Standard")) { color = color };
            planet.GetComponent<Renderer>().sharedMaterial = fallback;
        }

        // Rotation script
        var rot = planet.AddComponent<PlanetRotation>();
        rot.rotationSpeed = 40f;
        rot.axialTilt     = tilt;

        // Saturn ring
        if (hasSaturnRing)
        {
            var ring = planet.AddComponent<SaturnRingBuilder>();
            Material ringMat = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/Materials/SaturnRingMat.mat");
            ring.ringMaterial = ringMat;
        }

        // --- World-space Label Canvas ---
        GameObject canvasGO = new GameObject(planetName + "Canvas");
        canvasGO.transform.SetParent(imageTarget.transform);
        canvasGO.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        canvasGO.transform.localScale    = new Vector3(0.001f, 0.001f, 0.001f);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rt = canvasGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 50f);

        // Label text
        GameObject textGO = new GameObject("Label");
        textGO.transform.SetParent(canvasGO.transform);

        RectTransform trt = textGO.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        UnityEngine.UI.Text txt = textGO.AddComponent<UnityEngine.UI.Text>();
        txt.text      = planetName;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.fontSize  = 28;
        txt.color     = Color.white;
        txt.font      = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Hook label to handler
        handler.planetLabel = txt;

        Selection.activeGameObject = imageTarget;
    }
}
#endif
