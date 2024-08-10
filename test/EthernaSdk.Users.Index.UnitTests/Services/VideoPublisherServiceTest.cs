// Copyright 2020-present Etherna SA
// This file is part of Etherna SDK .Net.
// 
// Etherna SDK .Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna SDK .Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Etherna SDK .Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using Etherna.Sdk.Users.Index.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.Sdk.Users.Index.Services
{
    public class VideoPublisherServiceTest
    {
        // Fields.
        private readonly IChunkService chunkService;
        private readonly IVideoPublisherService videoPublisherService;

        // Constructor.
        public VideoPublisherServiceTest()
        {
            chunkService = new ChunkService();
            videoPublisherService = new VideoPublisherService(chunkService);
        }

        // Tests.
        [Fact]
        public async Task CreateVideoManifestChunksAsync()
        {
            // Prepare.
            var videoManifest = new VideoManifest(
                aspectRatio: 0.123f,
                batchId: "f389278a2fa242de94e858e318bbfa7c10489533797ff923f9aa4524fabfcd34",
                createdAt: new DateTimeOffset(2024, 07, 04, 16, 45, 42, TimeSpan.Zero),
                description: "My description",
                duration: TimeSpan.FromSeconds(42),
                title: "I'm a title",
                ownerEthAddress: "0x7cd4878e21d9ce3da6611ae27a1b73827af81374",
                personalData: "my personal data",
                videoSources: new[]
                {
                    new VideoManifestVideoSource(
                        fileName: "720p.m3u8",
                        swarmHash: SwarmHash.Zero,
                        videoType: VideoType.Hls,
                        quality: null,
                        totalSourceSize: 45678,
                        additionalFiles:
                        [
                            new("1.ts", SwarmHash.Zero, new SwarmUri("720p/1.ts", UriKind.Relative)),
                            new("2.ts", SwarmHash.Zero, new SwarmUri("720p/2.ts", UriKind.Relative))
                        ])
                },
                thumbnail: new VideoManifestImage(
                    aspectRatio: 0.123f,
                    "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
                    new[]
                    {
                        new VideoManifestImageSource(
                            fileName: "720.png",
                            imageType: ImageType.Png,
                            SwarmHash.Zero,
                            width: 720)
                    }),
                updatedAt: new DateTimeOffset(2024, 07, 12, 12, 01, 08, TimeSpan.Zero));
            var chunkDirectory = Directory.CreateTempSubdirectory();

            // Run.
            var result = await videoPublisherService.CreateVideoManifestChunksAsync(
                videoManifest,
                chunkDirectory.FullName);
            
            // Assert.
            Assert.Equal("389542c84e9bf611a119a6a71d64a49c931ec3f2ada23bcfc50592a4c5fdeb66", result);
            Assert.Equal(
                new[]
                {
                    "06342e7e027dfdb35b426d58a67e3bef58269dd61f641e10755ab93a789d3bc5.chunk",
                    "0cc878d32c96126d47f63fbe391114ee1438cd521146fc975dea1546d302b6c0.chunk",
                    "247b8d39d2e7e3ce0a7d7b6bb24d47e7bd5d656ef34ac50d29466bbed0f1b452.chunk",
                    "389542c84e9bf611a119a6a71d64a49c931ec3f2ada23bcfc50592a4c5fdeb66.chunk",
                    "4db76c367ba03914e71db8e182095247cb2b96d03ce70f414c99db482be13b95.chunk",
                    "7203b2e34f565dbb86003fb1cb4d98edb3309e12807feb525b79e65834029f88.chunk",
                    "7242541ed2fc108f8f90a9c73604ca82171a66495f8c5e77053a43f484798e05.chunk",
                    "8504f2a107ca940beafc4ce2f6c9a9f0968c62a5b5893ff0e4e1e2983048d276.chunk",
                    "e3d7946f40bd3163ab914286fdd0ecf52df00b8ac29083a580f393b6e37043d5.chunk"
                },
                Directory.GetFiles(chunkDirectory.FullName).Select(Path.GetFileName).Order());
            
            // Cleanup.
            Directory.Delete(chunkDirectory.FullName, true);
        }
    }
}