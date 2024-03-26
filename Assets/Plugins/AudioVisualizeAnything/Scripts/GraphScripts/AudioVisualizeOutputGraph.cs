using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVisualizeOutputGraph : MonoBehaviour
{
    private Text MaxYText;
    private Text MinYText;

    public enum Graph
    {
        Enabled,
        Disabeled
    }
    [SerializeField] private Graph graph;

    private bool TogglePlot = true;
    public enum ScalingMode  //缩放模式
    {
        AutoScale,
        Manual
    }
    [SerializeField] private ScalingMode scalingMode;

    [SerializeField] private float GraphYMaximum = .2f; //显示图Y值最大
    private float LastTickYMaximum;

    [SerializeField] private float GraphYMinimum = 0f; //显示图Y值最小
    private float LastTickYMinimum;

    [SerializeField] private float GraphXIncrement = 2f;//显示图x值增量

    [Range(1, 10)]
    [SerializeField] private int PlotsPerTick = 1;  //每刻度绘图

    [SerializeField] private GameObject graphReference;   
    [SerializeField] private RectTransform graphContainerReference; //绘画容器
    [SerializeField] private Sprite marksSprite; //点图片
    [SerializeField] private Image backGround;
    
    [SerializeField] private GameObject linkPre;

    List<float> OutputData = new List<float>();
    List<GameObject> pointsOnGraphList = new List<GameObject>();

    private Vector2 _lastPointPos = Vector2.zero;

    private void Awake()
    {
        LastTickYMaximum = GraphYMaximum;
        LastTickYMinimum = GraphYMinimum;

        switch (scalingMode)
        {
            case ScalingMode.AutoScale:
                GraphYMaximum = 1;
                break;
            case ScalingMode.Manual:
                GraphYMaximum = 1;
                break;
            default:
                break;
        }

        //graphContainerReference = FindObjectOfType<graphContainerScript>().GetComponent<RectTransform>();

        if (graphReference != null)
        {
            MaxYText = graphReference.GetComponentInChildren<MaxYTextScript>(true).GetComponent<Text>();
            MinYText = graphReference.GetComponentInChildren<MinYTextScript>(true).GetComponent<Text>();
        }        

        //MaxYText = FindObjectOfType<MaxYTextScript>().GetComponent<Text>();  // not sure why TypeAll is being depricated
        //MinYText = FindObjectOfType<MinYTextScript>().GetComponent<Text>();

        switch (graph)
        {
            case Graph.Enabled:
                graphReference.SetActive(true);
                break;
            case Graph.Disabeled:
                graphReference.SetActive(false);
                break;
            default:
                break;
        }
    }

    // public void OutputListener()
    // {
    //     if (TogglePlot)
    //     {
    //         PopulateGraphList(AudioVisualizeManager.Output_Volume);
    //     }
    // }
    
    /// <summary>
    /// 填充图列表
    /// </summary>
    /// <param name="Output"></param>
    public void PopulateGraphList(float Output)
    {
        //CheckXBounds();

        CalculateMaxY(Output);
        OutputData.Add(Output);

        if (PlotsPerTick <= 0)
        {
            PlotsPerTick = 1;
        }

        
        if (LastTickYMaximum == GraphYMaximum && PlotsPerTick ==1) //This is much more effeciant, Consider getting rid of PlotsPerTick
        {                                                          //PlotsPerTick was initially intorduce to reduce processing, because we redrew the entire graph everytime. this is no longer the case, but only with PlotsPerTick ==1
            InjectGraphDataPoints(Output);
        }
        else
        {           
            if (OutputData.Count > 0 && OutputData.Count % PlotsPerTick == 0)  //Only do this id Y max Changes, otherwise Plot one at a time
            {
                foreach (var point in pointsOnGraphList)
                {
                    Destroy(point.gameObject);                                     //Should use object pooling for this
                }
                pointsOnGraphList.Clear();

                InjectGraphDataList(OutputData);

                LastTickYMaximum = GraphYMaximum;
            }
        } 
    }

    
    public void ShowMusicMapByDataList(List<float> dataList)
    {
        // 创建纹理并绘制音频波形
        Texture2D waveformTexture = new Texture2D(dataList.Count, 1);
        Color[] waveformColors = new Color[dataList.Count];
        for (int i = 0; i < dataList.Count; i++)
        {
            waveformColors[i] = new Color(Mathf.Abs(dataList[i]), Mathf.Abs(dataList[i]), Mathf.Abs(dataList[i]));
        }
        waveformTexture.SetPixels(waveformColors);
        waveformTexture.Apply();

        // 将纹理显示到UI Image上
        backGround.sprite = Sprite.Create(waveformTexture, new Rect(0f, 0f, waveformTexture.width, waveformTexture.height), Vector2.one * 0.5f);
        
        // foreach (var point in pointsOnGraphList)
        // {
        //     Destroy(point.gameObject);                                     //Should use object pooling for this
        // }
        // pointsOnGraphList.Clear();
        //
        // InjectGraphDataList(dataList);
        //
        // LastTickYMaximum = GraphYMaximum;
    }
    

    /// <summary>
    /// 注入图形数据列表
    /// 我们循环浏览这里的列表并绘制其中的所有内容  //we cycle through the list here and plot everyhing in it
    /// </summary>
    /// <param name="DataList"></param>
    public void InjectGraphDataList(List<float> DataList)
    {
        float graphHeight = graphContainerReference.sizeDelta.y; //750

        for (int i = 0; i < DataList.Count; i++)
        {
            float xPosition = i * GraphXIncrement;
            float yPosition = (DataList[i] / GraphYMaximum ) * graphHeight / 2f; //除以2，因为我们希望图表的中间为0 Devided by 2 because we want the middile of the graph to be 0
            
            yPosition += graphHeight / 2f;                                      //移动 Y 轴  Shifting the Y Axes
            CreateCircle(new Vector2(xPosition, yPosition));
        }
    }

    /// <summary>
    /// 注入图形数据点
    /// 分别绘制每个点  Plot each individually
    /// </summary>
    /// <param name="Data"></param>
    public void InjectGraphDataPoints(float Data) 
    {
        float graphHeight = graphContainerReference.sizeDelta.y; //750

        float xPosition = OutputData.Count * GraphXIncrement;
        float yPosition = (Data / GraphYMaximum ) * graphHeight / 2f;  //Devided by 2 because we want the middile of the graph to be 0
        
        yPosition += graphHeight / 2f;   //this is middile of graph //Shifting the Y Axes        
        CreateCircle(new Vector2(xPosition, yPosition));
    }


    /// <summary>
    /// 创建圆
    /// </summary>
    /// <param name="anchoredPosition"></param>
    /// 
    private void CreateCircle(Vector2 anchoredPosition)
    {
        //覆盖该函数内游戏对象的含义，它将不再引用该脚本所在的游戏对象 //Overriding what gameObject means inside this function, it will no longer refere to the gameObject that this script it placed on
        GameObject link = Instantiate(linkPre,graphContainerReference); //new GameObject("circle", typeof(Image));  
        //天哪，这不是指该脚本所在的游戏对象，而是指我刚刚创建的游戏对象！  omg, this does not refere to the gameobject this script is place on, it referes to the one I just created!!
        //gameObject.transform.SetParent(graphContainerReference, false);
        //需要添加一个圆形图片   Need to add a Circle sprite
        link.SetActive(true);
        link.GetComponent<Image>().sprite = marksSprite;                       
        pointsOnGraphList.Add(link);
        RectTransform rectTransform = link.GetComponent<RectTransform>();
        // if (_lastPointPos == Vector2.zero)
        // {
            _lastPointPos = anchoredPosition;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(5, 5);
        //}
        // else
        // {
        //     rectTransform.anchoredPosition = _lastPointPos;
        //     float dis = Vector3.Distance(_lastPointPos, anchoredPosition);
        //     Debug.Log($"--->>> last = ({_lastPointPos.x},{_lastPointPos.y}) , now = ({anchoredPosition.x},{anchoredPosition.y}) , dis = {dis}");
        //     rectTransform.sizeDelta = new Vector2(1, dis);
        //     double angle = Math.Atan2(anchoredPosition.y - _lastPointPos.y, anchoredPosition.x - _lastPointPos.x) * 180 / Math.PI;
        //     rectTransform.transform.rotation = Quaternion.Euler(0, 0, (float)angle + 270);
        //     
        //     _lastPointPos = anchoredPosition;
        // }

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }



    /// <summary>
    /// 计算最大Y值
    /// </summary>
    /// <param name="Output"></param>
    public void CalculateMaxY(float Output)
    {
        if (GraphYMaximum < Mathf.Abs(Output))   
        {
            switch (scalingMode)
            {
                case ScalingMode.AutoScale:
                    GraphYMaximum = Mathf.Abs(Output);
                    break;
                case ScalingMode.Manual:
                    break;
                default:
                    break;
            }
        }
        MaxYText.text = GraphYMaximum.ToString();
        MinYText.text = (GraphYMaximum * (-1f)).ToString();
        
    }
    
    /// <summary>
    /// 检查X轴界限
    /// </summary>
    private void CheckXBounds()
    {
        if (graphContainerReference.sizeDelta.x - (OutputData.Count * GraphXIncrement) <= 0f)
        {
            OutputData.Clear();
            foreach (var point in pointsOnGraphList)
            {
                //用对象池回收
                Destroy(point.gameObject);             
            }
            pointsOnGraphList.Clear();
        }
    }



    //Editor
    /// <summary>
    /// 是否显示图谱
    /// </summary>
    public void EnableDisableGraph() 
    {
        switch (graph)
        {
            case Graph.Enabled:
                graphReference.SetActive(true);
                break;
            case Graph.Disabeled:
                graphReference.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 按钮设置 //make this a button
    /// </summary>
    public void TogglePlotButton() 
    {
        if (TogglePlot)
        {
            TogglePlot = false;
        }
        else
        {
            TogglePlot = true;
        }
    }

    /// <summary>
    /// 自动缩放切换
    /// </summary>
    public void AutoScaleToggle()
    {       
        switch (scalingMode)
        {
            case ScalingMode.AutoScale:                
                float Ymax = 0;
                foreach (var Output in OutputData)
                {                    
                    if (Ymax < Mathf.Abs(Output))
                    {
                        Ymax = Mathf.Abs(Output);
                    }
                }
                GraphYMaximum = Ymax;
                break;
            case ScalingMode.Manual:
                break;
            default:
                break;
        }
    }


}
