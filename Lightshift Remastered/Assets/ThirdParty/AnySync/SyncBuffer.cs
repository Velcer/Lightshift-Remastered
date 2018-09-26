using System;
using System.Collections.Generic;
using UnityEngine;

public class SyncBuffer : MonoBehaviour
{
    #region Inspector
    /// <summary>
    /// It's basically a keyframe buffer length defined in seconds.
    /// Values above send interval increase resistance to bad network conditions.
    /// Values below send interval compensate the latency by using extrapolation.
    /// </summary>
    [Range(0.05f, 0.5f)]
    public float TargetLatency = 0.075f;

    /// <summary>
    /// Smoothly lerps extrapolation errors back into right place.
    /// Increase TargetLatency if you want to reduce extrapolation errors or avoid them altogether.
    /// </summary>
    [NonSerialized]
    public float ErrorCorrectionSpeed = 10f;

    /// <summary>
    /// Smoothly lerps time drift back into correct timeframe.
    /// High values make movement look laggy if multiple packets often arrive together at the same time.
    /// Time drift will also occur on packet loss if no fake intermediate keyframes are created.
    /// </summary>
    [NonSerialized]
    public float TimeCorrectionSpeed = 3f;

    /// <summary>
    /// Sometimes extrapolation may not be desired at all because of overshooting.
    /// Use high enough TargetLatency, otherwise the object will lerp positions based on TimeCorrectionSpeed.
    /// </summary>
    [NonSerialized]
    public bool DisableExtrapolation;
    #endregion // Inspector


    protected float _playbackTime;
    protected float _timeDrift;
    protected List<Keyframe> _keyframes = new List<Keyframe>();
    public struct Keyframe
    {
        /// <summary>
        /// Time since last keyframe.
        /// </summary>
        public float InterpolationTime;

        public Vector3 Position;
        /// <summary>
        /// Optional for extrapolation, calculated from position difference by default.
        /// </summary>
        public Vector3 Velocity;
        /// <summary>
        /// Optional for extrapolation, 0 by default.
        /// </summary>
        public Vector3 Acceleration;

        public Quaternion Rotation;
        /// <summary>
        /// Optional for extrapolation, calculated from rotation difference by default.
        /// </summary>
        public Vector3 AngularVelocity;
        /// <summary>
        /// Optional for extrapolation, 0 by default.
        /// </summary>
        public Vector3 AngularAcceleration;
    }

    /// <summary>
    /// Used to identify if it's currently playing back through keyframes or predicting the future.
    /// </summary>
    public virtual bool IsExtrapolating
    {
        get { return _keyframes.Count == 1; }
    }

    /// <summary>
    /// Used to smooth out extrapolation error correction. You can force it to fully correct by setting this to 0.
    /// </summary>
    [NonSerialized]
    public Vector3 ExtrapolationPositionDrift;
    /// <summary>
    /// Used to smooth out extrapolation error correction. You can force it to fully correct by setting this to 0.
    /// </summary>
    [NonSerialized]
    public Quaternion ExtrapolationRotationDrift;

    /// <summary>
    /// Current interpolated or extrapolated position.
    /// </summary>
    public virtual Vector3 Position
    {
        get
        {
            return PositionNoErrorCorrection + ExtrapolationPositionDrift;
        }
    }

    /// <summary>
    /// Same as Position, but doesn't smoothly correct extrapolation drift when new keyframe arrives.
    /// </summary>
    public virtual Vector3 PositionNoErrorCorrection
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access position in an empty buffer. Zero vector returned.");
                return Vector3.zero;
            }

            if (DisableExtrapolation && IsExtrapolating)
                return _keyframes[0].Position;

            // using physics formula "delta x = v0 * t + 0.5 * a * t * t"
            // https://www.khanacademy.org/science/physics/one-dimensional-motion/kinematic-formulas/a/what-are-the-kinematic-formulas
            return _keyframes[0].Position + _keyframes[0].Velocity * _playbackTime + 0.5f * _keyframes[0].Acceleration * _playbackTime * _playbackTime;
        }
    }

    /// <summary>
    /// Current velocity. Properly affected by acceleration.
    /// </summary>
    public virtual Vector3 Velocity
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access velocity in an empty buffer. Zero vector returned.");
                return Vector3.zero;
            }

            return _keyframes[0].Velocity + _keyframes[0].Acceleration * _playbackTime;
        }
    }

    /// <summary>
    /// Interpolated or extrapolated rotation.
    /// </summary>
    public virtual Quaternion Rotation
    {
        get
        {
            return RotationNoErrorCorrection * ExtrapolationRotationDrift;
        }
    }

    /// <summary>
    /// Same as Rotation, but doesn't smoothly correct extrapolation drift when new keyframe arrives.
    /// </summary>
    public virtual Quaternion RotationNoErrorCorrection
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access rotation in an empty buffer. Zero rotation returned.");
                return Quaternion.identity;
            }

            if (DisableExtrapolation && IsExtrapolating)
                return _keyframes[0].Rotation;
            
            return _keyframes[0].Rotation * Quaternion.Euler(_keyframes[0].AngularVelocity * _playbackTime + 0.5f * _keyframes[0].AngularAcceleration * _playbackTime * _playbackTime);
        }
    }

    /// <summary>
    /// Current angular velocity. Properly affected by acceleration.
    /// </summary>
    public virtual Vector3 AngularVelocity
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access angular velocity in an empty buffer. Zero vector returned.");
                return Vector3.zero;
            }

            return _keyframes[0].AngularVelocity + _keyframes[0].AngularAcceleration * _playbackTime;
        }
    }

    /// <summary>
    /// Indicates if the buffer ever received any keyframes.
    /// </summary>
    public virtual bool HasKeyframes
    {
        get { return _keyframes.Count != 0; }
    }

    /// <summary>
    /// Last received keyframe used for correcting extrapolation overshooting or debugging.
    /// </summary>
    public virtual Keyframe LastReceivedKeyframe
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access LastReceivedKeyframe in an empty buffer. Blank keyframe returned.");
                return new Keyframe();
            }
            return _keyframes[_keyframes.Count - 1];
        }
    }

    /// <summary>
    /// Keyframe you're currently interpolating to. For debugging purposes.
    /// </summary>
    public virtual Keyframe NextKeyframe
    {
        get
        {
            if (_keyframes.Count < 2)
            {
                Debug.LogWarning("Trying to access NextKeyframe in a buffer that is empty or currently extrapolating. Blank keyframe returned.");
                return new Keyframe();
            }
            return _keyframes[1];
        }
    }

    /// <summary>
    /// Keyframe you're currently interpolating from. For debugging purposes.
    /// </summary>
    public virtual Keyframe CurrentKeyframe
    {
        get
        {
            if (!HasKeyframes)
            {
                Debug.LogWarning("Trying to access CurrentKeyframe in an empty buffer. Blank keyframe returned.");
                return new Keyframe();
            }
            return _keyframes[0];
        }
    }

    /// <summary>
    /// Use this to add a new keyframe to buffer.
    /// </summary>
    /// <param name="interpolationTime">Time it would take to get from last keyframe to this new keyframe</param>
    /// <param name="position"></param>
    /// <param name="rotation">Not required if you don't plan to sync rotations</param>
    /// <param name="velocity">Optional, lets you skip keyframes and improves extrapolation results</param>
    /// <param name="angularVelocity">Optional, lets you skip keyframes and improves extrapolation results</param>
    /// <param name="acceleration">Optional, improves extrapolation results</param>
    /// <param name="angularAcceleration">Optional, improves extrapolation results</param>
    public virtual void AddKeyframe(float interpolationTime, Vector3 position, Quaternion rotation = default(Quaternion), Vector3? velocity = null, Vector3? angularVelocity = null, Vector3 acceleration = default(Vector3), Vector3 angularAcceleration = default(Vector3))
    {
        // prevent long first frame if some keyframes was skipped before the first frame
        if (_keyframes.Count < 1)
            interpolationTime = Mathf.Max(TargetLatency, 0.01f);

        // CALCULATE TIME DRIFT
        var timeTillNewKeyframe = interpolationTime - _playbackTime;
        // add keyframes from the unplayed buffer
        for (var keyframeIterator = 1; keyframeIterator < _keyframes.Count; keyframeIterator++)
            timeTillNewKeyframe += _keyframes[keyframeIterator].InterpolationTime;

        _timeDrift = TargetLatency - timeTillNewKeyframe;

        // INSERTING EMPTY FAKE KEYFRAME IF NOTHING IS IN BUFFER (LastReceivedKeyframe doesn't exist)
        if (_keyframes.Count < 1)
        {
            var fakeKeyframe = new Keyframe
            {
                InterpolationTime = 0f,
                Position = position,
                Velocity = Vector3.zero,
                Acceleration = Vector3.zero,

                Rotation = rotation,
                AngularVelocity = Vector3.zero,
                AngularAcceleration = Vector3.zero
            };
            _keyframes.Add(fakeKeyframe);
        }

        var positionBeforeNewKeyframe = Position;
        var rotationBeforeNewKeyframe = Rotation;
        
        // ADD THE ACTUAL KEYFRAME
        var lastReceivedKeyframe = LastReceivedKeyframe;
        var calculatedVelocity = interpolationTime > 0f ? (position - lastReceivedKeyframe.Position) / interpolationTime : Vector3.zero;
        var calculatedRotationDifference = GetRotationDifference(lastReceivedKeyframe.Rotation, rotation);
        var calculatedAngularVelocity = interpolationTime > 0f ? FormatEulerRotation180(calculatedRotationDifference.eulerAngles) / interpolationTime : Vector3.zero;
        
        var keyframe = new Keyframe
        {
            InterpolationTime = interpolationTime,
            Position = position,
            Velocity = velocity ?? calculatedVelocity,
            Acceleration = acceleration,

            Rotation = rotation,
            AngularVelocity = angularVelocity ?? calculatedAngularVelocity,
            AngularAcceleration = angularAcceleration
        };
        _keyframes.Add(keyframe);

        // SET PREVIOUS KEYFRAME VELOCITY TO MATCH ARRIVED POSITION
        lastReceivedKeyframe.Velocity = calculatedVelocity;
        lastReceivedKeyframe.AngularVelocity = calculatedAngularVelocity;
        lastReceivedKeyframe.Acceleration = Vector3.zero;
        lastReceivedKeyframe.AngularAcceleration = Vector3.zero;
        _keyframes[_keyframes.Count - 2] = lastReceivedKeyframe;

        // get onto a new frame if needed
        UpdatePlayback(0f);

        // CALCULATE DRIFT
        var positionAfterNewKeyframe = PositionNoErrorCorrection;
        var rotationAfterNewKeyframe = RotationNoErrorCorrection;

        ExtrapolationPositionDrift = positionBeforeNewKeyframe - positionAfterNewKeyframe;
        ExtrapolationRotationDrift = GetRotationDifference(rotationAfterNewKeyframe, rotationBeforeNewKeyframe);
    }

    /// <summary>
    /// Progress the playback further by deltaTime.
    /// </summary>
    /// <param name="deltaTime">Time in seconds.</param>
    public virtual void UpdatePlayback(float deltaTime)
    {
        // if haven't yet received first keyframe
        if (_keyframes.Count < 1)
        {
            Debug.LogWarning("Trying to update playback in an empty buffer.");
            return;
        }

        // skip calculations if no delta time
        if (deltaTime > 0f)
        {
            // CORRECT TIME DRIFT
            _playbackTime += deltaTime;
            var timeDriftCorrection = -Mathf.Lerp(0f, _timeDrift, TimeCorrectionSpeed * deltaTime);
            _playbackTime += timeDriftCorrection;
            _timeDrift += timeDriftCorrection;

            // CORRECT EXTRAPOLATION DRIFT
            ExtrapolationPositionDrift = Vector3.Lerp(ExtrapolationPositionDrift, Vector3.zero, ErrorCorrectionSpeed * deltaTime);
            ExtrapolationRotationDrift = Quaternion.Lerp(ExtrapolationRotationDrift, Quaternion.identity, ErrorCorrectionSpeed * deltaTime);
        }

        // REMOVE OLD KEYFRAMES
        while (_keyframes.Count > 1 && _playbackTime >= _keyframes[1].InterpolationTime)
        {
            // if you're going through an instant keyframe, nullify drifting, since this is most likely a teleport
            if (_keyframes[1].InterpolationTime == 0f)
            {
                ExtrapolationPositionDrift = Vector3.zero;
                ExtrapolationRotationDrift = Quaternion.identity;
            }

            _playbackTime -= _keyframes[1].InterpolationTime;
            _keyframes.RemoveAt(0);
        }
    }

    /// <summary>
    /// Gets the rotation difference between two quaternions.
    /// Basically output is "toRotation - fromRotation", but in quaternion math.
    /// </summary>
    /// <param name="fromRotation"></param>
    /// <param name="toRotation"></param>
    /// <returns></returns>
    protected Quaternion GetRotationDifference(Quaternion fromRotation, Quaternion toRotation)
    {
        return Quaternion.Inverse(fromRotation) * toRotation;
    }

    /// <summary>
    /// Converts rotation values from 0 to 360 range into -180 to 180 range
    /// </summary>
    /// <param name="eulerRotation"></param>
    /// <returns></returns>
    protected Vector3 FormatEulerRotation180(Vector3 eulerRotation)
    {
        while (eulerRotation.x > 180f)
            eulerRotation.x -= 360f;
        while (eulerRotation.y > 180f)
            eulerRotation.y -= 360f;
        while (eulerRotation.z > 180f)
            eulerRotation.z -= 360f;
        
        while (eulerRotation.x <= -180f)
            eulerRotation.x += 360f;
        while (eulerRotation.y <= -180f)
            eulerRotation.y += 360f;
        while (eulerRotation.z <= -180f)
            eulerRotation.z += 360f;

        return eulerRotation;
    }
}
