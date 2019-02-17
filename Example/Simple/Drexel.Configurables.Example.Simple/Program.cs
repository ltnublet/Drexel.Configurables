using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Configurations;
using Drexel.Configurables.Contracts.Ids;
using Drexel.Configurables.Contracts.Localization;
using Drexel.Configurables.Contracts.Relations;
using Drexel.Configurables.Contracts.Structs;
using Drexel.Configurables.Json.Newtonsoft;
using Drexel.Configurables.Serialization.Json.Newtonsoft;

namespace Drexel.Configurables.Example.Simple
{
    public class Program
    {
        private static readonly Guid RequirementOneGuid = Guid.Parse("38793291-C8CC-40B1-855E-20BC8070D201");
        private static readonly Guid RequirementTwoGuid = Guid.Parse("290027C9-1427-4517-A5A5-260E0860DB56");
        private static readonly Guid RequirementThreeGuid = Guid.Parse("D42D1BAE-D7F3-4869-86F3-01FDB1D3CA59");
        private static readonly Guid RequirementFourGuid = Guid.Parse("0405F121-B6D9-452F-B446-BB8E34D9CB8A");
        private static readonly Guid RequirementFiveGuid = Guid.Parse("969811BC-B384-4462-A7CE-134B055EB071");

        private static readonly string RequirementOneName = "RequirementOne";
        private static readonly string RequirementTwoName = "RequirementTwo";
        private static readonly string RequirementThreeName = "RequirementThree";
        private static readonly string RequirementFourName = "RequirementFour";
        private static readonly string RequirementFiveName = "RequirementFive";

        private static readonly string RequirementOneDescription = "Requirement one description.";
        private static readonly string RequirementTwoDescription = "Requirement two description.";
        private static readonly string RequirementThreeDescription = "Requirement three description.";
        private static readonly string RequirementFourDescription = "Requirement four description.";
        private static readonly string RequirementFiveDescription = "Requirement five description.";

        static Program()
        {
            StructCollectionRequirementType<int> type =
                new StructCollectionRequirementType<int>();
            Program.RequirementOne = new StructCollectionRequirement<int>(type);
            Program.RequirementTwo = new StructCollectionRequirement<int>(type);
            Program.RequirementThree = new StructCollectionRequirement<int>(type);
            Program.RequirementFour = new StructCollectionRequirement<int>(type);
            Program.RequirementFive = new StructCollectionRequirement<int>(type);
        }

        private static StructCollectionRequirement<int> RequirementOne { get; }
        private static StructCollectionRequirement<int> RequirementTwo { get; }
        private static StructCollectionRequirement<int> RequirementThree { get; }
        private static StructCollectionRequirement<int> RequirementFour { get; }
        private static StructCollectionRequirement<int> RequirementFive { get; }

        public static async Task Main(string[] args)
        {
            // Assign unique IDs to each of the requirements.
            RequirementIdMapBuilder idMapBuilder = new RequirementIdMapBuilder()
                .Add(Program.RequirementOne, Program.RequirementOneGuid)
                .Add(Program.RequirementTwo, Program.RequirementTwoGuid)
                .Add(Program.RequirementThree, Program.RequirementThreeGuid)
                .Add(Program.RequirementFour, Program.RequirementFourGuid)
                .Add(Program.RequirementFive, Program.RequirementFiveGuid);
            RequirementIdMap idMap = idMapBuilder.Build();

            // Assign relations between the requirements.
            // * RequirementOne
            //   - RequirementTwo
            //     - RequirementThree
            // * RequirementFour
            //   - RequirementFive
            // (Where RequirementOne and RequirementFour are mutually exclusive.)
            RequirementRelationsBuilder relationsBuilder = new RequirementRelationsBuilder()
                .Add(Program.RequirementOne, Program.RequirementTwo, RequirementRelation.DependedUpon)
                .Add(Program.RequirementTwo, Program.RequirementThree, RequirementRelation.DependedUpon)
                .Add(Program.RequirementFour, Program.RequirementFive, RequirementRelation.DependedUpon)
                .Add(Program.RequirementOne, Program.RequirementFour, RequirementRelation.ExclusiveWith);
            RequirementRelations relations = relationsBuilder.Build();

            // Assign names and descriptions to each of the requirements.
            RequirementLocalizationDictionaryBuilder localizationBuilder =
                new RequirementLocalizationDictionaryBuilder()
                    .AddName(Program.RequirementOne, Program.RequirementOneName)
                    .AddName(Program.RequirementTwo, Program.RequirementTwoName)
                    .AddName(Program.RequirementThree, Program.RequirementThreeName)
                    .AddName(Program.RequirementFour, Program.RequirementFourName)
                    .AddName(Program.RequirementFive, Program.RequirementFiveName)
                    .AddDescription(Program.RequirementOne, Program.RequirementOneDescription)
                    .AddDescription(Program.RequirementTwo, Program.RequirementTwoDescription)
                    .AddDescription(Program.RequirementThree, Program.RequirementThreeDescription)
                    .AddDescription(Program.RequirementFour, Program.RequirementFourDescription)
                    .AddDescription(Program.RequirementFive, Program.RequirementFiveDescription);
            RequirementLocalizationDictionary localization = localizationBuilder.Build();

            // Create a configuration given the supplied requirements.
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder(relations);
            foreach (Requirement requirement in relations
                .Forest
                .SelectMany(x => x.BreadthFirstSort())
                .Select(x => x.Value))
            {
                Console.WriteLine(
                    $"Please enter value for requirement '{localization[requirement].Name}' ('{localization[requirement].Description}'):");

                object userInput = Program.ReadValue(requirement.Type);
                configurationBuilder.Add(requirement, userInput);
            }

            Configuration configuration = configurationBuilder.Build();

            // Serialize the configuration.
            MemoryStream stream = new MemoryStream();
            // TODO: Where do types come into play? They need IDs too, since we want to support migrations (where a
            // type that the user controls "steals" the ID of a pre-existing type, so that they can migrate from older
            // versions to newer version)
            using (Serializer serializer = new Serializer(stream, idMap))
            {
                await serializer.SerializeAsync(configuration);
            }

            // Deserialize the configuration.
            stream.Seek(0L, SeekOrigin.Begin);
            Configuration deserializedConfiguration = null;
            // TODO: Serialization isn't done yet, which will probably give a better idea of what information is needed
            // to deserialize.
            using (Deserializer deserializer = new Deserializer(stream, idMap))
            {
                deserializedConfiguration = await deserializer.DeserializeAsync();
            }

            // TODO: Wow, amazing, look at how the configuration object survived that round-trip!
            Console.WriteLine(configuration.Equals(deserializedConfiguration));

            Console.ReadLine();
        }

        private static object ReadValue(RequirementType type)
        {
            return null;
        }
    }
}
