using SC2APIProtocol;
using Sharky;
using Sharky.DefaultBot;
using SharkyExampleBot.Micro;
using SharkyExampleBot.Tasks;
using System;

namespace SharkyExampleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // first we need to create a game connection for the SC2 api. The bot uses this to communicate with the game
            var gameConnection = new GameConnection();

            // We get a default bot that has everything setup. You can create one from scratch instead if you want to more heavily customize it.  
            var defaultSharkyBot = new DefaultSharkyBot(gameConnection);

            var debug = false;
#if DEBUG
            debug = true; // if you run in debug mode you can see useful information on the game screen
#endif
            defaultSharkyBot.SharkyOptions.Debug = debug;

            // we configure the bot with our own builds
            defaultSharkyBot.BuildChoices[Race.Protoss] = MyBuildChoices.GetProtossBuildChoices(defaultSharkyBot);
            defaultSharkyBot.BuildChoices[Race.Zerg] = MyBuildChoices.GetZergBuildChoices(defaultSharkyBot);
            defaultSharkyBot.BuildChoices[Race.Terran] = MyBuildChoices.GetTerranBuildChoices(defaultSharkyBot);

            // we add custom micro
            defaultSharkyBot.MicroData.IndividualMicroControllers[UnitTypes.TERRAN_MARINE] = new MyMarineMicroController(defaultSharkyBot.MapDataService, defaultSharkyBot.SharkyUnitData, defaultSharkyBot.ActiveUnitData, defaultSharkyBot.DebugService, defaultSharkyBot.SharkySimplePathFinder, defaultSharkyBot.BaseData, defaultSharkyBot.SharkyOptions, defaultSharkyBot.DamageService, defaultSharkyBot.UnitDataService, MicroPriority.LiveAndAttack, false);
            defaultSharkyBot.MicroData.IndividualMicroControllers[UnitTypes.TERRAN_BANSHEE] = new MyBansheeMicroController(defaultSharkyBot.MapDataService, defaultSharkyBot.SharkyUnitData, defaultSharkyBot.ActiveUnitData, defaultSharkyBot.DebugService, defaultSharkyBot.SharkySimplePathFinder, defaultSharkyBot.BaseData, defaultSharkyBot.SharkyOptions, defaultSharkyBot.DamageService, defaultSharkyBot.UnitDataService, MicroPriority.LiveAndAttack, false);

            // we add custom tasks
            var bansheeHarassController = new MyBansheeMicroController(defaultSharkyBot.MapDataService, defaultSharkyBot.SharkyUnitData, defaultSharkyBot.ActiveUnitData, defaultSharkyBot.DebugService, defaultSharkyBot.SharkyPathFinder, defaultSharkyBot.BaseData, defaultSharkyBot.SharkyOptions, defaultSharkyBot.DamageService, defaultSharkyBot.UnitDataService, MicroPriority.LiveAndAttack, false); // we use SharkyPathFinder for the harass micro, a more resource intensive pathing controller that should be used only for a few units at once
            defaultSharkyBot.MicroTaskData.MicroTasks["MyBansheeHarassTask"] = new MyBansheeHarassTask(defaultSharkyBot.BaseData, defaultSharkyBot.TargetingData, defaultSharkyBot.MapDataService, bansheeHarassController);

            // we add our custom enemy strategies
            var zealotRush = new EnemyStrategies.ZealotRush(defaultSharkyBot.EnemyStrategyHistory, defaultSharkyBot.ChatService, defaultSharkyBot.UnitCountService, defaultSharkyBot.SharkyOptions);
            defaultSharkyBot.EnemyData.EnemyStrategies[zealotRush.Name()] = zealotRush;

            // we create a bot with the modified default bot we made
            var sharkyExampleBot = defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugService);

            var myRace = Race.Random; // if you change your bot's race make sure you also update ladderbots.json if you're using it on the ladder
            if (args.Length == 0)
            {
                // if we just run the program without arguments we'll play against a random built in AI
                gameConnection.RunSinglePlayer(sharkyExampleBot, @"AutomatonLE.SC2Map", myRace, Race.Random, Difficulty.VeryHard, AIBuild.RandomBuild).Wait();
            }
            else
            {
                // when a bot runs on the ladder it will pass arguments for a specific map, enemy, etc.
                gameConnection.RunLadder(sharkyExampleBot, myRace, args).Wait();
            }
        }
    }
}
