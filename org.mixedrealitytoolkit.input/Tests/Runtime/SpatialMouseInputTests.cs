// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable CS1591

using MixedReality.Toolkit.Input.Experimental;
using MixedReality.Toolkit.Core.Tests;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using System.Collections.Generic;

using Unity.XR.CoreUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Basic tests for verifying mouse interactions.
    /// </summary>
    public class SpatialMouseInputTests : BaseRuntimeInputTests
    {
        private const string SpatialMouseControllerPrefabGuid = "dc525621b8522034e867ed2799129315";
        private static readonly string SpatialMouseControllerPrefabPath = AssetDatabase.GUIDToAssetPath(SpatialMouseControllerPrefabGuid);

        private static GameObject spatialMouseControllerGameObject;
        private static SpatialMouseInteractor spatialMouseInteractor;

        private const string CameraOffsetName = "Camera Offset";
        private const string MRTKSpatialMouseControllerName = "MRTK Spatial Mouse Controller";
        private const string MRTKSpatialMouseInteractorName = "SpatialMouseInteractor";

        [UnitySetUp]
        public override IEnumerator Setup()
        {
            yield return base.SetupForControllerlessRig();
            spatialMouseControllerGameObject = InstantiateSpatialMouseController();

            List<GameObject> rigChildren = new List<GameObject>();
            InputTestUtilities.RigReference.GetChildGameObjects(rigChildren);
            var cameraOffset = rigChildren.Find(go => go.name == CameraOffsetName);
            spatialMouseControllerGameObject.transform.parent = cameraOffset.transform;

            List<GameObject> spatialMouseControllerChildren = new List<GameObject>();
            spatialMouseControllerGameObject.GetChildGameObjects(spatialMouseControllerChildren);
            spatialMouseInteractor = spatialMouseControllerChildren.Find(go => go.name == MRTKSpatialMouseInteractorName).GetComponent<SpatialMouseInteractor>();

            yield return null;
        }

        /// <summary>
        /// Very basic test of SpatialMouseInteractor clicking an Interactable.
        /// </summary>
        [UnityTest]
        [Ignore("Temporarily ignoring this while while its XRI3+ equivalent is created.")]
        public IEnumerator SpatialMouseInteractorSmokeTest()
        {
            var mouse = InputSystem.AddDevice<Mouse>();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<StatefulInteractable>();
            cube.transform.position = new Vector3(0, 0, 0.4f);
            cube.transform.localScale = Vector3.one * 0.1f;

            // For this test, we won't use poke selection.
            cube.GetComponent<StatefulInteractable>().DisableInteractorType(typeof(PokeInteractor));

            StatefulInteractable firstCubeInteractable = cube.GetComponent<StatefulInteractable>();

            // Verify that the mouse is hidden by default.
            var notHoveringMouseInteractor = firstCubeInteractable.HoveringRayInteractors.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNull(notHoveringMouseInteractor,
                "SpatialMouseInteractor is hovering without initial input.");

            // Inject mouse deltas.
            using (StateEvent.From(mouse, out var eventPtr))
            {
                ((DeltaControl)mouse["delta"]).WriteValueIntoEvent(new Vector2(1, 1), eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();

                ((DeltaControl)mouse["delta"]).WriteValueIntoEvent(new Vector2(-1, -1), eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();
            }

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that the mouse is hovering the cube.
            var hoveringMouseInteractor = firstCubeInteractable.HoveringRayInteractors.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNotNull(hoveringMouseInteractor,
                "StatefulInteractable did not get Hovered by SpatialMouseInteractor.");

            // Inject mouse down.
            /* Original code
            using (StateEvent.From(mouse, out var eventPtr))
            {
                ((ButtonControl)mouse["press"]).WriteValueIntoEvent(1f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();
            }/**/

            spatialMouseInteractor.selectInput.QueueManualState(true, 1f);
            yield return null;

            // Verify that the mouse is selecting the cube.
            var selectingMouseInteractor = firstCubeInteractable.interactorsSelecting.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNotNull(selectingMouseInteractor,
                "StatefulInteractable did not get Selected by SpatialMouseInteractor.");

            /* Original code
            // Inject mouse up.
            using (StateEvent.From(mouse, out var eventPtr))
            {
                ((ButtonControl)mouse["press"]).WriteValueIntoEvent(0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();
            }/**/

            spatialMouseInteractor.selectInput.QueueManualState(false, 0f);
            yield return null;
            //yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that the mouse is no longer selecting the cube.
            var notSelectingMouseInteractor = firstCubeInteractable.interactorsSelecting.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNull(notSelectingMouseInteractor,
                "StatefulInteractable did not get Unselected by SpatialMouseInteractor.");

            yield return null;
        }

        /// <summary>
        /// Creates and returns the Spatial Mouse Controller.
        /// </summary>
        public static GameObject InstantiateSpatialMouseController()
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(SpatialMouseControllerPrefabPath, typeof(Object));
            spatialMouseControllerGameObject = Object.Instantiate(prefab) as GameObject;
            return spatialMouseControllerGameObject;
        }

        /// <summary>
        /// Destroys the Spatial Mouse Controller.
        /// </summary>
        public static void TeardownSpatialMouseController()
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(spatialMouseControllerGameObject);
            }
        }

        public override IEnumerator TearDown()
        {
            TeardownSpatialMouseController();

            yield return base.TearDown();
        }
    }
}
#pragma warning restore CS1591
