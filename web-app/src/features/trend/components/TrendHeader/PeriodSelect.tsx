import { getI18n } from "@shared/helpers";
import type { TrendPeriod } from "../../api/trend.schemas";
import { TREND_PERIODS } from "../../constants/trend-periods";
import { TREND_I18N } from "../../trend-i18n";
import PeriodSwitcher from "./PeriodSwitcher";

interface PeriodSelectProps {
	language: string;
	period: Promise<TrendPeriod>;
}

/** Period switcher set to the period of the URL. */
const PeriodSelect = async ({
	language,
	period,
}: Readonly<PeriodSelectProps>) => {
	const t = getI18n(language, TREND_I18N);
	const selected = await period;

	return (
		<PeriodSwitcher
			label={t("period")}
			selected={
				TREND_PERIODS.find((item) => item.period === selected)?.param ??
				TREND_PERIODS[0].param
			}
			options={TREND_PERIODS.map((item) => ({
				param: item.param,
				label: t(item.labelKey),
			}))}
		/>
	);
};

export default PeriodSelect;
