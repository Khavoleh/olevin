import { Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { TREND_I18N } from "./trend-i18n";

interface TrendProps {
	language: string;
}

/** Trend page: the index and its subsystems over the months. */
const Trend = ({ language }: Readonly<TrendProps>) => {
	const t = getI18n(language, TREND_I18N);

	return <Heading level={1}>{t("title")}</Heading>;
};

export default Trend;
