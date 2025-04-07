using System;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.WordSlots.Services;

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
            _clusterService = clusterService ?? throw new ArgumentNullException(nameof(clusterService));
            _wordSlotService = wordSlotService ?? throw new ArgumentNullException(nameof(wordSlotService));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
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