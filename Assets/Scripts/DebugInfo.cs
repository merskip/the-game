using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DebugInfo : MonoBehaviour {

    private Text info;
    private string infoFormat;

    private FPSCounter fpsCounter;

    private CharacterController playerController;
    private PlayerCameraOrbit cameraOrbit;

    private GameObject player;
    private Vector3 playerStartPosition;

	void Start() {
        info = GetComponent<Text>();
        infoFormat = info.text;

        fpsCounter =  new FPSCounter();
        fpsCounter.setUpdatesForOneSecond(2);

        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<CharacterController>();

        GameObject camera = GameObject.FindWithTag("MainCamera");
        cameraOrbit = camera.GetComponent<PlayerCameraOrbit>();

        playerStartPosition = player.transform.position;
	}
	
	void Update() {
        fpsCounter.tick();

        float fps = fpsCounter.FPS;
        bool isGrounded = playerController.isGrounded;
        string playerPosition = player.transform.position.ToString();
        Vector2 cameraPosition = cameraOrbit.position;
        float cameraDistance = cameraOrbit.distance;

        info.text = string.Format(infoFormat,
                                  fps, isGrounded,
                                  playerPosition,
                                  cameraPosition.x, cameraPosition.y, cameraDistance);
	}

    public void restorePlayerPositionAndClearFocus() {
        player.transform.position = playerStartPosition;
        player.GetComponent<CharacterMovement>().clearMove();

        EventSystem.current.SetSelectedGameObject(null, null);
    }
}

class FPSCounter {

    private float deltaTime = 0.0f;
    private int frameCount = 0;
    private float updateTime = 1.0f;

    private float currentFps = 0.0f;

    public float FPS {
        get {
            return currentFps;
        }
    }

    public void tick() {
        frameCount++;
        deltaTime += Time.deltaTime;

        if (isTimeForUpdate())
            updateFPSAndClearCouter();
    }

    private bool isTimeForUpdate() {
        return deltaTime > updateTime;
    }

    private void updateFPSAndClearCouter() {
        currentFps = frameCount / deltaTime;
        frameCount = 0;
        deltaTime -= updateTime;
    }

    public void setUpdatesForOneSecond(int count) {
        updateTime = 1.0f / count;
    }

    public int getUpdatesForOneSecond() {
        return (int) (1.0f / updateTime);
    }
}