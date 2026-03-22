using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;

public class SpritePivotChanger : EditorWindow
{
    private Vector2 customPivot = new Vector2(0.31f, -0.04f);

    [MenuItem("Tools/Bulk Sprite Pivot Changer")]
    public static void ShowWindow()
    {
        GetWindow<SpritePivotChanger>("Sprite Pivot Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Chỉnh Pivot Hàng Loạt (Dành cho ảnh)", EditorStyles.boldLabel);
        
        customPivot = EditorGUILayout.Vector2Field("Tọa độ Custom Pivot (X,Y)", customPivot);

        GUILayout.Space(10);
        if (GUILayout.Button("Áp dụng Pivot cho các Ảnh đang chọn"))
        {
            ChangePivot();
        }
    }

    private void ChangePivot()
    {
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Vui lòng chọn ít nhất 1 ảnh (Sprite) trong cửa sổ Project.");
            return;
        }

        int count = 0;
        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                // Xử lý ảnh xé lẻ (Multiple)
                if (importer.spriteImportMode == SpriteImportMode.Multiple)
                {
                    var factory = new SpriteDataProviderFactories();
                    factory.Init();
                    var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);

                    if (dataProvider != null)
                    {
                        dataProvider.InitSpriteEditorDataProvider();
                        var spriteRects = dataProvider.GetSpriteRects();
                        bool changed = false;
                        for (int i = 0; i < spriteRects.Length; i++)
                        {
                            spriteRects[i].alignment = SpriteAlignment.Custom;
                            spriteRects[i].pivot = customPivot;
                            changed = true;
                        }

                        if (changed)
                        {
                            dataProvider.SetSpriteRects(spriteRects);
                            dataProvider.Apply();
                        }
                    }
                }
                // Xử lý ảnh đơn (Single)
                else if (importer.spriteImportMode == SpriteImportMode.Single)
                {
                    TextureImporterSettings settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteAlignment = (int)SpriteAlignment.Custom;
                    importer.SetTextureSettings(settings);
                    importer.spritePivot = customPivot;
                }

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
                count++;
            }
        }

        Debug.Log($"Đã áp dụng thông số Pivot thành công cho {count} ảnh/spritesheet!");
    }
}
