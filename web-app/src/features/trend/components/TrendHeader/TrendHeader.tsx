import { Heading, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import type { TrendPeriod } from "../../api/trend.schemas";
import { TREND_I18N } from "../../trend-i18n";
import MonthRange from "./MonthRange";
import PeriodSelect from "./PeriodSelect";

interface TrendHeaderProps {
	language: string;
	period: Promise<TrendPeriod>;
}

/** Title of the trend with its months and the period switcher. */
const TrendHeader = ({ language, period }: Readonly<TrendHeaderProps>) => {
	const t = getI18n(language, TREND_I18N);

	return (
		<div className="flex flex-wrap items-end justify-between gap-3">
			<div className="grid gap-0.5">
				<Text variant="eyebrow" className="min-h-4">
					<Suspense
						fallback={
							<span
								aria-hidden
								className="block h-4 w-40 rounded-sm bg-skeleton"
							/>
						}
					>
						<MonthRange language={language} period={period} />
					</Suspense>
				</Text>
				<Heading level={1}>{t("title")}</Heading>
			</div>

			<Suspense
				fallback={
					<span
						aria-hidden
						className="block h-10.5 w-52 rounded-md bg-skeleton"
					/>
				}
			>
				<PeriodSelect language={language} period={period} />
			</Suspense>
		</div>
	);
};

export default TrendHeader;
