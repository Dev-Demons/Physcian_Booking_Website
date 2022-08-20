namespace Binke.Models
{
    public class JsonResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status">
        /// 0 = false
        /// 1 = true
        /// </param>
        /// <param name="Data"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public int Status { get; set; }
        public object Data { get; set; }
        public string Message { get; set; } = "Something is wrong.";
    }
}
