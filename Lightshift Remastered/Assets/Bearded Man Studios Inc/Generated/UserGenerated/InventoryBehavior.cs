using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"int\", \"string\", \"int\", \"byte[]\"][\"string\", \"int\", \"byte[]\"][\"int\"][\"int\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"slotId\", \"itemKey\", \"amount\", \"itemAttributeObject\"][\"itemKey\", \"amount\", \"attributes\"][\"slotId\"][\"slotId\"]]")]
	public abstract partial class InventoryBehavior : NetworkBehavior
	{
		public const byte RPC_SET_SLOT_ITEM = 0 + 5;
		public const byte RPC_SET_HELD_ITEM = 1 + 5;
		public const byte RPC_SLOT_RIGHT_CLICKED = 2 + 5;
		public const byte RPC_SLOT_LEFT_CLICKED = 3 + 5;
		
		public InventoryNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (InventoryNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("SetSlotItem", SetSlotItem, typeof(int), typeof(string), typeof(int), typeof(byte[]));
			networkObject.RegisterRpc("SetHeldItem", SetHeldItem, typeof(string), typeof(int), typeof(byte[]));
			networkObject.RegisterRpc("SlotRightClicked", SlotRightClicked, typeof(int));
			networkObject.RegisterRpc("SlotLeftClicked", SlotLeftClicked, typeof(int));

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
			Initialize(new InventoryNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new InventoryNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// int slotId
		/// string itemKey
		/// int amount
		/// byte[] itemAttributeObject
		/// </summary>
		public abstract void SetSlotItem(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string itemKey
		/// int amount
		/// byte[] attributes
		/// </summary>
		public abstract void SetHeldItem(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int slotId
		/// </summary>
		public abstract void SlotRightClicked(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int slotId
		/// </summary>
		public abstract void SlotLeftClicked(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}