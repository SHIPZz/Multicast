using System;
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
using CodeBase.UI.Levels;
using CodeBase.UI.LoadingCurtains;
using CodeBase.UI.Menu;
using CodeBase.UI.NoInternet;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Settings;
using CodeBase.UI.Victory;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.States.States
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IAssetDownloadService _assetDownloadService;
        private readonly IInternetConnectionService _internetConnectionService;
        private readonly IWindowService _windowService;
        private readonly IRemoteConfigService _unityRemoteConfigService;
        private readonly IPersistentService _persistentService;
        private readonly ISaveOnApplicationPauseSystem _saveOnApplicationPauseSystem;
        private readonly IStaticDataService _staticDataService;

        public BootstrapState(IStateMachine stateMachine,
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
            BindAndOpenLoadingWindowAsync().Forget();

            await _staticDataService.LoadAllAsync();

            BindWindows();
            
            await InitializeAdressablesAsync();

            LaunchInternetChecking();

            while (!_internetConnectionService.IsInternetAvailable) 
                await UniTask.Yield();
            
            _windowService.Close<NoInternetWindow>();
            _windowService.OpenWindow<LoadingCurtainWindow>();

            await InitializeConfigAsync();

            _persistentService.LoadAll();
            
            _saveOnApplicationPauseSystem.Initialize();
            
            _stateMachine.Enter<LoadingMenuState>();
        }

        private void LaunchInternetChecking()
        {
            _internetConnectionService.LaunchCheckingEveryFixedIntervalAsync();

            OpenNoInternetWindowOnNoInternet();
        }

        private async UniTask BindAndOpenLoadingWindowAsync()
        {
            await _staticDataService.LoadWindowAsync<LoadingCurtainWindow>();
            
            _windowService.Bind<LoadingCurtainWindow,LoadingCurtainWindowController>();
            
            _windowService.OpenWindow<LoadingCurtainWindow>();
        }

        private async UniTask InitializeConfigAsync()
        {
            await _unityRemoteConfigService.InitializeAsync();
            await _unityRemoteConfigService.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
        }

        private async UniTask InitializeAdressablesAsync()
        {
            await _assetDownloadService.InitializeDownloadDataAsync();
            
            if (_assetDownloadService.GetDownloadSizeMb() > 0)
                await _assetDownloadService.UpdateContentAsync();
        }

        private void BindWindows()
        {
            _windowService.Bind<NoInternetWindow,NoInternetWindowController>();
            _windowService.Bind<GameWindow,GameWindowController>();
            _windowService.Bind<LevelWindow,LevelWindowController>();
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