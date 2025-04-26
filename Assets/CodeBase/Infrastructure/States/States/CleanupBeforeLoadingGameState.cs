using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.WordSlots.Services;

namespace CodeBase.Infrastructure.States.States
{
    public class CleanupBeforeLoadingGameState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IWordSlotFacade _wordSlotFacade;
        private readonly IClusterService _clusterService;

        public CleanupBeforeLoadingGameState(IWordSlotFacade wordSlotFacade,
            IClusterService clusterService,
            IStateMachine stateMachine)
        {
            _clusterService = clusterService;
            _wordSlotFacade = wordSlotFacade;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _clusterService.Cleanup();
            _wordSlotFacade.Cleanup();

            _stateMachine.Enter<LoadGameState>();
        }

        public void Exit()
        {
            
        }
    }
}