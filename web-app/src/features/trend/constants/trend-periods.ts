import { TrendPeriod } from "../api/trend.schemas";

/** Periods of the trend: the value of the `period` URL parameter, the API period and the label key. */
export const TREND_PERIODS = [
	{ param: "6m", period: TrendPeriod.SixMonths, labelKey: "six_months" },
	{ param: "12m", period: TrendPeriod.TwelveMonths, labelKey: "twelve_months" },
	{ param: "all", period: TrendPeriod.All, labelKey: "all" },
] as const;
