using UnityEngine;
using System.Collections;

public class PlayerCameraOrbit : MonoBehaviour {

    public Transform target;
    public float distance = 5.0f;
    public float speed = 120.0f;
    public float zoomSpeed = 5.0f;

    public Vector2 position = Vector2.zero;

    public float distanceMinLimit = 2.0f;
    public float distanceMaxLimit = 15.0f;

    public float slopMinLimit = 0.0f;
    public float slopMaxLimit = 90.0f;

    public bool invertMouseY = true;
    public bool invertMouseWheel = true;

    private Vector2 mouse;
    private CameraCollision cameraCollision;

    void Start() {
        cameraCollision = new CameraCollision(this);
        setOrbit(position, distance);
        cameraCollision.fixPosition();
    }

    void LateUpdate() {
        position = getCameraPosition();
        distance = getCameraDistance();
        setOrbit(position, distance);
        cameraCollision.fixPosition();
    }

    public void setOrbitDistance(float distance) {
        setOrbit(position, distance);
    }

    public void setOrbit(Vector2 position, float distance) {
        Quaternion rotation = Quaternion.Euler(position.y, position.x, 0);
        setCameraTransform(rotation, distance);
    }

    public void setCameraTransform(Quaternion rotation, float distance) {
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        
        transform.position = rotation * negDistance + target.position;
        transform.rotation = rotation;
    }

    private bool isPlayerMovingCamera() {
        return Input.GetMouseButton(0);
    }
    
    private Vector2 getCameraPosition() {
        Vector2 position = this.position;
        if (isPlayerMovingCamera()) {
            position = getPlayerCameraPosition();
            position = getNormalizedPosition(position);
        }
        return position;
    }

    private Vector2 getPlayerCameraPosition() {
        Vector2 position = this.position;
        position.x += getMovedX();
        position.y += getMovedY();
        return position;
    }
    
    private float getMovedX() {
        float mouse = Input.GetAxis("Mouse X");
        return mouse * speed;
    }

    private float getMovedY() {
        float mouse = Input.GetAxis("Mouse Y");
        float invertFactor = invertMouseY ? -1.0f : 1.0f;
        return mouse * speed * invertFactor;
    }

    private float getCameraDistance() {
        float distance = this.distance;
        distance += getDeltaZoom();
        distance = getLimitedDistance(distance);
        return distance;
    }
    
    private float getDeltaZoom() {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        float invertFactor = invertMouseWheel ? -1.0f : 1.0f;
        return mouseWheel * zoomSpeed * invertFactor;
    }

    private Vector2 getNormalizedPosition(Vector2 position) {
        position.y = getLimitedSlop(position.y);
        float x = position.x % 360.0f;
        float y = position.y;

        if (x < 0.0f) x += 360.0f;
        if (y < -180.0f) y += 360.0f;
        if (y > 180.0f)  y -= 360.0f;

        return new Vector2(x, y);
    }

    private float getLimitedSlop(float slop) {
        return Mathf.Clamp(slop, slopMinLimit, slopMaxLimit);
    }

    private float getLimitedDistance(float distance) {
        return Mathf.Clamp(distance, distanceMinLimit, distanceMaxLimit);
    }
}

class CameraCollision {

    public float collisionMargin = 0.2f;
    public float minTargetAlpha = 0.50f;

    private PlayerCameraOrbit cameraOrbit;

    private Transform camera;
    private Transform target;


    private Material targetMaterial;

    public CameraCollision(PlayerCameraOrbit cameraOrbit) {
        this.cameraOrbit = cameraOrbit;
        this.camera = cameraOrbit.transform;
        this.target = cameraOrbit.target;
        
        Renderer targetRenderer = target.GetComponent<Renderer>();
        this.targetMaterial = targetRenderer.material;
    }
    
    public void fixPosition() {
        float distance = getSafeDistance();
        cameraOrbit.setOrbitDistance(distance);
        setTransparentTarget(distance);
    }

    private float getSafeDistance() {
        float baseDistance = cameraOrbit.distance;
        float raycastDistance = getDistanceForRaycast(baseDistance);
        float safeDistance = raycastDistance - collisionMargin;
        return safeDistance;
    }
    
    private float getDistanceForRaycast(float distance) {
        RaycastHit hit;
        bool interact = raycastHitFromTargetToCamera(out hit);
        if (!interact)
            return distance; // Brak kolizji, domyślna odległość

        return hit.distance;
    }

    private bool raycastHitFromTargetToCamera(out RaycastHit hitOut) {
        Vector3 targetStart = target.position;
        Vector3 cameraEnd = camera.position;
        RaycastHit hit;
        int withoutPlayerMask = getLayerMaskWithoutPlayer();
        bool interact = Physics.Linecast(targetStart, cameraEnd, out hit, withoutPlayerMask);
        if (interact) {
            hitOut = hit;
            return true;
        } else {
            hitOut = default(RaycastHit);
            return false;
        }
    }

    private int getLayerMaskWithoutPlayer() {
        int playerLayer = LayerMask.NameToLayer("Player");
        int mask = ~(1 << playerLayer);
        return mask;
    }
    
    private void setTransparentTarget(float distance) {
        Color color = targetMaterial.color;
        color.a = getAlphaForDistance(distance);
        targetMaterial.color = color;
    }

    private float getAlphaForDistance(float distance) {
        float limitDistance = cameraOrbit.distanceMinLimit;
        if (distance > limitDistance) // Odległość jest większa niż limit...
            return 1.0f; // ...bez zmiany przezroczystości

        float alpha = distance / limitDistance;
        if (alpha < minTargetAlpha) // Przezroczystość jest mniejsza niż dopuszczalna...
            return minTargetAlpha; // ...zwrócenie minimalnej przezroczystości
        
        return alpha;
    }
}
