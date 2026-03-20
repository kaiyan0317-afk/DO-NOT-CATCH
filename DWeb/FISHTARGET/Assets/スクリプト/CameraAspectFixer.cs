using UnityEngine;

public class CameraAspectFixer : MonoBehaviour
{
    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // スマホの画面比率に関わらず、カメラのサイズを「5」に固定する
        // ※画像52枚目のGameビューで「16:9 Portrait」にした時のSize数値を入力してください
        cam.orthographicSize = 5f;

        // アスペクト比を9:16（16:9の縦）に強制計算する設定
        float targetAspect = 9f / 16f;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            cam.rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            cam.rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
        }
    }
}
