using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

public class FaceTest1 : MonoBehaviour
{
    public InputField inputImageURL;
    public string ImageURL = "";
    //按钮上的文本
    public Text Btn_ShibieText;
    //显示结果
    public GameObject ShowResult;
    //相机／用于截图使用
    public Camera Cam;

    Rect rect;
    //截屏开始的位置
    Vector3 s_pos;
    //截屏结束的位置
    Vector3 e_pos;
    //是否绘制
    bool isDraw;
    //绘制状态
    bool stateDraw = false;
    //开始绘制
    bool stateDrawStart = false;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stateDraw == true)
        {
            //按下鼠标左键时，记录当前鼠标的位置为开始截屏时的位置
            if (Input.GetMouseButtonDown(0))
            {
                stateDrawStart = true;
                s_pos = Input.mousePosition;
            }
            //鼠标处于按下状态时
            if (Input.GetMouseButton(0))
            {
                e_pos = Input.mousePosition;
                //可以开始绘制
                isDraw = true;
            }
            //抬起鼠标左键时，记录当前鼠标的位置为结束截屏时的位置
            if (Input.GetMouseButtonUp(0) && stateDrawStart == true)
            {
                //结束绘制
                isDraw = false;
                e_pos = Input.mousePosition;
                //获取到截屏框起始点的位置，和宽高。
                rect = new Rect(Mathf.Min(s_pos.x, e_pos.x), Mathf.Min(s_pos.y, e_pos.y), Mathf.Abs(s_pos.x - e_pos.x), Mathf.Abs(s_pos.y - e_pos.y));
                //开启绘制的协程方法
                StartCoroutine(Capsture(rect));
                stateDraw = false;
                stateDrawStart = false;
            }
        }
    }

    public void TestHttpSend()
    {
        if(inputImageURL.text != "")
        {
            ImageURL = inputImageURL.text;
        }
        //识别文字
        WWWForm form = new WWWForm();
        form.AddField("api_key", "4diXTcIKqzKR1kE7HqJxQhdSKoDaQlQS");
        form.AddField("api_secret", "CBrU3vHl8sPdkEUc2efZaBBhhdZRBaX4");
        form.AddField("image_url", ImageURL);
        StartCoroutine(SendPost("https://api-cn.faceplusplus.com/imagepp/v1/recognizetext", form));
    }
    //提交数据进行识别
    IEnumerator SendPost(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)//if (postData.error != "") 如果这里出错可以把null换成""试试
        {
            //Debug.Log(postData.error);
            Btn_ShibieText.text = "识别";
            //ShowResult.transform.Find("Text").GetComponent<Text>().text = "识别失败！";
            GameObject.Find("DebugText").GetComponent<Text>().text = postData.error;
        }
        else
        {
            Btn_ShibieText.text = "识别";
            //Debug.Log(postData.text);
            GameObject.Find("DebugText").GetComponent<Text>().text = postData.text;
            JsonJieXi(postData.text);
        }
    }
    void JsonJieXi(string str)
    {
        JsonData jd = JsonMapper.ToObject(str);
        string ResultStr = "";
        //Debug.Log(jd["result"].Count);
        for (int i = 0; i < jd["result"].Count; i++)
        {
            for (int j = 0; j < jd["result"][i]["child-objects"].Count; j++)
            {
                //Debug.Log(jd["result"][i]["child-objects"][j]["type"].ToString());
                Debug.Log(jd["result"][i]["child-objects"][j]["value"].ToString());
                ResultStr = ResultStr + jd["result"][i]["child-objects"][j]["value"].ToString();
            }
        }
        ShowResult.transform.Find("Text").GetComponent<Text>().text = ResultStr;
    }
    /// <summary>
    /// 拍照按钮
    /// </summary>
    public void Btn_JieTu()
    {
        Btn_ShibieText.text = "请框选截图";
        stateDraw = true;
        //CaptureCamera(Cam, new Rect(0, 0, Screen.width * 0.5f, Screen.height * 0.5f));
    }
    ////截图并且识别
    //void CaptureCamera(Camera camera, Rect rect)
    //{
    //    // 创建一个RenderTexture对象  
    //    RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
    //    // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
    //    camera.targetTexture = rt;
    //    camera.Render();

    //    RenderTexture.active = rt;
    //    Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
    //    screenShot.ReadPixels(rect, 0, 0);
    //    screenShot.Apply();

    //    // 重置相关参数，以使用camera继续在屏幕上显示  
    //    camera.targetTexture = null;
    //    RenderTexture.active = null;
    //    GameObject.Destroy(rt);
    //    //		// 最后将这些纹理数据，成一个png图片文件  
    //    byte[] bytes = screenShot.EncodeToPNG();
    //    string filename = Application.dataPath + "/StreamingAssets/Screenshot.png";
    //    System.IO.File.WriteAllBytes(filename, bytes);
    //    Debug.Log(string.Format("截屏了一张照片: {0}", filename));

    //    Debug.Log("文字");
    //    //识别文字
    //    WWWForm form = new WWWForm();
    //    form.AddField("api_key", "4diXTcIKqzKR1kE7HqJxQhdSKoDaQlQS");
    //    form.AddField("api_secret", "CBrU3vHl8sPdkEUc2efZaBBhhdZRBaX4");
    //    form.AddBinaryData("image_file", screenShot.EncodeToPNG());
    //    StartCoroutine(SendPost("https://api-cn.faceplusplus.com/imagepp/beta/recognizetext", form));
    //}

    IEnumerator Capsture(Rect rect)
    {
        yield return new WaitForEndOfFrame();

        //创建纹理（纹理贴图的大小和截屏的大小相同）
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height,TextureFormat.RGB24, false);
        //读取像素点
        tex.ReadPixels(rect, 0, 0);
        //将像素点应用到纹理上，绘制图片
        tex.Apply();
        //将图片装换成jpg的二进制格式，保存在byte数组中（计算机是以二进制的方式存储数据）
        byte[] result = tex.EncodeToPNG();
        //文件夹（如果StreamAssets文件夹不存在，在Assets文件下创建该文件夹）
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);
        //将截屏图片存储到本地
        string filename = Application.dataPath + "/StreamingAssets/Screenshot.png";
        File.WriteAllBytes(filename, result);

        //Debug.Log("文字");
        //识别文字
        WWWForm form = new WWWForm();
        form.AddField("api_key", "4diXTcIKqzKR1kE7HqJxQhdSKoDaQlQS");
        form.AddField("api_secret", "CBrU3vHl8sPdkEUc2efZaBBhhdZRBaX4");
        form.AddBinaryData("image_file", tex.EncodeToPNG());
        StartCoroutine(SendPost("https://api-cn.faceplusplus.com/imagepp/v1/recognizetext", form));
    }
    //在这里要用GL实现绘制截屏的矩形框
    //1.GL的回调函数
    //2.定义一个材质Material
    public Material lineMaterial;

    void OnPostRender()
    {
        if (!isDraw) return;
        //print(s_pos);

        Vector3 sPos = Camera.main.ScreenToWorldPoint(s_pos + new Vector3(0, 0, 10));
        Vector3 ePos = Camera.main.ScreenToWorldPoint(e_pos + new Vector3(0, 0, 10));

        //print(string.Format("GL.....{0},  {1}", sPos, ePos));
        // Set your materials Done
        GL.PushMatrix();
        // yourMaterial.SetPass( );
        lineMaterial.SetPass(0);//告诉GL使用该材质绘制
                                // Draw your stuff
                                //始终在最前面绘制
        GL.invertCulling = true;
        GL.Begin(GL.LINES);//开始绘制

        //GL.Vertex(sPos);
        //GL.Vertex(ePos);
        //如果想要绘制，矩形，将下面代码启动
        GL.Vertex(sPos);
        GL.Vertex(new Vector3(ePos.x, sPos.y, 0));


        GL.Vertex(new Vector3(ePos.x, sPos.y, 0));
        GL.Vertex(ePos);

        GL.Vertex(ePos);
        GL.Vertex(new Vector3(sPos.x, ePos.y, 0));

        GL.Vertex(new Vector3(sPos.x, ePos.y, 0));
        GL.Vertex(sPos);
        GL.End();//结束绘制

        GL.PopMatrix();
    }
}
