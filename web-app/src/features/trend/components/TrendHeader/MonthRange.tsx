import type { TrendPeriod } from "../../api/trend.schemas";
import { getTrendPoints } from "../../get-trend-points";
import { formatMonthRange } from "../../helpers/format-month-range/format-month-range";

interface MonthRangeProps {
	language: string;
	period: Promise<TrendPeriod>;
}

/** First and last month of the trend. */
const MonthRange = async ({ language, period }: Readonly<MonthRangeProps>) => {
	const result = await getTrendPoints(language, await period);

	if ("error" in result) return null;

	const [first] = result.points;
	const last = result.points.at(-1);

	return first && last ? formatMonthRange(first, last, language) : null;
};

export default MonthRange;
