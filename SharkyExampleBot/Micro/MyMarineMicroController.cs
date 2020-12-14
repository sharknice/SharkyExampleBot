using SC2APIProtocol;
using Sharky;
using Sharky.Managers;
using Sharky.MicroControllers;
using Sharky.Pathing;
using System.Linq;

namespace SharkyExampleBot.Micro
{
    public class MyMarineMicroController : IndividualMicroController
    {
        public MyMarineMicroController(MapDataService mapDataService, UnitDataManager unitDataManager, IUnitManager unitManager, DebugManager debugManager, IPathFinder sharkyPathFinder, IBaseManager baseManager, SharkyOptions sharkyOptions, MicroPriority microPriority, bool groupUpEnabled)
            : base(mapDataService, unitDataManager, unitManager, debugManager, sharkyPathFinder, baseManager, sharkyOptions, microPriority, groupUpEnabled)
        {

        }

        protected override bool OffensiveAbility(UnitCommander commander, Point2D target, Point2D defensivePoint, Point2D groupCenter, UnitCalculation bestTarget, int frame, out SC2APIProtocol.Action action)
        {
            action = null;

            if (UnitDataManager.ResearchedUpgrades.Contains((uint)Upgrades.STIMPACK))
            {
                if (commander.UnitCalculation.Unit.BuffIds.Contains((uint)Buffs.STIMPACK)) // don't double stim
                {
                    return false;
                }

                if (commander.UnitCalculation.EnemiesInRange.Sum(e => e.Unit.Health + e.Unit.Shield) > 100) // stim if more than 100 hitpoints in range
                {
                    action = commander.Order(frame, Abilities.EFFECT_STIM);
                    return true;
                }
            }

            return false;
        }
    }
}
