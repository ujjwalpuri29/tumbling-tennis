using UnityEngine;
using UnityEngine.InputSystem;

public enum MouseHitButton
{
    Left,
    Right,
    Middle
}

public class PlayerInput : MonoBehaviour
{
    private PlayerController controller;

    [Header("Primary Controls")]
    [SerializeField] private Key left = Key.A;
    [SerializeField] private Key right = Key.D;
    [SerializeField] private Key jump = Key.W;
    [SerializeField] private Key softHit = Key.LeftShift;
    [SerializeField] private Key hardHit = Key.LeftCtrl;

    [Header("Single Player Alternate Movement")]
    [SerializeField] private Key alternateLeft = Key.LeftArrow;
    [SerializeField] private Key alternateRight = Key.RightArrow;
    [SerializeField] private Key alternateJump = Key.UpArrow;

    [Header("Single Player Hit Controls")]
    [SerializeField] private Key singlePlayerSoftHit = Key.Space;
    [SerializeField] private Key singlePlayerHardHit = Key.C;

    [Header("Mouse Hit Controls")]
    [SerializeField] private bool enableMouseHits = true;
    [SerializeField] private MouseHitButton softHitMouseButton = MouseHitButton.Left;
    [SerializeField] private MouseHitButton hardHitMouseButton = MouseHitButton.Right;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogError(
                "PlayerInput requires a PlayerController on the same GameObject.",
                this
            );

            enabled = false;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        bool isSinglePlayerHuman = (controller.PlayerID == Player.playerOne) &&
            (ModeChoose.Instance != null) && (ModeChoose.Instance.gameType == GameType.PVE);

        controller.horizontalMoveDir = 0;

        if (keyboard != null)
        {
            if (keyboard[left].isPressed || (isSinglePlayerHuman && keyboard[alternateLeft].isPressed))
            {
                controller.horizontalMoveDir--;
            }

            if (keyboard[right].isPressed || (isSinglePlayerHuman && keyboard[alternateRight].isPressed))
            {
                controller.horizontalMoveDir++;
            }

            if (keyboard[jump].wasPressedThisFrame || (isSinglePlayerHuman && keyboard[alternateJump].wasPressedThisFrame))
            {
                controller.ifJump = true;
            }

            bool softHitPressed = keyboard[softHit].wasPressedThisFrame ||
                                  (isSinglePlayerHuman && keyboard[singlePlayerSoftHit].wasPressedThisFrame);

            bool hardHitPressed = keyboard[hardHit].wasPressedThisFrame ||
                                  (isSinglePlayerHuman && keyboard[singlePlayerHardHit].wasPressedThisFrame);

            bool useMouseHits = enableMouseHits && (controller.PlayerID == Player.playerTwo || isSinglePlayerHuman);

            if (useMouseHits && mouse != null)
            {
                if (softHitMouseButton == MouseHitButton.Left)
                {
                    softHitPressed |= mouse.leftButton.wasPressedThisFrame;
                }
                else if (softHitMouseButton == MouseHitButton.Right)
                {
                    softHitPressed |= mouse.rightButton.wasPressedThisFrame;
                }

                if (hardHitMouseButton == MouseHitButton.Left)
                {
                    hardHitPressed |= mouse.leftButton.wasPressedThisFrame;
                }
                else if (hardHitMouseButton == MouseHitButton.Right)
                {
                    hardHitPressed |= mouse.rightButton.wasPressedThisFrame;
                }
            }

            if (softHitPressed)
            {
                controller.ifHit = 1;
            }

            if (hardHitPressed)
            {
                controller.ifHit = 2;
            }
        }
    }
}
