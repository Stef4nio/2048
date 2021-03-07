using UnityEngine;
using Zenject;

public class MainGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameModel>().To<GameModel>().AsSingle();
    }
}