using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCFServiceWebRole1.Models
{
    public class SessionEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Session";
        public string RowKey { get; set; } 
        public string SessionId { get; set; }

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}