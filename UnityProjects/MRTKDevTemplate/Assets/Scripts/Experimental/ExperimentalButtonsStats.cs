using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Text;

//public class ExperimentalButtonsCustomProfiler : MonoBehaviour
class ExperimentalButtonsStats
{
    //public static readonly ProfilerCategory ExperimentalButtonsProfilerCategory = ProfilerCategory.Scripts;
    //public static readonly ProfilerCategory MyProfilerCategory = ProfilerCategory.Scripts;
    public static readonly ProfilerCategory ExperimentalButtonsStatsCategory = new ProfilerCategory("Experimental\nButtons Stats");

    //public static readonly ProfilerCounter<float> Last10SecondsRenderingTimeAverage = new ProfilerCounter<float>(ExperimentalButtonsProfilerCategory,
    //                                                                                                             "10 seconds average",
    //                                                                                                             ProfilerMarkerDataUnit.Count);
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

    public static readonly ProfilerCounter<float> TestMetric =
           new ProfilerCounter<float>(ExperimentalButtonsStatsCategory,
                                      "Test metric",
                                      ProfilerMarkerDataUnit.Undefined);

    //public static ProfilerCounterValue<float> SecondMeasure =
    //        new ProfilerCounterValue<float>(ExperimentalButtonsStatsCategory,
    //                                        "Tempo",
    //                                        ProfilerMarkerDataUnit.Undefined,
    //                                        ProfilerCounterOptions.FlushOnEndOfFrame);

    public static ProfilerCounterValue<float> TimeSinceStart =
            new ProfilerCounterValue<float>(ExperimentalButtonsStatsCategory,
                                            "Time since start",
                                            ProfilerMarkerDataUnit.TimeNanoseconds,
                                            ProfilerCounterOptions.FlushOnEndOfFrame);
}
