﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Sandboxable.Hyak.Common
{
  /// <summary>Helper extension methods used by tracing providers.</summary>
  public static class TracingExtensions
  {
    /// <summary>
    /// Returns string representation of a HttpRequestMessage.
    /// </summary>
    /// <param name="httpRequest">Request to format.</param>
    /// <returns>Formatted string.</returns>
    public static string AsFormattedString(this HttpRequestMessage httpRequest)
    {
      if (httpRequest == null)
        throw new ArgumentNullException("httpRequest");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(((object) httpRequest).ToString());
      if (httpRequest.Content != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Body:");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(httpRequest.Content.ReadAsStringAsync().Result);
        stringBuilder.AppendLine("}");
      }
      return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns string representation of a HttpResponseMessage.
    /// </summary>
    /// <param name="httpResponse">Response to format.</param>
    /// <returns>Formatted string.</returns>
    public static string AsFormattedString(this HttpResponseMessage httpResponse)
    {
      if (httpResponse == null)
        throw new ArgumentNullException("httpResponse");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(((object) httpResponse).ToString());
      if (httpResponse.Content != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Body:");
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine(httpResponse.Content.ReadAsStringAsync().Result);
        stringBuilder.AppendLine("}");
      }
      return stringBuilder.ToString();
    }

    /// <summary>Converts given dictionary into a log string.</summary>
    /// <typeparam name="TKey">The dictionary key type</typeparam>
    /// <typeparam name="TValue">The dictionary value type</typeparam>
    /// <param name="dictionary">The dictionary collection object</param>
    /// <returns>The log string</returns>
    public static string AsFormattedString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
      if (dictionary == null)
        return "{}";
      return "{" + string.Join(",", dictionary.Select<KeyValuePair<TKey, TValue>, string>((Func<KeyValuePair<TKey, TValue>, string>) (kv => kv.Key.ToString() + "=" + ((object) kv.Value == null ? string.Empty : kv.Value.ToString()))).ToArray<string>()) + "}";
    }
  }
}