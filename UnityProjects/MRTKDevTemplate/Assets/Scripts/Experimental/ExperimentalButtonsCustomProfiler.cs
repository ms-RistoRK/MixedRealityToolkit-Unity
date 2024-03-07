using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Text;
using UnityEngine.Timeline;

//public class ExperimentalButtonsCustomProfiler : MonoBehaviour
public class ExperimentalButtonsCustomProfiler : MonoBehaviour
{
    //Queue<float>
    //ProfilerRecorder renderTimeRecoder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder renderThreadTimeRecorder;
    double startTime;
    public const int framesToTime = 60;
    FrameTiming[] frameTimings = new FrameTiming[framesToTime];

    static double GetRecorderFrameAverageBySampleCount(ProfilerRecorder recorder, int samplesCount)
    {
        //Debug.Log($"Frame samples: {recorder.Capacity}");
        //var samplesCount = recorder.Capacity;
        if (samplesCount > recorder.Capacity)
        {
            Debug.LogError("Invalid sample count");
            return 0;
        }
        //if (samplesCount == 0)
        //    return 0;

        double r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        return r;
    }

    private void OnEnable()
    {
        //renderTimeRecoder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 60);
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", framesToTime);
        //renderTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render", framesToTime);
        //renderTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Gfx.PresentFrame", framesToTime);
        renderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Render Thread", framesToTime);
    }

    private void OnDisable()
    {
        mainThreadTimeRecorder.Dispose();
        renderThreadTimeRecorder.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.realtimeSinceStartupAsDouble;
    }

    // Update is called once per frame
    void Update()
    {
        ExperimentalButtonsStats.TimeSinceStart.Value = (float)(Time.realtimeSinceStartupAsDouble - startTime);
        //ExperimentalButtonsStats.Last60FramesAverage.Sample(renderTimeRecoder.LastValue * 1000.0f);
        //var frameTime = mainThreadTimeRecorder.LastValue; // ? Won't work until we use Unity 2022 ?
        FrameTimingManager.CaptureFrameTimings();
        var ret = FrameTimingManager.GetLatestTimings((uint)frameTimings.Length, frameTimings);
        if (ret > 0)
        {
            Debug.Log("Bingo!");
        }
        var sb = new StringBuilder(500);
        //float value = (float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder);
        //sb.AppendLine($"Frame Time: {value * (1e-6f):F1} ms");
        //sb.AppendLine($"Frame Time: {value}");
        //Debug.Log($"Running? {mainThreadTimeRecorder.IsRunning} => {sb.ToString()}");
        //var temp = (float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder);
        //ExperimentalButtonsStats.LastFrame.Sample(((float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 1)));
        //ExperimentalButtonsStats.Last5FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 5)));
        //ExperimentalButtonsStats.Last10FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 10)));
        //ExperimentalButtonsStats.Last30FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 30)));
        //ExperimentalButtonsStats.Last60FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 60)));

        ExperimentalButtonsStats.LastFrame.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 1)));
        ExperimentalButtonsStats.Last5FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 5)));
        ExperimentalButtonsStats.Last10FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 10)));
        ExperimentalButtonsStats.Last30FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 30)));
        ExperimentalButtonsStats.Last60FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 60)));
        ExperimentalButtonsStats.TestMetric.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 60)));

        Debug.Log($"Frame time: {(float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 1) * (1e-6f)} ms");
        if (Time.realtimeSinceStartup - startTime > 10.0d)
        {
            //ExperimentalButtonsStats.OneSecondRenderingAverage.Sample(909);
            //ExperimentalButtonsStats.SecondMeasure.Value = 313;
        }
        else
        {
            //ExperimentalButtonsStats.OneSecondRenderingAverage.Sample(500);
            //ExperimentalButtonsStats.SecondMeasure.Value = 600;
        }
    }

    private void OnGUI()
    {
        //GUI.TextArea(new Rect(10, 30, 250, 50), statsText);
    }
}
