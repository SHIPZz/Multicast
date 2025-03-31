using CodeBase.Common.Services.SaveLoad;
using CodeBase.Gameplay.Common.Services.Cluster;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Loading;
using CodeBase.Infrastructure.States.Factory;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.StaticData;
using CodeBase.UI.Services;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Services.WordSlots;
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

            Container.BindInterfacesAndSelfTo<StateMachine>().AsSingle();
        }

        private void BindClusterService()
        {
            Container.Bind<IClusterService>().To<ClusterService>().AsSingle();
        }

        private void BindLevelService()
        {
            Container.Bind<ILevelService>().To<LevelService>().AsSingle();
        }

        private void BindUIServices()
        {
            Container.Bind<IWindowService>().To<WindowService>().AsSingle();
            Container.Bind<IUIProvider>().To<UIProvider>().AsSingle();
            Container.Bind<IUIStaticDataService>().To<UIStaticDataService>().AsSingle();
            Container.Bind<IWordSlotUIFactory>().To<WordSlotUIFactory>().AsSingle();
            Container.Bind<IClusterUIFactory>().To<ClusterUIFactory>().AsSingle();
        }
        
        private void BindStates()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingMenuState>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuState>().AsSingle();
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
            Container.Bind<ISaveLoadSystem>().To<PlayerPrefsSaveLoadSystem>().AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IStateMachine>().Enter<BootstrapState>();
        }
    }
}