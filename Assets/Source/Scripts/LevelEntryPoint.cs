using Assets.Source.Scripts.InputSystem;
using Assets.Source.Scripts.LevelStructure;
using LDG.SoundReactor;
using Source.Scripts;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Source
{
    public class LevelEntryPoint : LifetimeScope
    {
        [SerializeField] private GameConfig _gameConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_gameConfig);

            builder.RegisterComponentInHierarchy<AudioMidiSync>();
            builder.RegisterComponentInHierarchy<TracksContainer>();

//            builder.Register<DesktopPlayerInput>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MobilePlayerInput>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            
            builder.Register<LevelGenerator>(Lifetime.Singleton).AsImplementedInterfaces();


            builder.RegisterComponentInHierarchy<ScoreView>();
            builder.RegisterComponentInHierarchy<ReceptorView>();

            builder.Register<Receptor>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<ReceptorPresenter>(Lifetime.Singleton);
            
            builder.Register<ScoreCounter>(Lifetime.Singleton);
            builder.Register<ScorePresenter>(Lifetime.Singleton);
            
            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<ReceptorPresenter>();
                container.Resolve<ScorePresenter>();
            });
    }

        protected override void OnDestroy()
        {
            Dispose();
        }
    }
}