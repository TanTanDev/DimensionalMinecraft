using UnityEngine;
using Zenject;
using Tantan;
using TanTanTools;
using TanTanTools.Audio;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private AudioManagerMono m_audioManager;
    public override void InstallBindings()
    {
        Container.Bind<IAudioManager>().FromInstance(m_audioManager);
    }
}
