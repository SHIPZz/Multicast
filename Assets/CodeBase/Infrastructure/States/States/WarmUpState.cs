using System;
using System.Threading;
using CodeBase.Common.Services.InternetConnection;
using CodeBase.Common.Services.Persistent;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Common.Services.Unity;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.StaticData;
using CodeBase.UI.Game;
using CodeBase.UI.Hint;
using CodeBase.UI.LoadingCurtains;
using CodeBase.UI.Menu;
using CodeBase.UI.NoInternet;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Settings;
using CodeBase.UI.Victory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure.States.States
{
    public class WarmUpState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IAssetDownloadService _assetDownloadService;
        private readonly IInternetConnectionService _internetConnectionService;
        private readonly IWindowService _windowService;
        private readonly IRemoteConfigService _unityRemoteConfigService;
        private readonly IPersistentService _persistentService;
        private readonly ISaveOnApplicationPauseSystem _saveOnApplicationPauseSystem;
        private readonly IStaticDataService _staticDataService;
        private readonly CancellationTokenSource _cancellationToken = new();
        
        public WarmUpState(IStateMachine stateMachine,
            IAssetDownloadService assetDownloadService,
            IInternetConnectionService internetConnectionService,
            IWindowService windowService,
            IRemoteConfigService unityRemoteConfigService, 
            IPersistentService persistentService, 
            ISaveOnApplicationPauseSystem saveOnApplicationPauseSystem,
            IStaticDataService staticDataService)
        {
            _internetConnectionService = internetConnectionService ?? throw new ArgumentNullException(nameof(internetConnectionService));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _unityRemoteConfigService = unityRemoteConfigService ?? throw new ArgumentNullException(nameof(unityRemoteConfigService));
            _persistentService = persistentService ?? throw new ArgumentNullException(nameof(persistentService));
            _saveOnApplicationPauseSystem = saveOnApplicationPauseSystem ?? throw new ArgumentNullException(nameof(saveOnApplicationPauseSystem));
            _assetDownloadService = assetDownloadService ?? throw new ArgumentNullException(nameof(assetDownloadService));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _staticDataService = staticDataService ?? throw new ArgumentNullException(nameof(staticDataService));
        }

        public async void Enter()
        {
            try
            {
                BindAndOpenLoadingWindowAsync(_cancellationToken.Token).Forget();
                
                await _staticDataService.LoadAllAsync(_cancellationToken.Token);

                BindWindows();
            
                await InitializeAdressablesAsync(_cancellationToken.Token);

                LaunchInternetChecking();

                await ProcessNoInternetConnectionAsync(_cancellationToken.Token);

                await InitializeConfigAsync(_cancellationToken.Token);

                LoadData();
            
                InitSaveOnPauseSystem();
            
                _stateMachine.Enter<LoadingMenuState>();
            }
            catch (Exception e)
            {
                Debug.LogError("Error during warm-up state: " + e.Message);
            }
        }

        private async UniTask ProcessNoInternetConnectionAsync(CancellationToken cancellationTokenToken)
        {
            while (!_internetConnectionService.IsInternetAvailable) 
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationTokenToken);
            
            _windowService.Close<NoInternetWindow>();
            _windowService.OpenWindow<LoadingCurtainWindow>();
        }

        private void LaunchInternetChecking()
        {
            _internetConnectionService.LaunchCheckingEveryFixedIntervalAsync(_cancellationToken.Token);

            OpenNoInternetWindowOnNoInternet();
        }

        private void InitSaveOnPauseSystem()
        {
            _saveOnApplicationPauseSystem.Initialize();
        }

        private void LoadData()
        {
            _persistentService.LoadAll();
        }

        private async UniTaskVoid BindAndOpenLoadingWindowAsync(CancellationToken cancellationToken)
        {
            await _staticDataService.LoadWindowAsync<LoadingCurtainWindow>(cancellationToken);
            
            _windowService.Bind<LoadingCurtainWindow,LoadingCurtainWindowController>();
            
            _windowService.OpenWindow<LoadingCurtainWindow>();
        }

        private async UniTask InitializeConfigAsync(CancellationToken cancellationTokenToken)
        {
            await _unityRemoteConfigService.InitializeAsync(cancellationTokenToken);
            await _unityRemoteConfigService.FetchConfigsAsync(new UserAttributes(), new AppAttributes(), cancellationTokenToken);
        }

        private async UniTask InitializeAdressablesAsync(CancellationToken cancellationTokenToken)
        {
            await _assetDownloadService.InitializeDownloadDataAsync(cancellationTokenToken);
            
            if (_assetDownloadService.GetDownloadSizeMb() > 0)
                await _assetDownloadService.UpdateContentAsync(cancellationTokenToken);
        }

        private void BindWindows()
        {
            _windowService.Bind<NoInternetWindow,NoInternetWindowController>();
            _windowService.Bind<GameWindow,GameWindowController>();
            _windowService.Bind<MenuWindow,MenuWindowController>();
            _windowService.Bind<SettingsWindow,SettingsWindowController>();
            _windowService.Bind<VictoryWindow,VictoryWindowController>();
            _windowService.Bind<HintWindow,HintWindowController>();
        }

        private void OpenNoInternetWindowOnNoInternet()
        {
            if (!_internetConnectionService.IsInternetAvailable)
            {
                _windowService.Close<LoadingCurtainWindow>();
                _windowService.OpenWindow<NoInternetWindow>(true);
            }
        }

        public void Exit() { }
    }
}