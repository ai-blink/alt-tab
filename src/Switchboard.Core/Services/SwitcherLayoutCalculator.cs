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
    public const double MinimumLayoutWidth = 875;
    private const double MinLayoutHeight = 500;

    public static SwitcherLayout Calculate(
        int windowCount,
        double workAreaWidth,
        double workAreaHeight,
        double appScale,
        SwitcherViewMode mode,
        SwitcherSizingPolicy sizingPolicy,
        double cardWidth,
        double cardHeight)
    {
        windowCount = Math.Max(1, windowCount);
        appScale = appScale > 0 ? appScale : 1;

        var logicalScreenWidth = ScaleWorkAreaToLogicalSize(workAreaWidth, appScale);
        var logicalScreenHeight = ScaleWorkAreaToLogicalSize(workAreaHeight, appScale);
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
        var columns = mode == SwitcherViewMode.List
            ? Math.Clamp(2, 1, Math.Min(windowCount, maxColumns))
            : ChooseBestColumns(
                windowCount,
                maxColumns,
                itemWidth,
                cardHeight,
                detailsHeaderHeight,
                logicalScreenWidth,
                logicalScreenHeight,
                sizingPolicy);
        var rows = (int)Math.Ceiling(windowCount / (double)columns);
        var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
        var desiredHeight = CalculateDesiredHeight(windowCount, columns, cardHeight, detailsHeaderHeight);

        var layoutWidth = Math.Min(logicalScreenWidth, Math.Max(MinimumLayoutWidth, desiredWidth));
        var layoutHeight = Math.Min(logicalScreenHeight, Math.Max(MinLayoutHeight, desiredHeight));

        return new SwitcherLayout(
            columns,
            rows,
            layoutWidth,
            layoutHeight,
            desiredHeight > layoutHeight);
    }

    public static double CalculateMinimumLayoutHeight(SwitcherViewMode mode, double cardHeight)
    {
        var detailsHeaderHeight = mode == SwitcherViewMode.List ? ListDetailsHeaderHeight : 0;
        return CalculateDesiredHeight(windowCount: 1, columns: 1, cardHeight, detailsHeaderHeight);
    }

    public static double ScaleLayoutDimension(double logicalDimension, double appScale)
    {
        appScale = appScale > 0 ? appScale : 1;
        var fixedMargin = OuterMargin * 2;
        return fixedMargin + (Math.Max(0, logicalDimension - fixedMargin) * appScale);
    }

    private static int ChooseBestColumns(
        int windowCount,
        int maxColumns,
        double itemWidth,
        double cardHeight,
        double detailsHeaderHeight,
        double screenWidth,
        double screenHeight,
        SwitcherSizingPolicy sizingPolicy)
    {
        var columnLimit = Math.Max(1, Math.Min(windowCount, maxColumns));
        var best = new LayoutCandidate(1, double.MaxValue);

        for (var columns = 1; columns <= columnLimit; columns++)
        {
            var rows = (int)Math.Ceiling(windowCount / (double)columns);
            var emptySlots = (rows * columns) - windowCount;
            var desiredWidth = (OuterMargin * 2) + ContentHorizontalMargin + (columns * itemWidth) + LayoutSafetyPadding;
            var desiredHeight = CalculateDesiredHeight(windowCount, columns, cardHeight, detailsHeaderHeight);
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
        int windowCount,
        int columns,
        double cardHeight,
        double detailsHeaderHeight)
    {
        var rows = (int)Math.Ceiling(windowCount / (double)columns);
        return (OuterMargin * 2) +
            HeaderHeight +
            FooterHeight +
            ContentVerticalMargin +
            detailsHeaderHeight +
            (rows * (cardHeight + ItemVerticalGap + SelectionFramePadding)) +
            LayoutSafetyPadding;
    }

    private static double ScaleWorkAreaToLogicalSize(double workAreaSize, double appScale)
    {
        var fixedMargin = OuterMargin * 2;
        return fixedMargin + (Math.Max(1, workAreaSize - fixedMargin) / appScale);
    }

    private readonly record struct LayoutCandidate(int Columns, double Score);
}

public readonly record struct SwitcherLayout(
    int Columns,
    int Rows,
    double Width,
    double Height,
    bool RequiresVerticalScroll);
