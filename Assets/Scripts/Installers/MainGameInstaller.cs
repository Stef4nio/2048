using UnityEngine;
using Zenject;

public class MainGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameModel>().To<GameModel>().AsSingle();
        Container.Bind<EventSystem>().To<EventSystem>().AsSingle();
        Container.Bind<CellFactory>().To<CellFactory>().AsSingle();
    }
}