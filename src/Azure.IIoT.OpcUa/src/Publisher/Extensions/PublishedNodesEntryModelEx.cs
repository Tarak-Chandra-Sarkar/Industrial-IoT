﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Config.Models
{
    using Azure.IIoT.OpcUa.Publisher.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// PublishedNodesEntryModel extensions
    /// </summary>
    public static class PublishedNodesEntryModelEx
    {
        /// <summary>
        /// Validates if the entry has same group as the model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="that"></param>
        public static bool HasSameGroup(this PublishedNodesEntryModel model,
            PublishedNodesEntryModel that)
        {
            if (model == that)
            {
                return true;
            }
            if (model == null || that == null)
            {
                return false;
            }
            if (!string.Equals(model.DataSetWriterGroup,
                that.DataSetWriterGroup, StringComparison.Ordinal))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Compare entry to connection endpoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(model))]
        public static PublishedNodesEntryModel? ToPublishedNodesEntry(this ConnectionModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new PublishedNodesEntryModel
            {
                EndpointUrl = model.Endpoint?.Url,
                UseSecurity = model.Endpoint?.SecurityMode != SecurityMode.None,
                OpcAuthenticationMode = (model.User?.Type ?? CredentialType.None)
                    == CredentialType.None ?
                        OpcAuthenticationMode.Anonymous : OpcAuthenticationMode.UsernamePassword,
                OpcAuthenticationPassword = model.User.GetPassword(),
                OpcAuthenticationUsername = model.User.GetUserName(),
                OpcNodes = new List<OpcNodeModel>()
            };
        }

        /// <summary>
        /// Check if has same endpoint
        /// </summary>
        /// <param name="model"></param>
        /// <param name="that"></param>
        /// <returns></returns>
        public static bool HasSameEndpoint(this PublishedNodesEntryModel model,
            PublishedNodesEntryModel that)
        {
            if (model == that)
            {
                return true;
            }
            if (model == null || that == null)
            {
                return false;
            }
            if (model.EndpointUrl != that.EndpointUrl)
            {
                return false;
            }
            if (model.UseSecurity != that.UseSecurity)
            {
                return false;
            }
            if (model.OpcAuthenticationMode != that.OpcAuthenticationMode)
            {
                return false;
            }
            if (!string.Equals(model.OpcAuthenticationUsername ?? string.Empty,
                that.OpcAuthenticationUsername ?? string.Empty, StringComparison.Ordinal))
            {
                return false;
            }
            if (!string.Equals(model.OpcAuthenticationPassword ?? string.Empty,
                that.OpcAuthenticationPassword ?? string.Empty, StringComparison.Ordinal))
            {
                return false;
            }
            if (!string.Equals(model.EncryptedAuthUsername ?? string.Empty,
                that.EncryptedAuthUsername ?? string.Empty, StringComparison.Ordinal))
            {
                return false;
            }
            if (!string.Equals(model.EncryptedAuthPassword ?? string.Empty,
                that.EncryptedAuthPassword ?? string.Empty, StringComparison.Ordinal))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return a cloaked published nodes entry that can be used as lookup input to
        /// <see cref="HasSameDataSet(PublishedNodesEntryModel, PublishedNodesEntryModel)"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(model))]
        public static PublishedNodesEntryModel? ToDataSetEntry(this PublishedNodesEntryModel? model)
        {
            if (model == null)
            {
                return null;
            }
            return new PublishedNodesEntryModel
            {
                DataSetClassId = model.DataSetClassId,
                DataSetDescription = model.DataSetDescription,
                DataSetKeyFrameCount = model.DataSetKeyFrameCount,
                DataSetName = model.DataSetName,
                DataSetPublishingInterval = model.DataSetPublishingInterval,
                DataSetPublishingIntervalTimespan = model.DataSetPublishingIntervalTimespan,
                DataSetWriterGroup = model.DataSetWriterGroup,
                DataSetWriterId = model.DataSetWriterId,
                EncryptedAuthUsername = model.EncryptedAuthUsername,
                EndpointUrl = model.EndpointUrl,
                MetaDataUpdateTime = model.MetaDataUpdateTime,
                MetaDataUpdateTimeTimespan = model.MetaDataUpdateTimeTimespan,
                LastChangeTimespan = model.LastChangeTimespan,
                OpcAuthenticationUsername = model.OpcAuthenticationUsername,
                OpcAuthenticationMode = model.OpcAuthenticationMode,
                UseSecurity = model.UseSecurity,
                Version = model.Version,
                NodeId = null,
                EncryptedAuthPassword = null,
                OpcAuthenticationPassword = null,
                OpcNodes = null
            };
        }

        /// <summary>
        /// Validates if the entry has same data set definition as the model.
        /// Comarison excludes OpcNodes.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="that"></param>
        public static bool HasSameDataSet(this PublishedNodesEntryModel model,
            PublishedNodesEntryModel that)
        {
            if (!string.Equals(model.DataSetWriterId ?? string.Empty,
                that.DataSetWriterId ?? string.Empty, StringComparison.Ordinal))
            {
                return false;
            }
            if (!model.HasSameGroup(that))
            {
                return false;
            }
            if (!model.HasSameEndpoint(that))
            {
                return false;
            }
            if (!string.Equals(model.DataSetName ?? string.Empty,
                that.DataSetName ?? string.Empty, StringComparison.Ordinal))
            {
                return false;
            }
            if (model.DataSetClassId != that.DataSetClassId)
            {
                return false;
            }
            if (model.DataSetKeyFrameCount != that.DataSetKeyFrameCount)
            {
                return false;
            }
            if (model.MetaDataUpdateTimeTimespan != that.MetaDataUpdateTimeTimespan)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves the timespan flavor of a PublishedNodesEntryModel's DataSetPublishingInterval
        /// </summary>
        /// <param name="model"></param>
        /// <param name="defaultPublishingTimespan"></param>
        public static TimeSpan? GetNormalizedDataSetPublishingInterval(
            this PublishedNodesEntryModel model, TimeSpan? defaultPublishingTimespan = null)
        {
            return model.DataSetPublishingIntervalTimespan
                .GetTimeSpanFromMiliseconds(model.DataSetPublishingInterval, defaultPublishingTimespan);
        }

        /// <summary>
        /// Promote the default publishing interval of the model to all of
        /// its nodes to support apples to apples comparison.
        /// </summary>
        /// <param name="model"></param>
        public static PublishedNodesEntryModel PropagatePublishingIntervalToNodes(
            this PublishedNodesEntryModel model)
        {
            if (model.OpcNodes != null && model.OpcNodes.Count != 0)
            {
                var rootInterval = model.GetNormalizedDataSetPublishingInterval();
                if (rootInterval == null)
                {
                    return model;
                }
                foreach (var node in model.OpcNodes)
                {
                    var nodeInterval = node.GetNormalizedPublishingInterval();
                    if (nodeInterval == null)
                    {
                        // Set publishing interval from root
                        node.OpcPublishingIntervalTimespan = rootInterval;
                    }
                }
            }
            // Remove root interval
            model.DataSetPublishingInterval = null;
            model.DataSetPublishingIntervalTimespan = null;
            return model;
        }
    }
}
