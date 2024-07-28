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
    public class VideoManifestServiceTest
    {
        // Fields.
        private readonly ICalculatorService calculatorService;
        private readonly IVideoManifestService videoManifestService;

        // Constructor.
        public VideoManifestServiceTest()
        {
            calculatorService = new CalculatorService();
            videoManifestService = new VideoManifestService(calculatorService);
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
                ownerAddress: "0x7cd4878e21d9ce3da6611ae27a1b73827af81374",
                personalData: "my personal data",
                sources: new[]
                {
                    new VideoManifestVideoSource(
                        uri: new SwarmUri("sources/hls/720p.m3u8", UriKind.Relative),
                        type: VideoSourceType.Hls,
                        quality: null,
                        size: 45678)
                },
                thumbnail: new VideoManifestImage(
                    aspectRatio: 0.123f,
                    "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
                    new[]
                    {
                        new VideoManifestImageSource(
                            uri: new SwarmUri("thumb/720-png", UriKind.Relative),
                            type: "png",
                            width: 720)
                    }),
                updatedAt: new DateTimeOffset(2024, 07, 12, 12, 01, 08, TimeSpan.Zero));
            var chunkDirectory = Directory.CreateTempSubdirectory();

            // Run.
            var result = await videoManifestService.CreateVideoManifestChunksAsync(
                videoManifest,
                chunkDirectory.FullName);
            
            // Assert.
            Assert.Equal("9663df6f443a777d0aa4aed6338ea55fc5e9b9afed029426149261b327bd4f8d", result);
            Assert.Equal(
                new[]
                {
                    "0cc878d32c96126d47f63fbe391114ee1438cd521146fc975dea1546d302b6c0.chunk",
                    "247b8d39d2e7e3ce0a7d7b6bb24d47e7bd5d656ef34ac50d29466bbed0f1b452.chunk",
                    "3f8b3428d8c11677afd6fdcb7a6577232210d088d6026f127a3ac188e73918c4.chunk",
                    "7203b2e34f565dbb86003fb1cb4d98edb3309e12807feb525b79e65834029f88.chunk",
                    "9663df6f443a777d0aa4aed6338ea55fc5e9b9afed029426149261b327bd4f8d.chunk",
                    "f5efef27d24dc0626ab2408ef07ae4ed08f5d0612406b5c02bb5cf4462dcbe5d.chunk"
                },
                Directory.GetFiles(chunkDirectory.FullName).Select(Path.GetFileName).Order());
            
            // Cleanup.
            Directory.Delete(chunkDirectory.FullName, true);
        }
    }
}