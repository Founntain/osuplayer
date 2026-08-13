using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Services;

namespace OsuPlayer.Tests;

public class LyricsServiceCandidateTests
{
    [Test]
    public async Task LookupTriesOrdinaryAndUnicodeMetadataCandidates()
    {
        var handler = new CandidateAwareHandler();
        using var client = new HttpClient(handler);
        var cacheDirectory = Path.Combine(Path.GetTempPath(), $"osuplayer-lyrics-test-{Guid.NewGuid():N}");
        var service = new LyricsService(client, cacheDirectory);

        try
        {
            var result = await service.GetLyricsAsync(new LyricsLookupRequest(
                "Asu no Yozora Shoukaihan",
                "Yuaru",
                "アスノヨゾラ哨戒班",
                "ゆある"));

            Assert.Multiple(() =>
            {
                Assert.That(result.Status, Is.EqualTo(LyricsLookupStatus.Found));
                Assert.That(result.SongId, Is.EqualTo(428642839));
                Assert.That(handler.SearchQueries, Has.Some.Contains("Asu no Yozora Shoukaihan"));
                Assert.That(handler.SearchQueries, Has.Some.Contains("アスノヨゾラ哨戒班"));
            });
        }
        finally
        {
            if (Directory.Exists(cacheDirectory))
                Directory.Delete(cacheDirectory, true);
        }
    }

    private sealed class CandidateAwareHandler : HttpMessageHandler
    {
        public List<string> SearchQueries { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.RequestUri?.AbsolutePath.Contains("cloudsearch", StringComparison.Ordinal) == true)
            {
                var form = await request.Content!.ReadAsStringAsync(cancellationToken);
                var query = Uri.UnescapeDataString(form.Split('&')
                    .Single(part => part.StartsWith("s=", StringComparison.Ordinal))[2..]
                    .Replace('+', ' '));
                SearchQueries.Add(query);

                var json = query.Contains("アスノヨゾラ哨戒班", StringComparison.Ordinal)
                    ? """{"code":200,"result":{"songs":[{"id":428642839,"name":"アスノヨゾラ哨戒班","ar":[{"name":"ゆある"}]}]}}"""
                    : """{"code":200,"result":{"songs":[]}}""";
                return JsonResponse(json);
            }

            return JsonResponse("""{"lrc":{"lyric":"[00:01.00]test line"},"tlyric":{"lyric":""}}""");
        }

        private static HttpResponseMessage JsonResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
