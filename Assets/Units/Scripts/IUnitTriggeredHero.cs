using System;

using Cysharp.Threading.Tasks;

public interface IUnitTriggeredHero
{
    UniTask OnTriggeredHero(Action<UnitBase> onTriggeredHero);
}
