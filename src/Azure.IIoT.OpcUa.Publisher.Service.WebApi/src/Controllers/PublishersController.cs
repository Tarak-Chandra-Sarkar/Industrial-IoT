// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Azure.IIoT.OpcUa.Publisher.Service.WebApi.Controllers
{
    using Azure.IIoT.OpcUa.Publisher.Service.WebApi.Filters;
    using Azure.IIoT.OpcUa.Publisher.Models;
    using Furly.Extensions.AspNetCore.OpenApi;
    using Furly.Extensions.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Read, Update and Query publisher resources
    /// </summary>
    [ApiVersion("2")]
    [Route("registry/v{version:apiVersion}/publishers")]
    [ExceptionsFilter]
    [Authorize(Policy = Policies.CanRead)]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        /// <summary>
        /// Create controller for publisher services
        /// </summary>
        /// <param name="publishers"></param>
        public PublishersController(IPublisherRegistry publishers)
        {
            _publishers = publishers;
        }

        /// <summary>
        /// Get publisher registration information
        /// </summary>
        /// <remarks>
        /// Returns a publisher's registration and connectivity information.
        /// A publisher id corresponds to the twin modules module identity.
        /// </remarks>
        /// <param name="publisherId">Publisher identifier</param>
        /// <param name="onlyServerState">Whether to include only server
        /// state, or display current client state of the endpoint if
        /// available</param>
        /// <param name="ct"></param>
        /// <returns>Publisher registration</returns>
        [HttpGet("{publisherId}")]
        public async Task<PublisherModel> GetPublisherAsync(string publisherId,
            [FromQuery] bool? onlyServerState, CancellationToken ct)
        {
            return await _publishers.GetPublisherAsync(publisherId,
                onlyServerState ?? false, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Update publisher configuration
        /// </summary>
        /// <remarks>
        /// Allows a caller to configure operations on the publisher module
        /// identified by the publisher id.
        /// </remarks>
        /// <param name="publisherId">Publisher identifier</param>
        /// <param name="request">Patch request</param>
        /// <param name="ct"></param>
        /// <exception cref="ArgumentNullException"><paramref name="request"/>
        /// is <c>null</c>.</exception>
        [HttpPatch("{publisherId}")]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task UpdatePublisherAsync(string publisherId,
            [FromBody][Required] PublisherUpdateModel request, CancellationToken ct)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            await _publishers.UpdatePublisherAsync(publisherId, request,
                ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Get list of publishers
        /// </summary>
        /// <remarks>
        /// Get all registered publishers and therefore twin modules in paged form.
        /// The returned model can contain a continuation token if more results are
        /// available.
        /// Call this operation again using the token to retrieve more results.
        /// </remarks>
        /// <param name="onlyServerState">Whether to include only server
        /// state, or display current client state of the endpoint if available</param>
        /// <param name="continuationToken">Optional Continuation token</param>
        /// <param name="pageSize">Optional number of results to return</param>
        /// <param name="ct"></param>
        /// <returns>
        /// List of publishers and continuation token to use for next request
        /// in x-ms-continuation header.
        /// </returns>
        [HttpGet]
        [AutoRestExtension(NextPageLinkName = "continuationToken")]
        public async Task<PublisherListModel> GetListOfPublisherAsync(
            [FromQuery] bool? onlyServerState,
            [FromQuery] string? continuationToken,
            [FromQuery] int? pageSize, CancellationToken ct)
        {
            if (Request.Headers.ContainsKey(HttpHeader.ContinuationToken))
            {
                continuationToken = Request.Headers[HttpHeader.ContinuationToken]
                    .FirstOrDefault();
            }
            if (Request.Headers.ContainsKey(HttpHeader.MaxItemCount))
            {
                pageSize = int.Parse(
                    Request.Headers[HttpHeader.MaxItemCount].FirstOrDefault()!,
                    CultureInfo.InvariantCulture);
            }
            return await _publishers.ListPublishersAsync(continuationToken,
                onlyServerState ?? false, pageSize, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Query publishers
        /// </summary>
        /// <remarks>
        /// Get all publishers that match a specified query.
        /// The returned model can contain a continuation token if more results are
        /// available.
        /// Call the GetListOfPublisher operation using the token to retrieve
        /// more results.
        /// </remarks>
        /// <param name="query">Publisher query model</param>
        /// <param name="onlyServerState">Whether to include only server
        /// state, or display current client state of the endpoint if
        /// available</param>
        /// <param name="pageSize">Number of results to return</param>
        /// <param name="ct"></param>
        /// <returns>Publisher</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/>
        /// is <c>null</c>.</exception>
        [HttpPost("query")]
        public async Task<PublisherListModel> QueryPublisherAsync(
            [FromBody][Required] PublisherQueryModel query,
            [FromQuery] bool? onlyServerState,
            [FromQuery] int? pageSize, CancellationToken ct)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (Request.Headers.ContainsKey(HttpHeader.MaxItemCount))
            {
                pageSize = int.Parse(
                    Request.Headers[HttpHeader.MaxItemCount].FirstOrDefault()!,
                    CultureInfo.InvariantCulture);
            }

            // TODO: Filter results based on RBAC

            return await _publishers.QueryPublishersAsync(query,
                onlyServerState ?? false, pageSize, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Get filtered list of publishers
        /// </summary>
        /// <remarks>
        /// Get a list of publishers filtered using the specified query parameters.
        /// The returned model can contain a continuation token if more results are
        /// available.
        /// Call the GetListOfPublisher operation using the token to retrieve
        /// more results.
        /// </remarks>
        /// <param name="query">Publisher Query model</param>
        /// <param name="onlyServerState">Whether to include only server
        /// state, or display current client state of the endpoint if
        /// available</param>
        /// <param name="pageSize">Number of results to return</param>
        /// <param name="ct"></param>
        /// <returns>Publisher</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/>
        /// is <c>null</c>.</exception>
        [HttpGet("query")]
        public async Task<PublisherListModel> GetFilteredListOfPublisherAsync(
            [FromQuery][Required] PublisherQueryModel query,
            [FromQuery] bool? onlyServerState,
            [FromQuery] int? pageSize, CancellationToken ct)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (Request.Headers.ContainsKey(HttpHeader.MaxItemCount))
            {
                pageSize = int.Parse(
                    Request.Headers[HttpHeader.MaxItemCount].FirstOrDefault()!,
                    CultureInfo.InvariantCulture);
            }

            // TODO: Filter results based on RBAC

            return await _publishers.QueryPublishersAsync(query,
                onlyServerState ?? false, pageSize, ct).ConfigureAwait(false);
        }

        private readonly IPublisherRegistry _publishers;
    }
}
