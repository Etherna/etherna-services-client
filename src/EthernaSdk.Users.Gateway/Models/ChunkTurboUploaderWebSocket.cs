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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Gateway.Models
{
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    public sealed class ChunkTurboUploaderWebSocket(
        ushort chunkBatchSize,
        WebSocket webSocket)
        : ChunkUploaderWebSocket(webSocket)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    {
        // Fields.
        private readonly byte[] responseBuffer = new byte[SwarmHash.HashSize]; //not really used
        
        // Methods.
        public override Task SendChunkAsync(SwarmChunk chunk, CancellationToken cancellationToken) =>
            SendChunksAsync([chunk], null, cancellationToken);

        /// <summary>
        /// Divide chunks in batches and send them to BeeTurbo
        /// </summary>
        [SuppressMessage("Performance", "CA1851:Possible multiple enumerations of \'IEnumerable\' collection")]
        public override async Task SendChunksAsync(
            IEnumerable<SwarmChunk> chunks,
            Action<int>? onChunkBatchSent = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunks, nameof(chunks));

            // Iterate on chunk batches.
            for (int i = 0; i < chunks.Count(); i += chunkBatchSize)
            {
                var chunksInBatch = chunks.Skip(i).Take(chunkBatchSize).ToArray();
                
                //send amount of chunks in batch
                var chunkBatchSizeByteArray = BitConverter.GetBytes((ushort)chunksInBatch.Length);
                await webSocket.SendAsync(chunkBatchSizeByteArray, WebSocketMessageType.Binary, false, cancellationToken).ConfigureAwait(false);
                
                //send chunks
                for (var j = 0; j < chunksInBatch.Length; j++)
                {
                    var chunkBytes = chunksInBatch[j].GetSpanAndData();
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
                        j + 1 == chunksInBatch.Length,
                        cancellationToken).ConfigureAwait(false);
                }
            
                //wait response
                var response = await webSocket.ReceiveAsync(responseBuffer, CancellationToken.None).ConfigureAwait(false);
                if (response.MessageType == WebSocketMessageType.Close)
                    throw new OperationCanceledException(
                        $"Connection closed by server, message: {response.CloseStatusDescription}");
                
                //invoke callback
                onChunkBatchSent?.Invoke(i + chunksInBatch.Length);
            }
        }
    }
}