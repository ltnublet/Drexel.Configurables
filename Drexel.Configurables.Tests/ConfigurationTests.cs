using System;
using System.Collections.Generic;
using System.Linq;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Mocks;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void Configuration_Ctor_Succeeds()
        {
            const int requirementCount = 50;

            IEnumerable<IConfigurationRequirement> requirements =
                TestUtil.CreateIConfigurationRequirementCollection(requirementCount, true);
            MockRequirementSource configurable = new MockRequirementSource(requirements);

            Dictionary<IConfigurationRequirement, object> validObjects =
                requirements
                    .Select(x =>
                        new KeyValuePair<IConfigurationRequirement, object>(
                            x,
                            TestUtil.GetDefaultValidObjectForRequirement(x)))
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value);

            Configuration boundConfiguration = new Configuration(configurable, validObjects);
            Assert.IsNotNull(boundConfiguration);
        }

        [TestMethod]
        public void Configuration_Ctor_MissingOptionalRequirement_Succeeds()
        {
            const int requirementCount = 50;

            IEnumerable<IConfigurationRequirement> requirements =
                TestUtil.CreateIConfigurationRequirementCollection(requirementCount, true, areOptional: true);
            MockRequirementSource configurable = new MockRequirementSource(requirements);

            Dictionary<IConfigurationRequirement, object> validObjects =
                requirements
                    .Select(x =>
                        new KeyValuePair<IConfigurationRequirement, object>(
                            x,
                            TestUtil.GetDefaultValidObjectForRequirement(x)))
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value);

            // Remove one of the requirements that we're supplying to the constructor. Because they're all optional,
            // this shouldn't cause any problems.
            validObjects.Remove(validObjects.First().Key);

            Configuration boundConfiguration = new Configuration(configurable, validObjects);
            Assert.IsNotNull(boundConfiguration);
        }

        [TestMethod]
        public void Configuration_Ctor_RequirementSourceNull_ThrowsArgumentNull()
        {
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                new Configuration(null, new Dictionary<IConfigurationRequirement, object>()));
            StringAssert.Contains(exception.Message, "Parameter name: requirementSource");
        }

        [TestMethod]
        public void Configuration_Ctor_MappingsNull_ThrowsArgumentNull()
        {
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                new Configuration(
                    new MockRequirementSource(new IConfigurationRequirement[0]),
                    null));
            StringAssert.Contains(exception.Message, "Parameter name: mappings");
        }

        [TestMethod]
        public void Configuration_Ctor_ConfigurableRequirementsNull_ThrowsArgument()
        {
            InvalidRequirementsException exception = Assert.ThrowsException<InvalidRequirementsException>(() =>
                new Configuration(
                    new MockRequirementSource(null),
                    new Dictionary<IConfigurationRequirement, object>()));
            StringAssert.Contains(exception.Message, Configuration.ConfigurationRequirementsMustNotBeNull);
        }

        [TestMethod]
        public void Configuration_Ctor_RequiredRequirementIsNotPresent_ThrowsAggregate()
        {
            IConfigurationRequirement required =
                TestUtil.CreateConfigurationRequirement(isOptional: false);

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(required);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(configurable, new Dictionary<IConfigurationRequirement, object>()));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            StringAssert.Contains(exception.InnerExceptions.Single().Message, "Missing required requirement");
        }

        [TestMethod]
        public void Configuration_Ctor_RequirementFailsValidation_ThrowsAggregate()
        {
            const string exceptionMessage = "BOUNDCONFIGURATIONTESTS_RequirementFailsValidation";

            IConfigurationRequirement required = TestUtil.CreateConfigurationRequirement(
                validator: (x, y, z) => throw new NotImplementedException(exceptionMessage));

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(required);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(
                    configurable,
                    new Dictionary<IConfigurationRequirement, object>()
                    {
                        [required] = "dontCare"
                    }));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            Assert.AreEqual(exceptionMessage, exception.InnerExceptions.Single().Message);
        }

        [TestMethod]
        public void Configuration_Ctor_RequirementDependsOnMissing_ThrowsAggregate()
        {
            // dependedUpon needs to be optional or we'll get two errors (because required requirement is missing).
            IConfigurationRequirement dependedUpon = TestUtil.CreateConfigurationRequirement(isOptional: true);
            IConfigurationRequirement dependsOn = TestUtil.CreateConfigurationRequirement(
                type: ConfigurationRequirementType.String,
                dependsOn: new IConfigurationRequirement[] { dependedUpon });

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(dependedUpon, dependsOn);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(
                    configurable,
                    new Dictionary<IConfigurationRequirement, object>()
                    {
                        [dependsOn] = "Passes validation"
                    }));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            StringAssert.Contains(
                exception.InnerExceptions.Single().Message,
                "does not have its dependencies fulfilled.");
        }

        [TestMethod]
        public void Configuration_Ctor_ExclusiveRequirementsSupplied_ThrowsAggregate()
        {
            IConfigurationRequirement first = TestUtil.CreateConfigurationRequirement(
                type: ConfigurationRequirementType.String);
            IConfigurationRequirement second = TestUtil.CreateConfigurationRequirement(
                type: ConfigurationRequirementType.String,
                exclusiveWith: new IConfigurationRequirement[] { first });

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(first, second);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(
                    configurable,
                    new Dictionary<IConfigurationRequirement, object>()
                    {
                        [first] = "Passes validation",
                        [second] = "Also passes validation"
                    }));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            StringAssert.Contains(
                exception.InnerExceptions.Single().Message,
                "has conflicting requirements specified.");
        }

        [TestMethod]
        [Description("Make sure that if multiple depended-upon requirements are missing, all of them get their own exception.")]
        public void Configuration_Ctor_RequiredDependsOnMissing_MultipleMissing_ThrowsAggregate()
        {
            const int numberOfMissingDependedUpons = 5;

            IConfigurationRequirement[] dependedUpons = Enumerable
                .Range(1, numberOfMissingDependedUpons)
                .Select(x => TestUtil.CreateConfigurationRequirement(isOptional: true))
                .ToArray();
            IConfigurationRequirement[] dependsOns = dependedUpons
                .Select(x => TestUtil.CreateConfigurationRequirement(dependsOn: new IConfigurationRequirement[] { x }))
                .ToArray();

            Dictionary<IConfigurationRequirement, object> values = dependsOns
                .Select(x => new KeyValuePair<IConfigurationRequirement, object>(
                    x,
                    TestUtil.GetDefaultValidObjectForRequirement(x)))
                .ToDictionary(x => x.Key, x => x.Value);

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(dependsOns);
            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(configurable, values));
            Assert.AreEqual(numberOfMissingDependedUpons, exception.InnerExceptions.Count);
            Assert.IsTrue(
                exception
                    .InnerExceptions
                    .Select(x => x.Message)
                    .All(x => x.Contains("does not have its dependencies fulfilled.")),
                "Unexpected exception message encountered.");
        }

        [TestMethod]
        [Description("Make sure that if multiple exclusive requirements are supplied, all of them get their own exception.")]
        public void Configuration_Ctor_ExclusiveRequirementsSupplied_Multiple_ThrowsAggregate()
        {
            const int numberOfConflictingRequirements = 5;

            List<IConfigurationRequirement> requirements =
                new List<IConfigurationRequirement>()
                {
                    TestUtil.CreateConfigurationRequirement()
                };

            for (int counter = 0; counter < numberOfConflictingRequirements; counter++)
            {
                requirements.Add(TestUtil.CreateConfigurationRequirement(
                    exclusiveWith: new IConfigurationRequirement[] { requirements.Last() }));
            }

            Dictionary<IConfigurationRequirement, object> values = requirements
                .Select(x => new KeyValuePair<IConfigurationRequirement, object>(
                    x,
                    TestUtil.GetDefaultValidObjectForRequirement(x)))
                .ToDictionary(x => x.Key, x => x.Value);

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(requirements.ToArray());
            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(configurable, values));
            Assert.AreEqual(numberOfConflictingRequirements, exception.InnerExceptions.Count);
            Assert.IsTrue(
                exception
                    .InnerExceptions
                    .Select(x => x.Message)
                    .All(x => x.Contains("has conflicting requirements specified.")),
                "Unexpected exception message encountered.");
        }

        [TestMethod]
        [Description("Check that when multiple different types of failures happen, they all get returned.")]
        public void Configuration_Ctor_MultipleFailuresOccur_ThrowsAggregateException()
        {
            const string validationFailureMessage = "BOUNDCONFIGURATIONTESTS_MultipleFailuresOccur";
            const string testFailureMessage = "Missing/too many matches for expected exception.";
            const int expectedFailureCount = 4;

            IConfigurationRequirement requiredButMissing = TestUtil.CreateConfigurationRequirement(
                baseName: nameof(requiredButMissing),
                isOptional: false);
            IConfigurationRequirement dependsOnIsMissing = TestUtil.CreateConfigurationRequirement(
                baseName: nameof(dependsOnIsMissing),
                dependsOn: new IConfigurationRequirement[] { requiredButMissing });
            IConfigurationRequirement failsValidation = TestUtil.CreateConfigurationRequirement(
                baseName: nameof(failsValidation),
                validator: (x, y, z) => throw new NotImplementedException(validationFailureMessage));
            IConfigurationRequirement exclusiveWith = TestUtil.CreateConfigurationRequirement(
                baseName: nameof(exclusiveWith),
                exclusiveWith: new IConfigurationRequirement[] { failsValidation });
            IConfigurationRequirement isFine = TestUtil.CreateConfigurationRequirement(
                baseName: nameof(isFine),
                validator: (x, y, z) => null);

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(
                requiredButMissing,
                dependsOnIsMissing,
                failsValidation,
                exclusiveWith,
                isFine);

            Dictionary<IConfigurationRequirement, object> bindings =
                new IConfigurationRequirement[] { dependsOnIsMissing, failsValidation, exclusiveWith, isFine }
                    .Select(x => new KeyValuePair<IConfigurationRequirement, object>(x, null))
                    .ToDictionary(x => x.Key, x => x.Value); 

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new Configuration(configurable, bindings));
            Assert.AreEqual(expectedFailureCount, exception.InnerExceptions.Count);
            Assert.IsNotNull(
                exception
                    .InnerExceptions
                    .SingleOrDefault(x => x.Message.Contains(validationFailureMessage)),
                testFailureMessage);
            Assert.IsNotNull(
                exception
                    .InnerExceptions
                    .SingleOrDefault(x => x.Message.Contains("has conflicting requirements specified.")),
                testFailureMessage);
            Assert.IsNotNull(
                exception
                    .InnerExceptions
                    .SingleOrDefault(x => x.Message.Contains("does not have its dependencies fulfilled.")),
                testFailureMessage);
            Assert.IsNotNull(
                exception
                    .InnerExceptions
                    .SingleOrDefault(x => x.Message.Contains("Missing required requirement")),
                testFailureMessage);
        }

        [TestMethod]
        public void Configuration_Ctor_DependsOnChains_Succeeds()
        {
            IConfigurationRequirement parent = TestUtil.CreateConfigurationRequirement(baseName: "Parent");
            IConfigurationRequirement child = TestUtil.CreateConfigurationRequirement(
                baseName: "Child",
                dependsOn: new IConfigurationRequirement[] { parent });

            Dictionary<IConfigurationRequirement, object> supplied =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [parent] = TestUtil.GetDefaultValidObjectForRequirement(parent),
                    [child] = TestUtil.GetDefaultValidObjectForRequirement(child)
                };

            MockRequirementSource configurable = new MockRequirementSource(new IConfigurationRequirement[] { parent, child });
            Configuration configuration = new Configuration(configurable, supplied);

            Assert.IsNotNull(configuration);
            CollectionAssert.AreEquivalent(
                supplied.Select(x => new Mapping(x.Key, x.Value)).ToArray(),
                configuration.ToArray());
        }

        [TestMethod]
        public void Configuration_Ctor_DependsOn_PropagatesDependencies()
        {
            IConfigurationRequirement parent1 = TestUtil.CreateConfigurationRequirement(baseName: "ParentOne");
            IConfigurationRequirement parent2 = TestUtil.CreateConfigurationRequirement(baseName: "ParentTwo");
            IConfigurationRequirement child1 = TestUtil.CreateConfigurationRequirement(
                baseName: "ChildOne",
                dependsOn: new IConfigurationRequirement[] { parent1 },
                validator: (x, y, z) =>
                {
                    CollectionAssert.Contains(z.Keys.ToArray(), parent1);

                    return null;
                });
            IConfigurationRequirement child2 = TestUtil.CreateConfigurationRequirement(
                baseName: "ChildTwo",
                dependsOn: new IConfigurationRequirement[] { child1 },
                validator: (x, y, z) =>
                {
                    CollectionAssert.Contains(z.Keys.ToArray(), child1);

                    return null;
                });
            IConfigurationRequirement child3 = TestUtil.CreateConfigurationRequirement(
                baseName: "ChildThree",
                dependsOn: new IConfigurationRequirement[] { parent2, child2 },
                validator: (x, y, z) =>
                {
                    CollectionAssert.Contains(z.Keys.ToArray(), parent2);
                    CollectionAssert.Contains(z.Keys.ToArray(), child2);

                    return null;
                });

            Dictionary<IConfigurationRequirement, object> supplied =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [parent1] = TestUtil.GetDefaultValidObjectForRequirement(parent1),
                    [parent2] = TestUtil.GetDefaultValidObjectForRequirement(parent2),
                    [child1] = TestUtil.GetDefaultValidObjectForRequirement(child1),
                    [child2] = TestUtil.GetDefaultValidObjectForRequirement(child2),
                    [child3] = TestUtil.GetDefaultValidObjectForRequirement(child3)
                };

            MockRequirementSource configurable = new MockRequirementSource(
                new IConfigurationRequirement[]
                {
                    parent1,
                    parent2,
                    child1,
                    child2,
                    child3
                });
            Configuration configuration = new Configuration(configurable, supplied);

            Assert.IsNotNull(configuration);
            CollectionAssert.AreEquivalent(
                supplied.Select(x => new Mapping(x.Key, x.Value)).ToArray(),
                configuration.ToArray());
        }

        [TestMethod]
        public void Configuration_Ctor_Configurator_Propagates()
        {
            MockRequirementSource source = new MockRequirementSource(Enumerable.Empty<IConfigurationRequirement>());
            MockConfigurator configurator = new MockConfigurator(source, (x, y) => null);

            Configuration configuration = new Configuration(
                source,
                new Dictionary<IConfigurationRequirement, object>(),
                configurator);

            Assert.AreSame(configurator, configuration.Configurator);
        }

        [TestMethod]
        public void Configuration_Keys_Succeeds()
        {
            const int numberOfRequirements = 50;

            IConfigurationRequirement[] requirements = TestUtil
                .CreateIConfigurationRequirementCollection(numberOfRequirements, randomTypes: true)
                .ToArray();

            Dictionary<IConfigurationRequirement, object> supplied = requirements
                .Select(x =>
                    new KeyValuePair<IConfigurationRequirement, object>(
                        x,
                        TestUtil.GetDefaultValidObjectForRequirement(x)))
                .ToDictionary(x => x.Key, x => x.Value);

            IRequirementSource requirementSource = ConfigurationTests.CreateRequirementSource(requirements);

            Configuration configuration = new Configuration(requirementSource, supplied);

            CollectionAssert.AreEquivalent(supplied.Keys, configuration.Keys.ToArray());
        }

        [TestMethod]
        public void Configuration_CanBeEnumerated_Succeeds()
        {
            const int numberOfRequirements = 50;

            IConfigurationRequirement[] requirements = TestUtil
                .CreateIConfigurationRequirementCollection(numberOfRequirements, randomTypes: true)
                .ToArray();

            Dictionary<IConfigurationRequirement, object> supplied = requirements
                .Select(x =>
                    new KeyValuePair<IConfigurationRequirement, object>(
                        x,
                        TestUtil.GetDefaultValidObjectForRequirement(x)))
                .ToDictionary(x => x.Key, x => x.Value);
            IMapping[] expected = supplied.Select(x => new Mapping(x.Key, x.Value)).ToArray();

            IRequirementSource requirementSource = ConfigurationTests.CreateRequirementSource(requirements);

            IConfiguration configuration = new Configuration(requirementSource, supplied);
            IMapping[] actual = configuration.ToArray();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Configuration_Indexer_Get_Succeeds()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();
            object expected = TestUtil.GetDefaultValidObjectForRequirement(requirement);
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [requirement] = expected
                };

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(requirement);

            Configuration configuration = new Configuration(configurable, bindings);

            Assert.AreEqual(expected, configuration[requirement]);
        }

        [TestMethod]
        public void Configuration_GetOrDefault_RequirementNull_ThrowsArgumentNull()
        {
            Configuration configuration = ConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(() => configuration.GetOrDefault(null, () => null));
        }

        [TestMethod]
        public void Configuration_GetOrDefault_ValueFactoryNull_ThrowsArgumentNull()
        {
            Configuration configuration = ConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(() => configuration.GetOrDefault(requirement, null));
        }

        [TestMethod]
        public void Configuration_GetOrDefault_Succeeds()
        {
            object expected = null;
            Func<IConfigurationRequirement, object> valueFactory =
                x =>
                {
                    expected = TestUtil.GetDefaultValidObjectForRequirement(x);
                    return expected;
                };

            Configuration configuration = ConfigurationTests.CreateConfiguration(
                valueFactory,
                out IConfigurationRequirement requirement);

            Assert.AreEqual(expected, configuration.GetOrDefault(requirement, () => null));
        }

        [TestMethod]
        public void Configuration_GetOrDefault_ValueNotPresent_UsesValueFactory()
        {
            const string fallbackValue = "Configuration_GetOrDefault";

            IConfigurationRequirement present = TestUtil.CreateConfigurationRequirement();
            IConfigurationRequirement notPresent = TestUtil.CreateConfigurationRequirement(isOptional: true);
            object expected = TestUtil.GetDefaultValidObjectForRequirement(present);
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [present] = expected
                };

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(notPresent);

            Configuration configuration = new Configuration(configurable, bindings);

            Assert.AreEqual(fallbackValue, configuration.GetOrDefault(notPresent, () => fallbackValue));
        }

        [TestMethod]
        public void Configuration_TryGetOrDefault_RequirementNull_ThrowsArgumentNull()
        {
            Configuration configuration = ConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(
                () => configuration.TryGetOrDefault<object>(null, () => null, out object dontCare));
        }

        [TestMethod]
        public void Configuration_TryGetOrDefault_ValueFactoryNull_ThrowsArgumentNull()
        {
            Configuration configuration = ConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(
                () => configuration.TryGetOrDefault<object>(requirement, null, out object dontCare));
        }

        [TestMethod]
        public void Configuration_TryGetOrDefault_Succeeds()
        {
            // The type here needs to match the expected generic type used later.
            ConfigurationRequirementType type = ConfigurationRequirementType.Bool;

            bool expected = false;

            Configuration configuration = ConfigurationTests.CreateConfiguration(
                x =>
                {
                    // Use the inverse of the default value for our expected (just in case the default somehow slips in)
                    expected = !(bool)TestUtil.GetDefaultValidObjectForRequirement(x);
                    return expected;
                },
                out IConfigurationRequirement requirement,
                () => TestUtil.CreateConfigurationRequirement(type: type));

            Assert.IsTrue(configuration.TryGetOrDefault<bool>(requirement, () => !expected, out bool actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Configuration_TryGetOrDefault_ValueNotPresent_UsesValueFactory()
        {
            ConfigurationRequirementType type = ConfigurationRequirementType.Uri;

            Configuration configuration = ConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement present);
            IConfigurationRequirement notPresent = TestUtil.CreateConfigurationRequirement(type: type);

            Uri expected = (Uri)TestUtil.GetDefaultValidObjectForRequirement(notPresent);

            Assert.IsFalse(configuration.TryGetOrDefault(notPresent, () => expected, out Uri actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Configuration_TryGetOrDefault_ValuePresentButWrongType_ConversionFails_UsesValueFactory()
        {
            const string expected = "Configuration_TryGetOrDefault";
            
            // `expected` must be of a different type than `type`
            ConfigurationRequirementType type = ConfigurationRequirementType.Int32;

            Configuration configuration = ConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement,
                () => TestUtil.CreateConfigurationRequirement(type: type));

            Assert.IsFalse(configuration.TryGetOrDefault<string>(requirement, () => expected, out string actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Configuration_TryGetOrDefault_ValuePresentButWrongType_ConversionSucceeds()
        {
            const long expected = 8675309L;
            const int notExpected = 111111;

            ConfigurationRequirementType type = ConfigurationRequirementType.Int32;

            Configuration configuration = ConfigurationTests.CreateConfiguration(
                x => expected,
                out IConfigurationRequirement requirement,
                () => TestUtil.CreateConfigurationRequirement(type: type));

            Assert.IsTrue(configuration.TryGetOrDefault<long>(requirement, () => notExpected, out long actual));
            Assert.AreEqual(expected, actual);
        }

        private static IRequirementSource CreateRequirementSource(params IConfigurationRequirement[] required) =>
            new MockRequirementSource(required);

        private static Configuration CreateConfiguration(
            Func<IConfigurationRequirement, object> valueFactory,
            out IConfigurationRequirement requirement,
            Func<IConfigurationRequirement> requirementFactory = null)
        {
            requirement = (requirementFactory ?? (() => TestUtil.CreateConfigurationRequirement())).Invoke();
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [requirement] = valueFactory.Invoke(requirement)
                };

            IRequirementSource configurable = ConfigurationTests.CreateRequirementSource(requirement);

            return new Configuration(configurable, bindings);
        }
    }
}
