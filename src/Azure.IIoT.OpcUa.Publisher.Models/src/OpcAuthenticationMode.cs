﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Enum that defines the authentication method
    /// </summary>
    [DataContract]
    public enum OpcAuthenticationMode
    {
        /// <summary>
        /// Anonymous authentication
        /// </summary>
        [EnumMember]
        Anonymous,

        /// <summary>
        /// Username/Password authentication
        /// </summary>
        [EnumMember]
        UsernamePassword
    }
}
