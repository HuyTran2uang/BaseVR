using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class FolderBackgroundColorEditor
{
    static FolderBackgroundColorEditor()
    {
        // Lắng nghe sự kiện vẽ lại Project view
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect rect)
    {
        // Lấy đường dẫn của item trong Project
        string path = AssetDatabase.GUIDToAssetPath(guid);

        // Kiểm tra xem item có phải là folder không
        if (AssetDatabase.IsValidFolder(path))
        {
            // Lấy tên thư mục
            string folderName = System.IO.Path.GetFileName(path);

            // Xác định màu sắc dựa trên tên thư mục
            if(_colors.TryGetValue(folderName, out Color folderColor))
            {
                // Vẽ màu nền cho thư mục
                Rect newRect = new Rect(rect.x, rect.y, rect.width, rect.height * .95f);
                EditorGUI.DrawRect(newRect, folderColor);
            }
            
            AddVerticalLine(rect);
        }
    }

    private static Dictionary<string, Color> _colors = new Dictionary<string, Color>()
    {
        { "_Main", new Color(0.5f, 0.8f, 0.5f, 0.2f) },
        { "Scripts", new Color(0.8f, 0.5f, 0.5f, 0.2f) },
        { "Prefabs", new Color(0.5f, 0.5f, 0.8f, 0.2f) },
        { "Sprites", new Color(0.5f, 0.5f, 0.5f, 0.2f) },
    };
    
    // Hàm để vẽ một đường kẻ dọc phía trước mỗi folder
    private static void AddVerticalLine(Rect rect)
    {
        float lineWidth = 2f; // Độ rộng của đường kẻ dọc
        float lineXPosition = rect.x - lineWidth - 20.25f; // Vị trí đường kẻ dọc phía bên trái folder

        // Vẽ đường kẻ dọc với màu sắc tùy chỉnh (màu đen trong ví dụ này)
        EditorGUI.DrawRect(new Rect(lineXPosition, rect.y, lineWidth, rect.height), new Color(0.5f, 0.5f, 0.5f, 0.2f));
    }
}