using UnityEngine;
using System.Collections;
using System.IO;//引入IO流
public class NewBehaviourScript : MonoBehaviour
{
    //定义一个存储截屏图片的路径
    string filePath;
    void Start()
    {
        //图片存储在StreamingAssets文件夹内。
        filePath = Application.streamingAssetsPath + "/Screenshot.png";
    }
    Rect rect;
    //截屏开始的位置
    Vector3 s_pos;
    //截屏结束的位置
    Vector3 e_pos;
    //是否绘制
    bool isDraw;
    void Update()
    {
        //按下鼠标左键时，记录当前鼠标的位置为开始截屏时的位置
        if (Input.GetMouseButtonDown(0))
        {
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
        if (Input.GetMouseButtonUp(0))
        {
            //结束绘制
            isDraw = false;
            e_pos = Input.mousePosition;
            //获取到截屏框起始点的位置，和宽高。
            rect = new Rect(Mathf.Min(s_pos.x, e_pos.x), Mathf.Min(s_pos.y, e_pos.y), Mathf.Abs(s_pos.x - e_pos.x), Mathf.Abs(s_pos.y - e_pos.y));
            //开启绘制的协程方法
            StartCoroutine(Capsture(filePath, rect));
        }
    }


    IEnumerator Capsture(string filePath, Rect rect)
    {
        yield return new WaitForEndOfFrame();
        //创建纹理（纹理贴图的大小和截屏的大小相同）
        Texture2D tex = new Texture2D((int)rect.width, (int)rect.height);
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
        File.WriteAllBytes(filePath, result);
    }
    //在这里要用GL实现绘制截屏的矩形框
    //1.GL的回调函数
    //2.定义一个材质Material
    public Material lineMaterial;

    void OnPostRender()
    {
        if (!isDraw) return;
        print(s_pos);

        Vector3 sPos = Camera.main.ScreenToWorldPoint(s_pos + new Vector3(0, 0, 10));
        Vector3 ePos = Camera.main.ScreenToWorldPoint(e_pos + new Vector3(0, 0, 10));

        print(string.Format("GL.....{0},  {1}", sPos, ePos));
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