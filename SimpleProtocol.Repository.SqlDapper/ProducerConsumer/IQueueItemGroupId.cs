//Kopie z D3Soft.Lib.Common.ProducerConsumer
namespace SimpleProtocol.Repository.SqlDapper.ProducerConsumer
{
    /// <summary>
    /// Kazdy QueueItem objekt co se posila do QueueWithThread musi identifikovat, do ktere group fronty spada
    /// </summary>
    public interface IQueueItemGroupId
    {
        string QueueGroupId { get; }
    }
}
