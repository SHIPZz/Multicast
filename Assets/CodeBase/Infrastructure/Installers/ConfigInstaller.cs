using CodeBase.Gameplay.SO.Level;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
    public class ConfigInstaller : MonoInstaller
    {
        [SerializeField] private LevelDataSO _levelDataSo;

        public override void InstallBindings()
        {
            Container.BindInstance(_levelDataSo).AsSingle();
        }
    }
}