import { Card, ErrorToast, Text } from "@shared/components";
import { formatIndex, formatShortMonth, getI18n } from "@shared/helpers";
import { TrendPeriod } from "../api/trend.schemas";
import { TREND_METRICS } from "../constants/trend-metrics";
import { getTrendPoints } from "../get-trend-points";
import { TREND_I18N } from "../trend-i18n";
import TrendCard from "./TrendCard";
import TrendChart from "./TrendChart/TrendChart";

interface TrendChartsProps {
	language: string;
	period: Promise<TrendPeriod>;
}

const EMPTY_KEYS: Record<TrendPeriod, string> = {
	[TrendPeriod.SixMonths]: "empty_six_months",
	[TrendPeriod.TwelveMonths]: "empty_twelve_months",
	[TrendPeriod.All]: "empty_all",
};

/** Charts of the index, cash flow and buffer, or a note when the period has no snapshot. */
const TrendCharts = async ({
	language,
	period,
}: Readonly<TrendChartsProps>) => {
	const t = getI18n(language, TREND_I18N);
	const selected = await period;
	const result = await getTrendPoints(language, selected);

	if ("error" in result) return <ErrorToast message={result.error} />;

	const { points } = result;

	if (points.length === 0) {
		return (
			<Card>
				<div className="rounded-lg bg-surface-sunken p-3">
					<Text variant="hint">{t(EMPTY_KEYS[selected])}</Text>
				</div>
			</Card>
		);
	}

	const snapshots = points.filter((point) => point.index !== null).length;

	return (
		<>
			{snapshots === 1 ? (
				<div className="rounded-lg bg-surface-sunken p-3">
					<Text variant="hint">{t("one_snapshot")}</Text>
				</div>
			) : null}

			{TREND_METRICS.map((item) => (
				<TrendCard
					key={item.metric}
					title={t(item.titleKey)}
					caption={t(item.captionKey)}
				>
					<TrendChart
						title={t(item.titleKey)}
						points={points.map((point) => {
							const value = point[item.metric];

							return {
								month: formatShortMonth(point.year, point.month, language),
								year: point.year,
								value,
								text: value === null ? "" : formatIndex(value, language),
							};
						})}
						ticks={[...item.ticks]}
						terms={item.terms.map((term) => ({
							value: term.value,
							label: t(term.labelKey),
						}))}
						hasZones={item.hasZones}
					/>
				</TrendCard>
			))}
		</>
	);
};

export default TrendCharts;
