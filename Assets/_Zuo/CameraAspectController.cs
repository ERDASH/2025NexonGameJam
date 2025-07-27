using UnityEngine;

public class CameraAspectController : MonoBehaviour
{
    public static CameraAspectController Instance;

    private readonly float targetAspect = 16f / 9f;

    void Awake()
    {
        // 중복 방지 (DontDestroyOnLoad)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AdjustCamera();
    }

    void AdjustCamera()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = Camera.main;

        if (scaleHeight < 1f)
        {
            Rect rect = camera.rect;

            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;

            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;

            camera.rect = rect;
        }
    }

    void OnPreCull()
    {
        // 배경을 검게 칠해줌 (letterbox or pillarbox 영역)
        GL.Clear(true, true, Color.black);
    }
}