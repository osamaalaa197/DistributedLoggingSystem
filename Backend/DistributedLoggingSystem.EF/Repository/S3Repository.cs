using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.EF.Repository
{
    public class S3Repository : IS3Repository
    {
        private readonly S3StorageConfig _S3StorageOption;
        private readonly HttpClient _httpClient;

        public S3Repository(IOptions<S3StorageConfig> S3StorageOption, HttpClient httpClient)
        {
            _S3StorageOption = S3StorageOption.Value;
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<LogEntry>> RetrieveAllLogsAsync()
        {
            var logs = new List<LogEntry>();

            var allLogObjects = await ListAllLogsAsync();
            foreach (var logObject in allLogObjects)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_S3StorageOption.Endpoint}/{_S3StorageOption.BucketName}/{logObject.ObjectKey}");
                SignRequest(request, "GET");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var logEntry = ParseLogContent(content, logObject.ObjectKey);
                if (logEntry != null)
                {
                    logs.Add(logEntry);
                }
            }

            return logs;
        }

        public async Task UploadFileAsync(LogEntry logEntry)
        {
            var objectKey = $"{logEntry.Service}/{logEntry.Timestamp:yyyy-MM-dd_HH-mm-ss}.txt";
            var content = $"{logEntry.Timestamp} [{logEntry.Level}] {logEntry.Service}: {logEntry.Message}";
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_S3StorageOption.Endpoint}/{_S3StorageOption.BucketName}/{objectKey}")
            {
                Content = new StringContent(content, Encoding.UTF8, "text/plain")
            };
            SignRequest(request, "PUT");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        private void SignRequest(HttpRequestMessage request, string method)
        {
            var date = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            request.Headers.Add("x-amz-date", date);

            var canonicalRequest = $"{method}\n/{_S3StorageOption.BucketName}/{request.RequestUri.AbsolutePath}\n\nhost:{_S3StorageOption.Endpoint}\nx-amz-date:{date}\n\nhost;x-amz-date\nUNSIGNED-PAYLOAD";
            var stringToSign = $"AWS4-HMAC-SHA256\n{date}\n{date.Substring(0, 8)}/us-east-1/s3/aws4_request\n{Hash(canonicalRequest)}";
            var signature = HmacSha256(stringToSign, _S3StorageOption.SecretKey);

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("AWS4-HMAC-SHA256", $"Credential={_S3StorageOption.AccessKey},SignedHeaders=host;x-amz-date,Signature={signature}");
        }
        private string Hash(string text)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string HmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private async Task<IEnumerable<S3LogMetadata>> ListAllLogsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_S3StorageOption.Endpoint}/{_S3StorageOption.BucketName}?list-type=2");
            SignRequest(request, "GET");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return ParseS3Response(content);
        }
        private IEnumerable<S3LogMetadata> ParseS3Response(string xmlContent)
        {
            var logs = new List<S3LogMetadata>();
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var nodes = xmlDoc.GetElementsByTagName("Contents");
            foreach (System.Xml.XmlNode node in nodes)
            {
                var key = node["Key"]?.InnerText;
                var lastModified = node["LastModified"]?.InnerText;

                if (!string.IsNullOrEmpty(key))
                {
                    logs.Add(new S3LogMetadata
                    {
                        ObjectKey = key,
                        LastModified = DateTime.Parse(lastModified)
                    });
                }
            }

            return logs;
        }

        private LogEntry ParseLogContent(string content, string objectKey)
        {
            try
            {
                var parts = content.Split(new[] { '[', ']', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4) return null;

                return new LogEntry
                {
                    Timestamp = DateTime.Parse(parts[0].Trim()),
                    Level = parts[1].Trim(),
                    Service = parts[2].Trim(),
                    Message = string.Join(':', parts.Skip(3)).Trim()
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
