using CodeBase.Gameplay.WordSlots;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Services.Cluster;

namespace CodeBase.Infrastructure.States.States
{
    public class CleanupBeforeLoadingGameState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IWordSlotService _wordSlotService;
        private readonly IClusterService _clusterService;

        public CleanupBeforeLoadingGameState(IWordSlotService wordSlotService,
            IClusterService clusterService,
            IStateMachine stateMachine)
        {
            _clusterService = clusterService;
            _wordSlotService = wordSlotService;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _clusterService.Cleanup();
            _wordSlotService.Cleanup();

            _stateMachine.Enter<LoadGameState>();
        }

        public void Exit()
        {
            
        }
    }
}