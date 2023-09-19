

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
public class MoveCamera : MonoBehaviour
{
    float moveSpeed = 2f;
    float rotationSensitivity = 0.25f;
    float rotationSensitivity2 = 0.05f;
    string now = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    Vector3 v3forward;
    Vector3 v3forward_altered;
    DateTime foo;
    string unixTime;
    Text userNameInputText;
    System.Random random = new System.Random();
  
    int t;
    Vector3 savedPosition;
    Vector3 savedRotation;
    string sName;
    double fTime;
    double curTime;
    List<string> lst;
    float vertical;
    float rotate;
    // Start is called before the first frame update
    void Start()
    {
        curTime = 1682481247.85909;
        savedPosition = new Vector3();
        savedRotation = new Vector3();
        //transform.position = new Vector3(-360.0f, -220.0f, -300.0f);
        //-364.0629 - 284.0748 - 220 - 0.03742233 0.3869938 0.01572122 0.9211885
        transform.position = new Vector3(-362.8002f, -220f, -274.1598f);
        transform.rotation = new Quaternion(0.05358163f, 0.5318121f, -0.03374261f, 0.8444918f);
        CreateText();

        System.IO.DirectoryInfo di = new DirectoryInfo("nasa/rgb/");
       
        foreach (FileInfo file in di.GetFiles())
        {
            sName = file.Name;
            //UnityEngine.Debug.Log(sName);
            sName = sName.Replace(".png", "");
            //UnityEngine.Debug.Log(sName);
            fTime = Convert.ToDouble(sName);

            //UnityEngine.Debug.Log(fTime.ToString());
            if (fTime > curTime)
            {
                file.Delete();
            }
        }
        
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }

        //System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        //curTime = (System.DateTime.UtcNow - epochStart).TotalSeconds;
        //UnityEngine.Debug.Log(curTime.ToString());
       
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        t += 1;
        
        if (Input.GetKey("escape"))
        {
            //Application.Quit();
            EditorApplication.ExitPlaymode();
        }
        savedRotation = transform.forward;
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotationSensitivity2, 0);
        transform.Rotate(-Input.GetAxis("Mouse Y") * rotationSensitivity2, 0, 0);
        rotate = 0;
        
        if (Input.GetKey("z"))
            rotate = -rotationSensitivity;
        if (Input.GetKey("x"))
            rotate = rotationSensitivity;
        transform.Rotate(0, rotate, 0);
        


        float verticalRotation = 0;
        float horizontalRotation = 0;

        if (Input.GetKey("i"))
            verticalRotation = -rotationSensitivity;  // Tilt up
        if (Input.GetKey("k"))
            verticalRotation = rotationSensitivity;  // Tilt down
        if (Input.GetKey("j"))
            horizontalRotation = -rotationSensitivity;  // Turn left
        if (Input.GetKey("l"))
            horizontalRotation = rotationSensitivity;  // Turn right

        transform.Rotate(verticalRotation, horizontalRotation, 0);

        v3forward = transform.forward;
        v3forward_altered = new Vector3(v3forward.x, 0, v3forward.z);
    
        transform.forward = v3forward_altered;
        savedPosition = transform.position;
        vertical = (Input.GetAxis("Vertical") > 0) ? Input.GetAxis("Vertical") : 0;
        transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0.0f, moveSpeed * vertical * Time.deltaTime);
        //if (Input.GetAxis("Horizontal") != 0f)
        //transform.Translate(moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0.0f, moveSpeed * 0.3f * Time.deltaTime);
        if (transform.position.x > -245  || transform.position.x < -490 || transform.position.z > -245 || transform.position.z < -355)
        {
            transform.position = savedPosition;
        }

        /*
        if (transform.eulerAngles.y > 180)
        {
            //transform.rotation = savedRotation;
            transform.forward = v3forward;
        }
        else
        {
            transform.forward = savedRotation;
        }
        */
        transform.forward = v3forward;

        unixTime = curTime.ToString();
        if (Input.GetKeyDown("b"))
        {
            ScreenCapture.CaptureScreenshot(System.IO.Directory.GetCurrentDirectory() + "/nasa/rgb/" + unixTime + ".png");
            string content = unixTime + " " + transform.position.x.ToString() + " " + transform.position.z.ToString() + " " + transform.position.y.ToString() + " " + transform.rotation.x.ToString() + " " + transform.rotation.y.ToString() + " " + transform.rotation.z.ToString() + " " + transform.rotation.w.ToString() + '\n';
            File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + "/nasa/groundtruth.txt", content);
            content = unixTime + " rgb/" + unixTime + ".png" + '\n';
            File.AppendAllText(System.IO.Directory.GetCurrentDirectory() + "/nasa/rgb.txt", content);
            //displayText.text = "";

        }

        curTime += 0.03;
        //UnityEngine.Debug.Log(curTime.ToString());
    }


    void CreateText( )
    {
        string path = System.IO.Directory.GetCurrentDirectory() + "/nasa/groundtruth.txt";
        string content = "# ground truth trajectory" + '\n' + "#file: 'rgbd_dataset_freiburg1_xyz.bag'" + '\n' + "# timestamp tx ty tz qx qy qz qw";
        //File.WriteAllText(path, content);
        lst = File.ReadAllLines(path).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
        lst.RemoveRange(0, 3);
        for (int i = 0; i < lst.Count; i++)
        {
            //UnityEngine.Debug.Log(i);
            //UnityEngine.Debug.Log(lst.Count);
            //UnityEngine.Debug.Log(lst[i].Split(' ')[0]);
            //UnityEngine.Debug.Log(Convert.ToDouble(lst[i].Split(' ')[0]) > curTime);
            if (Convert.ToDouble(lst[i].Split(' ')[0]) > curTime)
            {
                lst.RemoveRange(i, lst.Count - i);
                break;
            }
        }

        lst.Insert(0, content);
        File.WriteAllLines(path, lst);


        path = System.IO.Directory.GetCurrentDirectory() + "/nasa/rgb.txt";
        lst = File.ReadAllLines(path).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
        lst.RemoveRange(0, 3);
        for (int i = 0; i < lst.Count; i++)
        {
            if (Convert.ToDouble(lst[i].Split(' ')[0]) > curTime)
            {
                lst.RemoveRange(i, lst.Count - i);
                break;
            }
        }

        content = "# color images" + '\n' + "# file: 'rgbd_dataset_freiburg1_xyz.bag'" + '\n' + "# timestamp filename";
        lst.Insert(0, content);
        File.WriteAllLines(path, lst);

    }
}




