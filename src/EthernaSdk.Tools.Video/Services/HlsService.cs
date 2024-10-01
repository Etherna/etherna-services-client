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

using Etherna.BeeNet;
using Etherna.BeeNet.Models;
using Etherna.Sdk.Tools.Video.Models;
using Etherna.UniversalFiles;
using M3U8Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Etherna.Sdk.Tools.Video.Services
{
    public class HlsService(
        IBeeClient beeClient,
        IUFileProvider uFileProvider)
        : IHlsService
    {
        public async Task<HlsVideoEncoding> ParseVideoEncodingFromHlsMasterPlaylistFileAsync(
            TimeSpan duration,
            FileBase masterFile,
            SwarmAddress? masterSwarmAddress,
            MasterPlaylist masterPlaylist)
        {
            ArgumentNullException.ThrowIfNull(masterFile, nameof(masterFile));
            ArgumentNullException.ThrowIfNull(masterPlaylist, nameof(masterPlaylist));

            // Get master playlist directory.
            var masterFileDirectory = Path.GetDirectoryName(masterFile.UUri.OriginalUri);
            if (masterFileDirectory is null)
                throw new InvalidOperationException($"Can't get parent directory of {masterFile.UUri.OriginalUri}");
                        
            // Build video variants from streams on master playlist.
            List<HlsVideoVariant> variants = [];
            foreach (var streamInfo in masterPlaylist.Streams)
            {
                // Read stream info.
                var streamAbsoluteUri = Path.Combine(masterFileDirectory, streamInfo.Uri);
                UUri streamUUri = masterFile.UUri switch
                {
                    BasicUUri _ => new BasicUUri(streamAbsoluteUri),
                    SwarmUUri _ => new SwarmUUri(streamAbsoluteUri),
                    _ => throw new InvalidOperationException()
                };

                // Build stream playlist file.
                var streamPlaylistFile = await FileBase.BuildFromUFileAsync(
                    uFileProvider.BuildNewUFile(streamUUri)).ConfigureAwait(false);
                SwarmAddress? streamSwarmAddress = null;
                if (masterSwarmAddress is not null)
                {
                    streamSwarmAddress = SwarmAddress.FromString(masterFileDirectory.TrimEnd(SwarmAddress.Separator) + SwarmAddress.Separator + streamInfo.Uri);
                    var streamSwarmChunkRef = await beeClient.ResolveAddressToChunkReferenceAsync(streamSwarmAddress.Value).ConfigureAwait(false);
                    streamPlaylistFile.SwarmHash = streamSwarmChunkRef.Hash;
                }
                
                // Parse stream playlist.
                var variant = await ParseVideoVariantFromHlsStreamPlaylistFileAsync(
                    streamPlaylistFile,
                    streamSwarmAddress,
                    (int)streamInfo.Resolution.Height,
                    (int)streamInfo.Resolution.Width).ConfigureAwait(false);
                
                variants.Add(variant);
            }
                        
            return new HlsVideoEncoding(
                duration,
                masterFileDirectory,
                masterFile,
                variants.ToArray());
        }

        public async Task<HlsVideoVariant> ParseVideoVariantFromHlsStreamPlaylistFileAsync(
            FileBase streamPlaylistFile,
            SwarmAddress? streamPlaylistSwarmAddress,
            int height,
            int width)
        {
            ArgumentNullException.ThrowIfNull(streamPlaylistFile, nameof(streamPlaylistFile));

            // Get stream playlist directory.
            var streamPlaylistDirectory = Path.GetDirectoryName(streamPlaylistFile.UUri.OriginalUri);
            if (streamPlaylistDirectory is null)
                throw new InvalidOperationException(
                    $"Can't get parent directory of {streamPlaylistFile.UUri.OriginalUri}");

            // Parse segments.
            var streamPlaylist =
                MediaPlaylist.LoadFromText(await streamPlaylistFile.ReadToStringAsync().ConfigureAwait(false));
            List<FileBase> segmentFiles = [];
            foreach (var segment in streamPlaylist.MediaSegments.First().Segments)
            {
                // Read segments info.
                var segmentAbsoluteUri = Path.Combine(streamPlaylistDirectory, segment.Uri);
                UUri segmentUUri = streamPlaylistFile.UUri switch
                {
                    BasicUUri _ => new BasicUUri(segmentAbsoluteUri, UUriKind.LocalAbsolute),
                    SwarmUUri _ => new SwarmUUri(segmentAbsoluteUri, UUriKind.OnlineAbsolute),
                    _ => throw new InvalidOperationException()
                };

                // Build segment file.
                var segmentFile = await FileBase.BuildFromUFileAsync(uFileProvider.BuildNewUFile(segmentUUri))
                    .ConfigureAwait(false);
                if (streamPlaylistSwarmAddress is not null)
                {
                    var segmentSwarmAddress = SwarmAddress.FromString(
                        streamPlaylistDirectory.TrimEnd(SwarmAddress.Separator) + SwarmAddress.Separator + segment.Uri);
                    var segmentSwarmChunkRef = await beeClient.ResolveAddressToChunkReferenceAsync(segmentSwarmAddress)
                        .ConfigureAwait(false);
                    segmentFile.SwarmHash = segmentSwarmChunkRef.Hash;
                }

                segmentFiles.Add(segmentFile);
            }

            return new HlsVideoVariant(
                streamPlaylistFile,
                segmentFiles.ToArray(),
                height,
                width);
        }

        public async Task<MasterPlaylist?> TryParseHlsMasterPlaylistFromFileAsync(FileBase hlsPlaylist)
        {
            ArgumentNullException.ThrowIfNull(hlsPlaylist, nameof(hlsPlaylist));
            
            var hlsPlaylistText = await hlsPlaylist.ReadToStringAsync().ConfigureAwait(false);
            
            // We need to exclude at first that this is a stream playlist.
            // In fact, parsing a stream playlist as a master playlist gives false positive,
            // but parsing a master playlist as a stream playlist throws exception.
            try
            {
                MediaPlaylist.LoadFromText(hlsPlaylistText); //must throw exception.
                return null;
            }
            catch (NullReferenceException) { }

            return MasterPlaylist.LoadFromText(hlsPlaylistText);
        }
    }
}