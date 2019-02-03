using System;
using System.Collections.Generic;
using System.Text;
using Drexel.Configurables.Contracts.Relations;

namespace Drexel.Configurables.Contracts.Configurations
{
#nullable enable
    public sealed class ConfigurationBuilder
    {
        private readonly RequirementRelations relations;

        public ConfigurationBuilder(RequirementRelations relations)
        {
            this.relations = relations ?? throw new ArgumentNullException(nameof(relations));
        }

        public ConfigurationBuilder Add(Requirement requirement, object? value)
        {
            // TODO: Where's muh type safety?
            throw new NotImplementedException();
        }

        public Configuration Build()
        {
            throw new NotImplementedException();
        }
    }
#nullable disable
}
