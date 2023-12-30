using UnityEngine;

public class InputManager : MonoBehaviour
{
    // #region Instance
    //
    // public static InputManager instance;
    //
    // private void Awake()
    // {
    //     if (instance == null)
    //     {
    //         instance = this;
    //     }
    // }
    //
    // #endregion

    public Player player;
    // public WhipWeapon whip;
    
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
        }
        
        playerInput.Enable();
    }

    private void OnDisable() => playerInput.Disable();

    public void HandleMovement(float speed)
    {
        player.playerMovement.Movement(movementInput, speed);
        player.playerAnimator.UpdateAnimatorValues(movementInput);
    }
    
    public void HandleCamera() => player.playerCamera.CalculateCameraPosition();

    // public void HandleWhipAttack(float deltaTime) => whip.Attack(deltaTime, player.playerAnimator.isLeft);

    public void HandleQuickSlots()
    {
        playerInput.Player.Health.performed += i => xInput = true;
        playerInput.Player.Shield.performed += i => bInput = true;
        playerInput.Player.Speed.performed += i => aInput = true;
        playerInput.Player.Magnet.performed += i => yInput = true;
        
        if (xInput) player.playerInventory.inventorySlots[3].UseItem();
        
        if (bInput) player.playerInventory.inventorySlots[1].UseItem();
        
        if (aInput) player.playerInventory.inventorySlots[2].UseItem();
        
        if (yInput) player.playerInventory.inventorySlots[0].UseItem();
    }

    public void PauseGame()
    {
        playerInput.Player.Pause.performed += i => startInput = true;
        
        if(startInput) player.playerUI.pauseMenu.PlayPause();
    }
}