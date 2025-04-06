using CodeBase.Common.Services.InternetConnection;
using CodeBase.Common.Services.Persistent;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Common.Services.Sound;
using CodeBase.Common.Services.Unity;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Hint;
using CodeBase.Gameplay.WordSlots;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Loading;
using CodeBase.Infrastructure.States.Factory;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.StaticData;
using CodeBase.UI.LoadingCurtains;
using CodeBase.UI.Services;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Services.WordSlots;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, ICoroutineRunner, IInitializable
    {
        public override void InstallBindings()
        {
            BindInfrastructureServices();
            BindAssetManagementServices();
            BindCommonServices();
            BindUIServices();
            BindStates();
            BindLevelService();
            BindClusterService();
            BindHintService();
            BindUnityRemoteConfigService();
            SetupDevice();
            BindInternetConnectionService();

            Container.BindInterfacesAndSelfTo<StateMachine>().AsSingle();
        }

        private void BindInternetConnectionService()
        {
            Container.BindInterfacesTo<InternetConnectionService>().AsSingle();
        }

        private void SetupDevice()
        {
            Application.targetFrameRate = 90;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        private void BindUnityRemoteConfigService()
        {
            Container.BindInstance(RemoteConfigService.Instance).AsSingle();
            Container.BindInterfacesTo<UnityRemoteConfigService>().AsSingle();
        }

        private void BindHintService()
        {
            Container.BindInterfacesTo<HintService>().AsSingle();
        }

        private void BindClusterService()
        {
            Container.BindInterfacesTo<ClusterService>().AsSingle();
        }

        private void BindLevelService()
        {
            Container.BindInterfacesTo<LevelService>().AsSingle();
        }

        private void BindUIServices()
        {
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IUIProvider>().To<UIProvider>().AsSingle();
            Container.Bind<IStaticDataService>().To<StaticDataService>().AsSingle();
            Container.Bind<IWordSlotUIFactory>().To<WordSlotUIFactory>().AsSingle();
            Container.BindInterfacesTo<WordSlotService>().AsSingle();
            Container.Bind<IClusterUIFactory>().To<ClusterUIFactory>().AsSingle();
        }
        
        private void BindStates()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<CleanupBeforeLoadingGameState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadGameState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameState>().AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container.Bind<IStateFactory>().To<StateFactory>().AsSingle();
            Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();
        }

        private void BindAssetManagementServices()
        {
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<IAssetDownloadService>().To<LabeledAssetDownloadService>().AsSingle();
            Container.Bind<IAssetDownloadReporter>().To<AssetDownloadReporter>().AsSingle();
        }

        private void BindCommonServices()
        {
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<IPersistentService>().To<PersistentService>().AsSingle();
            Container.Bind<ISaveLoadSystem>().To<PlayerPrefsSaveLoadSystem>().AsSingle();
            Container.BindInterfacesTo<SoundService>().AsSingle();
            Container.BindInterfacesTo<SoundFactory>().AsSingle();
            Container.BindInterfacesTo<SaveOnApplicationPauseSystem>().AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IStateMachine>().Enter<BootstrapState>();
        }
    }
}