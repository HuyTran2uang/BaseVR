#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public static class CustomHierarchy
{
    static Dictionary<string, string> icons = new Dictionary<string, string>()
    {
        //systems
        {"EventSystem", "EventSystem Icon" },
        {"GameManager", "GameManager Icon" },
        {"StandaloneInputModule", "StandaloneInputModule Icon" },
        {"Camera", "Camera Icon" },
        {"VideoPlayer", "VideoPlayer Icon" },

        //components
        {"Transform", "Transform Icon" },
        {"RectTransform", "RectTransform Icon" },


        //colliders
        {"BoxCollider", "BoxCollider Icon" },
        {"SphereCollider", "SphereCollider Icon" },
        {"CapsuleCollider", "CapsuleCollider Icon" },
        {"MeshCollider", "MeshCollider Icon" },
        {"TerrainCollider", "TerrainCollider Icon" },

        //mesh
        {"MeshFilter", "MeshFilter Icon" },
        {"MeshRenderer", "MeshRenderer Icon" },
        {"Material", "Material Icon" },
        {"SkinnedMeshRenderer", "SkinnedMeshRenderer Icon" },
        {"Animator", "Animator Icon" },
        {"Animation", "Animation Icon" },

        //env
        {"Light", "Light Icon" },
        {"PointLight", "Light Icon" },
        {"DirectionalLight", "DirectionalLight Icon" },
        {"SpotLight", "SpotLight Icon" },
        {"AreaLight", "AreaLight Icon" },
        {"RectangleLight", "AreaLight Icon" },
        {"DiscLight", "AreaLight Icon" },
        {"Terrain", "Terrain Icon" },
        {"WindZone", "WindZone Icon" },

        //UI
        {"Canvas", "Canvas Icon" },
        {"CanvasScaler", "CanvasScaler Icon" },
        {"GraphicRaycaster", "GraphicRaycaster Icon" },
        {"RectMask2D", "RectMask2D Icon" },
        {"CanvasRenderer", "CanvasRenderer Icon" },
        {"Image", "Image Icon" },
        {"Button", "Button Icon" },
        {"ScrollRect", "ScrollRect Icon" },
        {"Scrollbar", "Scrollbar Icon" },
        {"InputField", "InputField Icon" },
        {"Text", "Text Icon" },
        {"Dropdown", "Dropdown Icon" },
        {"Slider", "Slider Icon" },
        {"Toggle", "Toggle Icon" },
        {"RawImage", "RawImage Icon" },
        {"Mask", "Mask Icon" },
        {"VerticalLayoutGroup", "VerticalLayoutGroup Icon" },
        {"HorizontalLayoutGroup", "HorizontalLayoutGroup Icon" },
        {"GridLayoutGroup", "GridLayoutGroup Icon" },
        {"ContentSizeFitter", "ContentSizeFitter Icon" },
        {"AspectRatioFitter", "AspectRatioFitter Icon" },
        {"LayoutElement", "LayoutElement Icon" },

        //tmp
        {"TMP_Text", "Text Icon" },
        {"TextMeshProUGUI", "Text Icon" },
        {"TMP_InputField", "InputField Icon" },
        {"TMP_Dropdown", "Dropdown Icon" },

        //audio
        {"AudioListener", "AudioListener Icon" },
        {"AudioSource", "AudioSource Icon" },

        //2d
        {"SpriteRenderer", "SpriteRenderer Icon" },
        {"Grid", "Grid Icon" },
        {"Tilemap", "Tilemap Icon" },
        {"TilemapRenderer", "TilemapRenderer Icon" },
        {"TilemapCollider2D", "TilemapCollider2D Icon" },
        {"Rigidbody2D", "Rigidbody2D Icon" },
        {"CompositeCollider2D", "CompositeCollider2D Icon" },
        {"SpriteMask", "SpriteMask Icon" },
        {"SpriteShapeRenderer", "SpriteShapeRenderer Icon" },

        //effect
        {"LineRenderer", "LineRenderer Icon" },
        {"TrailRenderer", "TrailRenderer Icon" },
        {"ParticleSystem", "ParticleSystem Icon" },

        //network
        {"NetworkIdentity", "NetworkIdentity Icon" },
        {"NetworkManager", "NetworkManager Icon" },
        {"NetworkManagerHUD", "NetworkManagerHUD Icon" },
        {"NetworkTransformReliable", "NetworkTransform Icon" },
        {"NetworkTransform", "NetworkTransform Icon" },
        {"NetworkAnimator", "NetworkAnimator Icon" },
        {"NetworkDiscovery", "NetworkDiscovery Icon" },
        {"NetworkStartPosition", "NetworkStartPosition Icon" },
        {"KcpTransport", "NetworkView Icon" },
        {"DeviceAuthenticator", "NetworkView Icon" },

        //physic
        {"SpringJoint", "SpringJoint Icon" },
        {"SpringJoint2D", "SpringJoint2D Icon" },
        {"Rigidbody", "Rigidbody Icon" },
        {"WheelCollider", "WheelCollider Icon" },

        //scripts
        {"cs Script", "cs Script Icon" },
    };

    [MenuItem("Tools/Show Hierarchy Debug Mode _F3")]
    public static void ShowWindow()
    {
        isShowGui = !isShowGui;
    }

    static CustomHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static bool isShowGui = true;
    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (!isShowGui) return;
        UpdateIcon(instanceID, selectionRect);
        UpdateLine(instanceID, selectionRect);
    }

    static void UpdateIcon(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (obj != null)
        {
            // Duyệt qua tất cả các component của GameObject
            var components = obj.GetComponents<Component>();
            int i = 0;
            float x = selectionRect.x + selectionRect.width - 10;
            foreach (var component in components)
            {
                if(component == null) continue;
                string componentName = component.GetType().Name;

                //ignore if this is not defined component
                if (!icons.ContainsKey(componentName))
                {
                    Rect rect = new Rect(x - i * 16, selectionRect.y, 16, 16);
                    ShowIcon("cs Script", rect, componentName);
                    i++;
                    continue;
                }

                //light
                if (component is Light)
                {
                    var light = (Light)component;
                    componentName = light.type.ToString() + "Light";
                    if (!icons.ContainsKey(componentName))
                    {
                        componentName = "Light";
                    }
                }

                //show icon some defined components
                Rect iconRect = new Rect(x - i * 16, selectionRect.y, 16, 16);
                ShowIcon(componentName, iconRect);
                i++;

                //show icon material -> because material is not component 
                if (component is MeshRenderer)
                {
                    var render = (MeshRenderer)component;
                    if (render.sharedMaterials == null || render.sharedMaterials.Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        iconRect = new Rect(x - i * 16, selectionRect.y, 16, 16);
                        ShowIcon("Material", iconRect);
                        i++;
                    }
                }
            }
        }
    }

    private static void ShowIcon(string iconName, Rect rect, string name = "")
    {
        if(name == null)
        {
            name = iconName;
        }
        var icon = EditorGUIUtility.IconContent(icons[iconName]).image as Texture2D;
        var content = new GUIContent(icon, $"This is a {name}");
        GUI.Label(rect, content);
    }

    static void UpdateLine(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj != null)
        {
            // Line
            Rect lineVerRect = new Rect(selectionRect.x - 20, selectionRect.y, 1, selectionRect.height);
            Rect lineHorRect = new Rect(selectionRect.x - 20, selectionRect.y + 8, 10, 1);
            Color lineColor = new Color(0.4078431f, 0.4078431f, 0.4078431f, 1);
            EditorGUI.DrawRect(lineVerRect, lineColor);
            EditorGUI.DrawRect(lineHorRect, lineColor);
        }
    }
}

#endif