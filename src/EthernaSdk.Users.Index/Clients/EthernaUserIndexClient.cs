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
using Etherna.Sdk.Index.GenClients;
using Etherna.Sdk.Users.Index.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Index.Clients
{
    public class EthernaUserIndexClient : IEthernaUserIndexClient
    {
        // Fields.
        private readonly IBeeClient beeClient;
        private readonly CommentsClient generatedCommentsClient;
        private readonly ModerationClient generatedModerationClient;
        private readonly SearchClient generatedSearchClient;
        private readonly SystemClient generatedSystemClient;
        private readonly UsersClient generatedUsersClient;
        private readonly VideosClient generatedVideosClient;

        // Constructor.
        public EthernaUserIndexClient(
            Uri baseUrl,
            IBeeClient beeClient,
            HttpClient httpClient)
        {
            this.beeClient = beeClient;
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            generatedCommentsClient = new CommentsClient(baseUrl.AbsoluteUri, httpClient);
            generatedModerationClient = new ModerationClient(baseUrl.AbsoluteUri, httpClient);
            generatedSearchClient = new SearchClient(baseUrl.AbsoluteUri, httpClient);
            generatedSystemClient = new SystemClient(baseUrl.AbsoluteUri, httpClient);
            generatedUsersClient = new UsersClient(baseUrl.AbsoluteUri, httpClient);
            generatedVideosClient = new VideosClient(baseUrl.AbsoluteUri, httpClient);
        }

        // Methods.
        public Task AdminForceNewValidationByManifestHashAsync(
            SwarmHash manifestHash,
            CancellationToken cancellationToken = default) =>
            generatedSystemClient.ManifestAsync(manifestHash.ToString(), cancellationToken);

        public Task AdminForceNewValidationByVideoIdAsync(
            string videoId,
            CancellationToken cancellationToken = default) =>
            generatedSystemClient.VideoAsync(videoId, cancellationToken);

        public Task AdminReindexAllVideosAsync(CancellationToken cancellationToken = default) =>
            generatedSearchClient.ReindexAsync(cancellationToken);

        public async Task<Comment> CreateCommentAsync(
            string videoId,
            string commentText,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedVideosClient.Comments2PostAsync(
                videoId,
                commentText,
                cancellationToken).ConfigureAwait(false);
            return new Comment(
                id: response.Id,
                creationDateTime: response.CreationDateTime,
                isEditable: response.IsEditable,
                isFrozen: response.IsFrozen,
                ownerAddress: response.OwnerAddress,
                textHistory: response.TextHistory,
                videoId: response.VideoId);
        }

        public Task DeleteOwnedCommentAsync(string commentId, CancellationToken cancellationToken = default) =>
            generatedCommentsClient.CommentsAsync(commentId, cancellationToken);
        
        public async Task<IEnumerable<IndexedVideo>> GetAllVideosByOwnerAsync(string userAddress)
        {
            var videos = new List<IndexedVideo>();
            const int maxForPage = 100;

            PaginatedResult<IndexedVideo>? page = null;
            do
            {
                page = await GetVideosByOwnerAsync(
                    userAddress,
                    page is null ? 0 : page.CurrentPage + 1,
                    maxForPage).ConfigureAwait(false);
                videos.AddRange(page.Elements);
            } while (page.Elements.Any());

            return videos;
        }

        public async Task<IEnumerable<VideoValidationStatus>> GetBulkVideoValidationStatusByIdsAsync(
            IEnumerable<string> videoIds,
            CancellationToken cancellationToken = default) =>
            (await generatedVideosClient.BulkValidation2Async(videoIds, cancellationToken).ConfigureAwait(false)).Select(s => new VideoValidationStatus(
                errorDetails: s.ErrorDetails.Select(e => new VideoValidationErrorDetail(
                    errorMessage: e.ErrorMessage,
                    errorType: Enum.Parse<VideoValidationErrorDetail.ErrorTypes>(e.ErrorType.ToString()))),
                hash: s.Hash,
                isValid: s.IsValid,
                validationTime: s.ValidationTime,
                videoId: s.VideoId));

        public async Task<IEnumerable<VideoValidationStatus>> GetBulkVideoValidationStatusByManifestsAsync(
            IEnumerable<SwarmHash> manifestHashes,
            CancellationToken cancellationToken = default) =>
            (await generatedVideosClient.BulkValidationPut2Async(
                manifestHashes.Select(h => h.ToString()), cancellationToken).ConfigureAwait(false)).Select(s => new VideoValidationStatus(
                    errorDetails: s.ErrorDetails.Select(e => new VideoValidationErrorDetail(
                        errorMessage: e.ErrorMessage,
                        errorType: Enum.Parse<VideoValidationErrorDetail.ErrorTypes>(e.ErrorType.ToString()))),
                    hash: s.Hash,
                    isValid: s.IsValid,
                    validationTime: s.ValidationTime,
                    videoId: s.VideoId));

        public async Task<PaginatedResult<VideoPreview>> GetLastPublishedVideosAsync(int? page = null, int? take = null, CancellationToken cancellationToken = default)
        {
            // Get API result.
            var result = await generatedVideosClient.Latest3Async(page, take, cancellationToken).ConfigureAwait(false);

            // Build response.
            List<VideoPreview> videoPreviews = [];
            foreach (var v in result.Elements)
            {
                List<VideoManifestImageSource> thumbnailSources = [];
                foreach (var thumbSource in v.Thumbnail.Sources)
                    thumbnailSources.Add(
                        await BuildVideoManifestImageSourceAsync(thumbSource, v.Hash).ConfigureAwait(false));
                
                videoPreviews.Add(new VideoPreview(
                    hash: v.Hash is not null ?
                        v.Hash : (SwarmHash?)null,
                    id: v.Id,
                    createdAt: v.CreatedAt,
                    duration: v.Duration,
                    ownerAddress: v.OwnerAddress,
                    thumbnail: new VideoManifestImage(
                        aspectRatio: v.Thumbnail.AspectRatio,
                        blurhash: v.Thumbnail.Blurhash,
                        sources: thumbnailSources),
                    title: v.Title,
                    updatedAt: v.UpdatedAt));
            }
            
            return new PaginatedResult<VideoPreview>(
                videoPreviews,
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<PaginatedResult<IndexUserInfo>> GetRegisteredUsersAsync(int? page = null, int? take = null, CancellationToken cancellationToken = default)
        {
            var result = await generatedUsersClient.List2Async(page, take, cancellationToken).ConfigureAwait(false);
            return new PaginatedResult<IndexUserInfo>(
                result.Elements.Select(u => new IndexUserInfo(
                    id: u.Id,
                    address: u.Address,
                    creationDateTime: u.CreationDateTime)),
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<IndexParameters> GetIndexParametersAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedSystemClient.ParametersAsync(cancellationToken).ConfigureAwait(false);
            return new IndexParameters(
                commentMaxLength: response.CommentMaxLength,
                videoDescriptionMaxLength: response.VideoDescriptionMaxLength,
                videoPersonalDataMaxLength: response.VideoPersonalDataMaxLength,
                videoTitleMaxLength: response.VideoTitleMaxLength);
        }

        public async Task<IndexUserInfo> GetUserInfoByAddressAsync(string address,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedUsersClient.UsersGetAsync(address, cancellationToken).ConfigureAwait(false);
            return new IndexUserInfo(
                id: response.Id,
                address: response.Address,
                creationDateTime: response.CreationDateTime);
        }

        public async Task<IndexedVideo> GetVideoByIdAsync(
            string videoId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedVideosClient.Find2Async(videoId, cancellationToken).ConfigureAwait(false);
            return await BuildIndexedVideoAsync(response).ConfigureAwait(false);
        }

        public async Task<IndexedVideo> GetVideoByManifestAsync(
            SwarmHash manifestHash,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedVideosClient.Manifest2Async(manifestHash.ToString(), cancellationToken).ConfigureAwait(false);
            return await BuildIndexedVideoAsync(response).ConfigureAwait(false);
        }

        public async Task<PaginatedResult<Comment>> GetVideoCommentsAsync(string videoId, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            var result = await generatedVideosClient.Comments3Async(videoId, page, take, cancellationToken).ConfigureAwait(false);
            return new PaginatedResult<Comment>(
                result.Elements.Select(c => new Comment(
                    id: c.Id,
                    creationDateTime: c.CreationDateTime,
                    isEditable: c.IsEditable,
                    isFrozen: c.IsFrozen,
                    ownerAddress: c.OwnerAddress,
                    textHistory: c.TextHistory,
                    videoId: c.VideoId)),
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<PaginatedResult<IndexedVideo>> GetVideosByOwnerAsync(string userAddress, int? page = null,
            int? take = null,
            CancellationToken cancellationToken = default)
        {
            var result = await generatedUsersClient.Videos3Async(userAddress, page, take, cancellationToken).ConfigureAwait(false);

            List<IndexedVideo> indexedVideos = [];
            foreach (var v in result.Elements)
                indexedVideos.Add(await BuildIndexedVideoAsync(v).ConfigureAwait(false));
            
            return new PaginatedResult<IndexedVideo>(
                indexedVideos,
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<IEnumerable<VideoValidationStatus>> GetVideoValidationStatusByIdAsync(string videoId, CancellationToken cancellationToken = default) =>
            (await generatedVideosClient.Validation2Async(videoId, cancellationToken).ConfigureAwait(false)).Select(s => new VideoValidationStatus(
                errorDetails: s.ErrorDetails.Select(e => new VideoValidationErrorDetail(
                    errorMessage: e.ErrorMessage,
                    errorType: Enum.Parse<VideoValidationErrorDetail.ErrorTypes>(e.ErrorType.ToString()))),
                hash: s.Hash,
                isValid: s.IsValid,
                validationTime: s.ValidationTime,
                videoId: s.VideoId));

        public async Task<VideoValidationStatus> GetVideoValidationStatusByManifestAsync(SwarmHash manifestHash,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedVideosClient.ValidationGet2Async(
                manifestHash.ToString(),
                cancellationToken).ConfigureAwait(false);
            return new VideoValidationStatus(
                errorDetails: response.ErrorDetails.Select(e => new VideoValidationErrorDetail(
                    errorMessage: e.ErrorMessage,
                    errorType: Enum.Parse<VideoValidationErrorDetail.ErrorTypes>(e.ErrorType.ToString()))),
                hash: response.Hash,
                isValid: response.IsValid,
                validationTime: response.ValidationTime,
                videoId: response.VideoId);
        }

        public Task ModerateCommentAsync(string commentId, CancellationToken cancellationToken = default) =>
            generatedModerationClient.CommentsAsync(commentId, cancellationToken);

        public Task ModerateVideoAsync(string videoId, CancellationToken cancellationToken = default) =>
            generatedModerationClient.VideosAsync(videoId, cancellationToken);

        public Task OwnerRemoveVideoAsync(string videoId, CancellationToken cancellationToken = default) =>
            generatedVideosClient.VideosDeleteAsync(videoId, cancellationToken);

        public Task<string> PublishNewVideoAsync(SwarmHash manifestHash, CancellationToken cancellationToken = default) =>
            generatedVideosClient.VideosPostAsync(new VideoCreateInput { ManifestHash = manifestHash.ToString() }, cancellationToken);

        public Task ReportUnsuitableVideoAsync(string videoId, SwarmHash manifestHash, string description, CancellationToken cancellationToken = default) =>
            generatedVideosClient.ReportsAsync(videoId, manifestHash.ToString(), description, cancellationToken);

        public Task<PaginatedResult<VideoPreview>> SearchVideosAsync(string? query = null, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOwnedVideoCommentAsync(string commentId, string newCommentText, CancellationToken cancellationToken = default) =>
            generatedVideosClient.CommentsPutAsync(commentId, newCommentText, cancellationToken);

        public Task UpdateVideoManifestAsync(string videoId, SwarmHash newManifestHash, CancellationToken cancellationToken = default) =>
            generatedVideosClient.Update2Async(videoId, newManifestHash.ToString(), cancellationToken);

        public Task VotesVideoAsync(string id, VoteValue value, CancellationToken cancellationToken = default) =>
            generatedVideosClient.VotesAsync(id, Enum.Parse<Value>(value.ToString()), cancellationToken);
        
        // Helpers.
        private async Task<IndexedVideo> BuildIndexedVideoAsync(Video2Dto videoDto)
        {
            List<VideoManifestImageSource> thumbnailSources = [];
            List<VideoManifestVideoSource> videoSources = [];
            if (videoDto.LastValidManifest is not null) //can be null, see: https://etherna.atlassian.net/browse/EID-229
            {
                //thumb sources
                foreach (var thumbnailSource in videoDto.LastValidManifest.Thumbnail.Sources)
                    thumbnailSources.Add(
                        await BuildVideoManifestImageSourceAsync(
                            thumbnailSource,
                            videoDto.LastValidManifest.Hash).ConfigureAwait(false));
                
                //video sources
                foreach (var videoSource in videoDto.LastValidManifest.Sources)
                    videoSources.Add(
                        await BuildVideoManifestVideoSourceAsync(
                            videoSource,
                            videoDto.LastValidManifest.Hash).ConfigureAwait(false));
            }
            
            return new IndexedVideo(
                id: videoDto.Id,
                creationDateTime: videoDto.CreationDateTime,
                currentVoteValue: videoDto.CurrentVoteValue.HasValue ?
                    Enum.Parse<VoteValue>(videoDto.CurrentVoteValue.Value.ToString()) : null,
                lastValidManifest: videoDto.LastValidManifest is not null ?
                    new PublishedVideoManifest(
                        hash: videoDto.LastValidManifest.Hash,
                        manifest: new(
                            aspectRatio: videoDto.LastValidManifest.AspectRatio,
                            batchId: videoDto.LastValidManifest.BatchId ?? PostageBatchId.Zero,
                            createdAt: DateTimeOffset.FromUnixTimeSeconds(videoDto.LastValidManifest.CreatedAt),
                            description: videoDto.LastValidManifest.Description ?? "",
                            duration: TimeSpan.FromSeconds(videoDto.LastValidManifest.Duration ?? 0),
                            title: videoDto.LastValidManifest.Title ?? "",
                            videoDto.OwnerAddress,
                            personalData: videoDto.LastValidManifest.PersonalData,
                            videoSources: videoSources,
                            thumbnail: new VideoManifestImage(
                                aspectRatio: videoDto.LastValidManifest.Thumbnail.AspectRatio,
                                blurhash: videoDto.LastValidManifest.Thumbnail.Blurhash,
                                sources: thumbnailSources),
                            updatedAt: videoDto.LastValidManifest.UpdatedAt is null ?
                                null :
                                DateTimeOffset.FromUnixTimeSeconds(videoDto.LastValidManifest.UpdatedAt.Value)),
                        manifestVersion: null) : null,
                ownerAddress: videoDto.OwnerAddress,
                totDownvotes: videoDto.TotDownvotes,
                totUpvotes: videoDto.TotUpvotes);
        }

        private async Task<VideoManifestVideoSource> BuildVideoManifestVideoSourceAsync(
            VideoSourceDto videoSourceDto,
            string videoManifestHashStr)
        {
            if (!Enum.TryParse<VideoType>(videoSourceDto.Type, true, out var videoType))
                videoType = VideoType.Mp4;

            var fileName = videoSourceDto.Path.Split(SwarmAddress.Separator).Last();
            if (!Path.HasExtension(fileName))
                fileName += videoType switch
                {
                    VideoType.Mp4 => ".mp4",
                    VideoType.Dash => ".mpd",
                    VideoType.Hls => ".m3u8",
                    _ => throw new InvalidOperationException()
                };
            
            var swarmUri = new SwarmUri(videoSourceDto.Path, UriKind.RelativeOrAbsolute);
            var hash = (await beeClient.ResolveAddressToChunkReferenceAsync(
                swarmUri.ToSwarmAddress(videoManifestHashStr)).ConfigureAwait(false)).Hash;

            return new(
                fileName,
                hash,
                videoType,
                videoSourceDto.Quality,
                videoSourceDto.Size);
        }

        private async Task<VideoManifestImageSource> BuildVideoManifestImageSourceAsync(
            ImageSourceDto imageSourceDto,
            string? videoManifestHashStr)
        {
            if (!Enum.TryParse<ImageType>(imageSourceDto.Type, true, out var imageType))
                imageType = ImageType.Jpeg; //only used jpeg at first

            var fileName = imageSourceDto.Path.Split(SwarmAddress.Separator).Last();
            if (!Path.HasExtension(fileName))
                fileName += imageType switch
                {
                    ImageType.Avif => ".avif",
                    ImageType.Jpeg => ".jpeg",
                    ImageType.Png => ".png",
                    ImageType.Webp => ".webp",
                    _ => throw new InvalidOperationException()
                };

            var hash = SwarmHash.Zero;
            var swarmUri = new SwarmUri(imageSourceDto.Path, UriKind.RelativeOrAbsolute);
            if (swarmUri.UriKind == UriKind.Absolute)
                hash = (await beeClient.ResolveAddressToChunkReferenceAsync(
                    swarmUri.ToSwarmAddress()).ConfigureAwait(false)).Hash;
            else if (videoManifestHashStr != null)
            {
                hash = (await beeClient.ResolveAddressToChunkReferenceAsync(
                    swarmUri.ToSwarmAddress(videoManifestHashStr)).ConfigureAwait(false)).Hash;
            }
            
            return new(
                fileName,
                imageType,
                hash,
                imageSourceDto.Width
            );
        }
    }
}
