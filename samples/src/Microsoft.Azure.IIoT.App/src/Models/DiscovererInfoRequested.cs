﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.App.Models
{
    using System.Collections.Generic;

    public class DiscovererInfoRequested
    {
        /// <summary>
        /// Address ranges to scan (null == all wired nics)
        /// </summary>
        public string RequestedAddressRangesToScan { get; set; }

        /// <summary>
        /// Port ranges to scan (null == all unassigned)
        /// </summary>
        public string RequestedPortRangesToScan { get; set; }

        /// <summary>
        /// Max network probes that should ever run.
        /// </summary>
        public string RequestedMaxNetworkProbes { get; set; }

        /// <summary>
        /// Max port probes that should ever run.
        /// </summary>
        public string RequestedMaxPortProbes { get; set; }

        /// <summary>
        /// Network probe timeout
        /// </summary>
        public string RequestedNetworkProbeTimeout { get; set; }

        /// <summary>
        /// Port probe timeout
        /// </summary>
        public string RequestedPortProbeTimeout { get; set; }

        /// <summary>
        /// Delay time between discovery sweeps in seconds
        /// </summary>
        public string RequestedIdleTimeBetweenScans { get; set; }

        /// <summary>
        /// List of preset discovery urls to use
        /// </summary>
        public List<string> RequestedDiscoveryUrls { get; set; }
        /// <summary>
        /// Add url
        /// </summary>
        /// <param name="url"></param>
        public void AddDiscoveryUrl(string url)
        {
            RequestedDiscoveryUrls ??= new List<string>();
            RequestedDiscoveryUrls.Add(url);
        }

        /// <summary>
        /// Clear url list
        /// </summary>
        /// <param name="list"></param>
        public void ClearDiscoveryUrlList(List<string> list)
        {
            list?.Clear();
        }
    }
}
