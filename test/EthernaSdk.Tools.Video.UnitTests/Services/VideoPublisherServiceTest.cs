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
using Etherna.Sdk.Tools.Video.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.Sdk.Tools.Video.Services
{
    public class VideoPublisherServiceTest
    {
        // Fields.
        private readonly VideoPublisherService videoPublisherService = new(
            new ChunkService());

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
                videoSources:
                [
                    VideoManifestVideoSource.BuildFromNewContent(
                        sourceRelativePath: "master.m3u8",
                        contentSwarmHash: SwarmHash.Zero,
                        videoType: VideoType.Hls,
                        quality: null,
                        totalSourceSize: 0,
                        additionalFiles: []),
                    VideoManifestVideoSource.BuildFromNewContent(
                        sourceRelativePath: "720p/playlist.m3u8",
                        contentSwarmHash: SwarmHash.Zero,
                        videoType: VideoType.Hls,
                        quality: null,
                        totalSourceSize: 45678,
                        additionalFiles:
                        [
                            new("1.ts", SwarmHash.Zero),
                            new("2.ts", SwarmHash.Zero)
                        ])
                ],
                thumbnail: new VideoManifestImage(
                    aspectRatio: 0.123f,
                    "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
                    [
                        VideoManifestImageSource.BuildFromNewContent(
                            fileName: "720.png",
                            imageType: ImageType.Png,
                            contentSwarmHash: SwarmHash.Zero,
                            width: 720)
                    ]),
                captionSources:
                [
                    new VideoManifestCaptionSource(
                        "eng",
                        "en-uk",
                        "0.ts",
                        SwarmHash.Zero)
                ],
                updatedAt: new DateTimeOffset(2024, 07, 12, 12, 01, 08, TimeSpan.Zero));
            var chunkDirectory = Directory.CreateTempSubdirectory();

            // Run.
            var result = await videoPublisherService.CreateVideoManifestChunksAsync(
                videoManifest,
                chunkDirectory.FullName);
            
            // Assert.
            Assert.Equal("b1b982c642ccf2c989e71ca022f8242a70005da081020ceaa1fab1ec3f3be654", result);
            Assert.Equal(
                [
                    "0cc878d32c96126d47f63fbe391114ee1438cd521146fc975dea1546d302b6c0.chunk",
                    "6115e0287b1d06dfe32a0a47e13de4512742e2cc7c2bf0dd78ae090e25d224c3.chunk",
                    "8504f2a107ca940beafc4ce2f6c9a9f0968c62a5b5893ff0e4e1e2983048d276.chunk",
                    "a966438c28b6566f5762471c52cbd4e3d83445f2ed07d20e93b9205f6c9e9998.chunk",
                    "b1b982c642ccf2c989e71ca022f8242a70005da081020ceaa1fab1ec3f3be654.chunk",
                    "dd031c128974182a84ccc69a3a6d1fdfd6962ac958f215d012866cce425281b4.chunk",
                    "e250fc8865894b98b21a28002decf162874f00a81f44c8af96c1249bef84c3fc.chunk",
                    "ea159eff3a8d34080b78d5d06c856f6eb86050e7dd559229fe2517393ea4a11c.chunk"
                ],
                Directory.GetFiles(chunkDirectory.FullName).Select(Path.GetFileName).Order());
            
            // Cleanup.
            Directory.Delete(chunkDirectory.FullName, true);
        }
    }
}