using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Text;

class ExperimentalButtonsStats
{
    public static readonly ProfilerCategory ExperimentalButtonsStatsCategory = new ProfilerCategory("Experimental\nButtons Stats");

    public static readonly ProfilerCounter<float> LastFrame =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Last frame rendering",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last5FrameAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Last 5 frames average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last10FrameAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Last 10 frames average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last30FrameAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Last 30 frames average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last60FrameAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Last 60 frames average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> RenderThreadTimeAverageSinceStart =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Render Thread time average since start",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> TestMetric =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Test metric",
                                      ProfilerMarkerDataUnit.Undefined);

    public static readonly ProfilerCounter<float> Last1FrameGPUFrameTimeAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "1 GPUFrameTime average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last5FrameGPUFrameTimeAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "5 GPUFrameTime average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last10FrameGPUFrameTimeAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "10 GPUFrameTime average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last30FrameGPUFrameTimeAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "30 GPUFrameTime average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> Last60FrameGPUFrameTimeAverage =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "60 GPUFrameTime average",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static readonly ProfilerCounter<float> GPUFrameTimeAverageSinceStart =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "GPUFrameTime average since start",
                                      ProfilerMarkerDataUnit.TimeNanoseconds);

    public static ProfilerCounterValue<float> TimeSinceStart =
            new ProfilerCounterValue<float>(ExperimentalButtonsStatsCategory,
                                            "Time since start",
                                            ProfilerMarkerDataUnit.TimeNanoseconds,
                                            ProfilerCounterOptions.FlushOnEndOfFrame);

}
