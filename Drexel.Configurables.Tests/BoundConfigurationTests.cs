using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Tests.Mocks;
using Drexel.Configurables.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Configurables.Tests
{
    [TestClass]
    public class BoundConfigurationTests
    {
        [TestMethod]
        public void BoundConfiguration_Ctor_Succeeds()
        {
            const int requirementCount = 50;

            IEnumerable<IConfigurationRequirement> requirements =
                TestUtil.CreateIConfigurationRequirementCollection(requirementCount, true);
            MockConfigurable configurable = new MockConfigurable(requirements);

            Dictionary<IConfigurationRequirement, object> validObjects =
                requirements
                    .Select(x =>
                        new KeyValuePair<IConfigurationRequirement, object>(
                            x,
                            TestUtil.GetDefaultValidObjectForRequirement(x)))
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value);

            BoundConfiguration boundConfiguration = new BoundConfiguration(configurable, validObjects);
            Assert.IsNotNull(boundConfiguration);
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_ConfigurableNull_ThrowsArgumentNull()
        {
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                new BoundConfiguration(null, new Dictionary<IConfigurationRequirement, object>()));
            StringAssert.Contains(exception.Message, "Parameter name: configurable");
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_BindingsNull_ThrowsArgumentNull()
        {
            ArgumentNullException exception = Assert.ThrowsException<ArgumentNullException>(() =>
                new BoundConfiguration(
                    new MockConfigurable(new IConfigurationRequirement[0]),
                    null));
            StringAssert.Contains(exception.Message, "Parameter name: bindings");
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_ConfigurableRequirementsNull_ThrowsArgument()
        {
            ArgumentException exception = Assert.ThrowsException<ArgumentException>(() =>
                new BoundConfiguration(
                    new MockConfigurable(null),
                    new Dictionary<IConfigurationRequirement, object>()));
            StringAssert.Contains(exception.Message, BoundConfiguration.ConfigurableRequirementsMustNotBeNull);
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_RequiredRequirementIsNotPresent_ThrowsAggregate()
        {
            IConfigurationRequirement required =
                TestUtil.CreateConfigurationRequirement(isOptional: false);

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(required);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new BoundConfiguration(configurable, new Dictionary<IConfigurationRequirement, object>()));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            StringAssert.Contains(exception.InnerExceptions.Single().Message, "Missing required requirement");
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_RequirementFailsValidation_ThrowsAggregate()
        {
            const string exceptionMessage = "BOUNDCONFIGURATIONTESTS_RequirementFailsValidation";

            IConfigurationRequirement required = TestUtil.CreateConfigurationRequirement(
                validator: (x, y, z) => throw new NotImplementedException(exceptionMessage));

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(required);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new BoundConfiguration(
                    configurable,
                    new Dictionary<IConfigurationRequirement, object>()
                    {
                        [required] = "dontCare"
                    }));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            Assert.AreEqual(exceptionMessage, exception.InnerExceptions.Single().Message);
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_RequirementDependsOnMissing_ThrowsAggregate()
        {
            // dependedUpon needs to be optional or we'll get two errors (because required requirement is missing).
            IConfigurationRequirement dependedUpon = TestUtil.CreateConfigurationRequirement(isOptional: true);
            IConfigurationRequirement dependsOn = TestUtil.CreateConfigurationRequirement(
                type: ConfigurationRequirementType.String,
                dependsOn: new IConfigurationRequirement[] { dependedUpon });

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(dependedUpon, dependsOn);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new BoundConfiguration(
                    configurable,
                    new Dictionary<IConfigurationRequirement, object>()
                    {
                        [dependsOn] = "Passes validation"
                    }));
            Assert.AreEqual(1, exception.InnerExceptions.Count);
            StringAssert.Contains(exception.InnerExceptions.Single().Message, "does not have its dependencies fulfilled.");
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_ExclusiveRequirementsSupplied_ThrowsAggregate()
        {
            IConfigurationRequirement first = TestUtil.CreateConfigurationRequirement(
                type: ConfigurationRequirementType.String);
            IConfigurationRequirement second = TestUtil.CreateConfigurationRequirement(
                type: ConfigurationRequirementType.String,
                exclusiveWith: new IConfigurationRequirement[] { first });

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(first, second);

            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new BoundConfiguration(
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
        public void BoundConfiguration_Ctor_RequiredDependsOnMissing_MultipleMissing_ThrowsAggregate()
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

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(dependsOns);
            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new BoundConfiguration(configurable, values));
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
        public void BoundConfiguration_Ctor_ExclusiveRequirementsSupplied_Multiple_ThrowsAggregate()
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

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(requirements.ToArray());
            AggregateException exception = Assert.ThrowsException<AggregateException>(() =>
                new BoundConfiguration(configurable, values));
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
        public void BoundConfiguration_Ctor_MultipleFailuresOccur_ThrowsAggregateException()
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

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(
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
                new BoundConfiguration(configurable, bindings));
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
        public void BoundConfiguration_Ctor_DependsOnChains_Succeeds()
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

            MockConfigurable configurable = new MockConfigurable(new IConfigurationRequirement[] { parent, child });
            BoundConfiguration configuration = new BoundConfiguration(configurable, supplied);

            Assert.IsNotNull(configuration);
            CollectionAssert.AreEquivalent(
                supplied.Select(x => new Binding(x.Key, x.Value)).ToArray(),
                configuration.Bindings.ToArray());
        }

        [TestMethod]
        public void BoundConfiguration_Ctor_DependsOn_PropagatesDependencies()
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

            MockConfigurable configurable = new MockConfigurable(
                new IConfigurationRequirement[]
                {
                    parent1,
                    parent2,
                    child1,
                    child2,
                    child3
                });
            BoundConfiguration configuration = new BoundConfiguration(configurable, supplied);

            Assert.IsNotNull(configuration);
            CollectionAssert.AreEquivalent(
                supplied.Select(x => new Binding(x.Key, x.Value)).ToArray(),
                configuration.Bindings.ToArray());
        }

        [TestMethod]
        public void BoundConfiguration_Bindings_Succeeds()
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
            IBinding[] expected = supplied.Select(x => new Binding(x.Key, x.Value)).ToArray();

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(requirements);

            IBoundConfiguration configuration = new BoundConfiguration(configurable, supplied);
            IBinding[] actual = configuration.Bindings.ToArray();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void BoundConfiguration_Indexer_Get_Succeeds()
        {
            IConfigurationRequirement requirement = TestUtil.CreateConfigurationRequirement();
            object expected = TestUtil.GetDefaultValidObjectForRequirement(requirement);
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [requirement] = expected
                };

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(requirement);

            BoundConfiguration configuration = new BoundConfiguration(configurable, bindings);

            Assert.AreEqual(expected, configuration[requirement]);
        }

        [TestMethod]
        public void BoundConfiguration_GetOrDefault_RequirementNull_ThrowsArgumentNull()
        {
            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(() => configuration.GetOrDefault(null, () => null));
        }

        [TestMethod]
        public void BoundConfiguration_GetOrDefault_ValueFactoryNull_ThrowsArgumentNull()
        {
            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(() => configuration.GetOrDefault(requirement, null));
        }

        [TestMethod]
        public void BoundConfiguration_GetOrDefault_Succeeds()
        {
            object expected = null;
            Func<IConfigurationRequirement, object> valueFactory =
                x =>
                {
                    expected = TestUtil.GetDefaultValidObjectForRequirement(x);
                    return expected;
                };

            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                valueFactory,
                out IConfigurationRequirement requirement);

            Assert.AreEqual(expected, configuration.GetOrDefault(requirement, () => null));
        }

        [TestMethod]
        public void BoundConfiguration_GetOrDefault_ValueNotPresent_UsesValueFactory()
        {
            const string fallbackValue = "BOUNDCONFIGURATION_GetOrDefault";

            IConfigurationRequirement present = TestUtil.CreateConfigurationRequirement();
            IConfigurationRequirement notPresent = TestUtil.CreateConfigurationRequirement(isOptional: true);
            object expected = TestUtil.GetDefaultValidObjectForRequirement(present);
            Dictionary<IConfigurationRequirement, object> bindings =
                new Dictionary<IConfigurationRequirement, object>()
                {
                    [present] = expected
                };

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(notPresent);

            BoundConfiguration configuration = new BoundConfiguration(configurable, bindings);

            Assert.AreEqual(fallbackValue, configuration.GetOrDefault(notPresent, () => fallbackValue));
        }

        [TestMethod]
        public void BoundConfiguration_TryGetOrDefault_RequirementNull_ThrowsArgumentNull()
        {
            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(
                () => configuration.TryGetOrDefault<object>(null, () => null, out object dontCare));
        }

        [TestMethod]
        public void BoundConfiguration_TryGetOrDefault_ValueFactoryNull_ThrowsArgumentNull()
        {
            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement);
            Assert.ThrowsException<ArgumentNullException>(
                () => configuration.TryGetOrDefault<object>(requirement, null, out object dontCare));
        }

        [TestMethod]
        public void BoundConfiguration_TryGetOrDefault_Succeeds()
        {
            // The type here needs to match the expected generic type used later.
            ConfigurationRequirementType type = ConfigurationRequirementType.Bool;

            bool expected = false;

            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
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
        public void BoundConfiguration_TryGetOrDefault_ValueNotPresent_UsesValueFactory()
        {
            ConfigurationRequirementType type = ConfigurationRequirementType.Uri;

            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement present);
            IConfigurationRequirement notPresent = TestUtil.CreateConfigurationRequirement(type: type);

            Uri expected = (Uri)TestUtil.GetDefaultValidObjectForRequirement(notPresent);

            Assert.IsFalse(configuration.TryGetOrDefault(notPresent, () => expected, out Uri actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BoundConfiguration_TryGetOrDefault_ValuePresentButWrongType_ConversionFails_UsesValueFactory()
        {
            const string expected = "BOUNDCONFIGURATION_TryGetOrDefault";
            
            // `expected` must be of a different type than `type`
            ConfigurationRequirementType type = ConfigurationRequirementType.Int32;

            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                TestUtil.GetDefaultValidObjectForRequirement,
                out IConfigurationRequirement requirement,
                () => TestUtil.CreateConfigurationRequirement(type: type));

            Assert.IsFalse(configuration.TryGetOrDefault<string>(requirement, () => expected, out string actual));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BoundConfiguration_TryGetOrDefault_ValuePresentButWrongType_ConversionSucceeds()
        {
            const long expected = 8675309L;
            const int notExpected = 111111;

            ConfigurationRequirementType type = ConfigurationRequirementType.Int32;

            BoundConfiguration configuration = BoundConfigurationTests.CreateConfiguration(
                x => expected,
                out IConfigurationRequirement requirement,
                () => TestUtil.CreateConfigurationRequirement(type: type));

            Assert.IsTrue(configuration.TryGetOrDefault<long>(requirement, () => notExpected, out long actual));
            Assert.AreEqual(expected, actual);
        }

        private static IConfigurable CreateConfigurable(params IConfigurationRequirement[] required) =>
            new MockConfigurable(required);

        private static BoundConfiguration CreateConfiguration(
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

            IConfigurable configurable = BoundConfigurationTests.CreateConfigurable(requirement);

            return new BoundConfiguration(configurable, bindings);
        }
    }
}
