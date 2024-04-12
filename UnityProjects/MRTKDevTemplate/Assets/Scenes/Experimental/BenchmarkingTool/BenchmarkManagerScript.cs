using System.Collections;
using UnityEngine;
using TMPro;
using System.Text;
using System;
using System.Collections.Generic;

public class BenchmarkManagerScript : MonoBehaviour
{
    private const string AUTO_START_NOT_SET = "Auto-start not set in Editor";
    private const string FPS_LABEL_TAG = "FPSLabel";
    private const string TESTSTARTSIN_TAG = "TestStartsIn";
    private const string FPS_PREFIX = "FPS: ";
    private const string TEST_START_MESSAGE_TAG = "TestStartMessage";
    private const string SCENE_DESCRIPTION_PANEL_TAG = "SceneDescriptionPanel";
    private const string CLONABLE_PRESSABLE_BUTTON_TAG = "ClonablePressableButton";
    private const string TOTAL_BUTTONS_IN_SCENE_TAG = "TotalButtonsInScene";
    private const string TITLE_TAG = "Title";
    private const string PER_RUN_RESULT = "PerRunResult";

    // Text for the DescriptionPanel's Subtitle when the benchmark starts
    private const string BENCHMARK_STARTED_MESSAGE = "Benchmark started!";

    // Text for the DescriptionPanel's Subtitle when the benchmark completes
    private const string BENCHMARK_COMPLETED_MESSAGE = "All benchmark runs completed.";

    // Text for the DescriptionPanel's Subtitle
    private const string BENCHMARK_STARTS_IN = "Benchmark starts in ";

    private const string PER_RUN_RESULT_TEXT = "Runs {0} of {1} completed.\nAverage: {2}\nMin total buttons: {3}\nMax total buttons: {4}";

    // Whether to start the test automatically when the framerate has been above threshold for more than n seconds.
    [SerializeField]
    private bool autoStartEnabled = true;

    // Minimum acceptable framerate for starting the test.
    [SerializeField]
    private int acceptableFPSForAutoStartingBenchmark = 30;

    // How many seconds to wait before starting the benchmark automatically.
    [SerializeField]
    private int SecondsToAutoStartBenchmark = 5;

    // Reference to the GameObject to spawn.
    [SerializeField]
    private GameObject clonablePressableButton;

    // Parent for the instantiated objects.
    [SerializeField]
    private GameObject canvasParent;

    // Distance between the instantiated objects.
    [SerializeField]
    private float offset = 0;

    // How many columns to use to display the instantiated objects.
    [SerializeField]
    private int columns = 20;

    // How many rows to use to display the instantiated objects.
    [SerializeField]
    private int rows = 10;

    // The test runs until it drops below this framerate.
    [SerializeField]
    private int targetLowFramerate = 50;

    // How many seconds between updates of the framerate text.
    [SerializeField]
    private float secondsBetweenFramerateUpdates = 0.25f;

    // Horizontal offset when CanvasParent is null in BenchmarkManager (set in Editor)
    [SerializeField]
    private float nonCanvasHorizontalOffset = -0.85f;

    // Vertical offset when CanvasParent is null in BenchmarkManager (set in Editor)
    [SerializeField]
    private float nonCanvasVerticalOffset = -0.85f;

    // Depth offset when CanvasParent is null in BenchmarkManager (set in Editor)
    [SerializeField]
    private float nonCanvasDepthOffset = 0.95f;

    // How many times to run the benchmark (set in Editor).
    [SerializeField]
    private int benchmarkRunPasses = 10;

    // Counter for the benchmark runs.
    private int benchmarkRunPassesCounter = 0;

    // The average of buttons per benchmark runs.
    private float buttonsCounterAverage = 0f;

    // The maximum total buttons in all benchmark runs.
    private float buttonsMaxTotal = float.MinValue;

    // The minimum total buttons in all benchmark runs.
    private float buttonsMinTotal = float.MaxValue;

    // Which column is being filled.
    private int yRank = 0;

    // y-Axis local distance for the current instantiated object.
    private float yOffset = 0.0f;

    // Which rank in the z axis is being filled.
    private int zRank = 0;

    // x-Axis local distance for the current instantiated object.
    private float zOffset = 0.0f;

    // How many frames have had the framerate below the target.
    private int lowFramerateFrameCount = 0;

    // Boolean for tracking the end of the test.
    private bool testComplete = true;

    // Current object count.
    private int currentCount = 0;

    // List for tracking the instantiated objects.
    private List<GameObject> testObjects = new List<GameObject>();

    // tracking field for the framerate text update.
    private float secondsSinceLastFramerateUpdate = 0.0f;

    // How many frames before instantiating the next object.
    private int frameWait = 10;

    // The current framerate.
    private float frameRate = 0f;

    // Whether the previous frame was acceptable for the test to start.
    private bool frameRateWasAcceptableInPreviousStep = false;

    private int buttonCount = 0;
    private Vector3 newButtonOffset;
    private bool waitingToStartTest = true;
    private DateTime frameRateBecomeAcceptable;
    private GameObject sceneDescriptionPanel;
    private GameObject totalButtonsInScene;
    private GameObject perPerRunResult;
    private GameObject title;
    public AnimationCurve resultAnimationCurve;
    private TextMeshPro fpsLabelTMP;
    private TextMeshPro testStartsInTMP;
    private TextMeshPro totalButtonsInSceneTMP;
    private TextMeshProUGUI titleTMPUGUI;
    private TextMeshPro perPerRunResultTMP;
    private static StringBuilder FPS_LABEL_TEXT = new StringBuilder(FPS_PREFIX);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("[BenchmarkManagerScript] Start()");

        // prevent divide by zero
        if (columns == 0)
        {
            columns = 20;
        }

        sceneDescriptionPanel = GameObject.FindGameObjectWithTag(SCENE_DESCRIPTION_PANEL_TAG);
        totalButtonsInScene = GameObject.FindGameObjectWithTag(TOTAL_BUTTONS_IN_SCENE_TAG);
        title = GameObject.FindGameObjectWithTag(TITLE_TAG);

        fpsLabelTMP = GameObject.FindGameObjectWithTag(FPS_LABEL_TAG).GetComponent<TextMeshPro>();
        totalButtonsInSceneTMP = GameObject.FindGameObjectWithTag(TOTAL_BUTTONS_IN_SCENE_TAG).GetComponent<TextMeshPro>();
        testStartsInTMP = GameObject.FindGameObjectWithTag(TESTSTARTSIN_TAG).GetComponent<TextMeshPro>();
        titleTMPUGUI = GameObject.FindGameObjectWithTag(TITLE_TAG).GetComponent<TextMeshProUGUI>();
        perPerRunResult = GameObject.FindGameObjectWithTag(PER_RUN_RESULT);
        perPerRunResultTMP = GameObject.FindGameObjectWithTag(PER_RUN_RESULT).GetComponent<TextMeshPro>();
        GameObject.FindGameObjectWithTag(TEST_START_MESSAGE_TAG).GetComponent<TextMeshProUGUI>().text = $"Benchmark starts when current frame rate is > {acceptableFPSForAutoStartingBenchmark} fps for more than {SecondsToAutoStartBenchmark} seconds.";

        ValidateNeededGameObjectsReferences();

        if (!autoStartEnabled)
        {
            testStartsInTMP.text = AUTO_START_NOT_SET;
        }

        titleTMPUGUI.text += $" ({Application.version})";

        newButtonOffset = new Vector3(0.1f, 0.1f, 0.1f);
        totalButtonsInSceneTMP.text = buttonCount.ToString();

        UpdatePerRunResult();

        buttonsMaxTotal = float.MinValue;
        buttonsMinTotal = float.MaxValue;
    }

    private void UpdatePerRunResult()
    {
        // Update the min and max total buttons in all benchmark runs
        buttonsMaxTotal = currentCount > buttonsMaxTotal ? currentCount : buttonsMaxTotal;
        buttonsMinTotal = currentCount < buttonsMinTotal ? currentCount : buttonsMinTotal;

        perPerRunResultTMP.text = string.Format(PER_RUN_RESULT_TEXT, benchmarkRunPassesCounter, benchmarkRunPasses, buttonsCounterAverage, buttonsMinTotal, buttonsMaxTotal);
    }

    public void StartTest()
    {
        // Reset min and max per-run counters
        //buttonsMaxTotal = float.MinValue;
        //buttonsMinTotal = float.MaxValue;

        // Trigger the test
        title.SetActive(false);
        lowFramerateFrameCount = 0;
        SetModelCount(0);
        testComplete = false;
    }

    /// <summary>
    /// A Unity event function that is called every frame after normal update functions, if this object is enabled.
    /// </summary>
    private void LateUpdate()
    {
        // Framerate calculations.
        secondsSinceLastFramerateUpdate += Time.deltaTime;
        frameRate = (int)(1.0f / Time.smoothDeltaTime);
        if (secondsSinceLastFramerateUpdate >= secondsBetweenFramerateUpdates)
        {
            fpsLabelTMP.text = frameRate.ToString();
            secondsSinceLastFramerateUpdate = 0;
        }
    }

    /// <summary>
    /// Validates that all needed gameobjects references are not null.
    /// </summary>
    private void ValidateNeededGameObjectsReferences()
    {
        if (fpsLabelTMP == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): fpsLabelTMP reference is null.");
        }
        if (testStartsInTMP == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): testStartsInTMP reference is null.");
        }
        if (sceneDescriptionPanel == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): sceneDescriptionPanel reference is null.");
        }
        if (clonablePressableButton == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): clonablePressableButton reference is null.");
        }
        if (totalButtonsInScene == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): totalButtonsinScene reference is null.");
        }
        if (totalButtonsInSceneTMP == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): totalButtonsInSceneTMP reference is null.");
        }
        if (titleTMPUGUI == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): titleTMP reference is null.");
        }
        if (perPerRunResultTMP == null)
        {
            Debug.LogError($"[BenchmarkManagerScript] Start(): perPerRunResultTMP reference is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingToStartTest && autoStartEnabled) //For auto-start feature
        {
            if (frameRate > acceptableFPSForAutoStartingBenchmark && !frameRateWasAcceptableInPreviousStep)
            {
                frameRateWasAcceptableInPreviousStep = true;
                frameRateBecomeAcceptable = DateTime.Now;
            }
            else if (frameRate < acceptableFPSForAutoStartingBenchmark)
            {
                frameRateWasAcceptableInPreviousStep = false;
            }
            else if (frameRate > acceptableFPSForAutoStartingBenchmark && frameRateWasAcceptableInPreviousStep)
            {
                TimeSpan timeSpan = DateTime.Now - frameRateBecomeAcceptable;
                testStartsInTMP.text = FPS_LABEL_TEXT.Clear().Append(BENCHMARK_STARTS_IN).Append(SecondsToAutoStartBenchmark - timeSpan.Seconds).ToString();
                if (timeSpan.Seconds > SecondsToAutoStartBenchmark)
                {
                    waitingToStartTest = false;
                    testStartsInTMP.text = BENCHMARK_STARTED_MESSAGE;
                    StartTest();
                }
            }
        }
        else
        {

            if (testComplete)
            {
                return;
            }

            if (frameRate < targetLowFramerate)
            {
                lowFramerateFrameCount++;
            }

            if (currentCount < 2000 && lowFramerateFrameCount < 60)
            {
                if (frameWait == 0)
                {
                    int cachedCount = currentCount;
                    cachedCount++;
                    SetModelCount(cachedCount);
                    frameWait = 10;
                }
                else
                    frameWait--;
            }
            else
            {
                testComplete = true;
                testStartsInTMP.text = BENCHMARK_COMPLETED_MESSAGE;
                titleTMPUGUI.text = $"Test dropped below target framerate after {currentCount} objects.  Test complete.";
                Debug.Log(titleTMPUGUI.text);
                title.SetActive(false);

                UpdateMultiPassBenchmarkStatus();
            }
        }
    }

    private void UpdateMultiPassBenchmarkStatus()
    {
        benchmarkRunPassesCounter++;
        if (benchmarkRunPassesCounter <= benchmarkRunPasses)
        {
            frameRateWasAcceptableInPreviousStep = false;
            if (benchmarkRunPassesCounter < benchmarkRunPasses)
            {
                waitingToStartTest = true;
            }
            testComplete = false;

            //Compute new average considering previous average and the count of the current run
            float accumulatedCount = buttonsCounterAverage * (benchmarkRunPassesCounter - 1);
            accumulatedCount += currentCount;
            buttonsCounterAverage = accumulatedCount / benchmarkRunPassesCounter;

            UpdatePerRunResult();
        }
        else
        {
            StartCoroutine(resultAnimationEnumerator());
            testStartsInTMP.text = BENCHMARK_COMPLETED_MESSAGE;
        }
    }

    /// <summary>
    /// Animates the result panel.
    /// </summary>
    /// <returns>IEnumerator yield</returns>
    IEnumerator resultAnimationEnumerator()
    {
        float curveTime = 0.0f;
        float curveValue = resultAnimationCurve.Evaluate(curveTime);
        float scaleMultiplier = 1.25f;
        float speedMultiplier = 0.4f;
        Vector3 originalScale = perPerRunResult.transform.localScale;

        while (curveValue < 1.0f)
        {
            curveTime += Time.unscaledDeltaTime * speedMultiplier;
            curveValue = resultAnimationCurve.Evaluate(curveTime);

            perPerRunResult.transform.localScale = originalScale + new Vector3(curveValue * scaleMultiplier, curveValue * scaleMultiplier, 0);

            yield return null;
        }
    }

    public void SetModelCount(int count)
    {
        if (count < currentCount)
        {
            // delete models
            while (count < currentCount && testObjects.Count > 0)
            {
                Destroy(testObjects[testObjects.Count - 1]);
                testObjects.RemoveAt(testObjects.Count - 1);
                currentCount--;
            }
        }
        else if (count > currentCount)
        {
            // spawn object
            while (count > currentCount)
            {
                var m = Instantiate(clonablePressableButton);

                if (canvasParent != null) //use canvasParent to add the new buttons as its children
                {
                    //m.transform.parent = canvasParent.transform;
                    m.transform.SetParent(canvasParent.transform, false);
                }

                m.transform.localScale = Vector3.one;
                zRank = currentCount / (rows * columns);
                zOffset = zRank * offset;
                yRank = (int)(currentCount / columns);
                yOffset = (yRank % rows) * offset;

                if (canvasParent != null)
                {
                    m.transform.localPosition = new Vector3((currentCount % columns) * offset, yOffset, zOffset);
                }
                else
                {
                    m.transform.localPosition = new Vector3((currentCount % columns) * offset + nonCanvasHorizontalOffset, yOffset + nonCanvasVerticalOffset, zOffset + nonCanvasDepthOffset);
                }

                testObjects.Add(m);
                currentCount++;
            }
        }

        totalButtonsInSceneTMP.text = currentCount.ToString();
    }
}
