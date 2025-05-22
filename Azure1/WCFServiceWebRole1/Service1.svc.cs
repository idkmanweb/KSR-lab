using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WCFServiceWebRole1.Models;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private readonly string storageConnectionString = "UseDevelopmentStorage=true";
        private readonly string blobContainerName = "files";
        private readonly string tableName = "UserSessions";

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        private TableClient GetTableClient()
        {
            var serviceClient = new TableServiceClient(storageConnectionString);
            var tableClient = serviceClient.GetTableClient(tableName);
            tableClient.CreateIfNotExists();
            return tableClient;
        }

        private BlobContainerClient GetBlobContainerClient()
        {
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            containerClient.CreateIfNotExists();
            return containerClient;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public bool Create(string login, string haslo)
        {
            var table = GetTableClient();
            var user = new UserEntity { RowKey = login, Haslo = haslo };
            try
            {
                table.AddEntity(user);
                return true;
            }
            catch
            {
                // user already exists
                return false;
            }
        }

        public Guid? Login(string login, string haslo)
        {
            var table = GetTableClient();
            try
            {
                var user = table.GetEntity<UserEntity>("User", login);
                if (user.Value.Haslo != haslo) return null;

                var sessionId = Guid.NewGuid();
                var session = new SessionEntity { RowKey = login, SessionId = sessionId.ToString() };
                table.UpsertEntity(session);
                return sessionId;
            }
            catch
            {
                return null;
            }
        }

        public bool Logout(string login)
        {
            var table = GetTableClient();
            try
            {
                table.DeleteEntity("Session", login);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsSessionValid(string login, Guid sessionId)
        {
            var table = GetTableClient();
            try
            {
                var session = table.GetEntity<SessionEntity>("Session", login);
                return session.Value.SessionId == sessionId.ToString();
            }
            catch
            {
                return false;
            }
        }

        public bool Put(string nazwa, string tresc, Guid id_sesji)
        {
            var login = GetLoginBySession(id_sesji);
            if (login == null) return false;

            var container = GetBlobContainerClient();
            var blob = container.GetBlobClient(nazwa);

            var bytes = Encoding.UTF8.GetBytes(tresc);
            using (var stream = new MemoryStream(bytes))
            {
                blob.Upload(stream, overwrite: true);
            }

            return true;
        }


        public string Get(string nazwa, Guid id_sesji)
        {
            var login = GetLoginBySession(id_sesji);
            if (login == null) return null;

            var container = GetBlobContainerClient();
            var blob = container.GetBlobClient(nazwa);
            if (!blob.Exists()) return null;

            var download = blob.DownloadContent();
            return download.Value.Content.ToString();
        }

        private string GetLoginBySession(Guid sessionId)
        {
            var table = GetTableClient();
            Pageable<SessionEntity> sessions = table.Query<SessionEntity>(s => s.PartitionKey == "Session");

            foreach (var session in sessions)
            {
                if (session.SessionId == sessionId.ToString())
                    return session.RowKey;
            }
            return null;
        }
    }
}
