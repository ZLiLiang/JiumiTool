using System.Collections.Generic;

namespace Z.JiumiTool.Extensions
{
    public static class QueueExtension
    {
        /// <summary>
        /// 将队列的元素全部移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static List<T> DequeueToList<T>(this Queue<T> queue)
        {
            var result = new List<T>(queue.Count);
            while (queue.TryDequeue(out T element))
            {
                result.Add(element);
            }

            return result;
        }
    }
}
