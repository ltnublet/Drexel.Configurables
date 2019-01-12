using System;
using System.Collections.Generic;
using System.Text;

namespace Drexel.Configurables.Structures
{
    class RequirementGroupNode
    {
        // TODO: A `Requirement` corresponds to a single value (string, bool, whatever). An application may consist of
        // multiple configurable resources (for example, there might be 3 totally independent things that need
        // configuration, and can be configured independent of each other). Even though the resources are independent
        // from their perspective, because they all belong to a single application, the application may want to group
        // them together for the purposes of presenting them to the user.
        //
        // For example, the application may have two separate configurable concerns: network settings, and resource
        // consumption limits.
        // * "Network Settings":
        //   - Listen Port (uint16)
        //   - Certificate (X502Certificate2)
        // * "Resource Consumption Limits"
        //   - CPU Cores (uint16)
        //   - RAM (uint64)
        //
        // An application may want to nest these groups. For example, imagine an application which consists of multiple
        // services: the Analytics Engine, the Host Service, and the Indexing Service.
        // + "Analytics Engine":
        //   - CPU Cores (uint16)
        //   - RAM (uint64)
        // + "Host Service":
        //   * "Network Settings":
        //     - Listen Port (uint16)
        //     - Certificate (X502Certificate2)
        //   * "Account":
        //     - Username (string)
        //     - Password (SecureString)
        // + "Indexing Service":
        //   * "Index Location":
        //     - Folder (FilePath)
        //   * "Account":
        //     - Username (string)
        //     - Password (SecureString)
        //
        // In the above example, note that the "Analytics Engine" group directly contained requirements.
    }
}
