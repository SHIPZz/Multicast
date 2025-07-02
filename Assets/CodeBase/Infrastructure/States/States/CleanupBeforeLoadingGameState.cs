using System;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.WordCells.Services;

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
            _clusterService = clusterService ?? throw new ArgumentNullException(nameof(clusterService));
            _wordSlotFacade = wordSlotFacade ?? throw new ArgumentNullException(nameof(wordSlotFacade));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
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