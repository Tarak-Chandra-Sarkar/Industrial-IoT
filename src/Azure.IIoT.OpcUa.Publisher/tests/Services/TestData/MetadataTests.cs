// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Tests.Services.TestData
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using Azure.IIoT.OpcUa.Publisher.Services;
    using Azure.IIoT.OpcUa.Publisher.Testing.Fixtures;
    using Azure.IIoT.OpcUa.Publisher.Testing.Tests;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(ReadCollection.Name)]
    public class MetadataTests
    {
        public MetadataTests(TestDataServer server, ITestOutputHelper output)
        {
            _server = server;
            _output = output;
        }

        private NodeMetadataTests<ConnectionModel> GetTests()
        {
            return new NodeMetadataTests<ConnectionModel>(
                () => new NodeServices<ConnectionModel>(_server.Client,
                    _output.BuildLoggerFor<NodeServices<ConnectionModel>>(Logging.Level)),
                _server.GetConnection());
        }

        private readonly TestDataServer _server;
        private readonly ITestOutputHelper _output;

        [Fact]
        public Task GetServerCapabilitiesTestAsync()
        {
            return GetTests().GetServerCapabilitiesTestAsync();
        }

        [Fact]
        public Task HistoryGetServerCapabilitiesTestAsync()
        {
            return GetTests().HistoryGetServerCapabilitiesTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForFolderTypeTestAsync()
        {
            return GetTests().NodeGetMetadataForFolderTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForServerObjectTestAsync()
        {
            return GetTests().NodeGetMetadataForServerObjectTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForConditionTypeTestAsync()
        {
            return GetTests().NodeGetMetadataForConditionTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataTestForBaseEventTypeTestAsync()
        {
            return GetTests().NodeGetMetadataTestForBaseEventTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForBaseInterfaceTypeTestAsync()
        {
            return GetTests().NodeGetMetadataForBaseInterfaceTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForBaseDataVariableTypeTestAsync()
        {
            return GetTests().NodeGetMetadataForBaseDataVariableTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForPropertyTypeTestAsync()
        {
            return GetTests().NodeGetMetadataForPropertyTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForAudioVariableTypeTestAsync()
        {
            return GetTests().NodeGetMetadataForAudioVariableTypeTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForServerStatusVariableTestAsync()
        {
            return GetTests().NodeGetMetadataForServerStatusVariableTestAsync();
        }

        [Fact]
        public Task NodeGetMetadataForRedundancySupportPropertyTestAsync()
        {
            return GetTests().NodeGetMetadataForRedundancySupportPropertyTestAsync();
        }
    }
}
