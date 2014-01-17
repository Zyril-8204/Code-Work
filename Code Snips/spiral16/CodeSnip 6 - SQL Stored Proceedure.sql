USE [cci]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[SelectengageResults]
(
	@engageResultID UNIQUEIDENTIFIER = NULL,
	@engageCampaignID UNIQUEIDENTIFIER = NULL,
	@engageQueryID UNIQUEIDENTIFIER = NULL,
	@SiteTypeIDs dbo.IntList READONLY,
	@Top INT = 5,
	@SortOrder VARCHAR(4) = 'DESC',
	@SortColumn VARCHAR(15) = 'DatePosted',
	@Content VARCHAR(MAX) = NULL,
	@Viewed BIT = 0,
	@Favorite BIT = NULL,
	@StartDate DATETIME = '1/1/1900',
	@EndDate DATETIME = '12/31/9999',
	@Page INT = 1,
	@SparkUserID INT = NULL
)
AS
BEGIN
	
	SET @Page = (@Page - 1) * @Top
	
	DECLARE @STI TABLE (SiteTypeID INT NOT NULL)
	IF EXISTS (SELECT 1 FROM @SiteTypeIDs)
	BEGIN
		INSERT INTO @STI (SiteTypeID)
		SELECT Value FROM @SiteTypeIDs
	END
	ELSE
	BEGIN
		INSERT INTO @STI (SiteTypeID)
		SELECT SiteTypeID FROM SiteType
	END

	SELECT
		er.engageResultID engageResultID,
		a.AuthorID AuthorID,
		erq.engageQueryID engageQueryID,
		er.SiteTypeID SiteTypeID,
		er.DatePosted DatePosted,
		er.Latitude Latitude,
		er.Longitude Longitude,
		er.URL URL,
		eri.ContentBlock ContentBlock,
		eri.AuthorLink AuthorLink,
		eri.PostType PostType,
		eri.SocialAuthorID SocialAuthorID,
		eri.SocialID SocialID,
		erq.Viewed Viewed,
		a.Name AS AuthorName,
		a.Followers AS AuthorFollowers,
		a.Friends AS AuthorFriends,
		a.Klout AS AuthorKlout,
		a.PrefferedName AS AuthorPrefferedName,
		a.Avatar AS AuthorAvatar,
		erq.Favorite Favorite
	FROM engageResult er (NOLOCK)
	INNER JOIN engageResultInfo eri (NOLOCK) ON eri.engageResultID = er.engageResultID
	INNER JOIN engageResultAuthor_Xref era (NOLOCK) ON era.engageResultID = er.engageResultID
	INNER JOIN Author a (NOLOCK) ON era.AuthorID = a.AuthorID
	INNER JOIN engageResultengageQuery_Xref erq (NOLOCK) ON erq.engageResultID = er.engageResultID
	INNER JOIN engageQuery eq (NOLOCK) ON eq.engageQueryID = erq.engageQueryID
	INNER JOIN engageCampaign_SparkUser_XREF ecs (NOLOCK) ON ecs.engageCampaignID = eq.engageCampaignID 
	INNER JOIN @STI sti ON sti.SiteTypeID = er.SiteTypeID
	AND er.DatePosted BETWEEN @StartDate AND @EndDate	
	AND erq.engageResultID = ISNULL(@engageResultID, er.engageResultID)
	AND erq.Viewed = @Viewed
	AND erq.Favorite = ISNULL(@Favorite, erq.Favorite)
	AND eq.engageQueryID = ISNULL(@engageQueryID, eq.engageQueryID)
	AND eq.engageCampaignID = ISNULL(@engageCampaignID, eq.engageCampaignID)
	--AND eri.ContentBlock LIKE '%' + ISNULL(@Content, '') + '%' -- not being used, and it cause a big lull in the execution plan
	AND ecs.SparkUserID = ISNULL(@SparkUserID, ecs.SparkUserID)
	ORDER BY
		CASE WHEN @SortOrder = 'desc' THEN
			CASE
				WHEN @SortColumn = 'DatePosted' THEN DatePosted 
			END
		END DESC,
		CASE WHEN @SortOrder = 'asc' THEN
			CASE
				WHEN @SortColumn = 'DatePosted' THEN DatePosted 
			END
		END ASC,
		CASE WHEN @SortOrder = 'desc' THEN
			er.engageResultID
		END ASC,
		CASE WHEN @SortOrder = 'asc' THEN
			er.engageResultID
		END DESC
	OFFSET @Page  ROWS Fetch NEXT @Top ROWS ONLY;

END

