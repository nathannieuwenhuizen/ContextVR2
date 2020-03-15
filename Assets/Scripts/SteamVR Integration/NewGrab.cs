//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Basic throwable object
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Rigidbody))]
    public class NewGrab : MonoBehaviour
    {
        [EnumFlags]
        [Tooltip("The flags used to attach this object to the hand.")]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
        public Transform attachmentOffset;

        [Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)")]
        public float catchingSpeedThreshold = -1;

        public ReleaseStyleA releaseVelocityStyle = ReleaseStyleA.GetFromHand;

        [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
        public float releaseVelocityTimeOffset = -0.011f;

        public float scaleReleaseVelocity = 1.1f;

        [Tooltip("The release velocity magnitude representing the end of the scale release velocity curve. (-1 to disable)")]
        public float scaleReleaseVelocityThreshold = -1.0f;
        [Tooltip("Use this curve to ease into the scaled release velocity based on the magnitude of the measured release velocity. This allows greater differentiation between a drop, toss, and throw.")]
        public AnimationCurve scaleReleaseVelocityCurve = AnimationCurve.EaseInOut(0.0f, 0.1f, 1.0f, 1.0f);

        [Tooltip("When detaching the object, should it return to its original parent?")]
        public bool restoreOriginalParent = false;



        protected VelocityEstimator velocityEstimator;
        protected bool attached = false;
        protected float attachTime;
        protected Vector3 attachPosition;
        protected Quaternion attachRotation;
        protected Transform attachEaseInTransform;

        public UnityEvent onPickUp;
        public UnityEvent onDetachFromHand;
        public HandEvent onHeldUpdate;


        protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        protected new Rigidbody rigidbody;

        [HideInInspector]
        public Interactable interactable;


        //Grabber
        [SerializeField] private Grabber otherHand;
        [SerializeField] float GrabberRange = .1f;
        [SerializeField] private bool grabbed = false;
        public GameObject grabbedObject;
        private GameObject scalingObject;
        private float scaleMultip;
        private float startDistance;

        private GameObject hoverObject;

        [SerializeField] private float scaleMin = 0.01f;
        [SerializeField] private float scaleMax = .5f;

        //-------------------------------------------------
        protected virtual void Awake()
        {
            velocityEstimator = GetComponent<VelocityEstimator>();
            interactable = GetComponent<Interactable>();



            rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = 50.0f;


            if (attachmentOffset != null)
            {
                // remove?
                //interactable.handFollowTransform = attachmentOffset;
            }

        }


        //-------------------------------------------------
        protected virtual void OnHandHoverBegin(Hand hand)
        {
            bool showHint = false;

            // "Catch" the throwable by holding down the interaction button instead of pressing it.
            // Only do this if the throwable is moving faster than the prescribed threshold speed,
            // and if it isn't attached to another hand
            if (!attached && catchingSpeedThreshold != -1)
            {
                float catchingThreshold = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);

                GrabTypes bestGrabType = hand.GetBestGrabbingType();

                if (bestGrabType != GrabTypes.None)
                {
                    if (rigidbody.velocity.magnitude >= catchingThreshold)
                    {
                        hand.AttachObject(gameObject, bestGrabType, attachmentFlags);
                        showHint = false;
                    }
                }
            }

            if (showHint)
            {
                hand.ShowGrabHint();
            }
        }


        //-------------------------------------------------
        protected virtual void OnHandHoverEnd(Hand hand)
        {
            hand.HideGrabHint();
        }


        //-------------------------------------------------
        protected virtual void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
                hand.HideGrabHint();
            }
        }

        //-------------------------------------------------
        protected virtual void OnAttachedToHand(Hand hand)
        {
            GameObject focusedObject = this.gameObject;
            //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + hand.GetGrabStarting().ToString());

            //check if spawnerobject
            if (focusedObject.GetComponent<NewSpawner>()) focusedObject = focusedObject.GetComponent<NewSpawner>().spawnObject();

            //apply variable
            grabbedObject = focusedObject;
            Grabbed = true;

            //Attach to Hand
            hadInterpolation = this.rigidbody.interpolation;

            attached = true;

            onPickUp.Invoke();

            hand.HoverLock(null);

            rigidbody.interpolation = RigidbodyInterpolation.None;

            if (velocityEstimator != null)
                velocityEstimator.BeginEstimatingVelocity();

            attachTime = Time.time;
            attachPosition = transform.position;
            attachRotation = transform.rotation;

            //add haircomponent
            if (grabbedObject.GetComponent<HairObject>() == null)
            {
                grabbedObject.AddComponent<HairObject>();
            }

            //lock rigidbody but still exist for collission detection with head and hair
            grabbedObject.GetComponent<HairObject>().ToggleRigidBody(true, true);

            grabbedObject.GetComponent<HairObject>().Grabbed = true;
            grabbedObject.GetComponent<HairObject>().Hover = true;

            //update slider
            HSVColorPanel.instance.SelectedObject = grabbedObject;
        }


        //-------------------------------------------------
        protected virtual void OnDetachedFromHand(Hand hand)
        {
            if (scalingObject != null)
            {
                scalingObject = null;
                return;
            }
            if (!Grabbed) { return; }
            Grabbed = false;

            grabbedObject.transform.parent = null;

            //update color panel
            HSVColorPanel.instance.SelectedObject = null;

            attached = false;

            onDetachFromHand.Invoke();

            hand.HoverUnlock(null);

            rigidbody.interpolation = hadInterpolation;

            //if collides with head, attach it, otherwise fall on ground
            if (grabbedObject.GetComponent<HairObject>().AttachedAtHead)
            {
                grabbedObject.GetComponent<HairObject>().Grabbed = false;
                grabbedObject.GetComponent<HairObject>().ToggleRigidBody(false);

                grabbedObject.transform.parent = grabbedObject.GetComponent<HairObject>().ParentTransform;
            }
            else
            {
                Vector3 velocity;
                Vector3 angularVelocity;

                GetReleaseVelocities(hand, out velocity, out angularVelocity);

                rigidbody.velocity = velocity;
                rigidbody.angularVelocity = angularVelocity;
            }
        }


        public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyleA.NoChange)
                releaseVelocityStyle = ReleaseStyleA.ShortEstimation; // only type that works with fallback hand is short estimation.

            switch (releaseVelocityStyle)
            {
                case ReleaseStyleA.ShortEstimation:
                    if (velocityEstimator != null)
                    {
                        velocityEstimator.FinishEstimatingVelocity();
                        velocity = velocityEstimator.GetVelocityEstimate();
                        angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                    }
                    else
                    {
                        Debug.LogWarning("[SteamVR Interaction System] Throwable: No Velocity Estimator component on object but release style set to short estimation. Please add one or change the release style.");

                        velocity = rigidbody.velocity;
                        angularVelocity = rigidbody.angularVelocity;
                    }
                    break;
                case ReleaseStyleA.AdvancedEstimation:
                    hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                    break;
                case ReleaseStyleA.GetFromHand:
                    velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                    angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                    break;
                default:
                case ReleaseStyleA.NoChange:
                    velocity = rigidbody.velocity;
                    angularVelocity = rigidbody.angularVelocity;
                    break;
            }

            if (releaseVelocityStyle != ReleaseStyleA.NoChange)
            {
                float scaleFactor = 1.0f;
                if (scaleReleaseVelocityThreshold > 0)
                {
                    scaleFactor = Mathf.Clamp01(scaleReleaseVelocityCurve.Evaluate(velocity.magnitude / scaleReleaseVelocityThreshold));
                }

                velocity *= (scaleFactor * scaleReleaseVelocity);
            }
        }

        //-------------------------------------------------
        protected virtual void HandAttachedUpdate(Hand hand)
        {


            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject, restoreOriginalParent);

                // Uncomment to detach ourselves late in the frame.
                // This is so that any vehicles the player is attached to
                // have a chance to finish updating themselves.
                // If we detach now, our position could be behind what it
                // will be at the end of the frame, and the object may appear
                // to teleport behind the hand when the player releases it.
                //StartCoroutine( LateDetach( hand ) );
            }
            if (scalingObject != null)
            {
                if (scalingObject.GetComponent<HairObject>().Grabbed)
                {
                    float scale = (Vector3.Distance(transform.position, otherHand.transform.position) / startDistance) * scaleMultip;
                    scale = Mathf.Clamp(scale, scaleMin, scaleMax);
                    scalingObject.transform.localScale = new Vector3(scale, scale, scale);
                }
            }

            if (onHeldUpdate != null)
                onHeldUpdate.Invoke(hand);
        }

        void scaleCheck()
        {
            if (otherHand.grabbedObject != null)
            {
                scalingObject = otherHand.grabbedObject;
                scaleMultip = scalingObject.transform.localScale.x;
                startDistance = Vector3.Distance(transform.position, otherHand.transform.position);
                return;
            }
        }


        //-------------------------------------------------
        protected virtual IEnumerator LateDetach(Hand hand)
        {
            yield return new WaitForEndOfFrame();

            hand.DetachObject(gameObject, restoreOriginalParent);
        }


        //-------------------------------------------------
        protected virtual void OnHandFocusAcquired(Hand hand)
        {
            gameObject.SetActive(true);

            if (velocityEstimator != null)
                velocityEstimator.BeginEstimatingVelocity();
        }


        //-------------------------------------------------
        protected virtual void OnHandFocusLost(Hand hand)
        {
            gameObject.SetActive(false);

            if (velocityEstimator != null)
                velocityEstimator.FinishEstimatingVelocity();
        }

        public bool Grabbed
        {
            get { return grabbed; }
            set
            {
                grabbed = value;
            }
        }
    }

    public enum ReleaseStyleA
    {
        NoChange,
        GetFromHand,
        ShortEstimation,
        AdvancedEstimation,
    }


}
