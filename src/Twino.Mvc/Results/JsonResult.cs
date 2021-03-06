﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twino.Core;

namespace Twino.Mvc.Results
{
    /// <summary>
    /// JSON Action result
    /// </summary>
    public class JsonResult : IActionResult
    {
        /// <summary>
        /// Result HTTP Status code
        /// </summary>
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// Result content type (such as application/json, text/xml, text/plain)
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Result content body
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Additional custom headers with key and value
        /// </summary>
        public Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Creates new empty JSON result
        /// </summary>
        public JsonResult(HttpStatusCode code = HttpStatusCode.OK)
        {
            Code = code;
            ContentType = ContentTypes.APPLICATION_JSON;
            Headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Creates new JSON result
        /// </summary>
        public JsonResult(object model, HttpStatusCode code = HttpStatusCode.OK)
        {
            Code = code;
            ContentType = ContentTypes.APPLICATION_JSON;
            Headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            Set(model);
        }

        /// <summary>
        /// Sets json objects of json result
        /// </summary>
        public async Task SetAsync(object model)
        {
            Stream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(Stream, model, model.GetType());
        }

        /// <summary>
        /// Sets json objects of json result
        /// </summary>
        public void Set(object model)
        {
            string serialized = System.Text.Json.JsonSerializer.Serialize(model, model.GetType());
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
        }
    }
}