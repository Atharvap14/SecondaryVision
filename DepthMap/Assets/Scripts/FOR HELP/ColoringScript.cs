using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using OpenCvSharp.Demo;

public class ColoringScript : MonoBehaviour
{
    public MeshRenderer target; 
    public GameObject canvas; 
    public RawImage viewL, viewR; 
    UnityEngine.Rect capRect;
    Texture2D capTexture; 
    Texture2D colTexture; 
    Texture2D binTexture; 
    
    Mat bgr, bin;
    // Use this for initialization
    void Start()
    {
        int w = Screen.width;
        int h = Screen.height;
       
        int sx = (int)(w * 0.2f); 
        int sy = (int)(h * 0.3f); 
        w = (int)(w * 0.6f); 
        h = (int)(h * 0.4f); 
        
        capRect = new UnityEngine.Rect(sx, sy, w, h);
       
        capTexture = new Texture2D(w, h, TextureFormat.RGB24, false);
    }

    IEnumerator ImageProcessing()
    {
        canvas.SetActive(false);//CanvasUI
        yield return new WaitForEndOfFrame();
        CreateImage(); 
        Point[] corners; 
        FindRect(out corners);
        TransformImage(corners); 
        ShowImage(); 
        
        bgr.Release();
        bin.Release();
        canvas.SetActive(true);//CanvasUI
    }
    void TransformImage(Point[] corners)
    {
       
        if (corners == null) return;
       
        SortCorners(corners);
      
        Point2f[] input = { corners[0], corners[1],
                         corners[2], corners[3] };
       
        Point2f[] square =
                 { new Point2f(0, 0), new Point2f(0, 255),
                new Point2f(255, 255), new Point2f(255, 0) };
       
        Mat transform = Cv2.GetPerspectiveTransform(input, square);
       
        Cv2.WarpPerspective(bgr, bgr, transform, new Size(256, 256));
        int s = (int)(256 * 0.05f); 
        int w = (int)(256 * 0.9f);
        OpenCvSharp.Rect innerRect = new OpenCvSharp.Rect(s, s, w, w);
        bgr = bgr[innerRect];

    }
    void SortCorners(Point[] corners)
    {
        System.Array.Sort(corners, (a, b) => a.X.CompareTo(b.X));
        if (corners[0].Y > corners[1].Y)
        {
            corners.Swap(0, 1);
        }
        if (corners[3].Y > corners[2].Y)
        {
            corners.Swap(2, 3);
        }
    }
    void FindRect(out Point[] corners)
    {
       
        corners = null;
       
        Point[][] contours;
        HierarchyIndex[] h;
       
        bin.FindContours(out contours, out h, RetrievalModes.External,
                             ContourApproximationModes.ApproxSimple);
      
        double maxArea = 0;
        for (int i = 0; i < contours.Length; i++)
        {
           
            double length = Cv2.ArcLength(contours[i], true);
           
            Point[] tmp = Cv2.ApproxPolyDP(contours[i], length * 0.01f, true);

            double area = Cv2.ContourArea(contours[i]);
            if (tmp.Length == 4 && area > maxArea)
            {
                maxArea = area;
                corners = tmp;
            }
        }
       
    }

    void CreateImage()
    {
        capTexture.ReadPixels(capRect, 0, 0);
        capTexture.Apply();
       
        bgr = OpenCvSharp.Unity.TextureToMat(capTexture);
       
        bin = bgr.CvtColor(ColorConversionCodes.BGR2GRAY);
       
        bin = bin.Threshold(100, 255, ThresholdTypes.Otsu);
        Cv2.BitwiseNot(bin, bin);
    }
    void ShowImage()
    {
        //Texture
        if (colTexture != null) { DestroyImmediate(colTexture); }
        if (binTexture != null) { DestroyImmediate(binTexture); }
        //Mat
        colTexture = OpenCvSharp.Unity.MatToTexture(bgr);
        binTexture = OpenCvSharp.Unity.MatToTexture(bin);
        //RawImage
        viewL.texture = colTexture;
        viewR.texture = binTexture;
        
        target.material.mainTexture = colTexture;
    }
    public void StartCV()
    {
        StartCoroutine(ImageProcessing());
    }

    // Update is called once per frame
    void Update()
    {

    }
}