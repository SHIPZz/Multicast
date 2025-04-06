using CodeBase.Gameplay.SO.Hints;
using CodeBase.Gameplay.SO.Level;
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