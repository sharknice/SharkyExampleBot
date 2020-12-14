using Sharky;
using Sharky.EnemyStrategies;
using Sharky.Managers;

namespace SharkyExampleBot.EnemyStrategies
{
    public class ZealotRush : EnemyStrategy
    {
        public ZealotRush(EnemyStrategyHistory enemyStrategyHistory, IChatManager chatManager, IUnitManager unitManager, SharkyOptions sharkyOptions)
        {
            EnemyStrategyHistory = enemyStrategyHistory;
            ChatManager = chatManager;
            UnitManager = unitManager;
            SharkyOptions = sharkyOptions;
        }

        protected override bool Detect(int frame)
        {
            if (UnitManager.EnemyCount(UnitTypes.PROTOSS_ZEALOT) >= 5 && frame < SharkyOptions.FramesPerSecond * 4 * 60)
            {
                return true;
            }

            return false;
        }
    }
}
