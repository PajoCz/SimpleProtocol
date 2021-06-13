//Kopie z D3Soft.Lib.Common.ProducerConsumer
namespace SimpleProtocol.Repository.SqlDapper.ProducerConsumer
{
    /// <summary>
    /// V jakem poradi vyzvedava obsluzne vlakno Items z fronty
    /// </summary>
    public enum QueueConsumerFindModeItem
    {
        /// <summary>
        /// Prechodem na GetNextItem() vracim po jednom zaznamu z kazde fronty a rotuju.
        /// </summary>
        RotateQueue,
        /// <summary>
        /// Prechodem na GetNextItem() vracim vsechny zaznamy ze stejne fronty dokud najake mam. Az fronta dojde, potom zkusim dalsi frontu
        /// </summary>
        ByQueue,
    }
}
