using ThunderRoad;
using UnityEngine;

namespace RealisticBleeding.Systems
{
	public class FallingBloodDropRenderingSystem : BaseSystem
	{
		private const int MaxInstancesPerBatch = 1023;

		private static readonly Matrix4x4[] InstanceMatrices = new Matrix4x4[MaxInstancesPerBatch];

		private readonly FastList<FallingBloodDrop> _fallingBloodDrops;
		private readonly Mesh _mesh;

		private Material _material;

		private bool _firstFrame = true;

		public FallingBloodDropRenderingSystem(FastList<FallingBloodDrop> fallingBloodDrops, Mesh mesh)
		{
			_fallingBloodDrops = fallingBloodDrops;
			_mesh = mesh;
		}

		protected override void UpdateInternal(float deltaTime)
		{
			if (_firstFrame)
			{
				_firstFrame = false;

				Catalog.LoadAssetAsync("RealisticBleeding/BloodDrop",
					(Material material) =>
					{
						if (SystemInfo.supportsInstancing)
						{
							material.enableInstancing = true;
						}

						_material = material;
					}, null);
			}

			if (_material == null) return;

			if (!_material.enableInstancing)
			{
				for (var index = 0; index < _fallingBloodDrops.Count; index++)
				{
					Graphics.DrawMesh(_mesh, GetMatrix(ref _fallingBloodDrops[index]), _material, 0);
				}

				return;
			}

			var instanceCount = 0;

			for (var index = 0; index < _fallingBloodDrops.Count; index++)
			{
				InstanceMatrices[instanceCount++] = GetMatrix(ref _fallingBloodDrops[index]);

				if (instanceCount == MaxInstancesPerBatch)
				{
					Graphics.DrawMeshInstanced(_mesh, 0, _material, InstanceMatrices, instanceCount);

					instanceCount = 0;
				}
			}

			if (instanceCount > 0)
			{
				Graphics.DrawMeshInstanced(_mesh, 0, _material, InstanceMatrices, instanceCount);
			}
		}

		private static Matrix4x4 GetMatrix(ref FallingBloodDrop bloodDrop)
		{
			var magnitude = bloodDrop.Velocity.magnitude;
			var size = Mathf.Lerp(1, 3.5f, Mathf.InverseLerp(0, 4, magnitude));

			var rotation = magnitude > 0.01f
				? Quaternion.LookRotation(bloodDrop.Velocity / magnitude)
				: Quaternion.identity;

			return Matrix4x4.TRS(bloodDrop.Position, rotation, new Vector3(1, 1, size) * 0.007f);
		}
	}
}
