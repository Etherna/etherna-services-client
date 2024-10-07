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
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Gateway.Models
{
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    public sealed class ChunkTurboUploaderWebSocket(
        ushort chunkBatchMaxSize,
        WebSocket webSocket) : IDisposable
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    {
        // Fields.
        private readonly byte[] responseBuffer = new byte[SwarmHash.HashSize]; //not really used

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

        /// <summary>
        /// Send chunk batch to BeeTurbo
        /// </summary>
        public async Task SendChunksAsync(
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

            // Wait response.
            var response = await webSocket.ReceiveAsync(responseBuffer, CancellationToken.None).ConfigureAwait(false);
            if (response.MessageType == WebSocketMessageType.Close)
                throw new OperationCanceledException(
                    $"Connection closed by server, message: {response.CloseStatusDescription}");
        }
    }
}