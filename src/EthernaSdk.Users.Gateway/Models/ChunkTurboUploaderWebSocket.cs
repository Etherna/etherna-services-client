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
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Gateway.Models
{
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    public sealed class ChunkTurboUploaderWebSocket(
        ushort chunkBatchMaxSize,
        WebSocket webSocket)
        : ChunkUploaderWebSocket(webSocket)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    {
        // Fields.
        private readonly byte[] responseBuffer = new byte[SwarmHash.HashSize]; //not really used
        
        // Methods.
        public override Task SendChunkAsync(SwarmChunk chunk, CancellationToken cancellationToken) =>
            SendChunksAsync([chunk], cancellationToken);

        /// <summary>
        /// Send chunk batch to BeeTurbo
        /// </summary>
        public override async Task SendChunksAsync(
            SwarmChunk[] chunkBatch,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkBatch, nameof(chunkBatch));
            if (chunkBatch.Length > chunkBatchMaxSize)
                throw new ArgumentOutOfRangeException(nameof(chunkBatch), "The chunk batch is larger than max size");

            //send amount of chunks in batch
            var chunkBatchSizeByteArray = BitConverter.GetBytes((ushort)chunkBatch.Length);
            await webSocket.SendAsync(chunkBatchSizeByteArray, WebSocketMessageType.Binary, false, cancellationToken)
                .ConfigureAwait(false);

            //send chunks
            for (var j = 0; j < chunkBatch.Length; j++)
            {
                var chunkBytes = chunkBatch[j].GetSpanAndData();
                var chunkSizeByteArray = BitConverter.GetBytes((ushort)chunkBytes.Length);

                //send chunk size
                await webSocket.SendAsync(
                    chunkSizeByteArray,
                    WebSocketMessageType.Binary,
                    false,
                    cancellationToken).ConfigureAwait(false);

                //send chunk data
                await webSocket.SendAsync(
                    chunkBytes,
                    WebSocketMessageType.Binary,
                    j + 1 == chunkBatch.Length,
                    cancellationToken).ConfigureAwait(false);
            }

            //wait response
            var response = await webSocket.ReceiveAsync(responseBuffer, CancellationToken.None).ConfigureAwait(false);
            if (response.MessageType == WebSocketMessageType.Close)
                throw new OperationCanceledException(
                    $"Connection closed by server, message: {response.CloseStatusDescription}");
        }
    }
}