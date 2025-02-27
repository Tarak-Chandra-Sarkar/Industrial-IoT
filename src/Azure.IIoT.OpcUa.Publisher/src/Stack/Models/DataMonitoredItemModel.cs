﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Stack.Models
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using System;

    /// <summary>
    /// Data monitored item
    /// </summary>
    public sealed record class DataMonitoredItemModel : BaseMonitoredItemModel
    {
        /// <summary>
        /// Field id in class
        /// </summary>
        public Guid DataSetClassFieldId { get; init; }

        /// <summary>
        /// Data change filter
        /// </summary>
        public DataChangeFilterModel? DataChangeFilter { get; init; }

        /// <summary>
        /// Aggregate filter
        /// </summary>
        public AggregateFilterModel? AggregateFilter { get; init; }

        /// <summary>
        /// heartbeat interval not present if zero
        /// </summary>
        public TimeSpan? HeartbeatInterval { get; init; }

        /// <summary>
        /// Skip first value
        /// </summary>
        public bool SkipFirst { get; init; }
    }
}
