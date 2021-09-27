using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;


public class OpenCVTrial : MonoBehaviour
{
    WebCamTexture webCamTexture;
    Mat src_gray , frame;
    Point[][] contours ;
    HierarchyIndex[] heirarchy;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        webCamTexture = new WebCamTexture(devices[0].name);
        webCamTexture.Play();
        src_gray = new Mat();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<Renderer>().material.mainTexture = webCamTexture;
         frame = OpenCvSharp.Unity.TextureToMat(webCamTexture);
        detectobj(frame);
    }
    void detectobj(Mat frame)
    {

        RNG rng= new RNG(12345);

        Cv2.CvtColor(frame , src_gray , OpenCvSharp.ColorConversionCodes.BGR2GRAY);
        Cv2.Canny(src_gray,  src_gray, 30, 200);
        Cv2.WaitKey(0);
        Cv2.FindContours(src_gray, out  contours, out heirarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);

        Point[] contour_poly = new Point[contours.Length]  ;
        List<Point2f> centres = new List <Point2f> (contours.Length);
        List<float> radius = new List<float>(contours.Length);
        List<OpenCvSharp.Rect> bounding = new List<OpenCvSharp.Rect> ( contours.Length);
       
        foreach(Point[] contour in contours)
        {
           contour_poly= Cv2.ApproxPolyDP(contour,  3, true);
            bounding.Add( Cv2.BoundingRect(contour_poly));

            Point2f centre;
            float r;
            Cv2.MinEnclosingCircle(contour_poly, out centre,out r);
            centres.Add( centre);
            radius.Add(r);
            
        }

       // Mat drawing = Mat.Zeros(frame.Size(), frame.Type());
        for(int j = 0; j < contours.Length; j++)
        {
            Scalar color = new Scalar(rng.Uniform(0, 256), rng.Uniform(0, 256), rng.Uniform(0, 256));
            Cv2.Rectangle(frame, bounding[j].TopLeft, bounding[j].BottomRight, color, 2);
            //Cv2.Circle(drawing, centres[j], Mathf.RoundToInt(radius[j]), color, 2);

        }
        Texture newtex = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = newtex;
        Cv2.WaitKey(200);

    }
}
