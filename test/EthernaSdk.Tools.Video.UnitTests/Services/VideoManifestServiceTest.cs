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

using Etherna.BeeNet;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using Etherna.Sdk.Tools.Video.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.Sdk.Tools.Video.Services
{
    public class VideoManifestServiceTest
    {
        // Classes.
        public class ParseManifestTestElement(
            SwarmHash manifestHash,
            Dictionary<SwarmAddress, byte[]> swarmResources,
            Dictionary<SwarmAddress, SwarmHash> swarmAdditionalResources,
            PublishedVideoManifest expectedManifest)
        {
            public SwarmHash ManifestHash { get; } = manifestHash;
            public Dictionary<SwarmAddress, byte[]> SwarmResources { get; } = swarmResources;
            public Dictionary<SwarmAddress, SwarmHash> SwarmAdditionalResources { get; } = swarmAdditionalResources;
            public PublishedVideoManifest ExpectedManifest { get; } = expectedManifest;
        }

        // Data.
        public static IEnumerable<object[]> ParseManifestTests
        {
            get
            {
                var tests = new List<ParseManifestTestElement>
                {
                    //v2.1
                    new(SwarmHash.Zero,
                        new Dictionary<SwarmAddress, byte[]>
                        {
                            [SwarmHash.Zero] = """{"v":"2.1","title":"I'm a title","createdAt":1724285934,"updatedAt":1724285934,"ownerAddress":"0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246","duration":587,"thumbnail":{"aspectRatio":1.7777778,"blurhash":"UEENPeIpX=w=f*j[ngRktTj=adtSaKjYo#R*","sources":[{"width":480,"type":"jpeg","path":"thumb/270.jpg"},{"width":960,"type":"jpeg","path":"thumb/540.jpg"}]}}"""u8.ToArray(),
                            [new SwarmAddress(SwarmHash.Zero, "/details")] = """{"description":"my description","aspectRatio":1.7777778,"batchId":"9d4d4e923cc054a94f7884772f5f4e588be0c3ec10c3ec397b64c7307c4c3336","personalData":"{\u0022v\u0022:\u00221\u0022,\u0022cliName\u0022:\u0022EthernaImporter\u0022,\u0022cliV\u0022:\u00220.3.9.0\u0022,\u0022srcName\u0022:\u0022youtube\u0022,\u0022srcVId\u0022:\u00225a7ace3d1c209f7f1982fa4ed0113d8fb35bdbf2698761cb4f8da276bbef3993\u0022}","sources":[{"type":"hls","path":"sources/hls/master.m3u8","size":0},{"type":"hls","quality":"480p","path":"sources/hls/480p/playlist.m3u8","size":2471},{"type":"hls","quality":"360p","path":"sources/hls/360p/playlist.m3u8","size":2448}],"captions":[{"label":"Arabic","lang":"ar","path":"captions/0.vtt"},{"label":"Bulgarian","lang":"bg","path":"captions/1.vtt"},{"label":"Catalan","lang":"ca","path":"captions/2.vtt"}]}"""u8.ToArray(),
                            [new SwarmAddress(SwarmHash.Zero, "/sources/hls/480p/playlist.m3u8")] =
                                """
                                #EXTM3U
                                #EXT-X-VERSION:3
                                #EXT-X-TARGETDURATION:13
                                #EXT-X-MEDIA-SEQUENCE:0
                                #EXT-X-PLAYLIST-TYPE:VOD
                                #EXTINF:9.800000,
                                0.ts
                                #EXTINF:10.000000,
                                1.ts
                                #EXTINF:10.000000,
                                2.ts
                                #EXT-X-ENDLIST
                                """u8.ToArray(),
                            [new SwarmAddress(SwarmHash.Zero, "/sources/hls/360p/playlist.m3u8")] =
                                """
                                #EXTM3U
                                #EXT-X-VERSION:3
                                #EXT-X-TARGETDURATION:14
                                #EXT-X-MEDIA-SEQUENCE:0
                                #EXT-X-PLAYLIST-TYPE:VOD
                                #EXTINF:9.800000,
                                0.ts
                                #EXTINF:10.000000,
                                1.ts
                                #EXTINF:10.000000,
                                2.ts
                                #EXT-X-ENDLIST
                                """u8.ToArray()
                        },
                        new Dictionary<SwarmAddress, SwarmHash>
                        {
                            [new SwarmAddress(SwarmHash.Zero, "captions/0.vtt")] = "0000000000000000000000000000000000000000000000000000000000000007",
                            [new SwarmAddress(SwarmHash.Zero, "captions/1.vtt")] = "0000000000000000000000000000000000000000000000000000000000000008",
                            [new SwarmAddress(SwarmHash.Zero, "captions/2.vtt")] = "0000000000000000000000000000000000000000000000000000000000000009",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/master.m3u8")] = "0000000000000000000000000000000000000000000000000000000000000012",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/playlist.m3u8")] = "0000000000000000000000000000000000000000000000000000000000000013",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/0.ts")] = "0000000000000000000000000000000000000000000000000000000000000001",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/1.ts")] = "0000000000000000000000000000000000000000000000000000000000000002",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/2.ts")] = "0000000000000000000000000000000000000000000000000000000000000003",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/playlist.m3u8")] = "0000000000000000000000000000000000000000000000000000000000000014",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/0.ts")] = "0000000000000000000000000000000000000000000000000000000000000004",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/1.ts")] = "0000000000000000000000000000000000000000000000000000000000000005",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/2.ts")] = "0000000000000000000000000000000000000000000000000000000000000006",
                            [new SwarmAddress(SwarmHash.Zero, "thumb/270.jpg")] = "0000000000000000000000000000000000000000000000000000000000000010",
                            [new SwarmAddress(SwarmHash.Zero, "thumb/540.jpg")] = "0000000000000000000000000000000000000000000000000000000000000011",
                        },
                        new PublishedVideoManifest(SwarmHash.Zero, new VideoManifest(
                            1.7777778f,
                            PostageBatchId.FromString("9d4d4e923cc054a94f7884772f5f4e588be0c3ec10c3ec397b64c7307c4c3336"),
                            DateTimeOffset.Parse("8/22/2024 12:18:54 AM +00:00"),
                            "my description",
                            TimeSpan.FromSeconds(587),
                            "I'm a title",
                            "0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246",
                            """{"v":"1","cliName":"EthernaImporter","cliV":"0.3.9.0","srcName":"youtube","srcVId":"5a7ace3d1c209f7f1982fa4ed0113d8fb35bdbf2698761cb4f8da276bbef3993"}""",
                            [
                                new VideoManifestVideoSource(
                                    "master.m3u8",
                                    VideoType.Hls,
                                    null,
                                    0,
                                    [],
                                    "0000000000000000000000000000000000000000000000000000000000000012"),
                                new VideoManifestVideoSource(
                                    "480p/playlist.m3u8",
                                    VideoType.Hls,
                                    "480p",
                                    2471,
                                    [
                                        new VideoManifestVideoSourceAdditionalFile("480p/0.ts", "0000000000000000000000000000000000000000000000000000000000000001"),
                                        new VideoManifestVideoSourceAdditionalFile("480p/1.ts", "0000000000000000000000000000000000000000000000000000000000000002"),
                                        new VideoManifestVideoSourceAdditionalFile("480p/2.ts", "0000000000000000000000000000000000000000000000000000000000000003")
                                    ],
                                    "0000000000000000000000000000000000000000000000000000000000000013"),
                                new VideoManifestVideoSource(
                                    "360p/playlist.m3u8",
                                    VideoType.Hls,
                                    "360p",
                                    2448,
                                    [
                                        new VideoManifestVideoSourceAdditionalFile("360p/0.ts", "0000000000000000000000000000000000000000000000000000000000000004"),
                                        new VideoManifestVideoSourceAdditionalFile("360p/1.ts", "0000000000000000000000000000000000000000000000000000000000000005"),
                                        new VideoManifestVideoSourceAdditionalFile("360p/2.ts", "0000000000000000000000000000000000000000000000000000000000000006")
                                    ],
                                    "0000000000000000000000000000000000000000000000000000000000000014")
                            ],
                            new VideoManifestImage(
                                1.7777778f,
                                "UEENPeIpX=w=f*j[ngRktTj=adtSaKjYo#R*",
                                [
                                    new VideoManifestImageSource(
                                        "270.jpg",
                                        ImageType.Jpeg,
                                        480,
                                        "0000000000000000000000000000000000000000000000000000000000000010"),
                                    new VideoManifestImageSource(
                                        "540.jpg",
                                        ImageType.Jpeg,
                                        960,
                                        "0000000000000000000000000000000000000000000000000000000000000011")
                                ]),
                            [
                                new VideoManifestCaptionSource(
                                    "Arabic",
                                    "ar",
                                    "0.vtt",
                                    "0000000000000000000000000000000000000000000000000000000000000007"),
                                new VideoManifestCaptionSource(
                                    "Bulgarian",
                                    "bg",
                                    "1.vtt",
                                    "0000000000000000000000000000000000000000000000000000000000000008"),
                                new VideoManifestCaptionSource(
                                    "Catalan",
                                    "ca",
                                    "2.vtt",
                                    "0000000000000000000000000000000000000000000000000000000000000009")
                            ],
                            DateTimeOffset.Parse("8/22/2024 12:18:54 AM +00:00")),
                            [])),
                    
                    //v2.0
                    new(SwarmHash.Zero,
                        new Dictionary<SwarmAddress, byte[]>
                        {
                            [SwarmHash.Zero] = """{"v":"2.0","title":"I'm a title","createdAt":1724285934,"updatedAt":1724285934,"ownerAddress":"0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246","duration":587,"thumbnail":{"aspectRatio":1.7777778,"blurhash":"UEENPeIpX=w=f*j[ngRktTj=adtSaKjYo#R*","sources":[{"width":480,"type":"jpeg","path":"thumb/270.jpg"},{"width":960,"type":"jpeg","path":"thumb/540.jpg"}]}}"""u8.ToArray(),
                            [new SwarmAddress(SwarmHash.Zero, "/details")] = """{"description":"my description","aspectRatio":1.7777778,"batchId":"9d4d4e923cc054a94f7884772f5f4e588be0c3ec10c3ec397b64c7307c4c3336","personalData":"{\u0022v\u0022:\u00221\u0022,\u0022cliName\u0022:\u0022EthernaImporter\u0022,\u0022cliV\u0022:\u00220.3.9.0\u0022,\u0022srcName\u0022:\u0022youtube\u0022,\u0022srcVId\u0022:\u00225a7ace3d1c209f7f1982fa4ed0113d8fb35bdbf2698761cb4f8da276bbef3993\u0022}","sources":[{"type":"hls","path":"sources/hls/master.m3u8","size":0},{"type":"hls","quality":"480p","path":"sources/hls/480p/playlist.m3u8","size":2471},{"type":"hls","quality":"360p","path":"sources/hls/360p/playlist.m3u8","size":2448}]}"""u8.ToArray(),
                            [new SwarmAddress(SwarmHash.Zero, "/sources/hls/480p/playlist.m3u8")] =
                                """
                                #EXTM3U
                                #EXT-X-VERSION:3
                                #EXT-X-TARGETDURATION:13
                                #EXT-X-MEDIA-SEQUENCE:0
                                #EXT-X-PLAYLIST-TYPE:VOD
                                #EXTINF:9.800000,
                                0.ts
                                #EXTINF:10.000000,
                                1.ts
                                #EXTINF:10.000000,
                                2.ts
                                #EXT-X-ENDLIST
                                """u8.ToArray(),
                            [new SwarmAddress(SwarmHash.Zero, "/sources/hls/360p/playlist.m3u8")] =
                                """
                                #EXTM3U
                                #EXT-X-VERSION:3
                                #EXT-X-TARGETDURATION:14
                                #EXT-X-MEDIA-SEQUENCE:0
                                #EXT-X-PLAYLIST-TYPE:VOD
                                #EXTINF:9.800000,
                                0.ts
                                #EXTINF:10.000000,
                                1.ts
                                #EXTINF:10.000000,
                                2.ts
                                #EXT-X-ENDLIST
                                """u8.ToArray()
                        },
                        new Dictionary<SwarmAddress, SwarmHash>
                        {
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/master.m3u8")] = "0000000000000000000000000000000000000000000000000000000000000012",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/playlist.m3u8")] = "0000000000000000000000000000000000000000000000000000000000000013",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/0.ts")] = "0000000000000000000000000000000000000000000000000000000000000001",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/1.ts")] = "0000000000000000000000000000000000000000000000000000000000000002",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/480p/2.ts")] = "0000000000000000000000000000000000000000000000000000000000000003",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/playlist.m3u8")] = "0000000000000000000000000000000000000000000000000000000000000014",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/0.ts")] = "0000000000000000000000000000000000000000000000000000000000000004",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/1.ts")] = "0000000000000000000000000000000000000000000000000000000000000005",
                            [new SwarmAddress(SwarmHash.Zero, "sources/hls/360p/2.ts")] = "0000000000000000000000000000000000000000000000000000000000000006",
                            [new SwarmAddress(SwarmHash.Zero, "thumb/270.jpg")] = "0000000000000000000000000000000000000000000000000000000000000010",
                            [new SwarmAddress(SwarmHash.Zero, "thumb/540.jpg")] = "0000000000000000000000000000000000000000000000000000000000000011",
                        },
                        new PublishedVideoManifest(SwarmHash.Zero, new VideoManifest(
                            1.7777778f,
                            PostageBatchId.FromString("9d4d4e923cc054a94f7884772f5f4e588be0c3ec10c3ec397b64c7307c4c3336"),
                            DateTimeOffset.Parse("8/22/2024 12:18:54 AM +00:00"),
                            "my description",
                            TimeSpan.FromSeconds(587),
                            "I'm a title",
                            "0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246",
                            """{"v":"1","cliName":"EthernaImporter","cliV":"0.3.9.0","srcName":"youtube","srcVId":"5a7ace3d1c209f7f1982fa4ed0113d8fb35bdbf2698761cb4f8da276bbef3993"}""",
                            [
                                new VideoManifestVideoSource(
                                    "master.m3u8",
                                    VideoType.Hls,
                                    null,
                                    0,
                                    [],
                                    "0000000000000000000000000000000000000000000000000000000000000012"),
                                new VideoManifestVideoSource(
                                    "480p/playlist.m3u8",
                                    VideoType.Hls,
                                    "480p",
                                    2471,
                                    [
                                        new VideoManifestVideoSourceAdditionalFile("480p/0.ts", "0000000000000000000000000000000000000000000000000000000000000001"),
                                        new VideoManifestVideoSourceAdditionalFile("480p/1.ts", "0000000000000000000000000000000000000000000000000000000000000002"),
                                        new VideoManifestVideoSourceAdditionalFile("480p/2.ts", "0000000000000000000000000000000000000000000000000000000000000003")
                                    ],
                                    "0000000000000000000000000000000000000000000000000000000000000013"),
                                new VideoManifestVideoSource(
                                    "360p/playlist.m3u8",
                                    VideoType.Hls,
                                    "360p",
                                    2448,
                                    [
                                        new VideoManifestVideoSourceAdditionalFile("360p/0.ts", "0000000000000000000000000000000000000000000000000000000000000004"),
                                        new VideoManifestVideoSourceAdditionalFile("360p/1.ts", "0000000000000000000000000000000000000000000000000000000000000005"),
                                        new VideoManifestVideoSourceAdditionalFile("360p/2.ts", "0000000000000000000000000000000000000000000000000000000000000006")
                                    ],
                                    "0000000000000000000000000000000000000000000000000000000000000014")
                            ],
                            new VideoManifestImage(
                                1.7777778f,
                                "UEENPeIpX=w=f*j[ngRktTj=adtSaKjYo#R*",
                                [
                                    new VideoManifestImageSource(
                                        "270.jpg",
                                        ImageType.Jpeg,
                                        480,
                                        "0000000000000000000000000000000000000000000000000000000000000010"),
                                    new VideoManifestImageSource(
                                        "540.jpg",
                                        ImageType.Jpeg,
                                        960,
                                        "0000000000000000000000000000000000000000000000000000000000000011")
                                ]),
                            [],
                            DateTimeOffset.Parse("8/22/2024 12:18:54 AM +00:00")),
                            [])),
                    
                    //v1.1
                    new(SwarmHash.Zero,
                        new Dictionary<SwarmAddress, byte[]>
                        {
                            [SwarmHash.Zero] = """{"v":"1.1","title":"title 3","description":"test!!!","duration":19,"originalQuality":"1954p","ownerAddress":"0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246","createdAt":1660397733617,"updatedAt":1660397733617,"thumbnail":{"blurhash":"UTHoa;-VEVO=??v]SlOu2ep0slR:kisia*bJ","aspectRatio":1.7777777777777777,"sources":{"720w":"5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1","480w":"0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325"}},"sources":[{"reference":"e44671417466df08d3b67d74a081021ab2bba70224fc0d6e4d00c35d80328c6c","quality":"1954p","size":3739997,"bitrate":1574736}],"batchId":"5d35cbf4cea6349c1f74340ce9f0befd7a60a17426508da7b205871d683a3a23"}"""u8.ToArray()
                        },
                        new Dictionary<SwarmAddress, SwarmHash>
                        {
                            ["5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1"] = "5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1",
                            ["0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325"] = "0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325",
                            ["e44671417466df08d3b67d74a081021ab2bba70224fc0d6e4d00c35d80328c6c"] = "e44671417466df08d3b67d74a081021ab2bba70224fc0d6e4d00c35d80328c6c",
                        },
                        new PublishedVideoManifest(SwarmHash.Zero, new VideoManifest(
                            1.7777777777777777f,
                            PostageBatchId.FromString("5d35cbf4cea6349c1f74340ce9f0befd7a60a17426508da7b205871d683a3a23"),
                            DateTimeOffset.Parse("8/13/2022 1:35:33.617 PM +00:00"),
                            "test!!!",
                            TimeSpan.FromSeconds(19),
                            "title 3",
                            "0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246",
                            personalData: null,
                            [
                                new VideoManifestVideoSource(
                                    "1954p.mp4",
                                    VideoType.Mp4,
                                    "1954p",
                                    3739997,
                                    [],
                                    "e44671417466df08d3b67d74a081021ab2bba70224fc0d6e4d00c35d80328c6c")
                            ],
                            new VideoManifestImage(
                                1.7777777777777777f,
                                "UTHoa;-VEVO=??v]SlOu2ep0slR:kisia*bJ",
                                [
                                    new VideoManifestImageSource(
                                        "720.jpg",
                                        ImageType.Jpeg,
                                        720,
                                        "5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1"),
                                    new VideoManifestImageSource(
                                        "480.jpg",
                                        ImageType.Jpeg,
                                        480,
                                        "0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325")
                                ]),
                            [],
                            updatedAt: DateTimeOffset.Parse("8/13/2022 1:35:33.617 PM +00:00")),
                            [])),
                    
                    //v1.0
                    new(SwarmHash.Zero,
                        new Dictionary<SwarmAddress, byte[]>
                        {
                            [SwarmHash.Zero] = """{"title":"Test 1","description":"desc","createdAt":1645091199100,"duration":18,"originalQuality":"720p","ownerAddress":"0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246","thumbnail":{"blurhash":"UTHoa;-VEVO=??v]SlOu2ep0slR:kisia*bJ","aspectRatio":1.7777777777777777,"sources":{"720w":"5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1","480w":"0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325"}},"sources":[{"quality":"720p","reference":"94f4fcb1a902597c2bc53c5b48637af952a99328ec299f33e129740818a9e302","size":448350,"bitrate":216398}],"v":"1.0"}"""u8.ToArray()
                        },
                        new Dictionary<SwarmAddress, SwarmHash>
                        {
                            ["5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1"] = "5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1",
                            ["0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325"] = "0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325",
                            ["94f4fcb1a902597c2bc53c5b48637af952a99328ec299f33e129740818a9e302"] = "94f4fcb1a902597c2bc53c5b48637af952a99328ec299f33e129740818a9e302"
                        },
                        new PublishedVideoManifest(SwarmHash.Zero, new VideoManifest(
                            1.7777777777777777f,
                            PostageBatchId.Zero,
                            DateTimeOffset.Parse("2/17/2022 9:46:39.100 AM +00:00"),
                            "desc",
                            TimeSpan.FromSeconds(18), 
                            "Test 1",
                            "0x6163C4b8264a03CCAc412B83cbD1B551B6c6C246",
                            personalData: null,
                            [
                                new VideoManifestVideoSource(
                                    "720p.mp4",
                                    VideoType.Mp4,
                                    "720p",
                                    448350,
                                    [],
                                    "94f4fcb1a902597c2bc53c5b48637af952a99328ec299f33e129740818a9e302")
                            ],
                            new VideoManifestImage(
                                1.7777777777777777f,
                                "UTHoa;-VEVO=??v]SlOu2ep0slR:kisia*bJ",
                                [
                                    new VideoManifestImageSource(
                                        "720.jpg",
                                        ImageType.Jpeg,
                                        720,
                                        "5d69d94f1ffa17560a88abc4a99aa40b0cabe6012766f51e5c19193887adacb1"),
                                    new VideoManifestImageSource(
                                        "480.jpg",
                                        ImageType.Jpeg,
                                        480,
                                        "0b7425036143ed65932ac64cd6c4ddb4f2fd3e9bd51ed0f13bd406926c45c325")
                                ]),
                            [],
                            updatedAt: null),
                            []))
                };

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Fact]
        public async Task CreateVideoManifestChunksAsync()
        {
            // Setup.
            var videoManifestService = new VideoManifestService(
                new Mock<IBeeClient>().Object,
                new ChunkService());
            
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
                    new VideoManifestVideoSource(
                        sourceRelativePath: "master.m3u8",
                        videoType: VideoType.Hls,
                        quality: null,
                        totalSourceSize: 0,
                        additionalFiles: [],
                        directContentHash: SwarmHash.Zero),
                    new VideoManifestVideoSource(
                        sourceRelativePath: "720p/playlist.m3u8",
                        videoType: VideoType.Hls,
                        quality: null,
                        totalSourceSize: 45678,
                        additionalFiles:
                        [
                            new("1.ts", SwarmHash.Zero),
                            new("2.ts", SwarmHash.Zero)
                        ],
                        directContentHash: SwarmHash.Zero)
                ],
                thumbnail: new VideoManifestImage(
                    aspectRatio: 0.123f,
                    "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
                    [
                        new VideoManifestImageSource(
                            fileName: "720.png",
                            imageType: ImageType.Png,
                            width: 720,
                            directContentHash: SwarmHash.Zero)
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
            var result = await videoManifestService.CreateVideoManifestChunksAsync(
                videoManifest,
                chunkDirectory.FullName);
            
            // Assert.
            Assert.Equal("dbdf9da90a4c1ad04899527ec7a6b35e3cac07709947ce23ee01ff3526cf494d", result);
            Assert.Equal(
                [
                    "0cc878d32c96126d47f63fbe391114ee1438cd521146fc975dea1546d302b6c0.chunk",
                    "8504f2a107ca940beafc4ce2f6c9a9f0968c62a5b5893ff0e4e1e2983048d276.chunk",
                    "8d32d3dbda22b76d7b5d27237c38ec37f65db202a995a84955a8808316a70a13.chunk",
                    "a966438c28b6566f5762471c52cbd4e3d83445f2ed07d20e93b9205f6c9e9998.chunk",
                    "dbdf9da90a4c1ad04899527ec7a6b35e3cac07709947ce23ee01ff3526cf494d.chunk",
                    "dd031c128974182a84ccc69a3a6d1fdfd6962ac958f215d012866cce425281b4.chunk",
                    "e189fae0c2bbd455c1d81bc53817e173cf292bd0e9cfd1a3f7e8d02c02534344.chunk",
                    "e250fc8865894b98b21a28002decf162874f00a81f44c8af96c1249bef84c3fc.chunk"
                ],
                Directory.GetFiles(chunkDirectory.FullName).Select(Path.GetFileName).Order());
            
            // Cleanup.
            Directory.Delete(chunkDirectory.FullName, true);
        }
        
        [Theory, MemberData(nameof(ParseManifestTests))]
        public async Task ParseManifestAsync(ParseManifestTestElement test)
        {
            ArgumentNullException.ThrowIfNull(test, nameof(test));
            
            // Setup.
            var beeMock = new Mock<IBeeClient>();
            foreach (var (resourceAddress, resourceByteArray) in test.SwarmResources)
            {
                var resourceStream = new MemoryStream(resourceByteArray);
                beeMock.Setup(b => b.GetFileAsync(
                        resourceAddress,
                        It.IsAny<bool?>(),
                        It.IsAny<RedundancyStrategy?>(),
                        It.IsAny<bool?>(),
                        It.IsAny<string?>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(
                        new FileResponse(null, new Dictionary<string, IEnumerable<string>>(), resourceStream)));
            }
            foreach (var (resourceAddress, resourceHash) in test.SwarmAdditionalResources)
            {
                beeMock.Setup(b => b.ResolveAddressToChunkReferenceAsync(resourceAddress))
                    .Returns(Task.FromResult(new SwarmChunkReference(resourceHash, null, false)));
            }
        
            VideoManifestService videoManifestService = new(
                beeMock.Object,
                new Mock<ChunkService>().Object);

            // Action.
            var videoManifest = await videoManifestService.GetPublishedVideoManifestAsync(test.ManifestHash);

            // Assert.
            Assert.Equal(test.ExpectedManifest, videoManifest);
        }
    }
}