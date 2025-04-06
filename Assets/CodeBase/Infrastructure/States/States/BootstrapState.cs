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
        private readonly ILoadingCurtain _loadingCurtain;

        public BootstrapState(IStateMachine stateMachine,
            IAssetDownloadService assetDownloadService,
            IInternetConnectionService internetConnectionService,
            IWindowService windowService,
            IRemoteConfigService unityRemoteConfigService, 
            IPersistentService persistentService, 
            ISaveOnApplicationPauseSystem saveOnApplicationPauseSystem,
            IStaticDataService staticDataService, 
            ILoadingCurtain loadingCurtain)
        {
            _internetConnectionService = internetConnectionService;
            _windowService = windowService;
            _unityRemoteConfigService = unityRemoteConfigService;
            _persistentService = persistentService;
            _saveOnApplicationPauseSystem = saveOnApplicationPauseSystem;
            _assetDownloadService = assetDownloadService;
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _staticDataService = staticDataService;
            _loadingCurtain = loadingCurtain;
        }

        public async void Enter()
        {
            _loadingCurtain.Show();
            
            await InitializeAdressablesAndConfig();
            await _staticDataService.LoadAllAsync();
            
            BindWindows();
            
            if (!_internetConnectionService.CheckConnection())
            {
                _windowService.OpenWindow<NoInternetWindow>();
                return;
            }
            
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