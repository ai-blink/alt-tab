using Switchboard.Core.Models;

namespace Switchboard.Core.Services;

public static class SwitcherLayoutCalculator
{
    private const double OuterMargin = 18;
    private const double ContentHorizontalMargin = 28;
    private const double ContentVerticalMargin = 20;
    private const double HeaderHeight = 52;
    private const double FooterHeight = 30;
    private const double ListDetailsHeaderHeight = 30;
    private const double ItemHorizontalGap = 6;
    private const double ItemVerticalGap = 8;
    private const double SelectionFramePadding = 8;
    private const double LayoutSafetyPadding = 24;
    private const double MinLayoutWidth = 875;
    private const double MinLayoutHeight = 500;

    public static SwitcherLayout Calculate(
        int windowCount,
        double workAreaWidth,
        double workAreaHeight,
        double uiScale,
        SwitcherViewMode mode,
        SwitcherSizingPolicy sizingPolicy,
        double cardWidth,
        double cardHeight,
        int? maximumColumns = null,
        int? maximumRows = null)
    {
        windowCount = Math.Max(1, windowCount);
        uiScale = uiScale > 0 ? uiScale : 1;

        var logicalScreenWidth = ScaleWorkAreaToLogicalSize(workAreaWidth, uiScale);
        var logicalScreenHeight = ScaleWorkAreaToLogicalSize(workAreaHeight, uiScale);
        var detailsHeaderHeight = mode == SwitcherViewMode.List ? ListDetailsHeaderHeight : 0;
        var itemWidth = cardWidth + ItemHorizontalGap + SelectionFramePadding;
        var availableColumns = Math.Max(
            1,
            (int)Math.Floor(
                (logicalScreenWidth - (OuterMargin * 2) - ContentHorizontalMargin - LayoutSafetyPadding) /
                itemWidth));
        var maxColumns = mode == SwitcherViewMode.List
            ? Math.Min(2, availableColumns)
            : availableColumns;
        maxColumns = maximumColumns is > 0
            ? Math.Min(maxColumns, maximumColumns.Value)
            : maxColumns;
        var columns = mode switch
        {
            SwitcherViewMode.List => Math.Clamp(2, 1, Math.Min(windowCount, maxColumns)),
            SwitcherViewMode.Compact when maximumColumns is > 0 =>
                Math.Min(windowCount, maxColumns),
            _ => ChooseBestColumns(
                windowCount,
                maxColumns,
                itemWidth,
                cardHeight,
                detailsHeaderHeight,
                logicalScreenWidth,
                logicalScreenHeight,
                sizingPolicy,
                maximumRows)
        };
        var rows = (int)Math.Ceiling(windowCount / (double)columns);
        var visibleRows = maximumRows is > 0
            ? Math.Min(rows, maximumRows.Value)
            : rows;
        var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
        var desiredHeight = CalculateDesiredHeight(visibleRows, cardHeight, detailsHeaderHeight);

        return new SwitcherLayout(
            columns,
            rows,
            Math.Min(logicalScreenWidth, Math.Max(MinLayoutWidth, desiredWidth)),
            Math.Min(logicalScreenHeight, Math.Max(MinLayoutHeight, desiredHeight)),
            rows > visibleRows || desiredHeight > logicalScreenHeight);
    }

    private static int ChooseBestColumns(
        int windowCount,
        int maxColumns,
        double itemWidth,
        double cardHeight,
        double detailsHeaderHeight,
        double screenWidth,
        double screenHeight,
        SwitcherSizingPolicy sizingPolicy,
        int? maximumRows)
    {
        var columnLimit = Math.Max(1, Math.Min(windowCount, maxColumns));
        var best = new LayoutCandidate(1, double.MaxValue);

        for (var columns = 1; columns <= columnLimit; columns++)
        {
            var rows = (int)Math.Ceiling(windowCount / (double)columns);
            var emptySlots = (rows * columns) - windowCount;
            var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
            var visibleRows = maximumRows is > 0
                ? Math.Min(rows, maximumRows.Value)
                : rows;
            var desiredHeight = CalculateDesiredHeight(visibleRows, cardHeight, detailsHeaderHeight);
            var widthOverflow = Math.Max(0, desiredWidth - screenWidth);
            var heightOverflow = Math.Max(0, desiredHeight - screenHeight);
            var rightBlankRatio = Math.Max(0, screenWidth - desiredWidth) / screenWidth;
            var rowColumnBalance = Math.Abs(rows - columns);
            var denseWeight = sizingPolicy == SwitcherSizingPolicy.Dense ? 1.0 : 0.0;
            var score =
                (emptySlots * (55 - (denseWeight * 25))) +
                (rightBlankRatio * (70 + (denseWeight * 45))) +
                (rowColumnBalance * (3 + (denseWeight * 2))) +
                (rows * 1.8) +
                (widthOverflow * 100) +
                (heightOverflow * 100);

            if (score < best.Score)
            {
                best = new LayoutCandidate(columns, score);
            }
        }

        return best.Columns;
    }

    private static double CalculateDesiredHeight(
        int visibleRows,
        double cardHeight,
        double detailsHeaderHeight)
    {
        return (OuterMargin * 2) +
            HeaderHeight +
            FooterHeight +
            ContentVerticalMargin +
            detailsHeaderHeight +
            (visibleRows * (cardHeight + ItemVerticalGap + SelectionFramePadding)) +
            LayoutSafetyPadding;
    }

    private static double ScaleWorkAreaToLogicalSize(double workAreaSize, double uiScale)
    {
        var fixedMargin = OuterMargin * 2;
        return fixedMargin + (Math.Max(1, workAreaSize - fixedMargin) / uiScale);
    }

    private readonly record struct LayoutCandidate(int Columns, double Score);
}

public readonly record struct SwitcherLayout(
    int Columns,
    int Rows,
    double Width,
    double Height,
    bool RequiresVerticalScroll);
