using System;
using System.Collections.Generic;
using System.Text;
using Drexel.Configurables.External;

namespace Drexel.Configurables.Serialization.Json.Newtonsoft.Tests
{
    public class MockPathInteractor : IPathInteractor
    {
        public static MockPathInteractor Instance { get; } = new MockPathInteractor();

        public string GetFullPath(string path) => path;

        public bool IsPathRooted(string path) => true;
    }
}
