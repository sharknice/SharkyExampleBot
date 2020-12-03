using SC2APIProtocol;
using Sharky;
using Sharky.Builds;
using Sharky.Builds.Protoss;
using Sharky.Builds.Terran;
using Sharky.Builds.Zerg;
using Sharky.DefaultBot;
using SharkyExampleBot.Builds;
using System;
using System.Collections.Generic;

namespace SharkyExampleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // first we need to create a game connection for the SC2 api. The bot uses this to communicate with the game
            var gameConnection = new GameConnection();

            // We get a default bot that has everything setup.  You can manually create one instead if you want to more heavily customize it.  
            var defaultSharkyBot = new DefaultSharkyBot(gameConnection);

            var debug = false;
#if DEBUG
            debug = true;
#endif
            defaultSharkyBot.SharkyOptions.Debug = debug;

            // we configure the bot with our own builds
            defaultSharkyBot.BuildChoices[Race.Protoss] = GetProtossBuildChoices(defaultSharkyBot);
            defaultSharkyBot.BuildChoices[Race.Zerg] = GetZergBuildChoices(defaultSharkyBot);
            defaultSharkyBot.BuildChoices[Race.Terran] = GetTerranBuildChoices(defaultSharkyBot);

            // we create a bot with the modified default bot we made
            var sharkyExampleBot = defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugManager);

            var myRace = Race.Random;
            if (args.Length == 0)
            {
                // if there are no arguments passed we play against a comptuer opponent
                gameConnection.RunSinglePlayer(sharkyExampleBot, @"AutomatonLE.SC2Map", myRace, Race.Random, Difficulty.VeryHard).Wait();
            }
            else
            {
                // when a bot runs on the ladder it will pass arguments for a specific map, enemy, etc.
                gameConnection.RunLadder(sharkyExampleBot, myRace, args).Wait();
            }
        }

        static BuildChoices GetProtossBuildChoices(DefaultSharkyBot defaultSharkyBot)
        {
            // we can use this to switch builds mid-game if we detect certain strategies
            var protossCounterTransitioner = new ProtossCounterTransitioner(defaultSharkyBot.EnemyStrategyManager, defaultSharkyBot.SharkyOptions);

            // We create all of our builds
            var proxyVoidRay = new ProxyVoidRay(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager, defaultSharkyBot.NexusManager, defaultSharkyBot.SharkyOptions, defaultSharkyBot.MicroManager, protossCounterTransitioner, defaultSharkyBot.UnitDataManager, defaultSharkyBot.ProxyLocationService);
            var zealotRush = new ZealotRush(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager, defaultSharkyBot.NexusManager, protossCounterTransitioner);
            var robo = new Robo(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager, defaultSharkyBot.NexusManager, defaultSharkyBot.EnemyRaceManager, defaultSharkyBot.MicroManager, protossCounterTransitioner);
            var nexusFirst = new NexusFirst(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager, defaultSharkyBot.NexusManager, protossCounterTransitioner);
            var protossRobo = new ProtossRobo(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager, defaultSharkyBot.NexusManager, defaultSharkyBot.SharkyOptions, defaultSharkyBot.MicroManager, defaultSharkyBot.EnemyRaceManager, protossCounterTransitioner);

            // We add all the builds to a build dictionary which we will later pass to the BuildChoices. 
            var protossBuilds = new Dictionary<string, ISharkyBuild>
            {
                [proxyVoidRay.Name()] = proxyVoidRay,
                [zealotRush.Name()] = zealotRush,
                [robo.Name()] = robo,
                [nexusFirst.Name()] = nexusFirst,
                [protossRobo.Name()] = protossRobo,
            };

            // we create build sequences to be used by each matchup
            var defaultSequences = new List<List<string>>
            {
                new List<string> { nexusFirst.Name(), robo.Name(), protossRobo.Name() },
                new List<string> { proxyVoidRay.Name() }
            };
            var zergSequences = new List<List<string>>
            {
                new List<string> { zealotRush.Name() },
                new List<string> { proxyVoidRay.Name() }
            };
            var transitionSequences = new List<List<string>>
            {
                new List<string> { robo.Name(), protossRobo.Name() }
            };
            var protossBuildSequences = new Dictionary<string, List<List<string>>>
            {
                [Race.Terran.ToString()] = defaultSequences,
                [Race.Zerg.ToString()] = zergSequences,
                [Race.Protoss.ToString()] = defaultSequences,
                [Race.Random.ToString()] = defaultSequences,
                ["Transition"] = transitionSequences
            };

            return new BuildChoices { Builds = protossBuilds, BuildSequences = protossBuildSequences };
        }

        static BuildChoices GetZergBuildChoices(DefaultSharkyBot defaultSharkyBot)
        {
            var zerglingRush = new BasicZerglingRush(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager, defaultSharkyBot.MicroManager);
            var mutaliskRush = new MutaliskRush(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager);

            var zergBuilds = new Dictionary<string, ISharkyBuild>
            {
                [zerglingRush.Name()] = zerglingRush,
                [mutaliskRush.Name()] = mutaliskRush
            };

            var defaultSequences = new List<List<string>>
            {
                new List<string> { zerglingRush.Name(), mutaliskRush.Name() },
                new List<string> { mutaliskRush.Name() },
            };

            var buildSequences = new Dictionary<string, List<List<string>>>
            {
                [Race.Terran.ToString()] = defaultSequences,
                [Race.Zerg.ToString()] = defaultSequences,
                [Race.Protoss.ToString()] = defaultSequences,
                [Race.Random.ToString()] = defaultSequences,
                ["Transition"] = defaultSequences
            };

            return new BuildChoices { Builds = zergBuilds, BuildSequences = buildSequences };
        }

        static BuildChoices GetTerranBuildChoices(DefaultSharkyBot defaultSharkyBot)
        {
            var massMarines = new MassMarines(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager);
            var bansheesAndMarines = new BansheesAndMarines(defaultSharkyBot.BuildOptions, defaultSharkyBot.MacroData, defaultSharkyBot.UnitManager, defaultSharkyBot.AttackData, defaultSharkyBot.ChatManager);

            var builds = new Dictionary<string, ISharkyBuild>
            {
                [massMarines.Name()] = massMarines,
                [bansheesAndMarines.Name()] = bansheesAndMarines
            };

            var defaultSequences = new List<List<string>>
            {
                new List<string> { massMarines.Name(), bansheesAndMarines.Name() },
                new List<string> { bansheesAndMarines.Name() }
            };

            var buildSequences = new Dictionary<string, List<List<string>>>
            {
                [Race.Terran.ToString()] = defaultSequences,
                [Race.Zerg.ToString()] = defaultSequences,
                [Race.Protoss.ToString()] = defaultSequences,
                [Race.Random.ToString()] = defaultSequences,
                ["Transition"] = defaultSequences
            };

            return new BuildChoices { Builds = builds, BuildSequences = buildSequences };
        }
    }
}
