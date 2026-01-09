using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Rebinding : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference moveAction;

    [Header("Jump UI")]
    [SerializeField] private Button jumpButton;
    [SerializeField] private TMP_Text jumpBindingText;
    [SerializeField] private GameObject jumpWaiting;
    [SerializeField] private GameObject jumpCurrent;

    [Header("Move UI")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private TMP_Text forwardBindingText;
    [SerializeField] private GameObject forwardWaiting;
    [SerializeField] private GameObject forwardCurrent;

    [SerializeField] private Button backwardButton;
    [SerializeField] private TMP_Text backwardBindingText;
    [SerializeField] private GameObject backwardWaiting;
    [SerializeField] private GameObject backwardCurrent;

    [SerializeField] private Button leftButton;
    [SerializeField] private TMP_Text leftBindingText;
    [SerializeField] private GameObject leftWaiting;
    [SerializeField] private GameObject leftCurrent;

    [SerializeField] private Button rightButton;
    [SerializeField] private TMP_Text rightBindingText;
    [SerializeField] private GameObject rightWaiting;
    [SerializeField] private GameObject rightCurrent;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
	private bool isRebinding;

    private void Awake()
	{
        if (jumpButton) jumpButton.onClick.AddListener(() => StartRebindSimple(jumpAction, jumpBindingText, jumpWaiting, jumpCurrent));

        if (forwardButton)  forwardButton.onClick.AddListener(() => StartRebindMovePart("up",    forwardBindingText,  forwardWaiting,  forwardCurrent));
        if (backwardButton) backwardButton.onClick.AddListener(() => StartRebindMovePart("down",  backwardBindingText, backwardWaiting, backwardCurrent));
        if (leftButton)     leftButton.onClick.AddListener(() => StartRebindMovePart("left",  leftBindingText,     leftWaiting,     leftCurrent));
        if (rightButton)    rightButton.onClick.AddListener(() => StartRebindMovePart("right", rightBindingText,    rightWaiting,    rightCurrent));

        RefreshMoveTexts();
        RefreshJumpText();
    }

    private void OnDestroy()
    {
        rebindingOperation?.Dispose();
    }

    // Rebind for simple button actions (Jump)
    private void StartRebindSimple(InputActionReference actionRef, TMP_Text text, GameObject waiting, GameObject current)
    {
		if (isRebinding) return;
		SetRebindUI(true);
		
        if (actionRef == null) return;

        waiting?.SetActive(true);
        current?.SetActive(false);

        actionRef.action.Disable();

        rebindingOperation?.Dispose();
        rebindingOperation = actionRef.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
			.WithCancelingThrough("<Keyboard>/escape")
			.WithControlsExcluding("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(_ => FinishSimple(actionRef, text, waiting, current))
            .OnComplete(_ => FinishSimple(actionRef, text, waiting, current))
            .Start();
    }

    private void FinishSimple(InputActionReference actionRef, TMP_Text text, GameObject waiting, GameObject current)
    {
		SetRebindUI(false);
		
        rebindingOperation?.Dispose();
        rebindingOperation = null;

        waiting?.SetActive(false);
        current?.SetActive(true);

		int bindingIndex = actionRef.action.bindings.IndexOf(b => !b.isComposite && !b.isPartOfComposite);

        if (text) text.text = InputControlPath.ToHumanReadableString(
			actionRef.action.bindings[bindingIndex].effectivePath,
			InputControlPath.HumanReadableStringOptions.OmitDevice);

        actionRef.action.Enable();
    }

    // Rebind a specific part of the Move composite
    private void StartRebindMovePart(string partName, TMP_Text text, GameObject waiting, GameObject current)
    {
		if (isRebinding) return;
		SetRebindUI(true);
		
        if (moveAction == null) return;

        int bindingIndex = FindMoveCompositePartBindingIndex(moveAction.action, partName);
        if (bindingIndex < 0)
        {
            Debug.LogError($"Could not find Move composite part '{partName}'. Check your Move bindings.");
            return;
        }

        waiting?.SetActive(true);
        current?.SetActive(false);

        moveAction.action.Disable();

        rebindingOperation?.Dispose();
        rebindingOperation = moveAction.action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
			.WithCancelingThrough("<Keyboard>/escape")
			.WithControlsExcluding("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(_ => FinishMovePart(bindingIndex, text, waiting, current))
            .OnComplete(_ => FinishMovePart(bindingIndex, text, waiting, current))
            .Start();
    }

    private void FinishMovePart(int bindingIndex, TMP_Text text, GameObject waiting, GameObject current)
    {
		SetRebindUI(false);
		
        rebindingOperation?.Dispose();
        rebindingOperation = null;

        waiting?.SetActive(false);
        current?.SetActive(true);

        if (text) text.text = moveAction.action.GetBindingDisplayString(bindingIndex);

        moveAction.action.Enable();
    }

    // Find the binding index of the FIRST 2DVector composite's part by name
    private int FindMoveCompositePartBindingIndex(InputAction action, string partName)
    {
        // This finds the first composite in the action
        int compositeIndex = -1;

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].isComposite)
            {
                compositeIndex = i;
                break; // first composite
            }
        }

        if (compositeIndex < 0) return -1;

        for (int i = compositeIndex + 1; i < action.bindings.Count; i++)
        {
            var b = action.bindings[i];
            if (!b.isPartOfComposite) break;

            if (b.name == partName) return i;
        }

        return -1;
    }

    private void RefreshMoveTexts()
    {
        if (moveAction == null) return;

        forwardBindingText.text = moveAction.action.GetBindingDisplayString(FindMoveCompositePartBindingIndex(moveAction.action, "up"));
        backwardBindingText.text = moveAction.action.GetBindingDisplayString(FindMoveCompositePartBindingIndex(moveAction.action, "down"));
        leftBindingText.text = moveAction.action.GetBindingDisplayString(FindMoveCompositePartBindingIndex(moveAction.action, "left"));
        rightBindingText.text = moveAction.action.GetBindingDisplayString(FindMoveCompositePartBindingIndex(moveAction.action, "right"));
    }

    private void RefreshJumpText()
    {
        if (jumpAction == null || jumpBindingText == null) return;
		
		int bindingIndex = jumpAction.action.bindings.IndexOf(b => !b.isComposite && !b.isPartOfComposite);

		jumpBindingText.text = InputControlPath.ToHumanReadableString(
			jumpAction.action.bindings[bindingIndex].effectivePath,
			InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

	private void SetRebindUI(bool rebinding)
	{
		isRebinding = rebinding;

		// Disable all buttons while rebinding
		if (jumpButton) jumpButton.interactable = !rebinding;
		if (forwardButton) forwardButton.interactable = !rebinding;
		if (backwardButton) backwardButton.interactable = !rebinding;
		if (leftButton) leftButton.interactable = !rebinding;
		if (rightButton) rightButton.interactable = !rebinding;
	}
}
