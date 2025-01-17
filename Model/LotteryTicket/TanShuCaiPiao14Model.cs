using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.LotteryTicket
{
    /// <summary>
    /// 探数彩票模型
    /// </summary>
    public class TanShuCaiPiao14Model
    {
        [JsonProperty("data")]
        public TanShuCaiPiao14DataModel? Data { get; set; }
    }

    public class TanShuCaiPiao14DataModel
    {
        [JsonProperty("issueno")]
        public int Issueno { get; set; }

        [JsonProperty("number")]
        public string? Number { get; set; }

        [JsonProperty("refernumber")]
        public string? Refernumber { get; set; }
    }
}
