// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Tests.Services.HistoricalAccess
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using Azure.IIoT.OpcUa.Publisher.Services;
    using Azure.IIoT.OpcUa.Publisher.Testing.Fixtures;
    using Azure.IIoT.OpcUa.Publisher.Testing.Tests;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(ReadCollection.Name)]
    public class ReadProcessedTests
    {
        public ReadProcessedTests(HistoricalAccessServer server, ITestOutputHelper output)
        {
            _server = server;
            _output = output;
        }

        private HistoryReadValuesProcessedTests<ConnectionModel> GetTests()
        {
            return new HistoryReadValuesProcessedTests<ConnectionModel>(_server,
                () => new HistoryServices<ConnectionModel>(
                    new NodeServices<ConnectionModel>(_server.Client,
                    _output.BuildLoggerFor<NodeServices<ConnectionModel>>(Logging.Level))),
                _server.GetConnection());
        }

        private readonly HistoricalAccessServer _server;
        private readonly ITestOutputHelper _output;

        [Fact]
        public Task HistoryReadUInt64ProcessedValuesTest1Async()
        {
            return GetTests().HistoryReadUInt64ProcessedValuesTest1Async();
        }

        [Fact]
        public Task HistoryReadUInt64ProcessedValuesTest2Async()
        {
            return GetTests().HistoryReadUInt64ProcessedValuesTest2Async();
        }

        [Fact]
        public Task HistoryReadUInt64ProcessedValuesTest3Async()
        {
            return GetTests().HistoryReadUInt64ProcessedValuesTest3Async();
        }

        [Fact]
        public Task HistoryStreamUInt64ProcessedValuesTest1Async()
        {
            return GetTests().HistoryStreamUInt64ProcessedValuesTest1Async();
        }

        [Fact]
        public Task HistoryStreamUInt64ProcessedValuesTest2Async()
        {
            return GetTests().HistoryStreamUInt64ProcessedValuesTest2Async();
        }

        [Fact]
        public Task HistoryStreamUInt64ProcessedValuesTest3Async()
        {
            return GetTests().HistoryStreamUInt64ProcessedValuesTest3Async();
        }
    }
}
