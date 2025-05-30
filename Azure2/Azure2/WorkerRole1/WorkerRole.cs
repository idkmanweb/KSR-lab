using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WorkerRole : RoleEntryPoint
{
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

    private const string StorageConnectionString = "UseDevelopmentStorage=true";
    private const string InputContainerName = "input";
    private const string OutputContainerName = "output";
    private const string QueueName = "blobencodequeue";

    private QueueClient queueClient;
    private BlobContainerClient inputContainerClient;
    private BlobContainerClient outputContainerClient;

    public override bool OnStart()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        ServicePointManager.DefaultConnectionLimit = 12;

        var blobServiceClient = new BlobServiceClient(StorageConnectionString);

        queueClient = new QueueClient(StorageConnectionString, QueueName);
        queueClient.CreateIfNotExists();

        inputContainerClient = blobServiceClient.GetBlobContainerClient(InputContainerName);
        inputContainerClient.CreateIfNotExists();

        outputContainerClient = blobServiceClient.GetBlobContainerClient(OutputContainerName);
        outputContainerClient.CreateIfNotExists();

        Trace.TraceInformation("WorkerRole starting");

        return base.OnStart();
    }

    public override void Run()
    {
        Trace.TraceInformation("WorkerRole running");
        try
        {
            RunAsync(cancellationTokenSource.Token).Wait();
        }
        finally
        {
            runCompleteEvent.Set();
        }
    }

    public override void OnStop()
    {
        Trace.TraceInformation("WorkerRole stopping");

        cancellationTokenSource.Cancel();
        runCompleteEvent.WaitOne();

        base.OnStop();

        Trace.TraceInformation("WorkerRole stopped");
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        Random random = new Random();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var receivedMessage = await queueClient.ReceiveMessageAsync();

                if (receivedMessage.Value != null)
                {
                    string blobName = receivedMessage.Value.MessageText;
                    Trace.TraceInformation($"Received message: {blobName}");

                    bool success = false;
                    int retryCount = 0;
                    const int maxRetries = 5;

                    while (!success && retryCount < maxRetries)
                    {
                        try
                        {
                            var inputBlob = inputContainerClient.GetBlobClient(blobName);

                            if (!await inputBlob.ExistsAsync())
                            {
                                Trace.TraceWarning($"Blob '{blobName}' does not exist.");
                                await queueClient.DeleteMessageAsync(receivedMessage.Value.MessageId, receivedMessage.Value.PopReceipt);
                                break;
                            }

                            string content;
                            using (var stream = await inputBlob.OpenReadAsync())
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                content = await reader.ReadToEndAsync();
                            }

                            string encodedContent = Rot13(content, random);

                            var outputBlob = outputContainerClient.GetBlobClient(blobName);
                            using (var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(encodedContent)))
                            {
                                await outputBlob.UploadAsync(outputStream, overwrite: true);
                            }

                            await queueClient.DeleteMessageAsync(receivedMessage.Value.MessageId, receivedMessage.Value.PopReceipt);

                            Trace.TraceInformation($"Blob '{blobName}' processed successfully.");
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            retryCount++;
                            Trace.TraceWarning($"Attempt {retryCount} failed for blob '{blobName}': {ex.Message}");
                            if (retryCount >= maxRetries)
                            {
                                Trace.TraceError($"Max retries reached for blob '{blobName}'. Skipping.");
                                break;
                            }
                            await Task.Delay(1000);
                        }
                    }
                }
                else
                {
                    await Task.Delay(1000); 
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Unexpected error: {ex.Message}");
                await Task.Delay(5000);
            }
        }
    }

    private string Rot13(string input, Random random)
    {
        if (random.Next(3) == 0)
        {
            throw new Exception("Random ROT13 failure.");
        }

        char Rot13Char(char c)
        {
            if ('a' <= c && c <= 'z')
                return (char)('a' + (c - 'a' + 13) % 26);
            if ('A' <= c && c <= 'Z')
                return (char)('A' + (c - 'A' + 13) % 26);
            return c;
        }

        var output = new char[input.Length];
        for (int i = 0; i < input.Length; i++)
            output[i] = Rot13Char(input[i]);
        return new string(output);
    }
}
