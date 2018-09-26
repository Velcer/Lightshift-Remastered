using UnityEngine;
using UnityEngine.Networking;

namespace AnySync.Examples
{
    /// <summary>
    /// Minimal example using the easiest possible techniques.
    /// Some of the best practices are ignored in favor of shorter code.
    /// </summary>
    public class UNetPlayerSync2D : NetworkBehaviour
    {
        private const float MinimumSendInterval = 0.05f; // up to 20 messages per second

        private const float MovementSpeed = 5f;
        private const float JumpVelocity = 8f;


        private float _timeSinceLastSync;
        private Vector3 _lastSentPosition;
        private bool _idle;
        private void Update()
        {
            if (isLocalPlayer)
            {
                var rigidbody2DComponent = GetComponent<Rigidbody2D>();

                // get original velocity
                var rigidbody2DVelocity = rigidbody2DComponent.velocity;

                // modify velocity according to input
                rigidbody2DVelocity.x = Input.GetAxisRaw("Horizontal") * MovementSpeed;
                if (Input.GetButtonDown("Jump"))
                    rigidbody2DVelocity.y = JumpVelocity;

                // apply velocity change
                rigidbody2DComponent.velocity = rigidbody2DVelocity;

                // in the nutshell, this commented out code does the same thing as lines 47-65
                // but to stop player from sending messages while idle, some extra code is required
                // you can decide which one to use for your game
                //if (_timeSinceLastSync >= MinimumSendInterval)
                //{
                //    CmdSync(_timeSinceLastSync, rigidbody2DComponent.position);
                //    _timeSinceLastSync = 0f;
                //}

                // send sync messages
                _timeSinceLastSync += Time.deltaTime;
                if (_timeSinceLastSync >= MinimumSendInterval)
                {
                    if (_lastSentPosition != transform.position)
                        _idle = false;

                    if (!_idle)
                    {
                        CmdSync(_timeSinceLastSync, transform.position);
                        // go idle only after sending a keyframe with the same position to prevent drifting while idle
                        if (_lastSentPosition == transform.position)
                            _idle = true;

                        _lastSentPosition = transform.position;
                        _timeSinceLastSync = 0f;
                    }
                }
            }
            else
            {
                var syncBufferComponent = GetComponent<SyncBuffer>();
                // avoid warnings if syncBuffer is empty
                if (syncBufferComponent.HasKeyframes)
                {
                    // update playback and apply new position
                    syncBufferComponent.UpdatePlayback(Time.deltaTime);
                    transform.position = syncBufferComponent.Position;
                }
            }
        }

        [Command]
        private void CmdSync(float interpolationTime, Vector2 position)
        {
            // add keyframe to buffer on server
            GetComponent<SyncBuffer>().AddKeyframe(interpolationTime, position);
            // send it to other clients
            RpcSync(interpolationTime, position);
        }

        [ClientRpc]
        private void RpcSync(float interpolationTime, Vector2 position)
        {
            // prevent receiving keyframes on owner client and host
            if (isLocalPlayer || isServer)
                return;

            // add keyframe to buffer on clients
            GetComponent<SyncBuffer>().AddKeyframe(interpolationTime, position);
        }
    }
}