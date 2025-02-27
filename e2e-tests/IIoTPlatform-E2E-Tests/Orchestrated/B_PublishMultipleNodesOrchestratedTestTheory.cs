// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace IIoTPlatform_E2E_Tests.Orchestrated
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Newtonsoft.Json;
    using RestSharp;
    using TestExtensions;
    using Xunit.Abstractions;
    using System.Threading;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// The test theory using different (ordered) test cases to go thru all required steps of publishing OPC UA node
    /// </summary>
    [TestCaseOrderer(TestCaseOrderer.FullName, TestConstants.TestAssemblyName)]
    [Collection("IIoT Multiple Nodes Test Collection")]
    [Trait(TestConstants.TraitConstants.PublisherModeTraitName, TestConstants.TraitConstants.PublisherModeOrchestratedTraitValue)]
    public class BPublishMultipleNodesOrchestratedTestTheory
    {
        private readonly ITestOutputHelper _output;
        private readonly IIoTMultipleNodesTestContext _context;

        public BPublishMultipleNodesOrchestratedTestTheory(IIoTMultipleNodesTestContext context, ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.OutputHelper = _output;
        }

        [Fact, PriorityOrder(40)]
        public async Task TestSetUnmanagedTagFalse()
        {
            _context.Reset();
            await TestHelper.SwitchToOrchestratedModeAsync(_context).ConfigureAwait(false);
        }

        /// <summary>
        /// <see cref="PublishSingleNodeOrchestratedTestTheory"/> has separated all the steps in different test cases
        /// For this test theory required preparation steps are combine in this single test case
        /// </summary>
        /// <returns></returns>
        [Fact, PriorityOrder(43)]
        public async Task TestRegisterOPCServerExpectSuccess()
        {
            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);

            // We will wait for microservices of IIoT platform to be healthy and modules to be deployed.
            await TestHelper.WaitForServicesAsync(_context, cts.Token).ConfigureAwait(false);
            await _context.RegistryHelper.WaitForIIoTModulesConnectedAsync(_context.DeviceConfig.DeviceId, cts.Token).ConfigureAwait(false);
            await _context.LoadSimulatedPublishedNodesAsync(cts.Token).ConfigureAwait(false);

            // Use the second OPC PLC for testing
            cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            var endpointUrl = TestHelper.GetSimulatedOpcServerUrls(_context).Skip(1).First();
            _context.OpcServerUrl = endpointUrl;
            var testPlc = _context.SimulatedPublishedNodes.Values.Skip(1).First();
            _context.ConsumedOpcUaNodes[testPlc.EndpointUrl] = IIoTMultipleNodesTestContext.GetEntryModelWithoutNodes(testPlc);
            var body = new
            {
                discoveryUrl = endpointUrl
            };

            const string route = TestConstants.APIRoutes.RegistryApplications;
            var response = TestHelper.CallRestApi(_context, Method.Post, route, body, ct: cts.Token);
            Assert.True(response.IsSuccessful, $"Got {response.StatusCode} registering {endpointUrl} discovery url");
        }

        [Fact, PriorityOrder(44)]
        public async Task TestGetApplicationsFromRegistryExpectOneRegisteredApplication()
        {
            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            dynamic json = await TestHelper.Discovery.WaitForDiscoveryToBeCompletedAsync(
                _context, new HashSet<string> { _context.OpcServerUrl }, cts.Token).ConfigureAwait(false);
            Assert.True(json != null, $"OPC Application with url {_context.OpcServerUrl} not found");
        }

        [Fact, PriorityOrder(45)]
        public async Task TestGetEndpointsExpectOneWithMultipleAuthentication()
        {
            // used if running test cases separately (during development)
            if (string.IsNullOrWhiteSpace(_context.OpcServerUrl))
            {
                await TestRegisterOPCServerExpectSuccess().ConfigureAwait(false);
                Assert.False(string.IsNullOrWhiteSpace(_context.OpcServerUrl));
            }

            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            var json = await TestHelper.Discovery.WaitForEndpointDiscoveryToBeCompletedAsync(
                _context, new HashSet<string> { _context.OpcServerUrl }, "SignAndEncrypt",
                "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256", cts.Token).ConfigureAwait(false);
            Assert.NotNull(json);

            var opcServerEndpoints = ((IEnumerable<dynamic>)json.items)
                .Where(item => item.registration.endpoint.url.Trim('/') == _context.OpcServerUrl.Trim('/')
                    && item.registration.endpoint.securityMode == "SignAndEncrypt"
                    && item.registration.endpoint.securityPolicy == "http://opcfoundation.org/UA/SecurityPolicy#Basic256Sha256");

            Assert.Single(opcServerEndpoints);

            var result = opcServerEndpoints.FirstOrDefault().registration;
            var endpointId = (string)result.id;
            Assert.NotEmpty(endpointId);

            // Authentication Checks
            Assert.Equal("None", result.authenticationMethods[0].credentialType);
            Assert.Equal("UserName", result.authenticationMethods[1].credentialType);
            Assert.Equal("X509Certificate", result.authenticationMethods[2].credentialType);

            // Store id of endpoint for further interaction
            _context.OpcUaEndpointId = endpointId;
        }

        [Fact, PriorityOrder(46)]
        public async Task TestActivateEndpointExpectSuccess()
        {
            // Used if running test cases separately (during development)
            if (string.IsNullOrWhiteSpace(_context.OpcUaEndpointId))
            {
                await TestGetEndpointsExpectOneWithMultipleAuthentication().ConfigureAwait(false);
                Assert.False(string.IsNullOrWhiteSpace(_context.OpcUaEndpointId));
            }

            await TestHelper.Registry.ActivateEndpointAsync(_context, _context.OpcUaEndpointId).ConfigureAwait(false);
        }

        [Fact, PriorityOrder(47)]
        public async Task TestCheckIfEndpointWasActivatedExpectActivatedAndConnected()
        {
            Assert.False(string.IsNullOrWhiteSpace(_context.OpcUaEndpointId));
            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            var json = await TestHelper.Registry.WaitForEndpointToBeActivatedAsync(_context, new HashSet<string> { _context.OpcUaEndpointId }, cts.Token).ConfigureAwait(false);
            Assert.True(json != null, "OPC UA Endpoint not found");
        }

        [Fact, PriorityOrder(52)]
        public async Task TestPublishNodeWithDefaultsExpectDataAvailableAtIoTHub()
        {
            // used if running test cases separately (during development)
            if (string.IsNullOrWhiteSpace(_context.OpcUaEndpointId))
            {
                await TestGetEndpointsExpectOneWithMultipleAuthentication().ConfigureAwait(false);
                Assert.False(string.IsNullOrWhiteSpace(_context.OpcUaEndpointId));
            }

            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            var testPlc = _context.SimulatedPublishedNodes[_context.ConsumedOpcUaNodes.First().Key];

            // We will filter out bad fast and slow nodes as they drop messages by design.
            _context.ConsumedOpcUaNodes.First().Value.OpcNodes = testPlc.OpcNodes
                .Where(node => !node.Id.Contains("bad", StringComparison.OrdinalIgnoreCase))
                .Skip(250).ToList();

            var body = new
            {
                NodesToAdd = _context.ConsumedOpcUaNodes.First().Value.OpcNodes.Select(node => new
                {
                    nodeId = node.Id,
                    samplingInterval = "00:00:00.250",
                    publishingInterval = "00:00:00.500",
                }).ToArray()
            };

            var route = string.Format(CultureInfo.InvariantCulture, TestConstants.APIRoutes.PublisherBulkFormat, _context.OpcUaEndpointId);
            var response = TestHelper.CallRestApi(_context, Method.Post, route, body, ct: cts.Token);
            Assert.True(response.IsSuccessful, $"Got {response.StatusCode} starting publishing bulk");
        }

        [Fact, PriorityOrder(53)]
        public async Task TestGetListOfJobsExpectJobWithEndpointId()
        {
            // Used if running test cases separately (during development)
            if (string.IsNullOrWhiteSpace(_context.OpcUaEndpointId))
            {
                await TestGetEndpointsExpectOneWithMultipleAuthentication().ConfigureAwait(false);
                Assert.False(string.IsNullOrWhiteSpace(_context.OpcUaEndpointId));
            }

            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            var route = TestConstants.APIRoutes.PublisherJobs;
            var response = TestHelper.CallRestApi(_context, Method.Get, route, ct: cts.Token);
            Assert.True(response.IsSuccessful, $"Got {response.StatusCode} getting publishing jobs");
            dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(response.Content, new ExpandoObjectConverter());

            bool found = false;
            for (int jobIndex = 0; jobIndex < (int)json.jobs.Count; jobIndex++)
            {
                var id = (string)json.jobs[jobIndex].id;
                if (id == _context.OpcUaEndpointId)
                {
                    found = true;
                    break;
                }
            }
            Assert.True(found, "Publishing Job was not created!");
        }

        [Fact, PriorityOrder(54)]
        public async Task TestVerifyDataAvailableAtIoTHub()
        {
            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);

            // Make sure that there is no active monitoring.
            await TestHelper.StopMonitoringIncomingMessagesAsync(_context, cts.Token).ConfigureAwait(false);

            // Use test event processor to verify data send to IoT Hub (expected* set to zero as data gap analysis is not part of this test case)
            await TestHelper.StartMonitoringIncomingMessagesAsync(_context, 50, 1000, 90_000_000, cts.Token).ConfigureAwait(false);

            // Wait some time to generate events to process
            // On VM in the cloud 90 seconds were not sufficient to publish data for 250 slow nodes
            await Task.Delay(TestConstants.DefaultTimeoutInMilliseconds * 4, cts.Token).ConfigureAwait(false);
            var json = await TestHelper.StopMonitoringIncomingMessagesAsync(_context, cts.Token).ConfigureAwait(false);
            Assert.True(json.TotalValueChangesCount > 0, "No messages received at IoT Hub");
            Assert.True(json.DroppedValueCount == 0, "Dropped messages detected");
            Assert.True(json.DuplicateValueCount == 0, "Duplicate values detected");
            Assert.Equal(0U, json.DroppedSequenceCount);
            // Uncomment once bug generating duplicate sequence numbers is resolved.
            //Assert.Equal(0U, json.DuplicateSequenceCount);
            Assert.Equal(0U, json.ResetSequenceCount);

            var unexpectedNodesThatPublish = new List<string>();
            // Check that every published node is sending data
            if (_context.ConsumedOpcUaNodes != null)
            {
                var expectedNodes = new List<string>(_context.ConsumedOpcUaNodes.First().Value.OpcNodes.Select(n => n.Id));
                foreach (var property in json.ValueChangesByNodeId)
                {
                    var propertyName = property.Key;
                    var nodeId = propertyName.Split('#').Last();
                    var expected = expectedNodes.Find(n => n.EndsWith(nodeId, StringComparison.Ordinal));
                    if (expected != null)
                    {
                        expectedNodes.Remove(expected);
                    }
                    else
                    {
                        unexpectedNodesThatPublish.Add(propertyName);
                    }
                }

                expectedNodes.ForEach(_context.OutputHelper.WriteLine);
                Assert.Empty(expectedNodes);

                unexpectedNodesThatPublish.ForEach(node => _context.OutputHelper.WriteLine($"Publishing from unexpected node: {node}"));
            }
        }

        [Fact, PriorityOrder(55)]
        public async Task TestBulkUnpublishedNodesExpectSuccess()
        {
            // Used if running test cases separately (during development)
            if (string.IsNullOrWhiteSpace(_context.OpcUaEndpointId))
            {
                await TestGetEndpointsExpectOneWithMultipleAuthentication().ConfigureAwait(false);
                Assert.False(string.IsNullOrWhiteSpace(_context.OpcUaEndpointId));
            }

            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);

            var testPlc = _context.SimulatedPublishedNodes[_context.ConsumedOpcUaNodes.First().Key];

            // We will filter out bad fast and slow nodes as they drop messages by design.
            _context.ConsumedOpcUaNodes.First().Value.OpcNodes = testPlc.OpcNodes
                .Where(node => !node.Id.Contains("bad", StringComparison.OrdinalIgnoreCase))
                .Skip(250).ToList();

            var body = new
            {
                NodesToRemove = _context.ConsumedOpcUaNodes.First().Value.OpcNodes.Select(node => node.Id).ToArray()
            };

            var route = string.Format(CultureInfo.InvariantCulture, TestConstants.APIRoutes.PublisherBulkFormat, _context.OpcUaEndpointId);
            var response = TestHelper.CallRestApi(_context, Method.Post, route, body, ct: cts.Token);
            Assert.True(response.IsSuccessful, $"Got {response.StatusCode} starting publishing bulk");
        }

        [Fact, PriorityOrder(56)]
        public async Task TestVerifyNoDataIncomingAtIoTHub()
        {
            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);

            // Wait untill the publishing has stopped
            await Task.Delay(TestConstants.DefaultTimeoutInMilliseconds * 4, cts.Token).ConfigureAwait(false);

            // Make sure that there is no active monitoring.
            await TestHelper.StopMonitoringIncomingMessagesAsync(_context, cts.Token).ConfigureAwait(false);

            // Use test event processor to verify data send to IoT Hub (expected* set to zero as data gap analysis is not part of this test case)
            await TestHelper.StartMonitoringIncomingMessagesAsync(_context, 0, 0, 0, cts.Token).ConfigureAwait(false);

            // Wait some time to generate events to process
            await Task.Delay(TestConstants.DefaultTimeoutInMilliseconds, cts.Token).ConfigureAwait(false);
            var json = await TestHelper.StopMonitoringIncomingMessagesAsync(_context, cts.Token).ConfigureAwait(false);
            Assert.True(json.TotalValueChangesCount == 0, $"{json.TotalValueChangesCount} Messages received at IoT Hub");
        }

        [Fact, PriorityOrder(57)]
        public async Task RemoveJobExpectSuccess()
        {
            // Used if running test cases separately (during development)
            if (string.IsNullOrWhiteSpace(_context.OpcUaEndpointId))
            {
                await TestGetEndpointsExpectOneWithMultipleAuthentication().ConfigureAwait(false);
                Assert.False(string.IsNullOrWhiteSpace(_context.OpcUaEndpointId));
            }

            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            var route = string.Format(CultureInfo.InvariantCulture, TestConstants.APIRoutes.PublisherJobsFormat, _context.OpcUaEndpointId);
            var response = TestHelper.CallRestApi(_context, Method.Delete, route, ct: cts.Token);
            Assert.True(response.IsSuccessful, $"Got {response.StatusCode} deleting publishing job");
        }

        [Fact, PriorityOrder(58)]
        public async Task TestRemoveAllApplications()
        {
            var cts = new CancellationTokenSource(TestConstants.MaxTestTimeoutMilliseconds);
            await TestHelper.Registry.RemoveAllApplicationsAsync(_context, ct: cts.Token).ConfigureAwait(false);
        }
    }
}
