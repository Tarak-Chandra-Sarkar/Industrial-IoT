﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Stack.Models
{
    using Azure.IIoT.OpcUa.Publisher.Stack.Services;
    using Opc.Ua;
    using Opc.Ua.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Notification model extensions
    /// </summary>
    public static class MonitoredItemNotificationModelEx
    {
        /// <summary>
        /// Clone notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(model))]
        public static MonitoredItemNotificationModel? Clone(this MonitoredItemNotificationModel? model,
            uint? sequenceNumber = null, DataValue? dataValue = null)
        {
            if (model == null)
            {
                return null;
            }
            return new MonitoredItemNotificationModel
            {
                Id = model.Id,
                DataSetFieldName = model.DataSetFieldName,
                DisplayName = model.DisplayName,
                NodeId = model.NodeId,
                AttributeId = model.AttributeId,
                Value = dataValue ?? model.Value, // Not cloning, should be immutable
                SequenceNumber = sequenceNumber ?? model.SequenceNumber,
                IsHeartbeat = model.IsHeartbeat
            };
        }

        /// <summary>
        /// Convert to monitored item notifications
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="monitoredItem"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static IEnumerable<MonitoredItemNotificationModel> ToMonitoredItemNotifications(
            this IEncodeable notification, MonitoredItem monitoredItem,
            Func<MonitoredItemNotificationModel>? defaultValue = null)
        {
            if (notification != null && monitoredItem != null)
            {
                if (notification is MonitoredItemNotification m)
                {
                    return m.ToMonitoredItemNotifications(monitoredItem);
                }
                if (notification is EventFieldList e)
                {
                    return e.ToMonitoredItemNotifications(monitoredItem);
                }
            }
            var def = defaultValue?.Invoke();
            return def == null ? Enumerable.Empty<MonitoredItemNotificationModel>() : def.YieldReturn();
        }

        /// <summary>
        /// Convert to monitored item notifications
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="monitoredItem"></param>
        /// <returns></returns>
        public static IEnumerable<MonitoredItemNotificationModel> ToMonitoredItemNotifications(
            this MonitoredItemNotification notification, MonitoredItem monitoredItem)
        {
            if (notification == null || monitoredItem == null)
            {
                yield break;
            }
            var handleId = monitoredItem.Handle as OpcUaMonitoredItem;
            if (handleId?.SkipMonitoredItemNotification() ?? false)
            {
                // Skip change notification
                yield break;
            }
            var sequence = notification.Message?.IsEmpty != false
                ? (uint?)null
                : notification.Message.SequenceNumber;
            yield return new MonitoredItemNotificationModel
            {
                Id = handleId?.Template?.Id ?? string.Empty,
                DataSetFieldName = string.IsNullOrEmpty(monitoredItem.DisplayName)
                    ? handleId?.Template?.Id : monitoredItem.DisplayName,
                DisplayName = monitoredItem.DisplayName,
                NodeId = handleId?.Template?.StartNodeId,
                AttributeId = monitoredItem.AttributeId,
                Value = notification.Value,
                SequenceNumber = sequence,
                IsHeartbeat = false
            };
        }

        /// <summary>
        /// Convert to monitored item notifications
        /// </summary>
        /// <param name="eventFieldList"></param>
        /// <param name="monitoredItem"></param>
        /// <returns></returns>
        public static IEnumerable<MonitoredItemNotificationModel> ToMonitoredItemNotifications(
            this EventFieldList eventFieldList, MonitoredItem monitoredItem)
        {
            var handleId = monitoredItem.Handle as OpcUaMonitoredItem;
            if (eventFieldList != null && monitoredItem != null &&
                handleId?.Fields.Count >= eventFieldList.EventFields.Count)
            {
                for (var i = 0; i < eventFieldList.EventFields.Count; i++)
                {
                    var sequenceNumber = eventFieldList.Message?.IsEmpty != false
                            ? (uint?)null
                            : eventFieldList.Message.SequenceNumber;
                    yield return new MonitoredItemNotificationModel
                    {
                        Id = handleId?.Template?.Id ?? string.Empty,
                        DataSetFieldName = handleId?.Fields[i].Name,
                        DisplayName = monitoredItem.DisplayName,
                        NodeId = handleId?.Template?.StartNodeId,
                        AttributeId = monitoredItem.AttributeId,
                        Value = new DataValue(eventFieldList.EventFields[i]),
                        SequenceNumber = sequenceNumber,
                        IsHeartbeat = false
                    };
                }
            }
        }
    }
}
