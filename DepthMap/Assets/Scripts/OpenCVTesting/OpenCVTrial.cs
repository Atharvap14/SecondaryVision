using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;


public class OpenCVTrial : MonoBehaviour
{
    WebCamTexture webCamTexture;
    public UnityEngine.Video.VideoPlayer vp;
    Mat src_gray , frame;
    Point[][] contours ;
    HierarchyIndex[] heirarchy;
    Texture texture;
    public RawImage raw;
    public Button btn;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        btn.onClick.AddListener(playvideo);
        webCamTexture = new WebCamTexture(devices[0].name);
        webCamTexture.Play();
        
        
        
    }
    void playvideo()
    {

        StartCoroutine(ImageProcessing());
            
        
        
    }
    private void Update()
    {
        detectobj();
        frame.Release();
        src_gray.Release();
    }
    IEnumerator ImageProcessing()
    {
        
        yield return new WaitForEndOfFrame();
        
        detectobj();
        frame.Release();
        src_gray.Release();
        

    }

    void detectobj()
    {
        src_gray = new Mat();
        Texture mainTexture = vp.texture;
        Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);
        Graphics.Blit(mainTexture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new UnityEngine.Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        Color[] pixels = texture2D.GetPixels();

        RenderTexture.active = currentRT;
        frame = OpenCvSharp.Unity.TextureToMat(texture2D);
        RNG rng= new RNG(12345);

        Cv2.Threshold(frame, frame, 50,255, ThresholdTypes.Binary);
        
        Cv2.CvtColor(frame , src_gray , OpenCvSharp.ColorConversionCodes.RGB2GRAY);

        Size size = src_gray.Size();
        Cv2.PyrDown(src_gray, src_gray);
        Size size2 = src_gray.Size();

        Cv2.Canny(src_gray,  src_gray, 30, 200);
        Cv2.WaitKey(0);
        Cv2.FindContours(src_gray, out  contours, out heirarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);

        Point[] contour_poly = new Point[contours.Length]  ;
      
        List<OpenCvSharp.Rect> bounding = new List<OpenCvSharp.Rect> ( contours.Length);
       
        foreach(Point[] contour in contours)
        {
           contour_poly= Cv2.ApproxPolyDP(contour,  3, false);
            for ( int i=0;i< contour_poly.Length;i++){
                contour_poly[i].X *= size.Width / size2.Width;
                contour_poly[i].Y *= size.Height / size2.Height;

            }
            bounding.Add( Cv2.BoundingRect(contour_poly));

        
            
        }
        List<OpenCvSharp.Rect> keep = new List<OpenCvSharp.Rect>();
        int length = -1;
        // Non Max Suppression Code
        while (bounding.Count != 0)
        {
            OpenCvSharp.Rect temp = bounding[0];
            keep.Add(bounding[0]);
            length++;
            bounding.RemoveAt(0);
            float area1 = temp.Height * temp.Width;
            List<int> indicesToRemove = new List<int>();
            for(int i = 0; i < bounding.Count; i++)
            {
                float area2 = bounding[i].Height * bounding[i].Width;
                float xx = Mathf.Max(bounding[i].BottomLeft.X, temp.BottomLeft.X);
                float yy = Mathf.Max(bounding[i].BottomLeft.Y, temp.BottomLeft.Y);
                float aa = Mathf.Max(bounding[i].TopRight.X, temp.TopRight.X);
                float bb = Mathf.Max(bounding[i].TopRight.Y, temp.TopRight.Y);

                float w = Mathf.Max(0, (aa - xx));
                float h = Mathf.Max(0, (bb - yy));
                float intersection_area = w * h;
                float union_area = area1 + area2 - intersection_area;
                float IoU = intersection_area / union_area;
                if (IoU > 0.5)
                {
                    indicesToRemove.Add(i);
                }

            }

            for(int i = 0; i < indicesToRemove.Count; i++)
            {
                bounding.RemoveAt(indicesToRemove[i]);
            }


        }


        

       
        for(int j = 0; j < keep.Count; j++)
        {
            //Filtering By size of boxes
            if (keep[j].Height * keep[j].Width > 10000 && keep[j].Height * keep[j].Width <100000)
            {


                Scalar color = new Scalar(rng.Uniform(0, 256), rng.Uniform(0, 256), rng.Uniform(0, 256));
                Cv2.Rectangle(frame, keep[j].TopLeft, keep[j].BottomRight, color, 2);
                
            }
        }
        texture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = texture;
        raw.texture = OpenCvSharp.Unity.MatToTexture(src_gray);
        Cv2.WaitKey(200);
        


    }
}
