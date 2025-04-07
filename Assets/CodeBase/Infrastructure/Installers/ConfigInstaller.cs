using CodeBase.Gameplay.Common.Services.Level.Configs;
using CodeBase.UI.Hint.Configs;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
    public class ConfigInstaller : MonoInstaller
    {
        [SerializeField] private LevelDataSO _levelDataSo;
        [SerializeField] private HintConfig _hintConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(_levelDataSo).AsSingle();
            Container.BindInstance(_hintConfig).AsSingle();
        }
    }
}