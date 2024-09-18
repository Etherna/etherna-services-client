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

using Xunit;

namespace Etherna.Sdk.Tools.Video.Models
{
    public class VideoManifestPersonalDataTest
    {
        [Fact]
        public void CanDeserializeManifestPersonalData()
        {
            var rawPersonalData = """{"v":"1","cliName":"MyClient","cliV":"0.1.2","srcName":"MySource","srcVId":"myId"}""";

            var result = VideoManifestPersonalData.TryDeserialize(rawPersonalData, out var personalData);
            
            Assert.True(result);
            Assert.Equal("MyClient", personalData.ClientName);
            Assert.Equal("0.1.2", personalData.ClientVersion);
            Assert.Equal("MySource", personalData.SourceProviderName);
            Assert.Equal("myId", personalData.SourceVideoId);
        }
        
        [Fact]
        public void CanSerializeManifestPersonalData()
        {
            var personalData = new VideoManifestPersonalData(
                "MyClient",
                "0.1.2",
                "MySource",
                "myId");

            var result = personalData.Serialize();
            
            Assert.Equal(
                """{"v":"1","cliName":"MyClient","cliV":"0.1.2","srcName":"MySource","srcVId":"myId"}""",
                result);
        }
    }
}