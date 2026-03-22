using UnityEditor;
using UnityEngine;

public class BulkSpritePivotSetter : EditorWindow
{
    private Vector2 pivot = new Vector2(0.5f, 0f); // Bottom-center (thường dùng cho nhân vật đi trên mặt đất)
    private string folderPath = "Assets/Animation/ZombieBoss/Attack/Test";

    [MenuItem("Tools/Bulk Set Sprite Pivot")]
    public static void ShowWindow()
    {
        GetWindow<BulkSpritePivotSetter>("Set Sprite Pivots");
    }

    void OnGUI()
    {
        GUILayout.Label("Cài đặt Pivot hàng loạt cho Sprite", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        folderPath = EditorGUILayout.TextField("Đường dẫn trỏ đến thư mục", folderPath);
        pivot = EditorGUILayout.Vector2Field("Tọa độ Pivot (0 đến 1)", pivot);
        EditorGUILayout.HelpBox("Ví dụ: \n(0.5, 0.5) là Tâm điểm (Center)\n(0.5, 0) là Dưới cùng ở giữa (Bottom Center)", MessageType.Info);

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply to All Sprites (Áp dụng cho tất cả)"))
        {
            ApplyPivot();
        }
    }

    void ApplyPivot()
    {
        if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"[BulkSpritePivotSetter] Không tìm thấy thư mục: {folderPath}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        int count = 0;

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                EditorUtility.DisplayProgressBar("Đang cập nhật Pivot", $"Đang xử lý: {path}", (float)i / guids.Length);
                
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null)
                {
                    if (importer.textureType != TextureImporterType.Sprite)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                    }

                    TextureImporterSettings settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    
                    settings.spriteAlignment = (int)SpriteAlignment.Custom;
                    settings.spritePivot = pivot;
                    
                    importer.SetTextureSettings(settings);
                    importer.SaveAndReimport();
                    count++;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"✅ Đã cài đặt Pivot {pivot} thành công cho {count} sprites trong thư mục {folderPath}.");
    }
}
