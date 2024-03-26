using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioVisualizeManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private AudioSource audioSource;

    private float[] samplesArray;
    private float[] fullArray;

    //[Header("Audio Stats")]
    [SerializeField] private int tottalAudioSampleLength;
    [SerializeField] private float maximumSampleValue = 0;
    [SerializeField] private float minimumSampleValue = 0;

    public static float Output_Volume = 0; //API or reference script and use Output

    [SerializeField] public float Output; //API
    [SerializeField] private float maxOutputRecorded;
    [SerializeField] private float minOutputRecorded;

    private float volume = 0;
    private float maxBlockAverageOutput = 0;
    private bool enumSwitched = true; // did I forget to use this? lol

    [Header("Beat Setings")] [SerializeField]
    private bool beat;

    public enum InvokeMode
    {
        PlayOnStart,
        Invoke
    }

    [SerializeField] private InvokeMode invokeMode;

    public enum BlockSize
    {
        Small,
        Medium,
        Large,
        XLarge
    }

    [SerializeField] private BlockSize blockSize;


    public enum Algorithm // 算法
    {
        ActualVolume, //实际音量
        AbsoluteVolume, //绝对音量
        ActualPlusOne, //实际加一
        ActualMinZero, // 实际最小0  将最小值添加到每个读数中
        StandardizedVolume, //标准化 min -1 or Max +1  
        StandardizedBlockVolume,//标准块音量
        Squared, //平方 在相加之前对每个值求平方，其作用有点像 RSI  Square every Value before adding it, acts a littile bit like an RSI
        SquaredWithDirection//向量平方
    }

    [SerializeField] private Algorithm algorithm;

    [SerializeField] private bool multiplyByMinusOne; //是否 乘-1


    [Range(0.01f, 1f)] [SerializeField] private float refreshTime = .1f;  //刷新时间
    [Range(0f, 1f)] [SerializeField] private float PushMultiplierPartOne = 0f; 

    [Range(0f, 100f)] [SerializeField] private float PushMultiplierPartTwo = 1f;

    [SerializeField] private float tottalMultiplier; // Addition of the two above //Add a editor script to this


    [SerializeField] private bool ClampOutput; //是否限制最大最小输出
    [Range(-3f, 1f)] [SerializeField] private float minOutput = -1f; 
    [Range(-1f, 3f)] [SerializeField] private float maxOutput = 1f;


    [Header("Subscribe To Events")] [SerializeField]
    private UnityEvent OnVolumeChange;  //音量改变事件

    [SerializeField] private UnityEvent OnClipEnd; //结束事件

    #endregion

    private void Start()
    {
        ClipPrep();
        switch (invokeMode)
        {
            case InvokeMode.PlayOnStart:
                beat = true;
                StartCoroutine(StartBeat());
                break;
            case InvokeMode.Invoke:
                beat = false;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 剪辑准备
    /// </summary>
    private void ClipPrep()
    {
        if (audioSource.clip != null)
        {
            CalculateTotalMultiplier();
            CalclateAudioStats();


            tottalAudioSampleLength = audioSource.clip.samples;
        }

        switch (blockSize)
        {
            case BlockSize.Small:
                samplesArray = new float[256];
                break;
            case BlockSize.Medium:
                samplesArray = new float[512];
                break;
            case BlockSize.Large:
                samplesArray = new float[1024];
                break;
            case BlockSize.XLarge:
                samplesArray = new float[2048];
                break;
            default:
                break;
        }

        AverageBlockVolumeCalc();
    }

    /// <summary>
    /// 开始节拍
    /// </summary>
    /// <returns></returns>
    IEnumerator StartBeat()
    {
        if (audioSource.clip == null)
        {
            beat = false;
            yield break;
        }
        
        while (beat)
        {
            if (audioSource.isPlaying)
            {
                tottalAudioSampleLength = audioSource.clip.samples;
        
                if ((audioSource.timeSamples - tottalAudioSampleLength) != 0)
                {
                    audioSource.clip.GetData(samplesArray, audioSource.timeSamples);
                    volume = 0;
                    AlgorithmSwitcher();
                    VolumeHasChange();
                }
                //else
                //{
                //   // Debug.Log("This would have been an erro due to value = " + (audioSource.timeSamples - tottalAudioSampleLength));
                //Looping the Audio Sources prevents the error too, need to investigate this
                //}                               
            }
            else
            {
                ClipHasStopped();
                beat = false;
                yield break;
            }
        
            if (audioSource.clip == null)
            {
                beat = false;
                yield break;
            }
        
            yield return new WaitForSeconds(refreshTime);
            //Debug.Log(audioSource.isPlaying);
        }

        yield break;
    }


    #region Algorithms and Preferences
    
    /// <summary>
    /// 算法切换选择
    /// </summary>
    private void AlgorithmSwitcher()
    {
        switch (algorithm)
        {
            case Algorithm.ActualVolume:
                AlgorithmActualVolume();
                break;
            case Algorithm.AbsoluteVolume:
                AlgorithmAbsoluteVolume();
                break;
            case Algorithm.ActualPlusOne:
                AlgorithmActualPlusOne();
                break;
            case Algorithm.StandardizedVolume:
                AlgorithmStandardizedVolume();
                break;
            case Algorithm.ActualMinZero:
                AlgorithmMinZero();
                break;
            case Algorithm.Squared:
                AlgorithmSquared();
                break;
            case Algorithm.SquaredWithDirection:
                AlgorithmSquaredWithDirection();
                break;
            case Algorithm.StandardizedBlockVolume:
                AlgorithmStandardizedBlockVolume();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 算计实际音量算法
    /// </summary>
    private void AlgorithmActualVolume()
    {
        foreach (var sample in samplesArray)
        {
            volume += sample;
        }

        Output_Volume = volume / samplesArray.Length;
        AdditionalMaths();
    }

    /// <summary>
    /// 绝对音量算法
    /// </summary>
    private void AlgorithmAbsoluteVolume()
    {
        foreach (var sample in samplesArray)
        {
            volume += sample;
        }

        Output_Volume = Mathf.Abs(volume / samplesArray.Length);
        AdditionalMaths();
    }

    /// <summary>
    /// 实际加一算法
    /// </summary>
    private void AlgorithmActualPlusOne()
    {
        foreach (var sample in samplesArray)
        {
            volume += (sample + 1f);
        }

        Output_Volume = (volume / samplesArray.Length);
        AdditionalMaths();
    }

    /// <summary>
    /// 标准化体积算法
    /// </summary>
    private void AlgorithmStandardizedVolume()
    {
        var MaxValue = (Mathf.Max(maximumSampleValue, Mathf.Abs(minimumSampleValue)));

        foreach (var sample in samplesArray)
        {
            if (sample != 0)
            {
                volume += sample / MaxValue; // not sure if I need he Minoutputrecorded
            }
        }

        Output_Volume = volume / samplesArray.Length;
        AdditionalMaths();
    }

    /// <summary>
    /// 最小0算法
    /// </summary>
    private void AlgorithmMinZero()
    {
        foreach (var sample in samplesArray)
        {
            volume += sample + (-1f * minimumSampleValue);
        }

        Output_Volume = volume / samplesArray.Length;
        AdditionalMaths();
    }

    /// <summary>
    /// 平方算法
    /// </summary>
    private void AlgorithmSquared()
    {
        foreach (var sample in samplesArray)
        {
            volume += sample * sample;
        }

        Output_Volume = volume / samplesArray.Length;
        AdditionalMaths();
    }

    /// <summary>
    /// 方向平方算法
    /// </summary>
    private void AlgorithmSquaredWithDirection()
    {
        foreach (var sample in samplesArray)
        {
            if (sample >= 0)
            {
                volume += sample * sample;
            }
            else
            {
                volume += sample * sample * -1;
            }
        }

        Output_Volume = volume / samplesArray.Length;
        AdditionalMaths();
    }

    /// <summary>
    /// 算法标准化区块体积
    /// </summary>
    private void AlgorithmStandardizedBlockVolume()
    {
        var MaxValue = maxBlockAverageOutput;

        foreach (var sample in samplesArray)
        {
            if (sample != 0)
            {
                volume += sample;
            }
        }

        Output_Volume = (volume / MaxValue) / samplesArray.Length;
        AdditionalMaths();
    }

    /// <summary>
    /// 附加数学计算
    /// </summary>
    private void AdditionalMaths()
    {
        Output_Volume *= (PushMultiplierPartOne + PushMultiplierPartTwo); //当前

        if (multiplyByMinusOne)
        {
            Output_Volume *= -1f;
        }

        if (ClampOutput)
        {
            Output_Volume = Mathf.Clamp(Output_Volume, minOutput, maxOutput); //限制最大小
        }

        if (maxOutputRecorded < Output_Volume) //最大输出记录
        {
            maxOutputRecorded = Output_Volume;
        }
        else if (minOutputRecorded > Output_Volume) //最小输出记录
        {
            minOutputRecorded = Output_Volume;
        }


        Output = Output_Volume; //for visualAid //I should make this static instead of averageBlockVolume
    }

    #endregion


    #region Unity Events

    //events functions

    private void VolumeHasChange()
    {
        OnVolumeChange.Invoke();
    }

    private void ClipHasStopped()
    {
        OnClipEnd.Invoke();
    }

    #endregion


    #region API Functions

    public void BeatToggle() //API
    {
        ClipPrep();
        if (beat)
        {
            beat = false;
        }
        else
        {
            beat = true;
            StartCoroutine(StartBeat());
        }
    }

    public void BeatToggleOn() //API
    {
        if (!beat)
        {
            ClipPrep();
            beat = true;
            StartCoroutine(StartBeat());
        }
    }

    public void BeatToggleOff() //API
    {
        ClipPrep();
        beat = false;
    }

    #endregion


    //Extra functions

    [ContextMenu("CalclateAudioStats")] //计算音频统计
    public void CalclateAudioStats()
    {
        fullArray = new float[audioSource.clip.samples];
        audioSource.clip.GetData(fullArray, audioSource.timeSamples);
        Debug.Log($"audioSource.timeSamples = {audioSource.timeSamples} fullArray.Count = {fullArray.Length}");
        tottalAudioSampleLength = fullArray.Length;
        maximumSampleValue = 0;
        minimumSampleValue = 0;

        int num = 0;
        foreach (var sample in fullArray)
        {
            if (sample > 0.5f || sample < -0.5f) num++;
            if (sample > maximumSampleValue)
            {
                maximumSampleValue = sample;
            }
            else if (sample < minimumSampleValue)
            {
                minimumSampleValue = sample;    
            }
        }
        Debug.Log($"num = {num}");
    }
    
    public void CalculateTotalMultiplier() //计算总数
    {
        tottalMultiplier = PushMultiplierPartOne + PushMultiplierPartTwo;
    }

    public void AverageBlockVolumeCalc() //平均音量计算
    {
        float[] samplesBlockArray = new float[256];

        switch (blockSize)
        {
            case BlockSize.Small:
                samplesBlockArray = new float[256];
                break;
            case BlockSize.Medium:
                samplesBlockArray = new float[512];
                break;
            case BlockSize.Large:
                samplesBlockArray = new float[1024];
                break;
            case BlockSize.XLarge:
                samplesBlockArray = new float[2048];
                break;
            default:
                break;
        }

        int blockSizeInt = samplesBlockArray.Length;
        float blockSizeFloat = (float)blockSizeInt;
        float NumberOfBlocksToRun = (float)tottalAudioSampleLength / blockSizeFloat;
        NumberOfBlocksToRun = Mathf.Ceil(NumberOfBlocksToRun) - 1;

        float sampleSum = 0;

        maxBlockAverageOutput = 0;
        for (int i = 0; i < NumberOfBlocksToRun; i++)
        {
            int runMovingAverageFrom = i * blockSizeInt;

            audioSource.clip.GetData(samplesBlockArray, runMovingAverageFrom);
            foreach (var sample in samplesBlockArray)
            {
                sampleSum += Mathf.Abs(sample);
            }

            sampleSum /= samplesBlockArray.Length;

            if (maxBlockAverageOutput < sampleSum)
            {
                maxBlockAverageOutput = sampleSum;
            }

            sampleSum = 0;
        }
    }


    //编辑器方法

    public void RunEditorFunctions() //编辑器方法 调用统计总数
    {
        CalculateTotalMultiplier();
    } //Updates on mouse over, do not put anything heavy on processing here
}