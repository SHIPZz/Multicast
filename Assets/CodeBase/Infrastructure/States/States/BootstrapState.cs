using System;
using System.Threading.Tasks;
using CodeBase.Common.Services.InternetConnection;
using CodeBase.Common.Services.Persistent;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Common.Services.Unity;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Game;
using CodeBase.UI.Hint;
using CodeBase.UI.LoadingWindow;
using CodeBase.UI.Menu;
using CodeBase.UI.NoInternet;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Settings;
using CodeBase.UI.Victory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure.States.States
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IAssetDownloadService _assetDownloadService;
        private readonly IUnityRemoteConfigService _unityRemoteConfigService;
        private readonly IWindowService _windowService;
        private readonly IPersistentService _persistentService;
        private readonly IInternetConnectionService _internetConnectionService;
        private readonly ISaveOnApplicationPauseSystem _saveOnApplicationPauseSystem;

        public BootstrapState(IStateMachine stateMachine,
            IAssetDownloadService assetDownloadService,
            IInternetConnectionService internetConnectionService,
            IWindowService windowService,
            IUnityRemoteConfigService unityRemoteConfigService, 
            IPersistentService persistentService, 
            ISaveOnApplicationPauseSystem saveOnApplicationPauseSystem)
        {
            _internetConnectionService = internetConnectionService;
            _windowService = windowService;
            _unityRemoteConfigService = unityRemoteConfigService;
            _persistentService = persistentService;
            _saveOnApplicationPauseSystem = saveOnApplicationPauseSystem;
            _assetDownloadService = assetDownloadService;
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        }

        public async void Enter()
        {
            BindWindows();
            
            if (!_internetConnectionService.CheckConnection())
                return;

            _windowService.OpenWindow<LoadingWindow>();
            
            await InitializeAdressablesAndConfig();

            _windowService.Close<LoadingWindow>();
            
            _persistentService.LoadAll();
            
            _saveOnApplicationPauseSystem.Initialize();
            
            _stateMachine.Enter<LoadingMenuState>();
        }

        private async UniTask InitializeAdressablesAndConfig()
        {
            await _assetDownloadService.InitializeDownloadDataAsync();
            await _unityRemoteConfigService.InitializeAsync();
            await _unityRemoteConfigService.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
            
            if (_assetDownloadService.GetDownloadSizeMb() > 0)
                await _assetDownloadService.UpdateContentAsync();
        }
        
        private void BindWindows()
        {
            _windowService.Bind<LoadingWindow,LoadingWindowController>();
            _windowService.Bind<NoInternetWindow,NoInternetWindowController>();
            _windowService.Bind<GameWindow,GameWindowController>();
            _windowService.Bind<MenuWindow,MenuWindowController>();
            _windowService.Bind<SettingsWindow,SettingsWindowController>();
            _windowService.Bind<VictoryWindow,VictoryWindowController>();
            _windowService.Bind<HintWindow,HintWindowController>();
        }

        public void Exit() { }
    }
}