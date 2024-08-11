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
        private readonly VideoPublisherService videoPublisherService = new(new ChunkService());

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
                        sourceRelativePath: "master.m3u8",
                        swarmHash: SwarmHash.Zero,
                        videoType: VideoType.Hls,
                        quality: null,
                        totalSourceSize: 0,
                        additionalFiles: []),
                    new VideoManifestVideoSource(
                        sourceRelativePath: "720p/playlist.m3u8",
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
            Assert.Equal("06902415095c715c4f09469110f1a66f0734f176e993a59a2497b376d4215611", result);
            Assert.Equal(
                new[]
                {
                    "06902415095c715c4f09469110f1a66f0734f176e993a59a2497b376d4215611.chunk",
                    "0cc878d32c96126d47f63fbe391114ee1438cd521146fc975dea1546d302b6c0.chunk",
                    "1b5f8d76cf6138abe1be02c5c7a99e9f68f20f9d7aba2bcf43102a90a10b5336.chunk",
                    "4db76c367ba03914e71db8e182095247cb2b96d03ce70f414c99db482be13b95.chunk",
                    "6c9fd645de3400e910825230f99e29b266be1ae159d939198df347957da1fc32.chunk",
                    "7242541ed2fc108f8f90a9c73604ca82171a66495f8c5e77053a43f484798e05.chunk",
                    "74078a89d6464471fd925f725ecd61675b136b12a1e15f42d14f009d0787dce3.chunk",
                    "8504f2a107ca940beafc4ce2f6c9a9f0968c62a5b5893ff0e4e1e2983048d276.chunk",
                    "d49adebbe9e0c03b4a38ca853e29b589330900dc4de42dc64a01d05b6ca39093.chunk",
                    "e3d7946f40bd3163ab914286fdd0ecf52df00b8ac29083a580f393b6e37043d5.chunk"
                },
                Directory.GetFiles(chunkDirectory.FullName).Select(Path.GetFileName).Order());
            
            // Cleanup.
            Directory.Delete(chunkDirectory.FullName, true);
        }
    }
}