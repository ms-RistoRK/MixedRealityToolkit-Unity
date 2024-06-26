// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MixedReality.Toolkit.Input
{
    /// <summary>
    /// An abstract XRDirectInteractor that represents a near interaction
    /// driven by articulated hand data. Implementers should define <see cref="TryGetInteractionPoint"/>
    /// to specify the desired interaction point, using the cached reference to
    /// the hands aggregator subsystem.
    /// </summary>
    public abstract class HandJointInteractor :
        XRDirectInteractor,
        IHandedInteractor
    {
        #region HandJointInteractor

        /// <summary>
        /// Concrete implementations should override this function to specify the point
        /// at which the interaction occurs. This would be the tip of the index finger
        /// for a poke interactor, or some other computed position from other data sources.
        /// </summary>
        protected abstract bool TryGetInteractionPoint(out Pose pose);

        #endregion HandJointInteractor

        #region IHandedInteractor

        /// <inheritdoc/>
        Handedness IHandedInteractor.Handedness
        {
            get
            {
#pragma warning disable CS0618 // xrController is obsolete
                if (forceDeprecatedInput)
                {
#pragma warning disable CS0612 // ArticulatedHandController is obsolete
                    return (xrController is ArticulatedHandController handController) ? handController.HandNode.ToHandedness() : Handedness.None;
#pragma warning restore CS0612 // ArticulatedHandController is obsolete
                }
                else
                {
                    return handedness.ToHandedness();
                }
#pragma warning restore CS0618 // xrController is obsolete
            }
        }

        #endregion IHandedInteractor

        #region XRBaseInteractor

        /// <summary>
        /// Used to keep track of whether the controller has an interaction point.
        /// </summary>
        private bool interactionPointTracked;

        /// <summary>
        /// Indicates whether this Interactor is in a state where it could hover.
        /// </summary>
        public override bool isHoverActive
        {
            // Only be available for hovering if the controller is tracked or we have joint data.
            get
            {
#pragma warning disable CS0618 // XRBaseController is obsolete
                if (forceDeprecatedInput)
                {
                    return base.isHoverActive && (xrController.currentControllerState.inputTrackingState.HasPositionAndRotation() || interactionPointTracked);
                }
#pragma warning restore CS0618 // XRBaseController is obsolete
                else
                {
                    TrackedPoseDriver trackedPoseDriver = transform.GetComponentInParent<TrackedPoseDriver>();
                    bool hasPositionAndRotation = ((InputTrackingState)trackedPoseDriver.trackingStateInput.action.ReadValue<int>()).HasPositionAndRotation();
                    return base.isHoverActive && (hasPositionAndRotation || interactionPointTracked);
                }
            }
        }

        #endregion XRBaseInteractor

        #region XRBaseInputInteractor

        private static readonly ProfilerMarker ProcessInteractorPerfMarker =
            new ProfilerMarker("[MRTK] HandJointInteractor.ProcessInteractor");

        /// <summary>
        /// Unity's <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.4/api/UnityEngine.XR.Interaction.Toolkit.XRInteractionManager.html">XRInteractionManager</see> 
        /// or containing <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.4/api/UnityEngine.XR.Interaction.Toolkit.IXRInteractionGroup.html">IXRInteractionGroup</see> 
        /// calls this method to update the Interactor before interaction events occur. See Unity's documentation for more information.
        /// </summary>
        /// <param name="updatePhase">The update phase this is called during.</param>
        /// <remarks>
        /// Please see the <see href="https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.4/api/UnityEngine.XR.Interaction.Toolkit.XRInteractionManager.html">XRInteractionManager</see> documentation for more
        /// details on update order.
        /// </remarks>
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            using (ProcessInteractorPerfMarker.Auto())
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    // Obtain near interaction point, and set our interactor's
                    // position/rotation to the interaction point's pose.
                    interactionPointTracked = TryGetInteractionPoint(out Pose interactionPose);
                    if (interactionPointTracked)
                    {
                        transform.SetPositionAndRotation(interactionPose.position, interactionPose.rotation);
                    }
                    else
                    {
                        // If we don't have a joint pose, reset to whatever our parent XRController's pose is.
                        transform.localPosition = Vector3.zero;
                        transform.localRotation = Quaternion.identity;
                    }

                    // Ensure that the attachTransform tightly follows the interactor's transform
                    attachTransform.SetPositionAndRotation(transform.position, transform.rotation);
                }
            }
        }

        #endregion XRBaseInputInteractor
    }
}
