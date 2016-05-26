namespace Sandboxable.Hyak.Common
{
  /// <summary>
  /// Interface used to represent resource groupings of ServiceClient
  /// operations.
  /// </summary>
  /// <typeparam name="TClient">Type of the ServiceClient.</typeparam>
  public interface IServiceOperations<TClient> where TClient : ServiceClient<TClient>
  {
    /// <summary>Gets a reference to the ServiceClient.</summary>
    TClient Client { get; }
  }
}