﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// History update type
    /// </summary>
    [DataContract]
    public enum HistoryUpdateOperation
    {
        /// <summary>
        /// Insert
        /// </summary>
        [EnumMember]
        Insert = 1,

        /// <summary>
        /// Replace
        /// </summary>
        [EnumMember]
        Replace,

        /// <summary>
        /// Update
        /// </summary>
        [EnumMember]
        Update,

        /// <summary>
        /// Delete
        /// </summary>
        [EnumMember]
        Delete,
    }
}
