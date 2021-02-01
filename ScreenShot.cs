﻿using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class ScreenShot : MonoBehaviour
{
    static Camera mainCamera;
    static Camera uiCamera;

    private void OnEnable()
    {
        mainCamera = GetComponent<Camera>();
        uiCamera = GetComponent<Camera>();
    }

    /// <summary>  
    /// 对相机截图。   
    /// </summary>  
    /// <returns>The screenshot2.</returns>  
    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    static Texture2D CaptureCamera(Camera camera, Camera camera2 ,Rect rect)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, -1);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
        camera2.targetTexture = rt;
        camera2.Render();
        //ps: -------------------------------------------------------------------  
 
        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();
 
        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        camera2.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));
 
        return screenShot;
    }

    [MenuItem("/屏幕截图/点击截图")]
    static void ScreenShotMenu()
    {
        CaptureCamera(mainCamera, uiCamera,new Rect(0, 0, Screen.width, Screen.height));
    }
}