using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;

public class Simulation : MonoBehaviour
{
    
    public UnityEngine.Video.VideoPlayer vp;
    Mat src_gray, frame;
    Point[][] contours;
    HierarchyIndex[] heirarchy;
    Texture texture;
    List<GameObject> reconstructed = new List<GameObject>();
    public GameObject particle;
    public GameObject renderingplane;
    int t = 0;
   
    


    private void Start()
    {
        // preprocess

        //vp.frame = 150;
        // vp.Pause();
        // Texture2D inputTexture = (Texture2D)Resources.Load("Screenshot 2021-11-16 181658");
        // frame = OpenCvSharp.Unity.TextureToMat(inputTexture);


        //reconstruct();
        // reconstruct1();
        // for every particle on list
        //destroy
        /* foreach (GameObject i in reconstructed)
         {
             Destroy(i);
         }


         frame.Release();*/
      
    }

    // Start is called before the first frame update


    private void Update()
    {
        // code for object detection
        
            
        
        
         
        if (t==4)
        {
            foreach (GameObject i in reconstructed)
            {
                Destroy(i);
            }
            t = 0;
            detectobj();
            Debug.Log("destroyed");
            
        }
        t++;
        
        
    }


    void reconstruct( OpenCvSharp.Rect f )
    {
        int r = frame.Rows;
        int c = frame.Cols;
        float dep = 0;
          for(int x = f.TopLeft.X; x < f.BottomRight.X; x++)
          {
              for (int y = f.TopLeft.Y; y < f.BottomRight.Y; y++)
              {
                  // for each pixel

                  //Find corresponding x , y coordinates --- map
                  
                  // depth calculator
                  dep += frame.At<Vec3b>(x, y)[0];
                      // add to list



              }
          }
          dep = dep / (r * c);
          dep = Mathf.Lerp(5, 0, dep/ (255));
        float xrel = Mathf.Lerp(-0.5f, 0.5f, ((float) f.Center.X) / r);
        float yrel = Mathf.Lerp(0, 2, ((float)f.Center.Y) / c);
        //spawn the particle there
        var p1 = Instantiate(particle, new Vector3(xrel, yrel, dep), Quaternion.identity);
        p1.transform.localScale = new Vector3(Mathf.Lerp(-2, 2, ((float)f.Width) / r ),Mathf.Lerp(0f, 2f, ((float)f.Height) / c), 0.1f);
  reconstructed.Add(p1);


          
        




    }

    void reconstruct1()
    {
        int r = frame.Rows;
        int c = frame.Cols;
        float dep = 0;
        for (int x = 0; x < r; x++)
        {
            for (int y = 0; y < c; y++)
            {
                // for each pixel

                //Find corresponding x , y coordinates --- map
                int xrel =  x - (r / 2);
                int yrel = y - (r / 2);
                // depth calculator
                dep = frame.At<Vec3b>(x, y)[0];
                // add to list
                dep = Mathf.Lerp(0, 5, dep / (255));

                //spawn the particle there
               
                reconstructed.Add(Instantiate(particle, new Vector3(xrel, yrel, dep), Quaternion.identity));



            }
        }
       
        dep = Mathf.Lerp(0, 5, dep / (255));

        //spawn the particle there
        reconstructed.Add(Instantiate(particle, new Vector3(0, 0, dep), Quaternion.identity));








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
        RNG rng = new RNG(12345);

        Cv2.Threshold(frame, frame, 50, 255, ThresholdTypes.Binary);

        Cv2.CvtColor(frame, src_gray, OpenCvSharp.ColorConversionCodes.RGB2GRAY);

        Size size = src_gray.Size();
        Cv2.PyrDown(src_gray, src_gray);
        Size size2 = src_gray.Size();

        Cv2.Canny(src_gray, src_gray, 30, 200);
        Cv2.WaitKey(0);
        Cv2.FindContours(src_gray, out contours, out heirarchy, RetrievalModes.External, ContourApproximationModes.ApproxTC89KCOS);

        Point[] contour_poly = new Point[contours.Length];

        List<OpenCvSharp.Rect> bounding = new List<OpenCvSharp.Rect>(contours.Length);

        foreach (Point[] contour in contours)
        {
            contour_poly = Cv2.ApproxPolyDP(contour, 3, false);
            for (int i = 0; i < contour_poly.Length; i++)
            {
                contour_poly[i].X *= size.Width / size2.Width;
                contour_poly[i].Y *= size.Height / size2.Height;

            }
            bounding.Add(Cv2.BoundingRect(contour_poly));



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
            for (int i = 0; i < bounding.Count; i++)
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
                if (IoU > 0.10)
                {
                    indicesToRemove.Add(i);
                }

            }

            for (int i = 0; i < indicesToRemove.Count; i++)
            {
                bounding.RemoveAt(indicesToRemove[i]);
            }


        }





        for (int j = 0; j < keep.Count; j++)
        {
            //Filtering By size of boxes
            if (keep[j].Height * keep[j].Width > 10000 && keep[j].Height * keep[j].Width < 100000)
            {


                // Scalar color = new Scalar(rng.Uniform(0, 256), rng.Uniform(0, 256), rng.Uniform(0, 256));
                // Cv2.Rectangle(frame, keep[j].TopLeft, keep[j].BottomRight, color, 2);
                reconstruct(keep[j]);
                
                
            }
            if (keep[j].Height * keep[j].Width > 10000 && keep[j].Height * keep[j].Width < 100000)
            {


                Scalar color = new Scalar(rng.Uniform(0, 256), rng.Uniform(0, 256), rng.Uniform(0, 256));
                Cv2.Rectangle(frame, keep[j].TopLeft, keep[j].BottomRight, color, 2);

            }
        }
        texture = OpenCvSharp.Unity.MatToTexture(frame);
        renderingplane.GetComponent<Renderer>().material.mainTexture = texture;
        Cv2.WaitKey(200);



    }
}
