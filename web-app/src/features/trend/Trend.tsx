import { getI18n } from "@shared/helpers";
import { TREND_I18N } from "./trend-i18n";

interface TrendProps {
	language: string;
}

/** Trend page: the index and its subsystems over the months. */
const Trend = ({ language }: Readonly<TrendProps>) => {
	const t = getI18n(language, TREND_I18N);

	return <h1>{t("title")}</h1>;
};

export default Trend;
