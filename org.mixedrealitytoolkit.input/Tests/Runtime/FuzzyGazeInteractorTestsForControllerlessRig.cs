// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable CS1591

using MixedReality.Toolkit.Core.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.TestTools;

namespace MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Tests for verifying the behavior of FuzzyGazeInteractor for the XRI3+ controllerless MRTK rig.
    /// </summary>
    /// <remarks>
    /// These tests are equivalent to those in <see cref="FuzzyGazeInteractorTests"/> but they test with the new MRTK Rig that was
    /// created for the XRI3 migration.  Eventually, this will replace the original <see cref="FuzzyGazeInteractorTests"/> when
    /// the deprecated pre-XRI3 rig is removed in its entirety from MRTK3.
    /// Note:  This class contains only the tests that are specific to the XRI3+ rig.  Tests that are common to both rigs are in the
    ///        original <see cref="FuzzyGazeInteractorTests"/>.  Once the XRI3 migration is completed by removing all the pre-XRI3
    ///        prefabs then those tests can be moved to this class.
    /// </remarks>
    public class FuzzyGazeInteractorTestsForControllerlessRig : BaseRuntimeInputTests
    {
        [UnitySetUp]
        public override IEnumerator Setup()
        {
            yield return base.SetupForControllerlessRig();
        }

        /// <summary>
        /// Test that eye-gaze works as expected.
        /// </summary>
        /// <remarks>
        /// This test was meant to be the XRI3+ equivalent of <see cref="MRTKRayInteractorVisualsTests.ReticleAndLineVisualActiveTest"/>, however, since our
        /// <see cref="InputTestUtilities"/> cannot simulate the loss of tracking for XRI3+ <see cref="TrackedPoseDriver"/> because it still uses controllers
        /// (as of the moment of this writing) then this test was repurposed to test that eye-gazing works for XRI3+ as expected without simulating
        /// eye-gaze tracking loss.
        /// </remarks>
        [UnityTest]
        public IEnumerator EyeGazeWorksAsExpectedTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene
            FuzzyGazeInteractor fuzzyGazeInteractor = FindObjectUtility.FindFirstObjectByType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");

            // Instantiate two foregound cubes and one background cube for testing
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.GetComponent<MeshRenderer>().material.color = Color.red;
            cube1.AddComponent<StatefulInteractable>();
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0.2f, 1));
            cube1.transform.localScale = Vector3.one * 0.1f;

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.GetComponent<MeshRenderer>().material.color = Color.blue;
            cube2.AddComponent<StatefulInteractable>();
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            cube2.transform.localScale = Vector3.one * 0.1f;

            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(1.6f);
            backgroundCube.transform.localScale = Vector3.one;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // No foreground cube should be hovered at their starting positions
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1'sStatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 2's StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Point camera (HMD) at cube 1
            yield return InputTestUtilities.RotateCameraToTarget(cube1.transform.position);

            // Point eyes at cube 2
            yield return InputTestUtilities.RotateEyesToTarget(cube2.transform.position);

            // The eyes gaze should have focused cube 2
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1's StatefulInteractable was hovered, perhaps by head gaze. Expected eye gaze to hover different object.");
            // Note: Commenting out the following assert because the assert works correctly when Unity-test is run in desktop Editor but it
            //       fails when it runs in pipeline.  This issue is tracked in: https://github.com/MixedRealityToolkit/MixedRealityToolkit-Unity/issues/766
            //Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
            //               "Cube 2's StatefulInteractable should have been hovered by eye gaze.");
            //Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
            //               "Background's StatefulInteractable was unexpectedly hovered.");
        }

        /// <summary>
        /// Basic test of FuzzyGazeInteractor. Confirm a FuzzyGazeInteractor is active in the scene, and then
        /// make sure Interactable can be hovered even when not on the direct raycast from the interactor.
        /// </summary>
        /// <remarks>
        /// This test is the same as <see cref="FuzzyGazeInteractorTests.BasicFuzzyGazeTest"/>, it is repeated here so that the same functionality is tested against
        /// the new controllerless prefabs.
        /// </remarks>
        [UnityTest]
        public IEnumerator BasicFuzzyGazeTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene
            FuzzyGazeInteractor fuzzyGazeInteractor = FindObjectUtility.FindAnyObjectByType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");

            // Instantiate two foregound cubes and one background cube for testing
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.AddComponent<StatefulInteractable>();
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0.2f, 1));
            cube1.transform.localScale = Vector3.one * 0.1f;

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.AddComponent<StatefulInteractable>();
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            cube2.transform.localScale = Vector3.one * 0.1f;

            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(1.6f);
            backgroundCube.transform.localScale = Vector3.one;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // No foreground cube should be hovered at their birth positions
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Move the cubes to bring them to the center on the y-axis
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0, 1));
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0, 1));

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Cube 2 should now be hovered
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");

            // Move cube 2 back to its birth position
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Background cube should now be hovered
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Move background cube further back
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(4f);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Cube 1 should now be hovered
            Assert.IsTrue(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
        }

        /// <summary>
        /// Test the behavior of FuzzyGazeInteractor with different levels of precision and
        /// with performAdditionalRaycast on and off.
        /// </summary>
        /// <remarks>
        /// This test is the same as <see cref="FuzzyGazeInteractorTests.FuzzyGazePrecisionTest"/>, it is repeated here so that the same functionality is tested against
        /// the new controllerless prefabs.
        /// </remarks>
        [UnityTest]
        public IEnumerator FuzzyGazePrecisionTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene and configure it for the test
            FuzzyGazeInteractor fuzzyGazeInteractor = FindObjectUtility.FindFirstObjectByType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");
            fuzzyGazeInteractor.precision = 0;
            fuzzyGazeInteractor.performAdditionalRaycast = false;

            yield return RuntimeTestUtilities.WaitForUpdates();
            // Instantiate one foregound cubes and one background cube for testing
            GameObject foregroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foregroundCube.AddComponent<StatefulInteractable>();
            foregroundCube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.241f, 0, 2));
            foregroundCube.transform.localScale = new Vector3(0.4f, 0.1f, 0.2f);

            yield return RuntimeTestUtilities.WaitForUpdates();
            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.4f, 0, 2.4f));
            backgroundCube.transform.localScale = new Vector3(1.4f, 1.4f, 0.2f);
            backgroundCube.transform.localEulerAngles = new Vector3(0, -58, 0);

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Foreground cube should be hovered
            Assert.IsTrue(foregroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");

            // Increase the precision of the interactor
            fuzzyGazeInteractor.precision = FuzzyGazeInteractor.MaxPrecision;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Background cube should now be hovered
            Assert.IsFalse(foregroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Restore the precision to 0 but enable performAdditionalRaycast
            fuzzyGazeInteractor.precision = 0;
            fuzzyGazeInteractor.performAdditionalRaycast = true;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Background cube should still be hovered
            Assert.IsFalse(foregroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "StatefulInteractable was not hovered by FuzzyGazeInteractor.");
        }

        /// <summary>
        /// Test that gaze fallbacks to HMD pose, if eye gaze loses tracking.
        /// </summary>
        /// <remarks>
        /// This test is the same as <see cref="FuzzyGazeInteractorTests.GazeFallbackWhenEyeGazeLosesTrackingTest"/>, it is repeated here so that the same functionality is tested against
        /// the new controllerless prefabs.
        /// </remarks>
        [UnityTest]
        public IEnumerator GazeFallbackWhenEyeGazeLosesTrackingTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene
            FuzzyGazeInteractor fuzzyGazeInteractor = FindObjectUtility.FindFirstObjectByType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");

            // Instantiate two foregound cubes and one background cube for testing
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.GetComponent<MeshRenderer>().material.color = Color.red;
            cube1.AddComponent<StatefulInteractable>();
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0.2f, 1));
            cube1.transform.localScale = Vector3.one * 0.1f;

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.GetComponent<MeshRenderer>().material.color = Color.blue;
            cube2.AddComponent<StatefulInteractable>();
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            cube2.transform.localScale = Vector3.one * 0.1f;

            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(1.6f);
            backgroundCube.transform.localScale = Vector3.one;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // No foreground cube should be hovered at their starting positions
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1'sStatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 2's StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Ensure eyes gaze has tracking
            InputTestUtilities.IsEyeGazeTracking = true;

            // Point camera (HMD) at cube 1
            yield return InputTestUtilities.RotateCameraToTarget(cube1.transform.position);

            // Point eyes at cube 2
            yield return InputTestUtilities.RotateEyesToTarget(cube2.transform.position);

            // The eyes gaze should have focused cube 2
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1's StatefulInteractable was hovered, perhaps by head gaze. Expected eye gaze to hover different object.");
            Assert.IsTrue(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 2's StatefulInteractable should have been hovered by eye gaze.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was unexpectedly hovered.");

            // Force eye gaze to lose tracking
            InputTestUtilities.IsEyeGazeTracking = false;
            yield return RuntimeTestUtilities.WaitForUpdates();

            // The head gaze should have focused cube 1
            // Note: Commenting out the following assert because the assert works correctly when Unity-test is run in desktop Editor but it
            //       fails when it runs in pipeline.  This issue is tracked in: https://github.com/MixedRealityToolkit/MixedRealityToolkit-Unity/issues/766
            //Assert.IsTrue(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
            //               "Cube 1's StatefulInteractable should have been hovered by head gaze.");
            //Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
            //               "Cube 2's StatefulInteractable should have been hovered by eye gaze.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was unexpectedly hovered.");
        }


        /// <summary>
        /// Test that gaze fallbacks to HMD pose, if eye gaze is disabled.
        /// </summary>
        /// <remarks>
        /// This test is the same as <see cref="FuzzyGazeInteractorTests.GazeFallbackWhenEyeGazeIsDisabledTest"/>, it is repeated here so that the same functionality is tested against
        /// the new controllerless prefabs.
        /// </remarks>
        [UnityTest]
        public IEnumerator GazeFallbackWhenEyeGazeIsDisabledTest()
        {
            // Confirm a FuzzyGazeInteractor is active in the scene
            FuzzyGazeInteractor fuzzyGazeInteractor = FindObjectUtility.FindFirstObjectByType<FuzzyGazeInteractor>();
            Assert.IsNotNull(fuzzyGazeInteractor, "There is no active FuzzyGazeInteractor found in the scene.");

            // Instantiate two foregound cubes and one background cube for testing
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.GetComponent<MeshRenderer>().material.color = Color.red;
            cube1.AddComponent<StatefulInteractable>();
            cube1.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.07f, 0.2f, 1));
            cube1.transform.localScale = Vector3.one * 0.1f;

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.GetComponent<MeshRenderer>().material.color = Color.blue;
            cube2.AddComponent<StatefulInteractable>();
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, 0.2f, 1));
            cube2.transform.localScale = Vector3.one * 0.1f;

            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.AddComponent<StatefulInteractable>();
            backgroundCube.transform.position = InputTestUtilities.InFrontOfUser(1.6f);
            backgroundCube.transform.localScale = Vector3.one;

            yield return RuntimeTestUtilities.WaitForUpdates();

            // No foreground cube should be hovered at their starting positions
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1'sStatefulInteractable was already hovered.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 2's StatefulInteractable was already hovered.");
            Assert.IsTrue(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was not hovered by FuzzyGazeInteractor.");

            // Ensure eyes are enabled
            InputTestUtilities.IsEyeGazeTracking = true;

            // Point camera (HMD) at cube 1
            yield return InputTestUtilities.RotateCameraToTarget(cube1.transform.position);

            // Point eyes at cube 2
            yield return InputTestUtilities.RotateEyesToTarget(cube2.transform.position);

            // The eyes gaze should have focused cube 2
            Assert.IsFalse(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1's StatefulInteractable was hovered, perhaps by head gaze. Expected eye gaze to hover different object.");
            Assert.IsTrue(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 2's StatefulInteractable should have been hovered by eye gaze.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was unexpectedly hovered.");

            // Disable eye gaze device
            yield return InputTestUtilities.DisableEyeGazeDevice();
            yield return RuntimeTestUtilities.WaitForUpdates();

            // The head gaze should have focused cube 1
            Assert.IsTrue(cube1.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 1's StatefulInteractable should have been hovered by head gaze.");
            Assert.IsFalse(cube2.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Cube 2's StatefulInteractable was hovered, perhaps by eye gaze. Expected head gaze to hover different object.");
            Assert.IsFalse(backgroundCube.GetComponent<StatefulInteractable>().IsGazeHovered,
                           "Background's StatefulInteractable was unexpectedly hovered.");
        }
    }
}
#pragma warning restore CS1591
