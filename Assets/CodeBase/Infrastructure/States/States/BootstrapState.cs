using System;
using CodeBase.Common.Services.Unity;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using UnityEngine;

namespace CodeBase.Infrastructure.States.States
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IAssetDownloadService _assetDownloadService;
        private readonly IUnityRemoteConfigService _unityRemoteConfigService;
        private readonly ILevelService _levelService;

        public BootstrapState(IStateMachine stateMachine,
            IAssetDownloadService assetDownloadService,
            ILevelService levelService,
            IUnityRemoteConfigService unityRemoteConfigService)
        {
            _levelService = levelService;
            _unityRemoteConfigService = unityRemoteConfigService;
            _assetDownloadService = assetDownloadService;
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        public async void Enter()
        {
            await _assetDownloadService.InitializeDownloadDataAsync();
            await _unityRemoteConfigService.InitializeAsync();
            await _unityRemoteConfigService.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
            
            if (_assetDownloadService.GetDownloadSizeMb() > 0)
                await _assetDownloadService.UpdateContentAsync();

            _levelService.Initialize();
            
            _stateMachine.Enter<LoadingMenuState>();
        }

        public void Exit() { }
    }
}