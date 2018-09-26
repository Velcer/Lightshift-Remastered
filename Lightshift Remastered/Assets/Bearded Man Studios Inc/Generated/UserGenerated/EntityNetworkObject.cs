using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0]")]
	public partial class EntityNetworkObject : NetworkObject
	{
		public const int IDENTITY = 3;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private int _horizontalInput;
		public event FieldEvent<int> horizontalInputChanged;
		public Interpolated<int> horizontalInputInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int horizontalInput
		{
			get { return _horizontalInput; }
			set
			{
				// Don't do anything if the value is the same
				if (_horizontalInput == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_horizontalInput = value;
				hasDirtyFields = true;
			}
		}

		public void SethorizontalInputDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_horizontalInput(ulong timestep)
		{
			if (horizontalInputChanged != null) horizontalInputChanged(_horizontalInput, timestep);
			if (fieldAltered != null) fieldAltered("horizontalInput", _horizontalInput, timestep);
		}
		private int _verticalInput;
		public event FieldEvent<int> verticalInputChanged;
		public Interpolated<int> verticalInputInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int verticalInput
		{
			get { return _verticalInput; }
			set
			{
				// Don't do anything if the value is the same
				if (_verticalInput == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_verticalInput = value;
				hasDirtyFields = true;
			}
		}

		public void SetverticalInputDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_verticalInput(ulong timestep)
		{
			if (verticalInputChanged != null) verticalInputChanged(_verticalInput, timestep);
			if (fieldAltered != null) fieldAltered("verticalInput", _verticalInput, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			horizontalInputInterpolation.current = horizontalInputInterpolation.target;
			verticalInputInterpolation.current = verticalInputInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _horizontalInput);
			UnityObjectMapper.Instance.MapBytes(data, _verticalInput);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_horizontalInput = UnityObjectMapper.Instance.Map<int>(payload);
			horizontalInputInterpolation.current = _horizontalInput;
			horizontalInputInterpolation.target = _horizontalInput;
			RunChange_horizontalInput(timestep);
			_verticalInput = UnityObjectMapper.Instance.Map<int>(payload);
			verticalInputInterpolation.current = _verticalInput;
			verticalInputInterpolation.target = _verticalInput;
			RunChange_verticalInput(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _horizontalInput);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _verticalInput);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (horizontalInputInterpolation.Enabled)
				{
					horizontalInputInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					horizontalInputInterpolation.Timestep = timestep;
				}
				else
				{
					_horizontalInput = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_horizontalInput(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (verticalInputInterpolation.Enabled)
				{
					verticalInputInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					verticalInputInterpolation.Timestep = timestep;
				}
				else
				{
					_verticalInput = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_verticalInput(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (horizontalInputInterpolation.Enabled && !horizontalInputInterpolation.current.UnityNear(horizontalInputInterpolation.target, 0.0015f))
			{
				_horizontalInput = (int)horizontalInputInterpolation.Interpolate();
				//RunChange_horizontalInput(horizontalInputInterpolation.Timestep);
			}
			if (verticalInputInterpolation.Enabled && !verticalInputInterpolation.current.UnityNear(verticalInputInterpolation.target, 0.0015f))
			{
				_verticalInput = (int)verticalInputInterpolation.Interpolate();
				//RunChange_verticalInput(verticalInputInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public EntityNetworkObject() : base() { Initialize(); }
		public EntityNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public EntityNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
