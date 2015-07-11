using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerCameraOrbit))]
public class CameraOrbitCollision : MonoBehaviour {

    private PlayerCameraOrbit cameraOrbit;
    private Transform target;

    private Material targetMaterial;

	void Start() {
        cameraOrbit = GetComponent<PlayerCameraOrbit>();
        target = cameraOrbit.target;

        Renderer targetRenderer = target.GetComponent<Renderer>();
        targetMaterial = targetRenderer.material;
	}
	
	void LateUpdate() {
        float baseDistance = cameraOrbit.distance;
        float raycastDistance = getDistanceForRaycastHit(baseDistance);
        cameraOrbit.setOrbitDistance(raycastDistance);
        setTransparencyTarget(raycastDistance);

        Debug.Log("CameraOrbitCollision");
	}

    private float getDistanceForRaycastHit(float distance) {
        Vector3 targetStart = target.position;
        Vector3 cameraEnd = transform.position;
        RaycastHit hit;
        bool interact = Physics.Linecast(targetStart, cameraEnd, out hit);
        if (interact) {
            Debug.DrawLine(target.position, hit.point);
            return hit.distance - 0.5f;
        }
        return distance;
    }

    private void setTransparencyTarget(float distance) {
        float limitDistance = cameraOrbit.distanceMinLimit;
        Color color = targetMaterial.color;
        if (distance < limitDistance)
            color.a = distance / limitDistance;
        else
            color.a = 1.0f;
        targetMaterial.color = color;
    }
}
