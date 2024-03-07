using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Text;
using UnityEngine.Timeline;
using System.Linq;

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
    private const float ns2ms = 1000000.0f;
    double GPUFrameTimeAverageSinceStart = 0.0;
    double RenderThreadTimeAverageSinceStart = 0.0;
    double totalFramesSinceStartForGPUFrameTime = 0.0;
    double totalFramesSinceStartForRenderThreadTime = 0.0;

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

    double GetUpdatedGPUFrameTimeAverageSinceStart(FrameTiming[] frameTimings)
    {
        totalFramesSinceStartForGPUFrameTime = totalFramesSinceStartForGPUFrameTime + 1.0;
        double accumulatedTime = GPUFrameTimeAverageSinceStart * (totalFramesSinceStartForGPUFrameTime - 1.0);
        accumulatedTime = accumulatedTime + frameTimings[0].gpuFrameTime;
        totalFramesSinceStartForGPUFrameTime = totalFramesSinceStartForGPUFrameTime + 1.0;
        GPUFrameTimeAverageSinceStart = accumulatedTime / (totalFramesSinceStartForGPUFrameTime);

        return GPUFrameTimeAverageSinceStart;
    }

    double GetUpdateRenderThreadTimeAverageSinceStart(ProfilerRecorder recorder)
    {
        totalFramesSinceStartForRenderThreadTime = totalFramesSinceStartForRenderThreadTime + 1.0;
        double accumulatedTime = RenderThreadTimeAverageSinceStart * (totalFramesSinceStartForRenderThreadTime - 1.0);
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[1];
            recorder.CopyTo(samples, 1);
            accumulatedTime = accumulatedTime + samples[0].Value;
        }
        totalFramesSinceStartForRenderThreadTime = totalFramesSinceStartForRenderThreadTime + 1.0;
        RenderThreadTimeAverageSinceStart = accumulatedTime / (totalFramesSinceStartForRenderThreadTime);

        return RenderThreadTimeAverageSinceStart;
    }

    static double GetGPUFrameTimeAverage(FrameTiming[] frameTimings, int samplesCount)
    {
        if (samplesCount > frameTimings.Length)
        {
            Debug.LogError("Sample count cannot be bigger than frameTimings length.");
        }

        double result = 0;

        for (int i = 0; i < samplesCount; i++)
        {
            result += frameTimings[i].gpuFrameTime;
        }

        result /= samplesCount;

        return result;
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
        ExperimentalButtonsStats.TimeSinceStart.Value = ((float)(Time.realtimeSinceStartupAsDouble - startTime)) * 1000000000.0f;
        //ExperimentalButtonsStats.Last60FramesAverage.Sample(renderTimeRecoder.LastValue * 1000.0f);
        //var frameTime = mainThreadTimeRecorder.LastValue; // ? Won't work until we use Unity 2022 ?
        FrameTimingManager.CaptureFrameTimings();
        var ret = FrameTimingManager.GetLatestTimings((uint)frameTimings.Length, frameTimings);
        if (ret > 0)
        {
            Debug.Log("Bingo!");
            //frameTimings[0].heightScale //ok
            //frameTimings[0].gpuFrameTime;
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

        //ExperimentalButtonsStats.LastFrame.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 1)));
        //ExperimentalButtonsStats.Last5FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 5)));
        //ExperimentalButtonsStats.Last10FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 10)));
        //ExperimentalButtonsStats.Last30FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 30)));
        //ExperimentalButtonsStats.Last60FrameAverage.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 60)));
        //ExperimentalButtonsStats.TestMetric.Sample(((float)GetRecorderFrameAverageBySampleCount(renderThreadTimeRecorder, 60)));
        //ExperimentalButtonsStats.Last1FrameGPUFrameTimeAverage.Sample(frameTimings[0].gpuFrameTime * 1000.0f);
        //ExperimentalButtonsStats.Last1FrameGPUFrameTimeAverage.Sample();
        ExperimentalButtonsStats.Last1FrameGPUFrameTimeAverage.Sample((float)GetGPUFrameTimeAverage(frameTimings, 1) * ns2ms);
        ExperimentalButtonsStats.Last5FrameGPUFrameTimeAverage.Sample((float)GetGPUFrameTimeAverage(frameTimings, 5) * ns2ms);
        ExperimentalButtonsStats.Last10FrameGPUFrameTimeAverage.Sample((float)GetGPUFrameTimeAverage(frameTimings, 10) * ns2ms);
        ExperimentalButtonsStats.Last30FrameGPUFrameTimeAverage.Sample((float)GetGPUFrameTimeAverage(frameTimings, 30) * ns2ms);
        ExperimentalButtonsStats.Last60FrameGPUFrameTimeAverage.Sample((float)GetGPUFrameTimeAverage(frameTimings, 60) * ns2ms);
        ExperimentalButtonsStats.GPUFrameTimeAverageSinceStart.Sample((float)GetUpdatedGPUFrameTimeAverageSinceStart(frameTimings) * ns2ms);
        ExperimentalButtonsStats.RenderThreadTimeAverageSinceStart.Sample((float)GetUpdateRenderThreadTimeAverageSinceStart(renderThreadTimeRecorder) * ns2ms);

        //Debug.Log($"Frame time: {(float)GetRecorderFrameAverageBySampleCount(mainThreadTimeRecorder, 1) * (1e-6f)} ms");
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
