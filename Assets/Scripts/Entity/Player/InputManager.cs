using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Player player;

    public bool xInput;
    public bool bInput;
    public bool aInput;
    public bool yInput;
    public bool startInput;

    public Vector2 movementInput;
    private PlayerInput playerInput;

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();

            playerInput.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            playerInput.Player.Pause.performed += i => startInput = true;

            playerInput.Player.Health.performed += i => xInput = true;
            playerInput.Player.Shield.performed += i => bInput = true;
            playerInput.Player.Speed.performed += i => aInput = true;
            playerInput.Player.Magnet.performed += i => yInput = true;
        }

        playerInput.Enable();
    }

    private void OnDisable() => playerInput.Disable();

    public void HandleMovement(float speed)
    {
        if (!player.playerUI.pauseMenu.paused)
        {
            player.playerMovement.Movement(movementInput, speed);
            player.playerAnimator.UpdateAnimatorValues(movementInput);
        }
    }

    public void HandleCamera() => player.playerCamera.CalculateCameraPosition();

    public void HandleQuickSlots()
    {
        if (yInput) player.playerInventory.inventorySlots[0].UseItem();

        if (bInput) player.playerInventory.inventorySlots[1].UseItem();

        if (aInput) player.playerInventory.inventorySlots[2].UseItem();

        if (xInput) player.playerInventory.inventorySlots[3].UseItem();
    }

    public void PauseGame()
    {
        player.playerUI.pauseMenu.TakePause(startInput);
    }

    public void StartInput() => startInput = !startInput;
}