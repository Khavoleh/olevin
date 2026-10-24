import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import TrendCard from "./components/TrendCard";
import TrendCharts from "./components/TrendCharts";
import TrendHeader from "./components/TrendHeader/TrendHeader";
import { TREND_METRICS } from "./constants/trend-metrics";
import { parsePeriodParam } from "./helpers/parse-period-param/parse-period-param";
import { TREND_I18N } from "./trend-i18n";

interface TrendProps {
	language: string;
	searchParams: Promise<{ period?: string | string[] }>;
}

/** Trend page: the index, cash flow and buffer month by month. */
const Trend = ({ language, searchParams }: Readonly<TrendProps>) => {
	const t = getI18n(language, TREND_I18N);
	const period = searchParams.then((params) => parsePeriodParam(params.period));

	return (
		<>
			<TrendHeader language={language} period={period} />

			<div className="grid gap-3 md:gap-4">
				<Suspense
					fallback={TREND_METRICS.map((item) => (
						<TrendCard
							key={item.metric}
							title={t(item.titleKey)}
							caption={t(item.captionKey)}
						>
							<div
								aria-hidden
								className="h-64 rounded-lg bg-skeleton md:h-76"
							/>
						</TrendCard>
					))}
				>
					<TrendCharts language={language} period={period} />
				</Suspense>
			</div>
		</>
	);
};

export default Trend;
