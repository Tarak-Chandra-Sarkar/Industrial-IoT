﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Stack
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using System;

    /// <summary>
    /// Subscription configuration
    /// </summary>
    public sealed class OpcUaSubscriptionOptions
    {
        /// <summary>
        /// The default interval for heartbeats if not configured.
        /// </summary>
        public TimeSpan? DefaultHeartbeatInterval { get; set; }

        /// <summary>
        /// The default flag whether to skip the first value if
        /// not set on node level.
        /// </summary>
        public bool? DefaultSkipFirst { get; set; }

        /// <summary>
        /// The default flag whether to descard new items in queue
        /// </summary>
        public bool? DefaultDiscardNew { get; set; }

        /// <summary>
        /// The default sampling interval.
        /// </summary>
        public TimeSpan? DefaultSamplingInterval { get; set; }

        /// <summary>
        /// The default publishing interval.
        /// </summary>
        public TimeSpan? DefaultPublishingInterval { get; set; }

        /// <summary>
        /// Default subscription keep alive counter
        /// </summary>
        public uint? DefaultKeepAliveCount { get; set; }

        /// <summary>
        /// Default subscription lifetime counter
        /// </summary>
        public uint? DefaultLifeTimeCount { get; set; }

        /// <summary>
        /// Whether to enable or disable data set metadata explicitly
        /// </summary>
        public bool? DisableDataSetMetaData { get; set; }

        /// <summary>
        /// Default metadata send interval.
        /// </summary>
        public TimeSpan? DefaultMetaDataUpdateTime { get; set; }

        /// <summary>
        /// Whether to enable or disable key frames explicitly
        /// </summary>
        public bool? DisableKeyFrames { get; set; }

        /// <summary>
        /// Default keyframe count
        /// </summary>
        public uint? DefaultKeyFrameCount { get; set; }

        /// <summary>
        /// Flag wether to grab the display name of nodes form
        /// the OPC UA Server.
        /// </summary>
        public bool? ResolveDisplayName { get; set; }

        /// <summary>
        /// set the default queue size for monitored items. If not
        /// set the default queue size will be configured (1 for
        /// data monitored items, and 0 for event monitoring).
        /// </summary>
        public uint? DefaultQueueSize { get; set; }

        /// <summary>
        /// set the default data change filter for monitored items. Default is
        /// status and value change triggering.
        /// </summary>
        public DataChangeTriggerType? DefaultDataChangeTrigger { get; set; }
    }
}
