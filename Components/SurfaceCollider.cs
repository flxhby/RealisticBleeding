using ThunderRoad;
using UnityEngine;

namespace RealisticBleeding.Components
{
	public struct SurfaceCollider
	{
		public Collider Collider;
		public RagdollPart RagdollPart;
		public Vector3 LastNormal;
		public float DistanceTravelled;

		public SurfaceCollider(Collider collider, Vector3 lastNormal)
		{
			Collider = collider;
			RagdollPart = ResolveRagdollPart(collider);
			LastNormal = lastNormal;

			DistanceTravelled = Random.Range(-100f, 100f);
		}

		public static RagdollPart ResolveRagdollPart(Collider collider)
		{
			if (collider == null) return null;

			var rb = collider.attachedRigidbody;
			if (rb == null) return null;

			return rb.GetComponent<RagdollPart>();
		}
	}
}