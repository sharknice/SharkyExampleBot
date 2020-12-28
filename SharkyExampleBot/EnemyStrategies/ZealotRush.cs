using Sharky;
using Sharky.EnemyStrategies;
using Sharky.Managers;

namespace SharkyExampleBot.EnemyStrategies
{
    public class ZealotRush : EnemyStrategy
    {
        public ZealotRush(EnemyStrategyHistory enemyStrategyHistory, IChatManager chatManager, UnitCountService unitCountService, SharkyOptions sharkyOptions)
        {
            EnemyStrategyHistory = enemyStrategyHistory;
            ChatManager = chatManager;
            UnitCountService = unitCountService;
            SharkyOptions = sharkyOptions;
        }

        protected override bool Detect(int frame)
        {
            if (UnitCountService.EnemyCount(UnitTypes.PROTOSS_ZEALOT) >= 5 && frame < SharkyOptions.FramesPerSecond * 4 * 60)
            {
                return true;
            }

            return false;
        }
    }
}
