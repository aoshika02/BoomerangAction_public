using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    public IReadOnlyReactiveProperty<Vector2> Move => _move;
    private ReactiveProperty<Vector2> _move = new ReactiveProperty<Vector2>(Vector2.zero);
    public IReadOnlyReactiveProperty<float> Jump => _jump;
    private ReactiveProperty<float> _jump = new ReactiveProperty<float>();
    public IReadOnlyReactiveProperty<float> Attack => _attack;
    private ReactiveProperty<float> _attack = new ReactiveProperty<float>();

    public IReadOnlyReactiveProperty<Vector2> Throw => _throw;
    private ReactiveProperty<Vector2> _throw = new ReactiveProperty<Vector2>();

    private PlayerAction _playerAction;

    #region Move

    public void SetMoveStartedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Move.started += action;
    }

    public void SetMovePerformedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Move.performed += action;
    }

    public void SetMoveCanceledAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Move.canceled += action;
    }

    public void RemoveMoveStartedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Move.started -= action;
    }

    public void RemoveMovePerformedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Move.performed -= action;
    }

    public void RemoveMoveCanceledAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Move.canceled -= action;
    }

    #endregion

    #region Throw
    public void SetThrowStartedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Throw.started += action;
    }
    public void SetThrowPerformedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Throw.performed += action;
    }
    public void SetThrowCanceledAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Throw.canceled += action;
    }
    public void RemoveThrowStartedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Throw.started -= action;
    }
    public void RemoveThrowPerformedAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Throw.performed -= action;
    }
    public void RemoveThrowCanceledAction(Action<InputAction.CallbackContext> action)
    {
        _playerAction.Player.Throw.canceled -= action;
    }
    #endregion

    protected override void Awake()
    {
        if (!CheckInstance()) return;
        DontDestroyOnLoad(this);
        _playerAction = new PlayerAction();
        _playerAction.Player.Enable();

        _playerAction.Player.Move.performed += ctx => _move.Value = ctx.ReadValue<Vector2>();
        _playerAction.Player.Move.canceled += ctx => _move.Value = Vector2.zero;

        _playerAction.Player.Jump.performed += ctx => _jump.Value = ctx.ReadValue<float>();
        _playerAction.Player.Jump.canceled += ctx => _jump.Value = 0f;

        _playerAction.Player.Attack.performed += ctx => _attack.Value = ctx.ReadValue<float>();
        _playerAction.Player.Attack.canceled += ctx => _attack.Value = 0f;

        _playerAction.Player.Throw.performed += ctx => _throw.Value = ctx.ReadValue<Vector2>();
        _playerAction.Player.Throw.canceled += ctx => _throw.Value = Vector2.zero;
    }
}
