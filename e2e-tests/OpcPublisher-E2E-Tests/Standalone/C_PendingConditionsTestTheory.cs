﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace OpcPublisher_AE_E2E_Tests.Standalone
{
    using OpcPublisher_AE_E2E_Tests.TestExtensions;
    using OpcPublisher_AE_E2E_Tests.TestModels;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// The test theory using different (ordered) test cases to go thru all required steps of publishing OPC UA node
    /// </summary>
    [TestCaseOrderer(TestCaseOrderer.FullName, TestConstants.TestAssemblyName)]
    [Trait(TestConstants.TraitConstants.PublisherModeTraitName, TestConstants.TraitConstants.PublisherModeTraitValue)]
    public class CPendingConditionsTestTheory : DynamicAciTestBase, IClassFixture<IIoTStandaloneTestContext>
    {
        public CPendingConditionsTestTheory(IIoTStandaloneTestContext context, ITestOutputHelper output)
        : base(context, output)
        {
        }

        [Fact, PriorityOrder(10)]
        public async void TestVerifyDataAvailableAtIoTHubExpectPendingAlarmsView()
        {
            // Arrange
            await TestHelper.CreateSimulationContainerAsync(_context, new List<string>
                {"/bin/sh", "-c", "./opcplc --autoaccept --alm --pn=50000"},
                _timeoutToken).ConfigureAwait(false);

            var messages = _consumer.ReadConditionMessagesFromWriterIdAsync<ConditionTypePayload>(_writerId, 1, _timeoutToken);

            // Act
            var pnJson = _context.PublishedNodesJson(
                50000,
                _writerId,
            TestConstants.PublishedNodesConfigurations.PendingConditionForAlarmsView());
            await TestHelper.SwitchToStandaloneModeAndPublishNodesAsync(pnJson, _context, _timeoutToken).ConfigureAwait(false);
            // Act
            var payloads = await messages.Select(v => v.Payload).ToListAsync(_timeoutToken).ConfigureAwait(false);

            // Assert
            ValidatePendingConditionsView(payloads);
        }
    }
}
