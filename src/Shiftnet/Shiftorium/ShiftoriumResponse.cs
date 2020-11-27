using System;
using System.Collections.Generic;
using System.Linq;
using AlkalineThunder.Pandemic;
using System.Text.Json;

namespace Shiftnet.Shiftorium
{
    public class ShiftoriumResponse
    {
        public ShiftoriumResponse(PropertySet properties, int statusCode)
        {
            StatusCode = statusCode;
            var errors = properties.GetValue("errors", Array.Empty<string>());
            Errors = errors;
            this.Payload = properties.GetValue<JsonElement>("result");
        }

        public IEnumerable<string> Errors { get; }
        public int StatusCode { get; }
        public JsonElement Payload { get; }

        public bool IsError => Errors.Any();
        public bool HasResult => Payload.ValueKind != JsonValueKind.Null;
    }
}