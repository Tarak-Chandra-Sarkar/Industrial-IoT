﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Config.Models
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Dataset source extensions
    /// </summary>
    public static class OpcNodeModelEx
    {
        /// <summary>
        /// Get comparer class for OpcNodeModel objects.
        /// </summary>
        public static EqualityComparer<OpcNodeModel> Comparer { get; } =
            new OpcNodeModelComparer();

        /// <summary>
        /// Check if nodes are equal
        /// </summary>
        /// <param name="model"></param>
        /// <param name="that"></param>
        public static bool IsSame(this OpcNodeModel? model, OpcNodeModel? that)
        {
            if (model == that)
            {
                return true;
            }
            if (model == null || that == null)
            {
                return false;
            }

            if (!string.Equals(model.Id ?? string.Empty,
                that.Id ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(model.DisplayName ?? string.Empty,
                that.DisplayName ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(model.DataSetFieldId ?? string.Empty,
                that.DataSetFieldId ?? string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (model.DataSetClassFieldId != that.DataSetClassFieldId)
            {
                return false;
            }

            if (!string.Equals(model.ExpandedNodeId,
                that.ExpandedNodeId, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (model.GetNormalizedPublishingInterval() != that.GetNormalizedPublishingInterval())
            {
                return false;
            }

            if (model.GetNormalizedSamplingInterval() != that.GetNormalizedSamplingInterval())
            {
                return false;
            }

            if (model.GetNormalizedHeartbeatInterval() != that.GetNormalizedHeartbeatInterval())
            {
                return false;
            }

            if ((model.SkipFirst ?? false) != (that.SkipFirst ?? false))
            {
                return false;
            }

            if ((model.DiscardNew ?? false) != (that.DiscardNew ?? false))
            {
                return false;
            }

            if (model.QueueSize != that.QueueSize)
            {
                return false;
            }

            //
            // Null is default and equals to StatusValue, but we allow StatusValue == 1
            // to be set specifically to enable a user to force a data filter to be
            // applied (otherwise it is not if nothing else is set)
            //
            if (model.DataChangeTrigger != that.DataChangeTrigger)
            {
                return false;
            }

            // Null is None == no deadband
            if (model.DeadbandType != that.DeadbandType)
            {
                return false;
            }

            if (model.DeadbandValue != that.DeadbandValue)
            {
                return false;
            }

            if (!model.EventFilter.IsSameAs(that.EventFilter))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the hashcode for a node
        /// </summary>
        /// <param name="model"></param>
        public static int GetHashCode(this OpcNodeModel model)
        {
            var hash = new HashCode();
            hash.Add(model.Id);
            hash.Add(model.DisplayName);
            hash.Add(model.DataSetFieldId);
            hash.Add(model.DataSetClassFieldId);
            hash.Add(model.ExpandedNodeId);
            hash.Add(model.GetNormalizedPublishingInterval());
            hash.Add(model.GetNormalizedSamplingInterval());
            hash.Add(model.GetNormalizedHeartbeatInterval());
            hash.Add(model.SkipFirst ?? false);
            hash.Add(model.DiscardNew ?? false);
            hash.Add(model.QueueSize);
            if (model.DataChangeTrigger == null)
            {
                //
                // Null is default and equals to StatusValue, but we allow StatusValue == 1
                // to be set specifically to enable a user to force a data filter to be
                // applied (otherwise it is not if nothing else is set)
                //
                hash.Add(-1);
            }
            else
            {
                hash.Add(model.DataChangeTrigger);
            }
            hash.Add(model.DeadbandValue);
            if (model.DeadbandType == null)
            {
                // Null is None == no deadband
                hash.Add(-1);
            }
            else
            {
                hash.Add(model.DeadbandType);
            }
            return hash.ToHashCode();
        }

        /// <summary>
        /// Retrieves the timespan flavor of a node's HeartbeatInterval
        /// </summary>
        /// <param name="model"></param>
        /// <param name="defaultHeatbeatTimespan"></param>
        /// <returns></returns>
        public static TimeSpan? GetNormalizedHeartbeatInterval(
            this OpcNodeModel model, TimeSpan? defaultHeatbeatTimespan = null)
        {
            return model.HeartbeatIntervalTimespan
                .GetTimeSpanFromSeconds(model.HeartbeatInterval, defaultHeatbeatTimespan);
        }

        /// <summary>
        /// Retrieves the timespan flavor of a node's PublishingInterval
        /// </summary>
        /// <param name="model"></param>
        /// <param name="defaultPublishingTimespan"></param>
        public static TimeSpan? GetNormalizedPublishingInterval(
            this OpcNodeModel model, TimeSpan? defaultPublishingTimespan = null)
        {
            return model.OpcPublishingIntervalTimespan
                .GetTimeSpanFromMiliseconds(model.OpcPublishingInterval, defaultPublishingTimespan);
        }

        /// <summary>
        /// Retrieves the timespan flavor of a node's SamplingInterval
        /// </summary>
        /// <param name="model"></param>
        /// <param name="defaultSamplingTimespan"></param>
        public static TimeSpan? GetNormalizedSamplingInterval(
            this OpcNodeModel model, TimeSpan? defaultSamplingTimespan = null)
        {
            return model.OpcSamplingIntervalTimespan
                .GetTimeSpanFromMiliseconds(model.OpcSamplingInterval, defaultSamplingTimespan);
        }

        /// <summary>
        /// Returns a the timespan value from the timespan when defined, respectively from
        /// the seconds representing integer. The Timespan value wins when provided
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="seconds"></param>
        /// <param name="defaultTimespan"></param>
        public static TimeSpan? GetTimeSpanFromSeconds(
            this TimeSpan? timespan,
            int? seconds,
            TimeSpan? defaultTimespan = null)
        {
            return timespan ?? (seconds.HasValue
                    ? TimeSpan.FromSeconds(seconds.Value)
                    : defaultTimespan);
        }

        /// <summary>
        /// Returns a the timespan value from the timespan when defined, respectively from
        /// the miliseconds representing integer. The Timespan value wins when provided
        /// </summary>
        /// <param name="timespan"></param>
        /// <param name="miliseconds"></param>
        /// <param name="defaultTimespan"></param>
        public static TimeSpan? GetTimeSpanFromMiliseconds(
            this TimeSpan? timespan,
            int? miliseconds,
            TimeSpan? defaultTimespan = null)
        {
            return timespan ?? (miliseconds.HasValue
                    ? TimeSpan.FromMilliseconds(miliseconds.Value)
                    : defaultTimespan);
        }

        /// <summary>
        /// Equality comparer for OpcNodeModel objects.
        /// </summary>
        private class OpcNodeModelComparer : EqualityComparer<OpcNodeModel>
        {
            /// <inheritdoc/>
            public override bool Equals(OpcNodeModel? node1, OpcNodeModel? node2)
            {
                return node1.IsSame(node2);
            }

            /// <inheritdoc/>
            public override int GetHashCode(OpcNodeModel node)
            {
                return OpcNodeModelEx.GetHashCode(node);
            }
        }
    }
}
