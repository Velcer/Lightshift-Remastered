using UnityEngine;
using UnityEngine.Networking;

namespace AnySync.Examples
{
    public class UNetPlayerSync : NetworkBehaviour
    {
        private const float SendInterval = 0.1f; // 10 messages per second
        private const float MovementAcceleration = 13f;

        private Vector2 _input;

        private Rigidbody _rigidbody;
        private SyncBuffer _syncBuffer;
        private Transform _transform;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _syncBuffer = GetComponent<SyncBuffer>();
            _transform = transform;
        }

        private void Update()
        {
            if (!hasAuthority)
            {
                if (_syncBuffer.HasKeyframes)
                {
                    _syncBuffer.UpdatePlayback(Time.deltaTime);
                    _transform.position = _syncBuffer.Position;
                    _transform.rotation = _syncBuffer.Rotation;
                }
            }
            else
            {
                // movement
                _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                // teleportation
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // send a keyframe to interpolate to before teleporting
                    SendSyncMessage();

                    var newPosition = new Vector3(Random.Range(-6f, 6f), 2f, Random.Range(-6f, 6f));
                    _rigidbody.position = newPosition;
                    _rigidbody.rotation = Quaternion.identity;
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;

                    // and then immediately send a teleportation keyframe with 0 interpolation time
                    SendSyncMessage();
                }
            }
        }

        private float _timeSinceLastSync;
        private Vector3 _lastSentVelocity;
        private Vector3 _lastSentPosition;
        private void FixedUpdate()
        {
            if (hasAuthority)
            {
                // control the rigidbody locally
                _rigidbody.AddForce(new Vector3(-_input.x, 0f, -_input.y) * MovementAcceleration * Time.deltaTime, ForceMode.VelocityChange);

                _timeSinceLastSync += Time.deltaTime;

                if (_rigidbody.velocity != _lastSentVelocity || _rigidbody.position != _lastSentPosition)
                {
                    if (Mathf.Approximately(_timeSinceLastSync, SendInterval) || _timeSinceLastSync > SendInterval)
                    {
                        // send rigidbody data from client to server on change
                        SendSyncMessage();
                    }
                }
            }
        }

        private void SendSyncMessage()
        {
            CmdSync(_timeSinceLastSync, _rigidbody.position, _rigidbody.rotation, _rigidbody.velocity);
            _lastSentVelocity = _rigidbody.velocity;
            _lastSentPosition = _rigidbody.position;
            _timeSinceLastSync = 0f;
        }

        [Command]
        private void CmdSync(float interpolationTime, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            // add keyframe to buffer
            _syncBuffer.AddKeyframe(interpolationTime, position, rotation, velocity);
            // send it to other clients
            RpcSync(interpolationTime, position, rotation, velocity);
        }

        [ClientRpc]
        private void RpcSync(float interpolationTime, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            // prevent receiving keyframes on owner client and host
            if (isServer)
                return;

            // add keyframe to buffer
            _syncBuffer.AddKeyframe(interpolationTime, position, rotation, velocity);
        }
    }
}
