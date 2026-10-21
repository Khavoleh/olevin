import {
	CaretDownIcon,
	CaretUpIcon,
	type Icon,
	MinusIcon,
} from "@phosphor-icons/react";
import { getI18n } from "@shared/helpers";
import type { IndexChangeResponse } from "../../api/home.schemas";
import { formatIndex } from "../../helpers/format-index/format-index";
import { formatSinceMonth } from "../../helpers/format-since-month/format-since-month";
import { INDEX_CARD_I18N } from "../../i18n/index-card-i18n";

interface IndexChangeProps {
	change: IndexChangeResponse;
	language: string;
}

interface Direction {
	Icon: Icon;
	className: string;
	key: string;
}

const DIRECTIONS: Record<number, Direction> = {
	1: { Icon: CaretUpIcon, className: "text-positive", key: "rose" },
	[-1]: { Icon: CaretDownIcon, className: "text-negative", key: "fell" },
	0: { Icon: MinusIcon, className: "text-fg-muted", key: "unchanged" },
};

/** Change of the index since the previous snapshot, as "3.6 since August" with an arrow. */
const IndexChange = ({ change, language }: Readonly<IndexChangeProps>) => {
	const t = getI18n(language, INDEX_CARD_I18N);
	const { Icon, className, key } = DIRECTIONS[Math.sign(change.delta)];

	return (
		<span
			className={`flex items-center gap-1 font-semibold text-sm tabular-nums ${className}`}
		>
			<Icon size={14} weight="fill" />
			<span>
				<span className="sr-only">{t(key)} </span>
				{formatIndex(Math.abs(change.delta), language)} {t("since")}{" "}
				{formatSinceMonth(change.year, change.month, language)}
			</span>
		</span>
	);
};

export default IndexChange;
