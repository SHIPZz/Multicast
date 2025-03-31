using System;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;

namespace CodeBase.Infrastructure.States.States
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IAssetDownloadService _assetDownloadService;

        public BootstrapState(IStateMachine stateMachine, IAssetDownloadService assetDownloadService)
        {
            _assetDownloadService = assetDownloadService;
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        public async void Enter()
        {
            // await _assetDownloadService.InitializeDownloadDataAsync();
            //
            // if (_assetDownloadService.GetDownloadSizeMb() > 0)
            //     await _assetDownloadService.UpdateContentAsync();

            _stateMachine.Enter<LoadingMenuState>();
        }

        public void Exit() { }
    }
}