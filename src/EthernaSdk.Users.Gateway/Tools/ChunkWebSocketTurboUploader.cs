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
using Etherna.BeeNet.Tools;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Gateway.Tools
{
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    public sealed class ChunkWebSocketTurboUploader(
        ushort chunkBatchMaxSize,
        WebSocket webSocket) : IChunkWebSocketUploader
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    {
        // Dispose.
        public void Dispose() =>
            webSocket.Dispose();

        // Methods.
        public async Task CloseAsync()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await webSocket.CloseOutputAsync(
                        WebSocketCloseStatus.NormalClosure,
                        null,
                        CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e) when (e is WebSocketException or OperationCanceledException)
                {
                }
            }
        }

        public Task SendChunkAsync(byte[] chunkPayload, CancellationToken cancellationToken) =>
            SendChunkAsync(SwarmChunk.BuildFromSpanAndData(SwarmHash.Zero, chunkPayload), cancellationToken);

        public Task SendChunkAsync(SwarmChunk chunk, CancellationToken cancellationToken) =>
            SendChunkBatchAsync([chunk], true, cancellationToken);

        /// <summary>
        /// Send chunk batch to BeeTurbo
        /// </summary>
        public async Task SendChunkBatchAsync(
            SwarmChunk[] chunkBatch,
            bool isLastBatch,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkBatch, nameof(chunkBatch));
            if (chunkBatch.Length > chunkBatchMaxSize)
                throw new ArgumentOutOfRangeException(nameof(chunkBatch), "The chunk batch is larger than max size");

            // Build payload.
            List<byte> sendPayload = [];
            for (var j = 0; j < chunkBatch.Length; j++)
            {
                var chunkBytes = chunkBatch[j].GetSpanAndData();
                var chunkSizeByteArray = BitConverter.GetBytes((ushort)chunkBytes.Length);

                //chunk size
                sendPayload.AddRange(chunkSizeByteArray);

                //chunk data
                sendPayload.AddRange(chunkBytes);
            }

            // Send.
            await webSocket.SendAsync(
                sendPayload.ToArray(),
                WebSocketMessageType.Binary,
                isLastBatch,
                cancellationToken).ConfigureAwait(false);
        }
    }
}