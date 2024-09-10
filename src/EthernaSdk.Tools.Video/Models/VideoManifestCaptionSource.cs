// Copyright 2022-present Etherna SA
// This file is part of Etherna Video Importer.
// 
// Etherna Video Importer is free software: you can redistribute it and/or modify it under the terms of the
// GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna Video Importer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Etherna Video Importer.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Models;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.Sdk.Tools.Video.Models
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    public class VideoManifestCaptionSource(
        string label,
        string languageCode,
        string fileName,
        SwarmHash contentSwarmHash)
    {
        // Properties.
        public SwarmHash ContentSwarmHash { get; } = contentSwarmHash;
        public string FileName { get; } = fileName;
        public string Label { get; } = label;
        public string LanguageCode { get; } = languageCode;
        public string MimeContentType => "text/vtt";
    }
}