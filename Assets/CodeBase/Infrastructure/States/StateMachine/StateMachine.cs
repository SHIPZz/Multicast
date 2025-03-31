using CodeBase.Infrastructure.States.Factory;
using CodeBase.Infrastructure.States.StateInfrastructure;
using Zenject;

namespace CodeBase.Infrastructure.States.StateMachine
{
  public class StateMachine : IStateMachine, ITickable
  {
    private IExitableState _activeState;
    private readonly IStateFactory _stateFactory;

    public StateMachine(IStateFactory stateFactory)
    {
      _stateFactory = stateFactory;
    }

    public void Tick()
    {
      if(_activeState is IUpdateable updateableState)
        updateableState.Update();
    }
    
    public void Enter<TState>() where TState : class, IState
    {
      if(_activeState != null && _activeState.GetType() == typeof(TState))
        return;
      
      IState state = ChangeState<TState>();
      state.Enter();
    }
    
    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
    {
      if(_activeState != null && _activeState.GetType() == typeof(TState))
        return;
      
      TState state = ChangeState<TState>();
      state.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
      _activeState?.Exit();
      
      TState state = _stateFactory.GetState<TState>();
      _activeState = state;
      
      return state;
    }
  }
}