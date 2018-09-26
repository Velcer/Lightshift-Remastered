using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"string\"][\"int\", \"string\", \"float\", \"float\", \"float\", \"float\", \"float\", \"bool\"][\"float\"][\"float\"][\"float\"][\"float\"][\"float\"][\"float\"][\"float\"][\"int\"][\"int\"][\"string\"][\"float\", \"Vector3\", \"Quaternion\", \"Vector3\"][]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"value\"][\"id\", \"key\", \"speed\", \"acceleration\", \"agility\", \"health\", \"shield\", \"flipped\"][\"value\"][\"value\"][\"value\"][\"value\"][\"value\"][\"value\"][\"value\"][\"value\"][\"value\"][\"value\"][\"interpolationTime\", \"position\", \"rotation\", \"velocity\"][]]")]
	public abstract partial class EntityBehavior : NetworkBehavior
	{
		public const byte RPC_UPDATE_TEAM = 0 + 5;
		public const byte RPC_UPDATE_MODULE = 1 + 5;
		public const byte RPC_UPDATE_HEALTH = 2 + 5;
		public const byte RPC_UPDATE_SHIELD = 3 + 5;
		public const byte RPC_UPDATE_MAX_HEALTH = 4 + 5;
		public const byte RPC_UPDATE_MAX_SHIELD = 5 + 5;
		public const byte RPC_UPDATE_ACCELERATION = 6 + 5;
		public const byte RPC_UPDATE_SPEED = 7 + 5;
		public const byte RPC_UPDATE_AGILITY = 8 + 5;
		public const byte RPC_UPDATE_VERTICAL_INPUT = 9 + 5;
		public const byte RPC_UPDATE_HORIZONTAL_INPUT = 10 + 5;
		public const byte RPC_UPDATE_NAME = 11 + 5;
		public const byte RPC_SYNC = 12 + 5;
		public const byte RPC_INIT = 13 + 5;
		
		public EntityNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (EntityNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("UpdateTeam", UpdateTeam, typeof(string));
			networkObject.RegisterRpc("UpdateModule", UpdateModule, typeof(int), typeof(string), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(bool));
			networkObject.RegisterRpc("UpdateHealth", UpdateHealth, typeof(float));
			networkObject.RegisterRpc("UpdateShield", UpdateShield, typeof(float));
			networkObject.RegisterRpc("UpdateMaxHealth", UpdateMaxHealth, typeof(float));
			networkObject.RegisterRpc("UpdateMaxShield", UpdateMaxShield, typeof(float));
			networkObject.RegisterRpc("UpdateAcceleration", UpdateAcceleration, typeof(float));
			networkObject.RegisterRpc("UpdateSpeed", UpdateSpeed, typeof(float));
			networkObject.RegisterRpc("UpdateAgility", UpdateAgility, typeof(float));
			networkObject.RegisterRpc("UpdateVerticalInput", UpdateVerticalInput, typeof(int));
			networkObject.RegisterRpc("UpdateHorizontalInput", UpdateHorizontalInput, typeof(int));
			networkObject.RegisterRpc("UpdateName", UpdateName, typeof(string));
			networkObject.RegisterRpc("Sync", Sync, typeof(float), typeof(Vector3), typeof(Quaternion), typeof(Vector3));
			networkObject.RegisterRpc("Init", Init);

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new EntityNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new EntityNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// string value
		/// </summary>
		public abstract void UpdateTeam(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int id
		/// string key
		/// float speed
		/// float acceleration
		/// float agility
		/// float health
		/// float shield
		/// bool flipped
		/// </summary>
		public abstract void UpdateModule(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateHealth(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateShield(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateMaxHealth(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateMaxShield(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateAcceleration(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateSpeed(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float value
		/// </summary>
		public abstract void UpdateAgility(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int value
		/// </summary>
		public abstract void UpdateVerticalInput(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int value
		/// </summary>
		public abstract void UpdateHorizontalInput(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string value
		/// </summary>
		public abstract void UpdateName(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// float interpolationTime
		/// Vector3 position
		/// Quaternion rotation
		/// Vector3 velocity
		/// </summary>
		public abstract void Sync(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void Init(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}