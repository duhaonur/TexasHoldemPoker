using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputController : MonoBehaviour
{
    private Ray _ray;
    private Camera _mainCamera;

    private Vector2 _touchStartPos;

    private bool _gameStarted = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += StartTouch;
        Touch.onFingerMove += FingerMoving;
        Touch.onFingerUp += EndTouch;

        GameEvents.OnStartGame += GameStarted;
    }
    private void OnDisable()
    {
        Touch.onFingerDown -= StartTouch;
        Touch.onFingerMove -= FingerMoving;
        Touch.onFingerUp -= EndTouch;
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();

        GameEvents.OnStartGame -= GameStarted;
    }
    private void GameStarted()
    {
        _gameStarted = true;
    }

    private void StartTouch(Finger finger)
    {
        if (_gameStarted)
            return;

        _touchStartPos = finger.currentTouch.startScreenPosition;
        _ray = _mainCamera.ScreenPointToRay(finger.currentTouch.screenPosition);
        if (Physics.Raycast(_ray, out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out Seat seat))
            {
                //
                //Debug.Log($"Is Null {transform.parent.gameObject == null}");
                SeatEvents.CallSeatSelected(gameObject, seat.SeatId);
                SeatEvents.CallGetSeatId(seat.SeatId, seat);
                GameEvents.CallPlayerSeated(seat.transform);
            }
        }
    }
    private void FingerMoving(Finger finger)
    {
    }
    private void EndTouch(Finger finger)
    {
    }
}
