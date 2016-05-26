using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;

namespace Sandboxable.Hyak.Common
{
  public static class TracingAdapter
  {
    private static long _nextInvocationId = 0;
    /// <summary>The collection of tracing interceptors to notify.</summary>
    private static List<ICloudTracingInterceptor> _interceptors;
    /// <summary>
    /// A read-only, thread-safe collection of tracing interceptors.  Since
    /// List is only thread-safe for reads (and adding/removing tracing
    /// interceptors isn't a very common operation), we simply replace the
    /// entire collection of interceptors so any enumeration of the list
    /// in progress on a different thread will not be affected by the
    /// change.
    /// </summary>
    private static List<ICloudTracingInterceptor> _threadSafeInterceptors;
    /// <summary>
    /// Lock used to synchronize mutation of the tracing interceptors.
    /// </summary>
    private static object _lock;

    /// <summary>
    /// Gets or sets a value indicating whether tracing is enabled.
    /// Tracing can be disabled for performance.
    /// </summary>
    public static bool IsEnabled { get; set; }

    /// <summary>
    /// Gets a sequence of the tracing interceptors to notify of changes.
    /// </summary>
    internal static IEnumerable<ICloudTracingInterceptor> TracingInterceptors
    {
      get
      {
        return (IEnumerable<ICloudTracingInterceptor>) TracingAdapter._threadSafeInterceptors;
      }
    }

    public static long NextInvocationId
    {
      get
      {
        return Interlocked.Increment(ref TracingAdapter._nextInvocationId);
      }
    }

    static TracingAdapter()
    {
      TracingAdapter.IsEnabled = true;
      TracingAdapter._lock = new object();
      TracingAdapter._interceptors = new List<ICloudTracingInterceptor>();
      TracingAdapter._threadSafeInterceptors = new List<ICloudTracingInterceptor>();
    }

    /// <summary>Add a tracing interceptor to be notified of changes.</summary>
    /// <param name="interceptor">The tracing interceptor.</param>
    public static void AddTracingInterceptor(ICloudTracingInterceptor interceptor)
    {
      if (interceptor == null)
        throw new ArgumentNullException("interceptor");
      lock (TracingAdapter._lock)
      {
        TracingAdapter._interceptors.Add(interceptor);
        TracingAdapter._threadSafeInterceptors = new List<ICloudTracingInterceptor>((IEnumerable<ICloudTracingInterceptor>) TracingAdapter._interceptors);
      }
    }

    /// <summary>
    /// Remove a tracing interceptor from change notifications.
    /// </summary>
    /// <param name="interceptor">The tracing interceptor.</param>
    /// <returns>
    /// True if the tracing interceptor was found and removed; false
    /// otherwise.
    /// </returns>
    public static bool RemoveTracingInterceptor(ICloudTracingInterceptor interceptor)
    {
      if (interceptor == null)
        throw new ArgumentNullException("interceptor");
      bool flag = false;
      lock (TracingAdapter._lock)
      {
        flag = TracingAdapter._interceptors.Remove(interceptor);
        if (flag)
          TracingAdapter._threadSafeInterceptors = new List<ICloudTracingInterceptor>((IEnumerable<ICloudTracingInterceptor>) TracingAdapter._interceptors);
      }
      return flag;
    }

    public static void Information(string message, params object[] parameters)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      TracingAdapter.Information(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, parameters));
    }

    public static void Configuration(string source, string name, string value)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.Configuration(source, name, value);
    }

    public static void Information(string message)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.Information(message);
    }

    public static void Enter(string invocationId, object instance, string method, IDictionary<string, object> parameters)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.Enter(invocationId, instance, method, parameters);
    }

    public static void SendRequest(string invocationId, HttpRequestMessage request)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.SendRequest(invocationId, request);
    }

    public static void ReceiveResponse(string invocationId, HttpResponseMessage response)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.ReceiveResponse(invocationId, response);
    }

    public static void Error(string invocationId, Exception ex)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.Error(invocationId, ex);
    }

    public static void Exit(string invocationId, object result)
    {
      if (!TracingAdapter.IsEnabled)
        return;
      foreach (ICloudTracingInterceptor tracingInterceptor in TracingAdapter.TracingInterceptors)
        tracingInterceptor.Exit(invocationId, result);
    }
  }
}
