import { TrendPeriod } from "../../api/trend.schemas";
import { TREND_PERIODS } from "../../constants/trend-periods";

/** Period of the `period` URL parameter; the last 6 months when it is missing or unknown. */
export const parsePeriodParam = (param: string | string[] | undefined) =>
	TREND_PERIODS.find((item) => item.param === param)?.period ??
	TrendPeriod.SixMonths;
