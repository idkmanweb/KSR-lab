using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Azure;
using Azure.Data.Tables;

namespace WCFServiceWebRole1.Models
{
    public class UserEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "User";
        public string RowKey { get; set; }
        public string Haslo { get; set; }

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}