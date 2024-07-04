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

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoManifestTest
    {
        // Classes.
        public class SerializeManifestTestElement(
            VideoManifest manifest,
            string expectedPreviewResult,
            string expectedDetailResult)
        {
            public VideoManifest Manifest { get; } = manifest;
            public string ExpectedDetailResult { get; } = expectedDetailResult;
            public string ExpectedPreviewResult { get; } = expectedPreviewResult;
        }
        
        // Data.
        public static IEnumerable<object[]> SerializeManifestTests
        {
            get
            {
                var tests = new List<SerializeManifestTestElement>
                {
                    new(new VideoManifest(
                            aspectRatio: 0.123f,
                            batchId: "f389278a2fa242de94e858e318bbfa7c10489533797ff923f9aa4524fabfcd34",
                            createdAt: new DateTimeOffset(2024, 07, 04, 16, 45, 42, TimeSpan.Zero),
                            description: "My description",
                            duration: TimeSpan.FromSeconds(42),
                            title: "I'm a title",
                            ownerAddress: "0x7cd4878e21d9ce3da6611ae27a1b73827af81374",
                            personalData: "my personal data",
                            sources: new []{ new VideoManifestVideoSource(
                                path: "sources/hls/720p.m3u8",
                                type: "hls",
                                quality: null,
                                size: 45678)},
                            thumbnail: new VideoManifestImage(
                                aspectRatio: 0.123f,
                                "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
                                new []{ new VideoManifestImageSource(
                                    path: "thumb/720-png",
                                    type: "png",
                                    width: 720) }),
                            updatedAt: new DateTimeOffset(2024, 07, 12, 12, 01, 08, TimeSpan.Zero)),
                        """{"v":"2.0","title":"I\u0027m a title","createdAt":1720111542,"updatedAt":1720785668,"ownerAddress":"0x7cd4878e21d9ce3da6611ae27a1b73827af81374","duration":42,"thumbnail":{"aspectRatio":0.123,"blurhash":"UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay","sources":[{"width":720,"type":"png","path":"thumb/720-png"}]}}""",
                        """{"description":"My description","aspectRatio":0.123,"sources":[{"type":"hls","path":"sources/hls/720p.m3u8","size":45678}],"batchId":"f389278a2fa242de94e858e318bbfa7c10489533797ff923f9aa4524fabfcd34","personalData":"my personal data"}""")
                };

                return tests.Select(t => new object[] { t });
            }
        }
        
        // Tests.
        [Theory, MemberData(nameof(SerializeManifestTests))]
        public void SerializeManifest(SerializeManifestTestElement test)
        {
            var previewResult = test.Manifest.SerializeManifestPreview();
            var detailResult = test.Manifest.SerializeManifestDetail();
            
            Assert.Equal(test.ExpectedPreviewResult, previewResult);
            Assert.Equal(test.ExpectedDetailResult, detailResult);
        }
    }
}